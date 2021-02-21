using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ManualPaymentViewModel
    {
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

    }

    
}