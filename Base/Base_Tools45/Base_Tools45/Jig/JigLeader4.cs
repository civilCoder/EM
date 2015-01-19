using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45.Jig
{
    public class JigLeader4 : EntityJig
    {
        private Point3dCollection _mPts;
        private Point3d _mTempPoint;

        public JigLeader4()
            : base(new Leader())
        {
            // Store the string passed in
            //_mContents = contents;
            // Create a point collection to store our vertices
            _mPts = new Point3dCollection();

            // Create mleader and set defaults
            Leader ml = Entity as Leader;
            ml.SetDatabaseDefaults();

            // Set up the MText contents
            //ml.ContentType = ContentType.MTextContent;
            //MText mt = new MText();
            //mt.SetDatabaseDefaults();
            //mt.Contents = _mContents;
            //ml.MText = mt;
            //ml.TextAlignmentType = TextAlignmentType.LeftAlignment;
            //ml.TextAttachmentType = TextAttachmentType.AttachmentMiddle;

            // Set the frame and landing properties
            //ml.EnableDogleg = true;
            //ml.EnableFrameText = true;
            //ml.EnableLanding = true;

            // Reduce the standard landing gap
            //ml.LandingGap = 0.05;
            ml.IsSplined = true;
            // Add a leader, but not a leader line (for now)
            //_mLeaderIndex = ml.AddLeader();
        }

        public void addVertex()
        {
            Leader ml = Entity as Leader;

            // For the first point...
            if (_mPts.Count == 0)
            {
                // Add a leader line
                //_mLeaderLineIndex = ml.AddLeaderLine(_mLeaderIndex);
                // And a start vertex
                ml.AppendVertex(_mTempPoint);

                // Add a second vertex that will be set
                // within the jig
                ml.AppendVertex(new Point3d(0, 0, 0));
            }
            else
            {
                // For subsequent points,
                // just add a vertex
                ml.AppendVertex(_mTempPoint);
            }

            // Reset the attachment point, otherwise
            // it seems to get forgotten
            // ml.TextAttachmentType = TextAttachmentType.AttachmentMiddle;

            // Add the latest point to our history
            _mPts.Add(_mTempPoint);
        }

        public void removeLastVertex()
        {
            // We don't need to actually remove
            // the vertex, just reset it
            Leader ml = Entity as Leader;
            if (_mPts.Count >= 1)
            {
                ml.RemoveLastVertex();
                //Vector3d dogvec = ml.GetDogleg(_mLeaderIndex);
                //double doglen = ml.DoglegLength;
                //double landgap = ml.LandingGap;
                //ml.TextLocation = _mPts[_mPts.Count - 1] + ((doglen + landgap) * dogvec);
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
                    Leader ml = Entity as Leader;
                    ml.AppendVertex(_mTempPoint);
                    // Adjust the text location
                    //Vector3d dogvec = ml.GetDogleg(_mLeaderIndex);
                    //double doglen = ml.DoglegLength;
                    //double landgap = ml.LandingGap;
                    //ml.TextLocation = _mTempPoint + ((doglen + landgap) * dogvec);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigLeader4.cs: line: 142", ex.Message));
            }
            return true;
        }
    }
}