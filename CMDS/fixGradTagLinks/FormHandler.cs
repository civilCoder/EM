using Autodesk.AutoCAD.ApplicationServices;

namespace fixGradeTagLinks
{
    public sealed class FormHandler
    {
        private static FormHandler frmHandler = new FormHandler();

        public static FormHandler fHandler{
            get{
                return frmHandler;
            }
        }

        private FormHandler(){
            wResults = new winResults();
        }

        public winResults wResults { get; set; }
    }
}