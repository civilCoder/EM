using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;


namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static partial class Draw
    {
        static double PI = System.Math.PI;

        public static ObjectId
        addArc(Point3d pnt3dCen, double radius, double angBeg, double angEnd,  string nameLayer = "0", short color = 256)
        {
            ObjectId id = ObjectId.Null;
            Layer.manageLayers(nameLayer);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    Arc arc = new Arc(pnt3dCen, radius, angBeg, angEnd);

                    arc.SetDatabaseDefaults();
                    arc.Layer = nameLayer;
                    arc.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    arc.Center = pnt3dCen;
                    arc.Normal = new Vector3d(0, 0, 1);

                    id = MS.AppendEntity(arc);
                    tr.AddNewlyCreatedDBObject(arc, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 43");
            }

            return id;
        }


        public static ObjectId
        addArc(Point3d pnt3dBeg, Point3d pnt3dEnd, double bulge, string nameLayer = "0", short color = 256)
        {
            ObjectId id = ObjectId.Null;
            Layer.manageLayers(nameLayer);

            double delta = Geom.getDelta(bulge);
            double radius = Geom.getRadius(pnt3dBeg, pnt3dEnd, bulge);
            double AzChord = Base_Tools45.Measure.getAzRadians(pnt3dBeg, pnt3dEnd);
            double AzTan = AzChord - delta / 2;
            double angBeg = (bulge < 0) ? AzTan + PI / 2 : AzTan - PI / 2;
            Point3d pnt3dCen = pnt3dBeg.traverse((AzTan + PI / 2), radius);
            double angEnd = pnt3dCen.getDirection(pnt3dEnd);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    Arc arc = new Arc(pnt3dCen, radius, angBeg, angEnd);

                    arc.SetDatabaseDefaults();
                    arc.Layer = nameLayer;
                    arc.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    arc.Normal = new Vector3d(0, 0, 1);

                    id = MS.AppendEntity(arc);
                    tr.AddNewlyCreatedDBObject(arc, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 83");
            }

            return id;
        }

        public static ObjectId
        addCircle(Point3d pntCEN, double RADIUS, string nameLayer = "0", short color = 256)
        {
            ObjectId id = ObjectId.Null;
            Circle circle = new Circle();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    circle.SetDatabaseDefaults();
                    circle.Center = pntCEN;
                    circle.Radius = RADIUS;
                    Layer.manageLayers(nameLayer);
                    circle.Layer = nameLayer;
                    circle.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);

                    id = MS.AppendEntity(circle);
                    tr.AddNewlyCreatedDBObject(circle, true);

                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 116");
            }

            return id;
        }

        public static ObjectId
        addLdr(Point3dCollection pnt3ds, bool isSplined = false, MText mTxt = null, string nameLayer = "0", short color = 256)
        {
            ObjectId idLdr = ObjectId.Null;
            Leader ldr = new Leader();
            double scale = Misc.getCurrAnnoScale();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    ldr.SetDatabaseDefaults();
                    ldr.HasArrowHead = true;
                    ldr.IsSplined = isSplined;
                    ldr.Dimasz = scale * 0.09;

                    Layer.manageLayers(nameLayer);
                    ldr.Layer = nameLayer;
                    ldr.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);

                    foreach (Point3d pnt3d in pnt3ds)
                    {
                        ldr.AppendVertex(pnt3d);
                    }
                    idLdr = MS.AppendEntity(ldr);
                    tr.AddNewlyCreatedDBObject(ldr, true);
                    if (mTxt != null)
                    {
                        ldr.Annotation = mTxt.ObjectId;
                        ldr.Dimtad = 0;
                        ldr.EvaluateLeader();
                    }
                    ldr.Annotative = AnnotativeStates.True;
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 162");
            }

            return idLdr;
        }

        public static ObjectId
        addLine(Point3d pntBEG, Point3d pntEND, string nameLayer = "0", short color = 256, LineWeight weight = LineWeight.ByLayer)
        {
            ObjectId idLine = ObjectId.Null;
            Line line = new Line();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    line.SetDatabaseDefaults();
                    line.StartPoint = pntBEG;
                    line.EndPoint = pntEND;

                    Layer.manageLayers(nameLayer);
                    line.Layer = nameLayer;

                    line.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    line.LineWeight = weight;
                    idLine = MS.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    tr.Commit();
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 195");
            }

            return idLine;
        }

        public static ObjectId
        addLineOffset(ObjectId idLine, double distOffset)
        {
            ObjectId idNewLine = ObjectId.Null;
            int side = -1;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Line line = (Line)tr.GetObject(idLine, OpenMode.ForRead);
                    double dir = line.Angle;
                    if (distOffset < 0)
                    {
                        side = 1;
                    }
                    Point3d pnt3dBeg = line.StartPoint.traverse(dir + System.Math.PI / 2 * side, System.Math.Abs(distOffset));
                    Point3d pnt3dEnd = line.EndPoint.traverse(dir + System.Math.PI / 2 * side, System.Math.Abs(distOffset));
                    Line newLine = new Line(pnt3dBeg, pnt3dEnd);
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    idNewLine = MS.AppendEntity(newLine);
                    tr.AddNewlyCreatedDBObject(newLine, true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 227");
            }
            return idNewLine;
        }

        public static ObjectId
        addPoly(Point2dCollection pnt2ds, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly = ObjectId.Null;
            Polyline poly = new Polyline();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    int i = -1;
                    poly.SetDatabaseDefaults();
                    poly.Layer = nameLayer;
                    poly.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    foreach (Point2d pnt2d in pnt2ds)
                    {
                        i = ++i;
                        poly.AddVertexAt(i, pnt2d, 0, 0, 0);
                    }

                    idPoly = MS.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 261");
            }

            return idPoly;
        }

        public static ObjectId
        addPoly(List<Point3d> pnt3ds, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly = ObjectId.Null;
            Point2d pnt2d;
            Polyline poly = new Polyline();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    int i = -1;
                    poly.SetDatabaseDefaults();
                    foreach (Point3d pnt3d in pnt3ds)
                    {
                        i = ++i;
                        pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                        poly.AddVertexAt(i, pnt2d, 0, 0, 0);
                    }
                    poly.Layer = nameLayer;
                    poly.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    idPoly = MS.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 297");
            }
            return idPoly;
        }

        public static ObjectId
        addPoly(Point3dCollection pnt3ds, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly = ObjectId.Null;
            Point2d pnt2d;
            Polyline poly = new Polyline();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    int i = -1;
                    poly.SetDatabaseDefaults();
                    foreach (Point3d pnt3d in pnt3ds)
                    {
                        i = ++i;
                        pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                        poly.AddVertexAt(i, pnt2d, 0, 0, 0);
                    }
                    poly.Layer = nameLayer;
                    poly.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    idPoly = MS.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 332");
            }
            return idPoly;
        }

        public static ObjectId
        addPoly(List<Vertex2d> vtxs2d, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly = ObjectId.Null;
            Polyline poly = new Polyline();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    int i = -1;
                    poly.SetDatabaseDefaults();
                    foreach (Vertex2d vtx2d in vtxs2d)
                    {
                        i = ++i;
                        Point2d pnt2d = new Point2d(vtx2d.Position.X, vtx2d.Position.Y);
                        poly.AddVertexAt(i, pnt2d, vtx2d.Bulge, vtx2d.StartWidth, vtx2d.EndWidth);
                    }
                    Layer.manageLayers(nameLayer);
                    poly.Layer = nameLayer;
                    poly.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);

                    idPoly = MS.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 368");
            }

            return idPoly;
        }

        public static ObjectId
        addPoly3d(this List<Point3d> pnt3ds, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Polyline3d poly3d = new Polyline3d();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable bt = (BlockTable)BaseObjs._db.BlockTableId.GetObject(OpenMode.ForRead);
                    BlockTableRecord MS = (BlockTableRecord)bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
                    
                    idPoly3d = MS.AppendEntity(poly3d);
                    tr.AddNewlyCreatedDBObject(poly3d, true);

                    poly3d.SetDatabaseDefaults();
                    Layer.manageLayers(nameLayer);
                    poly3d.Layer = nameLayer;
                    poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    foreach (Point3d pnt3d in pnt3ds)
                    {
                        PolylineVertex3d poly3dVertex = new PolylineVertex3d(pnt3d);
                        poly3d.AppendVertex(poly3dVertex);
                        tr.AddNewlyCreatedDBObject(poly3dVertex, true);
                    }
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 406");
            }
            return idPoly3d;
        }

        public static ObjectId
        addPoly3d(Point3d[] pnt3ds, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Polyline3d poly3d = new Polyline3d();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                    idPoly3d = MS.AppendEntity(poly3d);
                    tr.AddNewlyCreatedDBObject(poly3d, true);

                    int i = -1;
                    poly3d.SetDatabaseDefaults();
                    Layer.manageLayers(nameLayer);
                    poly3d.Layer = nameLayer;
                    poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);
                    foreach (Point3d pnt3d in pnt3ds)
                    {
                        i = ++i;
                        PolylineVertex3d poly3dVertex = new PolylineVertex3d(pnt3d);
                        poly3d.AppendVertex(poly3dVertex);
                        tr.AddNewlyCreatedDBObject(poly3dVertex, true);
                    }
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 443");
            }
            return idPoly3d;
        }

        public static ObjectId
        addPoly3d(Point3dCollection pnt3ds, string nameLayer = "0", short color = 256, LineWeight weight = LineWeight.ByLayer)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Polyline3d poly3d = null;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    BaseObjs._transactionManagerDoc.EnableGraphicsFlush(true);
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                        poly3d = new Polyline3d(Poly3dType.SimplePoly, pnt3ds, false);

                        Layer.manageLayers(nameLayer);
                        poly3d.Layer = nameLayer;
                        poly3d.Color = Color.FromColorIndex(ColorMethod.ByLayer, color);
                        poly3d.LineWeight = weight;

                        idPoly3d = MS.AppendEntity(poly3d);
                        tr.AddNewlyCreatedDBObject(poly3d, true);
                        tr.Commit();
                    }// end using
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 477");
            }
            return idPoly3d;
        }

        public static ObjectId
        addPoly3d(Point3d pnt3dBEG, Point3d pnt3dEND, string nameLayer = "0", short color = 256, LineWeight weight = LineWeight.ByLayer)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Point3dCollection pnts3d = new Point3dCollection() {
                pnt3dBEG,
                pnt3dEND
            };
            Polyline3d poly3d = null;
            Layer.manageLayers(nameLayer);
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        try
                        {
                            BlockTableRecord MS = Blocks.getBlockTableRecordMS();

                            poly3d = new Polyline3d(Poly3dType.SimplePoly, pnts3d, false);
                            poly3d.Layer = nameLayer;
                            poly3d.Color = Color.FromColorIndex(ColorMethod.ByLayer, color);
                            poly3d.LineWeight = weight;

                            MS.AppendEntity(poly3d);
                            idPoly3d = poly3d.ObjectId;
                            tr.AddNewlyCreatedDBObject(poly3d, true);
                            tr.Commit();
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 514");
                        }
                    }// end using
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 521");
            }
            return idPoly3d;
        }

        public static ObjectId
        addPoly3d(ObjectId idCgPntBEG, ObjectId idCgPntEND, string nameApp, string nameLayer = "0", short color = 256, LineWeight weight = LineWeight.ByLayer)
        {
            Point3d pnt3dBEG = idCgPntBEG.getCogoPntCoordinates();
            Point3d pnt3dEND = idCgPntEND.getCogoPntCoordinates();

            ObjectId idPoly3d = addPoly3d(pnt3dBEG, pnt3dEND, nameLayer, color, weight);
            idPoly3d.lnkPntsAndPoly3d(idCgPntBEG, idCgPntEND, nameApp);

            return idPoly3d;
        }


        public static ObjectId
        addPoly3d(Point3dCollection pnt3ds, string nameLayer)
        {
            ObjectId idPoly3d = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                        Polyline3d poly3d = new Polyline3d(Poly3dType.SimplePoly, pnt3ds, false);

                        Layer.manageLayers(nameLayer);
                        poly3d.Layer = nameLayer;

                        idPoly3d = MS.AppendEntity(poly3d);
                        tr.AddNewlyCreatedDBObject(poly3d, true);
                        tr.Commit();
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 561");
                    }
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 567");
            }

            return idPoly3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idPoly3d"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        addPolyFromPoly3d(ObjectId idPoly3d, string nameLayer = "0")
        {
            ObjectId idPoly = ObjectId.Null;
            Polyline poly = new Polyline();
            Layer.manageLayers(nameLayer);
            poly.Layer = nameLayer;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    int i = -1;
                    poly.SetDatabaseDefaults();
                    foreach (Point3d pnt3d in idPoly3d.getCoordinates3d())
                    {
                        i = ++i;
                        Point2d pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                        poly.AddVertexAt(i, pnt2d, 0, 0, 0);
                    }
                    idPoly = MS.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 606");
            }

            return idPoly;
        }

        public static ObjectId
        addPolygon(Point3d pnt3d, double radius, int numSides)
        {
            ObjectId idPoly = ObjectId.Null;
            Point2dCollection pnts2d = new Point2dCollection();

            Point2d pnt2dCen = new Point2d(pnt3d.X, pnt3d.Y);
            Point2d pnt2dBase = Geom.traverse_pnt2d(pnt2dCen, 3 * System.Math.PI / 2, radius);
            pnts2d.Add(pnt2dBase);

            double angle = 2 * System.Math.PI / numSides;
            double length = 2 * radius * System.Math.Sin(angle / 2);
            for (int i = 0; i < numSides; i++)
            {
                pnt2dBase = pnt2dBase.traverse(angle * (i + 1), length);
                pnts2d.Add(pnt2dBase);
            }

            idPoly = Base_Tools45.Draw.addPoly(pnts2d);
            return idPoly;
        }

        public static ObjectId
        addPolyOffset(ObjectId idPoly, double dist, string nameLayer = "0")
        {
            ObjectId idEnt = ObjectId.Null;
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord btrMS = Base_Tools45.Blocks.getBlockTableRecordMS();
                    Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForRead);
                    DBObjectCollection dbObjColl = poly.GetOffsetCurves(dist);
                    foreach (Entity ent in dbObjColl)
                    {
                        ent.Layer = nameLayer;
                        idEnt = btrMS.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 657");
            }
            return idEnt;
        }

        public static ObjectId
        addRectangle(Point3dCollection pnts3d)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Point3d ll = pnts3d[0];
            Point3d ur = pnts3d[1];

            Point3d lr = new Point3d(ur.X, ll.Y, 0.0);
            Point3d ul = new Point3d(ll.X, ur.Y, 0.0);

            pnts3d = new Point3dCollection {
                ll,
                lr,
                ur,
                ul,
                ll
            };
            idPoly3d = addPoly3d(pnts3d);
            return idPoly3d;
        }

        public static ObjectId
        addRegion(this ObjectId idEnt){
            ObjectId idReg = ObjectId.Null;
            using(var tr = BaseObjs.startTransactionDb()){
                BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                Polyline poly = (Polyline)tr.GetObject(idEnt, OpenMode.ForRead);

                DBObjectCollection dbObjs = new DBObjectCollection { poly };

                DBObjectCollection dbRegions = new DBObjectCollection();
                dbRegions = Region.CreateFromCurves(dbObjs);
                Region region = (Region)dbRegions[0];
                ms.AppendEntity(region);
                tr.AddNewlyCreatedDBObject(region, true);
                tr.Commit();
            }
            return idReg;
        }

        public static ObjectId
        addSymbolAndWipeout(Point3d pnt3d, double angleView, out ObjectId idWO, double radius, int numSides, bool addWipeOut = false)
        {
            ObjectId idSYM = ObjectId.Null;

            double pi = System.Math.PI;
            double pi2 = pi * 2.0;
            double angleBase = 0;
            double length = 0;
            double delta = 0;
            double scale = Misc.getCurrAnnoScale();
            double deltaBegin = 0;
            Point2d pnt2dBase = Point2d.Origin;
            Point2dCollection pnts2d = new Point2dCollection();
            Point2d pnt2dCen = new Point2d(pnt3d.X, pnt3d.Y);
            delta = 2 * pi / numSides;
            idWO = ObjectId.Null;

            switch (numSides)
            {
                case 3:
                    length = 1.33 * scale * 2 * radius;
                    pnt2dBase = pnt2dCen.traverse(angleView - pi / 2, length / 4);
                    pnt2dBase = pnt2dBase.traverse(angleView, length / 2);
                    pnts2d.Add(pnt2dBase);
                    deltaBegin = delta + angleView;
                    break;

                case 4:
                    angleBase = angleView + 7.0 / 8.0 * pi2;
                    length = 0.5 * 1.68 * scale * radius * System.Math.Sqrt(2);
                    pnt2dBase = pnt2dCen.traverse(angleBase, length);
                    pnts2d.Add(pnt2dBase);
                    length = scale * 1.68 * radius;
                    deltaBegin = delta + angleView;
                    break;

                case 6:
                    angleBase = angleView + 10.0 / 12.0 * pi2;
                    //angleBase = angleView;
                    length = 1.03 * scale * radius / System.Math.Cos(delta / 3);
                    pnt2dBase = pnt2dCen.traverse(angleBase, length);
                    pnts2d.Add(pnt2dBase); //pnt0
                    deltaBegin = angleBase + 2 * delta;
                    break;

                case 1024:
                    //angleBase = angleView + 3.0 / 2.0 * pi;
                    angleBase = angleView;
                    length = scale * 2.0 * radius * System.Math.Sin(delta / 2);
                    pnt2dBase = pnt2dCen.traverse(angleBase, scale * radius);
                    pnts2d.Add(pnt2dBase);
                    deltaBegin = angleBase + delta + pi / 2;
                    break;

                default:
                    break;
            }

            //adjust first segment to account for viewtwist
            pnt2dBase = pnt2dBase.traverse(deltaBegin, length);
            pnts2d.Add(pnt2dBase); //pnt1
            //pnt2dBase.addPointNode(34, 0.01);

            double deltaTotal = deltaBegin;
            //from then on it is
            for (int i = 1; i < numSides; i++)
            {
                deltaTotal = deltaTotal + delta;
                pnt2dBase = pnt2dBase.traverse(deltaTotal, length);
                pnts2d.Add(pnt2dBase); //pnts 2-6
                //pnt2dBase.addPointNode(34, 0.01);
            }

            if (numSides < 1024)
            {
                idSYM = addPoly(pnts2d, "BUBBLE", 5);
            }
            else
            {
                idSYM = Draw.addCircle(pnt3d, scale * radius, "BUBBLE", 5);
            }

            if (addWipeOut)
            {
                Wipeout wo = new Wipeout();
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                        wo.SetFrom(pnts2d, Vector3d.ZAxis);
                        wo.Layer = "BUBBLE";
                        wo.Color = Color.FromColorIndex(ColorMethod.ByBlock, 8);
                        ms.AppendEntity(wo);
                        tr.AddNewlyCreatedDBObject(wo, true);
                        idWO = wo.ObjectId;
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Draw.cs: line: 804");
                }
            }

            return idSYM;
        }

        public static ObjectId
        addTable(Point3d pnt3dIns, int rows, int cols, double rowHeight, double colWidth)
        {
            ObjectId idTable = ObjectId.Null;
            Table table = new Table();
            table.TableStyle = BaseObjs._db.Tablestyle;
            table.SetSize(rows, cols);
            foreach (Row row in table.Rows)
                row.Height = rowHeight;
            foreach (Column col in table.Columns)
                col.Width = colWidth;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable bt = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                idTable = btr.AppendEntity(table);
                tr.AddNewlyCreatedDBObject(table, true);
                tr.Commit();
            }

            return idTable;
        }
    }// Class Draw
}
