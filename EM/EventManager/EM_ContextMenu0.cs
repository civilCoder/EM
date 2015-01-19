using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using System.Diagnostics;

namespace EventManager
{
    public class EM_ContextMenu0
    {
        private const string cuiSectionName = "";

        public static void replaceTextEditContextMenu()
        {
            var acadCui = new Autodesk.AutoCAD.Customization.CustomizationSection(string.Format("{0}.cuix", Application.GetSystemVariable("MENUNAME")));
            var acMenuGroup = acadCui.MenuGroup;

            //var grp = default(MacroGroup);

            var popMenus = acMenuGroup.PopMenus;
            foreach (PopMenu pMenu in popMenus)
                Debug.Print(string.Format("{0}\n", pMenu.Name));
        }
    }
}
