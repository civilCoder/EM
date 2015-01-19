using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Cogo
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="Btr"></param>
        /// <returns></returns>
        public static ObjectIdCollection
        getNestedPoints(BlockTableRecord Btr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId idObj in Btr)
                    {
                        DBObject dbObj = tr.GetObject(idObj, OpenMode.ForRead);
                        if (dbObj.GetType() == typeof(CogoPoint))
                        {
                            ids.Add(dbObj.ObjectId);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Cogo.cs: line: 33", ex.Message));
            }
            return ids;
        }
    }// Class Cogo
}