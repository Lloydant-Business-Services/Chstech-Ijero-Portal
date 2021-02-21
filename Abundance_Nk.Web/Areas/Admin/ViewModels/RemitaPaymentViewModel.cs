using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class RemitaPaymentViewModel
    {
        public RemitaPaymentViewModel()
        {
            remitaPaymentList = new List<RemitaPayment>();
            remitaList = new List<Remita>();
            remitaResponseList = new List<RemitaResponse>();
        }
        public RemitaPayment remitaPayment { get; set; }
        public Remita remita { get; set; }
        public RemitaResponse remitaResponse { get; set; }
        public List<RemitaPayment> remitaPaymentList { get; set; }
        public List<Remita> remitaList { get; set; }
        public List<RemitaResponse> remitaResponseList { get; set; }
        public string order_Id { get; set; }
        public string url { get; set; }
        public string message { get; set; }
        public string rrr { get; set; }
        public string statuscode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}