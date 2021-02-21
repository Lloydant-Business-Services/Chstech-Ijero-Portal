using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Model.Model
{
    public class Remita
    {
        public string  merchantId {get;set;}
        public string  serviceTypeId {get;set;}
        public decimal  totalAmount {get;set;}
        public string  hash {get;set;}
        public string  payerName {get;set;}
        public string  payerEmail {get;set;}
        public string  payerPhone { get;set;}
        public string  responseurl { get;set;}
        public List<RemitaSplitItems> lineItems { get; set; }
        public string  orderId { get;set;}
        public string paymenttype { get; set; }
       
  }
}