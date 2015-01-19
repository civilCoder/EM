using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Math = Base_Tools45.Math;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;


namespace Stake
{
    public static class Stake_Algn
    {
        private static Point3d pnt3dOrg = Base_Tools45.Pub.pnt3dO;
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

        public static double
        getStationTargetAlign(double dblStaRF, double dblOffRR, Alignment objAlignRF, Alignment objAlignTAR)
        {
            AlignmentEntity objAlignEntRF = default(AlignmentEntity);
            AlignmentArc objAlignEntArcRF = default(AlignmentArc);
            AlignmentLine objAlignEntTanRF = default(AlignmentLine);

            AlignmentEntity objAlignEntTAR = default(AlignmentEntity);
            AlignmentArc objAlignEntArcTAR = default(AlignmentArc);
            AlignmentLine objAlignEntTanTAR = default(AlignmentLine);

            double dblAngEntRF = 0;
            double dblAngEntTAR = 0;

            double dblSkew = 0;
            double dblOffRF = 0;
            double dblStaTAR = 0;

            objAlignEntRF = objAlignRF.Entities.EntityAtStation(dblStaRF);
            if (objAlignEntRF.EntityType == AlignmentEntityType.Arc)
            {
                objAlignEntArcRF = (AlignmentArc)objAlignEntRF;
            }
            else if (objAlignEntRF.EntityType == AlignmentEntityType.Line)
            {
                objAlignEntTanRF = (AlignmentLine)objAlignEntRF;
            }

            Point2d pnt2dBEG = objAlignEntTanRF.StartPoint;
            Point2d pnt2dEND = objAlignEntTanRF.EndPoint;

            dblAngEntRF = pnt2dBEG.getDirection(pnt2dEND);

            objAlignRF.DistanceToAlignment(dblStaRF, objAlignTAR, AlignmentSide.Both, ref dblOffRF, ref dblStaTAR);

            objAlignEntTAR = objAlignTAR.Entities.EntityAtStation(dblStaTAR);
            if (objAlignEntTAR.EntityType == AlignmentEntityType.Arc)
            {
                objAlignEntArcTAR = (AlignmentArc)objAlignEntRF;
            }
            else if (objAlignEntTAR.EntityType == AlignmentEntityType.Line)
            {
                objAlignEntTanTAR = (AlignmentLine)objAlignEntTAR;
            }

            pnt2dBEG = objAlignEntTanTAR.StartPoint;

            pnt2dEND = objAlignEntTanTAR.EndPoint;

            dblAngEntTAR = pnt2dBEG.getDirection(pnt2dEND);

            dblSkew = dblAngEntTAR - dblAngEntRF;

            //dblStaTAR = dblStaRF * System.Math.Cos(dblSkew)
            return dblStaTAR;
        }

        public static void
        deleteAlign()
        {
            foreach (ObjectId id in CivilApplication.ActiveDocument.GetSitelessAlignmentIds())
                id.delete();
        }

        public static void
        poly3dFromAlignProfile()
        {
            double dblStation = 0;

            double dblX = 0;
            double dblY = 0;
            double dblZ = 0;

            int j = 0;

            List<Point3d> pnts3d = new List<Point3d>();
            Alignment align = Align.getAlignment("RIVER-CL");
            Profile profile = Prof.getProfile(align.ObjectId, "0");

            for (j = 0; j <= profile.PVIs.Count - 1; j++)
            {
                ProfilePVI profilePVI = profile.PVIs[j];
                dblStation = profilePVI.Station;
                dblZ = profilePVI.Elevation;

                align.PointLocation(dblStation, 0.0, ref dblX, ref dblY);
                pnts3d.Add(new Point3d(dblX, dblY, dblZ));
            }

            pnts3d.addPoly3d();
        }

