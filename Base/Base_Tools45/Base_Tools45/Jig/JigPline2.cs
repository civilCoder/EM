using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Base_Tools45.Jig.JigPline2.PlineJig))] 

namespace Base_Tools45.Jig
{
    public class JigPline2
    {
        public class PlineJig : EntityJig
        {
            private Point3dCollection pnts3d;
            private Point3d pnt3dTmp;
            private Plane plane;

            public PlineJig(Matrix3d ucs)
                : base(new Polyline())
            {
                pnts3d = new Point3dCollection();
                Point3d pnt3dOrigin = Pub.pnt3dO;

                Vector3d v3d = new Vector3d(0, 0, 1);
                v3d = v3d.TransformBy(ucs);
                plane = new Plane(pnt3dOrigin, v3d);

                Polyline poly = Entity as Polyline;
                poly.SetDatabaseDefaults();
                poly.Normal = v3d;
                poly.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            }

            public Entity GetEntity()
            {
                return Entity;
            }

            public void AddLatestVertex()
            {
                pnts3d.Add(pnt3dTmp);
                Polyline poly = Entity as Polyline;
                poly.AddVertexAt(poly.NumberOfVertices, new Point2d(0, 0), 0, 0, 0);
            }

            public void RemoveLastVertex()
            {
                Polyline poly = Entity as Polyline;
                poly.RemoveVertexAt(pnts3d.Count);
            }

            /// <summary>
            /// Updates this instance.
            /// </summary>
            /// <returns></returns>
            protected override bool Update()
            {
                Polyline poly = Entity as Polyline;
                poly.SetPointAt(poly.NumberOfVertices - 1, pnt3dTmp.Convert2d(plane));
                return true;
            }

            /// <summary>
            /// Samplers the specified prompts.
            /// </summary>
            /// <param name="prompts">The prompts.</param>
            /// <returns></returns>
            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                JigPromptPointOptions jigOpts = new JigPromptPointOptions();
                jigOpts.UserInputControls = (UserInputControls.Accept3dCoordinates |
                                             UserInputControls.NullResponseAccepted |
                                             UserInputControls.NoNegativeResponseAccepted);
                if (pnts3d.Count == 0)
                {
                    jigOpts.Message = "\nPick start point of polyline: ";
                }
                else if (pnts3d.Count > 0)
                {
                    jigOpts.BasePoint = pnts3d.lastPoint();
                    jigOpts.UseBasePoint = true;
                    jigOpts.Message = "\nPick next vertex: ";
                }
                else
                    return SamplerStatus.Cancel;

                PromptPointResult res = prompts.AcquirePoint(jigOpts);
                if (pnt3dTmp == res.Value)
                {
                    return SamplerStatus.NoChange;
                }
                else if (res.Status == PromptStatus.OK)
                {
                    pnt3dTmp = res.Value;
                    return SamplerStatus.OK;
                }
                return SamplerStatus.Cancel;
            }
        }
    }
}