using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Invoice
    {
        public Person Person { get; set; }
        public Payment Payment { get; set; }
        public string JambRegistrationNumber { get; set; }
        public string MatricNumber { get; set; }
        public bool Paid { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public PaymentEtranzactType paymentEtranzactType { get; set; }
        public PaymentScholarship paymentScholarship { get; set; }
        public decimal Amount { get; set; }
        public Department Department { get; set; }
        public string barcodeImageUrl { get; set; }

    }



}
