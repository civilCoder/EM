using Autodesk.AutoCAD.ApplicationServices;
using System;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class SnapMode
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static int
        getOSnap()
        {
            int osMode = 0;
            int.TryParse(Application.GetSystemVariable("OSMODE").ToString(), out osMode);
            return osMode;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        public static void
        setOSnap(int mode)
        {
            Object osMode = mode;
            Application.SetSystemVariable("OSMODE", osMode);
        }
    }
}
