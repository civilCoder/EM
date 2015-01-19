using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System.Collections.Generic;

namespace Base_Tools45.Jig
{
    public class JigDraw : DrawJig
    {
        private Point3d pnt3dBase;
        private Point3d pnt3dRes;

        private List<Entity> entities;

        public JigDraw(Point3d pnt3d)
        {
            pnt3dBase = pnt3d.TransformBy(UCS);
            entities = new List<Entity>();
        }

        public Matrix3d UCS
        {
            get
            {
                return BaseObjs._editor.CurrentUserCoordinateSystem;
            }
        }

        public Point3d BASE
        {
            get
            {
                return pnt3dBase;
            }
            set
            {
                pnt3dBase = value;
            }
        }

        //public Vector3d V3D{
        //    get{
        //        return V3D;
        //    }
        //    set{
        //        V3D = value;
        //    }
        //}

        //public Matrix3d MAtrIX3D{
        //    get{
        //        return Matrix3d.Displacement(V3D);
        //    }
        //}
        public void AddEntity(ObjectIdCollection ids)
        {
            foreach (ObjectId id in ids)
            {
                entities.Add(id.getEnt());
            }
        }

        public void TransformEntities()
        {
            Matrix3d mat3d = Matrix3d.Displacement(pnt3dRes - pnt3dBase);
            foreach (Entity ent in entities)
                ent.TransformBy(mat3d);
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions prPntOpts = new JigPromptPointOptions("\nSelect new Basepoint: ");
            prPntOpts.UserInputControls = UserInputControls.Accept3dCoordinates | UserInputControls.NullResponseAccepted;

            prPntOpts.BasePoint = pnt3dBase;
            prPntOpts.UseBasePoint = true;

            PromptPointResult prResult = prompts.AcquirePoint(prPntOpts);
            pnt3dRes = prResult.Value;

            if (prResult.Status == PromptStatus.Cancel || prResult.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;

            if (pnt3dRes == pnt3dBase)
                return SamplerStatus.NoChange;

            if (prResult.Status == PromptStatus.OK)
                return SamplerStatus.OK;

            return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            Matrix3d mat3d = Matrix3d.Displacement(pnt3dRes - pnt3dBase);
            WorldGeometry wGeo = draw.Geometry;
            if (wGeo != null)
            {
                wGeo.PushModelTransform(mat3d);
                foreach (Entity ent in entities)
                {
                    wGeo.Draw(ent);
                }
                wGeo.PopModelTransform();
            }
            return true;
        }
    }
}