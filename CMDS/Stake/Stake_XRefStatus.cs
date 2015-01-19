using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;

namespace Stake
{
    public static class Stake_XRefStatus
    {
        public static void
        reloadXRefs()
        {
            string strMsg = "";

            ObjectIdCollection ids = xRef.getXRefBlockReferences();
            ObjectIdCollection idsReload = new ObjectIdCollection();
            bool isLoaded = false;
            bool isXRefed = false;

            foreach (ObjectId id in ids)
            {
                string name = xRef.getXRefFileName(id);

                if (name.Contains("GCAL"))
                {
                    isLoaded = xRef.checkIfLoaded(name, out isXRefed);
                    if (!isLoaded)
                    {
                        if (!isXRefed)
                        {
                            string message = string.Format("{0} is not attached.", name);
                            Application.ShowAlertDialog(message);
                        }
                        else
                        {
                            idsReload.Add(id);

                            strMsg = strMsg + string.Format("\n{0} reloaded", name);
                        }
                    }
                }

                if (name.Contains("CNTL"))
                {
                    isLoaded = xRef.checkIfLoaded(name, out isXRefed);
                    if (!isLoaded)
                    {
                        if (!isXRefed)
                        {
                            string message = string.Format("{0} is not attached.", name);
                            Application.ShowAlertDialog(message);
                        }
                        else
                        {
                            idsReload.Add(id);

                            strMsg = strMsg + string.Format("\n{0} reloaded", name);
                        }
                    }
                }

                if (name.Contains("SD"))
                {
                    isLoaded = xRef.checkIfLoaded(name, out isXRefed);
                    if (!isLoaded)
                    {
                        if (!isXRefed)
                        {
                            string message = string.Format("{0} is not attached.", name);
                            Application.ShowAlertDialog(message);
                        }
                        else
                        {
                            idsReload.Add(id);

                            strMsg = strMsg + string.Format("\n{0} reloaded", name);
                        }
                    }
                }

                if (name.Contains("UTIL"))
                {
                    isLoaded = xRef.checkIfLoaded(name, out isXRefed);
                    if (!isLoaded)
                    {
                        if (!isXRefed)
                        {
                            string message = string.Format("{0} is not attached.", name);
                            Application.ShowAlertDialog(message);
                        }
                        else
                        {
                            idsReload.Add(id);

                            strMsg = strMsg + string.Format("\n{0} reloaded", name);
                        }
                    }
                }
            }

            if (strMsg != "")
            {
                Application.ShowAlertDialog(strMsg);
            }
        }
    }
}