using Autodesk.AutoCAD.DatabaseServices;

namespace Stake
{
    public class AlgnData
    {
        public string AlignLayer { get; set; }

        public string AlignName { get; set; }

        public Handle AlignHandle { get; set; }

        public ObjectId AlignID { get; set; }

        public Handle TableHandle { get; set; }
    }
}