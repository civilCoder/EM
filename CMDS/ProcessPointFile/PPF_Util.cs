using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using System.Collections.Generic;

namespace ProcessPointFile
{
    public static class PPF_Util
    {
        public static void
        modifyString(ref List<string> flds)
        {
            for (int i = 5; i < flds.Count; i++)
            {
                switch (flds[i])
                {
                    case "BEG":
                        if (flds[i + 1] != "BOC")
                            flds[i + 1] = "";
                        break;

                    case "POC":
                    case "END":
                    case "BOC":
                        flds[i + 1] = "";
                        break;

                    case "EOC":
                        if (flds[i + 1] != "END")
                            flds[i + 1] = "";
                        break;
                }
            }
        }

        public static string
        removeExtraSpaces(string buf)
        {
            string ret;
            ret = buf.Replace("   ", " ");
            ret = buf.Replace("  ", " ");
            return ret;
        }

        public static string
        replaceSpace(string buf0, string key)
        {
            int pos1 = 0;
            int pos2 = 0;

            buf0 = removeExtraSpaces(buf0);
            //remove double spaces
            switch (key)
            {
                case " ":
                    while (buf0.Contains(" "))
                    {
                        pos1 = buf0.IndexOf(" ", System.StringComparison.Ordinal);
                        if (pos1 != 0)
                        {
                            buf0 = buf0.Substring(0, pos1 - 1);
                        }
                    }
                    break;

                default:
                    pos1 = buf0.IndexOf(key, System.StringComparison.Ordinal);
                    if (pos1 != 0)
                    {
                        pos2 = buf0.IndexOf(" ", System.StringComparison.Ordinal);
                        if (pos2 != 0)
                        {
                            buf0 = string.Format("{0}{1}", buf0.Substring(0, pos2), buf0.Substring(pos1));
                        }
                    }
                    break;
            }
            return buf0;
        }

        public static string
        replaceString(string tar)
        {
            string buf = string.Empty;

            switch (tar)
            {
                case "AP":
                case "BX":
                case "BTMX":
                case "CB":
                case "COR":
                case "D/L":
                    buf = "";
                    break;

                case "EC EP":
                    buf = "EC";
                    break;

                case "EP EC":
                    buf = "EP";
                    break;

                case "GATE":
                case "GB":
                case "PB":
                case "POL":
                case "STEP":
                case "TOPX":
                case "TX":
                    buf = "";
                    break;
            }

            return buf;
        }

        public static void
        saveAsDoc(this Document doc, string nameFull)
        {
            AcadDocument acDoc = (AcadDocument)doc.GetAcadDocument();
            acDoc.SaveAs(nameFull);
        }

        public static string
        strModDelete(string buf0, string tarTxt, string newTxt)
        {
            string buf = string.Empty;

            int len0 = buf0.Length;
            int len1 = 0;

            int lenTar = tarTxt.Length;
            int lenNew = newTxt.Length;

            int pos1 = buf0.IndexOf(tarTxt, System.StringComparison.Ordinal);
            int pos2 = 0;

            if (pos1 + lenTar - 1 == len0)                  //at end of string
                buf = string.Format("{0}{1}", buf0.Substring(0, pos1 - 1), newTxt);
            else
            {
                if (lenNew == 0)
                {
                    pos2 = buf0.IndexOf(" ", pos1 + 1, System.StringComparison.Ordinal);  //find next space
                    len1 = len0 - pos2;
                    buf = string.Format("{0}{1}", buf0.Substring(0, pos1 - 1), buf0.Substring(pos2 + 1, len1));
                }
                else
                {
                    buf = string.Format("{0}{1}", buf0.Substring(0, pos1 - 1), buf0.Substring(pos2, len1));
                }
            }

            return buf.TrimEnd();
        }
    }
}