using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;

namespace MNP
{
    public static class MNP_Profile
    {
        private static frmMNP fMNP = MNP_Forms.fMNP;

        public static ObjectId
        makeProfile(ObjectId idAlign)
        {
            Point3d pnt3dPick = Pub.pnt3dO;
            if (idAlign == ObjectId.Null)
            {
                Autodesk.AutoCAD.DatabaseServices.Entity ent = Select.selectEntity(typeof(Alignment), "\nSelect Alignment: ", "\nSelected object was not an alignment. Try again: ", out pnt3dPick);
                idAlign = ent.ObjectId;
            }
            bool exists = false;
            ProfileView pView = null;
            using (BaseObjs._acadDoc.LockDocument())
            {
                TinSurface tinSurf = Surf.getTinSurface("EXIST", out exists);
                string nameProfile = tinSurf.Name;
                Profile profEX = Prof.addProfileBySurface(nameProfile, idAlign, tinSurf.ObjectId, fMNP.idLayer, fMNP.idProfileStyleEX, fMNP.idProfileLabelSetEX);

                tinSurf = Surf.getTinSurface("CPNT-ON", out exists);
                nameProfile = tinSurf.Name;
                Profile profDE = Prof.addProfileBySurface(nameProfile, idAlign, tinSurf.ObjectId, fMNP.idLayer, fMNP.idProfileStyleDE, fMNP.idProfileLabelSetDE);
                fMNP.idProfile = profDE.ObjectId;
                PromptStatus ps;
                pnt3dPick = UserInput.getPoint("Select insertion point for Profile View", out ps, osMode: 0);

                pView = Prof.addProfileView(idAlign, pnt3dPick, fMNP.idProfileBandSetStyle, fMNP.idProfileViewStyle);
            }
            return pView.ObjectId;
        }
    }
}