using Autodesk.AutoCAD.DatabaseServices;

namespace Grading
{
    public class Grading_xRefID
    {
        public string name;
        public ObjectId id;

        public Grading_xRefID(string name, ObjectId id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
