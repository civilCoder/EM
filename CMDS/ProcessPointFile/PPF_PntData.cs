namespace ProcessPointFile
{
    public class PPF_PntData
    {
        public PPF_PntData()
        {
            PntNum = 0;
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            Desc = "";
            Code1 = "";
            Code2 = "";
            Code3 = "";
            Code4 = "";
            Code5 = "";
        }

        public PPF_PntData(int pntNum, double x, double y, double z, string desc,
            string code1, string code2, string code3, string code4, string code5)
        {
            PntNum = pntNum;
            X = x;
            Y = y;
            Z = z;
            Desc = desc;
            Code1 = code1;
            Code2 = code2;
            Code3 = code3;
            Code4 = code4;
            Code5 = code5;
        }

        public int PntNum
        {
            get;
            set;
        }

        public double X
        {
            get;
            set;
        }

        public double Z
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public string Desc
        {
            get;
            set;
        }

        public string Code1
        {
            get;
            set;
        }

        public string Code2
        {
            get;
            set;
        }

        public string Code3
        {
            get;
            set;
        }

        public string Code4
        {
            get;
            set;
        }

        public string Code5
        {
            get;
            set;
        }
    }
}