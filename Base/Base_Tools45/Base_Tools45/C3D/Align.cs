using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    public static class Align
    {
        public static Alignment
        addAlignmentFromPoly(string nameAlign, string nameLayer, ObjectId idPoly, string nameStyle, string nameLabelStyle, bool eraseEx, bool addCurves = false)
        {
            Alignment align = null;
            PolylineOptions opts = new PolylineOptions();
            opts.AddCurvesBetweenTangents = addCurves;
            opts.EraseExistingEntities = eraseEx;
            opts.PlineId = idPoly;
            Layer.manageLayers(nameLayer);
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        ObjectId id = Alignment.Create(BaseObjs._civDoc, opts, nameAlign, null, nameLayer, nameStyle, nameLabelStyle);
                        align = (Alignment)tr.GetObject(id, OpenMode.ForWrite);
                        align.ReferencePointStation = 1000;
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 35", ex.Message));
            }
            return align;
        }

        public static Alignment
        getAlignment(string name)
        {
            Alignment align = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectIdCollection alignIDs = CivilApplication.ActiveDocument.GetAlignmentIds();
                    foreach (ObjectId alignID in alignIDs)
                    {
                        Alignment a = (Alignment)tr.GetObject(alignID, OpenMode.ForRead);
                        if (a.Name == name)
                        {
                            align = a;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 56", ex.Message));
            }
            return align;
        }

        public static Alignment
        getAlignment(ObjectId id)
        {
            Alignment align = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    align = (Alignment)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 71", ex.Message));
            }
            return align;
        }

        public static ObjectId
        getAlignmentID(string name)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectIdCollection alignIDs = CivilApplication.ActiveDocument.GetAlignmentIds();
                    foreach (ObjectId alignID in alignIDs)
                    {
                        Alignment a = (Alignment)tr.GetObject(alignID, OpenMode.ForRead);
                        if (a.Name == name)
                        {
                            id = alignID;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 93", ex.Message));
            }
            return id;
        }

        public static ObjectIdCollection
        getAlignmentIDs()
        {
            ObjectIdCollection ids = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ids = CivilApplication.ActiveDocument.GetAlignmentIds();
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 108", ex.Message));
            }
            return ids;
        }

        public static ObjectId
        getAlignmentPoly(ObjectId idAlign)
        {
            Alignment align = (Alignment)idAlign.GetObject(OpenMode.ForRead);
            ObjectId idPoly = align.GetPolyline();
            return idPoly;
        }

        public static Polyline
        getAlignmentPoly(Alignment align)
        {
            ObjectId id = align.GetPolyline();
            Polyline poly = (Polyline)id.GetObject(OpenMode.ForRead);
            return poly;
        }

        public static string
        getAlignName(this ObjectId idAlign)
        {
            Alignment align = (Alignment)idAlign.getEnt();
            return align.Name;
        }

        public static string
        getAlignName(string nameLayer)
        {
            int number;

            string strAlignNumb;

            ObjectIdCollection alignIDs = getAlignmentIDs();

            if (alignIDs.Count == 0)
            {
                return string.Format("{0}01", nameLayer);
            }

            List<string> alignNames = new List<string>();
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                foreach (ObjectId alignID in alignIDs)
                {
                    //if(alignID.IsValid && !alignID.IsEffectivelyErased && !alignID.IsErased){
                    //    align = (Alignment)tr.GetObject(alignID, OpenMode.ForRead);
                    //    alignNames.Add(align.Name);
                    //}
                    alignNames.Add(Align.getAlignName(alignID));
                }
            }

            List<string> alignNumbs = new List<string>();
            List<string> alignNamesX = new List<string>();

            foreach (string name in alignNames)
            {
                if (name.Contains(nameLayer) == true)
                {
                    bool result = true;
                    int j = name.Length;

                    while (result == true)
                    {
                        j = j - 1;
                        string strChar = name.Substring(j, 1);

                        result = Int32.TryParse(strChar, out number);
                    }

                    if (j != name.Length)
                    {
                        strAlignNumb = name.Substring(j + 1);

                        result = Int32.TryParse(strAlignNumb, out number);
                        if (result == true)
                        {
                            alignNumbs.Add(number.ToString());
                        }
                    }
                    else
                    {
                        alignNumbs.Add("");
                    }

                    alignNamesX.Add(name.Substring(0, j + 1));
                }
            }

            int k = alignNumbs.Count;
            if (k == 0)
            {
                return string.Format("{0}1", nameLayer);
            }
            else if (k == 1)
            {
                if (alignNumbs[0] != "")
                {
                    bool result = Int32.TryParse(alignNumbs[0], out number);
                    if (result == true)
                    {
                        number = number + 1;
                    }
                    else
                    {
                        number = 0;
                    }
                    return string.Format("{0}{1}", alignNamesX[0].Trim(), number.ToString());
                }
            }
            else
            {
                alignNumbs.Sort();
                return string.Format("{0}{1}", alignNamesX[0].Trim(), string.Format("{0}{1}", alignNumbs[alignNumbs.Count - 1], 1).ToString());
            }
            return null;
        }

        public static string
        getAlignIndex(string nameBase)
        {
            List<string> alignNumbs = new List<string>();
            List<string> alignNames = new List<string>();
            List<string> alignNamesX = new List<string>();

            int j, k;

            ObjectIdCollection alignIDs = getAlignmentIDs();

            if (alignIDs.Count == 0)
            {
                if (nameBase.Substring(3, 4).ToUpper() == "LINE")
                    return "A";
                else if (nameBase.Substring(3, 3).ToUpper() == "LAT")
                    return "01";
                else
                    return "";
            }

            foreach (ObjectId alignID in alignIDs)
            {
                Alignment align = (Alignment)alignID.GetObject(OpenMode.ForRead);
                alignNames.Add(align.Name);
            }

            if (nameBase.Substring(3, 4) == "LINE")
            {
                alignNames.Sort();
                string index = alignNames[alignNames.Count - 1];
                index = index.Substring(index.Length - 1);
                index = Txt.incrementString(index);
                return index;
            }
            else if (nameBase.Substring(3, 3) == "LAT")
            {
                int number;

                foreach (string name in alignNames)
                {
                    if (name.Contains(nameBase) == true)
                    {
                        bool result = true;
                        j = name.Length + 1;

                        while (result == true)
                        {
                            j -= 1;
                            string strChar = name.Substring(j, 1);
                            result = Int32.TryParse(strChar, out number);
                        }

                        if (j != name.Length)
                        {
                            string strAlignNumb = name.Substring(j + 1);

                            result = Int32.TryParse(strAlignNumb, out number);
                            if (result == true)
                            {
                                alignNumbs.Add(number.ToString());
                            }
                        }
                        else
                        {
                            alignNumbs.Add("");
                        }

                        alignNamesX.Add(name.Substring(1, j));
                    }
                }

                k = alignNumbs.Count;
                if (k == 0)
                {
                    return "01";
                }
                else if (k == 1)
                {
                    if (alignNumbs[0] != "")
                    {
                        bool result = Int32.TryParse(alignNumbs[0], out number);
                        if (result == true)
                        {
                            number = number + 1;
                        }
                        else
                        {
                            number = 0;
                        }
                        return number.ToString();
                    }
                }
                else
                {
                    alignNumbs.Sort();
                    return alignNumbs[alignNumbs.Count - 1].ToString();
                }
            }

            return null;
        }

        public static Point2d
        getAlignRefPoint(this ObjectId idAlign){
            Alignment align = getAlignment(idAlign);
            return align.ReferencePoint;            
        }

        public static void
        getAlignStaOffset(this ObjectId idAlign, Point3d pnt3d, ref double station, ref double offset)
        {
            Alignment align = getAlignment(idAlign);
            align.StationOffset(pnt3d.X, pnt3d.Y, ref station, ref offset);
        }

        public static void
        getAlignPointLoc(this ObjectId idAlign, double station, double offset, ref double easting, ref double northing)
        {
            Alignment align = getAlignment(idAlign);
            align.PointLocation(station, offset, ref easting, ref northing);
        }

        public static void
        removeAlignment(string nameAlign)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = getAlignmentID(nameAlign);
                    if (id != ObjectId.Null)
                    {
                        Alignment align = (Alignment)tr.GetObject(id, OpenMode.ForWrite);
                        align.Erase();
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 223", ex.Message));
            }
        }

        public static ObjectId
        selectAlign(string message, string reject, out Point3d pnt3dPicked, out string nameAlign)
        {
            ObjectId idAlign = ObjectId.Null;

            BaseObjs.acadActivate();
            PromptEntityOptions peos = new PromptEntityOptions(message);
            peos.SetRejectMessage(reject);
            peos.AddAllowedClass(typeof(Alignment), false);
            peos.AllowNone = false;
            nameAlign = string.Empty;

            PromptEntityResult per = BaseObjs._editor.GetEntity(peos);
            pnt3dPicked = per.PickedPoint;

            switch (per.Status)
            {
                case PromptStatus.Cancel:
                case PromptStatus.Error:
                    break;

                case PromptStatus.OK:

                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            Alignment align = (Alignment)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                            nameAlign = align.Name;
                            idAlign = align.ObjectId;
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align.cs: line: 256", ex.Message));
                    }
                    break;
            }
            return idAlign;
        }
    }// Class Civil
}