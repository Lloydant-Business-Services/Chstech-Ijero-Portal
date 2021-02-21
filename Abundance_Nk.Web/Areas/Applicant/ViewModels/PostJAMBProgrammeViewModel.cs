
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostJAMBProgrammeViewModel
    {
        [Required]
        [Display(Name = "Confirmation Order Number")]
        public string ConfirmationOrderNumber { get; set; }



    }

}