using System.Collections.Generic;

namespace EventManager
{
    public class EM_DocList
    {
        private static readonly EM_DocList _emDocList = new EM_DocList();

        private List<string> _docList;

        private EM_DocList()
        {
            docList = new List<string>();
        }

        public static EM_DocList emDockList
        {
            get
            {
                return _emDocList;
            }
        }

        public List<string> docList
        {
            get
            {
                return _docList;
            }
            set
            {
                _docList = value;
            }
        }
    }
}
