using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class RegistrationBalanceReport
    {
        public string Department { get; set; }
        public string Programme { get; set; }
        public string ProgrammePayment { get; set; }
        public string ProgrammeRegistration { get; set; }
        public string Payment { get; set; }
        public string Registration { get; set; }
        public int PaymentNumber { get; set; }
        public int RegistrationNumber { get; set; }
    }
}
