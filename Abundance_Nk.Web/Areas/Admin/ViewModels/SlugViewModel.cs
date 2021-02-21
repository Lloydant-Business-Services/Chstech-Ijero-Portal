using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class SlugViewModel
    {
        public SlugViewModel()
        {
            ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            if (Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
        }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public AppliedCourse appliedCourse { get; set; }
        public List<Slug> applicantDetails { get; set; }

       
    }
}