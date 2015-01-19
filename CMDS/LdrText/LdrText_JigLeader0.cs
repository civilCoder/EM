using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using System;


namespace LdrText
{
    public class LdrText_JigLeader0 : EntityJig
    {
        public static bool _mIsJigStarted;

        private Point3dCollection _mPts;
        private Point3d _mTempPoint;

        public LdrText_JigLeader0(Point3d pnt3d, double txtSize, string nameLayer, short color)
            : base(new Leader())
        {
            Layer.manageLayers(nameLayer);
            _mPts = new Point3dCollection();
            if (pnt3d != Pub.pnt3dO)
            {
                _mPts.Add(pnt3d);
                _mTempPoint = pnt3d;
            }

            Leader leader = Entity as Leader;
            leader.DimensionStyle = Dim.getDimStyleTableRecord("Annotative");
            leader.SetDatabaseDefaults();
            if (pnt3d != Pub.pnt3dO)
            {
                try
                {
                    leader.AppendVertex(pnt3d);
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " LdrText_JigLeader0.cs: line: 42");
                }
            }

            leader.IsSplined = false;
            leader.Annotative = AnnotativeStates.True;
            leader.HasArrowHead = true;
            leader.Dimasz = txtSize;
            leader.Layer = nameLayer;
            leader.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);

            _mIsJigStarted = false;
        }

        public static ObjectId
        jigLeader0(Point3d pnt3d, double txtSize, string nameLayer, short color)
        {
            ObjectId idLDR = ObjectId.Null;
            if (pnt3d == Pub.pnt3dO)
                return idLDR;
            Application.SetSystemVariable("POLARMODE", 4);

            Editor ed = BaseObjs._editor;

            LdrText_JigLeader0 jig = new LdrText_JigLeader0(pnt3d, txtSize, nameLayer, color);

            bool bSuccess = true, bComplete = false;
            int pntCount = 1;

            while (bSuccess && !bComplete)
            {
                _mIsJigStarted = false;

                PromptResult dragres = ed.Drag(jig);

                bSuccess = (dragres.Status == PromptStatus.OK);
                if (bSuccess)
                {
                    jig.addVertex();
                    pntCount++;
                }

                bComplete = (dragres.Status == PromptStatus.None);
            }

            if (bComplete)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    Leader ldr = (Leader)jig.getEntity();
                    Object ortho = Application.GetSystemVariable("ORTHOMODE");
                    int o;
                    int.TryParse(ortho.ToString(), out o);
                    if (o == 1)
                        ldr.RemoveLastVertex();
                    idLDR = ms.AppendEntity(ldr);
                    tr.AddNewlyCreatedDBObject(ldr, true);
                    tr.Commit();
                }
            }
            Application.SetSystemVariable("POLARMODE", 0);
            Application.SetSystemVariable("ORTHOMODE", 0);
            return idLDR;
        }

        public void addVertex()
        {
            Leader leader = Entity as Leader;

            leader.AppendVertex(_mTempPoint);

            _mPts.Add(_mTempPoint);
        }

        public void removeLastVertex()
        {
            Leader leader = Entity as Leader;
            if (_mPts.Count >= 1)
            {
                leader.RemoveLastVertex();
            }
        }

        public Entity getEntity()
        {
            return Entity;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions opts = new JigPromptPointOptions();

            opts.UserInputControls = (UserInputControls.Accept3dCoordinates |
                                      UserInputControls.AcceptMouseUpAsPoint |
                                      UserInputControls.DoNotUpdateLastPoint);

            switch (_mPts.Count)
            {
                case 1:
                    opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                    opts.Message = "\nPick Next Vertex: ";
                    opts.BasePoint = _mTempPoint;
                    opts.UseBasePoint = true;
                    break;

                default:
                    opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                    opts.BasePoint = _mPts[_mPts.Count - 1];
                    opts.UseBasePoint = true;
                    opts.SetMessageAndKeywords("\nPick Next/Last Vertex or [End]; ", "End");
                    break;
            }


            PromptPointResult res = prompts.AcquirePoint(opts);

            if (res.Status == PromptStatus.Keyword)
            {
                if (res.StringResult == "End")
                {
                    return SamplerStatus.Cancel;
                }
            }
            if (_mTempPoint == res.Value)
            {
                return SamplerStatus.NoChange;
            }
            if (res.Status == PromptStatus.OK)
            {
                _mTempPoint = res.Value;
                return SamplerStatus.OK;
            }
            return SamplerStatus.Cancel;
        }

        protected override bool Update()
        {
            try
            {
                Leader leader = (Leader)Entity;

                if (_mIsJigStarted)
                {
                    if (leader.NumVertices > 1)
                        leader.RemoveLastVertex();
                }

                Point3d lastVertex = leader.VertexAt(leader.NumVertices - 1);
                if (!_mTempPoint.Equals(lastVertex))
                {
                    leader.AppendVertex(_mTempPoint);
                    _mIsJigStarted = true;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LdrText_JigLeader0.cs: line: 199");
            }
            return true;
        }
    }
}
