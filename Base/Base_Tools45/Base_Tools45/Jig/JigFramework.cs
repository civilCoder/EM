using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace Base_Tools45.Jig
{
    public abstract class Phase
    {
        public string Message;
        public object Value;

        public Phase(string msg)
        {
            Message = msg;
        }
    }

    public abstract class GeometryPhase : Phase
    {
        public Func<List<Phase>, Point3d, Vector3d> Offset;

        public GeometryPhase(string msg, Func<List<Phase>, Point3d, Vector3d> offset = null)
            : base(msg)
        {
            Offset = offset;
        }
    }

    public class DistancePhase : GeometryPhase
    {
        public DistancePhase(string msg, object defVal = null, Func<List<Phase>, Point3d, Vector3d> offset = null)
            : base(msg, offset)
        {
            Value = (defVal == null ? 0 : defVal);
        }
    }

    public class SolidDistancePhase : DistancePhase
    {
        public SolidDistancePhase(string msg, object defVal = null, Func<List<Phase>, Point3d, Vector3d> offset = null)
            : base(msg, defVal, offset)
        {
            Value = (defVal == null ? 1e-05 : defVal);
        }
    }

    public class PointPhase : GeometryPhase
    {
        public PointPhase(string msg, object defval = null, Func<List<Phase>, Point3d, Vector3d> offset = null)
            : base(msg, offset)
        {
            Value = (defval == null ? Pub.pnt3dO : defval);
        }
    }

    public class AnglePhase : GeometryPhase
    {
        public AnglePhase(string msg, object defval = null, Func<List<Phase>, Point3d, Vector3d> offset = null)
            : base(msg, offset)
        {
            Value = (defval == null ? 0 : defval);
        }
    }

    public class StringPhase : Phase
    {
        public StringPhase(string msg, string defval = null)
            : base(msg)
        {
            Value = (defval == null ? "" : defval);
        }
    }

    public class EntityJigFramework : EntityJig
    {
        // Member data
        private Matrix3d _ucs;

        private Entity _ent;
        private Point3d _pt;
        private int _phase;
        private List<Phase> _phases;
        private Func<Entity, List<Phase>, Point3d, Matrix3d, bool> _update;

        // Constructor
        public EntityJigFramework(Matrix3d ucs, Entity ent, Point3d pt, List<Phase> phases, Func<Entity, List<Phase>, Point3d, Matrix3d, bool> update)
            : base(ent)
        {
            _ucs = ucs;
            _ent = ent;
            _pt = pt;
            _phases = phases;
            _phase = 0;
            _update = update;
        }

        public Entity GetEntity()
        {
            return Entity;
        }

        // Move on to the next phase
        internal void NextPhase()
        {
            _phase++;
        }

        // Check whether we're at the last phase
        internal bool IsLastPhase()
        {
            return (_phase == _phases.Count - 1);
        }

        // Our method to perform the jig and step through the
        // phases until done
        internal void RunTillComplete(Editor ed, Transaction tr)
        {
            // Perform the jig operation in a loop
            while (true)
            {
                var res = ed.Drag(this);

                if (res.Status == PromptStatus.OK)
                {
                    if (!IsLastPhase())
                    {
                        // Progress the phase
                        NextPhase();
                    }
                    else
                    {
                        // Only commit when all phases have been accepted
                        tr.Commit();
                        return;
                    }
                }
                else
                {
                    // The user has cancelled: returning aborts the
                    // transaction
                    return;
                }
            }
        }

        // EntityJig protocol
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            // Get the current phase
            var p = _phases[_phase];

            // If we're dealing with a geometry-typed phase (distance,
            // point or angle input) we can use some common code

            var gp = p as GeometryPhase;
            if (gp != null)
            {
                JigPromptGeometryOptions opts;
                if (gp is DistancePhase)
                    opts = new JigPromptDistanceOptions();
                else if (gp is AnglePhase)
                    opts = new JigPromptAngleOptions();
                else if (gp is PointPhase)
                    opts = new JigPromptPointOptions();
                else // Should never happen
                    opts = null;

                // Set up the user controls

                opts.UserInputControls =
                    (UserInputControls.Accept3dCoordinates |
                     UserInputControls.NoZeroResponseAccepted |
                     UserInputControls.NoNegativeResponseAccepted);

                // All our distance inputs will be with a base point
                // (which means the initial base point or an offset from
                // that)

                opts.UseBasePoint = true;
                opts.Cursor = CursorType.RubberBand;
                opts.Message = p.Message;
                opts.BasePoint = (gp.Offset == null ? _pt.TransformBy(_ucs) : (_pt + gp.Offset.Invoke(_phases, _pt)).TransformBy(_ucs));

                // The acquisition method varies on the phase type

                if (gp is DistancePhase)
                {
                    var phase = (DistancePhase)gp;
                    var pdr = prompts.AcquireDistance((JigPromptDistanceOptions)opts);
                    if (pdr.Status == PromptStatus.OK)
                    {
                        // If the difference between the new value and its
                        // previous value is negligible, return "no change"
                        if (System.Math.Abs((double)phase.Value - pdr.Value) < Tolerance.Global.EqualPoint)
                            return SamplerStatus.NoChange;

                        // Otherwise we update the appropriate variable
                        // based on the phase

                        phase.Value = pdr.Value;
                        _phases[_phase] = phase;
                        return SamplerStatus.OK;
                    }
                }
                else if (gp is PointPhase)
                {
                    var phase = (PointPhase)gp;
                    var ppr = prompts.AcquirePoint((JigPromptPointOptions)opts);

                    if (ppr.Status == PromptStatus.OK)
                    {
                        // If the difference between the new value and its
                        // previous value is negligible, return "no change"
                        var tmp = ppr.Value.TransformBy(_ucs.Inverse());

                        if (tmp.DistanceTo((Point3d)phase.Value) < Tolerance.Global.EqualPoint)
                            return SamplerStatus.NoChange;

                        // Otherwise we update the appropriate variable
                        // based on the phase

                        phase.Value = tmp;
                        _phases[_phase] = phase;
                        return SamplerStatus.OK;
                    }
                }
                else if (gp is AnglePhase)
                {
                    var phase = (AnglePhase)gp;
                    var par = prompts.AcquireAngle((JigPromptAngleOptions)opts);

                    if (par.Status == PromptStatus.OK)
                    {
                        // If the difference between the new value and its
                        // previous value is negligible, return "no change"
                        if ((double)phase.Value - par.Value < Tolerance.Global.EqualPoint)
                            return SamplerStatus.NoChange;

                        // Otherwise we update the appropriate variable
                        // based on the phase

                        phase.Value = par.Value;
                        _phases[_phase] = phase;
                        return SamplerStatus.OK;
                    }
                }
            }
            else
            {
                // p is StringPhase
                var phase = (StringPhase)p;

                var psr = prompts.AcquireString(p.Message);

                if (psr.Status == PromptStatus.OK)
                {
                    phase.Value = psr.StringResult;
                    _phases[_phase] = phase;
                    return SamplerStatus.OK;
                }
            }
            return SamplerStatus.Cancel;
        }

        protected override bool Update()
        {
            // Right now we have an indiscriminate catch around our
            // entity update callback: this could be modified to be
            // more selective and/or to provide information on exceptions
            bool results = false;
            try
            {
                results = _update.Invoke(_ent, _phases, _pt, _ucs);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigFramework.cs: line: 236", ex.Message));
            }
            return results;
        }
    }
}