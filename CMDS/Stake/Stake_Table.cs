using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Math = Base_Tools45.Math;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace Stake
{
    public static class Stake_Table
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        private static Point3d pnt3dOrg = Base_Tools45.Pub.pnt3dO;

        public static ObjectId
        makeTable(ObjectId idAlign, List<POI> varpoi, AlgnData varAlignData = null, object varPntUpper = null)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();

            string strLayerName = "TABLES";
            Layer.manageLayers(strLayerName);
            Extents3d ext3d;
            Point3d pnt3dMin = Pub.pnt3dO, pnt3dMax = Pub.pnt3dO, pnt3dIns = Pub.pnt3dO;
            Table objTable = null;
            ObjectId idTable = ObjectId.Null;

            int numRows = varpoi.Count + 2;
            int numCols = 33;
            double rowHeight = 5;
            double colWidth = 30;
            if ((varAlignData == null))
            {
                if ((varPntUpper == null))
                {
                    ProfileView objProfileView = (ProfileView)fStake.HandleProfileView.getEnt();
                    ext3d = (Extents3d)objProfileView.Bounds;
                    pnt3dMin = ext3d.MinPoint;
                    pnt3dMax = ext3d.MaxPoint;
                    pnt3dIns = new Point3d(pnt3dMax.X + 10, pnt3dMax.Y, 0);

                    idTable = Draw.addTable(pnt3dIns, numRows, numCols, rowHeight, colWidth);

                    AlgnData algnData = new AlgnData();
                    algnData.AlignName = objAlign.Name;
                    algnData.AlignHandle = objAlign.Handle;
                    algnData.AlignLayer = objAlign.Layer;
                    algnData.AlignID = objAlign.ObjectId;
                    algnData.TableHandle = idTable.getHandle();

                    fStake.algnData.Add(algnData);
                }
                else //for WALL only
                {
                    pnt3dIns = new Point3d(pnt3dMax.X + 10, pnt3dMax.Y, 0);

                    idTable = Draw.addTable(pnt3dIns, numRows, numCols, rowHeight, colWidth);
                    AlgnData algnData = new AlgnData();
                    algnData.AlignName = objAlign.Name;
                    algnData.AlignHandle = objAlign.Handle;
                    algnData.AlignLayer = objAlign.Layer;
                    algnData.AlignID = objAlign.ObjectId;
                    algnData.TableHandle = idTable.getHandle();

                    fStake.algnData.Add(algnData);
                }
            }
            else
            {
                ProfileView objProfileView = (ProfileView)fStake.HandleProfileView.getEnt();
                ext3d = (Extents3d)objProfileView.Bounds;
                pnt3dMin = ext3d.MinPoint;
                pnt3dMax = ext3d.MaxPoint;
                pnt3dIns = new Point3d(pnt3dMax.X + 10, pnt3dMax.Y, 0);
                idTable = Draw.addTable(pnt3dIns, numRows, numCols, rowHeight, colWidth);
                varAlignData.TableHandle = idTable.getHandle();
            }

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                objTable = (Table)tr.GetObject(idTable, OpenMode.ForWrite);

                objTable.Layer = strLayerName;
                objTable.ColorIndex = 41;

                objTable.SuppressRegenerateTable(true);

                objTable.Cells[0, 0].TextString = objAlign.Name;

                objTable.Cells[1, 0].TextString = "INDEX";

                objTable.Cells[1, 1].TextString = "CLASS";
                objTable.Cells[1, 2].TextString = "DESCO";
                objTable.Cells[1, 3].TextString = "DESCX";
                objTable.Cells[1, 4].TextString = "STATION";
                objTable.Cells[1, 5].TextString = "OFFSET";
                objTable.Cells[1, 6].TextString = "ELEVATION";
                objTable.Cells[1, 7].TextString = "TOW";
                objTable.Cells[1, 8].TextString = "TOF";
                objTable.Cells[1, 9].TextString = "INVERT";
                objTable.Cells[1, 10].TextString = "SIZE";

                objTable.Cells[1, 11].TextString = "ANGDELTA";
                objTable.Cells[1, 12].TextString = "ANGDIR";
                objTable.Cells[1, 13].TextString = "ANGCHORD";
                objTable.Cells[1, 14].TextString = "ISRIGHTHAND";
                objTable.Cells[1, 15].TextString = "ISCLOSED";
                objTable.Cells[1, 16].TextString = "ISSTEP";
                objTable.Cells[1, 17].TextString = "CROSSDESC";
                objTable.Cells[1, 18].TextString = "CROSSALIGN";
                objTable.Cells[1, 19].TextString = "CROSSALIGNINV";
                objTable.Cells[1, 20].TextString = "CROSSALIGNSTA";

                objTable.Cells[1, 21].TextString = "CROSSALIGNSIZE";
                objTable.Cells[1, 22].TextString = "PNTNUM";
                objTable.Cells[1, 23].TextString = "PNTSOURCE";
                objTable.Cells[1, 24].TextString = "TYPE";
                objTable.Cells[1, 25].TextString = "BULGE";
                objTable.Cells[1, 26].TextString = "RADIUS";
                objTable.Cells[1, 27].TextString = "SIDE";
                objTable.Cells[1, 28].TextString = "SLOPEAHEAD";
                objTable.Cells[1, 29].TextString = "SLOPEBACK";
                objTable.Cells[1, 30].TextString = "SLOPEH2H";

                objTable.Cells[1, 31].TextString = "XCEN";
                objTable.Cells[1, 32].TextString = "YCEN";

                objTable.Columns[0].Width = 18.0;
                objTable.Columns[1].Width = 18.0;
                objTable.Columns[4].Width = 18.0;
                objTable.Columns[5].Width = 18.0;
                objTable.Columns[6].Width = 18.0;
                objTable.Columns[9].Width = 18.0;
                objTable.Columns[10].Width = 18.0;

                for (int i = 0; i < objTable.Rows.Count; i++)
                {
                    for (int j = 0; j < objTable.Columns.Count; j++)
                    {
                        objTable.Cells[i, j].TextHeight = 0.09 * Misc.getCurrAnnoScale();
                        objTable.Cells[i, j].Alignment = CellAlignment.MiddleCenter;
                    }
                }

                objTable.SuppressRegenerateTable(false);
                tr.Commit();
            }

            TypedValue[] tvs = new TypedValue[2] { new TypedValue(1001, "TABLE"), new TypedValue(1005, objTable.Handle) };
            objAlign.ObjectId.setXData(tvs, "TABLE");

            Stake_Dict.updateDictSTAKE();

            return objTable.ObjectId;
        }

        public static void
        addTableData(ObjectId idTable, List<POI> varpoi)
        {
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                Table objTable = (Table)tr.GetObject(idTable, OpenMode.ForWrite);
                objTable.SuppressRegenerateTable(true);

                for (int i = 0; i < varpoi.Count; i++)
                {
                    objTable.Cells[i + 2, 0].TextString = i.ToString();
                    objTable.Cells[i + 2, 1].TextString = varpoi[i].ClassObj;
                    objTable.Cells[i + 2, 2].TextString = varpoi[i].Desc0;
                    objTable.Cells[i + 2, 3].TextString = varpoi[i].DescX;
                    objTable.Cells[i + 2, 4].TextString = string.Format("{0:###+##.00}", varpoi[i].Station);
                    objTable.Cells[i + 2, 5].TextString = string.Format("{0:###.00}", System.Math.Round(varpoi[i].OFFSET, 3));
                    objTable.Cells[i + 2, 6].TextString = string.Format("{0:#####.000}", System.Math.Round(varpoi[i].Elevation, 3));
                    objTable.Cells[i + 2, 7].TextString = varpoi[i].ElevTOW.ToString();
                    objTable.Cells[i + 2, 8].TextString = varpoi[i].ElevTOF.ToString();
                    objTable.Cells[i + 2, 9].TextString = varpoi[i].Invert.ToString();
                    objTable.Cells[i + 2, 10].TextString = varpoi[i].Size.ToString();
                    objTable.Cells[i + 2, 11].TextString = varpoi[i].AngDelta.ToString();
                    objTable.Cells[i + 2, 12].TextString = varpoi[i].AngDir.ToString();
                    objTable.Cells[i + 2, 13].TextString = varpoi[i].AngChord.ToString();
                    objTable.Cells[i + 2, 14].TextString = varpoi[i].isRightHand.ToString();
                    objTable.Cells[i + 2, 15].TextString = varpoi[i].isClosed.ToString();
                    objTable.Cells[i + 2, 16].TextString = varpoi[i].isStep.ToString();
                    objTable.Cells[i + 2, 17].TextString = varpoi[i].CrossDesc;
                    objTable.Cells[i + 2, 18].TextString = varpoi[i].CrossAlign;
                    objTable.Cells[i + 2, 19].TextString = varpoi[i].CrossAlignInv.ToString();
                    objTable.Cells[i + 2, 20].TextString = varpoi[i].CrossAlignSta.ToString();
                    objTable.Cells[i + 2, 21].TextString = varpoi[i].CrossAlignSize.ToString();
                    objTable.Cells[i + 2, 22].TextString = varpoi[i].PntNum;
                    objTable.Cells[i + 2, 23].TextString = varpoi[i].PntSource;
                    objTable.Cells[i + 2, 24].TextString = varpoi[i].Type;
                    objTable.Cells[i + 2, 25].TextString = varpoi[i].Bulge.ToString();
                    objTable.Cells[i + 2, 26].TextString = varpoi[i].Radius.ToString();
                    objTable.Cells[i + 2, 27].TextString = varpoi[i].Side.ToString();
                    objTable.Cells[i + 2, 28].TextString = varpoi[i].SlopeAhead.ToString();
                    objTable.Cells[i + 2, 29].TextString = varpoi[i].SlopeBack.ToString();
                    objTable.Cells[i + 2, 30].TextString = varpoi[i].SlopeH2H.ToString();
                    objTable.Cells[i + 2, 31].TextString = varpoi[i].CenterPnt.X.ToString();
                    objTable.Cells[i + 2, 32].TextString = varpoi[i].CenterPnt.Y.ToString();
                }
                objTable.SuppressRegenerateTable(false);
                tr.Commit();
            }
        }

        public static List<POI>
        resetPOI(ObjectId idTable)
        {
            List<POI> varpoi = new List<POI>();
            POI vpoi = new POI();

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                Table objTable = (Table)tr.GetObject(idTable, OpenMode.ForRead);

                for (int i = 2; i < objTable.Rows.Count; i++)
                {
                    vpoi.ClassObj = objTable.Cells[i + 2, 1].TextString;
                    vpoi.Desc0 = objTable.Cells[i + 2, 2].TextString;
                    vpoi.DescX = objTable.Cells[i + 2, 3].TextString;
                    vpoi.Station = double.Parse(objTable.Cells[i + 2, 4].TextString.Replace("+", ""));
                    vpoi.OFFSET = double.Parse(objTable.Cells[i + 2, 5].TextString);
                    vpoi.Elevation = double.Parse(objTable.Cells[i + 2, 6].TextString);
                    vpoi.ElevTOW = double.Parse(objTable.Cells[i + 2, 7].TextString);
                    vpoi.ElevTOF = double.Parse(objTable.Cells[i + 2, 8].TextString);
                    vpoi.Invert = double.Parse(objTable.Cells[i + 2, 9].TextString);
                    vpoi.Size = double.Parse(objTable.Cells[i + 2, 10].TextString);
                    vpoi.AngDelta = double.Parse(objTable.Cells[i + 2, 11].TextString);
                    vpoi.AngDir = double.Parse(objTable.Cells[i + 2, 12].TextString);
                    vpoi.AngChord = double.Parse(objTable.Cells[i + 2, 13].TextString);
                    vpoi.isRightHand = (objTable.Cells[i + 2, 14].TextString.ToUpper() == "TRUE") ? true : false;
                    vpoi.isClosed = (objTable.Cells[i + 2, 15].TextString.ToUpper() == "TRUE") ? true : false;
                    vpoi.isStep = (objTable.Cells[i + 2, 16].TextString.ToUpper() == "TRUE") ? true : false;
                    vpoi.CrossDesc = objTable.Cells[i + 2, 17].TextString;
                    vpoi.CrossAlign = objTable.Cells[i + 2, 18].TextString;
                    vpoi.CrossAlignInv = double.Parse(objTable.Cells[i + 2, 19].TextString);
                    vpoi.CrossAlignSta = double.Parse(objTable.Cells[i + 2, 20].TextString);
                    vpoi.CrossAlignSize = double.Parse(objTable.Cells[i + 2, 21].TextString);
                    vpoi.PntNum = objTable.Cells[i + 2, 22].TextString;
                    vpoi.PntSource = objTable.Cells[i + 2, 23].TextString;
                    vpoi.Type = objTable.Cells[i + 2, 24].TextString;
                    vpoi.Bulge = double.Parse(objTable.Cells[i + 2, 25].TextString);
                    vpoi.Radius = double.Parse(objTable.Cells[i + 2, 26].TextString);
                    vpoi.Side = Int16.Parse(objTable.Cells[i + 2, 27].TextString);
                    vpoi.SlopeAhead = double.Parse(objTable.Cells[i + 2, 28].TextString);
                    vpoi.SlopeBack = double.Parse(objTable.Cells[i + 2, 29].TextString);
                    vpoi.SlopeH2H = double.Parse(objTable.Cells[i + 2, 30].TextString);
                    vpoi.CenterPnt = new Point3d(double.Parse(objTable.Cells[i + 2, 31].TextString), double.Parse(objTable.Cells[i + 2, 32].TextString), 0);

                    varpoi.Add(vpoi);
                }
                tr.Commit();
            }
            return varpoi;
        }

        public static void
        updateTableData(ObjectId idTable, List<POI> varpoi)
        {
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                Table objTable = (Table)tr.GetObject(idTable, OpenMode.ForWrite);
                int lngRows = objTable.Rows.Count - 3; //1 for Zero base and 2 for top two rows = 3

                Debug.Print(lngRows.ToString());

                int lngRowsDiff = varpoi.Count - lngRows;

                Debug.Print(lngRowsDiff.ToString());

                if (lngRowsDiff < 0)
                {
                    objTable.DeleteRows(lngRows + lngRowsDiff, System.Math.Abs(lngRowsDiff));
                }
                else if (lngRowsDiff > 0)
                {
                    objTable.InsertRows(lngRows, 5, lngRowsDiff);
                }

                objTable.SuppressRegenerateTable(true);

                for (int i = 2; i < objTable.Rows.Count; i++)
                {
                    for (int j = 0; j < objTable.Columns.Count; j++)
                    {
                        objTable.Cells[i, j].DeleteContent();
                    }
                }

                tr.Commit();
            }
            addTableData(idTable, varpoi);
        }

        public static void
        syncTableWithProfile()
        {
            List<POI> varPOIcur = fStake.POI_CALC;

            Alignment objAlign = fStake.ACTIVEALIGN;
            ObjectId idAlign = objAlign.ObjectId;
            ResultBuffer rb = idAlign.getXData("CLASS");
            if (rb == null)
                return;

            TypedValue[] tvs = rb.AsArray();
            fStake.ClassObj = tvs[1].Value.ToString();

            Profile objProfile = null;
            try
            {
                objProfile = Prof.getProfile(idAlign, "STAKE");
            }
            catch (System.Exception )
            {
            }

            ProfilePVICollection objProfilePVIs = objProfile.PVIs;
            Table objTable = null;
            ObjectId idTable = ObjectId.Null;
            try
            {
                idTable = Stake_Table.getTableId(idAlign);
            }
            catch (System.Exception )
            {
                try
                {
                    SelectionSet ss = Select.buildSSet(typeof(Table));
                    ObjectId[] ids = ss.GetObjectIds();

                    if (ids.Length > 0)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            idTable = ids[i];
                            objTable = (Table)idTable.getEnt();
                            if (objTable.Cells[1, 1].TextString == objAlign.Name)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Select Table Failed");
                    return;
                }
            }

            fStake.POI_CALC = resetPOI(idTable);
            List<POI> varPOITmp = fStake.POI_CALC;

            List<POI> varpoi = new List<POI>();
            List<POI> varpoiAdd = new List<POI>();

            Debug.Print(varPOITmp.Count.ToString());
            ProfilePVI objProfilePVI = null;

            for (int i = 0; i < varPOITmp.Count; i++)
            {
                for (int j = 0; j < objProfilePVIs.Count; j++)
                {
                    objProfilePVI = objProfilePVIs[j];
                    if (Math.roundDown2((objProfilePVI.Station)) == Math.roundDown2(varPOITmp[i].Station))
                    {
                        varpoi.Add(varPOITmp[i]);
                        break;
                    }
                }
            }
            bool boolExists = false;
            for (int i = 0; i < objProfilePVIs.Count; i++)
            {
                boolExists = false;
                objProfilePVI = objProfilePVIs[i];
                for (int j = 0; j < varpoi.Count; j++)
                {
                    if (Math.roundDown2(varpoi[j].Station) == Math.roundDown2((objProfilePVI.Station)))
                    {
                        boolExists = true;
                        break;
                    }
                }

                if (!boolExists)
                {
                    POI vpoi = new POI();
                    vpoi.Station = Math.roundDown3((objProfilePVI.Station));
                    vpoi.Elevation = objProfilePVI.Elevation;
                    vpoi.ClassObj = fStake.ClassObj;
                    vpoi.Desc0 = "GB";
                    varpoiAdd.Add(vpoi);
                }
            }

            if (varpoiAdd[0].Desc0 != "NOTHING")
            {
                for (int i = 0; i < varpoiAdd.Count; i++)
                {
                    varpoi.Add(varpoiAdd[i]);
                }
            }

            var sortSta = from p in varpoi
                          orderby p.Station ascending
                          select p;

            List<POI> poiTmp = new List<POI>();
            foreach (var p in sortSta)
                poiTmp.Add(p);

            varpoi = poiTmp;

            updateTableData(idTable, varpoi);

            fStake.POI_CALC = varpoi;
        }

        public static ObjectId
        getTableId(ObjectId idAlign)
        {
            ObjectId idTable = ObjectId.Null;
            List<AlgnData> algnData = fStake.algnData;
            for (int i = 1; i <= algnData.Count; i++)
            {
                if (idAlign.getHandle() == algnData[i].AlignHandle)
                {
                    Handle hTable = algnData[i].TableHandle;
                    idTable = hTable.getObjectId();
                    break;
                }
            }
            return idTable;
        }
    }
}