using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(typeof(Base_Tools45.C3D.AlignEnts))]

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public class AlignEnts
    {
        /// <summary>
        ///
        /// </summary>
        private EnumerateEntitiesDelegate EnumerateEntities;

        /// <summary>
        ///
        /// </summary>
        public AlignEnts()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="align"></param>
        private delegate void EnumerateEntitiesDelegate(Alignment align);

        /// <summary>
        ///
        /// </summary>
        public enum AlignmentEntityType
        {
            /// <summary>
            ///
            /// </summary>
            Arc = 0x102,

            /// <summary>
            ///
            /// </summary>
            CurveCurveReverseCurve = 0x113,

            /// <summary>
            ///
            /// </summary>
            CurveLineCurve = 0x111,

            /// <summary>
            ///
            /// </summary>
            CurveReverseCurve = 0x112,

            /// <summary>
            ///
            /// </summary>
            CurveSpiral = 0x109,

            /// <summary>
            ///
            /// </summary>
            CurveSpiralSpiral = 0x110,

            /// <summary>
            ///
            /// </summary>
            Line = 0x101,

            /// <summary>
            ///
            /// </summary>
            LineSpiral = 0x107,

            /// <summary>
            ///
            /// </summary>
            MultipleSegments = 0x10b,

            /// <summary>
            ///
            /// </summary>
            Spiral = 0x103,

            /// <summary>
            ///
            /// </summary>
            SpiralCurve = 0x108,

            /// <summary>
            ///
            /// </summary>
            SpiralCurveSpiral = 260,

            /// <summary>
            ///
            /// </summary>
            SpiralCurveSpiralCurveSpiral = 0x10c,

            /// <summary>
            ///
            /// </summary>
            SpiralCurveSpiralSpiralCurveSpiral = 0x10d,

            /// <summary>
            ///
            /// </summary>
            SpiralLine = 0x106,

            /// <summary>
            ///
            /// </summary>
            SpiralLineSpiral = 0x105,

            /// <summary>
            ///
            /// </summary>
            SpiralSpiral = 270,

            /// <summary>
            ///
            /// </summary>
            SpiralSpiralCurve = 0x10f,

            /// <summary>
            ///
            /// </summary>
            SpiralSpiralCurveSpiralSpiral = 0x10a
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("displayAlignEnts")]
        public void
        cmd_displayAlignEnts()
        {
            EnumerateEntities += enumerateEntsById;
            doDisplayAlignmentEntities();
        }

        /// <summary>
        ///
        /// </summary>
        [CommandMethod("displayAlignEntsInOrder")]
        public void
        cmd_displayAlignEntsByOrder()
        {
            EnumerateEntities += enumerateEntsByOrder;
            doDisplayAlignmentEntities();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idAlign"></param>
        private void displayAlignEnts(ObjectId idAlign)
        {
            Alignment align = (Alignment)idAlign.GetObject(OpenMode.ForRead);
            BaseObjs.write(string.Format("\nAlignment Name: {0}", align.Name));
            EnumerateEntities(align);
        }

        /// <summary>
        ///
        /// </summary>
        private void doDisplayAlignmentEntities()
        {
            Point3d pnt3dPicked;
            Alignment align = (Alignment)Select.selectEntity(typeof(Alignment), "Select Alignment.", "Selected entity is not an alignment.", out pnt3dPicked);
            if (align != null)
            {
                displayAlignEnts(align.ObjectId);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="align"></param>
        private void enumerateEntsById(Alignment align)
        {
            foreach (AlignmentEntity ent in align.Entities)
            {
                BaseObjs.write(string.Format("\n.. Entity ID: {0}", ent.EntityId));
                writeAlignData(ent);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="align"></param>
        private void enumerateEntsByOrder(Alignment align)
        {
            AlignmentEntityCollection ents = align.Entities;
            for (int i = 0; i < ents.Count; i++)
            {
                AlignmentEntity ent = ents.GetEntityByOrder(i);
                BaseObjs.write(string.Format("\n.. Entity Sequence: {0}", i));
                writeAlignData(ent);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        private void writeAlignData(AlignmentEntity ent)
        {
            BaseObjs.write(string.Format("\n.. Entity Class: {0}", ent.GetType()));
            BaseObjs.write(string.Format("\n.. Entity Type: {0}", ent.EntityType));
            BaseObjs.write(string.Format("\n.. Subentities: {0}", ent.SubEntityCount));
        }
    }
}