using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace Bubble
{
    public class BB_LnkBub_Scale
    {
        public static void
        updateSymbols(int scaleCurr)
        {
            bool exists = false;
            ObjectId idDictBubble = Dict.getNamedDictionary(apps.lnkBubs, out exists);
            List<DBDictionaryEntry> entries = idDictBubble.getDictEntries();
            if (entries.Count == 0)
                return;
            ObjectId idSM, idTX, idLDR;
            foreach (DBDictionaryEntry entry in entries)
            {
                string h = entry.Key;
                ObjectId id = h.stringToHandle().getObjectId();
                if (!id.IsValid)
                    return;

                ResultBuffer rb = id.getXData(apps.lnkBubs);
                TypedValue[] tvs = rb.AsArray();

                string typeObj = tvs[1].Value.ToString();

                switch (typeObj)
                {
                    case "TARGET":
                        ObjectId idTARGET = id;
                        break;

                    case "TX":
                        TypedValue[] tvsTX = tvs;
                        idTX = id;
                        int scale;
                        int.TryParse(tvsTX[3].Value.ToString(), out scale);
                        Handle hSM = tvsTX[2].Value.ToString().stringToHandle();
                        idSM = hSM.getObjectId();

                        ResultBuffer rbSM = hSM.getXData(apps.lnkBubs);
                        TypedValue[] tvsSM = rbSM.AsArray();
                        tvsSM = rbSM.AsArray();

                        Handle hWO = tvsSM[2].Value.ToString().stringToHandle();
                        ObjectId idWO = hWO.getObjectId();

                        int numSides;
                        int.TryParse(tvsSM[5].Value.ToString(), out numSides);

                        Point3d pnt3dCEN = hSM.getCenter(numSides);

                        idTX.moveObj(idTX.getMTextLocation(), pnt3dCEN);

                        double scaleFactor = (double)scaleCurr / (double)scale;
                        idWO.scaleObj(scaleFactor, pnt3dCEN);
                        idSM.scaleObj(scaleFactor, pnt3dCEN);

                        BB_Ldr.scaleLDRs(pnt3dCEN, scaleFactor, idSM);

                        TypedValue[] tvsNew = new TypedValue[tvsTX.Length];
                        for (int i = 0; i < 3; i++)
                        {
                            tvsNew.SetValue(new TypedValue(tvsTX[i].TypeCode, tvsTX[i].Value), i);
                        }
                        tvsNew.SetValue(new TypedValue(1070, scaleCurr), 3);
                        for (int j = 4; j < tvsTX.Length; j++)
                        {
                            tvsNew.SetValue(new TypedValue(tvsTX[j].TypeCode, tvsTX[j].Value), j);
                        }

                        idTX.setXData(tvsNew, apps.lnkBubs);
                        break;

                    case "SM":
                        idSM = id;
                        break;

                    case "LDR":
                        idLDR = id;
                        break;
                }
            }
        }
    }
}
