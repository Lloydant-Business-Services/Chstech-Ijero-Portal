using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
   public class RemitaPayment
    {
       public Payment payment { get; set; }
       [Display(Name="Remita Retrieval Reference")]
       public string RRR { get; set; }
       [Display(Name = "Reference / Order Id")]
       public string OrderId { get; set; }
       [Display(Name = "Transaction Status")]
       public string Status { get; set; }
       public string Receipt_No { get; set; }
       public string Description { get; set; }
       public string MerchantCode { get; set; }
       [Display(Name = "Transaction Amount")]
       public decimal TransactionAmount { get; set; }
       [Display(Name = "Transaction Date")]
       public DateTime TransactionDate { get; set; }
       public string ConfirmationNo { get; set; }
       public string BankCode { get; set; }
       public string CustomerId { get; set; }
       public string CustomerName { get; set; }

    }
}
