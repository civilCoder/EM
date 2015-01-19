using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public struct resultsRL
    {
        public string opt;
        public double val;

        public resultsRL(string o, double v)
        {
            opt = o;
            val = v;
        }
    }

    public static class cmdRL
    {
        public static void
        getRLpromptresults(out resultsRL resRL, out bool escape)
        {
            escape = true;
            resRL = new resultsRL { opt = "R", val = 0.0 };
            try
            {
                string prompt = string.Format("\nZ value difference / Rate of grade <{0}>:[Z/R]", resRL.opt);
                escape = UserInput.getUserInputKeyword(resRL.opt.ToUpper(), out resRL.opt, prompt, "Z R");
                if (escape)
                    return;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRL.cs: line: 40");
            }

            if (resRL.opt == string.Empty)
                return;

            switch (resRL.opt.ToUpper())
            {
                case "R":
                    resRL.val = Pub.Slope;
                    escape = UserInput.getUserInput("\nRate of Grade: ", out resRL.val, resRL.val);
                    if (escape)
                        return;
                    Pub.Slope = resRL.val;
                    break;

                case "Z":
                    resRL.val = Pub.dZ;
                    escape = UserInput.getUserInput("\nZ Value Difference", out resRL.val, resRL.val);
                    if (escape)
                        return;
                    Pub.dZ = resRL.val;
                    break;
            }
        }

        public static void
        RL(string nameApp, string nameCmd)
        {
            ObjectId idPoly = ObjectId.Null;
            bool escape = false;

            List<ObjectId> idPnts = new List<ObjectId>();
            ObjectId idCogoPntBASE = CgPnt.selectCogoPointByNode("\nSelect Base Point: ", osMode: 8);
            if (idCogoPntBASE == ObjectId.Null)
            {
                Application.ShowAlertDialog("CogoPoint not found.  Exiting......");
                return;
            }

            BaseObjs.updateGraphics();
            Point3d pnt3dBASE = idCogoPntBASE.getCogoPntCoordinates();

            idPnts.Add(idCogoPntBASE);

            string pntDesc = idCogoPntBASE.getCogoPntDesc();
            if (pntDesc == "")
                pntDesc = "CPNT-ON";

            string prompt = "\nLocate New Point: ";
            PromptStatus ps;
            Point3d pnt3dTAR = UserInput.getPoint(prompt, pnt3dBASE, out escape, out ps, osMode: 641);
            if (escape || pnt3dTAR == Pub.pnt3dO)
            {
                return;
            }

            double distance = 0.0;
            double DeltaZ = 0.0;
            double grade = 0.0;

            resultsRL resRL; 
            getRLpromptresults(out resRL, out escape);
            if (escape)
                return;

            switch (resRL.opt)
            {
                case "R":
                    grade = resRL.val;
                    distance = pnt3dBASE.getDistance(pnt3dTAR);
                    pnt3dTAR = new Point3d(pnt3dTAR.X, pnt3dTAR.Y, pnt3dBASE.Z + (grade * distance));
                    break;

                case "Z":
                    DeltaZ = resRL.val;
                    pnt3dTAR = new Point3d(pnt3dTAR.X, pnt3dTAR.Y, pnt3dBASE.Z + DeltaZ);
                    break;
            }

            uint pntNum;
            ObjectId idCogoPntTAR = pnt3dTAR.setPoint(out pntNum, pntDesc);
            ObjectId idPoly3d = ObjectId.Null;

            if (nameCmd == "cmdRL")
            {
                idPnts.Add(idCogoPntTAR);

                List<Handle> hPnts = new List<Handle>();
                hPnts.Add(idPnts[0].getHandle());
                hPnts.Add(idPnts[1].getHandle());

                using (BaseObjs._acadDoc.LockDocument())
                {
                    idPoly3d = BrkLine.makeBreakline(nameApp, nameCmd, out idPoly, idPnts);
                }
            }

            Grading_Palette.gPalette.pGrading.cmdRL_Default = resRL.opt;
            Grading_Palette.gPalette.pGrading.cmdRL_GRADE = grade.ToString();
            Grading_Palette.gPalette.pGrading.cmdRL_DELTAZ = DeltaZ.ToString();

            Dict.setCmdDefault("cmdRL", "cmdDefault", resRL.opt);
            Dict.setCmdDefault("cmdRL", "GRADE", grade.ToString());
            Dict.setCmdDefault("cmdRL", "DELTAZ", DeltaZ.ToString());

            bool exists = false;
            PointGroup pntGroup = CgPnt_Group.addPntGroup(pntDesc, out exists);
            ObjectId idPntLabelStyle = Pnt_Style.getPntLabelStyle(CgPnts.setup(pntDesc));

            if (!exists)
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        pntGroup.UpgradeOpen();
                        pntGroup.PointLabelStyleId = idPntLabelStyle;

                        StandardPointGroupQuery query = new StandardPointGroupQuery();
                        query.IncludeRawDescriptions = pntDesc;
                        pntGroup.SetQuery(query);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdRL.cs: line: 167");
                }
            }
        }
    }
}
