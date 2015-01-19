using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace SDrain
{
    public static class SDrain_App
    {
        public static void
        checkPointsToAlign(){
            SelectionSet ss = null;

            Debugg debugg = new Debugg();
            DefaultTraceListener dtl = debugg.getListner();
            dtl.LogFileName = @"M:\John\debug.txt";
            
            bool exists;
            ObjectId idDictObject = Dict.getNamedDictionary("ObjectDict", out exists);          
            ObjectId idDictSED = Dict.getNamedDictionary("SEDTagDict", out exists);

            ObjectIdCollection idsAlign = Align.getAlignmentIDs();
            ss = (SelectionSet)Select.buildSSet(typeof(CogoPoint), true);
            ObjectId[] idsCgPnt = ss.GetObjectIds();
            Dictionary<ObjectId, List<Handle>> dictM = new Dictionary<ObjectId, List<Handle>>();
            double station = 0.0;
            double offset = 0.0;

            foreach (ObjectId id in idsAlign)
            {
                Alignment align = (Alignment)id.getEnt();
                List<Handle> hList = new List<Handle>();
                dictM.Add(id, hList);
                dtl.Write(string.Format("{0}\n", id.ToString()));
                foreach (ObjectId idCgPnt in idsCgPnt)
                {
                    Point3d pnt3d = idCgPnt.getCogoPntCoordinates();
                    try
                    {
                        align.StationOffset(pnt3d.X, pnt3d.Y, ref station, ref offset);
                        if (station >= align.StartingStation && station <= align.EndingStation)
                        {
                            if (System.Math.Abs(offset) < 0.01)
                            {
                                Handle h = idCgPnt.getHandle();
                                hList.Add(h);
                                dtl.Write(string.Format("{0}\n", h.ToString()));
                            }
                        }
                    }
                    catch (System.Exception ex){
                        Application.ShowAlertDialog(ex.Message + " SDrain_App.cs: line: 66");
                        BaseObjs.writeDebug(ex.Message + " SDrain_App.cs: line: 66");
                    }
                }
            }

            //List<TypedValue[]> xrecs = Dict.getXRecs(idDictObject);
            //if (xrecs != null){
            //    foreach (TypedValue[] tvs in xrecs){
            //        dtl.Write(string.Format("{0}\n", tvs[2].Value.ToString()));                  
            //    }
            //}
            //Xrecord xRec = null;
            //List<DBDictionaryEntry> entries = Dict.getEntries(idDictObject);
            //foreach (DBDictionaryEntry entry in entries){
            //    xRec = Dict.getXRec(idDictObject, entry);                
            //}

            List<Handle> hListBad = new List<Handle>();
            ObjectId idBR = ObjectId.Null;
            ss = Select.buildSSet(typeof(BlockReference),true);
            ObjectId[] idsSS = ss.GetObjectIds();
            string idStr = idsSS[0].ToString();
            idStr = idStr.Replace("(", "");
            idStr = idStr.Replace(")", "");
            Int64 id64 = Int64.Parse(idStr);
            
            Int32 id32 = System.Convert.ToInt32(id64);
            
            foreach(ObjectId id in idsSS){
                Entity ent = (Entity)id.getEnt();
                BlockReference br = (BlockReference)ent;
                if (br.Name.Contains("sedtag") || br.Name.Contains("sdetag")){
                    ResultBuffer rbBR = Dict.getXRec(idDictObject, id.ToString());
                    if (rbBR != null){
                        TypedValue[] tvsBR = rbBR.AsArray();
                        Handle hPnt = tvsBR.getObjectId(2).getHandle();
                        ResultBuffer rbPnt = Dict.getXRec(idDictSED, hPnt.ToString());
                        if (rbPnt != null){
                            TypedValue[] tvsPnt = rbPnt.AsArray();                        
                            ObjectId idAlign = tvsPnt.getObjectId(2);
                            List<Handle> hList2 = null;
                            dictM.TryGetValue(idAlign, out hList2);
                            if (!hList2.Contains(hPnt)){
                                TypedValue[] tvsSED = new TypedValue[3];
                                tvsSED.SetValue(new TypedValue(1001, br.Handle.ToString()), 0);
                                Handle hLdr = tvsBR.getObjectId(0).getHandle();      //check
                                tvsSED.SetValue(new TypedValue(1002, hLdr), 1);
                                tvsSED.SetValue(new TypedValue(1003, idAlign.getHandle()), 2);
                            }else{
                                if(tvsPnt.getObjectId(2).getHandle() != idAlign.getHandle()){
                                    TypedValue[] tvsPntNew = new TypedValue[3];
                                    tvsPntNew[0] = tvsPnt[0];
                                    tvsPntNew[1] = tvsPnt[1];
                                    tvsPntNew[2] = new TypedValue(1003, idAlign.getHandle());
                                }
                            }
                        }

                    }else{
                        hListBad.Add(id.getHandle());
                    }
                }
            }





        }
    }
}
