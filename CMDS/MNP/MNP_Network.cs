using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;

namespace MNP
{
    public static class MNP_Network
    {
        private static frmMNP fMNP = MNP_Forms.fMNP;

        public static ObjectId
        getProfileViewPart()
        {
            SelectionSet ss = Select.buildSSet(typeof(ProfileViewPart));
            ObjectId[] ids = ss.GetObjectIds();
            return ids[0];
        }

        public static void
        makePipeNetwork(ObjectId idAlign, string nameNetwork, string pipeType)
        {
            string pntDesc = fMNP.pntDesc;

            Point3d pnt3dPick = Pub.pnt3dO;
            if (idAlign == ObjectId.Null)
            {
                Autodesk.AutoCAD.DatabaseServices.Entity ent = Select.selectEntity(typeof(Alignment), "Select Alignment: ", "Selected object was not an alignment. Try again: ", out pnt3dPick);
                idAlign = ent.ObjectId;
            }

            Network net = null;
            ObjectIdCollection ids = CivilApplication.ActiveDocument.GetPipeNetworkIds();
            ObjectId idNet = ObjectId.Null;

            if (ids.Count == 0)
            {
                idNet = Network.Create(CivilApplication.ActiveDocument, ref nameNetwork);
            }
            else
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    net = (Network)ids[i].getEnt();
                    //if (net.Name == nameNetwork){
                    //    string index = Align.getAlignIndex(nameNetwork);
                    //    nameNetwork = string.Format("{0}-{1}", nameNetwork, index);
                    //    break;
                    //}
                }
                idNet = Network.Create(CivilApplication.ActiveDocument, ref nameNetwork);
            }

            bool exists = false;

            Alignment align = (Alignment)idAlign.getEnt();
            AlignmentEntityCollection ents = align.Entities;
            TinSurface tinSurfDE = Surf.getTinSurface("CPNT-ON", out exists);

            List<AlgnEntData> algnData = MNP_Align.sortAlignEnts(align);
            List<string> hPipe = new List<string>();
            List<ObjectId> idsCgPnt = new List<ObjectId>();

            ObjectId idPipe, idPipeSize, idStruct, idStructSize;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                net = (Network)tr.GetObject(idNet, OpenMode.ForWrite);
                net.ReferenceAlignmentId = idAlign;
                net.ReferenceSurfaceId = Surf.getTinSurface("CPNT-ON", out exists).ObjectId;

                ObjectId idPartsList = CivilApplication.ActiveDocument.Styles.PartsListSet["Standard"];
                PartsList partsList = (PartsList)tr.GetObject(idPartsList, OpenMode.ForRead);

                idPipe = partsList["Concrete Pipe"];
                PartFamily partFamily = (PartFamily)tr.GetObject(idPipe, OpenMode.ForWrite);

                idPipeSize = partFamily[0];

                PartSize partSize = (PartSize)tr.GetObject(idPipeSize, OpenMode.ForRead);
                PartDataRecord pdr = partSize.SizeDataRecord;
                PartDataField[] pdf = pdr.GetAllDataFields();

                for (int i = 0; i < pdf.Length; i++)
                    System.Diagnostics.Debug.Print(string.Format("{0}: {1}", pdf[i].Description, pdf[i].Value.ToString()));

                idStruct = partsList["Rectangular Structure Slab Top Rectangular Frame"];
                partFamily = (PartFamily)tr.GetObject(idStruct, OpenMode.ForWrite);
                idStructSize = partFamily[0];

                double depth = -6, slope = 0, dZ = 0, top = 0;
                double diam = double.Parse(pdf[0].Value.ToString()) / 12;

                ObjectId idPipeNew = ObjectId.Null;

                AlignmentEntity ent = null;
                Structure sPrev = null;
                uint pntNum = 0;

                ObjectId idCgPntBeg, idCgPntEnd, idCgPntPrev = ObjectId.Null;
                TypedValue[] tvs;

                for (int i = 0; i < algnData.Count; i++)
                {
                    if (algnData.Count == 1)
                    {
                        ent = ents[0];
                    }
                    else
                    {
                        ent = ents.EntityAtId(algnData[i].ID);
                    }

                    ObjectId idStructNew = ObjectId.Null;
                    Structure s = null;
                    Pipe p = null;

                    if (ent.EntityType == AlignmentEntityType.Line)
                    {
                        AlignmentLine line = (AlignmentLine)ent;
                        Point2d pnt2dBeg = line.StartPoint;
                        Point2d pnt2dEnd = line.EndPoint;

                        try
                        {
                            top = tinSurfDE.FindElevationAtXY(pnt2dBeg.X, pnt2dBeg.Y) + depth;
                        }
                        catch (System.Exception)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Pipe endpoint is beyond limit of design surface");
                            return;
                        }

                        Point3d pnt3dBeg = new Point3d(pnt2dBeg.X, pnt2dBeg.Y, top);

                        try
                        {
                            top = tinSurfDE.FindElevationAtXY(pnt2dEnd.X, pnt2dEnd.Y) + depth;
                        }
                        catch (System.Exception)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Pipe endpoint is beyond limit of design surface");
                            return;
                        }

