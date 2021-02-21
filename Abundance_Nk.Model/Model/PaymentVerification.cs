using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentVerification
    {
        public Payment Payment { get; set; }
        public User User { get; set; }
        public StudentPayment StudentPayment { get; set; }
        public Department Department { get; set; }
        public DateTime DateVerified { get; set; }
        public string Comment { get; set; }
    }
}
