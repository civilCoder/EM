using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Label = System.Windows.Forms.Label;

namespace Stake
{
    public static class Stake_Grid
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmBldgNames fBldgNames = Forms.Stake_Forms.sForms.fBldgNames;
        private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

        public static double PI = System.Math.PI;
        private static List<GRID_DATA> gDatas = new List<GRID_DATA>();
        private static Color color;

        public static void
        processGrid()
        {
            ObjectId idAlign = Stake_Algn.createGridAlign();
            Alignment align = (Alignment)idAlign.getEnt();
            if ((align == null))
            {
                return;
            }

            Stake_CheckBldgElev.setBldgElevRef(align);

            List<POI> varpoi = new List<POI>();
            Stake_GetAnglePoints.getAnglePoints(idAlign, ref varpoi);
            //IDENTIFY ANGLE POINTS - CHECK IF CLOSED

            Stake_CheckBldgElev.getBldgElev(idAlign, ref varpoi);

            fStake.POI_ORG = varpoi;

            Stake_AddProfile.makeProfile(idAlign, varpoi, "STAKE", "ByLayout", false);

            getGridIntersects(idAlign, ref varpoi);

            //sortGridPOI(varpoi);
            var sort1 = from s in varpoi
                        orderby s.OFFSET ascending
                        group s by s.Station into staGrp
                        orderby staGrp.Key
                        select staGrp;

            List<POI> vpoi = new List<POI>();
            foreach (var s in sort1)
            {
                foreach (var p in s)
                {
                    vpoi.Add(p);
                }
            }

            Stake_DuplicateStations.resolveDuplicateStationsGrid(vpoi);

            var sort2 = from s in varpoi
                        orderby s.OFFSET ascending
                        group s by s.Station into staGrp
                        orderby staGrp.Key
                        select staGrp;

            varpoi = new List<POI>();
            foreach (var s in sort2)
            {
                foreach (var p in s)
                {
                    varpoi.Add(p);
                }
            }

            ObjectId idTable = Stake_Table.makeTable(idAlign, varpoi);
            Stake_Table.addTableData(idTable, varpoi);

            GRID_DATA grid_data = new GRID_DATA();

            grid_data.Name = align.Name;
            grid_data.POIs = varpoi;

            gDatas.Add(grid_data);
        }