                        Point3d pnt3dEnd = new Point3d(pnt2dEnd.X, pnt2dEnd.Y, top);

                        LineSegment3d seg3d = new LineSegment3d(pnt3dBeg, pnt3dEnd);

                        net.AddLinePipe(idPipe, idPipeSize, seg3d, ref idPipeNew, true);

                        p = (Pipe)idPipeNew.getEnt();
                        p.AddToProfileView(fMNP.idProfileView);

                        ObjectId idPart = getProfileViewPart();

                        slope = pnt3dBeg.getSlope(pnt3dEnd);

                        dZ = diam / (2 * (System.Math.Cos(System.Math.Atan(System.Math.Abs(slope)))));

                        if (i == 0)
                        {
                            pnt3dBeg = new Point3d(pnt3dBeg.X, pnt3dBeg.Y, pnt3dBeg.Z - dZ);
                            pnt3dEnd = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, pnt3dEnd.Z - dZ);

                            idCgPntBeg = pnt3dBeg.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntBeg);

                            idCgPntEnd = pnt3dEnd.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntEnd);

                            idCgPntPrev = idCgPntEnd;

                            tvs = new TypedValue[3];
                            tvs.SetValue(new TypedValue(1001, apps.lnkMNP), 0);
                            tvs.SetValue(new TypedValue(1005, idCgPntBeg.getHandle().ToString()), 1);
                            tvs.SetValue(new TypedValue(1005, idCgPntEnd.getHandle().ToString()), 2);

                            //idPart.setXData(rb, apps.lnkMNP);
                            idPipeNew.setXData(tvs, apps.lnkMNP);

                            net.AddStructure(idStruct, idStructSize, pnt3dBeg, 0, ref idStructNew, true);
                            s = (Structure)idStructNew.getEnt();
                            s.AddToProfileView(fMNP.idProfileView);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.Start);

                            net.AddStructure(idStruct, idStructSize, pnt3dEnd, 0, ref idStructNew, true);
                            s = (Structure)idStructNew.getEnt();
                            s.AddToProfileView(fMNP.idProfileView);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.End);

                            sPrev = s;
                            hPipe.Add(idPipeNew.getHandle().ToString());
                        }
                        else
                        {
                            pnt3dEnd = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, pnt3dEnd.Z - dZ);
                            idCgPntEnd = pnt3dEnd.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntEnd);

                            tvs = new TypedValue[3];
                            tvs.SetValue(new TypedValue(1001, apps.lnkMNP), 0);
                            tvs.SetValue(new TypedValue(1005, idCgPntPrev.getHandle().ToString()), 1);
                            tvs.SetValue(new TypedValue(1005, idCgPntEnd.getHandle().ToString()), 2);

                            //idPart.setXData(rb, apps.lnkMNP);
                            idPipeNew.setXData(tvs, apps.lnkMNP);

                            idCgPntPrev = idCgPntEnd;

                            net.AddStructure(idStruct, idStructSize, pnt3dEnd, 0, ref idStructNew, true);
                            s = (Structure)idStructNew.getEnt();
                            s.AddToProfileView(fMNP.idProfileView);
                            sPrev.ConnectToPipe(idPipeNew, ConnectorPositionType.Start);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.End);

                            sPrev = s;
                            hPipe.Add(idPipeNew.getHandle().ToString());
                        }
                    }
                    else if (ent.EntityType == AlignmentEntityType.Arc)
                    {
                        AlignmentArc arc = (AlignmentArc)ent;
                        double radius = arc.Radius;

                        Point2d pnt2dBeg = arc.StartPoint;
                        Point2d pnt2dEnd = arc.EndPoint;

                        try
                        {
                            top = tinSurfDE.FindElevationAtXY(pnt2dBeg.X, pnt2dBeg.Y) + depth;
                        }
                        catch (System.Exception)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Pipe endpoint is beyond limit of design surface");
                            return;
                        }
                        Point3d pnt3dBeg = new Point3d(pnt2dBeg.X, pnt2dBeg.Y, top);

                        try
                        {
                            top = tinSurfDE.FindElevationAtXY(pnt2dEnd.X, pnt2dEnd.Y) + depth;
                        }
                        catch (System.Exception)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Pipe endpoint is beyond limit of design surface");
                            return;
                        }

                        IntPtr iptr = (IntPtr)0;
                        Point3d pnt3dEnd = new Point3d(pnt2dEnd.X, pnt2dEnd.Y, top);

                        Arc a = new Arc();
                        a.Radius = arc.Radius;
                        a.StartPoint = new Point3d(pnt2dBeg.X, pnt2dBeg.Y, 0);
                        a.EndPoint = new Point3d(pnt2dEnd.X, pnt2dEnd.Y, 0);
                        a.Center = new Point3d(arc.CenterPoint.X, arc.CenterPoint.Y, 0);

                        Curve3d c = (Curve3d)a.GetGeCurve();

                        net.AddCurvePipe(idPipe, idPipeSize, c, arc.Clockwise, ref idPipeNew, true);

                        p = (Pipe)idPipeNew.getEnt();
                        p.AddToProfileView(fMNP.idProfileView);

                        ObjectId idPart = getProfileViewPart();
                        slope = (pnt3dEnd.Z - pnt3dBeg.Z) / arc.Length;

                        dZ = diam / (2 * (System.Math.Cos(System.Math.Atan(System.Math.Abs(slope)))));

                        if (i == 0)
                        {
                            pnt3dBeg = new Point3d(pnt3dBeg.X, pnt3dBeg.Y, pnt3dBeg.Z - dZ);
                            pnt3dEnd = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, pnt3dEnd.Z - dZ);

                            idCgPntBeg = pnt3dBeg.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntBeg);

                            idCgPntEnd = pnt3dEnd.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntEnd);

                            tvs = new TypedValue[3];
                            tvs.SetValue(new TypedValue(1001, apps.lnkMNP), 0);
                            tvs.SetValue(new TypedValue(1005, idCgPntBeg.getHandle().ToString()), 1);
                            tvs.SetValue(new TypedValue(1005, idCgPntEnd.getHandle().ToString()), 2);

                            //idPart.setXData(rb, apps.lnkMNP);
                            idPipeNew.setXData(tvs, apps.lnkMNP);

                            net.AddStructure(idStruct, idStructSize, pnt3dBeg, 0, ref idStructNew, true);
                            s = (Structure)tr.GetObject(idStructNew, OpenMode.ForWrite);
                            s.AddToProfileView(fMNP.idProfileView);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.Start);

                            net.AddStructure(idStruct, idStructSize, pnt3dBeg, 0, ref idStructNew, true);
                            s = (Structure)tr.GetObject(idStructNew, OpenMode.ForWrite);
                            s.AddToProfileView(fMNP.idProfileView);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.End);
                            sPrev = s;
                            hPipe.Add(idPipeNew.getHandle().ToString());
                        }
                        else
                        {
                            pnt3dEnd = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, pnt3dEnd.Z - dZ);

                            idCgPntEnd = pnt3dEnd.setPoint(out pntNum, pntDesc);
                            idsCgPnt.Add(idCgPntEnd);

                            tvs = new TypedValue[3];
                            tvs.SetValue(new TypedValue(1001, apps.lnkMNP), 0);
                            tvs.SetValue(new TypedValue(1005, idCgPntPrev.getHandle().ToString()), 1);
                            tvs.SetValue(new TypedValue(1005, idCgPntEnd.getHandle().ToString()), 2);

                            //idPart.setXData(rb, apps.lnkMNP);
                            idPipeNew.setXData(tvs, apps.lnkMNP);

                            idCgPntPrev = idCgPntEnd;

                            net.AddStructure(idStruct, idStructSize, pnt3dBeg, 0, ref idStructNew, true);
                            s = (Structure)tr.GetObject(idStructNew, OpenMode.ForWrite);
                            s.AddToProfileView(fMNP.idProfileView);
                            sPrev.ConnectToPipe(idPipeNew, ConnectorPositionType.Start);
                            s.ConnectToPipe(idPipeNew, ConnectorPositionType.End);
                            sPrev = s;
                            hPipe.Add(idPipeNew.getHandle().ToString());
                        }
                    }
                }
                tr.Commit();
            }

            hPipe.Insert(0, "-1");
            hPipe.Add("-1");

            for (int i = 0; i < idsCgPnt.Count; i++)
            {
                TypedValue[] tvs = new TypedValue[3];
                tvs.SetValue(new TypedValue(1001, apps.lnkMNP), 0);
                tvs.SetValue(new TypedValue(1005, hPipe[i]), 1);
                tvs.SetValue(new TypedValue(1005, hPipe[i + 1]), 2);
                idsCgPnt[i].setXData(tvs, apps.lnkMNP);
            }
        }
    }
}