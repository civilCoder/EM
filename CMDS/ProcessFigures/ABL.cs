using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;

namespace ProcessFigures
{
    public static class ABL
    {
        public static void
        addBreakline()
        {
            addBreaklines2Surface();
        }

        public static void
        addBreaklines2Surface(ObjectId[] ids = null, string nameSurface = null)
        {
            SelectionSet ss = null;
            if (ids == null)
            {
                TypedValue[] tvs = new TypedValue[1];
                tvs[0] = new TypedValue((int)DxfCode.Start, "POLYLINE");

                ss = Select.buildSSetBase(tvs, false);

                if (ss.Count > 0)
                {
                    ids = ss.GetObjectIds();
                }

                ObjectId idTinSurf = ObjectId.Null;
                bool exists;
                if (nameSurface == null)
                {
                    string nameDwg = BaseObjs.docName;
                    if (nameDwg.Contains("CGP"))
                    {
                        idTinSurf = Surf.getSurface("CPNT-ON", out exists);
                        nameSurface = "CPNT-ON";
                    }
                    else if (nameDwg.Contains("GCAL"))
                    {
                        idTinSurf = Surf.getSurface("CPNT-ON", out exists);
                        nameSurface = "CPNT-ON";
                    }
                    else if (nameDwg.Contains("MASS"))
                    {
                        idTinSurf = Surf.getSurface("CPNT-ON", out exists);
                        nameSurface = "CPNT-ON";
                    }
                    else if (nameDwg.Contains("CONT"))
                    {
                        idTinSurf = Surf.getSurface("EXIST", out exists);
                        nameSurface = "EXIST";
                    }
                }
                else
                {
                    idTinSurf = Surf.getSurface(nameSurface, out exists);
                }

                ObjectIdCollection idColl = new ObjectIdCollection();
                foreach (ObjectId id in ids)
                {
                    idColl.Add(id);
                }

                TinSurface tinSurf = (TinSurface)idTinSurf.getEnt();
                tinSurf.BreaklinesDefinition.AddStandardBreaklines(idColl, 1.0, 0, 0, 0);
            }
        }
    }
}