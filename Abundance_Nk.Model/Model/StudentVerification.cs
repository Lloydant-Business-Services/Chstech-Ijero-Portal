using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentVerification
    {
        public Student Student { get; set; }
        public Payment Payment { get; set; }
        public RemitaPayment RemitaPayment { get; set; }
        public Remita Remita { get; set; }
        public FeeType FeeType { get; set; }
        public decimal Amount { get; set; }
    }
}
