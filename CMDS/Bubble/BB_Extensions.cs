using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bubble
{
    public static class BB_Extensions
    {
        public static void
        activate<T>(this ObjectId id, Action<T> action) where T : DBObject
        {
            id.usngTransaction(
                tr =>
                {
                    var obj = (T)tr.GetObject(id, OpenMode.ForWrite);
                    action(obj);
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
            db.usngTransaction(
                tr =>
                {
                    var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    var tRec = (BlockTableRecord)tr.GetObject(bt[nameBlock], OpenMode.ForRead);
                    action(tr, tRec.Cast<ObjectId>());
                });
        }

        public static void
        usngTransaction(this ObjectId id, Action<Transaction> action)
        {
            using (var tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    action(tr);
                    tr.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                {
                    tr.Abort();
                }
            }
        }

        public static void
        usngTransaction(this Database db, Action<Transaction> action)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    action(tr);
                    tr.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                {
                    tr.Abort();
                }
            }
        }
    }
}
