using System;
using System.Windows.Forms;

namespace Base_Tools45
{
    public class BetterListBox : ListBox
    {
        private const int WM_VSCROLL = 0x0115;
        private const int SB_THUMBtrACK = 5;
        private const int SB_ENDSCROLL = 8;

        public delegate void BetterListBoxScrollDelegate(object Sender, BetterListBoxScrollArgs e);

        public event BetterListBoxScrollDelegate Scroll;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_VSCROLL)
            {
                int nfy = m.WParam.ToInt32() & 0xFFF;
                if (Scroll != null && (nfy == SB_THUMBtrACK || nfy == SB_ENDSCROLL))
                    Scroll(this, new BetterListBoxScrollArgs(this.TopIndex, nfy == SB_THUMBtrACK));
            }
        }

        //handler
        private void BetterListBox1_Scroll(object Sender, BetterListBox.BetterListBoxScrollArgs e)
        {
            Console.WriteLine("Scroll to {0}, tracking={1}", e.Top, e.Tracking);
        }

        public class BetterListBoxScrollArgs : EventArgs
        {
            private int mTop;
            private bool mTracking;

            public BetterListBoxScrollArgs(int top, bool tracking)
            {
                mTop = top;
                mTracking = tracking;
            }

            public int Top
            {
                get
                {
                    return mTop;
                }
            }

            public bool Tracking
            {
                get
                {
                    return mTracking;
                }
            }
        }
    }
}
