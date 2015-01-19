using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Base_Tools45;
namespace Grading
{
    public class Grading_BetterListBox : ListBox {

        public delegate void BetterListBoxScrollDelegate(object Sender, BetterListBoxScrollArgs e);
        public event BetterListBoxScrollDelegate Scroll;

        private const int WM_VSCROLL = 0x0115;
        private const int SB_THUMBTRACK = 5;
        private const int SB_ENDSCROLL = 8;

        protected override void WndProc(ref Message m){
            base.WndProc(ref m);
            if (m.Msg == WM_VSCROLL){
                int nfy = m.WParam.ToInt32() & 0xFFF;
                if(Scroll != null && (nfy == SB_THUMBTRACK || nfy == SB_ENDSCROLL))
                    Scroll(this, new BetterListBoxScrollArgs(this.TopIndex, nfy == SB_THUMBTRACK));
            }
        }

        public class BetterListBoxScrollArgs{
            private int mTop;
            private bool mTracking;
            public BetterListBoxScrollArgs(int top, bool tracking){
                mTop = top;
                mTracking = tracking;
            }
            public int Top{
                get {return mTop;}
            }
            public bool Tracking{
                get{return mTracking;}
            }
        }    
        //handler
        private void BetterListBox1_Scroll(object Sender, Grading_BetterListBox.BetterListBoxScrollArgs e){
            Console.WriteLine("Scroll to {0}, tracking={1}", e.Top, e.Tracking);
        }
    }



}
