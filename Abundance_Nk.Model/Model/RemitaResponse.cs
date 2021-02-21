using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class RemitaResponse
    {
       public string orderId { get; set; }
        [Display(Name="RRR")]
       public string RRR {get;set;}
       public string Status{get;set;}
       public string StatusCode { get; set; }
       public string Message { get; set; }
       public Remita RemitaDetails { get; set; }
       public string channnel { get; set; }
       public string bank { get;set;}
       public string branch { get;set;}
       public string serviceTypeID { get;set;}
       public decimal amount { get; set; }
       public string orderRef { get; set; }
       public string responseCode { get; set; }
       public string paymentDate { get; set; }
    }
}