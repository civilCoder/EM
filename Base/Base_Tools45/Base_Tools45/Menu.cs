using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;

namespace Base_Tools45
{
    public class Commands
    {
        const string cuiSectionName = "TTIF_Menus";

        [CommandMethod("MIC")]
        public static void MenuWithIcon()
        {
            var acadCui =
              new CustomizationSection(
                Application.GetSystemVariable("MENUNAME") + ".cuix"
              );
            var acMenuGroup = acadCui.MenuGroup;

            // Check if the macro group exists

            var grp = default(MacroGroup);
            if (acMenuGroup.MacroGroups.Contains(cuiSectionName))
                grp = acMenuGroup.FindMacroGroup(cuiSectionName);
            else
                grp = new MacroGroup(cuiSectionName, acMenuGroup);

            var mac =
              grp.CreateMenuMacro(
                "Table Export 2",
                "_.TABLEEXPORT2",
                "",
                "Exports a table to a Unicode-compatible CSV",
                MacroType.Edit,
                "tableexp.bmp",
                "tableexp.bmp",
                ""
              );

            // If the macros in the group, remove it
            // (otherwise we need to get it from the group for it
            // not to be added back in as duplicate when we
            // use it later... or so it seems)

            if (grp.MenuMacros.Contains(mac))
                grp.MenuMacros.Remove(mac);

            grp.AddMacro(mac);

            // Cycle through the PopMenus

            foreach (PopMenu menu in acadCui.MenuGroup.PopMenus)
            {
                // Ignore all except the "Edit" context menu

                if (!menu.Aliases.Contains("CMEDIT"))
                    continue;

                // Default position is the last in the list

                var pos = menu.PopMenuItems.Count;

                for (int i = 0; i < menu.PopMenuItems.Count; i++)
                {
                    // If we find the "Add Selected" item, then
                    // we'll insert our menu in front of that

                    var item = menu.PopMenuItems[i] as PopMenuItem;
                    if (item != null && item.MacroID == "ID_AddSelected")
                    {
                        pos = i - 1;
                        break;
                    }
                }

                // Add a separator and then the new menu item, but
                // only if it isn't already where we expect it to be

                var testPos =
                  pos < menu.PopMenuItems.Count ?
                  pos - 1 :
                  menu.PopMenuItems.Count - 1;

                var test = menu.PopMenuItems[testPos] as PopMenuItem;
                if (
                  test == null ||
                  (test != null && test.Name != "Unicode Export...")
                )
                {
                    var sep = new PopMenuItem(menu, pos);
                    var newItem =
                      new PopMenuItem(mac, "Unicode Export...", menu, pos + 1);

                    menu.PopMenuItems.Add(sep);
                    menu.PopMenuItems.Add(newItem);

                    // Only save and reload the CUI if we added our menu
                    // (the reload is particularly expensive)

                    acadCui.Save(true);
                    Application.ReloadAllMenus();
                }
                break;
            }
        }
    }
}