        public static void
        setupGrid(ObjectId idLine, string strName, string strGroup)
        {
            double dblAngTar = 0;

            switch (strGroup)
            {
                case "ALPHA":
                    Debug.Print("ALPHA COUNT = " + fGrid.GRIDALPHA.Count);
                    break;

                case "NUMERIC":
                    Debug.Print("NUMERIC COUNT = " + fGrid.GRIDNUMERIC.Count);
                    break;
            }

            if (fStake.NameStakeObject == "")
            {
                int i = idLine.getLayer().IndexOf("GRID");
                fStake.NameStakeObject = idLine.getLayer().Substring(i);
            }

            ObjectId idGridMemX = ObjectId.Null;

            switch (strGroup)
            {
                case "ALPHA":
                    try
                    {
                        idGridMemX = getGridMemX(strName, "NUMERIC");
                    }
                    catch (System.Exception)
                    {
                    }

                    break;

                case "NUMERIC":

                    try
                    {
                        idGridMemX = getGridMemX(strName, "ALPHA");
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
            }
            double angGridMemX = ((Line)idGridMemX.getEnt()).Angle;
            double angLine = ((Line)idLine.getEnt()).Angle;

            if ((idGridMemX != ObjectId.Null))
            {
                if (System.Math.Round(System.Math.Tan(angGridMemX), 4) == 0)
                {
                    if (System.Math.Round(1 / System.Math.Tan(angLine), 4) != 0)
                    {
                        Application.ShowAlertDialog("GRID A and GRID 1 are not perpendicular - revise and redo");
                        return;
                    }
                }
                else if (System.Math.Round(System.Math.Tan(angLine), 4) == 0)
                {
                    if (System.Math.Round(1 / System.Math.Tan(angGridMemX), 4) != 0)
                    {
                        Application.ShowAlertDialog("GRID A and GRID 1 are not perpendicular - revise and redo");
                        return;
                    }
                }
                else
                {
                    if (System.Math.Round(System.Math.Tan(angGridMemX) + 1 / System.Math.Tan(angLine), 4) != 0)
                    {
                        Application.ShowAlertDialog("GRID A and GRID 1 are not perpendicular - revise and redo");
                        return;
                    }
                }
            }

            dblAngTar = angLine;

            Point3d pnt3dBeg = Pub.pnt3dO;
            Point3d pnt3dEnd = Pub.pnt3dO;

            if (strGroup == "NUMERIC")
            {
                if (dblAngTar > 0 & dblAngTar < PI / 2)
                {
                }
                else if (dblAngTar > 3 * PI / 2 & dblAngTar < 2 * PI)
                {
                }
                else
                {
                    idLine.reverse();
                    dblAngTar = ((Line)idLine.getEnt()).Angle;
                }
            }
            else if (strGroup == "ALPHA")
            {
                if (dblAngTar > 0 & dblAngTar < PI)
                {
                    idLine.reverse();
                    dblAngTar = ((Line)idLine.getEnt()).Angle;
                }
                else
                {
                }
            }

            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue(1001, "CLASS"), 0);
            tvs.SetValue(new TypedValue(1000, "BLDG"), 1);

            TypedValue[] tvSel = new TypedValue[2];
            tvSel.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName), 0);
            tvSel.SetValue(new TypedValue((int)DxfCode.LayerName, idLine.getLayer()), 1);

            SelectionSet ss = Select.buildSSet(tvSel);
            ObjectId[] ids = ss.GetObjectIds();

            Line objGridMem = null;
            for (int i = 0; i <= ids.Length; i++)
            {
                ObjectId idGridMem = ids[i];
                idGridMem.setXData(tvSel, "CLASS");             //CLASS: BLDG
                objGridMem = (Line)idGridMem.getEnt();

                if (objGridMem.Handle != idLine.getHandle())
                {
                    double angDiff = System.Math.Round(System.Math.Abs(objGridMem.Angle - dblAngTar), 5);
                    if (angDiff == 0 || angDiff == System.Math.Round(2 * PI, 5))
                    {
                        if (strGroup == "NUMERIC")
                        {
                            fGrid.GRIDNUMERIC.Add(objGridMem.Handle);
                            color = new Color();
                            color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                            idGridMem.changeProp(color: color);
                        }
                        else if (strGroup == "ALPHA")
                        {
                            fGrid.GRIDALPHA.Add(objGridMem.Handle);
                            color = new Color();
                            color = Color.FromColorIndex(ColorMethod.ByBlock, 6);
                            idGridMem.changeProp(color: color);
                        }
                    }
                    else if (angDiff == System.Math.Abs(System.Math.Round(PI, 5)))
                    {
                        objGridMem.ReverseCurve();

                        //pnt3dBeg = objGridMem.StartPoint;
                        //pnt3dEnd = objGridMem.EndPoint;

                        //objGridMem.StartPoint = pnt3dEnd;
                        //objGridMem.EndPoint = pnt3dBeg;

                        if (strGroup == "NUMERIC")
                        {
                            fGrid.GRIDNUMERIC.Add(objGridMem.Handle);
                            color = new Color();
                            color = Color.FromColorIndex(ColorMethod.ByBlock, 3);

                            idGridMem.changeProp(color: color);
                        }
                        else if (strGroup == "ALPHA")
                        {
                            fGrid.GRIDALPHA.Add(objGridMem.Handle);
                            color = new Color();
                            color = Color.FromColorIndex(ColorMethod.ByBlock, 6);
                            idGridMem.changeProp(color: color);
                        }
                    }
                }
            }

            switch (strGroup)
            {
                case "ALPHA":
                    Debug.Print("ALPHA COUNT = " + fGrid.GRIDALPHA.Count);
                    break;

                case "NUMERIC":
                    Debug.Print("NUMERIC COUNT = " + fGrid.GRIDNUMERIC.Count);

                    break;
            }
        }

        public static void
        idGridSets()
        {
            BaseObjs.acadActivate();
            ObjectId idLine = ObjectId.Null;

            string strBldgNo = Align.getAlignName("GRID").Substring(5);

            do
            {
                BaseObjs.write(string.Format("\nSelect Building Grid Set no: {0}", strBldgNo));

                TypedValue[] tvSel = new TypedValue[2];
                tvSel.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName), 0);
                tvSel.SetValue(new TypedValue((int)DxfCode.LayerName, idLine.getLayer()), 1);

                SelectionSet ss = Select.buildSSet(tvSel);
                ObjectId[] ids = ss.GetObjectIds();

                if (ids.Length == 0)
                {
                    break;
                }

                string strLayer = string.Format("STAKE-BLDG-GRID{0}", strBldgNo);

                Layer.manageLayers(strLayer);
                Layer.manageLayer(strLayer, short.Parse(strBldgNo));
                color = new Color();
                color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

                for (int i = 0; i <= ids.Length; i++)
                {
                    idLine = ids[i];
                    idLine.changeProp(color, strLayer);
                }

                bool exists = false;
                ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);
                ObjectId idDictG = Dict.getSubEntry(idDictGRIDS, string.Format("GRID{0}", strBldgNo));
                ObjectId idDictT = Dict.getSubEntry(idDictG, string.Format("BLDG{0}", strBldgNo));

                strBldgNo = (int.Parse(strBldgNo) + 1).ToString();
            }
            while (true);

            fGrid.setupControls(int.Parse(strBldgNo) - 1);
            fGrid.BLDG_COUNT = (int.Parse(strBldgNo) - 1);
        }

        public static ObjectId
        getGridMemX(string strName, string strType)
        {
            ObjectId idLine = ObjectId.Null;

            bool exists = false;
            ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);
            ObjectId idDictG = Dict.getSubEntry(idDictGRIDS, strName);
            ObjectId idDictT = Dict.getSubEntry(idDictG, strType);

            ObjectId idDict1 = Dict.getSubEntry(idDictT, "INDEX");

            List<DBDictionaryEntry> entries = idDict1.getDictEntries();

            idLine = entries[0].Key.ToString().stringToHandle().getObjectId();
            return idLine;
        }

        public static void
        editBldgNum()
        {
            bool exists = false;
            ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);

            Control.ControlCollection cntrls = fBldgNames.Controls;
            Control cntrl;
            for (int i = cntrls.Count - 1; i >= 0; i += -1)
            {
                cntrl = cntrls[i];
                if (cntrl.Name != "cmdDone")
                {
                    cntrls.Remove(cntrl);
                }
            }

            Label lbl = new Label();
            lbl.Text = "GRID NO.";
            lbl.Name = "lblGridNo";
            cntrl = lbl;

            var _with1 = cntrl;
            _with1.Left = 6;
            _with1.Top = 6;
            _with1.Width = 48;
            _with1.Height = 18;

            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Text = "BLDG NO.";
            lbl.Name = "lblBldgNo";
            cntrl = lbl;

            var _with2 = cntrl;
            _with2.Left = 60;
            _with2.Top = 6;
            _with2.Width = 48;
            _with2.Height = 18;

            cntrls.Add(cntrl);

            int k = 0;
            List<DBDictionaryEntry> entries = Dict.getEntries(idDictGRIDS);
            foreach (DBDictionaryEntry entry in entries)
            {
                k++;
                string nameDictG = entry.Key.ToString();
                ObjectId idDictG = Dict.getSubDict(idDictGRIDS, nameDictG);

                ObjectId idDictT = Dict.getSubDict(idDictG, "BLDGNAME");
                List<TypedValue[]> tvs = Dict.getXRecs(idDictT);

                string strBldgName = tvs[0][0].Value.ToString();

                lbl = new Label();
                lbl.Name = string.Format("lblGridNo{0}", k);
                lbl.Text = nameDictG;

                cntrl = lbl;
                var _with3 = cntrl;
                _with3.Left = 6;
                _with3.Top = 24 + k * 18;
                _with3.Width = 48;
                _with3.Height = 18;

                TextBox txBox = new TextBox();
                txBox.Name = string.Format("tbxBldgName{0}", k);
                txBox.Text = strBldgName;

                cntrl = txBox;
                var _with4 = cntrl;
                _with4.Left = 60;
                _with4.Top = 24 + k * 18;
                _with4.Width = 48;
                _with4.Height = 18;
            }

            fBldgNames.cmdDone.Top = 24 + k * 18 + 24;
            fBldgNames.Height = 24 + k * 18 + 90;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fBldgNames, false);
        }

        public static void
        updateDictGridsBldgNum()
        {
            Label lbl = new Label();
            TextBox txBox = new TextBox();
            RadioButton opt = new RadioButton();
            Button cmd = new Button();

            bool exists = false;
            ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);

            Control.ControlCollection cntrls = fBldgNames.Controls;
            Control cntrl = null;

            for (int i = 0; i <= cntrls.Count - 1; i++)
            {
                cntrl = cntrls[i];

                if (cntrl.Name.Contains("tbxBldgName"))
                {
                    int j = int.Parse(cntrl.Name.Substring(12));

                    txBox = (TextBox)cntrl;

                    string strBldgName = txBox.Text;

                    lbl = (Label)cntrls["lblGridNo" + j];

                    ObjectId idDictG = Dict.getSubDict(idDictGRIDS, lbl.Text);

                    ObjectId idDictT = Dict.getSubDict(idDictG, "BLDGNAME");

                    List<DBDictionaryEntry> entries = Dict.getEntries(idDictT);
                    DBDictionaryEntry entry = entries[0];
                    Xrecord xRec = Dict.getXRec(idDictT, entry);
                    ResultBuffer rb = xRec.Data;
                    TypedValue[] tvs = rb.AsArray();

                    opt = (RadioButton)fGrid.Frame05.Controls["optBTN" + j];
                    opt.Text = "BLDG " + strBldgName;

                    cmd = (Button)fGrid.Frame08.Controls["GRID" + j];
                    cmd.Text = "BLDG " + strBldgName;

                    cmd = (Button)fGrid.Frame10.Controls["cmd" + j];
                    cmd.Text = "BLDG " + strBldgName;
                    TypedValue[] tvs2 = new TypedValue[tvs.Length];
                    tvs2.SetValue(new TypedValue(1001, strBldgName), 0);
                    tvs.CopyTo(tvs2, 1);

                    Dict.deleteXRec(idDictT, entry.Key.ToString());
                    Dict.addXRec(idDictT, strBldgName, new ResultBuffer(tvs2));
                }
            }
        }

        public static void
        labelGrids()
        {
            int intLimNumeric = int.Parse(fGrid.cbxNumeric.SelectedText);
            string strLimAlpha = fGrid.cbxAlpha.SelectedText;
            char[] charLim = strLimAlpha.ToCharArray();
            int intLimAlpha = (int)charLim[0];

            string strName = "";
            Control.ControlCollection objCntrls = fGrid.Frame05.Controls;

            for (int i = 0; i < objCntrls.Count - 1; i++)
            {
                Control objCntrl = objCntrls[i];
                if (objCntrl.Name.Contains("optBTN"))
                {
                    strName = objCntrl.Name;
                    strName = strName.Replace("optBTN", "GRID");
                    break;
                }
            }

            updateGridCollections(strName);

            int intBldgNo = int.Parse(strName.Substring(5));

            List<Handle> colNUMERIC = fGrid.GRIDNUMERIC;

            if (colNUMERIC.Count == 0)
            {
                Point3d pnt3dPicked = Pub.pnt3dO;
                string prompt = string.Format("Select a GRID Line on BLDG{0} to proceed....", intBldgNo);
                Autodesk.AutoCAD.DatabaseServices.Entity ent = Select.selectEntity(typeof(Line), prompt, "", out pnt3dPicked);
                int i = ent.Layer.IndexOf("GRID");
                strName = ent.Layer.Substring(i);

                updateGridCollections(strName);

                colNUMERIC = fGrid.GRIDNUMERIC;
            }

            List<Handle> colALPHA = fGrid.GRIDALPHA;

            labelGrid(colNUMERIC, intLimNumeric, strName, "NUMERIC");
            labelGrid(colALPHA, intLimAlpha, strName, "ALPHA");
        }

        public static void
        labelGrid(List<Handle> colGrid, int varLim, string strName, string varType)
        {
            ObjectId idBase = getGridMemX(strName, varType.ToString());
            Line objBase = (Line)idBase.getEnt();

            double dblAng = objBase.Angle;
            string strLayer = objBase.Layer;

            Layer.manageLayers(string.Format("{0}-LABEL", strLayer));

            //BEGIN OFFSET CALCULATION

            Point3d varPntA = objBase.StartPoint;

            double dblCa = varPntA.Y - System.Math.Tan(dblAng) * varPntA.X;

            List<GRIDPROP> vGridProps = new List<GRIDPROP>();
            GRIDPROP vGridProp = new GRIDPROP();

            vGridProp.GRIDMEM = objBase;
            vGridProp.DELTA = 0.0;
            vGridProp.OFFSET = 0.0;
            vGridProps.Add(vGridProp);

            Line objLine = null;
            for (int i = 1; i < colGrid.Count; i++)
            {
                if (colGrid[i] != objBase.Handle)
                {
                    objLine = (Line)colGrid[i].getEnt();
                    Point3d varPntB = objLine.StartPoint;
                    double dblCb = varPntB.Y - System.Math.Tan(dblAng) * varPntB.X;
                    double dblOff = System.Math.Round(System.Math.Abs((dblCb - dblCa) * System.Math.Cos(dblAng)), 2);
                    vGridProp = new GRIDPROP { GRIDMEM = objLine, OFFSET = dblOff };
                }
            }
            //END OFFSET CALCULATION

            var sortOff = from o in vGridProps
                          orderby o.OFFSET ascending
                          select o;

            List<GRIDPROP> vGridPropsX = new List<GRIDPROP>();
            foreach (var o in sortOff)
                vGridPropsX.Add(o);

            vGridProps = vGridPropsX;

            for (int i = 1; i < vGridProps.Count; i++)
            {
                vGridProp = vGridProps[i];
                vGridProp.DELTA = System.Math.Round(vGridProps[i - 0].OFFSET - vGridProps[i - 1].OFFSET, 3);
                vGridProps[i] = vGridProp;
            }

            colGrid = new List<Handle>();

            for (int i = 0; i < vGridProps.Count; i++)
            {
                objLine = vGridProps[i].GRIDMEM;
                colGrid.Add(objLine.Handle);
            }

            if (varType == "ALPHA")
            {
                if (fGrid.cboxUseI.Checked)
                {
                    varLim = varLim - 64;
                }
                else
                {
                    varLim = varLim - 64 - 1;
                }
                if (fGrid.cboxOmitO.Checked)
                {
                    varLim = varLim - 1;
                }
            }

            TypedValue[] tvs = new TypedValue[4];
            tvs.SetValue(new TypedValue(1001, "GRID"), 0);
            string gridId = "";
            Handle h;
            Point3d varPntDim = Pub.pnt3dO;
            if (colGrid.Count == varLim)
            {
                if (varType == "ALPHA")
                {
                    gridId = "A";
                    varPntDim = vGridProps[0].GRIDMEM.StartPoint;
                    varPntDim = varPntDim.traverse(objBase.Angle, -20);
                    varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[1].DELTA);

                    tvs.SetValue(new TypedValue(1000, gridId), 1);
                }
                else
                {
                    gridId = "1";
                    varPntDim = vGridProps[0].GRIDMEM.EndPoint;
                    varPntDim = varPntDim.traverse(objBase.Angle, 20);
                    varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[1].DELTA);
                    dblAng = dblAng - PI / 2;

                    tvs.SetValue(new TypedValue(1000, gridId), 1);
                }

                Point3d varPnt = objBase.StartPoint;
                varPnt = varPnt.traverse(objBase.Angle, 30);
                h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                tvs.SetValue(new TypedValue(1005, h), 2);

                varPnt = objBase.EndPoint;
                varPnt = varPnt.traverse(objBase.Angle, -30);
                h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                tvs.SetValue(new TypedValue(1005, h), 3);

                idBase.setXData(tvs, "GRID");

                for (int i = 1; i < vGridProps.Count; i++)
                {
                    objLine = vGridProps[i].GRIDMEM;

                    if (varType == "ALPHA")
                    {
                        gridId = fGrid.cbxAlpha.Items[i].ToString();
                    }
                    else if (varType == "NUMERIC")
                    {
                        gridId = (i + 1).ToString();
                    }

                    varPnt = objLine.StartPoint;
                    varPnt = varPnt.traverse(objLine.Angle, 30);
                    h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                    tvs.SetValue(new TypedValue(1005, h), 2);

                    varPnt = objLine.EndPoint;
                    varPnt = varPnt.traverse(objLine.Angle, -30);

                    h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                    tvs.SetValue(new TypedValue(1005, h), 3);

                    objLine.ObjectId.setXData(tvs, "GRID");

                    if (varType == "ALPHA")
                    {
                        varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[i - 1].DELTA);
                        addGridDims((vGridProps[i - 1].GRIDMEM.StartPoint), (vGridProps[i - 0].GRIDMEM.StartPoint), varPntDim, dblAng + PI / 2, strLayer + "-LABEL", "ALPHA");
                    }
                    else if (varType == "NUMERIC")
                    {
                        varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[i - 1].DELTA);
                        addGridDims((vGridProps[i - 1].GRIDMEM.EndPoint), (vGridProps[i - 0].GRIDMEM.EndPoint), varPntDim, dblAng, strLayer + "-LABEL", "NUMERIC");
                    }
                }
            }
            else
            {
                var sortDelta = from d in vGridProps
                                orderby d.DELTA ascending
                                select d;

                List<DELTA> deltas = new List<DELTA>();
                DELTA delta = new DELTA { count = 1, delta = vGridProps[0].DELTA };
                deltas.Add(delta);

                //BEGIN WEED OUT COLINEAR GRIDLINES
                int k = 1, n = vGridProps.Count;
                for (int i = 2; i < vGridProps.Count; i++)
                {
                    if (vGridProps[i - 1].DELTA != vGridProps[i - 0].DELTA)
                    {
                        delta = new DELTA { count = k, delta = vGridProps[i - 1].DELTA };
                        deltas.Add(delta);
                        k = 1;
                    }
                    else
                    {
                        k = k + 1;
                    }
                }

                //TEST LAST ITEM COMPARED TO ONE BEFORE
                if (vGridProps[n - 1].DELTA != vGridProps[n - 2].DELTA)
                {
                    delta = new DELTA { count = k, delta = vGridProps[n - 2].DELTA };
                    deltas.Add(delta);

                    delta = new DELTA { count = 1, delta = vGridProps[n - 1].DELTA };
                    deltas.Add(delta);
                }
                else
                {
                    delta = new DELTA { count = k, delta = vGridProps[n - 1].DELTA };
                    deltas.Add(delta);
                }
                //END WEED OUT COLINEAR GRIDLINES

                var maxD = deltas.OrderByDescending(item => item.count).First();
                double dblNomDelta = maxD.count;

                var srtOff = from s in vGridProps
                             orderby s.OFFSET ascending
                             select s;

                List<GRIDPROP> gridProps = new List<GRIDPROP>();

                foreach (var s in srtOff)
                    gridProps.Add(s);
                vGridProps = gridProps;

                objLine = vGridProps[0].GRIDMEM;
                dblAng = objLine.Angle;

                if (varType == "ALPHA")
                {
                    gridId = "A";
                }
                else
                {
                    gridId = "1";
                    dblAng = dblAng - PI / 2;
                }

                Point3d varPnt = objLine.StartPoint;
                varPnt = varPnt.traverse(objLine.Angle, 30);
                h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                tvs.SetValue(new TypedValue(1005, h), 2);

                varPnt = objLine.EndPoint;
                varPnt = varPnt.traverse(objLine.Angle, -30);
                h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                tvs.SetValue(new TypedValue(1005, h), 3);

                objLine.ObjectId.setXData(tvs, "GRID");

                if (varType == "ALPHA")
                {
                    varPntDim = vGridProps[0].GRIDMEM.StartPoint;
                    varPntDim = varPntDim.traverse(objBase.Angle, -20);
                    varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[1].DELTA);
                }
                else
                {
                    varPntDim = vGridProps[0].GRIDMEM.EndPoint;
                    varPntDim = varPntDim.traverse(objBase.Angle, 20);
                    varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[1].DELTA);
                }

                k = 1;

                for (int i = 1; i < vGridProps.Count; i++)
                {
                    objLine = vGridProps[i].GRIDMEM;
                    dblAng = objLine.Angle;

                    ResultBuffer rb = objLine.ObjectId.getXData("GRIDTYPE");
                    if (rb != null)
                        gridId = string.Format("{0}.X", k);
                    else if (vGridProps[i].OFFSET == vGridProps[i - 1].OFFSET)
                    {
                        gridId = k.ToString();
                        //    ElseIf vGridProps[i].DELTA = dblNomDelta Then
                        //
                        //      k = k + 1
                        //      varXDataVal(1) = k
                        //
                        //    ElseIf vGridProps[i].DELTA + vGridProps[i - 1].DELTA = dblNomDelta Then
                        //
                        //      k = k + 1
                        //      varXDataVal(1) = k
                        //
                        //    ElseIf vGridProps[i].DELTA > dblNomDelta Then
                        //
                        //      k = k + 1
                        //      varXDataVal(1) = k
                        //
                        //    ElseIf vGridProps[i].DELTA > dblNomDelta * 0.75 Then
                        //
                        //      If vGridProps[i].DELTA = vDELTAs(UBound(vDELTAs) - 1).DELTA Then
                        //
                        //        k = k + 1
                        //        varXDataVal(1) = k
                        //
                        //      End If
                        //
                    }
                    else
                    {
                        k++;
                        gridId = k.ToString();
                    }

                    if (varType == "ALPHA")
                    {
                        gridId = gridId.Replace(k.ToString(), fGrid.cbxAlpha.Items[k - 1].ToString());
                    }
                    else if (varType == "NUMERIC")
                    {
                        dblAng = dblAng - PI / 2;
                    }

                    //START POINT

                    varPnt = objLine.StartPoint;
                    varPnt = varPnt.traverse(objLine.Angle, 30);
                    h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                    tvs.SetValue(new TypedValue(1005, h), 2);

                    //DIMENSION

                    if (varType == "ALPHA")
                    {
                        varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[i].DELTA);
                        addGridDims((vGridProps[i - 1].GRIDMEM.StartPoint), (vGridProps[i - 0].GRIDMEM.StartPoint), varPntDim, dblAng + PI / 2, strLayer + "-LABEL", "ALPHA");
                    }
                    else if (varType == "NUMERIC")
                    {
                        varPntDim = varPntDim.traverse(objBase.Angle - PI / 2, vGridProps[i].DELTA);
                        addGridDims((vGridProps[i - 1].GRIDMEM.EndPoint), (vGridProps[i - 0].GRIDMEM.EndPoint), varPntDim, dblAng, strLayer + "-LABEL", "NUMERIC");
                    }

                    //END POINT

                    varPnt = objLine.EndPoint;
                    varPnt = varPnt.traverse(objLine.Angle, -30);
                    h = addLabelText(gridId, strLayer + "-LABEL", varPnt, dblAng);
                    tvs.SetValue(new TypedValue(1005, h), 3);

                    objLine.ObjectId.setXData(tvs, "GRID");
                }
            }
        }

        public static Handle
        addLabelText(string varLabel, string strLayer, Point3d varPnt, double dblRotation)
        {
            MText mTxt = null;
            ObjectId idMtxt = Txt.addMText(varLabel, varPnt, dblRotation, 0);
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                mTxt = (MText)tr.GetObject(idMtxt, OpenMode.ForWrite);
                mTxt.Rotation = dblRotation;
                mTxt.Layer = strLayer;
                tr.Commit();
            }
            return mTxt.Handle;
        }

        public static void
        addGridDims(Point3d varPnt1, Point3d varPnt2, Point3d varPntX, double dblAng, string nameLayer, string strType)
        {
            ObjectId idDim = Dim.addDimRotated(varPnt1, varPnt2, varPntX, dblAng, ObjectId.Null, nameLayer, 0.09, 2);
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                RotatedDimension objDim = (RotatedDimension)tr.GetObject(idDim, OpenMode.ForWrite);
                objDim.Dimtxt = 4;
                objDim.Dimasz = dblAng;
                objDim.Layer = nameLayer;
                objDim.Dimdec = 2;
                objDim.Dimscale = 1.0;
                tr.Commit();
            }
        }

        public static void
        getGridIntersects(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            updateGridCollections(objAlign.Name);

            List<Handle> colALPHA = fGrid.GRIDALPHA;
            List<Handle> colNUMERIC = fGrid.GRIDNUMERIC;

            for (int i = 1; i < colALPHA.Count; i++)
            {
                ObjectId idLineAlpha = colALPHA[i].getObjectId();
                Line objLineAlpha = (Line)idLineAlpha.getEnt();
                ResultBuffer rbAlpha = idLineAlpha.getXData("GRID");
                if (rbAlpha == null)
                    return;
                TypedValue[] tvsAlpha = rbAlpha.AsArray();

                for (int j = 1; j < colNUMERIC.Count; j++)
                {
                    ObjectId idLineNumeric = colNUMERIC[j].getObjectId();
                    Line objLineNumeric = (Line)idLineNumeric.getEnt();
                    ResultBuffer rbNumeric = idLineNumeric.getXData("GRID");
                    if (rbNumeric == null)
                        return;
                    TypedValue[] tvsNumeric = rbNumeric.AsArray();

                    List<Point3d> varPntInts = idLineAlpha.intersectWith(idLineNumeric, 0);
                    double dblStation = 0, dblOffset = 0;

                    for (int k = 0; k < varPntInts.Count; k++)
                    {
                        objAlign.StationOffset(varPntInts[k].X, varPntInts[k].Y, ref dblStation, ref dblOffset);
                        POI vpoi = new POI();
                        vpoi.Station = dblStation;
                        vpoi.OFFSET = dblOffset;
                        vpoi.ClassObj = fStake.ClassObj;

                        if (vpoi.OFFSET == 0)
                        {
                            vpoi.Desc0 = "LIM";
                        }
                        else
                        {
                            vpoi.Desc0 = "INT";
                        }

                        vpoi.DescX = tvsAlpha[1].Value.ToString() + "@" + tvsNumeric[1].Value.ToString();
                        varpoi.Add(vpoi);
                    }
                }
            }
        }

        public static void
        GridStakePOIs(ObjectId idAlign)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            if (objAlign == null)
                return;
            string strAlignName = objAlign.Name;

            List<POI> varPOI_ORG = new List<POI>();

            for (int i = 0; i < gDatas.Count; i++)
            {
                if (gDatas[i].Name == strAlignName)
                {
                    varPOI_ORG = gDatas[i].POIs;
                    break;
                }
            }

            if (varPOI_ORG.Count == 0)
            {
                ObjectId idTable = ObjectId.Null;
                idTable = Stake_Table.getTableId(objAlign.ObjectId);
                varPOI_ORG = Stake_Table.resetPOI(idTable);
            }

            ResultBuffer rb = idAlign.getXData("GRID");
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();

            int r = 0;

            List<string> strCntlNumeric = new List<string>();
            List<string> strCntlAlpha = new List<string>();

            for (int i = 1; i < tvs.Length; i++)
            {
                ObjectId idLine = tvs.getObjectId(i);
                ResultBuffer rbLine = idLine.getXData("GRID");
                if (rbLine == null)
                    return;
                TypedValue[] tvsLine = rb.AsArray();

                if (tvsLine[1].Value.ToString().isInteger(out r))       //need to include test for decimal also to pick up intermediates
                {
                    strCntlNumeric.Add(tvsLine[1].Value.ToString());
                }
                else
                {
                    strCntlAlpha.Add(tvsLine[1].Value.ToString());
                }
            }

            List<POI> varPOI_OUT = new List<POI>();
            List<POI> varPOI_IN = new List<POI>();
            List<POI> varPOI_STAKE = new List<POI>();

            for (int i = 0; i < varPOI_ORG.Count; i++)
            {
                if (System.Math.Round(varPOI_ORG[i].OFFSET, 1) == 0)
                {
                    varPOI_OUT.Add(varPOI_ORG[i]);
                }
                else
                {
                    varPOI_IN.Add(varPOI_ORG[i]);
                }
            }

            if (fGrid.optOUT.Checked)
            {
                if (fGrid.optALT.Checked)
                {
                    for (int i = 0; i < varPOI_OUT.Count; i++)
                    {
                        bool boolAddPOI = false;
                        bool boolAlpha = false;
                        bool boolNumeric = false;

                        int intPos = varPOI_OUT[i].DescX.IndexOf("@");
                        string strALPHA = varPOI_OUT[i].DescX.Substring(0, intPos - 1);
                        string strNUMERIC = varPOI_OUT[i].DescX.Substring(intPos + 1);

                        boolAlpha = (strCntlAlpha.Contains(strALPHA)) ? true : false;
                        boolNumeric = (strCntlNumeric.Contains(strNUMERIC)) ? true : false;

                        char[] charAlpha = strALPHA.ToCharArray();

                        bool isEvenAlpha = false;
                        bool isEvenNumeric = false;

                        isEvenAlpha = (((int)charAlpha[0] % 2) == 0) ? true : false;
                        isEvenNumeric = ((double.Parse(strNUMERIC) % 2) == 0) ? true : false;

                        if (varPOI_OUT[i].Desc0 == "AP")
                        {
                            boolAddPOI = true;
                        }
                        else if (!isEvenAlpha && !isEvenNumeric)
                        {
                            boolAddPOI = true;
                        }
                        else if (boolAlpha && !isEvenNumeric)
                        {
                            boolAddPOI = true;
                        }
                        else if (boolNumeric & !isEvenAlpha)
                        {
                            boolAddPOI = true;
                        }
                        else if (boolAlpha & boolNumeric)
                        {
                            boolAddPOI = true;
                        }

                        if (boolAddPOI)
                        {
                            varPOI_STAKE.Add(varPOI_OUT[i]);
                        }
                    }
                }
                else
                {
                    varPOI_STAKE = varPOI_OUT;
                }
            }
            else if (fGrid.optIN.Checked)
            {
                varPOI_STAKE = varPOI_IN;
            }
            else if (fGrid.optBOTH.Checked)
            {
                varPOI_STAKE = varPOI_ORG;
            }

            if (fGrid.optRBC.Checked)
            {
                for (int i = 0; i < varPOI_STAKE.Count; i++)
                {
                    POI vpoi = varPOI_STAKE[i];
                    vpoi.Elevation = varPOI_STAKE[i].Elevation + double.Parse(fGrid.tbxOffsetV.Text);
                    varPOI_STAKE[i] = vpoi;
                }
            }

            Stake_Grid2.stakeGridPoints(idAlign, varPOI_STAKE);
        }

        public static ObjectId
        createBldgPoly()
        {
            Line objLine = null;
            Point3d pnt3dPicked = Pub.pnt3dO;
            List<ObjectId> idLines = new List<ObjectId>();

            do
            {
                try
                {
                    Autodesk.AutoCAD.DatabaseServices.Entity ent = Select.selectEntity(typeof(Line), "Select Perimeter Building Grid:", "", out pnt3dPicked);
                    objLine = (Line)ent;
                }
                catch (System.Exception)
                {
                    break;
                }

                idLines.Add(objLine.ObjectId);
                objLine.Highlight();
            }
            while (true);

            if ((idLines.Count == 0))
            {
                return ObjectId.Null;
            }
            color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

            ObjectId idPoly = Misc.rebuildLWPoly(idLines);
            idPoly.changeProp(color, objLine.Layer);

            if ((idPoly == ObjectId.Null))
            {
                Application.ShowAlertDialog("Alignment operation failed - try again.........exiting");
                return ObjectId.Null;
            }

            int k = idLines.Count + 1;

            TypedValue[] tvs = new TypedValue[k];
            tvs.SetValue(new TypedValue(1001, "GRID"), 0);
            for (int i = 0; i < idLines.Count; i++)
            {
                tvs.SetValue(new TypedValue(1005, idLines[i].getHandle()), i + 1);
            }

            idPoly.setXData(tvs, "GRID");

            for (int i = 0; i < idLines.Count; i++)
            {
                objLine = (Line)idLines[i].getEnt();
                objLine.Unhighlight();
            }

            return idPoly;
        }

        public static ObjectId
        addToGroup()
        {
            BaseObjs.acadActivate();
            Line objLineX = null;
            Point3d pnt3dPicked = Pub.pnt3dO;
            try
            {
                Autodesk.AutoCAD.DatabaseServices.Entity ent = Select.selectEntity(typeof(Line), "Select Grid LINE to add", "", out pnt3dPicked);
                objLineX = (Line)ent;
            }
            catch (System.Exception)
            {
                return ObjectId.Null;
            }

            Line objLineA = (Line)fGrid.GRIDALPHA[0].getEnt();
            Line objLineN = (Line)fGrid.GRIDNUMERIC[0].getEnt();
            double dblAngleX = System.Math.Round(objLineX.Angle, 3);
            double dblAngleA = System.Math.Round(objLineA.Angle, 3);
            double dblAngleN = System.Math.Round(objLineN.Angle, 3);

            double dblAngleDiff = dblAngleA - dblAngleX;

            ObjectId idLineX = objLineX.ObjectId;

            color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

            if (dblAngleDiff == 0)
            {
                idLineX.changeProp(color, objLineA.Layer);
                fGrid.GRIDALPHA.Add(idLineX.getHandle());
            }
            else if (dblAngleDiff == System.Math.Round(PI, 3) || dblAngleDiff == System.Math.Round(-PI, 3))
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    objLineX = (Line)tr.GetObject(idLineX, OpenMode.ForWrite);
                    objLineX.ReverseCurve();
                    tr.Commit();
                }
                idLineX.changeProp(color, objLineA.Layer);
                fGrid.GRIDALPHA.Add(idLineX.getHandle());
            }
            else
            {
                idLineX.changeProp(color, objLineN.Layer);

                fGrid.GRIDNUMERIC.Add(idLineX.getHandle());

                if (dblAngleN - dblAngleX != 0)
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        objLineX = (Line)tr.GetObject(idLineX, OpenMode.ForWrite);
                        objLineX.ReverseCurve();
                        tr.Commit();
                    }
                }
            }
            return idLineX;
        }

        public static void
        updateGridCollections(string strName)
        {
            ObjectId idDictGRIDS = ObjectId.Null;
            bool exists = false;
            try
            {
                idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);
            }
            catch (System.Exception)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Dictionary GRIDS is missing - exiting........");
                return;
            }

            ObjectId idDictG = ObjectId.Null;
            try
            {
                idDictG = Dict.getSubDict(idDictGRIDS, strName);
            }
            catch (System.Exception)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Dictionary " + strName + " is missing - exiting.........");
                return;
            }

            ObjectId idDictT = ObjectId.Null;
            idDictT = Dict.getSubDict(idDictG, "ALPHA");
            List<Handle> colDict = new List<Handle>();

            List<DBDictionaryEntry> entries = Dict.getEntries(idDictT);

            foreach (DBDictionaryEntry entry in entries)
            {
                ObjectId idDictX = Dict.getSubDict(idDictT, entry.Key.ToString());
                if (entry.Key.ToString() != "INDEX")
                {
                    List<TypedValue[]> recs = Dict.getXRecs(idDictX);
                    colDict.Add(recs[0][0].Value.ToString().stringToHandle());
                }
            }

            fGrid.GRIDALPHA = colDict;

            colDict = new List<Handle>();

            idDictT = Dict.getSubDict(idDictG, "NUMERIC");
            entries = Dict.getEntries(idDictT);
            foreach (DBDictionaryEntry entry in entries)
            {
                ObjectId idDictX = Dict.getSubDict(idDictT, entry.Key.ToString());
                if (entry.Key.ToString() != "INDEX")
                {
                    List<TypedValue[]> recs = Dict.getXRecs(idDictX);
                    colDict.Add(recs[0][0].Value.ToString().stringToHandle());
                }
            }

            fGrid.GRIDNUMERIC = colDict;
        }

        public static void
        getEastWestBaseLineDir(ObjectId idPoly, double dblAngTar, ref int intMark)
        {
            List<Point3d> varPntsPline = idPoly.getCoordinates3dList();
            List<Point3d> pnts3d = new List<Point3d>();
            Point3d pnt3d1 = Pub.pnt3dO, pnt3d2 = Pub.pnt3dO, pnt3d3 = Pub.pnt3dO;

            pnts3d.Add(varPntsPline[0]);            //first vertex of polyline

            for (int i = 1; i < varPntsPline.Count; i++)
            {
                if (varPntsPline[i - 1].X == varPntsPline[i].X && varPntsPline[i - 1].Y == varPntsPline[i].Y)
                {
                    //exclude point - duplicate
                }
                else
                {
                    pnts3d.Add(varPntsPline[i]);
                }
            }
            varPntsPline = new List<Point3d>();
            varPntsPline.Add(pnts3d[0]);
            for (int i = 1; i < pnts3d.Count - 2; i++)
            {
                pnt3d1 = varPntsPline[i - 1];
                pnt3d2 = varPntsPline[i + 0];
                pnt3d3 = varPntsPline[i + 1];

                double dblAng3Pnts = System.Math.Round(Geom.getAngle3Points(pnt3d1, pnt3d2, pnt3d3), 3);
                //  Debug.Print dblAng3Pnts

                if (dblAng3Pnts != 0 || dblAng3Pnts != System.Math.Round(PI, 3))
                {
                    varPntsPline.Add(pnts3d[i]);
                }
            }

            int k = pnts3d.Count;
            varPntsPline.Add(pnts3d[k - 1]);

            double dblLenMax = 0;
            int j = varPntsPline.Count;

            //find longest tangent with west to east orientation
            for (int i = 1; i < j; i++)
            {
                pnt3d1 = varPntsPline[1 - 1];
                pnt3d2 = varPntsPline[i];

                double dblLen = pnt3d1.getDistance(pnt3d2);
                double dblAng = pnt3d1.getDirection(pnt3d2);

                if (dblLen >= dblLenMax)
                {
                    if (PI / 2 >= dblAng & dblAng >= 0)
                    {
                        dblLenMax = dblLen;
                        dblAngTar = dblAng;
                        intMark = i;
                    }
                    else if (3 * PI / 2 <= dblAng & dblAng <= 2 * PI)
                    {
                        dblLenMax = dblLen;
                        dblAngTar = dblAng;
                        intMark = i;
                    }
                }
            }
        }

        public static void
        idSecondaryGrids()
        {
            BaseObjs.acadActivate();

            SelectionSet ss = Select.buildSSet(typeof(Line), false, "\nSelect Secondary Grid Members: ");
            ObjectId[] ids = ss.GetObjectIds();
            if (ids.Count() == 0)
                return;

            TypedValue[] tvs = new TypedValue[2] { new TypedValue(1001, "GRIDTYPE"), new TypedValue(1000, "SECONDARY") };

            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 2);

            for (int i = 0; i < ids.Count(); i++)
            {
                Line objLine = (Line)ids[i].getEnt();
                ids[i].changeProp(LineWeight.ByLayer, color);
                ids[i].setXData(tvs, "GRIDTYPE");
            }
        }
    }
}