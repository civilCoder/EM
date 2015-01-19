using System.Diagnostics;

namespace Base_Tools45
{
    public class Debugg
    {
        public DefaultTraceListener
        getListner()
        {
            Trace.Listeners.RemoveAt(0);
            DefaultTraceListener defListener = new DefaultTraceListener();
            Trace.Listeners.Add(defListener);
            return defListener;
        }
    }
}
