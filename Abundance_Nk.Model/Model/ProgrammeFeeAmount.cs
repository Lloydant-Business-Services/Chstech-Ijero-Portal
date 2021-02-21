using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ProgrammeFeeAmount
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public Session Session { get; set; }
        public decimal Amount { get; set; }
    }
}
