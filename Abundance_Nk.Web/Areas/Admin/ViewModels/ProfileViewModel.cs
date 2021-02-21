using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            staffDetail = new Staff();
            staffDetail.State = new State();
            StateSelectList = Utility.PopulateStateSelectListItem();
            MaritalStatusSelectList = Utility.PopulateMaritalStatusSelectListItem();
            ReligionSelectList = Utility.PopulateReligionSelectListItem();
            //DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
        }
        public Staff staffDetail { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> MaritalStatusSelectList { get; set; }
        public List<SelectListItem> ReligionSelectList { get; set; }
        //public List<SelectListItem> DepartmentSelectList { get; set; }
    }
}