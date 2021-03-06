using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Base_Tools45.Jig
{
    public class JigSplineLeader_BB : EntityJig
    {
        public static bool _mIsJigStarted;

        private Point3dCollection _mPts;
        private Point3d _mTempPoint;

        public JigSplineLeader_BB(Point3d pnt3dFirst, double txtSize, string nameLayer, Color color)
            : base(new Leader())
        {
            // Create a point collection to store our vertices
            _mPts = new Point3dCollection();
            _mPts.Add(pnt3dFirst);
            Layer.manageLayers(nameLayer);
            Leader leader = Entity as Leader;
            leader.DimensionStyle = Dim.getDimStyleTableRecord("Annotative");
            leader.SetDatabaseDefaults();
            try
            {
                leader.AppendVertex(pnt3dFirst);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} BB_JigSplineLeader.cs: line: 30", ex.Message));
            }

            leader.IsSplined = true;
            leader.Annotative = AnnotativeStates.True;
            leader.HasArrowHead = true;
            leader.Dimasz = txtSize;
            leader.Layer = nameLayer;
            leader.Color = color;

            _mIsJigStarted = false;
        }

        [CommandMethod("MYSL0")]
        public static void
        MySplineLeader0()
        {
            Color color = Misc.getColorByBlock(5);
            jigSplineLeader(new Point3d(0, 0, 0), 0.09, "0", color);
        }

        public static ObjectId
        jigSplineLeader(Point3d pnt3dFirst, double txtSize, string nameLayer, Color color)
        {
            ObjectId idLDR = ObjectId.Null;
            Editor ed = BaseObjs._editor;

            JigSplineLeader_BB jig = new JigSplineLeader_BB(pnt3dFirst, txtSize, nameLayer, color);

            // Loop to set vertices
            bool bSuccess = true, bComplete = false;
            int pntCount = 0;

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

                bComplete = (dragres.Status == PromptStatus.None || pntCount == 3);
            }

            if (bComplete)
            {
                // Append entity
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                        Leader ldr = (Leader)jig.getEntity();

                        idLDR = ms.AppendEntity(ldr);
                        tr.AddNewlyCreatedDBObject(ldr, true);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} BB_JigSplineLeader.cs: line: 87", ex.Message));
                }
            }
            return idLDR;
        }

        public void addVertex()
        {
            Leader leader = Entity as Leader;

            leader.AppendVertex(_mTempPoint);

            // Add the latest point to our history
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

            // Not all options accept null response
            opts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.NoNegativeResponseAccepted);

            // Get the first point
            if (_mPts.Count == 0 || _mPts[0] == Pub.pnt3dO)
            {
                opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                opts.Message = "\nPick Leader Start Point on Target (vertex #1):";
                opts.UseBasePoint = false;
            }
            else if (_mPts.Count == 1)
            {
                opts.BasePoint = _mPts[_mPts.Count - 1];
                opts.UseBasePoint = true;
                opts.Message = "\nPick leader vertex (#2): ";
            }
            else if (_mPts.Count == 2)
            {
                Base_Tools45.SnapMode.setOSnap(512);
                opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                opts.BasePoint = _mPts[_mPts.Count - 1];
                opts.UseBasePoint = true;
                opts.Message = "\nPick last vertex (#3): ";
            }
            else // Should never happen

                return SamplerStatus.Cancel;

            PromptPointResult res = prompts.AcquirePoint(opts);

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
                // Set the last vertex to the new value
                Leader leader = Entity as Leader;

                if (_mIsJigStarted)
                {
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
                BaseObjs.writeDebug(string.Format("{0} BB_JigSplineLeader.cs: line: 165", ex.Message));
            }
            return true;
        }
    }
}