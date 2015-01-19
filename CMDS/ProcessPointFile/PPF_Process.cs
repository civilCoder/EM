using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ProcessPointFile
{
    public static class PPF_Process
    {
        public static void
        procPntFile(string nameFileIn)
        {
            string buf0 = string.Empty;

            int length = nameFileIn.Length;

            StreamReader sr = null;
            try
            {
                sr = new StreamReader(nameFileIn);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " PPF_Process.cs: line: 22");
            }

            List<PPF_PntData> pds = new List<PPF_PntData>();

            while (!sr.EndOfStream)
            {
                buf0 = sr.ReadLine();

                if (string.IsNullOrEmpty(buf0))
                    continue;

                buf0 = buf0.Trim();

                buf0 = PPF_Util.removeExtraSpaces(buf0);

                buf0 = buf0.Replace("AP", "");
                buf0 = buf0.Replace("COR", "");
                buf0 = buf0.Replace("POL", "");
                buf0 = buf0.Replace("PCC", "POC");
                buf0 = buf0.Replace("PRC", "POC");

                int pos;
                if (buf0.Contains("BEG"))
                {
                    pos = buf0.IndexOf("BEG", System.StringComparison.Ordinal);
                    if (buf0[pos - 1].ToString() != " ")
                        buf0 = buf0.Insert(pos, " ");
                }
                if (buf0.Contains("END"))
                {
                    pos = buf0.IndexOf("END", System.StringComparison.Ordinal);
                    if (buf0[pos - 1].ToString() != " ")
                        buf0 = buf0.Insert(pos, " ");
                }

                if (buf0.Contains("@"))
                {
                    int pos0 = buf0.IndexOf("@", 0, System.StringComparison.Ordinal);
                    int posX = buf0.IndexOf(" ", pos0, System.StringComparison.Ordinal);
                    if (posX != 0)
                    {
                        buf0 = string.Format("{0}{1}", buf0.Substring(0, pos0 - 1), buf0.Substring(posX));
                    }
                    else
                    {
                        buf0 = buf0.Substring(0, pos0 - 1);
                    }
                }

                if (buf0.Contains("EC"))
                {
                    pos = buf0.IndexOf("EC", System.StringComparison.Ordinal);
                    if (buf0[pos - 1].ToString() != ",")
                        buf0 = buf0.Replace("EC", "EOC");
                }

                if (buf0.Contains("BC"))
                {
                    pos = buf0.IndexOf("BC", System.StringComparison.Ordinal);
                    if (buf0[pos - 1].ToString(CultureInfo.InvariantCulture) != ",")
                        buf0 = buf0.Replace("BC", "BOC");
                }

                stripDESC(ref buf0, ",TC");
                stripDESC(ref buf0, ",EC");
                stripDESC(ref buf0, ",EP");
                stripDESC(ref buf0, ",LP");
                stripDESC(ref buf0, ",WIF");
                stripDESC(ref buf0, ",BCC");

                string[] keys = {
                    ",",
                    " "
                };
                string[] fields = splitFields(buf0, keys);

                List<string> flds = new List<string>();
                for (int n = 0; n < fields.Length; n++)
                {
                    flds.Add(fields[n]);
                }

                switch (flds.Count)
                { //force list to have 10 fields
                    case 5:
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        break;

                    case 6:
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        break;

                    case 7:
                        flds.Add(" ");
                        flds.Add(" ");
                        flds.Add(" ");
                        break;

                    case 8:
                        flds.Add(" ");
                        flds.Add(" ");
                        break;

                    case 9:
                        flds.Add(" ");
                        break;
                }

                for (int i = 5; i < 10; i++)
                {
                    if (flds[i] != "")
                        flds[i] = PPF_Util.replaceString(flds[i]);
                }

                PPF_Util.modifyString(ref flds);

                PPF_PntData pd = new PPF_PntData();

                pd.PntNum = int.Parse(flds[0]);
                pd.X = double.Parse(flds[1]);
                pd.Y = double.Parse(flds[2]);
                pd.Z = double.Parse(flds[3]);
                pd.Desc = flds[4];
                pd.Code1 = flds[5];
                pd.Code2 = flds[6];
                pd.Code3 = flds[7];
                pd.Code4 = flds[8];
                pd.Code5 = flds[9];
                pds.Add(pd);
            }

            PPF_APP.PNTDATA = pds;

            sr.Close();
        }

        public static string[]
        splitFields(string strIn, string[] key)
        {
            string[] fields = null;
            if (strIn != null)
            {
                fields = strIn.Split(key, StringSplitOptions.RemoveEmptyEntries);
            }
            return fields;
        }

        private static void
        stripDESC(ref string buf0, string target)
        {
            int pos0, posX;

            if (buf0.Contains(target))
            {
                pos0 = buf0.IndexOf(target, System.StringComparison.Ordinal);
                if (buf0.Substring(pos0).Contains(" "))
                {
                    posX = buf0.IndexOf(" ", pos0, System.StringComparison.Ordinal);
                    buf0 = string.Format("{0}{1}", buf0.Substring(0, pos0 + target.Length + 2), buf0.Substring(posX));
                }
                else
                {
                    buf0 = buf0.Substring(0, pos0 + target.Length + 2);
                }
            }
        }
    }
}