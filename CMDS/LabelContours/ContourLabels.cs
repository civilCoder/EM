using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace LabelContours
{
    public static class ContourLabels
    {
        public static void
        doLabels()
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("CMDECHO", 0);
            object osMode = SnapMode.getOSnap();

            SnapMode.setOSnap(512);

            ObjectId idSurface = ObjectId.Null;

            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        ObjectIdCollection ids = BaseObjs._civDoc.GetSurfaceIds();
                        switch (ids.Count)
                        {
                            case 0:
                                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("There are Zero surfaces in this drawing.  Exiting....");
                                return;

                            case 1:
                                idSurface = ids[0];
                                break;

                            default:
                                idSurface = getSurface();
                                break;
                        }
                        tr.Abort();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} ContourLabels.cs: line: 58", ex.Message));
                }

                if (!idSurface.IsNull)
                {
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ObjectId idStyleMajor = Surf_Styles.getSurfaceContourLabelStyleId("Thienes_Major");
                            ObjectId idStyleMinor = Surf_Styles.getSurfaceContourLabelStyleId("Thienes_Minor");

                            PromptPointResult ppr;
                            do
                            {
                                PromptPointOptions ppo = new PromptPointOptions("\nPick contour line.");
                                ppr = BaseObjs._editor.GetPoint(ppo);
                                if (ppr.Status == PromptStatus.OK)
                                {
                                    Point3d pnt3d = ppr.Value;

                                    Surface surf = (Surface)idSurface.GetObject(OpenMode.ForWrite);
                                    double dir = surf.FindDirectionAtXY(pnt3d.X, pnt3d.Y);

                                    Point3d pnt3dTar1 = Geom.traverse_pnt3d(pnt3d, dir + System.Math.PI, 0.5);

                                    Point3d pnt3dTar2 = Geom.traverse_pnt3d(pnt3d, dir, 0.5);
                                    Point2dCollection pnts2d = new Point2dCollection();

                                    pnts2d.Add(new Point2d(pnt3dTar1.X, pnt3dTar1.Y));
                                    pnts2d.Add(new Point2d(pnt3dTar2.X, pnt3dTar2.Y));

                                    doLabel(idSurface, pnts2d, idStyleMajor, idStyleMinor);
                                    //sclg.Annotative = AnnotativeStates.True;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            while (ppr.Status == PromptStatus.OK);

                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} ContourLabels.cs: line: 97", ex.Message));
                    }
                    finally
                    {
                        SnapMode.setOSnap((int)osMode);
                    }
                }
            }
        }

        public static void
        doLabel(ObjectId idSurface, Point2dCollection pnts2d, ObjectId idStyleMajor, ObjectId idStyleMinor)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId idLbl = SurfaceContourLabelGroup.Create(idSurface, pnts2d);
                    SurfaceContourLabelGroup sclg = (SurfaceContourLabelGroup)tr.GetObject(idLbl, OpenMode.ForWrite);
                    sclg.Mask = Autodesk.Civil.LabelMaskType.ObjectOnly;                    
                    sclg.MajorContourLabelStyleId = idStyleMajor;
                    sclg.MinorContourLabelStyleId = idStyleMinor;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} ContourLabels.cs: line: 24", ex.Message));
            }
        }

        private static ObjectId
        getSurface()
        {
            PromptEntityOptions peo = new PromptEntityOptions("Select Surface");
            peo.SetRejectMessage("Entity selected was of Type other then Surface.");
            peo.AddAllowedClass(typeof(Surface), false);

            PromptEntityResult per = BaseObjs._editor.GetEntity(peo);
            if (per.Status == PromptStatus.OK)
            {
                return per.ObjectId;
            }
            return ObjectId.Null;
        }
    }
}