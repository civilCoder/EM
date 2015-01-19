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
    public struct resultsRTd
    {
        public string opt1;
        public string opt2;
        public double valD;
        public double valE;
        public double valZ;
        public double valS;

        public resultsRTd(string o1, string o2, double vD, double vE, double vZ, double vS)
        {
            opt1 = o1;
            opt2 = o2;
            valD = vD;
            valE = vE;
            valZ = vZ;
            valS = vS;
        }
    }
    public static class cmdRTD
    {
        public static void
        RTD(string nameApp)
        {            
            resultsRTd resRTd = new resultsRTd { opt1 = "D", opt2 = "R", valD = 0.0, valE = 0.0, valZ = 0.0, valS = 0.0};
            
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

            string prompt = "\nPick Point for Direction: ";

            PromptStatus ps;
            Point3d pnt3dTAR = UserInput.getPoint(prompt, pnt3dBASE, out escape, out ps, osMode: 641);
            if (escape || pnt3dTAR == Pub.pnt3dO)
            {
                return;
            }

            double angle = 0;

            resRTd.opt1 = Dict.getCmdDefault("cmdRTd", "cmdDefault");

            if (resRTd.opt1 == string.Empty)
                resRTd.opt1 = "D";

            angle = Measure.getAzRadians(pnt3dBASE, pnt3dTAR);

            try
            {
                prompt = string.Format("\nDistance / target Elevation / Z value difference <{0}>: [D/E/Z]: ", resRTd.opt1);
                escape = UserInput.getUserInputKeyword(resRTd.opt1, out resRTd.opt1, prompt, "D E Z");
                if (escape)
                    return;
                if (resRTd.opt1 != "D" && resRTd.opt1 != "E" && resRTd.opt1 != "Z")
                    return;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRTD.cs: line: 88");
            }

            if (resRTd.opt1 == string.Empty)
                return;
            bool tryParse = false;
            switch (resRTd.opt1)
            {
                case "D":
                    resRTd.valD = Pub.Dist;
                    if(resRTd.valD == 0)
                        tryParse = double.TryParse(Dict.getCmdDefault("cmdRTD", "Distance"), out resRTd.valD);

                    if (!tryParse)
                        resRTd.valD = 0.0;;

                    try
                    {
                        escape = UserInput.getUserInput("\nEnter Distance [pos(+) value = target direction, neg(-) value = target direction + 180 degrees:", out resRTd.valD, resRTd.valD);
                        if (escape)
                            return;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdRTD.cs: line: 112");
                    }

                    Pub.Dist = resRTd.valD;

                    resRTd.valS = Pub.Slope;
                    if (resRTd.valS == 0.0)
                        tryParse = double.TryParse(Dict.getCmdDefault("cmdRTd", "Slope"), out resRTd.valS);

                    escape = UserInput.getUserInput("\nRate of Grade: ", out resRTd.valS, resRTd.valS);
                    if (escape)
                        return;

                    Pub.Slope = resRTd.valS;                           

                    break;

                case "E":
                    resRTd.valE = Pub.Elev;
                    if(resRTd.valE == 0)
                        resRTd.valE = double.Parse(Dict.getCmdDefault("cmdRTd", "Elevation"));
                    
                    PromptDoubleOptions pdo = new PromptDoubleOptions("\nEnter Target Elevation / ESC to select point for Elevation: ");

                    pdo.AllowArbitraryInput = true;
                    pdo.AllowNone = true;
                    pdo.UseDefaultValue = true;
                    pdo.DefaultValue = resRTd.valE;

                    PromptDoubleResult pdr = BaseObjs._editor.GetDouble(pdo);

                    switch (pdr.Status)
                    {
                        case PromptStatus.Cancel:
                            ObjectId idCgPnt = ObjectId.Null;
                            Point3d pnt3d = Pub.pnt3dO;
                            tryParse = double.TryParse(UserInput.getPoint("\nSelect Cogo Point with desired elevation: ", 
                                out idCgPnt, out pnt3d, pnt3d, osMode: 8, round: false), out resRTd.valE);
                            if (!tryParse)
                                break;
                            break;
                        case PromptStatus.Error:
                            break;
                        case PromptStatus.Other:
                            break;
                        case PromptStatus.OK:
                            resRTd.valE = pdr.Value;
                            break;   
                        case PromptStatus.None:
                            break;
                    }

                    Pub.Elev = resRTd.valE;

                    prompt = string.Format("\nDistance / Rate of grade <{0}>: [D/R]: ", resRTd.opt2);
                    escape = UserInput.getUserInputKeyword(resRTd.opt2, out resRTd.opt2, prompt, "D R");
                    if (escape)
                        return;

                    switch (resRTd.opt2)
                    {
                        case "D":
                            resRTd.valD = Pub.Dist;
                            if (resRTd.valD == 0)
                                tryParse = double.TryParse(Dict.getCmdDefault("cmdRTD", "Distance"), out resRTd.valD);
                            
                            if (!tryParse)
                                resRTd.valD = 0.0; ;
                            
                            try
                            {
                                escape = UserInput.getUserInput("\nEnter Distance [pos(+) value = target direction, neg(-) value = target direction + 180 degrees:", out resRTd.valD, resRTd.valD);
                                if (escape)
                                    return;
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " cmdRTD.cs: line: 189");
                            }
                            Pub.Dist = resRTd.valD;

                            break;
                        case "R":
                            resRTd.valS = Pub.Slope;
                            if (resRTd.valS == 0.0)
                                tryParse = double.TryParse(Dict.getCmdDefault("cmdRTd", "Slope"), out resRTd.valS);

                            escape = UserInput.getUserInput("\nRate of Grade: ", out resRTd.valS, resRTd.valS);
                            if (escape)
                                return;

                            Pub.Slope = resRTd.valS;
                            break;
                    }

                    break;
                case "Z":
                    resRTd.valZ = Pub.dZ;

                    escape = UserInput.getUserInput("\nZ Value Difference", out resRTd.valZ, resRTd.valZ);
                    
                    if (escape)
                    return;

                    prompt = string.Format("\nDistance / Rate of grade <{0}>: [D/R]: ", resRTd.opt2);
                    escape = UserInput.getUserInputKeyword(resRTd.opt2, out resRTd.opt2, prompt, "D R");
                    if (escape)
                        return;

                    switch (resRTd.opt2){
                        case "D":
                            resRTd.valD = Pub.Dist;
                            if(resRTd.valD == 00)
                                tryParse = double.TryParse(Dict.getCmdDefault("cmdRTD", "Distance"), out resRTd.valD);
                            if (!tryParse)
                                resRTd.valD = 0.0;
                            try
                            {
                                escape = UserInput.getUserInput("\nEnter Distance [pos(+) value = target direction, neg(-) value = target direction + 180 degrees:", out resRTd.valD, resRTd.valD);
                                if (escape)
                                    return;
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " cmdRTD.cs: line: 236");
                            }
                            Pub.Dist = resRTd.valD;

                            break;
                        case "R":
                            resRTd.valS = Pub.Slope;
                            if (resRTd.valS == 0.0)
                                 tryParse = double.TryParse(Dict.getCmdDefault("cmdRTd", "Slope"), out resRTd.valS);

                            escape = UserInput.getUserInput("\nRate of Grade: ", out resRTd.valS, resRTd.valS);
                            if (escape)
                                return;
                            Pub.Slope = resRTd.valS;                           
                            break;
                    }
                    break;
            }


            switch (resRTd.opt1)
            {
                case "D":
                    pnt3dTAR = new Point3d( pnt3dBASE.X + System.Math.Cos(angle) * resRTd.valD,
                                            pnt3dBASE.Y + System.Math.Sin(angle) * resRTd.valD,
                                            pnt3dBASE.Z + resRTd.valS * System.Math.Abs(resRTd.valD));
                    break;
                case "E":

                    switch (resRTd.opt2)
                    {
                        case "D":
                            pnt3dTAR = pnt3dBASE.traverse(angle, resRTd.valD, 0);
                            pnt3dTAR = new Point3d(pnt3dTAR.X, pnt3dTAR.Y, resRTd.valE);
                            break;
                        case "R":
                            double distance = System.Math.Abs((resRTd.valE - pnt3dBASE.Z) / resRTd.valS);
                            pnt3dTAR = new Point3d( pnt3dBASE.X + System.Math.Cos(angle) * distance,
                                                    pnt3dBASE.Y + System.Math.Sin(angle) * distance,
                                                    resRTd.valE);
                            break;
                    }
                    break;
                case "Z":
                    switch(resRTd.opt2){
                        case "D":
                            pnt3dTAR = new Point3d( pnt3dBASE.X + System.Math.Cos(angle) * resRTd.valD,
                                                    pnt3dBASE.Y + System.Math.Sin(angle) * resRTd.valD,
                                                    pnt3dBASE.Z + resRTd.valZ);
                            break;
                        case "R":
                            double distance = System.Math.Abs(resRTd.valZ / resRTd.valS);
                            pnt3dTAR = new Point3d(pnt3dBASE.X + System.Math.Cos(angle) * distance,
                                                    pnt3dBASE.Y + System.Math.Sin(angle) * distance,
                                                    pnt3dBASE.Z + resRTd.valZ);
                            break;                        
                    }
                    break;
            }

            uint pntNum;
            ObjectId idCogoPntTAR = pnt3dTAR.setPoint(out pntNum, pntDesc);
            ObjectId idPoly3d = ObjectId.Null;

            idPnts.Add(idCogoPntTAR);

            List<Handle> hPnts = new List<Handle>();
            hPnts.Add(idPnts[0].getHandle());
            hPnts.Add(idPnts[1].getHandle());

            ObjectId idPoly = ObjectId.Null;
            using (BaseObjs._acadDoc.LockDocument())
            {
                idPoly3d = BrkLine.makeBreakline(nameApp, "cmdRTd", out idPoly, idPnts);
            }

            Grading_Palette.gPalette.pGrading.cmdRTd_Default = resRTd.opt1;
            Grading_Palette.gPalette.pGrading.cmdRTd_Distance = resRTd.valD.ToString();
            Grading_Palette.gPalette.pGrading.cmdRTd_Elevation = resRTd.valE.ToString();
            Grading_Palette.gPalette.pGrading.cmdRTd_Slope = resRTd.valS.ToString();

            Dict.setCmdDefault("cmdRTd", "cmdDefault", resRTd.opt1);
            Dict.setCmdDefault("cmdRTd", "Distance", resRTd.valD.ToString());
            Dict.setCmdDefault("cmdRTd", "Elevation", resRTd.valE.ToString());
            Dict.setCmdDefault("cmdRTD", "Slope", resRTd.valS.ToString());

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
                    BaseObjs.writeDebug(ex.Message + " cmdRTD.cs: line: 343");
                }
            }
        }
    }
}
