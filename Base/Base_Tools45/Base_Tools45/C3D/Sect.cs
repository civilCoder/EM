using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Section = Autodesk.Civil.DatabaseServices.Section;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Sect
    {
        public static ObjectId
        addSampleLineGroupAndSampleLines(this ObjectId idAlign, string name)
        {
            ObjectId idSLG = ObjectId.Null;
            
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)tr.GetObject(idAlign, OpenMode.ForWrite);
                    idSLG = SampleLineGroup.Create(name, idAlign);
                    SampleLineGroup group = (SampleLineGroup)tr.GetObject(idSLG, OpenMode.ForWrite);

                    Station[] stations = align.GetStationSet(StationTypes.All);
                    foreach (Station station in stations)
                    {
                        ObjectId idSL = SampleLine.Create(string.Format("SL{0}", station.RawStation), idSLG, station.RawStation);
                        SampleLine SL = (SampleLine)tr.GetObject(idSL, OpenMode.ForWrite);
                        SL.StyleName = name;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect.cs: line: 30", ex.Message));
            }
            return idSLG;
        }

        public static void
        addSections(this ObjectId idAlign, ObjectId idSLG, double offLeft, double offRight)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SampleLineGroup group = (SampleLineGroup)tr.GetObject(idSLG, OpenMode.ForWrite);
                    foreach (ObjectId id in group.GetSampleLineIds())
                    {
                        SampleLine SL = (SampleLine)tr.GetObject(id, OpenMode.ForRead);
                        foreach (ObjectId idSection in SL.GetSectionIds())
                        {
                            Section section = (Section)tr.GetObject(idSection, OpenMode.ForWrite);                           
                            section.UpdateMode = SectionUpdateType.Dynamic;
                            section.LeftSwathWidth = offLeft;
                            section.RightSwathWidth = offRight;
                            section.StyleName = section.Name;
                            section.Layer = string.Format("{0}-SURFACE-SEC", section.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect.cs: line: 50", ex.Message));
            }
        }

        public static void
        addSurfaceToSample(this ObjectId idSLG, ObjectIdCollection idsSurface)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SampleLineGroup group = (SampleLineGroup)tr.GetObject(idSLG, OpenMode.ForWrite);
                    SectionSourceCollection sources = group.GetSectionSources();

                    foreach (SectionSource source in sources)
                    {
                        if (source.SourceType == SectionSourceType.TinSurface)
                        {
                            if (idsSurface.Contains(source.SourceId) == true)
                            {
                                source.IsSampled = true;
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect.cs: line: 71", ex.Message));
            }
        }

        public static ObjectIdCollection
        getSampleLineIDs(this ObjectId idSLG){
            ObjectIdCollection ids = new ObjectIdCollection();
            using(var tr = BaseObjs.startTransactionDb()){
                SampleLineGroup group = (SampleLineGroup)tr.GetObject(idSLG, OpenMode.ForWrite);
                ids = group.GetSampleLineIds();
                tr.Commit();
            }
            return ids;
        }
        public static void
        removeSampledSurfaces(this ObjectId idSLG)
        {           
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SampleLineGroup group = (SampleLineGroup)tr.GetObject(idSLG, OpenMode.ForWrite);
                    SectionSourceCollection sources = group.GetSectionSources();
                    foreach (SectionSource source in sources)
                    {
                        if (source.SourceType == SectionSourceType.TinSurface)
                        {
                            source.IsSampled = false;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect.cs: line: 89", ex.Message));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idAlign"></param>
        public static void
        removeSampleLineGroups(this ObjectId idAlign)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)tr.GetObject(idAlign, OpenMode.ForRead);
                    foreach (ObjectId id in align.GetSampleLineGroupIds())
                    {
                        SampleLineGroup group = (SampleLineGroup)tr.GetObject(id, OpenMode.ForWrite);
                        group.Erase();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect.cs: line: 110", ex.Message));
            }
        }
    }
}