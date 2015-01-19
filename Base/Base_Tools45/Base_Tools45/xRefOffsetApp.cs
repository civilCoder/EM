using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public class xRefOffsetApp : IExtensionApplication
    {
        // Maintain a list of temporary objects that require removal
        private readonly ObjectIdCollection _ids;

        /// <summary>
        ///
        /// </summary>
        public xRefOffsetApp()
        {
            _ids = new ObjectIdCollection();
        }

        /// <summary>
        ///
        /// </summary>
        public static void
        Terminate()
        {
        }

        // When the OFFSET command starts, let's add our selection manipulating event-handler
        /// <summary>
        ///
        /// </summary>
        public void Initialize()
        {
            DocumentCollection dm = Application.DocumentManager;

            // Remove any temporary objects at the end of the command

            dm.DocumentLockModeWillChange += delegate(object sender, DocumentLockModeWillChangeEventArgs e)
            {
                if (_ids.Count > 0)
                {
                    try
                    {
                        using (Transaction tr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                        {
                            foreach (ObjectId id in _ids)
                            {
                                DBObject obj = id.GetObject(OpenMode.ForWrite, true);
                                obj.Erase();
                            }
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " xRefOffsetApp.cs: line: 64");
                    }
                    _ids.Clear();
                }
            };

            // When a document is created, make sure we handle the important events it fires

            dm.DocumentCreated += delegate(object sender, DocumentCollectionEventArgs e)
            {
                e.Document.CommandWillStart += new CommandEventHandler(OnCommandWillStart);
                e.Document.CommandEnded += new CommandEventHandler(OnCommandFinished);
                e.Document.CommandCancelled += new CommandEventHandler(OnCommandFinished);
                e.Document.CommandFailed += new CommandEventHandler(OnCommandFinished);
            };

            // Do the same for any documents existing on application initialization

            foreach (Document doc in dm)
            {
                doc.CommandWillStart += new CommandEventHandler(OnCommandWillStart);
                doc.CommandEnded += new CommandEventHandler(OnCommandFinished);
                doc.CommandCancelled += new CommandEventHandler(OnCommandFinished);
                doc.CommandFailed += new CommandEventHandler(OnCommandFinished);
            }
        }

        void IExtensionApplication.Initialize()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        void IExtensionApplication.Terminate()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void
        OnCommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "OFFSET")
            {
                Document doc = (Document)sender;
                doc.Editor.PromptForEntityEnding += new PromptForEntityEndingEventHandler(OnPromptForEntityEnding);
            }
        }

        // And when the command ends, remove it
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void
        OnCommandFinished(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "OFFSET")
            {
                Document doc = (Document)sender;
                doc.Editor.PromptForEntityEnding -= new PromptForEntityEndingEventHandler(OnPromptForEntityEnding);
            }
        }

        // Here's where the heavy lifting happens...
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void
        OnPromptForEntityEnding(object sender, PromptForEntityEndingEventArgs e)
        {
            if (e.Result.Status == PromptStatus.OK)
            {
                Editor ed = sender as Editor;
                ObjectId objId = e.Result.ObjectId;
                Database db = objId.Database;

                try
                {
                    using (Transaction tr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                    {
                        // First get the currently selected object and check whether it's a block reference
                        BlockReference br = tr.GetObject(objId, OpenMode.ForRead) as BlockReference;
                        if (br != null)
                        {
                            // If so, we check whether the block table record to which it refers is actually from an XRef
                            ObjectId btrId = br.BlockTableRecord;
                            BlockTableRecord btr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                            if (btr != null)
                            {
                                if (btr.IsFromExternalReference)
                                {
                                    // If so, then we programmatically select the object underneath the pick-point already used
                                    PromptNestedEntityOptions pneo = new PromptNestedEntityOptions("");
                                    pneo.NonInteractivePickPoint = e.Result.PickedPoint;
                                    pneo.UseNonInteractivePickPoint = true;

                                    PromptNestedEntityResult pner = ed.GetNestedEntity(pneo);

                                    if (pner.Status == PromptStatus.OK)
                                    {
                                        try
                                        {
                                            ObjectId selId = pner.ObjectId;

                                            // Let's look at this programmatically-selected object, to see what it is

                                            DBObject obj = selId.GetObject(OpenMode.ForRead);

                                            // If it's a polyline vertex, we need to go one level up to the polyline itself

                                            if (obj is PolylineVertex3d || obj is Vertex2d)
                                                selId = obj.OwnerId;

                                            // We don't want to do anything at all for textual stuff, let's also make sure we
                                            // are dealing with an entity (should always be the case)

                                            if (obj is MText || obj is DBText || !(obj is Entity))
                                                return;

                                            // Now let's get the name of the layer, to use later

                                            Entity ent = (Entity)obj;
                                            LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(ent.LayerId, OpenMode.ForRead);
                                            string layName = ltr.Name;

                                            // Clone the selected object

                                            object o = ent.Clone();
                                            Entity clone = o as Entity;

                                            // We need to manipulate the clone to make sure it works

                                            if (clone != null)
                                            {
                                                // Setting the properties from the block reference helps certain entities get the
                                                // right references (and allows them to be offset properly)
                                                clone.SetPropertiesFrom(br);

                                                // But we then need to get the layer information from the database to set the
                                                // right layer (at least) on the new entity

                                                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                                                if (lt.Has(layName))
                                                    clone.LayerId = lt[layName];

                                                // Now we need to transform the entity for each of its Xref block reference containers
                                                // If we don't do this then entities in nested Xrefs may end up in the wrong place

                                                ObjectId[] conts = pner.GetContainers();
                                                foreach (ObjectId contId in conts)
                                                {
                                                    BlockReference cont = tr.GetObject(contId, OpenMode.ForRead) as BlockReference;
                                                    if (cont != null)
                                                        clone.TransformBy(cont.BlockTransform);
                                                }

                                                // Let's add the cloned entity to the current space

                                                BlockTableRecord space = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                                                if (space == null)
                                                {
                                                    clone.Dispose();
                                                    return;
                                                }

                                                ObjectId cloneId = space.AppendEntity(clone);
                                                tr.AddNewlyCreatedDBObject(clone, true);

                                                // Now let's flush the graphics, to help our clone get displayed

                                                tr.TransactionManager.QueueForGraphicsFlush();

                                                // And we add our cloned entity to the list for deletion

                                                _ids.Add(cloneId);

                                                // Created a non-graphical selection of our newly created object and replace it with
                                                // the selection of the container Xref

                                                IntPtr ip = (IntPtr)0;
                                                SelectedObject so = new SelectedObject(cloneId, SelectionMethod.NonGraphical, ip);  //?????????????????????????????????
                                                e.ReplaceSelectedObject(so);
                                            }
                                        }
                                        catch (System.Exception ex)
                                        {
                BaseObjs.writeDebug(ex.Message + " xRefOffsetApp.cs: line: 259");
                                        }
                                    }
                                }
                            }
                        }
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " xRefOffsetApp.cs: line: 270");
                }
            }
        }
    }
}
