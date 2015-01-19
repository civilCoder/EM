using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45.C3D;

using System.Globalization;

namespace Base_Tools45
{
    public static class Grading_Floor
    {
        public static void
        AVG(ObjectId idPoly, Point3d pnt3dCEN, double dblSlope, double dblPadElev, bool boolSetPointAtCenter, Point3d pnt3dTAR)
        {
            ObjectIdCollection idsPoly3d = new ObjectIdCollection();

            double dblDist = 0.0;
            uint pntNum;
            ObjectId idPoly3d = ObjectId.Null;

            switch (boolSetPointAtCenter)
            {
                case true:
                    pnt3dCEN.setPoint(out pntNum, "CPNT-ON");
                    break;

                case false:

                    pnt3dCEN = new Point3d(pnt3dCEN.X, pnt3dCEN.Y, dblPadElev);

                    if (pnt3dTAR != Pub.pnt3dO)
                    {
                        dblDist = pnt3dCEN.getDistance(pnt3dTAR);
                    }

                    using (BaseObjs._acadDoc.LockDocument())
                    {
                        pnt3dCEN.setPoint(out pntNum, "CPNT-ON");

                        if (pnt3dTAR != Pub.pnt3dO)
                        {
                            pnt3dTAR = new Point3d(pnt3dTAR.X, pnt3dTAR.Y, pnt3dCEN.Z + dblDist * dblSlope);
                            pnt3dTAR.setPoint(out pntNum, "CPNT-ON");
                        }

                        idsPoly3d = DrawFloorSlab.build3dPolyFloorSlab(idPoly, pnt3dCEN, pnt3dTAR, dblSlope, "CPNT-ON", "CPNT-BRKLINE", "AVG");

                        int numBldg = updateDictGRADEDOCK(idsPoly3d[0].getHandle(), dblSlope, pnt3dCEN, pnt3dTAR);

                        modSurface("CPNT-ON", "Finish Surface", idsPoly3d, true);

                        TypedValue[] tvs = new TypedValue[3];
                        tvs.SetValue(new TypedValue(1001, "AVG"), 0);
                        tvs.SetValue(new TypedValue(1000, "BLDG"), 1);
                        tvs.SetValue(new TypedValue(1070, numBldg), 2);       //Number assigned; to building

                        foreach(ObjectId id in idsPoly3d){
                            id.setXData(tvs, "AVG");
                        }
                    }
                    break;
            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                BaseObjs.regen();
            }
        }

        public static double
        getAverageElev(ObjectId idPoly, bool boolShowPoints, string strSurface)
        {
            double dblZ_Total = 0;
            double dblZ_AVG = 0;

            bool exists = false;
            ObjectId idSurfaceEXIST = Surf.getSurface(strSurface, out exists);
            TinSurface surfaceEXIST = (TinSurface)idSurfaceEXIST.getEnt();

            if (surfaceEXIST.GetGeneralProperties().MinimumElevation <= 0.0)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("Check Surface EXIST elevations: Minimum elevation: {0}", surfaceEXIST.GetGeneralProperties().MinimumElevation));
                return 0;
            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                if (!idPoly.isRightHand())
                {
                    idPoly.reversePolyX();
                }
                idPoly.checkIfClosed();
            }

            Point3dCollection pntsGrid = Misc.getBldgLimitsAVG(idPoly, 20);
            Point3dCollection pntsGridElev = new Point3dCollection();

            foreach (Point3d pnt3d in pntsGrid)
            {
                try
                {
                    double dblZ = surfaceEXIST.FindElevationAtXY(pnt3d.X, pnt3d.Y);
                    dblZ_Total = dblZ_Total + dblZ;

                    if (boolShowPoints)
                    {
                        pntsGridElev.Add(new Point3d(pnt3d.X, pnt3d.Y, dblZ));
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " Grading_Floor.cs: line: 115");
                }
            }