        public static void
        createNewAlign(string nameLayer)
        {
            string strPrompt = null;

            bool boolFirst = true;
            bool escape = true;
            bool done = false;

            Point3d pnt3d = Pub.pnt3dO;
            Point3dCollection pnts3d = new Point3dCollection();
            ObjectId idPoly = ObjectId.Null;
            PromptStatus ps;

            do
            {
                if (boolFirst)
                {
                    strPrompt = "Pick Start Point of Alignment";
                    boolFirst = false;
                }
                else
                    strPrompt = "Pick Next Point of Alignment";

                try
                {
                    pnt3d = UserInput.getPoint(strPrompt, pnt3d, out escape, out ps, osMode: 8);
                    if (escape)
                        done = true;
                    else
                        pnts3d.Add(pnt3d);
                }
                catch (System.Exception)
                {
                }
            } while (!done);

            if (pnts3d.Count > 1)
                idPoly = Draw.addPoly(pnts3d);

            string nameAlign = Align.getAlignName(nameLayer);

            Layer.manageLayers("STAKE");
            Alignment align = null;
            try
            {
                align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPoly, "STAKE", "STAKE", true);
            }
            catch (System.Exception)
            {
                align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPoly, "Standard", "Standard", true);
            }

            align.ReferencePointStation = 100;

