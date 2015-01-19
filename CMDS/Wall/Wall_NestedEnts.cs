using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Wall {
    public static class Wall_NestedEnts {
        const double PI = System.Math.PI;
        static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

        public static void
        selectNestedPoints(string strFunction) {
            string strPrompt = "Select " + strFunction + " Cogo Point";
            string strFile_Layer = "", strFile = "", strLayer = "";
            bool escape;
            CogoPoint objPnt = null;
            Alignment objAlign = fWall1.ACTIVEALIGN;
            double dblStation = 0, dblOffset = 0;
            List<PNT_DATA> varPNT_DATA = new List<PNT_DATA>();
            do {
                try {
                    Entity ent = xRef.getEntity(strPrompt, out escape, out strFile_Layer);
                    if (ent is CogoPoint) {
                        ObjectId idPnt = ent.ObjectId;
                        objPnt = (CogoPoint)ent;
                    }

                    if (objPnt != null) {
                        Misc.processLayername(strFile_Layer, ref strLayer, ref strFile);
                        try {
                            try {
                                objAlign.StationOffset(objPnt.Easting, objPnt.Northing, ref dblStation, ref dblOffset);
                            }
                            catch (Autodesk.Civil.PointNotOnEntityException ) {
                                dblStation = 0.0;
                            }

                            PNT_DATA vPNT_DATA = new PNT_DATA();
                            vPNT_DATA.DESC = objPnt.RawDescription;
                            vPNT_DATA.NUM = objPnt.PointNumber;
                            vPNT_DATA.x = objPnt.Easting;
                            vPNT_DATA.y = objPnt.Northing;
                            vPNT_DATA.z = System.Math.Round(objPnt.Elevation, 3);
                            vPNT_DATA.ALIGN = objAlign.Name;
                            vPNT_DATA.STA = System.Math.Round(dblStation, 2);
                            vPNT_DATA.OFFSET = System.Math.Round(dblOffset, 2);

                            varPNT_DATA.Add(vPNT_DATA);
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ) {
                        }
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    break;
                }
            }
            while (true);

            List<PNT_DATA> t = new List<PNT_DATA>();
            dynamic sta = from data in varPNT_DATA
                          orderby data.STA
                          select data;

            foreach (var d in sta)
                t.Add(d);

            switch (strFunction) {
                case "DESIGN":
                    fWall1.PNTSDESIGN = t;
                    break;
                case "EXIST":
                    fWall1.PNTSEXIST = t;
                    break;
            }
        }

        public static ObjectId
        getNestedPoint(string strFunction) {
            string strPrompt = "Select " + strFunction + " Cogo Point";
            string strFile_Layer = "";
            bool escape;
            ObjectId idPnt = ObjectId.Null;

            do {
                try {
                    Entity ent = xRef.getEntity(strPrompt, out escape, out strFile_Layer);

                    if (ent is CogoPoint) {
                        idPnt = ent.ObjectId;
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    break; 
                }
            }
            while (true);

            return idPnt;
        }

        public static bool
        getNestedPoints(ObjectId idAlign, ref List<POI> varPOI, BlockTableRecord btrMS, string strSource) {
            bool boolAdd = false;

            double dblToleranceAngle = PI, dblToleranceLength = 0, dblStation = 0, dblOffset = 0;

            Point2d pnt2dRef = idAlign.getAlignRefPoint();
            string strFilter = "*";

            if (strSource.Contains("GCAL"))
                strFilter = "*CPNT*";

            ObjectIdCollection ids = Cogo.getNestedPoints(btrMS);

            List<POI> varPOI_PNTs = new List<POI>();

            Alignment objAlign = (Alignment)idAlign.getEnt();
            using (var tr = BaseObjs.startTransactionDb()) {
                foreach (ObjectId id in ids) {
                    CogoPoint cogoPnt = (CogoPoint)tr.GetObject(id, OpenMode.ForRead);

                    if (cogoPnt.Layer.Contains(strFilter)) {
                        try {
                            idAlign.getAlignStaOffset(cogoPnt.Location, ref dblStation, ref dblOffset);
                        }
                        catch (Autodesk.Civil.PointNotOnEntityException ) {
                            dblStation = 0.0;
                        }

                        if (dblStation != 0) {
                            if (dblOffset < dblToleranceLength) {
                                boolAdd = true;
                            }
                        }else {
                            Debug.Print(string.Format("{0},{1}", cogoPnt.Easting, cogoPnt.Northing));
                            Point2d pnt2d = new Point2d(cogoPnt.Easting, cogoPnt.Northing);

                            //distance from reference point < tolerance
                            if (pnt2d.getDistance(pnt2dRef) < dblToleranceLength) {
                                double dblAngle1 = Base_Tools45.Math.roundDown3(pnt2d.getDirection(pnt2dRef));

                                AlignmentEntity objAlignEnt = objAlign.Entities.EntityAtId(objAlign.Entities.FirstEntity);

                                switch (objAlignEnt.EntityType) {
                                    case AlignmentEntityType.Arc:

                                        AlignmentArc objAlignEntArc = (AlignmentArc)objAlignEnt;

                                        double dblangle2 = Base_Tools45.Math.roundDown3(objAlignEntArc.StartDirection);

                                        if (System.Math.Abs(dblAngle1 - dblangle2) < dblToleranceAngle) {
                                            boolAdd = true;
                                            dblStation = objAlign.StartingStation;
                                        }else if (System.Math.Abs(Base_Tools45.Math.roundDown3(dblAngle1 - 2 * PI) - dblangle2) < dblToleranceAngle) {
                                            boolAdd = true;
                                            dblStation = objAlign.StartingStation;
                                        }

                                        break;
                                    case AlignmentEntityType.Line:

                                        AlignmentLine objAlignEntTan = (AlignmentLine)objAlignEnt;

                                        dblangle2 = Base_Tools45.Math.roundDown3(objAlignEntTan.Direction);

                                        if (System.Math.Abs(dblAngle1 - dblangle2) < dblToleranceAngle) {
                                            boolAdd = true;
                                            dblStation = objAlign.StartingStation;
                                        }

                                        break;
                                }
                            }else {
                                boolAdd = false;
                            }
                            //4
                        }

                        if (boolAdd == true) {
                            POI vPOI_PNT = new POI();
                            vPOI_PNT.Station = Base_Tools45.Math.roundDown3(dblStation);

                            vPOI_PNT.PntNum = cogoPnt.PointNumber.ToString();
                            vPOI_PNT.Elevation = System.Math.Round(cogoPnt.Elevation, 3);

                            vPOI_PNT.OFFSET = System.Math.Round(dblOffset, 2);
                            vPOI_PNT.PntSource = strSource;

                            try {
                                objAlign.StationOffset(cogoPnt.Easting, cogoPnt.Northing, ref dblStation, ref dblOffset);
                            }
                            catch (Autodesk.Civil.PointNotOnEntityException ) {
                                dblStation = 0.0;
                            }

                            vPOI_PNT.OFFSET = System.Math.Round(dblOffset, 2);
                            vPOI_PNT.Desc0 = string.Format("{0} {1} {2}", cogoPnt.RawDescription, vPOI_PNT.OFFSET, vPOI_PNT.Elevation);
                            vPOI_PNT.PntSource = "TOPO";

                            varPOI_PNTs.Add(vPOI_PNT);
                        }
                    }
                }
                tr.Commit();
            }

            if (varPOI_PNTs.Count > 0) {
                dynamic sta = from data in varPOI_PNTs
                              orderby data.Station
                              select data;

                List<POI> t = new List<POI>();
                foreach (var p in sta) 
                    t.Add(p);

                varPOI_PNTs = t;

                Misc.removeDuplicatePoints(ref varPOI_PNTs);

                sta = from data in varPOI_PNTs
                      orderby data.Station
                      select data;

                t = new List<POI>();
                foreach (var p in sta)
                    t.Add(p);

                varPOI = t;

            }else {
                return false;
            }
            return true;
        }
    }
}