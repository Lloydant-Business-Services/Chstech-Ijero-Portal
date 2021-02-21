using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentChannel : BasicSetup
    {
        public bool Status { get; set; }

        public enum Channels
        {
            Etranzact = 1
        }
    }



}