            if (boolShowPoints)
            {
                uint pntNum;

                using (BaseObjs._acadDoc.LockDocument())
                {
                    foreach (Point3d pnt3d in pntsGridElev)
                        pnt3d.setPoint(out pntNum, "CPNT-ON");
                }
            }

            dblZ_AVG = dblZ_Total / (pntsGrid.Count);

            return dblZ_AVG;
        }

        public static Point3d
        getPoint(string strPrompt, Point3d pnt3d, out bool escape)
        {
            escape = true;
            Point3d pnt3dX = Pub.pnt3dO;
            Editor ED = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointResult PPR = null;

            PromptPointOptions PPO = new PromptPointOptions(strPrompt);
            PPO.AllowNone = true;
            if (pnt3d != Pub.pnt3dO)
            {
                PPO.UseBasePoint = true;
                PPO.BasePoint = pnt3d;
                PPO.UseDashedLine = true;
            }

            PPR = ED.GetPoint(PPO);
            switch (PPR.Status)
            {
                case PromptStatus.Cancel:
                case PromptStatus.Other:
                case PromptStatus.None:
                    break;

                case PromptStatus.OK:
                    pnt3dX = PPR.Value;
                    escape = false;
                    break;
            }
            return pnt3dX;
        }

        public static void
        modSurface(string nameSurface, string descSurface, ObjectIdCollection idsPoly3d, bool boolNewSurface)
        {
            bool exists = false;
            TinSurface objSurface = Surf.getTinSurface(nameSurface, out exists);
            if (!exists)
            {
                objSurface.StyleId = Surf_Styles.getSurfaceStyle(nameSurface);
            }

            objSurface.BreaklinesDefinition.AddStandardBreaklines(idsPoly3d, 1, 0, 0, 0);
            objSurface.BuildOptions.CrossingBreaklinesElevationOption = Autodesk.Civil.CrossingBreaklinesElevationType.UseAverage;
            objSurface.Rebuild();
        }

        public static int
        updateDictGRADEDOCK(Handle HANDLE3D, double dblSlope, Point3d pnt3dBase0, Point3d pnt3dBase1)
        {
            ResultBuffer rb = null;
            TypedValue[] tvs;
            int numBldgs = 0;
            int numBldgCurr = 0;
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        bool exists = false;
                        Dict.removeNamedDictionary("GRADEDOCK");
                        ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);
                        if (exists)
                        {
                            rb = Dict.getXRec(idDictGRADEDOCK, "BLDGCOUNT");
                            tvs = rb.AsArray();
                            numBldgs = int.Parse(tvs[0].Value.ToString());
                            rb = new ResultBuffer();
                            rb.Add(new TypedValue(1070, numBldgs + 1));
                            Dict.addXRec(idDictGRADEDOCK, "BLDGCOUNT", rb);
                        }
                        else
                        {
                            rb = new ResultBuffer();
                            rb.Add(new TypedValue(1070, 1));
                            Dict.addXRec(idDictGRADEDOCK, "BLDGCOUNT", rb);
                        }

                        numBldgCurr = numBldgs + 1;
                        ObjectId idDictBLDG = Dict.addSubDict(idDictGRADEDOCK, numBldgCurr.ToString(CultureInfo.InvariantCulture));

                        rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.Handle, HANDLE3D));
                        Dict.addXRec(idDictBLDG, "HANDLE3D", rb);

                        rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.Real, dblSlope));
                        Dict.addXRec(idDictBLDG, "SLOPE", rb);

                        rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase0.X));
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase0.Y));
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase0.Z));
                        Dict.addXRec(idDictBLDG, "CENtrOID", rb);

                        rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase1.X));
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase1.Y));
                        rb.Add(new TypedValue((int)DxfCode.Real, pnt3dBase1.Z));
                        Dict.addXRec(idDictBLDG, "TARGET", rb);

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " Grading_Floor.cs: line: 243");
                }
            }
            return numBldgCurr;
        }
    }
}