            fStake.ACTIVEALIGN = align;
            fStake.HandleAlign = align.Handle;
        }

        public static ObjectId
        createGridAlign()
        {
            string strLayerName = null;

            ObjectId idPoly = Stake_Grid.createBldgPoly();

            if ((idPoly == ObjectId.Null))
            {
                return ObjectId.Null;
            }

            int i = idPoly.getLayer().IndexOf("GRID");

            string strAlignName = idPoly.getLayer().Substring(i);
            strLayerName = "STAKE" + "-" + strAlignName;
            Alignment objAlign = null;
            try
            {
                objAlign = Align.addAlignmentFromPoly(strAlignName, strLayerName, idPoly, "STAKE", "STAKE", false);
            }
            catch (System.Exception)
            {
                objAlign = Align.addAlignmentFromPoly(strAlignName, strLayerName, idPoly, "Standard", "Standard", false);
            }

            objAlign.ReferencePointStation = 100;

            fStake.ACTIVEALIGN = objAlign;
            fStake.HandleAlign = objAlign.Handle;
            fStake.objectID = objAlign.ObjectId;

            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue(1001, "CLASS"), 0);
            tvs.SetValue(new TypedValue(1000, fStake.ClassObj), 1);

            objAlign.ObjectId.setXData(tvs, "CLASS");

            ResultBuffer rb = idPoly.getXData("GRID");
            tvs = rb.AsArray();

            objAlign.ObjectId.setXData(tvs, "GRID");

            idPoly.delete();

            return objAlign.ObjectId;
        }

        public static void
        deleteGridAlign()
        {
            BaseObjs.acadActivate();

            Point3d pnt3dPick = Pub.pnt3dO;
            string nameAlign = "";
            ObjectId idAlign = Align.selectAlign("Select Alignment: ", "", out pnt3dPick, out nameAlign);
            if (idAlign == ObjectId.Null)
                return;

            if (nameAlign.Contains("GRID"))
            {
                ResultBuffer rb = idAlign.getXData("STAKE");
                if (rb == null)
                    return;

                TypedValue[] tvs = rb.AsArray();
                uint numBeg = uint.Parse(tvs[1].Value.ToString());
                uint numEnd = uint.Parse(tvs[2].Value.ToString());

                ObjectId idCgPnt = ObjectId.Null;
                for (int i = (int)numBeg; i <= (int)numEnd; i++)
                {
                    uint lngPntNum = (uint)i;
                    try
                    {
                        idCgPnt = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument.CogoPoints.GetPointByPointNumber(lngPntNum);
                        idCgPnt.delete();
                    }
                    catch (System.Exception)
                    {
                    }
                }

                string nameLayer = tvs[3].Value.ToString();

                tvs = new TypedValue[8];
                tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName), 1);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName), 2);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(DBText)).DxfName), 3);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(BlockReference)).DxfName), 4);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Dimension)).DxfName), 5);
                tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 6);
                tvs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 7);

                SelectionSet ss = Select.buildSSet(tvs);
                ObjectId[] ids = ss.GetObjectIds();
                foreach (ObjectId id in ids)
                    id.delete();

                Dict.deleteDictionary(nameAlign);

                nameLayer = string.Format("STAKE-BLDG-{0}-LABEL", nameAlign);

                tvs[7] = new TypedValue((int)DxfCode.LayerName, nameLayer);
                ss = Select.buildSSet(tvs);
                ids = ss.GetObjectIds();
                foreach (ObjectId id in ids)
                    id.delete();

                updateControls((nameAlign));

                idAlign.delete();
            }
        }

        public static void
        deleteAllGridAligns()
        {
            ObjectIdCollection idsAlign = BaseObjs._civDoc.GetAlignmentIds();

            foreach (ObjectId idAlign in idsAlign)
            {
                string nameAlign = Align.getAlignName(idAlign);
                if (nameAlign.Contains("GRID"))
                {
                    ResultBuffer rb = idAlign.getXData("STAKE");
                    if (rb == null)
                        return;

                    TypedValue[] tvs = rb.AsArray();
                    uint numBeg = uint.Parse(tvs[1].Value.ToString());
                    uint numEnd = uint.Parse(tvs[2].Value.ToString());

                    ObjectId idCgPnt = ObjectId.Null;
                    for (int i = (int)numBeg; i <= (int)numEnd; i++)
                    {
                        uint lngPntNum = (uint)i;
                        try
                        {
                            idCgPnt = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument.CogoPoints.GetPointByPointNumber(lngPntNum);
                            idCgPnt.delete();
                        }
                        catch (System.Exception)
                        {
                        }
                    }

                    string nameLayer = tvs[3].Value.ToString();

                    tvs = new TypedValue[8];
                    tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName), 1);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName), 2);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(DBText)).DxfName), 3);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(BlockReference)).DxfName), 4);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Dimension)).DxfName), 5);
                    tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 6);
                    tvs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 7);

                    SelectionSet ss = Select.buildSSet(tvs);
                    ObjectId[] ids = ss.GetObjectIds();
                    foreach (ObjectId id in ids)
                        id.delete();

                    Dict.deleteDictionary(nameAlign);

                    nameLayer = string.Format("STAKE-BLDG-{0}-LABEL", nameAlign);

                    tvs[7] = new TypedValue((int)DxfCode.LayerName, nameLayer);
                    ss = Select.buildSSet(tvs);
                    ids = ss.GetObjectIds();
                    foreach (ObjectId id in ids)
                        id.delete();

                    updateControls((nameAlign));

                    idAlign.delete();
                }
            }
        }

        public static void
        updateControls(string strName)
        {
            Form.ControlCollection cntrls = (Form.ControlCollection)fGrid.Frame05.Controls;
            Control cntrl = cntrls[string.Format("optBTN{0}", strName.Substring(5))];
            cntrls.Remove(cntrl);

            cntrls = (Form.ControlCollection)fGrid.Frame08.Controls;
            cntrl = cntrls[strName];
            cntrls.Remove(cntrl);

            cntrls = (Form.ControlCollection)fGrid.Frame10.Controls;
            cntrl = cntrls[string.Format("cmd{0}", strName.Substring(5))];
            cntrls.Remove(cntrl);
        }

        public static bool
        selectAlignment(ObjectId idAlign)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            AlgnData alignData = new AlgnData();
            AlgnData alignDataX = new AlgnData();

            Point3d pnt3dPicked = Pub.pnt3dO;
            string nameAlign = "";

            if ((idAlign == null))
            {
                idAlign = Align.selectAlign("\nSelect Alignment: ", "Alignment Selection Failed", out pnt3dPicked, out nameAlign);
            }

            ResultBuffer rb = idAlign.getXData("PROFILE");
            if (rb == null)
                return false;
            TypedValue[] tvs = rb.AsArray();
            fStake.HandleProfileView = tvs[1].Value.ToString().stringToHandle();

            rb = idAlign.getXData("CLASS");
            if (rb == null)
                return false;
            tvs = rb.AsArray();
            string strClass = tvs[1].Value.ToString();
            fStake.ClassObj = strClass;

            Alignment align = (Alignment)idAlign.getEnt();
            fStake.ACTIVEALIGN = align;
            fStake.HandleAlign = idAlign.getHandle();
            fStake.objectID = idAlign;

            int result = 0;
            for (int i = nameAlign.Length - 1; i >= 1; i += -1)
            {
                if (!nameAlign.Substring(i, 1).isInteger(out result))
                {
                    fStake.NameStakeObject = nameAlign.Substring(0, i);
                    break;
                }
            }

            ObjectId idTable = ObjectId.Null;

            Table table = null;
            try
            {
                idTable = Stake_Table.getTableId(idAlign);
            }
            catch (System.Exception)
            {
                try
                {
                    SelectionSet ss = Select.buildSSet(typeof(Table));
                    ObjectId[] ids = ss.GetObjectIds();

                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            table = (Table)tr.GetObject(ids[i], OpenMode.ForRead);
                            if (table.Cells[1, 1].Value.ToString() == nameAlign)
                            {
                                TypedValue[] t = new TypedValue[2];
                                t.SetValue(new TypedValue(1001, "TABLE"), 0);
                                t.SetValue(new TypedValue(1005, ids[i].getHandle()), 1);
                                idAlign.setXData(t, "TABLE");
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Select Table Failed");
                }
            }

            switch (fStake.algnData.Count)
            {
                case 0:

                    alignData.AlignHandle = align.Handle;
                    alignData.TableHandle = table.Handle;
                    alignData.AlignID = idAlign;
                    alignData.AlignName = nameAlign;
                    alignData.AlignLayer = align.Layer;

                    fStake.algnData.Add(alignData);

                    break;

                case 1:

                    alignData = fStake.algnData[0];

                    if (alignData.AlignHandle == align.Handle)
                    {
                        alignData.TableHandle = table.Handle;
                    }
                    else
                    {
                        alignDataX.AlignHandle = align.Handle;
                        alignDataX.TableHandle = table.Handle;
                        alignDataX.AlignID = idAlign;
                        alignDataX.AlignName = align.Name;
                        alignDataX.AlignLayer = align.Layer;

                        fStake.algnData.Add(alignDataX);
                    }
                    break;

                default:

                    bool boolFound = false;

                    for (int i = fStake.algnData.Count; i > 1; i--)
                    {
                        alignDataX = fStake.algnData[i - 1];
                        alignData = fStake.algnData[i];

                        if (alignDataX.AlignHandle == alignData.AlignHandle)
                        {
                            fStake.algnData.RemoveAt(i);
                        }
                    }

                    for (int i = 1; i < fStake.algnData.Count; i++)
                    {
                        alignData = fStake.algnData[i];
                        if (alignData.AlignHandle == objAlign.Handle)
                        {
                            alignData.TableHandle = idTable.getHandle();
                            boolFound = true;
                        }
                    }

                    if (!boolFound)
                    {
                        alignDataX.AlignHandle = objAlign.Handle;
                        alignDataX.TableHandle = idTable.getHandle();

                        alignDataX.AlignID = idAlign;
                        alignDataX.AlignName = objAlign.Name;
                        alignDataX.AlignLayer = objAlign.Layer;

                        fStake.algnData.Add(alignDataX);
                    }

                    break;
            }

            fStake.POI_CALC = Stake_Table.resetPOI(idTable);

            Stake_Misc.set_fStakeProps(strClass);

            if (strClass == "BLDG")
            {
                Stake_Grid.updateGridCollections(objAlign.Name);

                Application.ShowModelessDialog(Application.MainWindow.Handle, fGrid, false);
                fStake.Hide();
            }

            fStake.ACTIVEALIGN = objAlign;

            return true;
        }

        public static void reverseAlign(ObjectId idAlign)
        {
            List<POI> varpoi = null;
            List<POI> varPOI_ORG = null;
            List<POI> varPOI_Temp = null;
            ProfilePVICollection objPVIs = null;

            string nameLayer = idAlign.getLayer();

            TypedValue[] tvs = new TypedValue[9];
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName), 1);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Leader)).DxfName), 2);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(DBText)).DxfName), 3);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(MText)).DxfName), 4);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Circle)).DxfName), 5);
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Dimension)).DxfName), 6);
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 7);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 8);

            SelectionSet ss = Select.buildSSet(tvs);
            ObjectId[] ids = ss.GetObjectIds();
            foreach (ObjectId id in ids)
                id.delete();

            Alignment objAlign = (Alignment)idAlign.getEnt();
            double dblLenAlign = objAlign.Length;

            switch (fStake.ClassObj)
            {
                case "BLDG":
                    Profile profCPNT = Prof.getProfile(idAlign, "CPNT");
                    objPVIs = profCPNT.PVIs;
                    break;

                case "CURB":
                    Profile profFLOWLINE = Prof.getProfile(idAlign, "FLOWLINE");
                    objPVIs = profFLOWLINE.PVIs;

                    break;

                case "FL":

                    profCPNT = Prof.getProfile(idAlign, "CPNT");
                    objPVIs = profCPNT.PVIs;

                    break;

                case "SEWER":
                case "WTR":

                    profCPNT = Prof.getProfile(idAlign, "CPNT");
                    objPVIs = profCPNT.PVIs;

                    break;

                case "ALIGN":

                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Object is an existing design alignment - exiting");
                    return;
            }

            int j = objPVIs.Count;

            for (int i = 0; i < j; i++)
            {
                varPOI_Temp[i].Station = Math.roundDown3(dblLenAlign - (objPVIs[i].Station - objAlign.StartingStation)) + objAlign.StartingStation;
                //starting station is always 100
                varPOI_Temp[i].Elevation = objPVIs[i].Elevation;
            }

            objAlign.Reverse();

            varpoi = fStake.POI_CALC;

            //reverse varPOI stationing
            //need all POIs for descriptions
            for (int i = 0; i < varpoi.Count; i++)
            {
                varpoi[i].Station = Math.roundDown3(dblLenAlign - (varpoi[i].Station - objAlign.StartingStation)) + objAlign.StartingStation;
                //starting station is always 100
                varpoi[i].Side = varpoi[i].Side * -1;

                switch (varpoi[i].Desc0)
                {
                    case "AP":

                        if (varpoi[i].isRightHand)
                        {
                            varpoi[i].AngDir = varpoi[i].AngDir + varpoi[i].AngDelta;
                        }
                        else
                        {
                            varpoi[i].AngDir = varpoi[i].AngDir - varpoi[i].AngDelta;
                        }

                        varpoi[i].isRightHand = !varpoi[i].isRightHand;

                        break;

                    case "BC":

                        varpoi[i].Desc0 = "EC";
                        varpoi[i].DescX = varpoi[i].DescX.Replace("BC", "EC");

                        break;

                    case "EC":

                        varpoi[i].Desc0 = "BC";
                        varpoi[i].DescX = varpoi[i].DescX.Replace("EC", "BC");

                        break;
                }
            }

            varPOI_ORG = fStake.POI_ORG;

            //reverse varPOI_ORG stationing
            //need all POIs for descriptions
            for (int i = 0; i < varPOI_ORG.Count; i++)
            {
                varPOI_ORG[i].Station = Math.roundDown3(dblLenAlign - (varPOI_ORG[i].Station - objAlign.StartingStation)) + objAlign.StartingStation;
                //starting station is always 100
            }
            List<POI> poiTmp = new List<POI>();

            var sortPOI = from p in varpoi
                          orderby p.Station ascending
                          select p;
            foreach (var p in sortPOI)
                poiTmp.Add(p);
            varpoi = poiTmp;

            poiTmp = new List<POI>();
            var sortPOI_ORG = from n in varPOI_ORG
                              orderby n.Station ascending
                              select n;
            foreach (var n in sortPOI_ORG)
                poiTmp.Add(n);
            varPOI_ORG = poiTmp;

            poiTmp = new List<POI>();
            var sortPOI_Temp = from t in varPOI_Temp
                               orderby t.Station ascending
                               select t;
            foreach (var t in sortPOI_Temp)
                poiTmp.Add(t);
            varPOI_Temp = poiTmp;

            j = varpoi.Count;
            varpoi[0].DescX = varpoi[0].DescX.Replace("END", "BEG");
            varpoi[j].DescX = varpoi[j].DescX.Replace("BEG", "END");

            ObjectId idTable = Stake_Table.getTableId(idAlign);

            Stake_Table.addTableData(idTable, varpoi);

            fStake.POI_CALC = varpoi;
            fStake.POI_ORG = varPOI_ORG;

            switch (fStake.ClassObj)
            {
                case "BLDG":

                    Stake_AddProfile.makeProfile(idAlign, varPOI_Temp, "STAKE", "ByLayout", true);
                    Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_ORG), "STAKE", true, "STAKE");

                    break;

                case "CURB":

                    Stake_AddProfile.makeProfile(idAlign, varPOI_Temp, "CURB", "ByLayout", true);
                    Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_ORG), "FLOWLINE", true, "ORG");

                    break;

                case "FL":

                    Stake_AddProfile.makeProfile(idAlign, varPOI_Temp, "FLOWLINE", "BySurface", true);
                    Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_ORG), "STAKE", true, "ORG");

                    break;

                case "WTR":

                    Stake_AddProfile.makeProfile(idAlign, varPOI_Temp, "STAKE", "BySurface", true);
                    Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_ORG), "CPNT", true, "ORG");
                    Stake_UpdateProfile.updateProfile(idAlign, varpoi, "STAKE", false, "STAKE");

                    break;
            }
        }

        public static void
        changeAlignStartPoint()
        {
            //BEGIN: UPDATE PROFILE, TABLE, AND POIs

            AlgnData algnData = new AlgnData();
            ObjectId idAlign = ObjectId.Null;
            Alignment objAlign = null;

            if (selectAlignment(idAlign))
            {
                objAlign = fStake.ACTIVEALIGN;
            }
            else
            {
                return;
            }

            objAlign = fStake.ACTIVEALIGN;
            AlignmentEntityCollection objAlignEnts = objAlign.Entities;

            //BEGIN: UPDATE PROFILE, TABLE, AND POIs
            double dblLenAlign = objAlign.Length;
            PromptStatus ps;
            Point3d varPnt = UserInput.getPoint("Select Desired Start Point", out ps, osMode: 0);

            double dblStation = 0, dblOffset = 0;
            idAlign.getAlignStaOffset(varPnt, ref dblStation, ref dblOffset);
            double dblStationStart = objAlign.StartingStation;
            double dblStationDelta = dblStation - dblStationStart;
            // if varPnt is out of range then dblStation returns ZERO

            Profile profFLOWLINE = Prof.getProfile(idAlign, "FLOwLINE");
            ProfilePVICollection pvis = profFLOWLINE.PVIs;

            List<POI> varPOI_Temp = new List<POI>();
            foreach (ProfilePVI pvi in pvis)
            {
                POI poi = new POI { Station = Math.roundDown3(pvi.Station), Elevation = pvi.Elevation };
                varPOI_Temp.Add(poi);
            }

            Debug.Print("varPOI_Temp Before");
            for (int i = 0; i < varPOI_Temp.Count; i++)
            {
                Debug.Print(varPOI_Temp[i].Station + "  " + varPOI_Temp[i].Elevation);
            }

            //adjust POI_Temp stationing

            for (int i = 0; i < varPOI_Temp.Count; i++)
            {
                POI poi = varPOI_Temp[i];
                if (poi.Station < dblStationDelta + dblStationStart)
                {
                    poi.Station = Math.roundDown3(dblLenAlign + varPOI_Temp[i].Station);
                }
                else
                {
                    poi.Station = Math.roundDown3(varPOI_Temp[i].Station - dblStationDelta);
                }
                varPOI_Temp[i] = poi;
            }

            List<POI> poiTmp = new List<POI>();
            var sortPOI_Temp = from t in varPOI_Temp
                               orderby t.Station ascending
                               select t;
            foreach (var t in sortPOI_Temp)
                poiTmp.Add(t);
            varPOI_Temp = poiTmp;

            int k = varPOI_Temp.Count;

            POI poitemp = varPOI_Temp[k - 1];
            poitemp.Elevation = varPOI_Temp[0].Elevation;
            varPOI_Temp[k] = poitemp;

            Debug.Print("varPOI_Temp After");
            for (int i = 0; i < k; i++)
            {
                Debug.Print(varPOI_Temp[i].Station + "  " + varPOI_Temp[i].Elevation);
            }

            //END: UPDATE PROFILE, TABLE, AND POIs

            ObjectId idPoly = objAlign.GetPolyline();
            Polyline poly = (Polyline)idPoly.getEnt();

            string strAlignName = objAlign.Name;
            AlgnData aData;
            for (int j = 1; j < fStake.algnData.Count; j++)
            {
                aData = fStake.algnData[j];
                if (aData.AlignHandle == objAlign.Handle)
                {
                    break;
                }
            }

            objAlign.ReferencePointStation = 100.0;

            foreach (AlignmentEntity ent in objAlignEnts)
                objAlignEnts.Remove(ent);

            List<Vertex2d> v2ds = Conv.poly_Vertex2dList(idPoly);

            int p = 0;
            foreach (Vertex2d v in v2ds)
            {
                if (v.Position.IsEqualTo(varPnt, new Tolerance(0, 0)))
                {
                    break;
                }
                p++;
            }

            List<Vertex2d> v2dsNew = new List<Vertex2d>();
            for (int i = p; i < v2ds.Count; i++)
            {
                v2dsNew.Add(v2ds[i]);
            }

            for (int i = 1; i < p; i++)
            {
                v2dsNew.Add(v2ds[i]);
            }
            v2dsNew.Add(v2ds[p]);

            int lngID = 0;
            Point3d dblPntBeg = Pub.pnt3dO, dblPntEnd = Pub.pnt3dO, dblPntMid = Pub.pnt3dO;

            for (int i = 1; i < v2dsNew.Count; i++)
            {
                if (v2dsNew[i].Bulge == 0)
                {
                    dblPntBeg = v2dsNew[i - 1].Position;
                    dblPntEnd = v2dsNew[i = 0].Position;

                    AlignmentLine objAlignEntTan = objAlign.Entities.AddFixedLine(lngID, dblPntBeg, dblPntEnd);
                    lngID = objAlignEntTan.EntityId;
                }
                else
                {
                    dblPntBeg = v2dsNew[i - 1].Position;
                    dblPntEnd = v2dsNew[i = 0].Position;

                    int intDir = 0;

                    if (v2dsNew[i - 1].Bulge > 0)
                    {
                        intDir = 1;
                    }
                    else
                    {
                        intDir = -1;
                    }

                    Arc arc = (Arc)Arc.Create(IntPtr.Zero, true);
                    arc.StartPoint = dblPntBeg;
                    arc.EndPoint = dblPntEnd;

                    Point3d pnt3dMidLC = dblPntBeg.getMidPoint3d(dblPntEnd);
                    double lenLC = dblPntBeg.getDistance(dblPntEnd);
                    double dirLC = dblPntBeg.getDirection(dblPntEnd);
                    double lenM = System.Math.Abs(lenLC / 2 * v2dsNew[i - 1].Bulge);
                    dblPntMid = pnt3dMidLC.traverse(dirLC + System.Math.PI / 2 * intDir, lenM);

                    AlignmentArc objAlignEntArc = objAlign.Entities.AddFixedCurve(lngID, dblPntBeg, dblPntMid, dblPntEnd);
                    lngID = objAlignEntArc.EntityId;
                }
            }

            objAlign.Update();
            objAlign.ReferencePoint = varPnt.Convert2d(BaseObjs.xyPlane);
            objAlign.ReferencePointStation = System.Math.Round(100.0 + dblStationDelta, 3);

            //BEGIN: UPDATE PROFILE, TABLE, AND POIs
            List<POI> varpoi = fStake.POI_CALC;
            //POIs are updated when selectAlignment is executed

            //ADJUST POI STATIONING
            //need complete POIs for descriptions
            for (int i = 0; i < varpoi.Count; i++)
            {
                if (varpoi[i].Station < dblStationDelta + dblStationStart)
                {
                    varpoi[i].Station = Math.roundDown3(dblLenAlign + varpoi[i].Station);
                }
                else
                {
                    varpoi[i].Station = Math.roundDown3(varpoi[i].Station - dblStationDelta);
                }
            }

            varpoi = varpoi.sortPOIbyStation();

            int n = varpoi.Count;

            varpoi[0].DescX = "BEG " + varpoi[0].Desc0;
            varpoi[n - 2].DescX = varpoi[n - 2].DescX.Replace("END", "").Trim();
            varpoi[n - 1].DescX = varpoi[n - 1].DescX.Replace("BEG", "END").Trim();

            if (fStake.ClassObj == "CURB")
            {
                Stake_AddProfile.makeProfile(idAlign, varPOI_Temp, "CURB", "ByLayout", true);
            }

            ObjectId idTable = Stake_Table.makeTable(idAlign, varpoi);
            Stake_Table.addTableData(idTable, varpoi);

            if (Stake_GetBC_EC.getBC_EC(idAlign, ref varpoi) == false)
            {
                return;
            }

            fStake.POI_CALC = varpoi;
            //END: UPDATE PROFILE, TABLE, AND POIs
        }
    }
}