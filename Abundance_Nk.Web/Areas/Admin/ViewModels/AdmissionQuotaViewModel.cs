using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class AdmissionQuotaViewModel
    {
        public AdmissionQuotaViewModel()
        {

            SessionSelectList = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();

        }

        public List<AdmissionQuota> AdmissionQuota { get; set; }
        public AdmissionQuota AdmissionQuotaEdit { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public User User { get; set; }
        public long Quota { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }

    }
}