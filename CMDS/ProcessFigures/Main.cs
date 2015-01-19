namespace ProcessFigures
{
    public static class Main
    {
        private static long lLastBrkLine;

        public static long LASTBRKLINE
        {
            get
            {
                return lLastBrkLine;
            }
            set
            {
                lLastBrkLine = value;
            }
        }

        public struct pntList
        {
            public short index;
            public double length;
        }
    }
}