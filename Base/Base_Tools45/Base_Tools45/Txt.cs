using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static partial class Txt
    {
        /// <summary>
        ///
        /// </summary>
        public const double PI = System.Math.PI;

        public static ObjectId
        addMText(string strCallout, Point3d pnt3d1, double dblRotation, double width, double txtSize = 0.09,
            AttachmentPoint attachPnt = AttachmentPoint.MiddleCenter, string nameStyle = "Annotative",
            string nameLayer = "TEXT", Color color = null, string justify = null,
            AnnotativeStates annoState = AnnotativeStates.True,
            double xFactor = 0.8, bool bold = false,
            bool backgroundFill = false)
        {
            ObjectId idMtxt = ObjectId.Null;
            ObjectId idTxtStyle = ObjectId.Null;
            Database DB = BaseObjs._db;
            MText mtext = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord MS = (BlockTableRecord)tr.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    TextStyleTable TST = (TextStyleTable)tr.GetObject(DB.TextStyleTableId, OpenMode.ForRead);

                    if (!TST.Has(nameStyle))
                    {
                        idTxtStyle = makeTextStyle(nameStyle, annoState);
                    }
                    else
                    {
                        idTxtStyle = TST[nameStyle];
                        setTextStyleXScale(idTxtStyle, xFactor);
                    }

                    mtext = new MText();
                    mtext.SetDatabaseDefaults();

                    try
                    {
                        mtext.Layer = nameLayer;
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 61");
                    }

                    if (color == null)
                    {
                        color = new Color();
                        color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    }

                    mtext.Color = color;
                    mtext.Location = pnt3d1;

                    mtext.Rotation = dblRotation;
                    mtext.Annotative = annoState;
                    mtext.Width = width;
                    mtext.TextStyleId = idTxtStyle;

                    mtext.TextHeight = txtSize * Base_Tools45.Misc.getCurrAnnoScale();
                    mtext.BackgroundFill = backgroundFill;
                    if (backgroundFill)
                    {
                        mtext.BackgroundFillColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 1);
                        mtext.UseBackgroundColor = true;
                        mtext.BackgroundScaleFactor = 1.25;
                    }

                    if (justify == null)
                        justify = Pub.JUSTIFYLEFT;

                    string xScale = xFactor.ToString();

                    if (bold && xFactor != 0.8)
                        mtext.Contents = string.Format("{0}{{\\fromans.shx|b1|\\W{1};{2}}}", justify, xScale, strCallout);
                    else if (bold)
                        mtext.Contents = string.Format("{0}{{\\fromans.shx|b1|;{1}}}", justify, strCallout);
                    else
                        mtext.Contents = string.Format("{0}{{\\fromans.shx\\W{1};{2}}}", justify, xScale, strCallout);

                    mtext.Attachment = attachPnt;
                    MS.AppendEntity(mtext);
                    tr.AddNewlyCreatedDBObject(mtext, true);
                    idMtxt = mtext.ObjectId;
                    tr.Commit();
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 108");
            }
            return idMtxt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="pnt3d"></param>
        /// <param name="angle"></param>
        /// <param name="attachPoint"></param>
        /// <param name="nameStyle"></param>
        /// <returns>ObjectId</returns>
        public static ObjectId
        addText(string contents, Point3d pnt3d, double angle, AttachmentPoint attachPoint,
            string nameStyle = "Annotative", AnnotativeStates annoState = AnnotativeStates.True)
        {
            ObjectId idTxt = ObjectId.Null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    TextStyleTable tst = (TextStyleTable)tr.GetObject(DB.TextStyleTableId, OpenMode.ForRead);

                    ObjectId idTxtStyle = ObjectId.Null;

                    if (!tst.Has(nameStyle))
                    {
                        makeTextStyle(nameStyle, annoState);
                        idTxtStyle = tst[nameStyle];
                    }

                    using (DBText txt = new DBText())
                    {
                        txt.SetDatabaseDefaults();
                        txt.TextString = contents;
                        txt.Position = pnt3d;

                        txt.Rotation = angle;
                        txt.Justify = attachPoint;
                        txt.TextStyleId = idTxtStyle;

                        ms.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 161");
            }
            return idTxt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameFontFile"></param>
        /// <param name="size"></param>
        /// <param name="xscale"></param>
        /// <returns></returns>
        public static TextStyleTableRecord
        addTextStyleTableRecord(string name, string nameFontFile = "Romans.shx", double size = 0.0, double xscale = 0.8)
        {
            TextStyleTableRecord TStr = new TextStyleTableRecord();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TextStyleTable TST = getTextStyleTable();
                    TStr.Name = name;

                    TST.UpgradeOpen();
                    TST.Add(TStr);

                    TStr.FileName = nameFontFile;
                    TStr.TextSize = size;
                    TStr.XScale = xscale;
                    TStr.Annotative = AnnotativeStates.True;

                    tr.AddNewlyCreatedDBObject(TStr, true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 199");
            }
            return TStr;
        }

        public static void
        adjMTextPositionAndDirection(this ObjectId idMTxt, Point3d pnt3d, double dir)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
                    mTxt.Location = pnt3d;
                    mTxt.Rotation = dir;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 219");
            }
        }

        public static string
        asciiToString(this int asciiCode)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            return ascii.GetString(new Byte[] { (Byte)asciiCode });
        }

        public static void
        flipMTxt(this ObjectId idMTxt, Point3d pnt3dBase){
            idMTxt.rotateEnt(pnt3dBase, PI);
            using(var tr = BaseObjs.startTransactionDb()){
                MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
                
                string strCallout = mTxt.Text;
                string cnts = mTxt.Contents;
                string justify = cnts.Replace(strCallout, "");
                if (justify == Pub.JUSTIFYLEFT)
                    justify = Pub.JUSTIFYRIGHT;
                else if (justify == Pub.JUSTIFYRIGHT)
                    justify = Pub.JUSTIFYLEFT;

                AttachmentPoint atchPnt = mTxt.Attachment;
                if (atchPnt == AttachmentPoint.BottomRight)
                    atchPnt = AttachmentPoint.BottomLeft;
                else if (atchPnt == AttachmentPoint.BottomLeft)
                    atchPnt = AttachmentPoint.BottomRight;
                else if (atchPnt == AttachmentPoint.TopRight)
                    atchPnt = AttachmentPoint.TopLeft;
                else if (atchPnt == AttachmentPoint.TopLeft)
                    atchPnt = AttachmentPoint.TopRight;

                mTxt.Attachment = atchPnt;
                double xScale = 0.8;
                mTxt.Contents = string.Format("{0}{{\\fromans.shx\\W{1};{2}}}", justify, xScale, strCallout);

                tr.Commit();
            }
        }

        public static string
        getCalloutSuffix(string callout)
        {
            int n = callout.Length - 1;
            string suffix = string.Empty;
            for (int i = n; i > -1; i--)
            {
                double result;
                bool isNumeric = double.TryParse(callout.Substring(i, 1), out result);
                if (isNumeric)
                {
                    suffix = callout.Substring(i + 1);
                    break;
                }
            }
            return suffix;
        }

        public static string
        getMTextContents(this ObjectId idMTxt)
        {
            string contents = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mtxt = (MText)tr.GetObject(idMTxt, OpenMode.ForRead);
                    contents = mtxt.Contents;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 263");
            }
            return contents;
        }

        public static double
        getMTextHeight(this ObjectId idMTxt)
        {
            double h = 0.0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForRead);
                    tr.Commit();
                    h = mTxt.TextHeight;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 283");
            }
            return h;
        }

        public static Point3d
        getMTextLocation(this ObjectId idMTxt)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mtxt = (MText)tr.GetObject(idMTxt, OpenMode.ForRead);
                    pnt3d = mtxt.Location;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 303");
            }
            return pnt3d;
        }

        public static double
        getMTextRotation(this ObjectId idMTxt)
        {
            double dir = 0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
                    dir = mTxt.Rotation;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 323");
            }
            return dir;
        }
        public static string
        getMTextText(this ObjectId idMTxt)
        {
            string value = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mtxt = (MText)tr.GetObject(idMTxt, OpenMode.ForRead);
                    value = mtxt.Text.ToString();
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 342");
            }
            return value;
        }

        public static double
        getMTextWidth(this ObjectId idMTxt)
        {
            double width = 0.0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
                    width = mTxt.ActualWidth * 1.10;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 362");
            }
            return width;
        }

        //[DllImport
        //("acdb20.dll",
        //CharSet = CharSet.Unicode,
        //CallingConvention = CallingConvention.Cdecl,
        //EntryPoint = "?fromAcDbTextStyle@@YA?AW4ErrorStatus@Acad@@AEAVAcGiTextStyle@@AEBVAcDbObjectId@@@Z")
        //]

        //private static extern
        //Autodesk.AutoCAD.Runtime.ErrorStatus
        //    fromAcDbTextStyle(System.IntPtr style, ref�ObjectId id);


        //public static double
        //getMTextWidth(this ObjectId idMTxt)
        //{
        //    MText mTxt = (MText)idMTxt.getEnt();            
        //    ObjectId textStyleId = mTxt.TextStyleId;
        //    double width = 0.0, height = 0.0;
        //    string text = mTxt.Text;
                        
        //    using (Transaction tr = BaseObjs.startTransactionDb())
        //    {
        //        Autodesk.AutoCAD.GraphicsInterface.TextStyle iStyle = new Autodesk.AutoCAD.GraphicsInterface.TextStyle();

        //        if (fromAcDbTextStyle(iStyle.UnmanagedObject, ref�textStyleId) == Autodesk.AutoCAD.Runtime.ErrorStatus.OK)
        //        {
        //            Extents2d extents = iStyle.ExtentsBox(text, false, true, null);
        //            width = extents.MaxPoint.X - extents.MinPoint.X;
        //            width = width * 1.1;
        //            height = extents.MaxPoint.Y - extents.MinPoint.Y;

        //        }
        //        tr.Commit();
        //    }
        //    return width;
        //}

        public static ObjectId
        getTextStyleId(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TextStyleTable TST = getTextStyleTable();
                    ObjectId id = TST[name];
                    if (id != null)
                    {
                        return id;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 422");
            }
            return ObjectId.Null;
        }

        public static TextStyleTable
        getTextStyleTable()
        {
            TextStyleTable TST = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TST = (TextStyleTable)tr.GetObject(DB.TextStyleTableId, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 442");
            }
            return TST;
        }

        public static TextStyleTableRecord
        getTextStyleTableRecord(string name)
        {
            TextStyleTableRecord tstr = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TextStyleTable tst = getTextStyleTable();
                    if (tst.Has(name) == true)
                    {
                        tstr = (TextStyleTableRecord)tr.GetObject(tst[name], OpenMode.ForRead);
                    }
                    else
                    {
                        tstr = addTextStyleTableRecord(name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 469");
            }
            return tstr;
        }

        public static MText
        handleToMText(Handle handle)
        {
            MText mTxt = null;
            Database DB = BaseObjs._db;

            string strHandle = handle.ToString();
            long ln = System.Convert.ToInt64(strHandle, 16);
            Handle han = new Handle(ln);
            ObjectId objID = DB.GetObjectId(false, han, 0);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Autodesk.AutoCAD.DatabaseServices.DBObject dbObj = objID.GetObject(OpenMode.ForRead);

                    tr.Commit();
                    mTxt = (MText)dbObj;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 497");
            }
            return mTxt;
        }

        public static string
        incrementString(string s)
        {
            char[] c = s.ToCharArray();
            char p = c[0];
            p = (char)(Convert.ToInt16(p) + 1);
            return p.ToString();
        }

        public static ObjectId
        makeTextStyle(string nameStyle, AnnotativeStates annoState = AnnotativeStates.True, double xFactor = 0.8)
        {
            Database db = BaseObjs._db;

            TextStyleTable tst;
            ObjectId idTxtStyle = ObjectId.Null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForWrite);
                    TextStyleTableRecord tstr = new TextStyleTableRecord();
                    tstr.Name = nameStyle;
                    tstr.Annotative = annoState;
                    tstr.FileName = "romans.shx";
                    tstr.TextSize = 0.0;
                    tstr.ObliquingAngle = 0.0;
                    tstr.IsVertical = false;
                    tstr.IsShapeFile = false;
                    tstr.XScale = xFactor;

                    tst.Add(tstr);
                    idTxtStyle = tstr.ObjectId;
                    tr.AddNewlyCreatedDBObject(tstr, true);

                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 543");
            }
            return idTxtStyle;
        }

        public static void
        rotateText180(this ObjectId idTxt){
            using (var tr = BaseObjs.startTransactionDb()){
                Entity ent = (Entity)tr.GetObject(idTxt, OpenMode.ForWrite);
                if (ent is DBText)
                {
                    DBText txt = (DBText)ent;
                    txt.Rotation = txt.Rotation + PI;
                }
                else if (ent is MText)
                {
                    MText mTxt = (MText)ent;
                    mTxt.Rotation = mTxt.Rotation + PI;
                }
                tr.Commit();
            }
        }

        public static void
        setAnnoStyle()
        {
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TextStyleTable TST = (TextStyleTable)tr.GetObject(DB.TextStyleTableId, OpenMode.ForRead);
                    if (!TST.Has("Annotative"))
                    {
                        makeTextStyle("Annotative", AnnotativeStates.True);
                    }// end if
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 583");
            }
        }

        public static void
        setMTextAttachPointAndJustify(this ObjectId id, AttachmentPoint ap, string justify)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(id, OpenMode.ForWrite);
                    string contents = mTxt.Contents.Substring(10);
                    contents = string.Format("{0}{1}", justify, contents);
                    mTxt.Contents = contents;
                    mTxt.Attachment = ap;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 604");
            }
        }

        public static void
        setMTextLocation(this ObjectId id, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(id, OpenMode.ForWrite);

                    mTxt.Location = pnt3d;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 623");
            }
        }

        public static Double
        setMTextWidth(ObjectId objID, bool Left_Justify, out Point3d pnt3dLoc)
        {
            double dblWidth = 0;
            pnt3dLoc = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(objID, OpenMode.ForWrite);
                    dblWidth = mTxt.ActualWidth;
                    Point3d pnt3d = mTxt.Location;
                    if (Left_Justify == true)
                    {
                        pnt3dLoc = Base_Tools45.Math.traverse(pnt3d, mTxt.Rotation, dblWidth / 2 + mTxt.ActualHeight * 0.1);
                    }
                    else
                    {
                        pnt3dLoc = Base_Tools45.Math.traverse(pnt3d, mTxt.Rotation - PI, dblWidth / 2 + mTxt.ActualHeight * 0.1);
                    }

                    mTxt.Location = pnt3d;
                    mTxt.Width = dblWidth;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 655");
            }

            return dblWidth;
        }

        public static string[]
        splitFields(this string strIn, char delim)
        {
            return strIn.Split(delim);
        }

        /// <summary>
        /// split string into multiple lines
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns>array of string</returns>
        public static string[]
        splitLines(string strIn)
        {
            int intMark = -1;
            intMark = strIn.IndexOf("\\P", System.StringComparison.Ordinal);
            string[] Lines = new string[2];
            Lines[0] = strIn.Substring(0, intMark);
            Lines[1] = strIn.Substring(intMark + 2, strIn.Length - (intMark + 2));
            return Lines;
        }


        private static void
        setTextStyleXScale(ObjectId idTxtStyle, double xFactor)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TextStyleTableRecord tstr = (TextStyleTableRecord)tr.GetObject(idTxtStyle, OpenMode.ForWrite);
                    tstr.XScale = xFactor;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 698");
            }
        }

        //public static string
        //getMTextFormatString(this ObjectId idMTxt){
            
        //    try{
        //        using(var tr = BaseObjs.startTransactionDb()){
                    
        //        }
        //    }
        //    catch{}


        //}

        public static void
        updateMText(this ObjectId id, string value, string format = "")
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    MText mTxt = (MText)tr.GetObject(id, OpenMode.ForWrite);
                    string mTxtContents = mTxt.Contents;
                    string mTxtText = mTxt.Text;

                    if (format == "")
                    {
                        mTxtContents = mTxtContents.Replace(mTxtText, value);
                    }
                    else
                    {
                        mTxtContents = string.Format("{0}{1}}", format, value);
                    }

                    mTxt.Contents = mTxtContents;

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Txt.cs: line: 742");
            }
        }

        public static void
        changeGStxtAttachmentPoint(this ObjectId idMTxt){
            
            using(var tr = BaseObjs.startTransactionDb()){
                MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
                AttachmentPoint aPnt = mTxt.Attachment;
                if (aPnt == AttachmentPoint.BottomCenter)
                    mTxt.Attachment = AttachmentPoint.TopCenter;
                else if (mTxt.Attachment == AttachmentPoint.TopCenter)
                    mTxt.Attachment = AttachmentPoint.BottomCenter;
                tr.Commit();
            }

        }
    }// Class Text
}
