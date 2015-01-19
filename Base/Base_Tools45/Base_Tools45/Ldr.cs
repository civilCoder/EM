using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;

using System;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Ldr
    {

        /// <summary>
        ///     
        /// </summary>
        /// <param name="pnt3ds"></param>
        /// <param name="idLayer"></param>
        /// <param name="sizeArrow"></param>
        /// <param name="sizeGap"></param>
        /// <param name="mTxt"></param>
        /// <returns></returns>
        public static ObjectId
        addLdr(Point3dCollection pnt3ds, ObjectId idLayer, double sizeArrow, double sizeGap, Color color,
            ObjectId idMTxt, string nameStyle = "Annotative", bool spline = false)
        {
            ObjectId idLdr = ObjectId.Null;
            Leader ldr = new Leader();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = Blocks.getBlockTable();
                    BlockTableRecord MS = (BlockTableRecord)tr.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    ObjectId idDimStyle = Dim.getDimStyleTableRecord("Annotative");
                    DimStyleTableRecord dstr = (DimStyleTableRecord)tr.GetObject(idDimStyle, OpenMode.ForRead);

                    ldr.SetDatabaseDefaults();
                    ldr.HasArrowHead = true;
                    ldr.DimensionStyle = idDimStyle;
                    ldr.SetDimstyleData(dstr);
                    ldr.LayerId = idLayer;
                    ldr.Dimasz = sizeArrow;
                    ldr.Dimgap = sizeGap;
                    ldr.IsSplined = spline;
                    ldr.Color = color;
                    ldr.Annotative = AnnotativeStates.True;

                    for (int i = 0; i < pnt3ds.Count; i++)
                        try
                        {
                            ldr.AppendVertex(pnt3ds[i]);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Ldr.cs: line: 60");
                        }

                    idLdr = MS.AppendEntity(ldr);
                    tr.AddNewlyCreatedDBObject(ldr, true);

                    if (!idMTxt.IsNull)
                    {
                        ldr.Annotative = AnnotativeStates.True;
                        ldr.Annotation = idMTxt;
                        ldr.Dimtad = 0;
                        ldr.EvaluateLeader();
                    }

                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Ldr.cs: line: 79");
            }

            return idLdr;
        }

        public static ObjectId
        drawLdr(Point3d pnt3d, double txtSize, string nameLayer, short color)
        {
            ObjectId idLdr = ObjectId.Null;
            bool Stop = false;
            bool escape = false;
            PromptStatus ps;
            ObjectIdCollection ids = new ObjectIdCollection();
            Point3dCollection pnts3d = new Point3dCollection();

            pnts3d.Add(pnt3d);

            int k = 0;
            do
            {
                k = pnts3d.Count;
                pnt3d = UserInput.getPoint("\nPick Next Vertex: ", pnts3d[k - 1], out escape, out ps, osMode: 0);
                if (ps == PromptStatus.Cancel)
                {
                    ids.delete();
                    return ObjectId.Null;
                }

                if (ps == PromptStatus.None)
                {
                    switch (ids.Count)
                    {
                        case 0:
                            return ObjectId.Null;

                        default:
                            Stop = true;
                            idLdr = Draw.addLdr(pnts3d, false, nameLayer: "ARROW");
                            ids.delete();
                            break;
                    }
                }
                else
                {
                    ids.Add(Draw.addLine(pnts3d[k - 1], pnt3d));
                    pnts3d.Add(pnt3d);
                }
            }
            while (!Stop);
            return idLdr;
        }


        /// <summary>
        /// Gets the first LDR point.
        /// </summary>
        /// <param name="pnt3dPick">The PNT3D.</param>
        /// <param name="canLdr">if set to <c>true</c> [can LDR].</param>
        /// <param name="idTarget">The identifier target.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static Entity
        getFirstLdrPoint(out Point3d pnt3dPick, out bool canLdr, out Handle hEntX, out string layerTarget, out FullSubentityPath path)
        {
            bool escape = false;
            canLdr = false;
            hEntX = "0000".stringToHandle();

            Entity entX = xRef.getNestedEntity("\nPick Leader Start Point on Target (vertex #1): ", out escape, out pnt3dPick, out layerTarget, out path);
            if (escape || pnt3dPick == Pub.pnt3dO)
            {
                canLdr = true;
                return entX;
            }

            hEntX = entX.Handle;
            layerTarget = entX.Layer;

            return entX;

            //if (escape || pnt3d == Pub.pnt3dOrg || ps == PromptStatus.None) {
            //    canLdr = true;
            //    escape = true;
            //    idTarget = ObjectId.Null;
            //}
            //else {
            //    idsTarget = Select.getEntityatPoint(pnt3d);
            //    if (idsTarget != null) {
            //        switch (idsTarget.Length) {
            //            case 0:
            //                Application.ShowAlertDialog("\nEntity selection failed. Exiting..");
            //                canLdr = true;
            //                escape = false;
            //                idTarget = ObjectId.Null;
            //                pnt3d = Pub.pnt3dOrg;
            //                break;
            //            case 1:
            //                idTarget = idsTarget[0];
            //                canLdr = false;
            //                escape = false;
            //                break;
            //            default:
            //                Application.ShowAlertDialog(
            //                    "\nMultiple Entities found at selected location. \n\n Try selection again after isolating Target Entity");
            //                canLdr = true;
            //                escape = false;
            //                idTarget = ObjectId.Null;
            //                break;
            //        }
            //    }
            //    else {
            //        canLdr = true;
            //        escape = false;
            //        idTarget = ObjectId.Null;
            //    }

            //}
        }

        public static ObjectId
                getLdrAnnotationObject(ObjectId idLdr)
        {
            ObjectId idMTxt = ObjectId.Null;
            using (var tr = BaseObjs.startTransactionDb())
            {
                Leader ldr = (Leader)tr.GetObject(idLdr, OpenMode.ForRead);
                idMTxt = ldr.Annotation;
                tr.Commit();
            }
            return idMTxt;
        }

        public static int
                getLdrVerticeCount(ObjectId idLdr)
        {
            int numV = 0;
            using (var tr = BaseObjs.startTransactionDb())
            {
                Leader ldr = (Leader)tr.GetObject(idLdr, OpenMode.ForRead);
                numV = ldr.NumVertices;
                tr.Commit();
            }
            return numV;
        }

        public static void
                setLdrVertex(ObjectId idLdr, int indx, Point3d pnt3d)
        {
            using (var tr = BaseObjs.startTransactionDb())
            {
                Leader ldr = (Leader)tr.GetObject(idLdr, OpenMode.ForRead);
                ldr.SetVertexAt(indx, pnt3d);
                tr.Commit();
            }
        }

        /// <summary>
        /// Sets the LDR xdata.
        /// </summary>
        /// <param name="pnt3dEnd">The PNT3D end.</param>
        /// <param name="idLdr">The identifier LDR.</param>
        /// <param name="idSM">The identifier sm.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool
        setLdrXData(Point3d pnt3dEnd, ObjectId idLdr, ObjectId idSM)
        {
            bool success = false;
            TypedValue[] tvsLDR = new TypedValue[4];
            tvsLDR.SetValue(new TypedValue(1001, apps.lnkBubsLdrEndPnt), 0);
            tvsLDR.SetValue(new TypedValue(1040, pnt3dEnd.X), 1);
            tvsLDR.SetValue(new TypedValue(1040, pnt3dEnd.Y), 2);
            if (idSM.IsValid)
                tvsLDR.SetValue(new TypedValue(1005, idSM.getHandle()), 3);
            else
            {
                tvsLDR.SetValue(new TypedValue(1005, "0000".stringToHandle()), 3);
            }
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idLdr.setXData(tvsLDR, apps.lnkBubsLdrEndPnt);
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Ldr.cs: line: 266");
            }
            return success;
        }

        /// <summary>
        /// Updates the LDR xdata.
        /// </summary>
        /// <param name="idLdr">The identifier LDR.</param>
        public static void
        updateLdrXData(ObjectId idLdr)
        {
            ResultBuffer rbLdr = idLdr.getXData(apps.lnkBubsLdrEndPnt);
            TypedValue[] tvs = rbLdr.AsArray();
            Handle h = tvs[3].Value.ToString().stringToHandle();

            Point3d pnt3dEnd = idLdr.getEndPnt();

            TypedValue[] tvsLDR = new TypedValue[4];
            tvsLDR.SetValue(new TypedValue(1001, apps.lnkBubsLdrEndPnt), 0);
            tvsLDR.SetValue(new TypedValue(1040, pnt3dEnd.X), 1);
            tvsLDR.SetValue(new TypedValue(1040, pnt3dEnd.Y), 2);
            tvsLDR.SetValue(new TypedValue(1005, h), 3);

            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idLdr.setXData(tvsLDR, apps.lnkBubsLdrEndPnt);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Ldr.cs: line: 299");
            }
        }
    }
}
