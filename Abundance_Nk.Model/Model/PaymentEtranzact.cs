using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class PaymentEtranzact
    {
        public OnlinePayment Payment { get; set; }
        public PaymentTerminal Terminal { get; set; }
        public PaymentEtranzactType EtranzactType { get; set; }
        public string ReceiptNo { get; set; }
        public string PaymentCode { get; set; }
        public string MerchantCode { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string ConfirmationNo { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public bool Used { get; set; }
        public Int64 UsedBy { get; set; }
        //public int SessionId { get; set; }
    }



}
