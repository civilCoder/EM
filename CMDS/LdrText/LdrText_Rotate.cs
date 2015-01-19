using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;

namespace LdrText
{
    public static class LdrText_Rotate
    {
        private const double pi = System.Math.PI;

        public static void
        adjTxt(ObjectId idLDR, string nameApp)
        {
            TypedValue[] tvsLdr = idLDR.getXData(nameApp).AsArray();
            string nameCmd = tvsLdr[1].Value.ToString();
            ObjectId idTxTop = tvsLdr.getObjectId(3);

            TypedValue[] tvsTxTop = idTxTop.getXData(nameApp).AsArray();

            Point3dCollection pnts3d = idLDR.getCoordinates3d();
            int n = pnts3d.Count;
            double angle = pnts3d[n - 2].getDirection(pnts3d[n - 1]);   //angle of last segment of leader from next to last point to last point
            Point3d pnt3dEnd = pnts3d[n - 1];                           //end point of leader

            double angleTx = 0;

            bool left_justify = Base_Tools45.Math.left_Justify(angle);

            AttachmentPoint apTop;
            AttachmentPoint apBot;
            string justifyTop = string.Empty;
            string justifyBot = string.Empty;

            if (left_justify)
            {
                apTop = AttachmentPoint.BottomLeft;
                apBot = AttachmentPoint.TopLeft;
                angleTx = angle + pi;
                justifyTop = Pub.JUSTIFYLEFT;
                justifyBot = Pub.JUSTIFYLEFT;
            }
            else
            {
                apTop = AttachmentPoint.BottomRight;
                apBot = AttachmentPoint.TopRight;
                angleTx = angle;
                justifyTop = Pub.JUSTIFYRIGHT;
                justifyBot = Pub.JUSTIFYRIGHT;
            }

            Point3d pnt3dTop, pnt3dBot, pnt3dBot2;

            double scale = Misc.getCurrAnnoScale();

            pnt3dTop = pnt3dEnd.traverse(angleTx + pi / 2, 0.01 * scale); //angleTx is always from left to right as adjusted above
            pnt3dBot = pnt3dEnd.traverse(angleTx - pi / 2, 0.01 * scale);
            pnt3dBot2 = pnt3dEnd.traverse(angleTx - pi / 2, 0.14 * scale);

            double widthTop = 0, widthBot = 0, widthBot2 = 0;
            widthTop = idTxTop.getMTextWidth();

            ObjectId idTxBot = tvsTxTop.getObjectId(4);
            if (idTxBot != ObjectId.Null)
            {
                widthBot = idTxBot.getMTextWidth();
            }
            string bot = idTxBot.getMTextText();

            ObjectId idTxBot2 = tvsTxTop.getObjectId(5);
            if (idTxBot2 != ObjectId.Null)
                widthBot2 = idTxBot2.getMTextWidth();
            string bot2 = idTxBot2.getMTextText();

            if (nameCmd == "cmdDEP")
                justifyTop = Pub.JUSTIFYCENTER;
            if (bot.Length < 4)
                justifyBot = Pub.JUSTIFYCENTER;

            double width = 0;
            if (widthBot > widthTop)
                width = widthBot;
            else if (widthBot2 > width)
                width = widthBot;
            else
                width = widthTop;

            idTxTop.adjMTextXYandAngle(pnt3dTop, angleTx, width);
            idTxTop.setMTextAttachPointAndJustify(apTop, justifyTop);

            if (idTxBot != ObjectId.Null)
            {
                idTxBot.adjMTextXYandAngle(pnt3dBot, angleTx, width);
                idTxBot.setMTextAttachPointAndJustify(apBot, justifyBot);
            }

            if (idTxBot2 != ObjectId.Null)
            {
                idTxBot2.adjMTextXYandAngle(pnt3dBot2, angleTx, width);
                idTxBot2.setMTextAttachPointAndJustify(apBot, justifyBot);
            }

            tvsTxTop[6] = new TypedValue(1040, pnt3dTop.X);
            tvsTxTop[7] = new TypedValue(1040, pnt3dTop.Y);

            idTxTop.setXData(tvsTxTop, nameApp);

            Point3dCollection pnts3dLdr = idLDR.getCoordinates3d();
            for (int i = 0; i < pnts3dLdr.Count; i++)
            {
                tvsLdr[i * 2 + 4] = new TypedValue(1040, pnts3dLdr[i].X);
                tvsLdr[i * 2 + 5] = new TypedValue(1040, pnts3dLdr[i].Y);
            }

            idLDR.setXData(tvsLdr, nameApp);
        }
    }
}
