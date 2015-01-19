using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using pub = EW.EW_Pub;

namespace EW
{
    public static class EW_ModAdjacentAreas {
        static SelectionSet objSSet = null;

        public static void
        modAdjacentAreas_Subtract(ObjectId idBldgOX2, bool boolFirstPass) {
            if (pub.boolDebug) {
                BaseObjs.write("Select _XX- Area");
                objSSet = EW_Utility1.buildSSet20();   //select on screen
                
                boolFirstPass = true;
            }else {
                if (boolFirstPass) {
                    objSSet = EW_Utility1.buildSSet0();                                //get _XX-*                    
                }else {
                    objSSet = EW_Utility1.buildSSet19();                                //get OX-AREAS-2d - select all
                }
            }

            ObjectId[] ids = objSSet.GetObjectIds();

            ObjectId idAreaCopy = ObjectId.Null;

            List<ObjectId> idsElemOX2 = new List<ObjectId>();
            List<ObjectId> idsElemArea = new List<ObjectId>();

            for (int i = 0; i < ids.Length; i++) {
                ObjectId id = ids[i];

                Polyline objArea = (Polyline)id.getEnt();
                objArea.checkIfClosed();

                string strLayer = objArea.Layer;

                switch (strLayer) {
                    case "_XX-FLOOR SLAB_A":
                    case "_XX-FLOOR SLAB_B":
                    case "_XX-FLOOR SLAB_C":
                    case "_XX-OFFICE SLAB AND SAND":
                        //, "_XX-BUILDING ADJACENT LANDSCAPING"

                        break;
                    default:

                        if (boolFirstPass) {
                            idAreaCopy = Conv.processBndry(objArea.copy());
                            idAreaCopy.changeProp(clr.byl, "OX-AREAS-2d");
                        }else {
                            idAreaCopy = id;
                        }

                        if (pub.boolDebug) {
                            idAreaCopy.changeProp(LineWeight.LineWeight050, clr.yel);
                        }

                        List<Point3d> varPntsInt = idBldgOX2.intersectWith(idAreaCopy, extend.none);
                        
                        if (varPntsInt.Count == 0) {
                            TypedValue[] tvs = new TypedValue[2] {
                                new TypedValue(1001, "OX-Layer"),
                                new TypedValue(1000, strLayer)
                            };

                            idAreaCopy.setXData(tvs, "OX-Layer");
                        }else {
                            idsElemOX2.Add(idBldgOX2);

                            ObjectId idRegionOX2 = idBldgOX2.addRegion();
                            idRegionOX2.changeProp(LineWeight.LineWeight050, clr.c180);
                            idAreaCopy.checkIfClosed();
                            idAreaCopy.changeProp(LineWeight.LineWeight200, clr.cyn);

                            ObjectId idRegionArea = idAreaCopy.addRegion();
                            idRegionArea.changeProp(LineWeight.LineWeight050, clr.blu);

                            Region objRegionOX2 = (Region)idBldgOX2.getEnt();
                            Region objRegionArea = (Region)idRegionArea.getEnt();

                            objRegionArea.BooleanOperation(BooleanOperationType.BoolSubtract, objRegionOX2);                           

                            if (objRegionArea.Area > 0) {
                                DBObjectCollection varExplodeObjs = new DBObjectCollection();
                                objRegionArea.Explode(varExplodeObjs);

                                Entity objEnt = (Entity)varExplodeObjs[0];

                                if (objEnt is Region) {
                                    idRegionArea.delete();

                                    for (int j = 0; j < varExplodeObjs.Count; j++) {
                                        Region objRegion = (Region)varExplodeObjs[j];
                                        DBObjectCollection varEnts = new DBObjectCollection(); 
                                        objRegion.Explode(varEnts);
                                        objRegion.ObjectId.delete();
                                        idAreaCopy = varEnts.rebuildPoly();
                                        idAreaCopy.changeProp(clr.byl, "OX-AREAS-2d");

                                        TypedValue[] tvs = new TypedValue[3] {
                                            new TypedValue(1001, "OX-Layer"),
                                            new TypedValue(1000, strLayer),
                                            new TypedValue(1000, "remaining after subtraction of OX")
                                        };

                                        idAreaCopy.setXData(tvs, "OX-Layer");
                                    }
                                }else {
                                    idRegionArea.delete();
                                    idAreaCopy = varExplodeObjs.rebuildPoly();
                                    idAreaCopy.changeProp(clr.byl, "OX-AREAS-2d");

                                    TypedValue[] tvs = new TypedValue[3] {
                                        new TypedValue(1001, "OX-Layer"),
                                        new TypedValue(1000, strLayer),
                                        new TypedValue(1000, "remaining after subtraction of OX")
                                    };

                                    idAreaCopy.setXData(tvs, "OX-Layer");
                                }
                            }else {
                            }
                        }
                        break;
                }
            }
            return;
        }

        public static ObjectId
        modAdjacentOX_Intersect(ObjectId idBldgOX, ObjectId idGradingLimOffset)
        {

            idBldgOX.changeProp(LineWeight.LineWeight035, clr.c140);
            ObjectId idRegionOX = idBldgOX.addRegion();

            idRegionOX.changeProp(LineWeight.LineWeight050, clr.c180);

            idGradingLimOffset.changeProp(LineWeight.LineWeight200, clr.cyn);

            ObjectId idRegionGradingLim = idGradingLimOffset.addRegion();
            idRegionGradingLim.changeProp(LineWeight.LineWeight035, clr.blu);

            Region objRegionOX = (Region)idRegionOX.getEnt();
            Region objRegionGradingLim = (Region)idRegionGradingLim.getEnt();

            objRegionOX.BooleanOperation(BooleanOperationType.BoolIntersect, objRegionGradingLim);

            string strLayer = "OX-AREAS-2d";

            if (objRegionOX.Area > 0)
            {
                DBObjectCollection varExplodeObjs = new DBObjectCollection();
                objRegionOX.Explode(varExplodeObjs);

                Entity objEnt = (Entity)varExplodeObjs[0];

                if (objEnt is Region) {

                    for (int j = 0; j < varExplodeObjs.Count; j++) {
                        Region objRegion = (Region)varExplodeObjs[j];
                        DBObjectCollection varEnts = new DBObjectCollection(); 
                        objRegion.Explode(varEnts);
                        objRegion.ObjectId.delete();
                        idBldgOX = varEnts.rebuildPoly();
                        idBldgOX.changeProp(clr.byl, "OX-AREAS-2d");

                        TypedValue[] tvs = new TypedValue[2] {
                            new TypedValue(1001, "OX-Layer"),
                            new TypedValue(1000, strLayer)
                        };

                        idBldgOX.setXData(tvs, "OX-Layer");
                    }
                }else {
                    objRegionOX.ObjectId.delete();

                    idBldgOX = varExplodeObjs.rebuildPoly();
                    idBldgOX.changeProp(clr.byl, "OX-AREAS-2d");

                    TypedValue[] tvs = new TypedValue[2] {
                        new TypedValue(1001, "OX-Layer"),
                        new TypedValue(1000, strLayer)
                    };

                    idBldgOX.setXData(tvs, "OX-Layer");
                }

            }

            return idBldgOX;

        }
    }
}
