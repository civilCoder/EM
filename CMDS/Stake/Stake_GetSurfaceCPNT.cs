using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using sFrms = Stake.Forms.Stake_Forms;

namespace Stake
{
    public static class Stake_GetSurfaceCPNT
    {
        private static Forms.frmPickXref    fPickXref;
        public static void
        getSurfaceFromXRef(string nameSurf, string strSource)
        {
            string nameXRef = "";
            if (nameSurf == "CPNT-ON")
                nameXRef = "GCAL";
            if (nameSurf == "EXIST")
                nameXRef = "CONT";

            ResultBuffer rb = xRef.getXRefsContainingTargetDwgName(nameXRef);
            TypedValue[] tvs = rb.AsArray();

            switch (tvs.Length)
            {
                case 0:
                    Application.ShowAlertDialog(string.Format("{0} not attached", nameXRef = (nameSurf == "CPNT-ON") ? "GCAL" : "CONT"));
                    break;

                case 2:
                    foreach (TypedValue tv in tvs)
                        if (tv.TypeCode == 1001)
                            nameXRef = tv.Value.ToString();

                    break;

                default:
                    string prompt = string.Format("Multible {0} are attached. Select one to use for staking: ", nameXRef = (nameSurf == "CPNT-ON") ? "GCAL" : "CONT");
                    Application.ShowAlertDialog(prompt);
                    fPickXref = Forms.Stake_Forms.sForms.fPickXref;
                    foreach (TypedValue tv in tvs)
                    {
                        if (tv.TypeCode == 1001)
                        {
                            fPickXref.lboxSelectXref.Items.Add(tv.Value.ToString());
                        }
                    }
                    fPickXref.Text = string.Format("Multible {0} are attached. Select one to use for staking: ", nameXRef = (nameSurf == "CPNT-ON") ? "GCAL" : "CONT");
                    Application.ShowModalDialog(null, fPickXref, false);

                    nameXRef = fPickXref.nameXRef;
                    if (nameXRef == "")
                        return;

                    break;
            }

            Surf.removeSurface(nameSurf);
            BlockReference br = xRef.getXRefBlockReference(nameXRef);

            TinSurface s = xRef.getXRefTinSurface(br.Id, nameSurf);
            if (s == null)
                return;
            string nameLayer = string.Format("{0}-SURFACE", nameSurf);
            if (nameSurf == "CPNT-ON")
                nameLayer = "CPNT-SURFACE";
            Layer.manageLayers(nameLayer);

            TinSurface surf = null;
            string nameStyle = "CPNT-ON";
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                if (nameSurf == "CONT")
                    nameStyle = "EXIST";
                ObjectId idStyle = Surf_Styles.getSurfaceStyle(nameStyle);
                ObjectId idSurf = TinSurface.Create(nameSurf, idStyle);
                surf = (TinSurface)idSurf.GetObject(OpenMode.ForWrite);
                surf.Layer = nameLayer;
                surf.Description = string.Format("COPIED FROM {0} - {1}", nameXRef, DateTime.Now);
                tr.Commit();
            }

            surf.PasteSurface(s.ObjectId);

            Layer.manageLayer(nameLayer, layerFrozen: true);
            if (nameSurf == "CPNT-ON")
                if (strSource == "STAKE")
                {
                    sFrms.sForms.fStake.SurfaceCPNT = surf;
                }
        }
    }
}