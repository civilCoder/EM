using System;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;

namespace Grading {
    public static class Grading_Extensions {
        public static void
        addEvent(this ObjectId id) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction tr = BaseObjs.startTransactionDb()) {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    if (ent is Polyline3d) {
                        Grading_Events.activatePoly3ds("", id);
                    }else if (ent is CogoPoint) {
                        Grading_Events.activateCogoPnts((CogoPoint)ent);
                    }
                    tr.Commit();
                }
            }
        }
    }
}
