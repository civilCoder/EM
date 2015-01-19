using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Prof
    {
        public static Profile
        addProfileByLayout(string nameProfile, ObjectId idAlign, ObjectId idLayer, ObjectId idProfileStyle, ObjectId idLabelStyle)
        {
            Profile profile = null;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = Profile.CreateByLayout(nameProfile, idAlign, idLayer, idProfileStyle, idLabelStyle);
                    profile = (Profile)tr.GetObject(id, OpenMode.ForRead);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 27", ex.Message));
            }
            return profile;
        }

        public static Profile
        addProfileByLayout(string nameProfile, string nameAlign, string nameLayer, string nameProfileStyle, string nameLabelSetStyle)
        {
            Profile profile = null;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = Profile.CreateByLayout(nameProfile, BaseObjs._civDoc, nameAlign, nameLayer, nameProfileStyle, nameLabelSetStyle);
                    profile = (Profile)tr.GetObject(id, OpenMode.ForRead);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 27", ex.Message));
            }
            return profile;
        }

        public static Profile
        addProfileBySurface(string nameProfile, ObjectId idAlign, ObjectId idSurf, ObjectId idLayer, ObjectId idProfileStyle, ObjectId idLabelSet)
        {
            Profile profile = null;
            ObjectId id = ObjectId.Null;
            CivilDocument civDoc = BaseObjs._civDoc;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        profile = Prof.getProfile(idAlign, nameProfile);
                        if (profile != null)
                            Prof.removeProfile(idAlign, nameProfile);

                        id = Profile.CreateFromSurface(nameProfile, idAlign, idSurf, idLayer, idProfileStyle, idLabelSet);
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 45", ex.Message));
                    }
                    profile = (Profile)tr.GetObject(id, OpenMode.ForWrite);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 52", ex.Message));
            }
            return profile;
        }

        public static Profile
        addProfileBySurface(string nameProfile, Alignment align, TinSurface surf, string nameLayer, string nameLabelSet)
        {
            ObjectId id = ObjectId.Null;
            CivilDocument civDoc = BaseObjs._civDoc;

            ObjectId idAlign = ObjectId.Null;
            ObjectId idSurf = ObjectId.Null;
            ObjectId idLayer = ObjectId.Null;
            ObjectId idStyle = ObjectId.Null;
            ObjectId idLabel = ObjectId.Null;
            Profile profile = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    idAlign = align.Id;
                    idSurf = surf.ObjectId;
                    idLayer = Layer.manageLayers(nameLayer);

                    try
                    {
                        ProfileStyleCollection idStyles = civDoc.Styles.ProfileStyles;
                        if (idStyles.Contains(nameProfile))
                            idStyle = idStyles[nameProfile];
                        else
                            idStyle = idStyles["Standard"];

                        ProfileLabelSetStyleCollection idLabelStyles = civDoc.Styles.LabelSetStyles.ProfileLabelSetStyles;
                        if (idLabelStyles.Contains(nameLabelSet))
                            idLabel = idLabelStyles[nameLabelSet];
                        else
                            idLabel = idLabelStyles["Standard"];

                        try
                        {
                            id = Profile.CreateFromSurface(nameProfile, idAlign, idSurf, idLayer, idStyle, idLabel);
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 91", ex.Message));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 95", ex.Message));
                    }
                    profile = (Profile)tr.GetObject(id, OpenMode.ForWrite);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 102", ex.Message));
            }
            return profile;
        }

        public static ProfileView
        addProfileView(ObjectId alignID, Point3d pnt3dIns)
        {
            ObjectId id = ProfileView.Create(alignID, pnt3dIns);
            ProfileView pview = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    pview = (ProfileView)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 118", ex.Message));
            }
            return pview;
        }

        public static ProfileView
        addProfileView(ObjectId idAlign, Point3d pnt3dIns, ObjectId idProfileViewBandSet, ObjectId idProfileViewStyle)
        {
            ProfileView pview = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)tr.GetObject(idAlign, OpenMode.ForRead);
                    //ObjectId id = ProfileView.Create(idAlign, pnt3dIns, align.Name, idProfileViewBandSet, idProfileViewStyle);
                    ObjectId id = ProfileView.Create(BaseObjs._civDoc, idAlign.getAlignName(), idProfileViewBandSet, idAlign, pnt3dIns);

                    pview = (ProfileView)tr.GetObject(id, OpenMode.ForWrite);
                    pview.Layer = align.Layer;
                    pview.StyleId = idProfileViewStyle;
                    //pview.StationStart = align.StartingStation;
                    //pview.StationEnd = align.EndingStation;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 138", ex.Message));
            }
            return pview;
        }

        public static Profile
        getProfile(string nameAlign, string nameProfile)
        {
            Profile profile = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)tr.GetObject(Align.getAlignmentID(nameAlign), OpenMode.ForRead);

                    ObjectIdCollection profileIDs = align.GetProfileIds();
                    foreach (ObjectId profileID in profileIDs)
                    {
                        Profile p = (Profile)tr.GetObject(profileID, OpenMode.ForRead);

                        if (p.Name == nameProfile)
                        {
                            profile = p;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 161", ex.Message));
            }
            return profile;
        }

        public static Profile
        getProfile(ObjectId idAlign, string nameProfile)
        {
            Profile profile = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)idAlign.GetObject(OpenMode.ForRead);
                    ObjectIdCollection profileIDs = align.GetProfileIds();
                    foreach (ObjectId profileID in profileIDs)
                    {
                        Profile p = (Profile)profileID.GetObject(OpenMode.ForRead);

                        if (p.Name == nameProfile)
                        {
                            profile = p;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 183", ex.Message));
            }
            return profile;
        }

        public static double
        getProfileElev(ObjectId idAlign, string nameProfile, double station)
        {
            double elev = 0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)idAlign.GetObject(OpenMode.ForRead);
                    ObjectIdCollection profileIDs = align.GetProfileIds();
                    foreach (ObjectId profileID in profileIDs)
                    {
                        Profile p = (Profile)profileID.GetObject(OpenMode.ForRead);

                        if (p.Name == nameProfile)
                        {
                            try
                            {
                                elev = p.ElevationAt(station);
                            }
                            catch
                            {
                                try
                                {
                                    elev = p.ElevationAt(station + 0.01);
                                }
                                catch
                                {
                                    elev = p.ElevationAt(station - 0.01);
                                }
                            }
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 183", ex.Message));
            }
            return elev;
        }

        public static ObjectId
        getProfileID(ObjectId idAlign, string nameProfile)
        {
            ObjectId idProfile = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Alignment align = (Alignment)tr.GetObject(idAlign, OpenMode.ForRead);

                    ObjectIdCollection profileIDs = align.GetProfileIds();
                    foreach (ObjectId id in profileIDs)
                    {
                        Profile profile = (Profile)tr.GetObject(id, OpenMode.ForRead);

                        if (profile.Name == nameProfile)
                        {
                            idProfile = profile.ObjectId;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 206", ex.Message));
            }
            return idProfile;
        }

        public static double
        getProfileViewVerticalScale(ProfileView pv)
        {
            Extents3d ext3d = (Extents3d)pv.Bounds;
            Point3d pnt3dMin = ext3d.MinPoint;
            Point3d pnt3dMax = ext3d.MaxPoint;
            double scale = (pnt3dMax.Y - pnt3dMin.Y) / (pv.ElevationMax - pv.ElevationMin);
            return scale;
        }

        public static ProfileView
        getProfileView(string nameAlign, string nameProfileView)
        {
            ProfileView profileview = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId idAlign = Align.getAlignmentID(nameAlign);
                    Alignment align = (Alignment)idAlign.GetObject(OpenMode.ForRead);
                    ObjectIdCollection viewIDs = align.GetProfileViewIds();
                    foreach (ObjectId viewID in viewIDs)
                    {
                        ProfileView pv = (ProfileView)viewID.GetObject(OpenMode.ForRead);
                        if (pv.Name == nameProfileView)
                        {
                            profileview = pv;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 228", ex.Message));
            }
            return profileview;
        }

        public static ObjectIdCollection
        getProfileViews()
        {
            ObjectIdCollection idAligns = BaseObjs._civDoc.GetAlignmentIds();
            ObjectIdCollection idsPV = new ObjectIdCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in idAligns)
                    {
                        Alignment align = (Alignment)tr.GetObject(id, OpenMode.ForRead);
                        ObjectIdCollection viewIDs = align.GetProfileViewIds();
                        foreach (ObjectId viewID in viewIDs)
                        {
                            idsPV.Add(viewID);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 228", ex.Message));
            }
            return idsPV;
        }

        public static List<Point3d>
        getProfileViewOriginAndScale(ObjectId idPView, out double scale)
        {
            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;

            List<Point3d> pnts3d = new List<Point3d>();
            ProfileView pview = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    pview = (ProfileView)tr.GetObject(idPView, OpenMode.ForWrite);
                    if (pview.ElevationRangeMode == ElevationRangeType.Automatic)
                    {
                        pview.ElevationRangeMode = ElevationRangeType.UserSpecified;
                        pview.FindXYAtStationAndElevation(pview.StationStart, pview.ElevationMin, ref x1, ref y1);
                        pview.FindXYAtStationAndElevation(pview.StationEnd, pview.ElevationMax, ref x2, ref y2);
                    }
                    else
                    {
                        pview.FindXYAtStationAndElevation(pview.StationStart, pview.ElevationMin, ref x1, ref y1);
                        pview.FindXYAtStationAndElevation(pview.StationEnd, pview.ElevationMax, ref x2, ref y2);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 258", ex.Message));
            }

            scale = (y2 - y1) / (pview.ElevationMax - pview.ElevationMin);
            pnts3d.Add(new Point3d(x1, y1, 0.0));
            pnts3d.Add(new Point3d(x2, y2, 0.0));

            return pnts3d;
        }

        public static void
        removeProfile(ObjectId idAlign, string nameProfile)
        {
            ObjectId profileID = getProfileID(idAlign, nameProfile);
            if (profileID == ObjectId.Null)
                return;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    Profile profile = (Profile)tr.GetObject(profileID, OpenMode.ForWrite);
                    if (profile != null)
                        profile.Erase();
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 281", ex.Message));
                }
                tr.Commit();
            }
        }

        public static void
        removeProfileViews(Alignment align)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectIdCollection ids = align.GetProfileViewIds();
                    foreach (ObjectId id in ids)
                    {
                        ProfileView pview = (ProfileView)tr.GetObject(id, OpenMode.ForWrite);
                        pview.Erase();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof.cs: line: 308", ex.Message));
            }
        }
    }
}