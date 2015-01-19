using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdAJP
    {
        public static void
        AJP()
        {
            string elev = "";
            ObjectId idCgPntBASE = ObjectId.Null;
            elev = UserInput.getCogoPoint("\nSelect Base Point: ", out idCgPntBASE, Pub.pnt3dO, osMode: 8);
            if (elev == string.Empty)
                return;
            Point3d pnt3dBASE = idCgPntBASE.getCogoPntCoordinates();

            ObjectId idCgPntTAR = ObjectId.Null;
            elev = UserInput.getCogoPoint("\nSelect Target Point: ", out idCgPntTAR, idCgPntBASE, osMode: 8);
            if (elev == string.Empty)
                return;
            Point3d pnt3dTAR = idCgPntTAR.getCogoPntCoordinates();

            double slope = idCgPntBASE.getSlope(idCgPntTAR);

            string message = string.Format("\nExisting Rate of Grade is {0:F4}", slope);
            BaseObjs._editor.WriteMessage(message);                                                //show slope

            double slopeNew = 0.0;
            bool escape = true;

            escape = UserInput.getUserInput("\nEnter new Rate of Grade: ", out slopeNew);

            SelectionSet ss = Select.buildSSet(typeof(CogoPoint), selectAll: false, prompt: "\nSelect Cogo Points for adjustment: ");

            ObjectId[] ids = ss.GetObjectIds();

            Point3d pnt3d0 = Pub.pnt3dO;
            Point3d pnt3dE = Pub.pnt3dO;
            Point3d pnt3dX = Pub.pnt3dO;

            CogoPoint cgPntX = null;
            double dZ = 0.0;

            double elevE = 0.0;
            double elevX = 0.0;
            double elevPnt = 0.0;
            double dXE = 0.0;

            for (int i = 0; i < ids.Length; i++)
            {
                pnt3d0 = ids[i].getCogoPntCoordinates();        //existing location selected of modify

                dXE = Geom.getPerpDistToLine(pnt3dBASE, pnt3dTAR, pnt3d0);

                pnt3dE = new Point3d(pnt3d0.X, pnt3d0.Y, pnt3dBASE.Z + dXE * (slope));

                dZ = pnt3d0.Z - pnt3dE.Z;       //difference from cogo point to existing plane elevation

                pnt3dX = new Point3d(pnt3d0.X, pnt3d0.Y, pnt3dBASE.Z + dXE * (slopeNew));      //existing point on new plane
                pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dX.Z + dZ);                            //existing point adjusted

                elevE = pnt3dTAR.Z + dXE * slope;
                elevX = pnt3dTAR.Z + dXE * slopeNew;

                dZ = elevX - elevE;

                cgPntX = (CogoPoint)ids[i].getEnt();
                elevPnt = cgPntX.Elevation;

                elevPnt = elevPnt + dZ;

                List<ObjectId> idCgPnt = new List<ObjectId> { ids[i] };

                Cmds.cmdMCE.MCE(idCgPnt, elevPnt.ToString());
            }
        }
    }
}
