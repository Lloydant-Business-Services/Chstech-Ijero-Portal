using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.ComponentModel.DataAnnotations;


namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class PostjambResultSupportViewModel
    {
        public Abundance_Nk.Model.Model.PutmeResult putmeResult { get; set; }
        public List<PutmeResult> AllResults { get; set; }
        public ApplicantJambDetail jambDetail { get; set; }
        public ApplicationForm ApplicationDetail { get; set; }

        [Display(Name = "Jamb Number")]
        public string JambNumber { get; set; }

        public string ExamNumber { get; set; }
        public List<PutmeResult> PutmeResults { get; set; }
    }
}