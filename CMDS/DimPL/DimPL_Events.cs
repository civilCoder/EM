using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;

namespace DimPL
{
    /// <summary>
    ///
    /// </summary>
    public static class DimPL_Events
    {
        private static Point3d _txtIns;

        private static Point3d _ldrEndPoint3D;

        private static string _H;

        private static ResultBuffer _RB;

        private static ObjectId _idTxt;

        public static Point3d TxtIns
        {
            get
            {
                return _txtIns;
            }
            set
            {
                _txtIns = value;
            }
        }

        public static Point3d LdrEndPoint3D
        {
            get
            {
                return _ldrEndPoint3D;
            }
            set
            {
                _ldrEndPoint3D = value;
            }
        }

        public static string H
        {
            get
            {
                return _H;
            }
            set
            {
                _H = value;
            }
        }

        public static ResultBuffer RB
        {
            get
            {
                return _RB;
            }
            set
            {
                _RB = value;
            }
        }

        public static ObjectId idTxt
        {
            get
            {
                return _idTxt;
            }
            set
            {
                _idTxt = value;
            }
        }

        public static void
        activateObj(this ObjectId id)
        {
            id.activate<DBObject>(
                obj =>
                {
                    obj.Modified += new EventHandler(obj_Modified);
                });
        }

        public static void
        deactivateObj(this ObjectId id)
        {
            id.activate<DBObject>(
                Obj =>
                {
                    Obj.Modified -= new EventHandler(obj_Modified);
                });
        }

        public static void
        InitDictLnkDimPL(string action)
        {
            //Dict.deleteDictionary(lnkDimPL);
            bool exists = false;
            ObjectId idDictDimPL = Dict.getNamedDictionary(apps.lnkDimPL, out exists);
            if (exists)
            {
                List<DBDictionaryEntry> dictEntries = idDictDimPL.getEntries();
                foreach (DBDictionaryEntry entry in dictEntries)
                {
                    ObjectId idTX = entry.Key.stringToHandle().getObjectId();
                    if (idTX.IsValid)
                    {
                        switch (action)
                        {
                            case "ACTIVATE":
                                idTX.activateObj();
                                break;

                            case "DEACTIVATE":
                                idTX.deactivateObj();
                                break;
                        }
                    }
                }
            }
        }

        private static void
        obj_Modified(object sender, EventArgs e)
        {
            DBObject dbObj = (DBObject)sender;
            idTxt = dbObj.ObjectId;
            //Database db = HostApplicationServices.WorkingDatabase;
            //using (Transaction tr = db.TransactionManager.StartTransaction())
            //{
            //    if (idTxt.IsErased)
            //    {
            //        DBObject dbO = tr.GetObject(idTxt, OpenMode.ForWrite, true);
            //        dbO.Erase(false);
            //        H = dbO.Handle.ToString();
            //        RB = dbO.GetXDataForApplication(lnkDimPL);
            //        dbO.Erase();
            //    }
            //    tr.Commit();
            //}
        }
    }// class DimPL_Events
}