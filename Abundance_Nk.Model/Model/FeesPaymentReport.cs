using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class FeesPaymentReport 
    {
        public string Department { get; set; }
        public string Session { get; set; }
        public string Programme { get; set; }
        public string Level { get; set; }
        public string MatricNumber { get; set; }
        public string ApplicationNumber { get; set; }
        public string AcceptanceFeeInvoiceNumber { get; set; }
        public string FirstYearSchoolFeesInvoiceNumber { get; set; }
        public string SecondYearSchoolFeesInvoiceNumber { get; set; }
        public string Name { get; set; }
        public string ApplicationFormAmount { get; set; }
        public string ApplicationFormInvoiceNumber { get; set; }
        public string AcceptanceTransactionAmount { get; set; }
        public string FirstYearFeesTransactionAmount { get; set; }
        public string SecondYearFeesTransactionAmount { get; set; }
    }
}
