using System;

using acadDb = Autodesk.AutoCAD.DatabaseServices;

namespace Base_Tools.C3d.Application {

    /// <summary>
    /// Encapsulates a Transaction to eliminate dependencies on
    /// AutoCAD specific code.
    /// </summary>
    public class Transaction : IDisposable
    {
        /// <summary>
        /// Class constructor that initializes the transaction object.
        /// </summary>
        /// <param name="transaction">Parent document for the transaction.</param>
        internal Transaction(Document parent)
        {
            m_ParentDocument = parent;
            m_TheTransaction = m_ParentDocument._acadDoc.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Aborts a currently active transaction.
        /// </summary>
        public void Abort()
        {
            m_TheTransaction.Abort();
        }

        /// <summary>
        /// Commits changes made to the document during the transaction scope.
        /// </summary>
        public void Commit()
        {
            m_TheTransaction.Commit();
        }

        /// <summary>
        /// Disposes the transaction object and closes the transaction.
        /// </summary>
        public void Dispose()
        {
            m_ParentDocument._closeTransaction();
            m_TheTransaction.Dispose();
        }

        public Object GetObject(acadDb.ObjectId idObj, acadDb.OpenMode mode)
        {
            return (Object)m_TheTransaction.GetObject(idObj, mode);
        }

        public void AddNewlyCreatedDBOject(acadDb.DBObject dbObj, bool add)
        {
            m_TheTransaction.AddNewlyCreatedDBObject(dbObj, add);
        }

        private Document m_ParentDocument;
        private acadDb.Transaction m_TheTransaction;
    }
}
