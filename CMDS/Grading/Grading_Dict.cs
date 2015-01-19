using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using System.Windows.Forms;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Grading
{
    public static class Grading_Dict
    {
        private static Grading_Palette gPalette = Grading_Palette.gPalette;
        private static myForms.GradeSite fGrading = gPalette.pGrading;

        public static void
        addBrksToPntXDict(ObjectId idPoly3dRF, ObjectId idPoly3dTAR, double offset, double deltaZ, double beg, double end)
        {
            ResultBuffer rb = idPoly3dRF.getXData(apps.lnkBrks2);  //FL stores end Cogo Points
            if (rb == null)
                return;
            List<Handle> handles = rb.rb_handles();
            if (handles.Count == 0)
                return;

            ObjectId idDictM = ObjectId.Null;
            bool exists = false;
            try
            {
                idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);     //dictionary for storing edge parameters
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_Dict.cs: line: 34");
            }

            ObjectId idDictPntBEG = Dict.getSubEntry(idDictM, handles[0].ToString());       //Cogo Point stores edge parameters in dictionary
            if (idDictPntBEG == ObjectId.Null)
                idDictPntBEG = Dict.addSubDict(idDictM, handles[0].ToString());

            ObjectId idDictB = Dict.getSubEntry(idDictPntBEG, idPoly3dTAR.getHandle().ToString());
            if (idDictB == ObjectId.Null)
                idDictB = Dict.addSubDict(idDictPntBEG, idPoly3dTAR.getHandle().ToString());
            else
            {
                Dict.removeSubEntry(idDictPntBEG, idPoly3dTAR.getHandle().ToString());
                idDictB = Dict.addSubDict(idDictPntBEG, idPoly3dTAR.getHandle().ToString());
            }

            TypedValue tv = new TypedValue(1040, offset);
            Dict.addXRec(idDictB, "Offset", new ResultBuffer(tv));

            tv = new TypedValue(1040, deltaZ);
            Dict.addXRec(idDictB, "DeltaZ", new ResultBuffer(tv));

            tv = new TypedValue(1005, idPoly3dRF.getHandle().ToString());
            Dict.addXRec(idDictB, "HandleFL", new ResultBuffer(tv));

            tv = new TypedValue(1040, System.Math.Round(beg, 3));
            Dict.addXRec(idDictB, "Beg", new ResultBuffer(tv));

            tv = new TypedValue(1040, System.Math.Round(end, 3));
            Dict.addXRec(idDictB, "End", new ResultBuffer(tv));


            ObjectId idDictPntEND = Dict.getSubEntry(idDictM, handles[1].ToString());       //Cogo Point stores edge parameters in dictionary
            if (idDictPntEND == ObjectId.Null)
                idDictPntEND = Dict.addSubDict(idDictM, handles[1].ToString());

            ObjectId idDictE = Dict.getSubEntry(idDictPntEND, idPoly3dTAR.getHandle().ToString());
            if (idDictE == ObjectId.Null)
                idDictE = Dict.addSubDict(idDictPntEND, idPoly3dTAR.getHandle().ToString());
            else
            {
                Dict.removeSubEntry(idDictPntEND, idPoly3dTAR.getHandle().ToString());
                idDictE = Dict.addSubDict(idDictPntEND, idPoly3dTAR.getHandle().ToString());
            }

            tv = new TypedValue(1040, offset);
            Dict.addXRec(idDictE, "Offset", new ResultBuffer(tv));

            tv = new TypedValue(1040, deltaZ);
            Dict.addXRec(idDictE, "DeltaZ", new ResultBuffer(tv));

            tv = new TypedValue(1005, idPoly3dRF.getHandle().ToString());
            Dict.addXRec(idDictE, "HandleFL", new ResultBuffer(tv));

            tv = new TypedValue(1040, System.Math.Round(beg, 3));
            Dict.addXRec(idDictE, "Beg", new ResultBuffer(tv));

            tv = new TypedValue(1040, System.Math.Round(end, 3));
            Dict.addXRec(idDictE, "End", new ResultBuffer(tv));
        }

        public static void
        deletePntDictionary()
        {
            Point3d pnt3dPicked = Pub.pnt3dO;
            Entity ent = Select.selectEntity(typeof(CogoPoint), "Select Cogo Point", "oops", out pnt3dPicked);
            CogoPoint cogoPnt = (CogoPoint)ent;

            bool exists = false;
            ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);
            Dict.removeSubEntry(idDictM, cogoPnt.Handle.ToString());
        }

        public static void
        printPntDictionary()
        {
            BaseObjs.acadActivate();

            ObjectId idCogoPnt = Base_Tools45.C3D.CgPnt.selectPointEntity("", osMode: 8);

            bool exists = false;
            ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);

            ObjectId idDictPnt = Dict.getSubEntry(idDictM, idCogoPnt.getHandle().ToString());
            if (idDictPnt == ObjectId.Null)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Point does not have a Dictionary");
                return;
            }

            List<DBDictionaryEntry> entries = idDictPnt.getDictEntries();
            if (entries.Count == 0)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Point Dictionary is empty.");
                return;
            }

            string output = string.Empty;
            foreach (DBDictionaryEntry entry in entries)
            {
                ObjectId idDictX = Dict.getSubEntry(idDictPnt, entry.Key);

                ResultBuffer rb2 = Dict.getXRec(idDictX, "Offset");
                TypedValue[] tvs = rb2.AsArray();
                output = string.Format("{0}  Offset: {1}", entry.Key, tvs[0].Value.ToString());

                rb2 = Dict.getXRec(idDictX, "DeltaZ");
                tvs = rb2.AsArray();
                output = string.Format("{0}\r{1}  DeltaZ: {2}", output, entry.Key, tvs[0].Value.ToString());

                rb2 = Dict.getXRec(idDictX, "HandleFL");
                tvs = rb2.AsArray();
                output = string.Format("{0}\r{1}  HandleFL: {2}", output, entry.Key, tvs[0].Value.ToString());

                rb2 = Dict.getXRec(idDictX, "Beg");
                tvs = rb2.AsArray();
                output = string.Format("{0}\r{1}  Begin Sta.: {2}", output, entry.Key, tvs[0].Value.ToString());

                rb2 = Dict.getXRec(idDictX, "End");
                tvs = rb2.AsArray();
                output = string.Format("{0}\r{1}  End Sta.: {2}", output, entry.Key, tvs[0].Value.ToString());
                MessageBox.Show(output);
            }
        }

        public static void
        remove3dPolyFromPntDict(ObjectId idPoly3d, ResultBuffer xData3dP2)
        {
            bool exists = false;
            ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);

            TypedValue[] tvs = xData3dP2.AsArray();
            foreach (TypedValue tv in tvs)
            {
                ObjectId idDictPnt = Dict.getSubEntry(idDictM, tv.Value.ToString());
                Dict.deleteXRec(idDictPnt, idPoly3d.getHandle().ToString());
            }
        }

        public static void
        updateDictVGR()
        {
            Dict.setCmdDefault("cmdBV", "CONTROL", fGrading.cmdBV_Control);
            Dict.setCmdDefault("cmdBV", "SOURCE", fGrading.cmdBV_Source);
            Dict.setCmdDefault("cmdBV", "GUTTERDEPTH", fGrading.cmdBV_GutterDepth);
            Dict.setCmdDefault("cmdBV", "GUTTERWIDTH", fGrading.cmdBV_GutterWidth);
        }
    }
}
