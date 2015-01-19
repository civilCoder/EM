using System.Collections.Generic;

namespace EventManager
{
    public static class EW_Storage
    {
        private static List<EM_EData> _enData;

        public static List<EM_EData> enData
        {
            get
            {
                return _enData;
            }
            set
            {
                _enData = value;
            }
        }
    }
}
