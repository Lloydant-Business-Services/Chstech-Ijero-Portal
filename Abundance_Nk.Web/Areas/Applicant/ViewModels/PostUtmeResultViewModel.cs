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
    public class PostUtmeResultViewModel
    {
        public PutmeResult Result { get; set; }
        public ApplicantJambDetail jambDetail { get; set; }
        public ApplicationForm ApplicationDetail { get; set; }
        public Programme Programme { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> ResultSelectListItem { get; set; }
        [Required(ErrorMessage = "Please Enter your Examination Number")]
        public string JambRegistrationNumber { get; set; }        
        [Required(ErrorMessage = "Please Enter your Etranzact Confirmation Number")]
        public string PinNumber { get; set; }
        public bool ValidatePin()
        {
            return true;
        }
        public PostUtmeResultViewModel()
        {
            Result = new PutmeResult();
            Programme = new Programme();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
        }
    }
}