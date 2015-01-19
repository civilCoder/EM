//using Autodesk.Civil.DatabaseServices;
using Microsoft.Win32;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class UserSettings
    {
        /// <summary>
        /// get user settings from registry - for grading tools
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static int
        getUserSettings(string strKey)
        {
            int intValue = 1;
            int intReturnValue = 0;

            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "TEI";
            const string keyName = userRoot + "\\" + subkey;

            try
            {
                intReturnValue = (int)Registry.GetValue(keyName, strKey, intValue);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UserSettings.cs: line: 32");
            }

            return intReturnValue;
        }

        /// <summary>
        /// set user settings in registry for grading tools
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="intValue"></param>
        public static void
        setUserSettings(string strKey, int intValue)
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "TEI";
            const string keyName = userRoot + "\\" + subkey;
            Registry.SetValue(keyName, strKey, intValue);
        }
    }
}
