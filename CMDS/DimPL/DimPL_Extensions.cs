using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimPL
{
    public static class DimPL_Extensions
    {
        public static void
        activate<T>(this ObjectId id, Action<T> action) where T : DBObject
        {
            id.usingTransaction(
                tr =>
                {
                    var ent = (T)tr.GetObject(id, OpenMode.ForWrite);
                    action(ent);
                });
        }

        public static void
        usingModelSpace(this Database db, Action<Transaction, IEnumerable<ObjectId>> action)
        {
            db.usingBlockTable(BlockTableRecord.ModelSpace, action);
        }

        public static void
        usingBlockTable(this Database db, string nameBlock, Action<Transaction, IEnumerable<ObjectId>> action)
        {
            db.usingTransaction(
                tr =>
                {
                    var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    var tRec = (BlockTableRecord)tr.GetObject(bt[nameBlock], OpenMode.ForRead);
                    action(tr, tRec.Cast<ObjectId>());
                });
        }

        public static void
        usingTransaction(this ObjectId id, Action<Transaction> action)
        {
            using (var tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    action(tr);
                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} DimPL_Extensions.cs: line: 42", ex.Message));
                }
            }
        }

        public static void
        usingTransaction(this Database db, Action<Transaction> action)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    action(tr);
                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} DimPL_Extensions.cs: line: 55", ex.Message));
                }
            }
        }
    }
}