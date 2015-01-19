using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;

namespace Grading.Cmds
{
    public static class cmdDBL
    {
        public static void
        deleteBreaklinesInSurface(string nameSurface)
        {
            bool exists = false;
            TinSurface surf = Surf.getTinSurface(nameSurface, out exists);
            if (exists)
                surf.deleteBreaklines();
        }

        public static void
        deleteBreaklinesInDwg()
        {
            ObjectIdCollection ids = BaseObjs._acadDoc.getBrkLines();
            ids.delete();
        }
    }
}
