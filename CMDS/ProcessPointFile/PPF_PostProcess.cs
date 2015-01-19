using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProcessPointFile
{
    public static class PPF_PostProcess
    {
        private static List<string> targetDesc = new List<string>() {
            "BCB",
            "BCC",
            "BW",
            "CLF",
            "DL",
            "EC",
            "EP",
            "EW",
            "FL",
            "FW",
            "GB",
            "LP",
            "MS",
            "TB",
            "TC",
            "TOE",
            "TOP",
            "tr",
            "VG",
            "WIF"
        };

        public static void
        postProcPntFile()
        {
            if (PPF_APP.PNTDATA != null)
            {
                List<PPF_PntData> pds = PPF_APP.PNTDATA;

                var query = pds.GroupBy(pd => pd.Desc)
                               .Select(group => new
                               {
                                   Desc = @group.Key,
                                   pntGrp = @group.OrderBy(pn => pn.PntNum)
                               });

                pds = new List<PPF_PntData>();
                List<string> pntDescs = new List<string>();

                PPF_PntData pdPrior = new PPF_PntData();
                foreach (var group in query)
                {
                    pntDescs.Add(@group.Desc);
                    foreach (var pd in @group.pntGrp)
                    {
                        if (pd != pdPrior)
                        {
                            pd.Code1 = "BEG";
                        }
                        pds.Add(pd);
                        pdPrior = pd;
                    }
                }

                PPF_APP.DESCLIST = pntDescs;
                PPF_APP.PNTDATA = pds;
            }
        }

        public static bool
        testDesc(string pntDesc)
        {
            bool found = false;

            List<string> targetDesc2 = targetDesc;
            targetDesc2.Add("BC");
            targetDesc2.Add("BLDG");
            targetDesc2.Add("FG");
            targetDesc2.Add("FP");
            targetDesc2.Add("FS");
            targetDesc2.Add("MS");
            targetDesc2.Add("NG");
            targetDesc2.Add("T");
            targetDesc2.Add("TG");

            int outBuf;
            string buf = pntDesc;
            for (int i = pntDesc.Length - 1; i > 0; i--)
            {
                bool isNumeric = int.TryParse(pntDesc[i].ToString(CultureInfo.InvariantCulture), out outBuf);
                if (isNumeric)
                    continue;
                buf = pntDesc.Substring(0, i + 1);
            }

            if (targetDesc2.Contains(buf))
                found = true;

            return found;
        }

        public static bool
        testDescLine(ref string pntDesc)
        {
            bool found = false;

            int res;
            for (int i = 0; i < pntDesc.Length; i++)
            {
                if (int.TryParse(pntDesc[i].ToString(), out res))
                {
                    pntDesc = pntDesc.Substring(0, i);
                    break;
                }
            }

            if (targetDesc.Contains(pntDesc))
                found = true;

            return found;
        }
    }
}