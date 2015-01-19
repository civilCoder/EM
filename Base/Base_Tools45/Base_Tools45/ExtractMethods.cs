using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Base_Tools45
{
    public class ExtractMethods
    {
        public static void GetCmdMethodInfo()
        {
            Assembly assem = Assembly.GetExecutingAssembly();

            List<string> cmdsGlobal = new List<string>();
            List<string> cmdsLocal = new List<string>();
            List<string> cmdsGroup = new List<string>();

            Module[] mods = assem.GetModules(true);
            foreach (Module mod in mods)
            {
                Type[] types = mod.GetTypes();
                foreach (Type type in types)
                {
                    ResourceManager rm = new ResourceManager(type.FullName, assem);
                    rm.IgnoreCase = true;

                    MethodInfo[] meths = type.GetMethods();
                    foreach (MethodInfo meth in meths)
                    {
                        object[] atts = meth.GetCustomAttributes(typeof(CommandMethodAttribute), true);
                        foreach (object att in atts)
                        {
                            CommandMethodAttribute cmdAtt = (CommandMethodAttribute)att;
                            if (cmdAtt != null)
                            {
                                string nameGlobal = cmdAtt.GlobalName;
                                string nameLocal = nameGlobal;
                                string nameLocalized = cmdAtt.LocalizedNameId;
                                if (nameLocalized != null)
                                {
                                    try
                                    {
                                        nameLocal = rm.GetString(nameLocalized);
                                    }
                                    catch (System.Exception ex)
                                    {
                BaseObjs.writeDebug(ex.Message + " ExtractMethods.cs: line: 48");
                                    }
                                }
                                cmdsGlobal.Add(nameGlobal);
                                cmdsLocal.Add(nameLocal);
                                if (cmdAtt.GroupName != null && !cmdsGroup.Contains(cmdAtt.GroupName))
                                    cmdsGlobal.Add(cmdAtt.GroupName);
                            }
                        }
                    }
                }
            }
        }
    }
}
