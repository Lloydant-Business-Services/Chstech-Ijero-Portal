using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class EWalletViewModel
    {
        public EWalletViewModel()
        {
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateActiveFeeTypeSelectListItem();
            SessionSelectListItem = Utility.PopulateSchoolFeesSessionSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem();
        }
        public List<EWalletPayment> EWalletPayments { get; set; }

        public FeeType FeeType { get; set; }

        public PaymentType PaymentType { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public EWalletPayment EWalletPayment { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Person Person { get; set; }
        public Session Session { get; set; }
        public RemitaPayment RemitaPayment { get; set; }
        public string Hash { get; set; }

        public string ResponseUrl { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public AdmissionList AdmissionList { get; set; }
    }
}