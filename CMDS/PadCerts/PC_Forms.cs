using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadCerts
{
    public sealed class PC_Forms
    {
        private static readonly PC_Forms pc_Forms = new PC_Forms();

        public static PC_Forms pcForm{
            get{
                return pc_Forms;
            }
        }

        private PC_Forms(){
            wPadCerts = new winPadCerts();
        }

        public winPadCerts wPadCerts { get; set; }
    }
}
