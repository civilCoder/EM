using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

//using Autodesk.Civil.DatabaseServices;
using Base_Tools45.Jig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

[assembly: CommandClass(typeof(Base_Tools45.Test))]

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public class Test
    {
        [CommandMethod("TranslateCoordinates")]
        public static void TranslateCoordinates()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table record for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create a 2D polyline with two segments (3 points)
                using (Polyline2d acPoly2d = new Polyline2d())
                {
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acPoly2d);
                    acTrans.AddNewlyCreatedDBObject(acPoly2d, true);

                    // Before adding vertexes, the polyline must be in the drawing
                    Point3dCollection acPts2dPoly = new Point3dCollection();
                    acPts2dPoly.Add(new Point3d(1, 1, 0));
                    acPts2dPoly.Add(new Point3d(1, 2, 0));
                    acPts2dPoly.Add(new Point3d(2, 2, 0));
                    acPts2dPoly.Add(new Point3d(3, 2, 0));
                    acPts2dPoly.Add(new Point3d(4, 4, 0));

                    foreach (Point3d acPt3d in acPts2dPoly)
                    {
                        Vertex2d acVer2d = new Vertex2d(acPt3d, 0, 0, 0, 0);
                        acPoly2d.AppendVertex(acVer2d);
                        acTrans.AddNewlyCreatedDBObject(acVer2d, true);
                    }

                    // Set the normal of the 2D polyline
                    acPoly2d.Normal = new Vector3d(0, 1, 2);

                    // Get the first coordinate of the 2D polyline
                    Point3dCollection acPts3d = new Point3dCollection();
                    Vertex2d acFirstVer = null;

                    foreach (ObjectId acObjIdVert in acPoly2d)
                    {
                        acFirstVer = acTrans.GetObject(acObjIdVert,
                                                       OpenMode.ForRead) as Vertex2d;

                        acPts3d.Add(acFirstVer.Position);

                        break;
                    }

                    // Get the first point of the polyline and 
                    // use the eleveation for the Z value
                    Point3d pFirstVer = new Point3d(acFirstVer.Position.X,
                                                    acFirstVer.Position.Y,
                                                    acPoly2d.Elevation);

                    // Translate the OCS to WCS
                    Matrix3d mWPlane = Matrix3d.WorldToPlane(acPoly2d.Normal);
                    Point3d pWCSPt = pFirstVer.TransformBy(mWPlane);

                    Application.ShowAlertDialog("The first vertex has the following " +
                                                "coordinates:" +
                                                "\nOCS: " + pFirstVer.ToString() +
                                                "\nWCS: " + pWCSPt.ToString());
                }

                // Save the new objects to the database
                acTrans.Commit();
            }
        }
        // Custom ArcTangent method, as the Math.Atan
        // doesn't handle specific cases
        public static double Atan(double y, double x)
        {
            if (x > 0)
                return System.Math.Atan(y / x);
            else if (x < 0)
                return System.Math.Atan(y / x) - Math.PI;
            else
            {
                if (y > 0)
                    return Math.PI;
                else if (y < 0)
                    return -Math.PI;
                else // if (y == 0) theta is undefined
                    return 0.0;
            }
        }

        // Computes Angle between a provided vector and that
        // defined by the vector between two points
        public static double ComputeAngle(
            Point3d startPoint, Point3d endPoint,
            Vector3d xdir, Matrix3d ucs)
        {
            var v =
                new Vector3d(
                    (endPoint.X - startPoint.X) / 2,
                    (endPoint.Y - startPoint.Y) / 2,
                    (endPoint.Z - startPoint.Z) / 2);

            var cos = v.DotProduct(xdir);
            var sin =
                v.DotProduct(
                    Vector3d.ZAxis.TransformBy(ucs).CrossProduct(xdir));

            return Atan(sin, cos);
        }

        [CommandMethod("HLNESTED")]
        public static void
        HighlightNestedEntity()
        {
            Document doc =
                Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptNestedEntityResult rs =
                ed.GetNestedEntity("\nSelect nested entity: ");

            if (rs.Status == PromptStatus.OK)
            {
                ObjectId[] objIds = rs.GetContainers();
                ObjectId ensel = rs.ObjectId;
                int len = objIds.Length;

                // Reverse the "containers" list
                ObjectId[] revIds = new ObjectId[len + 1];
                for (int i = 0; i < len; i++)
                {
                    ObjectId id =
                        (ObjectId)objIds.GetValue(len - i - 1);
                    revIds.SetValue(id, i);
                }
                // Now add the selected entity to the end
                revIds.SetValue(ensel, len);

                // Retrieve the sub-entity path for this entity
                SubentityId subEnt =
                    new SubentityId(SubentityType.Null, (IntPtr)0);
                FullSubentityPath path =
                    new FullSubentityPath(revIds, subEnt);

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        // Open the outermost container...
                        ObjectId id = (ObjectId)revIds.GetValue(0);
                        Entity ent =
                            (Entity)tr.GetObject(id, OpenMode.ForRead);
                        // ... and highlight the nested entity
                        ent.Highlight(path, false);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 191");
                }
            }
        }

        [CommandMethod("QT")]
        static public void QuickText()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptStringOptions pso = new PromptStringOptions("\nEnter text string");
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);

            if (pr.Status != PromptStatus.OK)
                return;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    // Create the text object, set its normal and contents

                    DBText txt = new DBText();
                    txt.Normal = ed.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
                    txt.TextString = pr.StringResult;

                    // We'll add the text to the database before jigging
                    // it - this allows alignment adjustments to be
                    // reflected

                    btr.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);

                    // Create our jig

                    JigText pj = new JigText(tr, db, txt);

                    // Loop as we run our jig, as we may have keywords

                    PromptStatus stat = PromptStatus.Keyword;
                    while (stat == PromptStatus.Keyword)
                    {
                        PromptResult res = ed.Drag(pj);
                        stat = res.Status;
                        if (
                            stat != PromptStatus.OK &&
                            stat != PromptStatus.Keyword
                        )
                            return;
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 252");
            }
        }

        [CommandMethod("SID")]
        static public void SwapEntities()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityResult per = ed.GetEntity("\nSelect first entity: ");
            if (per.Status != PromptStatus.OK)
                return;

            ObjectId firstId = per.ObjectId;

            per = ed.GetEntity("\nSelect second entity: ");
            if (per.Status != PromptStatus.OK)
                return;

            ObjectId secondId = per.ObjectId;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                DBObject firstObj = tr.GetObject(firstId, OpenMode.ForRead);

                DBObject secondObj = tr.GetObject(secondId, OpenMode.ForRead);

                PrintIdentities(firstObj, secondObj);

                PromptKeywordOptions pko = new PromptKeywordOptions("\nSwap their identities?");
                pko.AllowNone = true;
                pko.Keywords.Add("Yes");
                pko.Keywords.Add("No");
                pko.Keywords.Default = "No";

                PromptResult pkr = ed.GetKeywords(pko);

                if (pkr.StringResult == "Yes")
                {
                    try
                    {
                        firstObj.UpgradeOpen();
                        firstObj.SwapIdWith(secondId, true, true);

                        PrintIdentities(firstObj, secondObj);
                    }
                    catch (SystemException ex)
                    {
                        ed.WriteMessage(string.Format("\nCould not swap identities: {0}", ex.Message));
                    }
                }
                tr.Commit();
            }
        }

        [CommandMethod("testGMT")]
        public static void
        testGetMTextValue()
        {
            PromptSelectionResult psr = BaseObjs._editor.GetSelection();
            SelectionSet ss = psr.Value;
            ObjectId[] ids = ss.GetObjectIds();
            string text = ids[0].getMTextText();
            Application.ShowAlertDialog(text);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testPoly")]
        public static void
        testPoly()
        {
            Point3dCollection pnts3d = new Point3dCollection();
            pnts3d.Add(new Point3d(0, 0, 0));
            pnts3d.Add(new Point3d(100, 100, 0));
            Draw.addPoly(pnts3d, "0");
        }

        [CommandMethod("AuditTest")]
        public void AuditTest()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            try
            {
                //to fix the error or not
                bool bFixErrors = true;
                //to show the message in commandline or not
                bool becho = true;

                //call audit API
                doc.Database.Audit(bFixErrors, becho);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 350");
            }
        }

        [CommandMethod("BOJ")]
        public void BoxJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // Let's get the initial corner of the box

            var ppr = ed.GetPoint("\nSpecify first corner: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected,
                // we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr =
                            (BlockTableRecord)tr.GetObject(
                                db.CurrentSpaceId, OpenMode.ForWrite);

                        var sol = new Solid3d();
                        btr.AppendEntity(sol);
                        tr.AddNewlyCreatedDBObject(sol, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, sol, ppr.Value,
                                new List<Phase>() {
                                    // Two phases, the second of which has a custom
                                    // offset for the base point

                                    new PointPhase("\nSpecify opposite corner: "),
                                    new SolidDistancePhase(
                                        "\nSpecify height: ",
                                        1e-05,
                                        (vals, pt) => {
                                            // Get the diagonal line between the corners
                                            var pt2 = (Point3d)vals[0].Value;
                                            var diag = pt2 - pt;
                                            var dlen = diag.Length;

                                            // Use Pythagoras' theorem to get the side
                                            // length

                                            var side = System.Math.Sqrt(dlen * dlen / 2);
                                            var halfSide = side / 2;

                                            // Start by getting the displacement from
                                            // the start point, adjusting for the fact
                                            // we're jigging from a corner, not the center

                                            var mat =
                                                Matrix3d.Displacement(
                                                                      pt.GetAsVector() +
                                                                      new Vector3d(
                                                                          halfSide,
                                                                          halfSide,
                                                                          (double)vals[1].Value / 2));

                                            // Calculate the angle between the diagonal
                                            // and the X axis

                                            var ang =
                                                ComputeAngle(
                                                    pt,
                                                    pt2,
                                                    Vector3d.XAxis,
                                                    Matrix3d.Identity);

                                            // Add a rotation component to the
                                            // transformation, adjusted by 45 degrees
                                            // to jig the box diagonally

                                            mat =
                                                mat.PostMultiplyBy(
                                                    Matrix3d.Rotation(
                                                        ang + (-45 * Math.PI / 180),
                                                        Vector3d.ZAxis,
                                                        new Point3d(-halfSide, -halfSide, 0)));

                                            return
                                            new Vector3d(halfSide, halfSide, 0).
                                            TransformBy(mat);
                                        })
                                },
                                (e, vals, pt, ucs) =>
                                {
                                    // Our entity update function
                                    // Get the diagonal line between the corners
                                    var pt2 = (Point3d)vals[0].Value;
                                    var diag = pt2 - pt;
                                    var dlen = diag.Length;

                                    // Use Pythagoras' theorem to get the side
                                    // length

                                    var side = System.Math.Sqrt(dlen * dlen / 2);
                                    var halfSide = side / 2;

                                    // Create our box with square sides and
                                    // the chosen height

                                    var s = (Solid3d)e;
                                    s.CreateBox(side, side, (double)vals[1].Value);

                                    // Start by getting the displacement from
                                    // the start point, adjusting for the fact
                                    // we're jigging from a corner, not the center
                                    // (need to adjust for the current UCS)

                                    var mat =
                                        Matrix3d.Displacement(
                                                              pt.GetAsVector() +
                                                              new Vector3d(
                                                                  halfSide,
                                                                  halfSide,
                                                                  (double)vals[1].Value / 2)).PreMultiplyBy(ucs);

                                    // Calculate the angle between the diagonal
                                    // and the X axis

                                    var ang =
                                        ComputeAngle(
                                            pt,
                                            pt2,
                                            Vector3d.XAxis,
                                            Matrix3d.Identity);

                                    // Add a rotation component to the
                                    // transformation, adjusted by 45 degrees
                                    // to jig the box diagonally

                                    mat =
                                        mat.PostMultiplyBy(
                                            Matrix3d.Rotation(
                                                ang + (-45 * Math.PI / 180),
                                                Vector3d.ZAxis,
                                                new Point3d(-halfSide, -halfSide, 0)));

                                    // Transform our solid

                                    s.TransformBy(mat);

                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 509");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR1")]
        public void
        ChangeEntityColor1()
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    RXClass circleClass = RXObject.GetClass(typeof(Circle));

                    foreach (ObjectId objectId in modelSpace)
                    {
                        if (objectId.ObjectClass.IsDerivedFrom(circleClass))
                        {
                            var circle = (Circle)tr.GetObject(objectId, OpenMode.ForRead);
                            if (circle.Radius < 1.0)
                            {
                                circle.UpgradeOpen();
                                circle.ColorIndex = 1;
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 546");
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR2")]
        public void
        ChangeEntityColor2()
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    var blockTable = (BlockTable)BaseObjs._db.BlockTableId.GetObject(OpenMode.ForRead);
                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    RXClass circleClass = RXObject.GetClass(typeof(Circle));
                    foreach (ObjectId objectId in modelSpace)
                    {
                        if (objectId.ObjectClass.IsDerivedFrom(circleClass))
                        {
                            var circle = (Circle)tr.GetObject(objectId, OpenMode.ForRead);
                            if (circle.Radius < 1.0)
                            {
                                circle.UpgradeOpen();
                                circle.ColorIndex = 1;
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 581");
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR3")]
        public void ChangeEntityColorWithDelegate()
        {
            UsingTransaction(ChangeSmallCirclesToRed);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR4")]
        public void
        ChangeEntityColorWithDelegate2()
        {
            UsingModelSpace(ChangeSmallCirclesToRed2);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR")]
        public void
        ChangeEntityColorWithForEach()
        {
            ForEach<Circle>(ProcessCircle);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR5")]
        public void
        ChangeEntityColorWithForEach5()
        {
            ForEach(
                delegate(Circle circle)
                {
                    if (circle.Radius < 1.0)
                    {
                        circle.UpgradeOpen();
                        circle.ColorIndex = 1;
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR6")]
        public void
        ChangeEntityColorWithForEach6()
        {
            ForEach<Circle>(
                circle =>
                {
                    if (circle.Radius < 1.0)
                    {
                        circle.UpgradeOpen();
                        circle.ColorIndex = 1;
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("ENTCOLOR7")]
        public void
        ChangeEntityColorWithForEach7()
        {
            BaseObjs._db.forEachMS<Circle>(
                circle =>
                {
                    if (circle.Radius < 1.0)
                    {
                        circle.UpgradeOpen();
                        circle.ColorIndex = 1;
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tr"></param>
        public void
        ChangeSmallCirclesToRed(Transaction tr)
        {
            var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
            var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
            RXClass circleClass = RXObject.GetClass(typeof(Circle));
            foreach (ObjectId objectId in modelSpace)
            {
                if (objectId.ObjectClass.IsDerivedFrom(circleClass))
                {
                    var circle = (Circle)tr.GetObject(objectId, OpenMode.ForRead);
                    if (circle.Radius < 1.0)
                    {
                        circle.UpgradeOpen();
                        circle.ColorIndex = 1;
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="modelSpace"></param>
        public void
        ChangeSmallCirclesToRed2(Transaction tr, BlockTableRecord modelSpace)
        {
            RXClass circleClass = RXObject.GetClass(typeof(Circle));
            foreach (ObjectId objectId in modelSpace)
            {
                if (objectId.ObjectClass.IsDerivedFrom(circleClass))
                {
                    var circle = (Circle)tr.GetObject(objectId, OpenMode.ForRead);
                    if (circle.Radius < 1.0)
                    {
                        circle.UpgradeOpen();
                        circle.ColorIndex = 1;
                    }
                }
            }
        }

        [CommandMethod("COJ")]
        public void ConeJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // First let's get the start position of the cylinder

            var ppr = ed.GetPoint("\nSpecify cone location: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected,
                // we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr =
                            (BlockTableRecord)tr.GetObject(
                                db.CurrentSpaceId, OpenMode.ForWrite);

                        var sol = new Solid3d();
                        btr.AppendEntity(sol);
                        tr.AddNewlyCreatedDBObject(sol, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, sol, ppr.Value,
                                new List<Phase>() {
                                    // Three phases, one of which has a custom
                                    // offset for the base point

                                    new SolidDistancePhase("\nSpecify radius: "),
                                    new SolidDistancePhase("\nSpecify height: "),
                                },
                                (e, vals, cen, ucs) =>
                                {
                                    // Our entity update function
                                    var s = (Solid3d)e;
                                    s.CreateFrustum(
                                        (double)vals[1].Value,
                                        (double)vals[0].Value,
                                        (double)vals[0].Value,
                                        0);
                                    s.TransformBy(
                                        Matrix3d.Displacement(
                                                              cen.GetAsVector() +
                                                              new Vector3d(0, 0, (double)vals[1].Value / 2)).PreMultiplyBy(ucs));
                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 774");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("CreateLeader")]
        public void CreateLeader()
        {
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        // Open the Block table for read
                        BlockTable acBlkTbl = (BlockTable)BaseObjs._db.BlockTableId.GetObject(OpenMode.ForRead);

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                            OpenMode.ForWrite) as BlockTableRecord;

                        // Create the leader
                        using (Leader acLdr = new Leader())
                        {
                            acLdr.AppendVertex(new Point3d(0, 0, 0));
                            acLdr.AppendVertex(new Point3d(4, 4, 0));
                            acLdr.AppendVertex(new Point3d(4, 5, 0));
                            acLdr.HasArrowHead = true;

                            // Add the new object to Model space and the transaction
                            acBlkTblRec.AppendEntity(acLdr);
                            tr.AddNewlyCreatedDBObject(acLdr, true);
                        }

                        // Commit the changes and dispose of the transaction
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 820");
            }
        }

        [CommandMethod("CYJ")]
        public void CylinderJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // First let's get the start position of the cylinder

            var ppr = ed.GetPoint("\nSpecify cylinder location: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected,
                // we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr =
                            (BlockTableRecord)tr.GetObject(
                                db.CurrentSpaceId, OpenMode.ForWrite);

                        var sol = new Solid3d();
                        btr.AppendEntity(sol);
                        tr.AddNewlyCreatedDBObject(sol, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, sol, ppr.Value,
                                new List<Phase>() {
                                    // Three phases, one of which has a custom
                                    // offset for the base point

                                    new SolidDistancePhase("\nSpecify radius: "),
                                    new SolidDistancePhase("\nSpecify height: "),
                                },
                                (e, vals, cen, ucs) =>
                                {
                                    // Our entity update function
                                    var s = (Solid3d)e;
                                    s.CreateFrustum(
                                        (double)vals[1].Value,
                                        (double)vals[0].Value,
                                        (double)vals[0].Value,
                                        (double)vals[0].Value);
                                    s.TransformBy(
                                        Matrix3d.Displacement(
                                                              cen.GetAsVector() +
                                                              new Vector3d(0, 0, (double)vals[1].Value / 2)).PreMultiplyBy(ucs));
                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 883");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void
        ForEach<T>(Action<T> action) where T : Entity
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    RXClass theClass = RXObject.GetClass(typeof(T));
                    foreach (ObjectId objectId in modelSpace)
                    {
                        if (objectId.ObjectClass.IsDerivedFrom(theClass))
                        {
                            var entity = (T)tr.GetObject(objectId, OpenMode.ForRead);
                            action(entity);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 916");
            }
        }

        [CommandMethod("FJ")]
        public void FrustumJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // First let's get the start position of the frustum

            var ppr = ed.GetPoint("\nSpecify frustum location: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected,
                // we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr =
                            (BlockTableRecord)tr.GetObject(
                                db.CurrentSpaceId, OpenMode.ForWrite);

                        var sol = new Solid3d();
                        btr.AppendEntity(sol);
                        tr.AddNewlyCreatedDBObject(sol, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, sol, ppr.Value,
                                new List<Phase>() {
                                    // Three phases, one of which has a custom
                                    // offset for the base point

                                    new SolidDistancePhase("\nSpecify bottom radius: "),
                                    new SolidDistancePhase("\nSpecify height: "),
                                    new SolidDistancePhase(
                                        "\nSpecify top radius: ",
                                        1e-05,
                                        (vals, pt) => {
                                            return
                                            new Vector3d(0, 0, (double)vals[1].Value);
                                        })
                                },
                                (e, vals, cen, ucs) =>
                                {
                                    // Our entity update function
                                    var s = (Solid3d)e;
                                    s.CreateFrustum(
                                        (double)vals[1].Value,
                                        (double)vals[0].Value,
                                        (double)vals[0].Value,
                                        (double)vals[2].Value);
                                    s.TransformBy(
                                        Matrix3d.Displacement(
                                                              cen.GetAsVector() +
                                                              new Vector3d(0, 0, (double)vals[1].Value / 2)).PreMultiplyBy(ucs));
                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 986");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>jig*.jig
        [CommandMethod("testXR")]
        public void
        getXRefBlock2()
        {
            string name = "CONT";
            BaseObjs._db.forEachBR<BlockReference>(
                BR =>
                {
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)tr.GetObject(idBtr, OpenMode.ForRead);
                            if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                            {
                                if (Btr.Name.Contains(name))
                                {
                                    BaseObjs._editor.WriteMessage(Btr.Name);
                                    BaseObjs._editor.WriteMessage(BR.Name);
                                }
                            }
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1021");
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="circle"></param>
        public void
        ProcessCircle(Circle circle)
        {
            if (circle.Radius < 1.0)
            {
                circle.UpgradeOpen();
                circle.ColorIndex = 1;
            }
        }

        [CommandMethod("RectJig")]      //doesn't work 10/21/2014
        public void RectJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // Let's get the initial corner of the box

            var ppr = ed.GetPoint("\nSpecify first corner: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected, we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                        //var sol = new Solid3d();
                        var poly3d = new Polyline3d();
                        btr.AppendEntity(poly3d);
                        tr.AddNewlyCreatedDBObject(poly3d, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, poly3d, ppr.Value,
                                new List<Phase>() {
                                    // Two phases, the second of which has a custom
                                    // offset for the base point

                                    new PointPhase("\nSpecify opposite corner: ")
                                },
                                (e, vals, pt, ucs) =>
                                {
                                    // Our entity update function Get the diagonal line between the corners
                                    var pt2 = (Point3d)vals[0].Value;
                                    var diag = pt2 - pt;
                                    var dlen = diag.Length;
                                    var dir = diag.AngleOnPlane(BaseObjs.xyPlane);
                                    var dx = dlen * System.Math.Cos(dir);
                                    var dy = dlen * System.Math.Sin(dir);

                                    var pt3 = new Point3d(pt2.X + dx, pt2.Y, 0.0);
                                    var pt4 = new Point3d(pt2.X, pt2.Y + dy, 0.0);

                                    // Use Pythagoras' theorem to get the side length

                                    var side = System.Math.Sqrt(dlen * dlen / 2);
                                    var halfSide = side / 2;

                                    // Create our box with square sides and
                                    // the chosen height

                                    var p = (Polyline3d)e;
                                    PolylineVertex3d v3d = new PolylineVertex3d(pt3);
                                    p.AppendVertex(v3d);

                                    v3d = new PolylineVertex3d(pt2);
                                    p.AppendVertex(v3d);

                                    v3d = new PolylineVertex3d(pt4);
                                    p.AppendVertex(v3d);

                                    v3d = new PolylineVertex3d(pt);
                                    p.AppendVertex(v3d);

                                    //s.CreateBox(side, side, (double)vals[1].Value);

                                    // Start by getting the displacement from the start point, adjusting for the fact
                                    // we're jigging from a corner, not the center (need to adjust for the current UCS)

                                    //var mat = Matrix3d.Displacement(pt.GetAsVector() + new Vector3d(halfSide, halfSide,
                                    //                              (double)vals[1].Value / 2)).PreMultiplyBy(ucs);

                                    // Calculate the angle between the diagonal
                                    // and the X axis

                                    //var ang = ComputeAngle(pt, pt2, Vector3d.XAxis, Matrix3d.Identity);

                                    // Add a rotation component to the transformation, adjusted by 45 degrees
                                    // to jig the box diagonally

                                    //mat = mat.PostMultiplyBy(Matrix3d.Rotation(ang + (-45 * Math.PI / 180), Vector3d.ZAxis,
                                    //            new Point3d(-halfSide, -halfSide, 0)));

                                    // Transform our solid

                                    //p.TransformBy(mat);

                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1140");
                }
            }
        }

        [CommandMethod("SysVarListTest")]
        public void SysVarListTest()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            using (SystemVariableEnumerator sysVar = new SystemVariableEnumerator())
            {
                string storage = "";

                while (sysVar.MoveNext())
                {
                    Variable var = sysVar.Current;

                    switch (var.Storage)
                    {
                        case Variable.StorageType.PerDatabase:
                            storage = "PerDatabase";
                            break;

                        case Variable.StorageType.PerProfile:
                            storage = "PerProfile";
                            break;

                        case Variable.StorageType.PerSession:
                            storage = "PerSession";
                            break;

                        case Variable.StorageType.PerUser:
                            storage = "PerUser";
                            break;

                        case Variable.StorageType.PerViewport:
                            storage = "PerViewport";
                            break;
                    }

                    ed.WriteMessage(string.Format("{0}   {1}\n", var.Name, storage));
                }
            }
            //use Application.GetSystemVariable() to get the value.
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testCopy")]
        public void
        testCopy()
        {
            string nameLayer = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    //Entity ent = xRef.getEntity("select something", true, ref nameLayer);
                    ObjectIdCollection ids = xRef.getXRefsContainingTargetLayer("CURB");
                    if (ids.Count > 0)
                    {
                        xRef.copyXRefEnts(ids[0], "CURB", "CURB-TEMP");
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1212");
            }
            MessageBox.Show(nameLayer);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testDelete")]
        public void testDelete()
        {
            PromptSelectionResult PSR = BaseObjs._editor.GetSelection();
            if (PSR.Status == PromptStatus.OK)
            {
                ObjectId[] ids = PSR.Value.GetObjectIds();
                Misc.deleteObjs(ids);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testF")]
        public void
        testF()
        {
            string directory = Path.GetDirectoryName(BaseObjs.docFullName);
            string file = FileManager.getFilesWithEditor("SELECT FILE FOR TRANSFER", "GETFILE", "Drawing (*.dwg)|*.dwg", directory, "Select File for Transfer");

            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(file);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("textInt")]
        public void
        testInt()
        {
            Point3d pnt3dPicked;
            Line line = (Line)Select.selectEntity(typeof(Line), "select Line", "oops", out pnt3dPicked);
            Polyline poly = (Polyline)Select.selectEntity(typeof(Polyline), "select Polyline", "oops", out pnt3dPicked);
            List<Point3d> pntsdInts = line.ObjectId.intersectWith(poly, true, 0);
            foreach (Point3d pnt3d in pntsdInts)
            {
                Draw.addCircle(pnt3d, 1);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testJig")]
        public void testJig()
        {
            Leader ldr = new Leader();
            LeaderJig.Jig(ldr);
        }

        [CommandMethod("testJigRect")]
        public void testJigRect()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // Let's get the initial corner of the box

            var ppr = ed.GetPoint("\nSpecify first corner: ");

            JigRect jgRect = new JigRect(ppr.Value);
            ed.Drag(jgRect);

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                Polyline ent = new Polyline();
                ent.SetDatabaseDefaults();
                for (int i = 0; i < jgRect.corners.Count - 1; i++)
                {
                    Point3d pnt3d = jgRect.corners[i];
                    Point2d pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                    ent.AddVertexAt(i, pnt2d, 0, db.Plinewid, db.Plinewid);
                }
                ent.Closed = true;
                ent.TransformBy(jgRect.UCS);
                btr.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);
                tr.Commit();
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testCntd")]
        public void
        testLay()
        {
            Point3d pnt3d = Pub.pnt3dO;
            Entity ent = Select.selectEntity(typeof(Polyline), "select", "oops", out pnt3d);
            Polyline poly = (Polyline)ent;
            pnt3d = Geom.getCentroid(poly);
            Application.ShowAlertDialog(pnt3d.ToString());
        }

        //[CommandMethod("testFD")]
        //public void
        //    testFileDialog()
        //{
        //    myFileDialog fDialog = new myFileDialog();
        //    string jn = BaseObjs.jobNumber();
        //    if (jn != "0" )
        //    {
        //        switch (jn.Length)
        //        {
        //            case 4:
        //                fDialog.Filter = "Current Design Files" + Strings.Chr(0) + "????CNTL.dwg;????T-SITE.dwg" + Strings.Chr(0) +
        //                                 "All Drawings (*.dwg)" + Strings.Chr(0) + "*.dwg" + Strings.Chr(0);
        //            break;

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testMyLdr")]
        public void testMyLdr()
        {
            ObjectId idLayer = Layer.manageLayers("0");
            ObjectId idStyle = Txt.getTextStyleId("Annotative");
            Point3dCollection pnts3d = new Point3dCollection();
            pnts3d.Add(new Point3d(0, 0, 0));
            pnts3d.Add(new Point3d(10, 10, 0));
            pnts3d.Add(new Point3d(20, 10, 0));
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    Ldr.addLdr(pnts3d, idLayer, 1.0, 0.1, clr.byl, ObjectId.Null);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1354");
            }
        }

        /// <summary>
        ///
        /// </summary>
        //            case 5:
        //                fDialog.Filter = "Current Design Files" + Strings.Chr(0) + "?????CNTL.dwg;?????T-SITE.dwg" + Strings.Chr(0) +
        //                                 "All Drawings (*.dwg)" + Strings.Chr(0) + "*.dwg" + Strings.Chr(0);
        //            break;
        //        }
        //    }
        //    fDialog.Title = "SELECT TARGET DRAWING";
        //    fDialog.OwnerHwnd = (int)Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle;
        //    fDialog.StartInDir = Path.GetDirectoryName(BaseObjs.docFullName);
        //    fDialog.HideReadOnly = true;
        //    Object files = fDialog.ShowOpen();
        //}
        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testOffH")]
        public void
        testOffH()
        {
            Point3d pnt3dPicked;
            Polyline poly = (Polyline)Select.selectEntity(typeof(Polyline), "select poly", "oops", out pnt3dPicked);
            poly.offset(-2.0, 1.0);
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testText")]
        public void testText()
        {
            Txt.addTextStyleTableRecord("ANNO");
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testVP")]
        public void
        testVP()
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId idVP = BaseObjs._editor.ActiveViewportId;
                    ViewportTableRecord vptr = (ViewportTableRecord)tr.GetObject(idVP, OpenMode.ForRead);
                    BaseObjs.write(vptr.Width.ToString());
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1413");
            }
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testXRef")]
        public void testXRef()
        {
            string nameLayer = string.Empty;
            Point3d pnt3dPick;
            ObjectId idBlkRef = ObjectId.Null;
            ObjectId idEnt = xRef.getEntity("Select xref entity", true, out nameLayer, out pnt3dPick, out idBlkRef);
            string[] nameFile_Layer = new string[2];
            string[] delim = new string[1];
            delim[0] = "|";
            nameFile_Layer = nameLayer.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            if (nameFile_Layer.Length == 1)
                nameLayer = nameFile_Layer[0];
            else
                nameLayer = nameFile_Layer[1];

            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Ent type is {0}  Layer: {1}", idEnt.GetType().ToString(), nameLayer));
        }

        /// <summary>
        ///
        /// </summary>
        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testXRefBlk")]
        public void testXRefBlk()
        {
            BlockReference BR = xRef.getXRefBlockReference("CNTL");
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(BR.Name.ToString());
        }

        [CommandMethod("TJ")]
        public void TorusJig()
        {
            var doc = BaseObjs._acadDoc;
            var db = doc.Database;
            var ed = doc.Editor;

            // First let's get the start position of the frustum

            var ppr = ed.GetPoint("\nSpecify torus location: ");

            if (ppr.Status == PromptStatus.OK)
            {
                // In order for the visual style to be respected,
                // we'll add the to-be-jigged solid to the database
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        var btr =
                            (BlockTableRecord)tr.GetObject(
                                db.CurrentSpaceId, OpenMode.ForWrite);

                        var sol = new Solid3d();
                        btr.AppendEntity(sol);
                        tr.AddNewlyCreatedDBObject(sol, true);

                        // Create our jig object passing in the selected point

                        var jf =
                            new EntityJigFramework(
                                ed.CurrentUserCoordinateSystem, sol, ppr.Value,
                                new List<Phase>() {
                                    // Three phases, one of which has a custom
                                    // offset for the base point

                                    new SolidDistancePhase("\nSpecify outer radius: "),
                                    new SolidDistancePhase(
                                        "\nSpecify inner radius: ",
                                        1e-05,
                                        (vals, pt) => {
                                            return
                                            new Vector3d(0, 0, (double)vals[0].Value);
                                        })
                                },
                                (e, vals, cen, ucs) =>
                                {
                                    // Our entity update function
                                    var s = (Solid3d)e;
                                    s.CreateTorus(
                                        (double)vals[0].Value,
                                        (double)vals[1].Value);
                                    s.TransformBy(
                                        Matrix3d.Displacement(
                                                              cen.GetAsVector() +
                                                              new Vector3d(0, 0, (double)vals[1].Value)).PreMultiplyBy(ucs));
                                    return true;
                                });
                        jf.RunTillComplete(ed, tr);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1515");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        public void
        UsingModelSpace(Action<Transaction, BlockTableRecord> action)
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    action(tr, modelSpace);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1539");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        public void
        UsingTransaction(Action<Transaction> action)
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    action(tr);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1560");
            }
        }

        private static void
        PrintIdentities(DBObject first, DBObject second)
        {
            PrintIdentity(first, "First");
            PrintIdentity(second, "Second");
        }

        private static void
        PrintIdentity(DBObject obj, string name)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            ed.WriteMessage("\n{{0}} object, of type {{1}}: ObjectId is {{2}}, Handle is {{3}}.", name, obj.GetType().Name,
                obj.ObjectId, obj.Handle);
        }

        /// <summary>
        ///
        /// </summary>
        public class SimpleGeometryJig : DrawJig
        {
            /// <summary>
            ///
            /// </summary>
            private readonly IList<Polyline> polylines;

            /// <summary>
            ///
            /// </summary>
            private Point3d currentPosition;

            /// <summary>
            ///
            /// </summary>
            private Vector2d currentVector;

            /// <summary>
            ///
            /// </summary>
            /// <param name="polylines"></param>
            /// <param name="referencePoint"></param>
            //Constructor
            public
            SimpleGeometryJig(IList<Polyline> polylines, Point3d referencePoint)
            {
                this.polylines = polylines;

                // use first point in polyline collection as reference point
                currentPosition = referencePoint;

                // init current vector as 0,0,0
                currentVector = new Vector2d(0, 0);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="prompts"></param>
            /// <returns></returns>
            protected override SamplerStatus
            Sampler(JigPrompts prompts)
            {
                JigPromptPointOptions jigOpt = new JigPromptPointOptions("select insertion point");
                jigOpt.UserInputControls = UserInputControls.Accept3dCoordinates;

                PromptPointResult res = prompts.AcquirePoint(jigOpt);

                if (res.Status != PromptStatus.OK)
                    return SamplerStatus.Cancel;

                // compare points
                if (res.Value.IsEqualTo(currentPosition, new Tolerance(0.1, 0.1)))
                    return SamplerStatus.NoChange;

                // get vector to current position
                Vector3d v3d = currentPosition.GetVectorTo(res.Value);
                currentVector = new Vector2d(v3d.X, v3d.Y);

                // reset current position
                currentPosition = res.Value;

                return SamplerStatus.OK;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="draw"></param>
            /// <returns></returns>
            protected override bool
            WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
            {
                try
                {
                    // add vector to all points of all polylines
                    foreach (var pl in polylines)
                    {
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            // add vector to point
                            pl.SetPointAt(i, pl.GetPoint2dAt(i).Add(currentVector));
                        }

                        draw.Geometry.Draw(pl);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Test.cs: line: 1673");
                }
                return true;
            }
        }
    }
}
