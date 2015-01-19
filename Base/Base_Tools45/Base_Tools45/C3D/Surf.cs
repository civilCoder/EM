using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Surf
    {
        public static TinSurface
        addTinSurface(string nameSurf, out bool exists)
        {
            TinSurface surfTin = null;
            Surface surf = null;
            exists = false;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection ids = BaseObjs._civDoc.GetSurfaceIds();
                    foreach (ObjectId id in ids)
                    {
                        surf = (Surface)tr.GetObject(id, OpenMode.ForRead);
                        if (surf is TinSurface)
                        {
                            surfTin = (TinSurface)surf;
                            if (surfTin.Name == nameSurf)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                    if (!exists)
                    {
                        ObjectId idSurf = TinSurface.Create(BaseObjs._db, nameSurf);
                        surfTin = (TinSurface)tr.GetObject(idSurf, OpenMode.ForWrite);
                        surfTin.Layer = string.Format("{0}-SURFACE", nameSurf);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 32", ex.Message));
            }
            return surfTin;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public static ObjectId
        getSurface(string strName, out bool exists)
        {
            ObjectId idSurface = ObjectId.Null;
            exists = false;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection surfaceIDs = BaseObjs._civDoc.GetSurfaceIds();

                    foreach (ObjectId surfaceID in surfaceIDs)
                    {
                        var objSurface = (Autodesk.Civil.DatabaseServices.Surface)surfaceID.GetObject(OpenMode.ForRead);
                        if (objSurface.Name == strName)
                        {
                            idSurface = surfaceID;
                            exists = true;
                            tr.Commit();
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 63", ex.Message));
            }
            return idSurface;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static List<string>
        getSurfaces()
        {
            List<string> surfaces = new List<string>();
            TinSurface surface = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection ids = BaseObjs._civDoc.GetSurfaceIds();
                    if (ids == null || ids.Count == 0)
                        return null;
                    foreach (ObjectId id in ids)
                    {
                        try
                        {
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                            if (ent is TinSurface)
                            {
                                surface = (TinSurface)ent;
                                surfaces.Add(surface.Name);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 90", ex.Message));
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 97", ex.Message));
            }
            return surfaces;
        }

        public static TinSurface
        getTinSurface(string nameSurf)
        {
            TinSurface objSurface = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection surfaceIDs = BaseObjs._civDoc.GetSurfaceIds();

                    foreach (ObjectId surfaceID in surfaceIDs)
                    {
                        Autodesk.Civil.DatabaseServices.TinSurface objTinSurface = (TinSurface)surfaceID.GetObject(OpenMode.ForRead);
                        if (objTinSurface.Name == nameSurf)
                        {
                            objSurface = objTinSurface;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {               
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 143", ex.Message));
            }
            return objSurface;
        }
        public static TinVolumeSurface
        getTinVolumeSurface(string nameSurf)
        {
            TinVolumeSurface objSurface = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection idsSurface = BaseObjs._civDoc.GetSurfaceIds();

                    foreach (ObjectId idSurface in idsSurface)
                    {
                        Autodesk.Civil.DatabaseServices.Surface surf = (Autodesk.Civil.DatabaseServices.Surface)tr.GetObject(idSurface, OpenMode.ForRead);
                        if(surf is TinVolumeSurface){
                            Autodesk.Civil.DatabaseServices.TinVolumeSurface objTinSurface = (TinVolumeSurface)surf;
                            if (objTinSurface.Name == nameSurf)
                            {
                                objSurface = objTinSurface;
                            }                            
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 189", ex.Message));
            }
            return objSurface;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="nameSurf"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public static TinSurface
        getTinSurface(string nameSurf, out bool exists)
        {
            TinSurface objSurface = null;
            ObjectId idStyle = ObjectId.Null;
            exists = false;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection surfaceIDs = BaseObjs._civDoc.GetSurfaceIds();

                    foreach (ObjectId surfaceID in surfaceIDs)
                    {
                        Autodesk.Civil.DatabaseServices.TinSurface objTinSurface = (TinSurface)surfaceID.GetObject(OpenMode.ForRead);
                        if (objTinSurface.Name == nameSurf)
                        {
                            objSurface = objTinSurface;
                            exists = true;
                            tr.Commit();
                            return objSurface;
                        }
                    }
                    foreach (ObjectId idSty in BaseObjs._civDoc.Styles.SurfaceStyles)
                    {
                        SurfaceStyle style = (SurfaceStyle)tr.GetObject(idSty, OpenMode.ForRead);
                        if (style.Name == nameSurf)
                        {
                            idStyle = idSty;
                            break;
                        }
                    }
                    if (idStyle == ObjectId.Null)
                        idStyle = BaseObjs._civDoc.Styles.SurfaceStyles[0];
                    ObjectId idSurf = TinSurface.Create(nameSurf, idStyle);
                    objSurface = (TinSurface)tr.GetObject(idSurf, OpenMode.ForWrite);
                    Layer.manageLayers(string.Format("{0}_SURFACE", nameSurf));
                    objSurface.Layer = string.Format("{0}_SURFACE", nameSurf);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 143", ex.Message));
            }
            return objSurface;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public static void
        removeSurface(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        TinSurface surface = getTinSurface(name);
                        if(surface != null){
                            surface.UpgradeOpen();
                            surface.Erase();                            
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 168", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf.cs: line: 174", ex.Message));
            }
        }

        public static Entity[]
        getSurfaceBoundaries(this ObjectId idSurf)
        {
            Entity[] ents = null;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                TinSurface surf = (TinSurface)tr.GetObject(idSurf, OpenMode.ForRead);
            }

            return ents;
        }
    }
}