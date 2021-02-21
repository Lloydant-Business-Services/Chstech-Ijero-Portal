using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EWalletPayment
    {
        public long Id { get; set; }
        public Payment Payment { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public Student Student { get; set; }
        public decimal Amount { get; set; }
        public bool PaymentStatus { get; set; }
        public string RRR { get; set; }
        public Person Person { get; set; }
    }
}
