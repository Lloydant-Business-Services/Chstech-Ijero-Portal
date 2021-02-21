
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Payment
    {
        public long Id { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public PaymentType PaymentType { get; set; }
        public PersonType PersonType { get; set; }
        public FeeType FeeType { get; set; }
        public DateTime DatePaid { get; set; }
        public Person Person { get; set; }
        public long? SerialNumber { get; set; }

        public Session Session { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        public List<FeeDetail> FeeDetails { get; set; }
        public string Amount { get; set; }
        public string ConfirmationNumber { get; set; }
    }



}
