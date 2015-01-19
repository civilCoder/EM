using Autodesk.AutoCAD.DatabaseServices;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class LineType
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static LinetypeTable
        getLineTypeTable()
        {
            LinetypeTable LTT = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LTT = (LinetypeTable)tr.GetObject(DB.LinetypeTableId, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LineType.cs: line: 30");
            }
            return LTT;
        }

        /// <summary>
        /// get LinetypeTableRecord
        /// </summary>
        /// <param name="name"></param>
        /// <returns>LinetypeTableRecord</returns>
        public static LinetypeTableRecord
        getLinetypeTableRecord(string name)
        {
            LinetypeTableRecord LTtr = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LinetypeTable LTT = getLineTypeTable();
                    if (LTT.Has(name) == true)
                    {
                        LTtr = (LinetypeTableRecord)tr.GetObject(LTT[name], OpenMode.ForRead);
                    }
                    else
                    {
                        BaseObjs._db.LoadLineTypeFile(name, "ACAD.LIN");
                        LTtr = (LinetypeTableRecord)tr.GetObject(LTT[name], OpenMode.ForRead);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LineType.cs: line: 63");
            }
            return LTtr;
        }
    }
}
