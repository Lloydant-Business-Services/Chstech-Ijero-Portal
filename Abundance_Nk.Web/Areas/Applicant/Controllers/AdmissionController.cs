using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Configuration;
using Abundance_Nk.Model.Entity.Model;
using static Abundance_Nk.Web.Areas.Admin.Views.SupportController;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class AdmissionController : BaseController
    {
        private AdmissionViewModel viewModel;

        public AdmissionController()
        {
            viewModel = new AdmissionViewModel();
        }

        public ActionResult RecieptIndex()
        {
            return View();
        }

        public JsonResult RecieptGeneration(string vInvoiceVumber)
        {
            JsonResultView result = new JsonResultView();

            try
            {
                RemitaPayment remitaPayment = new RemitaPayment();
                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                payment = paymentLogic.GetModelsBy(x => x.Invoice_Number == vInvoiceVumber).LastOrDefault();
                remitaPayment = remitaPaymentLogic.GetModelsBy(x => x.Payment_Id == payment.Id).LastOrDefault();
            }

             catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


     

       

        public ActionResult CheckStatus()
        {
            return View(viewModel);
        }

       

        [HttpPost]
        public ActionResult CheckStatus(AdmissionViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    //bool signIn = applicationFormLogic.IsValidApplicationNumberAndEtranzactPin(vModel.ApplicationForm.Number, vModel.ScratchCard.Pin);

                    bool signIn = true;

                    if (signIn)
                    {
                        ApplicationForm form = viewModel.GetApplicationFormBy(vModel.ApplicationForm.Number);
                        TempData["AppNumber"] = form.Number;
                        

                        if (form != null)
                        {
                            return RedirectToAction("Index", new { fid = Abundance_Nk.Web.Models.Utility.Encrypt(form.Id.ToString()) });
                        }
                        else
                        {
                            SetMessage("Application cannot be found, kindly check and try again.", Message.Category.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(vModel);
        }

        public ActionResult Index(string fid)
        {
            try
            {
                TempData["FormViewModel"] = null;
                Int64 formId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(fid));
                viewModel.GetApplicationBy(formId);


                PopulateAllDropDowns(viewModel);
                viewModel.GetOLevelResultBy(formId);
                EWalletViewModel eWalletViewModel = new EWalletViewModel();
                eWalletViewModel.ApplicationForm = viewModel.GetApplicationFormBy(formId);
                SetSelectedSittingSubjectAndGrade(viewModel);

                //Update Remita Payments
                UpdateStudentRRRPayments(viewModel.ApplicationForm.Person);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View("IndexAlt", viewModel);
        }

        private void PopulateAllDropDowns(AdmissionViewModel existingViewModel)
        {
            //AdmissionViewModel existingViewModel = (AdmissionViewModel)TempData["viewModel"];
            //PostJAMBFormPaymentViewModel postJAMBFormPaymentViewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];

            try
            {
                if (existingViewModel.Loaded)
                {
                    if (existingViewModel.FirstSittingOLevelResult == null) { existingViewModel.FirstSittingOLevelResult = new OLevelResult(); }
                    if (existingViewModel.SecondSittingOLevelResult == null) { existingViewModel.SecondSittingOLevelResult = new OLevelResult(); }

                    if (existingViewModel.FirstSittingOLevelResult.Type == null) { existingViewModel.FirstSittingOLevelResult.Type = new OLevelType(); }

                    ViewBag.FirstSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, 0);
                    }

                    SetSelectedSittingSubjectAndGrade(existingViewModel);

                    List<SelectListItem> modes = new List<SelectListItem>();
                    modes = Utility.PopulatePaymentModeSelectListItem();
                    if (existingViewModel.admissionList != null && (existingViewModel.admissionList.Programme.Id == (int)Programmes.HNDPartTime || existingViewModel.admissionList.Programme.Id == (int)Programmes.NDPartTime))
                    {
                        modes = modes.Where(m => m.Value == "1" || m.Value == "2").ToList();
                    }
                    else
                    {
                        modes = modes.Where(m => m.Value == "1").ToList();
                    }
                    
                    ViewBag.PaymentModes = modes;
                }
                else
                {
                    viewModel = new AdmissionViewModel();

                    ViewBag.FirstSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYearId = viewModel.ExamYearSelectList;

                    //SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult GenerateAcceptanceInvoice(string applicationFormNo)
        {
            Payment payment = null;
            Decimal Amt = 0;
            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(applicationFormNo);
                if (form != null && form.Id > 0)
                {

                    AdmissionList list = new AdmissionList();
                    AdmissionListLogic listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(form.Id);
                    PaymentLogic paymentLogic = new PaymentLogic();

                    AppliedCourse appliedCourse = new AppliedCourse();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(form.Person);



                    if (form != null && form.Id > 0)
                    {
                        //var doesInvoiceNumberExists = paymentLogic.GetModelBy(f => f.Person_Id == form.Person.Id && f.Payment_Id == form.Payment.Id);
                        //if(doesInvoiceNumberExists != null && doesInvoiceNumberExists.InvoiceNumber != null)
                        //{
                        //    viewModel.AcceptanceInvoiceNumber = doesInvoiceNumberExists.InvoiceNumber;
                        //    return Json(new { InvoiceNumber = doesInvoiceNumberExists.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                        //}

                        FeeType feeType = new FeeType() { Id = (int)FeeTypes.AcceptanceFee };
                        if (form.ProgrammeFee.Programme.Id > 1)
                        {
                            //feeType = new FeeType() { Id = 9 };
                        }


                        ApplicantStatus.Status status = ApplicantStatus.Status.GeneratedAcceptanceInvoice;
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                        {
                            payment = GenerateInvoiceHelper(form, feeType, status);
                            if (payment != null)
                            {
                                Amt = payment.FeeDetails.Sum(a => a.Fee.Amount);

                                if (Amt <= 0M)
                                {
                                    SetMessage("Amount not set! ", Message.Category.Error);
                                    return Json(new { }, "text/html", JsonRequestBehavior.AllowGet);
                                }

                                //GENERATE RRR
                                RemitaPayment remitaPayment = ProcessRemitaPayment(payment);
                                if (remitaPayment != null && !string.IsNullOrEmpty(remitaPayment.RRR))
                                {
                                    TempData["Acceptance_Invoice"] = remitaPayment.CustomerId;
                                    var acc_inv = TempData["Acceptance_Invoice"];
                                    transaction.Complete();
                                }
                                //transaction.Complete();

                                TempData.Keep("PaymentViewModel");
                                TempData.Keep("Acceptance_Invoice");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return Json(new { }, "text/html", JsonRequestBehavior.AllowGet);

            }

            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }
        public RemitaPayment ProcessRemitaPayment(Payment payment)
        {
            try
            {
                if (payment != null)
                {
                    decimal Amt = 0;
                    Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);

                    //Get Payment Specific Setting
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetBy(3);

                    //Get Split Specific details;
                    List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                    RemitaSplitItems singleItem = new RemitaSplitItems();
                    RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                    singleItem = splitItemLogic.GetBy(8);
                    singleItem.deductFeeFrom = "1";
                    splitItems.Add(singleItem);
                    singleItem = splitItemLogic.GetBy(9);
                    singleItem.deductFeeFrom = "0";
                    splitItems.Add(singleItem);

                    //Get BaseURL
                    string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                    RemitaPayment remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "ACCEPTANCE FEE", splitItems, settings, Amt);

                    string Hash = GenerateHash(settings.Api_key, remitaPayment);

                    Student.ViewModels.PaymentViewModel paymentViewModel = new Student.ViewModels.PaymentViewModel();
                    paymentViewModel.RemitaPayment = remitaPayment;
                    paymentViewModel.Hash = Hash;

                    TempData["PaymentViewModel"] = paymentViewModel;

                    return remitaPayment;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return null;
        }
        private string GenerateHash(string apiKey, RemitaPayment remitaPayment)
        {
            string hashConcatenate = null;
            try
            {
                if (remitaPayment != null)
                {
                    string hash = remitaPayment.MerchantCode + remitaPayment.RRR + apiKey;
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(hash);
                    hashConcatenate = remitaProcessor.HashPaymentDetailToSHA512(hash);
                }

                return hashConcatenate;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult GenerateChangeOfCourseInvoice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateChangeOfCourseInvoice(AdmissionViewModel viewModel)
        {
            string applictionNumber = viewModel.ApplicationForm.Number;
            Payment payment = null;
            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(applictionNumber);
                if (form != null && form.Id > 0)
                {
                    if (form != null && form.Id > 0)
                    {
                        FeeType feeType = new FeeType() { Id = (int)FeeTypes.ChangeOfCourseFees };
                        ApplicantStatus.Status status = ApplicantStatus.Status.SubmittedApplicationForm;
                        payment = GenerateInvoiceHelper(form, feeType, status);
                        if (payment == null)
                        {
                            SetMessage("An Error Occurred while generating invoice. Please try again! ", Message.Category.Error);
                            return View();
                        }
                    }
                }
                else
                {
                    SetMessage("Error! Application Number not found", Message.Category.Error);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return View();
            }

            return RedirectToAction("Invoice", new { ivn = payment.InvoiceNumber });
        }

        public ActionResult PayChangeOfCourseFee()
        {
            try
            {
                viewModel = new AdmissionViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult PayChangeOfCourseFee(AdmissionViewModel viewModel)
        {
            try
            {
                if (viewModel.ConfirmationOrderNumber != null)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();

                    if (viewModel.ConfirmationOrderNumber.Length > 12)
                    {
                        Model.Model.Session session = new Model.Model.Session() { Id = 7 };
                        FeeType feetype = new FeeType() { Id = (int)FeeTypes.ChangeOfCourseFees };
                        Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber, feetype.Id);
                        if (payment != null && payment.Id > 0)
                        {
                            if (payment.FeeType.Id != (int)FeeTypes.ChangeOfCourseFees)
                            {
                                SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrderNumber + ") entered is not for Change of Course Fee payment! Please enter your Change of Course Fee Confirmation Order Number.", Message.Category.Error);
                                return View(viewModel);
                            }

                            SetMessage("Your Change Of Course Fee payment has been successfully confirmed, you can now proceed with registration using the Check Admission Status link", Message.Category.Information);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult MakePayment(string PaymentId, string formId)
        {
            try
            {
                Int64 fid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(formId));
                string invoice = Abundance_Nk.Web.Models.Utility.Decrypt(PaymentId);
                RemitaPayment remitaPyament = new RemitaPayment();
                RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                Payment payment = new Payment();
                PaymentLogic pL = new PaymentLogic();
                payment = pL.GetModelBy(p => p.Invoice_Number == invoice);

                decimal Amount = pL.GetPaymentAmount(payment);

                List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                splitItems = splitItemLogic.GetAll();
                splitItems[0].deductFeeFrom = "0";
                if (splitItems.Count > 1)
                {
                    splitItems[1].deductFeeFrom = "1";
                }
                RemitaSplitItems its = new RemitaSplitItems();
                its = splitItemLogic.GetModelBy(a => a.Id == 1);
                its.deductFeeFrom = "0";

                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();

                RemitaSettings settings = new RemitaSettings();
                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);

                long milliseconds = DateTime.Now.Ticks;
                RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                Remita remita = new Remita()
                {

                    merchantId = settings.MarchantId,
                    serviceTypeId = settings.serviceTypeId,
                    orderId = payment.InvoiceNumber,
                    totalAmount = Amount,
                    payerName = payment.Person.FullName,
                    payerEmail = "support@lloydant.com",
                    payerPhone = payment.Person.MobilePhone,
                    responseurl = settings.Response_Url,
                    lineItems = splitItems,


                };

                RemitaResponse remitaResponse = remitaPayementProcessor.PostJsonDataToUrl(remitaBaseUrl, remita, payment);
                if (remitaResponse.Status != null && remitaResponse.StatusCode.Equals("025"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament.payment = payment;
                    remitaPyament.RRR = remitaResponse.RRR;
                    remitaPyament.OrderId = remitaResponse.orderId;
                    remitaPyament.Status = remitaResponse.StatusCode + ":" + remitaResponse.Status;
                    remitaPyament.TransactionAmount = remita.totalAmount;
                    remitaPyament.TransactionDate = DateTime.Now;
                    remitaPyament.MerchantCode = remita.merchantId;
                    remitaPyament.Description = "ACCEPTANCE FEES";
                    if (remitaLogic.GetBy(payment.Id) == null)
                    {
                        remitaLogic.Create(remitaPyament);
                    }
                    remita.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(remita.merchantId + remitaResponse.RRR + settings.Api_key);
                    viewModel.ApplicationForm.Id = fid;
                    viewModel.remita = remita;
                    viewModel.remitaResponse = remitaResponse;
                    return View(viewModel);

                }
                else if (remitaResponse.StatusCode.Trim().Equals("028"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament = remitaLogic.GetModelBy(r => r.OrderId == payment.InvoiceNumber);
                    if (remitaPyament != null)
                    {
                        viewModel.ApplicationForm.Id = fid;
                        remitaResponse.RRR = remitaPyament.RRR;
                        remitaResponse.orderId = remitaPyament.OrderId;
                        remita.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(remita.merchantId + remitaResponse.RRR + settings.Api_key);
                        viewModel.remita = remita;
                        viewModel.remitaResponse = remitaResponse;
                        return View(viewModel);
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred:" + ex.Message, Message.Category.Error);
                return View(viewModel);
            }

        }
        [HttpPost]
        public ActionResult GenerateSchoolFeesInvoice(string formNo, int paymentModeId)
        {
            Payment payment = null;
            Decimal Amt = 0;

            PaymentMode paymentMode = new PaymentMode() { Id = paymentModeId };
            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(formNo);
                if (form != null && form.Id > 0)
                {
                    //Check for Acceptance
                    bool hasPaidAcceptance = CheckAcceptancePayment(form);
                    if (!hasPaidAcceptance)
                    {
                        return Json(new { InvoiceNumber = "No Acceptance" }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                    //check for e-wallet payment
                    EWalletPaymentLogic walletPaymentLogic = new EWalletPaymentLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<Payment> otherPaymentOptions = paymentLogic.GetModelsBy(p => p.Person_Id == form.Person.Id && p.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.Session_Id == form.Setting.Session.Id);
                    for (int i = 0; i < otherPaymentOptions.Count; i++)
                    {
                        long currentPaymentId = otherPaymentOptions[i].Id;

                        EWalletPayment walletPayment = walletPaymentLogic.GetModelsBy(p => p.Payment_Id == currentPaymentId).LastOrDefault();
                        if (walletPayment != null)
                        {
                            return Json(new { InvoiceNumber = "You have already generated invoice for E-Wallet deposit, hence cannot use any other payment option!" }, "text/html", JsonRequestBehavior.AllowGet);
                           
                        }
                    }


                    //check late registration
                    SessionLogic sessionLogic = new SessionLogic();
                    Session lateRegSession = sessionLogic.GetModelBy(s => s.Session_Id == form.Setting.Session.Id);
                    if (lateRegSession != null && lateRegSession.IsLateRegistration == true)
                    {
                        //check late reg payment
                        RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                        RemitaPayment remitaPaymentLateReg = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == form.Person.Id && r.PAYMENT.Session_Id == form.Setting.Session.Id &&
                                                                r.PAYMENT.Fee_Type_Id == (int)FeeTypes.LateSchoolFees && (r.Status.Contains("01") || r.Description.Contains("manual"))).LastOrDefault();

                        if (remitaPaymentLateReg == null)
                        {
                            return Json(new { InvoiceNumber = "Pay late registration before proceeding." }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                    }

                    // return Json(new { Error = "School Fees payment is not yet enabled!" }, "text/html", JsonRequestBehavior.AllowGet);
                    FeeType feeType = new FeeType() { Id = (int)FeeTypes.SchoolFees };
                    ApplicantStatus.Status status = ApplicantStatus.Status.GeneratedSchoolFeesInvoice;
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = GenerateInvoiceHelper(form, feeType, status, paymentMode);


                        if (payment != null)
                        {

                            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                            viewModel.remitaPayment = remitaPaymentLogic.GetBy(payment.Id);

                            if (viewModel.remitaPayment == null)
                            {
                                //Get specific amount;

                                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                                AdmissionList admissionList = admissionListLogic.GetModelsBy(a => a.Application_Form_Id == form.Id).LastOrDefault();

                                //Level level = form.ProgrammeFee.Programme.Id == (int)Programmes.NDFullTime || form.ProgrammeFee.Programme.Id == (int)Programmes.NDPartTime ? new Level() { Id = (int)Levels.NDI } : new Level() { Id = (int)Levels.HNDI };
                                int levelId = GetLevel(admissionList.Programme.Id);
                                Level level = new Level { Id = levelId };

                                Amt = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment, level, admissionList.Programme,
                                                                                feeType, admissionList.Session, payment.PaymentMode);

                                if (Amt == 0)
                                {
                                    return Json(new { InvoiceNumber = "No Fee" }, "text/html", JsonRequestBehavior.AllowGet);
                                }

                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);

                                decimal lloydantCommission = 0M;

                                //Get Split Specific details;
                                List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                                RemitaSplitItems singleItem = new RemitaSplitItems();
                                RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "1";

                                lloydantCommission = Convert.ToDecimal(singleItem.beneficiaryAmount);

                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(2);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - lloydantCommission);
                                splitItems.Add(singleItem);


                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                viewModel.remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, Amt);
                                if (viewModel.remitaPayment != null)
                                {
                                    transaction.Complete();
                                }
                            }
                            else
                            {
                                transaction.Complete();
                            }
                            //transaction.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }
        private Int32 GetLevel(int ProgrammeId)
        {
            try
            {
                //set mode of study
                switch (ProgrammeId)
                {
                    case 1:
                        {
                            return 1;
                        }
                    case 2:
                        {
                            return 5;
                        }
                    case 3:
                        {
                            return 3;
                        }
                    case 4:
                        {
                            return 8;
                        }
                    case 5:
                        {
                            return 11;
                        }
                    case 6:
                        {
                            return 13;
                        }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return 0;
        }
        private bool CheckAcceptancePayment(ApplicationForm form)
        {
            try
            {
                int[] acceptanceFeeTypes = { (int)FeeTypes.AcceptanceFee, (int)FeeTypes.HNDAcceptance };
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == form.Person.Id && acceptanceFeeTypes.Contains(r.PAYMENT.Fee_Type_Id) && (r.Status.Contains("021") || r.Description.ToLower().Contains("manual"))).LastOrDefault();
                if (remitaPayment != null)
                    return true;

                PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = etranzactLogic.GetModelsBy(e => e.ONLINE_PAYMENT.PAYMENT.Person_Id == form.Person.Id && acceptanceFeeTypes.Contains(e.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id)).LastOrDefault();
                if (paymentEtranzact != null)
                    return true;

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Payment GenerateInvoiceHelper(ApplicationForm form, FeeType feeType, ApplicantStatus.Status status)
        {
            try
            {
                Payment payment = viewModel.GenerateInvoice(form, status, feeType);
                if (payment == null)
                {
                    SetMessage("Operation Failed! Invoice could not be generated. Refresh browser & try again", Message.Category.Error);
                }

                viewModel.Invoice.Payment = payment;
                viewModel.Invoice.Person = form.Person;
                viewModel.Invoice.JambRegistrationNumber = "";

                if (payment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                {
                    viewModel.AcceptanceInvoiceNumber = payment.InvoiceNumber;

                }
                else if (payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                {
                    viewModel.SchoolFeesInvoiceNumber = payment.InvoiceNumber;
                }
                payment.Person = form.Person;
                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Payment GenerateInvoiceHelper(ApplicationForm form, FeeType feeType, ApplicantStatus.Status status, PaymentMode paymentMode)
        {
            try
            {
                //Payment payment = viewModel.GenerateInvoice(form, status, feeType);
                Payment payment = viewModel.GenerateInvoice(form, status, feeType, paymentMode);
                if (payment == null)
                {
                    SetMessage("Operation Failed! Invoice could not be generated. Refresh browser & try again", Message.Category.Error);
                }

                viewModel.Invoice.Payment = payment;
                viewModel.Invoice.Person = form.Person;
                viewModel.Invoice.JambRegistrationNumber = "";

                if (payment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                {
                    viewModel.AcceptanceInvoiceNumber = payment.InvoiceNumber;

                }
                else if (payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                {
                    viewModel.SchoolFeesInvoiceNumber = payment.InvoiceNumber;
                }
                payment.Person = form.Person;
                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult GenerateAcceptanceReceipt(long fid, string ivn, string con, int st)
        {
            try
            {
                string successMeassge = "Acceptance Receipt has been successfully generated and ready for printing. " +
                    "Print the Acceptance Receipt or Admission Letter by clicking on the Print Receipt or Print Admission Letter button. You can click on the next button( or step 5) to proceed with school fees payment.";


                if (!GenerateReceiptHelper(fid, ivn, con, st, successMeassge))
                {
                    return PartialView("_Message", new Message("RRR has not been paid for! Please crosscheck and try again"));
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return PartialView("_Message", TempData["Message"]);
        }

        private bool GenerateReceiptHelper(long fid, string ivn, string con, int st, string successMeassge)
        {
            try
            {
    
                PaymentLogic paymentLogic = new PaymentLogic();

                Model.Model.Session session = new Model.Model.Session() { Id = 9 };
                Payment existingPayment = paymentLogic.GetBy(ivn);

                if (existingPayment != null)
                {
                    session = existingPayment.Session;
                }



                FeeType feetype = null;
                if (st == 3 || st == 4)
                {
                    Payment paymentx = new Payment();
                    PaymentLogic paymentLogics = new PaymentLogic();
                    paymentx = paymentLogics.GetBy(ivn);
                    if (paymentx.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                    {
                        feetype = new FeeType() { Id = (int)FeeTypes.AcceptanceFee };
                    }
                    else if (paymentx.FeeType.Id == (int)FeeTypes.HNDAcceptance)
                    {
                        feetype = new FeeType() { Id = (int)FeeTypes.HNDAcceptance };
                    }

                }
                else
                {
                    feetype = new FeeType() { Id = (int)FeeTypes.SchoolFees };
                }

                Payment payment = paymentLogic.InvalidConfirmationOrderNumber(con, session, feetype);
                if (payment != null && payment.Id > 0)
                {

                    if (payment.InvoiceNumber == ivn)
                    {
                        Receipt receipt = GetReceipt(ivn, fid, st);
                        if (receipt != null)
                        {
                            SetMessage(successMeassge, Message.Category.Information);
                            return true;
                        }
                    }
                    else
                    {
                        SetMessage("Your Receipt generation failed because the Confirmation Order Number (" + con + ") entered belongs to another invoice number! Please enter your Confirmation Order Number correctly.", Message.Category.Error);
                    }

                }



                SetMessage("Your Receipt generation failed because the Confirmation Number (" + con + ") entered could not be verified. Please confirm and try again", Message.Category.Error);
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GenerateSchoolFeesReceipt(long fid, string ivn, string con, int st)
        {
            try
            {
                string successMeassge = "School Fees Receipt has been successfully generated and ready for printing. Click on the Print Receipt button to print receipt.";

                using (TransactionScope transaction = new TransactionScope())
                {
                    bool isSuccessfull = GenerateReceiptHelper(fid, ivn, con, st, successMeassge);
                    if (isSuccessfull)
                    {
                        //assign matric number
                        ApplicantLogic applicantLogic = new ApplicantLogic();
                        ApplicationFormView applicant = applicantLogic.GetBy(fid);
                        if (applicant != null)
                        {

                            StudentLogic studentLogic = new StudentLogic();
                            bool matricNoAssigned = studentLogic.AssignMatricNumber(applicant);
                            if (matricNoAssigned)
                            {
                                transaction.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return PartialView("_Message", TempData["Message"]);
        }

        public ActionResult Invoice(string ivn)
        {
            try
            {
                if (string.IsNullOrEmpty(ivn))
                {
                    SetMessage("Invoice Not Found! Refresh and Try again ", Message.Category.Error);
                }

                viewModel.GetInvoiceBy(ivn);

                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Invoice_Number == ivn).LastOrDefault();
                if (remitaPayment != null)
                {
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    if (remitaPayment.payment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                    {
                        settings = settingsLogic.GetBy(3);
                    }
                    else if (remitaPayment.payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                    {
                        settings = settingsLogic.GetBy(1);
                    }

                    string Hash = GenerateHash(settings.Api_key, remitaPayment);

                    Student.ViewModels.PaymentViewModel paymentViewModel = new Student.ViewModels.PaymentViewModel();
                    paymentViewModel.RemitaPayment = remitaPayment;
                    paymentViewModel.Hash = Hash;

                    TempData["PaymentViewModel"] = paymentViewModel;
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel.Invoice);
        }

        public ActionResult Receipt(string ivn, long fid, int st)
        {
            Receipt receipt = null;

            try
            {
                receipt = GetReceipt(ivn, fid, st);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }

        private Receipt GetReceipt(string ivn, long fid, int st)
        {
            Receipt receipt = null;

            try
            {
                ApplicantStatus.Status status = (ApplicantStatus.Status)st;
                if (IsNextApplicationStatus(fid, st))
                {
                    receipt = viewModel.GenerateReceipt(ivn, fid, status);
                }
                else
                {
                    receipt = viewModel.GetReceiptBy(ivn);
                }

                if (receipt == null)
                {
                    SetMessage("No receipt found for Invoice No " + ivn, Message.Category.Error);
                }
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ProcessChangeOfCourse()
        {
            try
            {
                viewModel = new AdmissionViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ProcessChangeOfCourse(AdmissionViewModel viewModel)
        {
            try
            {
                if (viewModel.ConfirmationOrderNumber != null && viewModel.ApplicationForm.Number != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
                    ApplicantLogic applicantLogic = new ApplicantLogic();
                    ApplicantClearanceLogic applicantClearanceLogic = new ApplicantClearanceLogic();
                    OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                    PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
                    SponsorLogic sponsorLogic = new SponsorLogic();
                    ChangeOfCourseLogic changeOfCourseLogic = new ChangeOfCourseLogic();
                    PersonLogic personLogic = new PersonLogic();

                    ApplicationForm createdApplicationForm = new ApplicationForm();
                    Person createdPerson = new Person();

                    if (viewModel.ConfirmationOrderNumber.Length > 12)
                    {
                        FeeType feetype = new FeeType() { Id = (int)FeeTypes.ChangeOfCourseFees };
                        Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber, feetype.Id);
                        if (payment != null && payment.Id > 0)
                        {
                            if (payment.FeeType.Id != (int)FeeTypes.ChangeOfCourseFees)
                            {
                                SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrderNumber + ") entered is not for Change of Course Fee payment! Please enter your Change of Course Fee Confirmation Order Number.", Message.Category.Error);
                                return View(viewModel);
                            }

                            ApplicationForm applicationFormOld = applicationFormLogic.GetModelsBy(a => a.Application_Form_Number.Trim() == viewModel.ApplicationForm.Number.Trim()).LastOrDefault();
                            long oldApplicationId = applicationFormOld.Id;

                            ChangeOfCourse checkIfChangeOfCourseExist = changeOfCourseLogic.GetModelsBy(a => a.Old_Person_Id == applicationFormOld.Person.Id && a.Session_Id == applicationFormOld.Payment.Session.Id).LastOrDefault();
                            if (checkIfChangeOfCourseExist != null)
                            {
                                if (checkIfChangeOfCourseExist.ApplicationForm != null)
                                {
                                    AdmissionList admissionList = admissionListLogic.GetModelBy(a => a.Application_Form_Id == checkIfChangeOfCourseExist.ApplicationForm.Id && a.Session_Id == checkIfChangeOfCourseExist.Session.Id);
                                    if (admissionList != null)
                                    {
                                        viewModel.ApplicationForm = checkIfChangeOfCourseExist.ApplicationForm;

                                        SetMessage("Your Change Of Course Fee payment has been successfully confirmed, your new application details can be seen below. You can now proceed with registration using the Check Admission Status link", Message.Category.Information);

                                        return View(viewModel);
                                    }
                                }
                            }

                            using (TransactionScope scope = new TransactionScope())
                            {
                                if (applicationFormOld != null)
                                {
                                    Person oldPerson = personLogic.GetModelBy(p => p.Person_Id == applicationFormOld.Person.Id);
                                    Person newPerson = new Person();

                                    newPerson.ContactAddress = oldPerson.ContactAddress;
                                    newPerson.DateEntered = DateTime.Now;
                                    newPerson.DateOfBirth = oldPerson.DateOfBirth;
                                    newPerson.Email = oldPerson.Email;
                                    newPerson.FirstName = oldPerson.FirstName;
                                    newPerson.HomeAddress = oldPerson.HomeAddress;
                                    newPerson.HomeTown = oldPerson.HomeTown;
                                    newPerson.ImageFileUrl = oldPerson.ImageFileUrl;
                                    newPerson.Initial = oldPerson.Initial;
                                    newPerson.LastName = oldPerson.LastName;
                                    newPerson.LocalGovernment = oldPerson.LocalGovernment;
                                    newPerson.MobilePhone = oldPerson.MobilePhone;
                                    newPerson.Nationality = oldPerson.Nationality;
                                    newPerson.OtherName = oldPerson.OtherName;
                                    newPerson.Religion = oldPerson.Religion;
                                    newPerson.Role = oldPerson.Role;
                                    newPerson.Sex = oldPerson.Sex;
                                    newPerson.State = oldPerson.State;
                                    newPerson.SignatureFileUrl = oldPerson.SignatureFileUrl;
                                    newPerson.Title = oldPerson.Title;
                                    newPerson.Type = oldPerson.Type;

                                    createdPerson = personLogic.Create(newPerson);

                                    AdmissionList admissionList = admissionListLogic.GetModelsBy(a => a.Application_Form_Id == oldApplicationId).LastOrDefault();

                                    if (admissionList == null)
                                    {
                                        SetMessage("Error! Applicant has not been given admission", Message.Category.Error);
                                        return View(viewModel);
                                    }

                                    AppliedCourse appliedCourseOld = appliedCourseLogic.GetModelsBy(a => a.Application_Form_Id == oldApplicationId).LastOrDefault();

                                    AppliedCourse appliedCourseNew = new AppliedCourse();

                                    appliedCourseNew.Department = admissionList.Deprtment;
                                    appliedCourseNew.ApplicationForm = null;
                                    appliedCourseNew.Option = admissionList.DepartmentOption;
                                    appliedCourseNew.Person = createdPerson;
                                    appliedCourseNew.Programme = appliedCourseOld.Programme;

                                    appliedCourseLogic.Create(appliedCourseNew);

                                    //Create New Application
                                    ApplicationForm applicationFormNew = new ApplicationForm();

                                    applicationFormNew.DateSubmitted = DateTime.Now;
                                    applicationFormNew.Payment = payment;
                                    applicationFormNew.Person = createdPerson;
                                    applicationFormNew.ProgrammeFee = applicationFormOld.ProgrammeFee;
                                    applicationFormNew.RejectReason = applicationFormOld.RejectReason;
                                    applicationFormNew.Rejected = applicationFormOld.Rejected;
                                    applicationFormNew.Release = applicationFormOld.Release;
                                    applicationFormNew.Remarks = applicationFormOld.Remarks;
                                    applicationFormNew.Setting = applicationFormOld.Setting;
                                    applicationFormNew.IsAwaitingResult = applicationFormOld.IsAwaitingResult;

                                    createdApplicationForm = applicationFormLogic.Create(applicationFormNew);

                                    if (createdApplicationForm.Number == null)
                                    {
                                        SetMessage("Error! Try Again", Message.Category.Error);
                                        return View(viewModel);
                                    }

                                    admissionList.Form = createdApplicationForm;
                                    admissionListLogic.ModifyListOnly(admissionList);

                                    //Create New Jamb Details
                                    ApplicantJambDetail applicantJambDetail = applicantJambDetailLogic.GetModelsBy(a => a.Application_Form_Id == oldApplicationId).LastOrDefault();
                                    if (applicantJambDetail != null)
                                    {
                                        ApplicantJambDetail newApplicantJambDetail = new ApplicantJambDetail();
                                        newApplicantJambDetail.ApplicationForm = createdApplicationForm;
                                        newApplicantJambDetail.Person = createdPerson;
                                        newApplicantJambDetail.JambRegistrationNumber = applicantJambDetail.JambRegistrationNumber;
                                        newApplicantJambDetail.InstitutionChoice = applicantJambDetail.InstitutionChoice;
                                        newApplicantJambDetail.JambScore = applicantJambDetail.JambScore;
                                        newApplicantJambDetail.Subject1 = applicantJambDetail.Subject1;
                                        newApplicantJambDetail.Subject2 = applicantJambDetail.Subject2;
                                        newApplicantJambDetail.Subject3 = applicantJambDetail.Subject3;
                                        newApplicantJambDetail.Subject4 = applicantJambDetail.Subject4;

                                        applicantJambDetailLogic.Create(newApplicantJambDetail);
                                    }

                                    //create New Applicant
                                    Model.Model.Applicant applicant = applicantLogic.GetModelsBy(a => a.Application_Form_Id == oldApplicationId).LastOrDefault();
                                    if (applicant != null)
                                    {
                                        Model.Model.Applicant newApplicant = new Model.Model.Applicant();
                                        newApplicant.ApplicationForm = createdApplicationForm;
                                        newApplicant.Person = createdPerson;
                                        newApplicant.Ability = applicant.Ability;
                                        newApplicant.ExtraCurricullarActivities = applicant.ExtraCurricullarActivities;
                                        newApplicant.OtherAbility = applicant.OtherAbility;
                                        newApplicant.Status = applicant.Status;

                                        applicantLogic.Create(newApplicant);
                                    }

                                    //Modify Applicant Clearance if exist
                                    //ApplicantClearance applicantClearance = applicantClearanceLogic.GetModelsBy(a => a.Application_Form_Id == oldApplicationId).LastOrDefault();
                                    //if (applicantClearance != null)
                                    //{
                                    //    applicantClearance.ApplicationForm = createdApplicationForm;
                                    //    applicantClearanceLogic.Modify(applicantClearance);
                                    //}

                                    //Modify OLevelResult
                                    List<OLevelResult> oLevelResults = oLevelResultLogic.GetModelsBy(o => o.Application_Form_Id == oldApplicationId);
                                    if (oLevelResults.Count > 0)
                                    {
                                        for (int i = 0; i < oLevelResults.Count; i++)
                                        {
                                            oLevelResults[i].ApplicationForm = createdApplicationForm;
                                            oLevelResults[i].Person = createdPerson;
                                            oLevelResultLogic.Modify(oLevelResults[i]);
                                        }
                                    }

                                    //Check and Modify Previous Education
                                    PreviousEducation previousEducation = previousEducationLogic.GetModelsBy(p => p.Application_Form_Id == oldApplicationId).LastOrDefault();
                                    if (previousEducation != null)
                                    {
                                        previousEducation.ApplicationForm = createdApplicationForm;
                                        previousEducation.Person = createdPerson;
                                        previousEducationLogic.Modify(previousEducation);
                                    }


                                    //Modify Change Of Course Detail
                                    if (applicantJambDetail != null)
                                    {
                                        ChangeOfCourse changeOfCourse = changeOfCourseLogic.GetModelsBy(c => c.Jamb_Registration_Number == applicantJambDetail.JambRegistrationNumber && c.Session_Id == payment.Session.Id).LastOrDefault();
                                        changeOfCourse.ApplicationForm = createdApplicationForm;
                                        changeOfCourse.NewPerson = createdPerson;
                                        changeOfCourse.OldPerson = applicationFormOld.Person;

                                        changeOfCourseLogic.Modify(changeOfCourse);
                                    }
                                    else
                                    {
                                        SetMessage("No Jamb Detail record found for Applicant ", Message.Category.Error);
                                    }
                                }

                                scope.Complete();
                            }

                            AppliedCourse createdAppliedCourse = appliedCourseLogic.GetBy(createdPerson);
                            createdAppliedCourse.Person = createdPerson;
                            createdAppliedCourse.ApplicationForm = createdApplicationForm;
                            appliedCourseLogic.Modify(createdAppliedCourse);

                            //Check and Create Applicant Sponsor
                            Sponsor sponsor = sponsorLogic.GetModelsBy(s => s.Application_Form_Id == oldApplicationId).LastOrDefault();
                            if (sponsor != null)
                            {
                                Sponsor newSponsor = new Sponsor();
                                newSponsor.ApplicationForm = createdApplicationForm;
                                newSponsor.Person = createdPerson;
                                newSponsor.ContactAddress = sponsor.ContactAddress;
                                newSponsor.MobilePhone = sponsor.MobilePhone;
                                newSponsor.Name = sponsor.Name;
                                newSponsor.Relationship = sponsor.Relationship;

                                sponsorLogic.Create(newSponsor);
                            }

                            viewModel.ApplicationForm = applicationFormLogic.GetModelBy(a => a.Application_Form_Id == createdApplicationForm.Id);
                            viewModel.ApplicationForm.Person = personLogic.GetModelBy(p => p.Person_Id == createdPerson.Id);
                            SetMessage("Your Change Of Course Fee payment has been successfully confirmed, your new application details can be seen below. You can now proceed with registration using the Check Admission Status link", Message.Category.Information);

                        }
                    }
                    else
                    {
                        SetMessage("Error! the confirmation order number entered is not valid", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        private bool IsNextApplicationStatus(long formId, int status)
        {
            try
            {
                ApplicationForm form = new ApplicationForm() { Id = formId };

                ApplicantLogic applicantLogic = new ApplicantLogic();
                Model.Model.Applicant applicant = applicantLogic.GetBy(form);
                if (applicant != null)
                {
                    if (applicant.Status.Id < (int)status)
                    {
                        return true;
                    }
                    else
                    {
                        if (((int)status == (int)ApplicantStatus.Status.GeneratedAcceptanceInvoice || (int)status == (int)ApplicantStatus.Status.GeneratedAcceptanceReceipt) &&
                            applicant.Status.Id < 8)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    throw new Exception("Applicant Status not found!");
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetSelectedSittingSubjectAndGrade(AdmissionViewModel olevelViewModel)
        {
            try
            {
                if (olevelViewModel != null && olevelViewModel.FirstSittingOLevelResultDetails != null && olevelViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (OLevelResultDetail firstSittingOLevelResultDetail in olevelViewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null && firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }

                if (olevelViewModel != null && olevelViewModel.SecondSittingOLevelResultDetails != null && olevelViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (OLevelResultDetail secondSittingOLevelResultDetail in olevelViewModel.SecondSittingOLevelResultDetails)
                    {
                        if (secondSittingOLevelResultDetail.Subject != null && secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] = new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] = new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
        }

        private bool InvalidOlevelResult(AdmissionViewModel viewModel)
        {
            try
            {
                if (InvalidNumberOfOlevelSubject(viewModel.FirstSittingOLevelResultDetails, viewModel.SecondSittingOLevelResultDetails))
                {
                    return true;
                }

                if (InvalidOlevelSubjectOrGrade(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects, viewModel.OLevelGrades, Utility.FIRST_SITTING))
                {
                    return true;
                }

                if (viewModel.SecondSittingOLevelResult != null)
                {
                    if (viewModel.SecondSittingOLevelResult.ExamNumber != null && viewModel.SecondSittingOLevelResult.Type != null && viewModel.SecondSittingOLevelResult.Type.Id > 0 && viewModel.SecondSittingOLevelResult.ExamYear > 0)
                    {
                        if (InvalidOlevelSubjectOrGrade(viewModel.SecondSittingOLevelResultDetails, viewModel.OLevelSubjects, viewModel.OLevelGrades, Utility.SECOND_SITTING))
                        {
                            return true;
                        }
                    }
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.FirstSittingOLevelResultDetails, viewModel.FirstSittingOLevelResult, Utility.FIRST_SITTING))
                {
                    return true;
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.SecondSittingOLevelResultDetails, viewModel.SecondSittingOLevelResult, Utility.SECOND_SITTING))
                {
                    return true;
                }

                if (NoOlevelSubjectSpecified(viewModel.FirstSittingOLevelResultDetails, viewModel.FirstSittingOLevelResult, Utility.FIRST_SITTING))
                {
                    return true;
                }
                if (NoOlevelSubjectSpecified(viewModel.SecondSittingOLevelResultDetails, viewModel.SecondSittingOLevelResult, Utility.SECOND_SITTING))
                {
                    return true;
                }

                //if (InvalidOlevelType(viewModel.FirstSittingOLevelResult.Type, viewModel.SecondSittingOLevelResult.Type))
                //{
                //    return true;
                //}

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelSubjectOrGrade(List<OLevelResultDetail> oLevelResultDetails, List<OLevelSubject> subjects, List<OLevelGrade> grades, string sitting)
        {
            try
            {
                List<OLevelResultDetail> subjectList = null;
                if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                {
                    subjectList = oLevelResultDetails.Where(r => r.Subject.Id > 0 || r.Grade.Id > 0).ToList();
                }

                foreach (OLevelResultDetail oLevelResultDetail in subjectList)
                {
                    OLevelSubject subject = subjects.Where(s => s.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
                    OLevelGrade grade = grades.Where(g => g.Id == oLevelResultDetail.Grade.Id).SingleOrDefault();

                    List<OLevelResultDetail> results = subjectList.Where(o => o.Subject.Id == oLevelResultDetail.Subject.Id).ToList();
                    if (results != null && results.Count > 1)
                    {
                        SetMessage("Duplicate " + subject.Name.ToUpper() + " Subject detected in " + sitting + "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    else if (oLevelResultDetail.Subject.Id > 0 && oLevelResultDetail.Grade.Id <= 0)
                    {
                        SetMessage("No Grade specified for Subject " + subject.Name.ToUpper() + " in " + sitting + "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    else if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    {
                        SetMessage("No Subject specified for Grade" + grade.Name.ToUpper() + " in " + sitting + "! Please modify.", Message.Category.Error);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelResultHeaderInformation(List<OLevelResultDetail> resultDetails, OLevelResult oLevelResult, string sitting)
        {
            try
            {
                if (resultDetails != null && resultDetails.Count > 0)
                {
                    List<OLevelResultDetail> subjectList = resultDetails.Where(r => r.Subject.Id > 0).ToList();

                    if (subjectList != null && subjectList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(oLevelResult.ExamNumber))
                        {
                            SetMessage("O-Level Exam Number not set for " + sitting + " ! Please modify", Message.Category.Error);
                            return true;
                        }
                        else if (oLevelResult.Type == null || oLevelResult.Type.Id <= 0)
                        {
                            SetMessage("O-Level Exam Type not set for " + sitting + " ! Please modify", Message.Category.Error);
                            return true;
                        }
                        else if (oLevelResult.ExamYear <= 0)
                        {
                            SetMessage("O-Level Exam Year not set for " + sitting + " ! Please modify", Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool NoOlevelSubjectSpecified(List<OLevelResultDetail> oLevelResultDetails, OLevelResult oLevelResult, string sitting)
        {
            try
            {
                if (!string.IsNullOrEmpty(oLevelResult.ExamNumber) || (oLevelResult.Type != null && oLevelResult.Type.Id > 0) || (oLevelResult.ExamYear > 0))
                {
                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> oLevelResultDetailsEntered = oLevelResultDetails.Where(r => r.Subject.Id > 0).ToList();
                        if (oLevelResultDetailsEntered == null || oLevelResultDetailsEntered.Count <= 0)
                        {
                            SetMessage("No O-Level Subject specified for " + sitting + "! At least one subject must be specified when Exam Number, O-Level Type and Year are all specified for the sitting.", Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidNumberOfOlevelSubject(List<OLevelResultDetail> firstSittingResultDetails, List<OLevelResultDetail> secondSittingResultDetails)
        {
            const int FIVE = 5;

            try
            {
                int totalNoOfSubjects = 0;

                List<OLevelResultDetail> firstSittingSubjectList = null;
                List<OLevelResultDetail> secondSittingSubjectList = null;

                if (firstSittingResultDetails != null && firstSittingResultDetails.Count > 0)
                {
                    firstSittingSubjectList = firstSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (firstSittingSubjectList != null)
                    {
                        totalNoOfSubjects += firstSittingSubjectList.Count;
                    }
                }

                if (secondSittingResultDetails != null && secondSittingResultDetails.Count > 0)
                {
                    secondSittingSubjectList = secondSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (secondSittingSubjectList != null)
                    {
                        totalNoOfSubjects += secondSittingSubjectList.Count;
                    }
                }

                if (totalNoOfSubjects == 0)
                {
                    SetMessage("No O-Level Result Details found for both sittings!", Message.Category.Error);
                    return true;
                }
                else if (totalNoOfSubjects < FIVE)
                {
                    SetMessage("O-Level Result cannot be less than " + FIVE + " subjects in both sittings!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelType(OLevelType firstSittingOlevelType, OLevelType secondSittingOlevelType)
        {
            try
            {
                if (firstSittingOlevelType != null && secondSittingOlevelType != null)
                {
                    if ((firstSittingOlevelType.Id != secondSittingOlevelType.Id) && firstSittingOlevelType.Id > 0 && secondSittingOlevelType.Id > 0)
                    {
                        if (firstSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage("NABTEB O-Level Type in " + Utility.FIRST_SITTING + " cannot be combined with any other O-Level Type! Please modify.", Message.Category.Error);
                            return true;
                        }
                        else if (secondSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage("NABTEB O-Level Type in " + Utility.SECOND_SITTING + " cannot be combined with any other O-Level Type! Please modify.", Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult VerifyOlevelResult(AdmissionViewModel viewModel)
        {
            string enc = Abundance_Nk.Web.Models.Utility.Encrypt(viewModel.ApplicationForm.Id.ToString());

            try
            {
                //check if applicant has been previously cleared
                ApplicantClearanceLogic applicantClearanceLogic = new ApplicantClearanceLogic();
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                if (applicantClearanceLogic.IsCleared(viewModel.ApplicationForm))
                {
                    var getAppForm = applicationFormLogic.GetModelBy(ap => ap.Person_Id == viewModel.ApplicationForm.Person.Id);
                    //if (viewModel.ApplicationForm.CleardForAcceptance == true)
                    if (getAppForm != null && getAppForm.CleardForAcceptance == true)
                    {
                        if (viewModel.ApplicantStatusId < (int)ApplicantStatus.Status.ClearedAndAccepted)
                        {
                            ApplicantStatus.Status newStatus = ApplicantStatus.Status.ClearedAndAccepted;
                            ApplicantLogic applicantLogic = new ApplicantLogic();
                            applicantLogic.UpdateStatus(viewModel.ApplicationForm, newStatus);
                        }
                        SetMessage("You have already been successfully cleared. Congratulations! kindly move on to next step.", Message.Category.Information);
                        return RedirectToAction("Index", new { fid = enc });
                    }
                    else
                    {
                        SetMessage("You have already been successfully cleared. But we regret to re-inform you that you did not qualify with the following reason: " +
                            viewModel.ApplicationForm.Remarks + ". Kindly try again next academic session if you've corrected your deficiency.", Message.Category.Information);
                    }
                }

                //validate o-level result entry
                //if (InvalidOlevelResult(viewModel))
                //{
                //	return RedirectToAction("Index", new { fid = enc });
                //}

                using (TransactionScope transaction = new TransactionScope())
                {
                    //get applicant's applied course
                    //if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                    //{
                    //	viewModel.FirstSittingOLevelResult.ApplicationForm = viewModel.ApplicationForm;
                    //	viewModel.FirstSittingOLevelResult.Person = viewModel.ApplicationForm.Person;
                    //	viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
                    //}

                    //if (viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                    //{
                    //	viewModel.SecondSittingOLevelResult.ApplicationForm = viewModel.ApplicationForm;
                    //	viewModel.SecondSittingOLevelResult.Person = viewModel.ApplicationForm.Person;
                    //	viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
                    //}

                    //ModifyOlevelResult(viewModel.FirstSittingOLevelResult, viewModel.FirstSittingOLevelResultDetails);
                    //ModifyOlevelResult(viewModel.SecondSittingOLevelResult, viewModel.SecondSittingOLevelResultDetails);


                    //get applicant's applied course
                    //AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    //AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    //AppliedCourse appliedCourse = appliedCourseLogic.GetBy(viewModel.ApplicationForm.Person);

                    //Set department to admitted department since it might vary
                    //AdmissionList admissionList = new AdmissionList();
                    //admissionList = admissionListLogic.GetBy(viewModel.ApplicationForm.Person);
                    //appliedCourse.Department = admissionList.Deprtment;

                    //if (appliedCourse == null)
                    //{
                    //	SetMessage("Your O-Level was successfully verified, but could not be cleared because no Applied Course was not found for your application", Message.Category.Error);
                    //	return RedirectToAction("Index", new { fid = enc });
                    //}

                    //set reject reason if exist
                    ApplicantStatus.Status newStatus;
                    //AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
                    ApplicationFormLogic formLogic = new ApplicationFormLogic();
                    ApplicationForm form = formLogic.GetBy(viewModel.ApplicationForm.Id);

                    if (form.CleardForAcceptance == true)
                    {
                        newStatus = ApplicantStatus.Status.ClearedAndAccepted;
                        //newStatus = ApplicantStatus.Status.GeneratedAcceptanceReceipt;
                        viewModel.ApplicationForm.Rejected = false;
                        viewModel.ApplicationForm.Release = false;

                        SetMessage("You have already been successfully cleared. Congratulations! kindly move on to next step.", Message.Category.Information);
                        return RedirectToAction("Index", new { fid = enc });
                    }
                    else if (form.CleardForAcceptance == null)
                    {
                        SetMessage("You have not been cleard, kindly proceed to the clearing officers's office with your credentials.", Message.Category.Warning);
                        return RedirectToAction("Index", new { fid = enc });
                    }
                    else
                    {
                        newStatus = ApplicantStatus.Status.ClearedAndRejected;
                        viewModel.ApplicationForm.Rejected = true;
                        viewModel.ApplicationForm.Release = true;

                        SetMessage("You have already been successfully cleared. But we regret to re-inform you that you did not qualify with the following reason: " +
                            viewModel.ApplicationForm.Remarks + ". Kindly try again next academic session if you've corrected your deficiency.", Message.Category.Information);
                    }

                    //viewModel.ApplicationForm.RejectReason = rejectReason;
                    //ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    //applicationFormLogic.SetRejectReason(viewModel.ApplicationForm);


                    //set applicant new status
                    ApplicantLogic applicantLogic = new ApplicantLogic();
                    applicantLogic.UpdateStatus(viewModel.ApplicationForm, newStatus);


                    //save clearance metadata
                    //ApplicantClearance applicantClearance = new ApplicantClearance();
                    //applicantClearance = applicantClearanceLogic.GetBy(viewModel.ApplicationForm);
                    //if (applicantClearance == null)
                    //{
                    //	applicantClearance = new ApplicantClearance();
                    //	applicantClearance.ApplicationForm = viewModel.ApplicationForm;
                    //	applicantClearance.Cleared = string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason) ? true : false;
                    //	applicantClearance.DateCleared = DateTime.Now;
                    //	applicantClearanceLogic.Create(applicantClearance);
                    //}
                    //else
                    //{
                    //	applicantClearance.Cleared = string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason) ? true : false;
                    //	applicantClearance.DateCleared = DateTime.Now;
                    //	applicantClearanceLogic.Modify(applicantClearance);
                    //}

                    transaction.Complete();
                }

                //if (string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason))
                //{
                //	SetMessage("O-Level result has been successfully verified and you have been automatically cleared by the system", Message.Category.Information);
                //}
                //else
                //{
                //	SetMessage("We regret to inform you that you did not qualify with the following reason: " + viewModel.ApplicationForm.RejectReason, Message.Category.Error);
                //}
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return RedirectToAction("Index", new { fid = enc });
        }
        private void ModifyOlevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                OlevelResultdDetailsAudit olevelResultdDetailsAudit = new OlevelResultdDetailsAudit();
                olevelResultdDetailsAudit.Action = "Modify";
                olevelResultdDetailsAudit.Operation = "Modify O level (Admission Controller)";
                olevelResultdDetailsAudit.Client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                UserLogic loggeduser = new UserLogic();
                olevelResultdDetailsAudit.User = loggeduser.GetModelBy(u => u.User_Id == 1);

                OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null && oLevelResult.ExamYear > 0)
                {
                    if (oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult, olevelResultdDetailsAudit);
                    }
                    else
                    {
                        OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                        OLevelResult newOLevelResult = oLevelResultLogic.Create(oLevelResult);
                        oLevelResult.Id = newOLevelResult.Id;
                    }

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> olevelResultDetails = oLevelResultDetails.Where(m => m.Grade != null && m.Grade.Id > 0 && m.Subject != null && m.Subject.Id > 0).ToList();
                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetailLogic.Create(oLevelResultDetail, olevelResultdDetailsAudit);
                        }

                        //oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void UpdateStudentRRRPayments(Person person)
        {
            try
            {
                if (person != null)
                {
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    RemitaResponse remitaResponse = new RemitaResponse();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                    settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
                    string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);

                    List<RemitaPayment> remitaPayments = remitaPaymentLogic.GetModelsBy(m => m.PAYMENT.Person_Id == person.Id);

                    foreach (RemitaPayment remitaPayment in remitaPayments)
                    {
                        remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
                        if (remitaResponse != null && remitaResponse.Status != null)
                        {
                            remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                            remitaPayment.TransactionDate = !string.IsNullOrEmpty(remitaResponse.paymentDate) ?
                                                            Convert.ToDateTime(remitaResponse.paymentDate) :
                                                            remitaPayment.TransactionDate;
                            remitaPaymentLogic.Modify(remitaPayment);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}