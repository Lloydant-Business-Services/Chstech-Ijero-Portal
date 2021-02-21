using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentTerminal
    {
        public long Id { get; set; }
        public string TerminalId { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
    }


}
