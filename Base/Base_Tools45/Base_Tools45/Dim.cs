using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Collections.Generic;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Dim
    {
        public static ObjectId
        addDimRotated(Point3d pnt3d1, Point3d pnt3d2, Point3d pnt3dX, double rotation, ObjectId idStyle, string nameLayer, double dblTxtHeight, int precision)
        {
            ObjectId idDim = ObjectId.Null;
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    RotatedDimension rotDim = new RotatedDimension();

                    rotDim.XLine1Point = pnt3d1;
                    rotDim.XLine2Point = pnt3d2;
                    rotDim.DimLinePoint = pnt3dX;
                    rotDim.DimensionStyle = idStyle;
                    rotDim.Annotative = AnnotativeStates.True;
                    rotDim.Dimasz = rotation;
                    try
                    {
                        rotDim.Layer = nameLayer;
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dim.cs: line: 38");
                    }
                    rotDim.Dimtxt = dblTxtHeight;
                    rotDim.Dimdec = precision;

                    idDim = MS.AppendEntity(rotDim);
                    tr.AddNewlyCreatedDBObject(rotDim, true);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dim.cs: line: 51");
            }
            return idDim;
        }

        public static void
        addDims(Point3d varPntLower, double dblStaStart, double dblElevStart, List<POI> varpoi, string nameLayer, double dblScaleProfileView, string txt = "")
        {
            Point3d dblPnt1 = Pub.pnt3dO, dblPnt2 = Pub.pnt3dO, dblPntX = Pub.pnt3dO;
            double dblMaxY = 0;
            ObjectId idDim = ObjectId.Null;
            ObjectId idDimStyle = ObjectId.Null;

            for (int i = 1; i < varpoi.Count; i++)
            {
                dblPnt1 = new Point3d(varPntLower.X + (varpoi[i - 1].Station - dblStaStart), varPntLower.Y + (varpoi[i - 1].Elevation - dblElevStart) * dblScaleProfileView, 0);
                dblPnt2 = new Point3d(varPntLower.X + (varpoi[i - 0].Station - dblStaStart), varPntLower.Y + (varpoi[i - 0].Elevation - dblElevStart) * dblScaleProfileView, 0);

                if ((txt == ""))
                {
                    dblPntX = new Point3d(dblPnt1.X + (dblPnt2.X - dblPnt1.X) / 2, dblPnt1.Y + (dblPnt2.Y - dblPnt1.Y) / 2 + 10, 0);
                }
                else
                {
                    if (dblPnt1.Y > dblPnt2.Y)
                    {
                        dblMaxY = dblPnt1.Y;
                    }
                    else
                    {
                        dblMaxY = dblPnt2.Y;
                    }
                    dblPntX = new Point3d(dblPnt1.X + (dblPnt2.X - dblPnt1.X) / 2, dblMaxY + 10, 0);
                }

                idDimStyle = Dim.getDimStyleTableRecord("Annotative");
                idDim = Dim.addDimRotated(dblPnt1, dblPnt2, dblPntX, 0.0, idDimStyle, nameLayer, .09, 3);
            }

            if (txt != "")
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    RotatedDimension rotDim = (RotatedDimension)tr.GetObject(idDim, OpenMode.ForWrite);
                    rotDim.DimensionText = txt;
                    rotDim.Dimexo = 1.0;
                    rotDim.Dimgap = 0.1;
                    tr.Commit();
                }
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DimStyleTable
        getDimStyleTable()
        {
            DimStyleTable dst = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    dst = (DimStyleTable)DB.DimStyleTableId.GetObject(OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dim.cs: line: 123");
            }
            return dst;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectId
        getDimStyleTableRecord(string name)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DimStyleTable dst = (DimStyleTable)HostApplicationServices.WorkingDatabase.DimStyleTableId.GetObject(OpenMode.ForRead);
                    if (dst.Has(name) == true)
                    {
                        //dst.UpgradeOpen();
                        DimStyleTableRecord dstr = (DimStyleTableRecord)dst[name].GetObject(OpenMode.ForRead);
                        id = dstr.ObjectId;
                    }
                    else
                    {
                        dst.UpgradeOpen();
                        DimStyleTableRecord dstr = new DimStyleTableRecord();
                        dstr.Name = name;
                        dstr.Annotative = AnnotativeStates.True;
                        id = dst.Add(dstr);
                        tr.AddNewlyCreatedDBObject(dstr, true);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dim.cs: line: 162");
            }
            return id;
        }
    }
}
