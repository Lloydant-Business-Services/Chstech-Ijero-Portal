using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class OnlinePayment
    {
        public Payment Payment { get; set; }
        public PaymentChannel Channel { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime? TransactionDate { get; set; }

        

    }



}
