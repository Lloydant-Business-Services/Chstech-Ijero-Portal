using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class StudentViewModel
    {
        public Model.Model.Student Student { get; set; }
        [Display(Name="Old Password")]
        public string OldPassword { get; set; }
        [Display(Name="New Password")]
        public string NewPassword { get; set; }
        [Display(Name="New Password Confirmation")]
        public string ConfirmPassword { get; set; }

    }
}