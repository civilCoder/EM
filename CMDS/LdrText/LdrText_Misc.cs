using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using System;

namespace LdrText
{
    public static class LdrText_Misc
    {
        public static void
        updateXDataOnGSs(){
            ForEach<MText>(updateXDataOnGS);
        }

        private static void
        ForEach<T>(Action<T> action) where T : Entity
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    RXClass theClass = RXObject.GetClass(typeof(T));
                    foreach (ObjectId id in modelSpace)
                    {
                        if (id.ObjectClass.IsDerivedFrom(theClass))
                        {
                            try
                            {
                                var ent = (T)tr.GetObject(id, OpenMode.ForRead);
                                action(ent);
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " LdrText_Misc.cs: line: 37");
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LdrText_Misc.cs: line: 46");
            }
        }

        private static void
        updateXDataOnGS(MText mTxt)
        {
            ObjectId idMTxt = mTxt.ObjectId;

            ResultBuffer rb = idMTxt.getXData(apps.lnkGS);
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            ObjectId idLdr = tvs.getObjectId(3);
            
            Point3d pnt3dBeg = idLdr.getBegPnt();
            Point3d pnt3dEnd = idLdr.getEndPnt();

            ObjectId idLine = Draw.addLine(pnt3dBeg, pnt3dEnd);
            Line line = (Line)idLine.getEnt();

            ObjectId idPnt1 = tvs[9].Value.ToString().stringToHandle().getObjectId();
            ObjectId idPnt2 = tvs[10].Value.ToString().stringToHandle().getObjectId();

            Point3d pnt3d1, pnt3d2;
            if (idPnt1.IsValid)
                pnt3d1 = idPnt1.getCogoPntCoordinates();
            else
                pnt3d1 = Mod.stringCoordinateListToPoint3d(tvs[11]);

            if (idPnt2.IsValid)
                pnt3d2 = idPnt2.getCogoPntCoordinates();
            else
                pnt3d2 = Mod.stringCoordinateListToPoint3d(tvs[11]);

            double station = 0, offset = 0;

            Point3d pnt3dMTxtIns = idMTxt.getMTextLocation();
            Point3d pnt3dIns = line.GetClosestPointTo(pnt3dMTxtIns, false);

            idLine.delete();

            Geom.getStaOff(pnt3d1, pnt3d2, pnt3dIns, ref station, ref offset);

            tvs[5] = new TypedValue(1040, station);
            tvs[6] = new TypedValue(1040, offset);

            idMTxt.clearXData(apps.lnkGS);
            idMTxt.setXData(tvs, apps.lnkGS);
        }

    }
}
