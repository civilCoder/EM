using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System;

namespace Grading.Cmds
{
    public static class cmdS
    {
        public static ObjectId
        S(Point3d pnt3d, string desc)
        {
            ObjectId idPnt3d = ObjectId.Null;
            Object osMode = SnapMode.getOSnap();
            SnapMode.setOSnap(0);
            PromptStatus ps = PromptStatus.Cancel;

            try
            {
                if (pnt3d == Pub.pnt3dO)
                {
                    pnt3d = UserInput.getPoint("New Point: ", out ps, osMode: 641);
                    if (pnt3d == Pub.pnt3dO)
                    {
                        return ObjectId.Null;
                    }
                    else
                    {
                        if (pnt3d.Z == 0.0)
                        {
                            double elev;
                            bool escape = UserInput.getUserInput("Enter Elevation: ", out elev, 0.0);
                            if (escape)
                                return ObjectId.Null;
                            pnt3d = new Point3d(pnt3d.X, pnt3d.Y, elev);
                        }
                    }
                }

                uint pntNum;

                ObjectId idCgPnt = pnt3d.setPoint(out pntNum, desc);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdS.cs: line: 48");
            }
            finally
            {
                SnapMode.setOSnap((int)osMode);
            }

            return idPnt3d;
        }
    }
}
