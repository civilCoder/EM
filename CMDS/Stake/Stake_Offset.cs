using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Stake
{
    public static class Stake_Offset
    {
        public static int
        getOffset(ObjectId idPoly, Point3d varPntPick)
        {
            int intSide = 0;

            if (isRightHandRotation(idPoly, varPntPick))
            {
                intSide = 1;
            }
            else
            {
                intSide = -1;
            }

            return intSide;
        }

        public static bool
        isRightHandRotation(ObjectId idPoly, Point3d varPntPick)
        {
            bool isRight = false;

            //see vba code or vb.net code

            return isRight;
        }
    }
}