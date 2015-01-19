using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45.Jig
{
    public class JigMLeader : EntityJig
    {
        private Point3dCollection _mPts;
        private Point3d _mTempPoint;
        private string _mContents;
        private int _mLeaderIndex;
        private int _mLeaderLineIndex;

        public JigMLeader(string contents)
            : base(new MLeader())
        {
            // Store the string passed in
            _mContents = contents;

            // Create a point collection to store our vertices
            _mPts = new Point3dCollection();

            // Create mleader and set defaults

            MLeader ml = Entity as MLeader;
            ml.SetDatabaseDefaults();

            // Set up the MText contents
            ml.ContentType = ContentType.MTextContent;
            MText mt = new MText();
            mt.SetDatabaseDefaults();
            mt.Contents = _mContents;
            ml.MText = mt;
            ml.TextAlignmentType = TextAlignmentType.LeftAlignment;
            ml.TextAttachmentType = TextAttachmentType.AttachmentMiddle;

            // Set the frame and landing properties
            ml.EnableDogleg = true;
            ml.EnableFrameText = true;
            ml.EnableLanding = true;

            // Reduce the standard landing gap
            ml.LandingGap = 0.05;

            // Add a leader, but not a leader line (for now)
            _mLeaderIndex = ml.AddLeader();
            _mLeaderLineIndex = -1;
        }

        public static void
        MyMLeaderJig()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // Get the text outside of the jig

            PromptStringOptions pso = new PromptStringOptions("\nEnter text: ");
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);
            if (pr.Status == PromptStatus.OK)
            {
                // Create MleaderJig
                JigMLeader jig = new JigMLeader(pr.StringResult);

                // Loop to set vertices

                bool bSuccess = true, bComplete = false;
                while (bSuccess && !bComplete)
                {
                    PromptResult dragres = ed.Drag(jig);
                    bSuccess =
                        (dragres.Status == PromptStatus.OK);
                    if (bSuccess)
                        jig.addVertex();
                    bComplete =
                        (dragres.Status == PromptStatus.None);
                    if (bComplete)
                        jig.removeLastVertex();
                }

                if (bComplete)
                {
                    // Append entity
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            BlockTable bt =
                                (BlockTable)tr.GetObject(
                                    db.BlockTableId,
                                    OpenMode.ForRead,
                                    false);
                            BlockTableRecord btr =
                                (BlockTableRecord)tr.GetObject(
                                    bt[BlockTableRecord.ModelSpace],
                                    OpenMode.ForWrite,
                                    false);
                            btr.AppendEntity(jig.getEntity());
                            tr.AddNewlyCreatedDBObject(
                                jig.getEntity(),
                                true);
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} JigMLeader.cs: line: 101", ex.Message));
                    }
                }
            }
        }

        public void addVertex()
        {
            MLeader ml = Entity as MLeader;

            // For the first point...
            if (_mPts.Count == 0)
            {
                // Add a leader line
                _mLeaderLineIndex = ml.AddLeaderLine(_mLeaderIndex);

                // And a start vertex
                ml.AddFirstVertex(_mLeaderLineIndex, _mTempPoint);

                // Add a second vertex that will be set
                // within the jig
                ml.AddLastVertex(_mLeaderLineIndex, new Point3d(0, 0, 0));
            }
            else
            {
                // For subsequent points,
                // just add a vertex
                ml.AddLastVertex(_mLeaderLineIndex, _mTempPoint);
            }

            // Reset the attachment point, otherwise
            // it seems to get forgotten
            ml.TextAttachmentType = TextAttachmentType.AttachmentMiddle;

            // Add the latest point to our history
            _mPts.Add(_mTempPoint);
        }

        public void removeLastVertex()
        {
            // We don't need to actually remove
            // the vertex, just reset it
            MLeader ml = Entity as MLeader;
            if (_mPts.Count >= 1)
            {
                Vector3d dogvec = ml.GetDogleg(_mLeaderIndex);
                double doglen = ml.DoglegLength;
                double landgap = ml.LandingGap;
                ml.TextLocation = _mPts[_mPts.Count - 1] + ((doglen + landgap) * dogvec);
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
            opts.UserInputControls = (UserInputControls.Accept3dCoordinates |
                                      UserInputControls.NoNegativeResponseAccepted);

            // Get the first point
            if (_mPts.Count == 0)
            {
                opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                opts.Message = "\nStart point of mleader: ";
                opts.UseBasePoint = false;
            }
            else if (_mPts.Count == 1)
            {
                opts.BasePoint = _mPts[_mPts.Count - 1];
                opts.UseBasePoint = true;
                opts.Message = "\nSpecify mleader vertex: ";
            }
            else if (_mPts.Count > 1)
            {
                opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                opts.BasePoint = _mPts[_mPts.Count - 1];
                opts.UseBasePoint = true;
                opts.SetMessageAndKeywords("\nSpecify mleader vertex or [End]: ", "End");
            }
            else // Should never happen
                return SamplerStatus.Cancel;

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
            else if (res.Status == PromptStatus.OK)
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
                if (_mPts.Count > 0)
                {
                    // Set the last vertex to the new value
                    MLeader ml = Entity as MLeader;
                    ml.SetLastVertex(_mLeaderLineIndex, _mTempPoint);

                    // Adjust the text location
                    Vector3d dogvec = ml.GetDogleg(_mLeaderIndex);
                    double doglen = ml.DoglegLength;
                    double landgap = ml.LandingGap;
                    ml.TextLocation = _mTempPoint + ((doglen + landgap) * dogvec);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigMLeader.cs: line: 207", ex.Message));
            }
            return true;
        }
    }
}