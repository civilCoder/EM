using System;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;

using Base_Tools45;
using Base_Tools45.Jig;

namespace Bubble {
    public class BB_Jig : EntityJig {
        Point3dCollection _mPts;
        Point3d _mTempPoint;
        ObjectIdCollection ids;
        public static bool _mIsJigStarted;
        
        public BB_Jig(Point3d pnt3dFirst, ObjectIdCollection idsIn)
            : base(null) {
            _mPts = new Point3dCollection();
            _mPts.Add(pnt3dFirst);
            
            ids = idsIn;
            _mIsJigStarted = false;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            JigPromptPointOptions opts = new JigPromptPointOptions();

            opts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.NoNegativeResponseAccepted);

            if (_mPts.Count == 1 )
            {
                opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                opts.Message = "\nPick New Leader Start Point on Target (vertex #1):";
                opts.UseBasePoint = false;
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
                Leader leader = Entity as Leader;

                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                if (_mIsJigStarted)
                {
                    leader.RemoveLastVertex();
                }
                
                Point3d lastVertex = leader.VertexAt(leader.NumVertices-1);
                if (!_mTempPoint.Equals(lastVertex))
                {
                    leader.AppendVertex(_mTempPoint);
                    _mIsJigStarted = true;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " BB_Jig.cs: line: 81");
            }
            return true;
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

        [CommandMethod("MYSL")]
        public static void
            MySplineLeader() {

            jigSplineLeader(new Point3d(0,0,0), 0.09, "0", 5);
        }
  
        public static ObjectId 
            jigSplineLeader(Point3d pnt3dFirst, double txtSize, string nameLayer, short color) {

            ObjectId idLDR = ObjectId.Null;
            Editor ed = BaseObjs._editor;
            
            JigSplineLeader jig = new JigSplineLeader(pnt3dFirst, txtSize, nameLayer, color);

            bool bSuccess = true, bComplete = false;
            int pntCount = 0;

            while (bSuccess && !bComplete) {
                _mIsJigStarted = false;

                PromptResult dragres = ed.Drag(jig);

                bSuccess = (dragres.Status == PromptStatus.OK);
                if (bSuccess) {
                    jig.addVertex();
                    pntCount ++;
                }

                bComplete = (dragres.Status == PromptStatus.None || pntCount == 3);
            }

            if (bComplete) {
                
                try{
                    using (Transaction tr = BaseObjs.startTransactionDb()){
                        BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                        Leader ldr = (Leader)jig.getEntity();
                          
                        ms.AppendEntity(ldr);
                        tr.AddNewlyCreatedDBObject(ldr, true);
                        tr.Commit();
                        idLDR = ldr.ObjectId;
                    }
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " BB_Jig.cs: line: 155");
                }
            }
            return idLDR;
        }
    }
}
