using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class AdmissionProposalViewModel
    {
        public AdmissionProposalViewModel()
        {
           
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();

        }
        public List<Model.Model.Applicant> Applicants { set; get; }
        public List<OLevelResultDetail> ApplicantOLevel { get; set; }
        public List<ApplicationForm> ApplicationForms { get; set; }
        public List<AppliedCourse> ApplicantAppliedCourse { get; set; }
        public List<ProposeAdmission> ProposeAdmission { get; set; }
        public List<ApplicantJambDetail> ApplicantJambDetail { get; set; }
        public decimal? AggregateScore { get; set; }
        public List<ApplicantResult> ApplicantResult { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public Session Session { get; set; }



    }
}