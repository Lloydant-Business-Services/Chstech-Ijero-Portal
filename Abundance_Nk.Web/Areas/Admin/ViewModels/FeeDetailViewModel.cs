using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class FeeDetailViewModel
    {
        private FeeTypeLogic feeTypeLogic;
        private ProgrammeLogic programmeLogic;
        private LevelLogic levelLogic;
        private PaymentModeLogic paymentModeLogic;
        private DepartmentLogic departmentLogic;
        private SessionLogic sessionLogic;
        private FeeDetailLogic feeDetailLogic;

        public FeeDetailViewModel()
        {
            feeSelectListItem = Utility.PopulateFeeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();
            SchoolFeeAcceptanceFeeTypeSelectListItem = Utility.PopulateSchoolFeeAcceptanceFeeTypeSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            PaymentModeSelectListItem = Utility.PopulatePaymentModeSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            if (Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);

            }
        }

        public Fee fee { get; set; }
        public FeeDetail feeDetail { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session CurrentSession { get; set; }
        public Level level { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public FeeType feeType { get; set; }
        public List<FeeDetail> feeDetails { get; set; }
        public List<SelectListItem> feeSelectListItem { get; set; }
        public List<SelectListItem> SchoolFeeAcceptanceFeeTypeSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<DepartmentalSchoolFees> DepartmentalSchoolFeeList { get; set; }

        public void GetFeeDetails()
        {
            try
            {
                feeDetailLogic = new FeeDetailLogic();
                feeDetails = feeDetailLogic.GetAll();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int SN { get; set; }

        public string Id { get; set; }
        public PaymentTerminal PaymentTerminal { get; set; }
        public PaymentEtranzactType PaymentEtranzactType { get; set; }
        public string UploadedFilePath { get; set; }
        public List<PaymentUploadModel> Payments { get; set; }
        public List<PaymentUploadModel> SuccessfulPayments { get; set; }
        public List<FeeSetup> FeeSetups { get; set; }
        public FeeSetup FeeSetup { get; set; }
    }

    public partial class ArrayJsonView
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string FeeType { get; set; }
        public string Fee { get; set; }
        public string Session { get; set; }
        public string PaymentMode { get; set; }
        public string TerminalId { get; set; }
        public string EtranzactTypeName { get; set; }
        public string FeeName { get; set; }
        public string Amount { get; set; }
        public string FeeDescription { get; set; }
        public string FeeTypeName { get; set; }
        public string FeeTypeDescription { get; set; }
        public string EnteredBy { get; set; }
        public string PersonType { get; set; }
        public string PaymentType { get; set; }
        public string Name { get; set; }
    }

    public class PaymentUploadModel
    {
        public string PaymentDate { get; set; }
        public string PaymentRefNumber { get; set; }
        public string MerchantReference { get; set; }
        public string MerchantId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentItem { get; set; }
        public string Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Bank { get; set; }
    }
}