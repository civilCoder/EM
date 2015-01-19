using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EW {
    public static class EW_MakeSurfaceVol {
        public static void 
        makeVolSurface(string strNameBASE, string strNameCOMP, bool boolShowMessage) {
            string nameSurface = string.Format("VOL_{0}_{1}", strNameBASE, strNameCOMP);
            List<string> nameSurfaces = Surf.getSurfaces();

            for (int i = 0; i < nameSurfaces.Count; i++) { 
                if (nameSurfaces[i] == nameSurface) {
                    Surf.removeSurface(nameSurfaces[i]);
                }
            }

            TinSurface objSurfaceBASE = Surf.getTinSurface(strNameBASE);
            TinSurface objSurfaceCOMP = Surf.getTinSurface(strNameCOMP);

            int lngVolCut = 0;
            int lngVolFill = 0;

            ObjectId idSurfFill = TinVolumeSurface.Create(nameSurface, objSurfaceBASE.ObjectId, objSurfaceCOMP.ObjectId);              

            SelectionSet objSSetLim = EW_Utility1.buildSSetGradingLim();
            ObjectId[] ids = objSSetLim.GetObjectIds();
            TinVolumeSurface objSurfaceFILL = null;
            if (ids != null && ids.Length > 0) {
                ObjectId idPoly = ids[0];
                idPoly.checkIfClosed();

                ObjectId idPoly3d = Conv.poly_Poly3d(idPoly, 0, "0");
                ObjectId[] idBndrys = { idPoly3d };
                objSurfaceFILL = (TinVolumeSurface)idSurfFill.getEnt();
                objSurfaceFILL.BoundariesDefinition.AddBoundaries(new ObjectIdCollection(idBndrys), 1.0, Autodesk.Civil.SurfaceBoundaryType.Outer, true);
                objSurfaceFILL.Rebuild();
                idPoly3d.delete();
            }else {
                MessageBox.Show("GRADING LIMIT not found - OUTER BOUNDARY not added.");
            }

            lngVolCut = (int)objSurfaceFILL.GetVolumeProperties().UnadjustedCutVolume / 27;
            lngVolFill = (int)objSurfaceFILL.GetVolumeProperties().UnadjustedFillVolume / 27;

            if (boolShowMessage == true) {
                string mess = string.Format("Cut: {0} CY     Fill: {1} CY", lngVolCut, lngVolFill);
                MessageBox.Show(mess);
            }

            EW_Main.viewResults("VOL", false);
        }

        public static void
        updateVolSurface(string strNameBASE, string strNameCOMP) {
            string nameSurface = string.Format("VOL_{0}_{1}", strNameBASE, strNameCOMP);
            List<string> nameSurfaces = Surf.getSurfaces();

            for (int i = 0; i < nameSurfaces.Count; i++) {
                if (nameSurfaces[i] == nameSurface) {
                    Surf.removeSurface(nameSurfaces[i]);
                }
            }

            TinSurface objSurfaceBASE = Surf.getTinSurface(strNameBASE);
            TinSurface objSurfaceCOMP = Surf.getTinSurface(strNameCOMP);

            TinVolumeSurface.Create(nameSurface, objSurfaceBASE.ObjectId, objSurfaceCOMP.ObjectId);              
        }

        public static void
        makeRemSurface(double dblDepth) {
            EW_CheckSurfaceStyles.checkSurfaceStyles("EXIST");

            Point3d pnt3dMove0 = Point3d.Origin;
            Point3d pnt3dMoveX = new Point3d(0, 0, dblDepth * -1);
            
            Matrix3d mtx3d = Matrix3d.Displacement(pnt3dMoveX - pnt3dMove0);

            TinSurface objSurfaceExist = Surf.getTinSurface("EXIST");
            ObjectId idSurface = objSurfaceExist.copy();
            
            using (var tr = BaseObjs.startTransactionDb()) {
                TinSurface surface = (TinSurface)tr.GetObject(idSurface, OpenMode.ForWrite);  
                surface.Name = "EG-ADJ2";

                Layer.manageLayers("EG-ADJ2-SURFACE");
                surface.Layer = "EG-ADJ2" + "-SURFACE";
                surface.TransformBy(mtx3d);
                surface.Rebuild();
            }
        }
    }
}