using System.Collections.Generic;

namespace EventManager
{
    public static class EM_Global
    {
        public const double radius = 0.125;

        public const string esc = "'\x03'";

        public static List<EM_EData> enData { get; set; }

        public static int DrawingUnits { get; set; }
    }
    
    public struct SegProps
    {
        public static double dir;
        public static double len;
    }
}
