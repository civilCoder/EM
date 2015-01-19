using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    public static class DrawDock
    {
        public static ObjectId
        build3dPolyDockApron(this List<Point3d> pnts3d, string nameSurface, string nameLayer, string nameFunction, out List<ObjectId> idsCgPnts)
        {
            idsCgPnts = null;
            Layer.manageLayers(nameLayer);
            ObjectId idPoly3d = ObjectId.Null;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idPoly3d = Draw.addPoly3d(pnts3d, nameLayer);

                    idsCgPnts = new List<ObjectId>();

                    TypedValue[] tvs = new TypedValue[pnts3d.Count];
                    tvs.SetValue(new TypedValue(1001, apps.lnkBrks), 0);

                    uint pntNum;
                    for (int i = 0; i < pnts3d.Count - 1; i++)
                    {
                        ObjectId idCogoPnt = pnts3d[i].setPoint(out pntNum, "CPNT-ON");
                        idsCgPnts.Add(idCogoPnt);
                        tvs.SetValue(new TypedValue(1005, idCogoPnt.getHandle()), i + 1);
                    }

                    idPoly3d.setXData(tvs, apps.lnkBrks);

                    tvs = new TypedValue[2];
                    tvs.SetValue(new TypedValue(1001, apps.lnkBrks), 0);
                    tvs.SetValue(new TypedValue(1005, idPoly3d.getHandle()), 1);

                    for (int i = 0; i < idsCgPnts.Count; i++)
                    {
                        idsCgPnts[i].setXData(tvs, apps.lnkBrks);
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Draw3d.cs: line: 105", ex.Message));
            }
            return idPoly3d;
        }

    }
}
