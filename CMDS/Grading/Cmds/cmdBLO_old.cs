using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;
using gp = Grading.Grading_Public;

namespace Grading.Cmds
{
    public class cmdBLO_old
    {
        public static void
        BLO()
        {
            bool escape;
            string ResultBLO = "I";
            string prompt = string.Format("\nSelect Method (Interval distance/Number of points per segment) [I/N] <{0}>:", ResultBLO);
            escape = UserInput.getUserInputKeyword(ResultBLO.ToUpper(), out ResultBLO, prompt, "I N");
            if (escape)
                return;
            int numPnts = 1;
            double spacing = 10;
            PromptStatus ps;

            switch (ResultBLO)
            {
                case "I":
                    escape = UserInput.getUserInput(string.Format("\nInterval Distance <{0}>", spacing), out spacing, spacing);
                    if (escape)
                        return;
                    break;

                case "N":
                    ps = UserInput.getUserInputInt(string.Format("\nNumber of Points: <{0}>", numPnts), true, false, false, false, true, numPnts, out numPnts);
                    if (ps != PromptStatus.OK)
                        return;
                    break;
            }

            double deltaZ = 0.50;
            escape = UserInput.getUserInput(string.Format("\nElevation Offset <{0}>", deltaZ), out deltaZ, deltaZ);
            if (escape)
                return;

            double deltaXY = 2.00;
            escape = UserInput.getUserInput(string.Format("\nHorizontal Offset <{0}>", deltaXY), out deltaXY, deltaXY);
            if (escape)
                return;

            bool answer;
            ps = UserInput.getUserInputYesNo("\nAdjust Corner Points", true, out answer);
            if (ps != PromptStatus.OK)
                return;

            SnapMode.setOSnap(8);

            List<ObjectId> ids = new List<ObjectId>();
            ObjectId id = ObjectId.Null;
            prompt = "";

            Point3d pnt3dBase = Pub.pnt3dO;
            Point3d pnt3dLast = Pub.pnt3dO;

            bool proceed = true;

            do
            {
                if (ids.Count == 0)
                {
                    prompt = "\nSelect First Point: ";
                }
                else if (ResultBLO == "N")
                {
                    prompt = "\nSelect Second Point: ";
                    pnt3dBase = pnt3dLast;
                }
                else
                    prompt = "\nSelect Next Point: ";

                string resElev1 = UserInput.getCogoPoint(prompt, out id, Pub.pnt3dO, osMode: 8);
                if (resElev1 == string.Empty)
                    proceed = false;
                else
                {
                    pnt3dLast = id.getCogoPntCoordinates();
                    ids.Add(id);
                }

                if (ResultBLO == "N")
                    if (ids.Count == 2)
                        proceed = false;
            }
            while (proceed);

            if (ids.Count < 2)
                return;

            Point3d pnt3d1 = Pub.pnt3dO;
            Point3d pnt3d2 = Pub.pnt3dO;
            int k = 0;

            k = ids.Count;

            List<POI> vPOIs = new List<POI>();

            for (int i = 1; i < k; i++)
            {
                pnt3d1 = ids[i - 1].getCogoPntCoordinates();
                pnt3d2 = ids[i - 0].getCogoPntCoordinates();

                double dist = pnt3d1.getDistance(pnt3d2);
                double slope = (gp.pnt3d2.Z - gp.pnt3d1.Z) / dist;

                double remain = dist - (spacing * System.Math.Truncate(dist / spacing));

                if (remain > 0.0001)
                    numPnts = (int)System.Math.Truncate(dist / spacing);
                else
                    numPnts = (int)System.Math.Truncate(dist / spacing) - 1;
            }
        }
    }
}
