using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace slopeGrading
{
    internal class SG_Utility
    {
        private static Editor ed = BaseObjs._editor;

        public static void Write(string msg)
        {
            ed.WriteMessage(msg);
        }

        public static DialogResult msgBox(string strMsg, string strCaption)
        {
            DialogResult result;
            result = System.Windows.Forms.MessageBox.Show(strMsg, strCaption, MessageBoxButtons.YesNoCancel);
            return result;
        }

        public static int getSide(List<Point3d> pnts3d)
        {
            int intSide = 0;
            PromptPointOptions PPO = new PromptPointOptions("Select a location near the begin point of reference line and on the side to grade to.");
            PromptPointResult PPR = null;

            try
            {
                PPR = SG_Utility.ed.GetPoint(PPO);
            }
            catch (Exception)
            {
                intSide = 0;
            }

            if (PPR.Status == PromptStatus.OK)
            {
                Point3d pnt3d = PPR.Value;

                if (pnt3d.isRightSide(pnts3d[0], pnts3d[1]))
                {
                    intSide = -1;
                }
                else
                {
                    intSide = 1;
                }
            }

            return intSide;
        }
    }
}