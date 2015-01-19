using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace Base_Tools.C3d.Application {

    public class ObjectNodeProvider
    {
        private Document m_OwnerDocument;

        internal ObjectNodeProvider(Document owner)
        {
            m_OwnerDocument = owner;
        }

        public ObjectIdCollection GetNode(Type objectType)
        {
            return getNodeForType(objectType);
        }

        private ObjectIdCollection getNodeForType(Type objectType)
        {
            if (objectType == typeof(Alignment))
            {
                return _civildoc.GetAlignmentIds();
            }
            else
            {
                throw new NotImplementedException("Object type not registered");
            }
        }

        private CivilDocument _civildoc
        {
            get
            {
                return m_OwnerDocument._civDoc;
            }
        }
    }
}
