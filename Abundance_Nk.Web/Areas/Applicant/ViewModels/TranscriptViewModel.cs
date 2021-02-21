using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class TranscriptViewModel
    {
        public TranscriptViewModel()
        {
            StateSelectList = Utility.PopulateStateSelectListItem();
            CountrySelectList = Utility.PopulateCountrySelectListItem();
            FeesTypeSelectList = Utility.PopulateVerificationFeeTypeSelectListItem();
            transcriptRequest = new TranscriptRequest();
            StudentVerification = new StudentVerification();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();

            StateSelectListItem = Utility.PopulateStateSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();
            SessionSelectListItem = Utility.PopulateSingleSessionSelectListItem(7);
            AllSessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            DeliveryServiceSelectList = Utility.PopulateDeliveryServiceSelectListItem();
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();

        }
        public List<SelectListItem> FeesTypeSelectList { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public List<TranscriptRequest> TranscriptRequests { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public StudentVerification StudentVerification { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public bool Paymentstatus { get; set; }
        public RemitaPayment RemitaPayment { get; set; }
        public string Reciever { get; set; }

        public RemitaPayementProcessor RemitaPayementProcessor { get; set; }
        public string  RemitaBaseUrl { get; set; }
        public string  MatricNumber { get; set; }
        public string Hash { get; set; }
        public Fee Fee { get; set; }
        public FeeType FeeType { get; set; }
        public string RequestType { get; set; }
        public Model.Model.Student Student { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public Session Session { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Person Person { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool StudentAlreadyExist { get; set; }
        public bool ShowInvoicePage { get; set; }
        public List<SelectListItem> StateSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> AllSessionSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<SelectListItem> DeliveryServiceSelectList { get; set; }
        public DeliveryService DeliveryService { get; set; }
        public DeliveryServiceZone DeliveryServiceZone { get; set; }
    }
}