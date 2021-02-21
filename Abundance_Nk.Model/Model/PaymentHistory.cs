using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentHistory
    {
        public Student Student { get; set; }
        public List<PaymentView> Payments { get; set; }
        public List<Payment> PaymentLists { get; set; }
    }
}
