using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class AdmissionProcessingViewModel
    {
        private ApplicantLogic applicantLogic;
        private ApplicationFormLogic applicationFormLogic;

        public AdmissionProcessingViewModel()
        {
            Session = new Session();
            applicantLogic = new ApplicantLogic();
            applicationFormLogic = new ApplicationFormLogic();
            ApplicationForms = new List<ApplicationForm>();

            SessionSelectList = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();

        }

        public Level Level { get; set; }
        public Session Session { get; set; }
        public List<Model.Model.Applicant> Applicants { set; get; }
        public List<StudentLevel> StudentLevel { set; get; }
        public List<ApplicationForm> ApplicationForms { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        //public List<ApplicationSummaryReport> ApplicationSummaryReport { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<AdmissionList> ListOfAdmission { get; set; }
        public List<ApplicationForm> GetApplicationsBy(bool rejected, Session session)
        {
            try
            {
                ApplicationForms = applicationFormLogic.GetModelsBy(af => af.Rejected == rejected && af.APPLICATION_FORM_SETTING.Session_Id == session.Id);
                return ApplicationForms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetApplicantByStatus(ApplicantStatus.Status status)
        {
            try
            {
                Applicants = applicantLogic.GetModelsBy(a => a.Applicant_Status_Id == (int)status);
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        

        



    }




}