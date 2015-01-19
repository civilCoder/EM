using System.Collections.Generic;

namespace DimPL
{
    public static class DimPL_Global
    {
        public const int maxRows075 = 15;
        public const int maxRows090 = 4;

        private static List<string> _XRefNames;

        public static List<string> XRefNames
        {
            get
            {
                return _XRefNames;
            }
            set
            {
                _XRefNames = value;
            }
        }

        public static int countUpdates { get; set; }
    }
}