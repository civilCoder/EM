using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetNestedObjects
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmMisc fMisc = Forms.Stake_Forms.sForms.fMisc;
        private static Color color;

        private static double PI = System.Math.PI;

        public static bool
        checkObject(ObjectId idEnt)
        {
            color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 6);
            switch (idEnt.getType())
            {
                case "Arc":
                case "Line":
                case "Polyline":
                case "Polyline3d":
                case "Alignment":

                    idEnt.changeProp(color, idEnt.getLayer(), LineWeight.LineWeight050);
                    return true;

                case "PolylineVertex3d":
                    Entity ent = idEnt.getEnt();
                    Polyline3d poly3d = (Polyline3d)ent.OwnerId.getEnt();
                    idEnt.changeProp(color, idEnt.getLayer(), LineWeight.LineWeight050);
                    return true;

                default:
                    return false;
            }
        }

        public static Alignment
        copyAlignFromXref(ObjectId idAlign, string strSource)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            AlignmentEntityCollection objAlignEnts = default(AlignmentEntityCollection);
            AlignmentEntity objAlignEnt = default(AlignmentEntity);
            AlignmentArc objAlignEntArc = default(AlignmentArc);
            AlignmentLine objAlignEntTan = default(AlignmentLine);

            Point2d pnt2dBeg = objAlign.ReferencePoint;
            double dblStationStart = objAlign.ReferencePointStation;
            string strAlignName = objAlign.Name;

            StationEquationCollection objStationEQs = objAlign.StationEquations;
            List<AlgnEntData> varAlignEntData = listAlignEnts(objAlign);

            objAlignEnts = objAlign.Entities;

            List<Vertex2d> pnts2dPoly = new List<Vertex2d>();

            double bulge = 0;
            for (int i = 0; i < varAlignEntData.Count; i++)
            {
                objAlignEnt = objAlignEnts.EntityAtId(varAlignEntData[i].ID);

                if (objAlignEnt.EntityType == AlignmentEntityType.Arc)
                {
                    objAlignEntArc = (AlignmentArc)objAlignEnt;

                    if (i == objAlignEnts.Count - 1)
                    {
                        bulge = System.Math.Tan(objAlignEntArc.Delta / 4);
                        if (objAlignEntArc.Clockwise)
                        {
                            bulge = bulge * -1.0;
                        }
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntArc.StartPoint.X, objAlignEntArc.StartPoint.Y, 0), bulge, 0, 0, 0));
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntArc.EndPoint.X, objAlignEntArc.EndPoint.Y, 0), 0, 0, 0, 0));
                    }
                    else
                    {
                        bulge = System.Math.Tan(objAlignEntArc.Delta / 4);
                        if (objAlignEntArc.Clockwise)
                        {
                            bulge = bulge * -1;
                        }
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntArc.StartPoint.X, objAlignEntArc.StartPoint.Y, 0), bulge, 0, 0, 0));
                    }
                }
                else if (objAlignEnt.EntityType == AlignmentEntityType.Line)
                {
                    objAlignEntTan = (AlignmentLine)objAlignEnt;

                    if (i == objAlignEnts.Count - 1)
                    {
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntTan.StartPoint.X, objAlignEntTan.StartPoint.Y, 0), 0, 0, 0, 0));
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntTan.EndPoint.X, objAlignEntTan.EndPoint.Y, 0), 0, 0, 0, 0));
                    }
                    else
                    {
                        pnts2dPoly.Add(new Vertex2d(new Point3d(objAlignEntTan.StartPoint.X, objAlignEntTan.StartPoint.Y, 0), 0, 0, 0, 0));
                    }
                }
            }

            ObjectId idPoly = Draw.addPoly(pnts2dPoly);

            string strLayer = "";
            if (strSource == "WALLDESIGN")
            {
                strLayer = "WALL-PROFILE";
            }
            else
            {
                strLayer = strSource;
            }

            Layer.manageLayers(strLayer);

            Alignment objAlignNew = null;
            try
            {
                objAlignNew = Align.addAlignmentFromPoly(strAlignName, strLayer, idPoly, "Standard", "Standard", true);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return null;
            }

            objAlignNew.ReferencePointStation = dblStationStart;

            double dblStaBack = 0;
            double dblStaAhead = 0;
            StationEquationType objStaEqType = default(StationEquationType);

            for (int i = 0; i <= objStationEQs.Count - 1; i++)
            {
                dblStaBack = objStationEQs[i].RawStationBack;
                dblStaAhead = objStationEQs[i].StationAhead;
                objStaEqType = objStationEQs[i].EquationType;

                objAlignNew.StationEquations.Add(dblStaBack, dblStaAhead, objStaEqType);
            }
            return objAlignNew;
        }

        public static List<AlgnEntData>
        listAlignEnts(Alignment objAlign)
        {
            AlignmentEntity objAlignEnt = default(AlignmentEntity);
            AlignmentLine objAlignEntTan = default(AlignmentLine);
            AlignmentArc objAlignEntArc = default(AlignmentArc);

            List<AlgnEntData> varAlignData = null;
            List<AlgnEntData> varAlignDataX = null;

            int lngBefore = 0;

            AlignmentEntityCollection ents = objAlign.Entities;

            for (int i = 0; i < ents.Count; i++)
            {
                objAlignEnt = ents[i];

                AlgnEntData alignData = new AlgnEntData();
                if (objAlignEnt.EntityType == AlignmentEntityType.Arc)
                {
                    objAlignEntArc = (AlignmentArc)objAlignEnt;
                    alignData.ID = objAlignEntArc.EntityId;
                    alignData.Type = "Arc";
                    alignData.StaBeg = System.Math.Round(objAlignEntArc.StartStation, 2);
                    alignData.StaEnd = System.Math.Round(objAlignEntArc.EndStation, 2);
                    alignData.Length = System.Math.Round(objAlignEntArc.Length, 2);
                    alignData.Radius = System.Math.Round(objAlignEntArc.Radius, 2);
                    alignData.EntBefore = objAlignEntArc.EntityBefore;

                    varAlignData.Add(alignData);
                }
                else if (objAlignEnt.EntityType == AlignmentEntityType.Line)
                {
                    objAlignEntTan = (AlignmentLine)objAlignEnt;
                    alignData.ID = objAlignEntTan.EntityId;
                    alignData.Type = "Tangent";
                    alignData.StaBeg = System.Math.Round(objAlignEntTan.StartStation, 2);
                    alignData.StaEnd = System.Math.Round(objAlignEntTan.EndStation, 2);
                    alignData.Length = System.Math.Round(objAlignEntTan.Length, 2);
                    alignData.Radius = 0.0;
                    alignData.EntBefore = objAlignEntTan.EntityBefore;

                    varAlignData.Add(alignData);
                }
            }

            int n = -1;
            int k = 0;
            do
            {
                for (int i = 0; i < ents.Count; i++)
                {
                    if (varAlignData[i].EntBefore == lngBefore)
                    {
                        n += 1;						//n is index for new list
                        varAlignDataX[n] = varAlignData[i];
                        k = i;
                        break;
                    }
                }

                lngBefore = varAlignData[k].ID;
            }
            while (n < ents.Count);

            return varAlignDataX;
        }

        public static string
        conformLayer(string strLayerName)
        {
            int intPos = strLayerName.IndexOf("|");
            strLayerName = strLayerName.Substring(intPos + 1);
            return strLayerName;
        }

        public static void
        getNestedObjects(ObjectId idAlign, ref List<POI> varpoi, string nameDwg)
        {
            BlockTableRecord btrXRefDbModelSpace = fStake.XRefDbModelSpace;
            getNestedEntities(idAlign, ref varpoi, btrXRefDbModelSpace);			//same source as select item

            if (!fStake.XRefObjectName.Contains("CNTL"))
            {
                BlockTableRecord ms = xRef.getXRefBlockTableRecordMS(nameDwg);
                getNestedEntities(idAlign, ref varpoi, ms); //CNTL if not same as source of selected item
            }
        }

        public static void
        getNestedEntities(ObjectId idAlign, ref List<POI> pois, BlockTableRecord btrXRefDbModelSpace)
        {
            Alignment align = (Alignment)idAlign.getEnt();
            double alignLength = align.Length;

            string strNameStakeObject = fStake.NameStakeObject;
            string strLayerName = strNameStakeObject + "-TEMP";

            Layer.manageLayers(strLayerName);
            Layer.manageLayer(strLayerName, 2);

            Point3d varPntBeg, varPntEnd;

            List<ObjectId> ids = new List<ObjectId>();

            foreach (ObjectId id in btrXRefDbModelSpace)
            {
                Entity ent = id.getEnt();

                int intPos = ent.Layer.IndexOf("|");

                string strLayerObj = ent.Layer.Substring(intPos + 1);

                List<string> layers = new List<string> { strNameStakeObject, "CURB", "FL", "GB", "EC", "BLDG", "SEWER", "WATER", "SD-CL", "PROP-WAT" };

                if (layers.Contains(strLayerObj))
                {
                    switch (ent.GetType().ToString())
                    {
                        case "Line":
                            ids.Add(ent.ObjectId);
                            break;

                        case "Arc":
                            ids.Add(ent.ObjectId);
                            break;

                        case "Polyline":
                            ids.Add(ent.ObjectId);
                            break;
                    }
                }
            }

            double dblStationBeg = 0, dblStationEnd = 0;
            double dblOffsetBeg = 0, dblOffsetEnd = 0;

            color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 4);
            for (int i = 0; i < ids.Count; i++)
            {
                ObjectId id = ids[i];

                Entity ent = id.getEnt();
                id.changeProp(color, ent.Layer, LineWeight.LineWeight100);

                int intPos = ent.Layer.IndexOf("|");

                string strLayerObj = ent.Layer.Substring(intPos + 1);

                if (strLayerObj == strNameStakeObject)
                {
                    switch (ent.GetType().Name)
                    {
                        case "Arc":

                            Arc objArc = (Arc)ent;

                            varPntBeg = objArc.StartPoint;
                            varPntEnd = objArc.EndPoint;

                            align.StationOffset(varPntBeg.X, varPntBeg.Y, ref dblStationBeg, ref dblOffsetBeg);
                            align.StationOffset(varPntEnd.X, varPntEnd.Y, ref dblStationEnd, ref dblOffsetEnd);

                            break;

                        case "Line":

                            Line line = (Line)ent;

                            varPntBeg = line.StartPoint;
                            varPntEnd = line.EndPoint;

                            align.StationOffset(varPntBeg.X, varPntBeg.Y, ref dblStationBeg, ref dblOffsetBeg);
                            align.StationOffset(varPntEnd.X, varPntEnd.Y, ref dblStationEnd, ref dblOffsetEnd);

                            break;

                        case "Polyline":

                            Polyline poly = (Polyline)ent;
                            varPntBeg = poly.StartPoint;
                            varPntEnd = poly.EndPoint;

                            align.StationOffset(varPntBeg.X, varPntBeg.Y, ref dblStationBeg, ref dblOffsetBeg);
                            align.StationOffset(varPntEnd.X, varPntEnd.Y, ref dblStationEnd, ref dblOffsetEnd);

                            break;
                    }

                    if (dblStationBeg >= align.StartingStation & dblStationBeg <= align.EndingStation)
                    {
                        if (dblStationEnd >= align.StartingStation & dblStationEnd <= align.EndingStation)
                        {
                            if (System.Math.Abs(dblOffsetBeg) < 0.1)
                            {
                                POI poi = new POI();

                                poi.Station = Math.roundDown3(dblStationBeg);
                                poi.ClassObj = fStake.ClassObj;
                                poi.Desc0 = "TEE";
                                poi.CrossDesc = "TEE";

                                if (dblOffsetEnd > 0)
                                {
                                    poi.Side = 1;
                                }
                                else
                                {
                                    poi.Side = -1;
                                }

                                pois.Add(poi);
                            }
                            else if (System.Math.Abs(dblOffsetEnd) < 0.1)
                            {
                                POI poi = new POI();

                                poi.Station = Math.roundDown3(dblStationEnd);
                                poi.ClassObj = fStake.ClassObj;
                                poi.Desc0 = "TEE";
                                poi.CrossDesc = "TEE";

                                if (dblOffsetBeg > 0)
                                {
                                    poi.Side = 1;
                                }
                                else
                                {
                                    poi.Side = -1;
                                }

                                pois.Add(poi);
                            }
                        }
                    }
                }
                else
                {
                    List<Point3d> varPntInt = align.intersectWith(ent, false, extend.source);

                    try
                    {
                        for (int n = 0; n <= varPntInt.Count; n++)
                        {
                            Point3d pnt3dInt = varPntInt[i];
                            align.StationOffset(pnt3dInt.X, pnt3dInt.Y, ref dblStationBeg, ref dblOffsetBeg);

                            POI poi = new POI();

                            poi.Station = Math.roundDown3(dblStationBeg);
                            poi.ClassObj = fStake.ClassObj;
                            poi.Desc0 = strLayerObj;
                            poi.CrossDesc = strLayerObj;

                            if (strLayerObj.Contains("SEW") || strLayerObj.Contains("WAT") || strLayerObj.Contains("SD") || strLayerObj.Contains("WTR"))
                                poi.Type = strLayerObj;

                            pois.Add(poi);
                        }
                    }
                    catch (System.Exception )
                    {
                    }
                }
            }

            for (int i = 0; i < ids.Count; i++)
                ids[i].delete();
        }

        public static bool
        getNestedPoints(ObjectId idAlign, ref List<POI> varpoi, BlockTableRecord objXRefDbModelSpace, string strSource)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();

            string strNameStakeObject = fStake.NameStakeObject;
            double dblToleranceLength = 0.15;

            double dblToleranceAngle = PI;

            Point2d varPntRef = objAlign.ReferencePoint;
            string strFilter = "";

            if (strSource.Contains("GCAL")
            )
            {
                strFilter = "*CPNT*";
            }
            else
            {
                strFilter = "*";
            }

            List<string> strPntNums = new List<string>();
            double dblStation = 0, dblOffset = 0;
            ResultBuffer rb = null;
            TypedValue[] tvs = null;

            List<POI> varPOI_PNTs = new List<POI>();
            foreach (ObjectId id in objXRefDbModelSpace)
            {
                bool boolAdd = false;

                Entity objXRefObj = id.getEnt();

                if (objXRefObj is CogoPoint)
                {
                    CogoPoint objPnt = (CogoPoint)objXRefObj;
                    Debug.Print(objPnt.PointNumber.ToString());

                    if (objPnt.Layer.Contains(strFilter))
                    {
                        strPntNums.Add(objPnt.PointNumber.ToString());

                        if (strSource == "GCAL")
                        {
                            rb = id.getXData("STAKE");
                            tvs = rb.AsArray();
                        }

                        objAlign.StationOffset(objPnt.Easting, objPnt.Northing, ref dblStation, ref dblOffset);

                        Debug.Print(objPnt.Easting + "," + objPnt.Northing);

                        Point2d pnt2d = new Point2d(objPnt.Location.X, objPnt.Location.Y);

                        if (System.Math.Abs(dblOffset) < dblToleranceLength)
                        {
                            if (dblStation >= objAlign.StartingStation && dblStation <= objAlign.EndingStation)
                                boolAdd = true;
                        }
                        else if (pnt2d.isEqual(varPntRef, dblToleranceLength))
                        {
                            Point3d pnt3d = new Point3d(objPnt.Easting, objPnt.Northing, 0);

                            double dblAngle1 = Math.roundDown3(pnt2d.getDirection(varPntRef));

                            AlignmentEntity objAlignEnt = objAlign.Entities.EntityAtId(objAlign.Entities.FirstEntity);

                            switch (objAlignEnt.EntityType)
                            {
                                case AlignmentEntityType.Arc:

                                    AlignmentArc objAlignEntArc = (AlignmentArc)objAlignEnt;
                                    double dblangle2 = 0;
                                    if (objAlignEntArc.Clockwise)
                                    {
                                        dblangle2 = objAlignEntArc.StartDirection - PI / 2 + objAlignEntArc.Delta / 2;
                                    }
                                    else
                                    {
                                        dblangle2 = objAlignEntArc.StartDirection + PI / 2 - objAlignEntArc.Delta / 2;
                                    }

                                    if (System.Math.Abs(dblAngle1 - dblangle2) < dblToleranceAngle)
                                    {
                                        boolAdd = true;
                                        dblStation = objAlign.StartingStation;
                                    }
                                    else if (System.Math.Abs(Math.roundDown3(dblAngle1 - 2 * PI) - dblangle2) < dblToleranceAngle)
                                    {
                                        boolAdd = true;
                                        dblStation = objAlign.StartingStation;
                                    }

                                    break;

                                case AlignmentEntityType.Line:

                                    AlignmentLine objAlignEntTan = (AlignmentLine)objAlignEnt;

                                    dblangle2 = Math.roundDown3((objAlignEntTan.Direction));

                                    if (System.Math.Abs(dblAngle1 - dblangle2) < dblToleranceAngle)
                                    {
                                        boolAdd = true;
                                        dblStation = objAlign.StartingStation;
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            boolAdd = false;
                        }
                    }

                    if (boolAdd == true)
                    {
                        POI varPOI_PNT = new POI();

                        varPOI_PNT.Station = Math.roundDown3(dblStation);
                        varPOI_PNT.PntNum = objPnt.PointNumber.ToString();
                        varPOI_PNT.Elevation = System.Math.Round(objPnt.Elevation, 3);

                        if (strSource != "WALLDESIGN")
                        {
                            varPOI_PNT.ClassObj = fStake.ClassObj;
                        }

                        varPOI_PNT.OFFSET = System.Math.Round(dblOffset, 2);
                        varPOI_PNT.PntSource = strSource;

                        if (strSource == "GCAL")
                        {
                            try
                            {
                                if (isTG(objXRefDbModelSpace, tvs[1].Value.ToString().stringToHandle()))
                                {
                                    varPOI_PNT.Desc0 = "TG";
                                }
                            }
                            catch (System.Exception)
                            {
                                varPOI_PNT.Desc0 = "PNT";
                            }
                        }
                        else if (strSource == "WALLDESIGN")
                        {
                            objAlign.StationOffset(objPnt.Easting, objPnt.Northing, ref dblStation, ref dblOffset);
                            varPOI_PNT.OFFSET = System.Math.Round(dblOffset, 2);
                            varPOI_PNT.Desc0 = string.Format("{0} {0} {2}", objPnt.RawDescription, varPOI_PNT.OFFSET, varPOI_PNT.Elevation);
                            varPOI_PNT.PntSource = "TOPO";
                        }
                        else
                        {
                            varPOI_PNT.Desc0 = "PNT";
                        }
                        varPOI_PNTs.Add(varPOI_PNT);
                    }
                }
            }

            if (varPOI_PNTs.Count > 0)
            {
                var sortSta = from p in varPOI_PNTs
                              orderby p.Station ascending
                              select p;

                List<POI> poiTmp = new List<POI>();
                foreach (var p in sortSta)
                    poiTmp.Add(p);

                Stake_Util.removeDuplicatePoints(ref poiTmp);

                varPOI_PNTs = poiTmp;

                if (strSource != "WALLDESIGN")
                {
                    fStake.POI_PNTs = varPOI_PNTs;
                }

                if (varPOI_PNTs[0].Desc0 != "NOTHING")
                {
                    for (int i = 0; i < varPOI_PNTs.Count; i++)
                    {
                        varpoi.Add(varPOI_PNTs[i]);
                    }
                }

                poiTmp = varpoi;
                sortSta = from p in poiTmp
                          orderby p.Station ascending
                          select p;
                varpoi = new List<POI>();
                foreach (var p in sortSta)
                    varpoi.Add(p);

                return true;
            }
            return false;
        }

        public static bool
        isTG(BlockTableRecord objXRefDbModelSpace, Handle varHandle)
        {
            foreach (ObjectId id in objXRefDbModelSpace)
            {
                Entity objXRefObj = id.getEnt();

                if (objXRefObj.Handle == varHandle)
                {
                    BlockReference objBlkRef = (BlockReference)objXRefObj;
                    var varAtts = objBlkRef.AttributeCollection;

                    ObjectId idAtt = varAtts[1];

                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        AttributeReference objAttRef = (AttributeReference)tr.GetObject(idAtt, OpenMode.ForRead);
                        //per characteristics of FL/TG callout
                        if (objAttRef.TextString.Contains("TG"))
                        {
                            return true;
                        }
                        tr.Commit();
                    }
                }
            }
            return false;
        }

        public static Polyline
        getBLDG(BlockTableRecord objXRefDbModelSpace)
        {
            List<ObjectId> idsPoly = new List<ObjectId>();
            foreach (ObjectId id in objXRefDbModelSpace)
            {
                Entity objXRefObj = id.getEnt();
                if (objXRefObj is Polyline)
                {
                    if (objXRefObj.Layer.Contains("BLDG"))
                    {
                        idsPoly.Add(id);
                        //test if grid is inside this boundary to match up grid to bldg limit
                    }
                }
            }
            if (idsPoly.Count == 0)
                return null;
            ObjectIdCollection ids = new ObjectIdCollection();
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable bt = (BlockTable)objXRefDbModelSpace.Database.BlockTableId.GetObject(OpenMode.ForRead);
                BlockReference br = (BlockReference)objXRefDbModelSpace.OwnerId.GetObject(OpenMode.ForRead);
                ids = xRef.copyXRefEnts(br.ObjectId, idsPoly);
            }

            Polyline objPoly = (Polyline)ids[0].getEnt();

            return objPoly;
        }

        public static void
        copyGRID(BlockTableRecord objXRefDbModelSpace)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            List<ObjectId> idsLines = new List<ObjectId>();
            foreach (ObjectId id in objXRefDbModelSpace)
            {
                Entity objXRefObj = id.getEnt();

                if (objXRefObj is Line)
                {
                    if (objXRefObj.Layer.Contains("GRID"))
                    {
                        idsLines.Add(id);
                    }
                }
            }
            Layer.manageLayers(fStake.XRefObject.Layer);

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable bt = (BlockTable)objXRefDbModelSpace.Database.BlockTableId.GetObject(OpenMode.ForRead);
                BlockReference br = (BlockReference)objXRefDbModelSpace.OwnerId.GetObject(OpenMode.ForRead);
                ids = xRef.copyXRefEnts(br.ObjectId, idsLines);
            }

            fStake.NameStakeObject = Align.getAlignName("GRID");
            string strLayerName = "STAKE-BLDG-" + "GRID0";

            Layer.manageLayers(strLayerName, 30);

            for (int k = 0; k < ids.Count; k++)
            {
                ids[k].changeProp(nameLayer: strLayerName);
            }
            return;
        }
    }
}