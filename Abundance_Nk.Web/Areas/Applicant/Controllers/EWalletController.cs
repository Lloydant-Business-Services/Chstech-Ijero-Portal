using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using BarcodeLib;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class EWalletController : BaseController
    {
        private Person _person;
        private PersonLogic _personLogic;
        private EWalletViewModel _viewModel;
        private EWalletPaymentLogic _eWalletPaymentLogic;
        public EWalletController()
        {
            _personLogic = new PersonLogic();
        }
        public ActionResult Index()
        {
            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
            _viewModel = new EWalletViewModel();
            var applicationFormNumber = TempData["AppNumber"] as string;
            TempData.Keep("AppNumber");
            var getApplicantDetails = applicationFormLogic.GetModelBy(a => a.Application_Form_Number == applicationFormNumber);

            _viewModel.ApplicationForm = getApplicantDetails;
            return View(_viewModel);
        }
        [HttpPost]
        public ActionResult Index(EWalletViewModel viewModel)
        {
            try
            {
                var applicationFormNumber = TempData["AppNumber"] as string;
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                ApplicationForm applicationForm = applicationFormLogic.GetModelsBy(a => a.Application_Form_Number == applicationFormNumber).LastOrDefault();
                if (applicationForm == null)
                {
                    SetMessage("Application form not found.", Message.Category.Error);
                    return View(viewModel);
                }

                viewModel.ApplicationForm = applicationForm;

                _eWalletPaymentLogic = new EWalletPaymentLogic();
                viewModel.EWalletPayments = _eWalletPaymentLogic.GetModelsBy(s => s.Person_Id == applicationForm.Person.Id);

                viewModel.EWalletPayments.ForEach(e =>
                {
                    e.PaymentStatus = _eWalletPaymentLogic.GetPaymentStatus(e);
                    e.RRR = _eWalletPaymentLogic.GetPaymentPin(e);
                });

                if (viewModel.EWalletPayments != null && viewModel.EWalletPayments.Count > 0)
                    viewModel.EWalletPayments = viewModel.EWalletPayments.OrderBy(e => e.Session.Id).ThenBy(e => e.FeeType.Id).ThenBy(e => e.Payment.DatePaid).ToList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult GenerateInvoice(string personId)
        {
            _viewModel = new EWalletViewModel();
            try
            {
                long applicantId = Convert.ToInt64(Utility.Decrypt(personId));

                _viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.ShortFall };
                _viewModel.PaymentType = new PaymentType() { Id = 2 };

                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                AdmissionList admissionList = admissionListLogic.GetModelsBy(s => s.APPLICATION_FORM.Person_Id == applicantId).LastOrDefault();

                ViewBag.Sessions = _viewModel.SessionSelectListItem;
                ViewBag.Programmes = _viewModel.ProgrammeSelectListItem;
                ViewBag.Departments = _viewModel.DepartmentSelectListItem;
                ViewBag.Levels = _viewModel.LevelSelectListItem;
                TempData["ApplicantSession"] = admissionList.Session.Id;
                TempData.Keep("ApplicantSession");

                if (admissionList != null)
                {
                    PersonLogic personLogic = new PersonLogic();

                    _viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == applicantId);
                    _viewModel.AdmissionList = admissionList;

                    ViewBag.Programmes = new SelectList(_viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, _viewModel.AdmissionList.Programme.Id);
                    ViewBag.Departments = new SelectList(_viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT, _viewModel.AdmissionList.Deprtment.Id);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(_viewModel);
        }
        [HttpPost]
        public ActionResult GenerateInvoice(EWalletViewModel viewModel)
        {
            try
            {
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                EWalletPaymentLogic walletPaymentLogic = new EWalletPaymentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                decimal Amt = 0M;
                Amt = viewModel.EWalletPayment.Amount;
                var applicantSession = TempData["ApplicantSession"];
                int convertedSession = Convert.ToInt16(applicantSession);
                Session session = sessionLogic.GetModelBy(s => s.Session_Id == convertedSession);

                if (Amt < 5000M)
                {
                    SetMessage("Amount must be greater than N5000!", Message.Category.Error);
                    return RedirectToAction("GenerateInvoice", new { personId = Utility.Encrypt(viewModel.Person.Id.ToString()) });
                }

                //check programme
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                ApplicantLogic applicantLogic = new ApplicantLogic();
                AdmissionList admissionList = admissionListLogic.GetModelsBy(s => s.APPLICATION_FORM.Person_Id == viewModel.Person.Id).LastOrDefault();

                bool isFirstInvoice = IsFirstInvoice(session, viewModel.Person);

                //check if paid first invoice generated
                if (!isFirstInvoice)
                {
                    bool paidFirstInvoice = PaidFirstInvoice(session, viewModel.Person);
                    if (!paidFirstInvoice)
                    {
                        SetMessage("Kindly make payment for the first deposit invoice before making other deposits!", Message.Category.Error);
                        return RedirectToAction("GenerateInvoice", new { personId = Utility.Encrypt(viewModel.Person.Id.ToString()) });
                    }
                }
                


                int[] allowedProgrammes = { (int)Programmes.HNDEvening, (int)Programmes.HNDPartTime, (int)Programmes.NDEveningFullTime, (int)Programmes.NDPartTime };

                if (admissionList == null || !allowedProgrammes.Contains(admissionList.Programme.Id))
                {
                    SetMessage("You cannot use this means of payment.", Message.Category.Error);
                    return RedirectToAction("GenerateInvoice", new { personId = Utility.Encrypt(viewModel.Person.Id.ToString()) });
                }

                //check for normal installmental payment
                List<Payment> otherPaymentOptions = paymentLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.Session_Id == session.Id);
                for (int i = 0; i < otherPaymentOptions.Count; i++)
                {
                    long currentPaymentId = otherPaymentOptions[i].Id;

                    EWalletPayment walletPayment = walletPaymentLogic.GetModelsBy(p => p.Payment_Id == currentPaymentId).LastOrDefault();
                    if (walletPayment == null)
                    {
                        SetMessage("You have already generated invoice for installmental payment, hence cannot use the E-Wallet!", Message.Category.Error);
                        return RedirectToAction("GenerateInvoice", new { personId = Utility.Encrypt(viewModel.Person.Id.ToString()) });
                    }
                }

                Payment payment = null;

                using (TransactionScope transaction = new TransactionScope())
                {
                    FeeType feeType = new FeeType() { Id = (int)FeeTypes.ShortFall };
                    viewModel.FeeType = feeType;
                    payment = CreatePayment(viewModel);

                    if(isFirstInvoice)
                    {
                        applicantLogic.UpdateStatus(admissionList.Form, ApplicantStatus.Status.GeneratedSchoolFeesInvoice);
                    }

                    //Get Payment Specific Setting
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetBy(1);

                    //Get Split Specific details;
                    List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                    RemitaSplitItems singleItem = new RemitaSplitItems();
                    RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();

                    singleItem = splitItemLogic.GetBy(1);
                    singleItem.deductFeeFrom = "1";
                    splitItems.Add(singleItem);
                    singleItem = splitItemLogic.GetBy(2);
                    singleItem.deductFeeFrom = "0";
                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                    splitItems.Add(singleItem);

                    //Get BaseURL
                    string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);

                    viewModel.RemitaPayment = !isFirstInvoice ? remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES - E-WALLET", settings, Amt) :
                                                remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES - E-WALLET", splitItems, settings, Amt);

                    viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                    if (viewModel.RemitaPayment != null)
                    {
                        transaction.Complete();
                    }
                }

                TempData["EWalletViewModel"] = viewModel;
                return RedirectToAction("Invoice", "EWallet", new { Area = "Applicant", paymentId = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()), });
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("GenerateInvoice", new { personId = Utility.Encrypt(viewModel.Person.Id.ToString()) });
        }

        private bool PaidFirstInvoice(Model.Model.Session session, Person person)
        {
            try
            {
                _eWalletPaymentLogic = new EWalletPaymentLogic();
                EWalletPayment eWalletPayment = _eWalletPaymentLogic.GetModelsBy(s => s.Person_Id == person.Id && s.Session_Id == session.Id).FirstOrDefault();
                if (eWalletPayment == null)
                {
                    return false;
                }
                else if (!_eWalletPaymentLogic.GetPaymentStatus(eWalletPayment))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool IsFirstInvoice(Model.Model.Session session, Person person)
        {
            try
            {
                _eWalletPaymentLogic = new EWalletPaymentLogic();
                EWalletPayment eWalletPayment = _eWalletPaymentLogic.GetModelsBy(s => s.Person_Id == person.Id && s.Session_Id == session.Id).FirstOrDefault();
                if (eWalletPayment == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Payment CreatePayment(EWalletViewModel viewModel)
        {
            Payment newPayment = new Payment();
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                var applicantSession = TempData["ApplicantSession"];
                int convertedSession = Convert.ToInt16(applicantSession);
                Session session = sessionLogic.GetModelBy(s => s.Session_Id == convertedSession);
                viewModel.Session = session;

                Payment payment = new Payment();
                payment.PaymentMode = new PaymentMode() { Id = (int)PaymentModes.Full };
                payment.PaymentType = viewModel.PaymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = viewModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                OnlinePayment newOnlinePayment = null;
                newPayment = paymentLogic.Create(payment);
                newPayment.FeeDetails = new List<FeeDetail> { new FeeDetail { Fee = new Fee { Amount = viewModel.EWalletPayment.Amount } } };
                if (newPayment != null)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }

                CreateEWalletPayment(newPayment, viewModel);

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateEWalletPayment(Payment newPayment, EWalletViewModel viewModel)
        {
            try
            {
                if (newPayment.Id > 0)
                {
                    EWalletPayment eWalletPayment = new EWalletPayment();
                    _eWalletPaymentLogic = new EWalletPaymentLogic();

                    eWalletPayment.Session = viewModel.Session;
                    eWalletPayment.Amount = viewModel.EWalletPayment.Amount;
                    eWalletPayment.FeeType = new FeeType { Id = (int)FeeTypes.SchoolFees };
                    eWalletPayment.Payment = newPayment;
                    eWalletPayment.Person = viewModel.Person;

                    _eWalletPaymentLogic.Create(eWalletPayment);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult CardPayment()
        {
            EWalletViewModel viewModel = (EWalletViewModel)TempData["EWalletViewModel"];
            viewModel.ResponseUrl = ConfigurationManager.AppSettings["RemitaResponseUrl"].ToString();
            TempData.Keep("EWalletViewModel");

            return View(viewModel);
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
        [AllowAnonymous]
        public ActionResult Invoice(string paymentId)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(paymentId));
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(paymentid);

                Invoice invoice = new Invoice();
                invoice.Person = payment.Person;
                invoice.Payment = payment;

                invoice.barcodeImageUrl = GenerateBarcode(paymentid);

                Model.Model.Student student = new Model.Model.Student();
                StudentLogic studentLogic = new StudentLogic();
                student = studentLogic.GetBy(payment.Person.Id);

                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic levelLogic = new StudentLevelLogic();
                if (student != null)
                {
                    studentLevel = levelLogic.GetBy(student.Id);
                    invoice.MatricNumber = student.MatricNumber;
                }

                ApplicationFormLogic formLogic = new ApplicationFormLogic();
                ApplicationForm form = formLogic.GetModelsBy(a => a.Person_Id == payment.Person.Id).LastOrDefault();
                if (student == null && form != null)
                {
                    invoice.MatricNumber = form.Number;
                }
                
                invoice.Department = studentLevel != null ? studentLevel.Department : null;

                invoice.Payment = payment;

                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                if (remitaPayment != null)
                {
                    invoice.remitaPayment = remitaPayment;

                    if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                    {
                        payment.FeeDetails = new List<FeeDetail>();
                        payment.FeeDetails.Add(new FeeDetail { Fee = new Fee { Amount = remitaPayment.TransactionAmount, Name = "E-Deposit" } });
                        invoice.Payment = payment;
                    }

                    invoice.Amount = remitaPayment.TransactionAmount;
                }

                EWalletViewModel viewModel = (EWalletViewModel)TempData["EWalletViewModel"];
                if (viewModel == null && remitaPayment != null)
                {
                    //Get Payment Specific Setting
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetBy(1);

                    viewModel = new EWalletViewModel();

                    viewModel.RemitaPayment = remitaPayment;
                    viewModel.Hash = GenerateHash(settings.Api_key, remitaPayment);

                    TempData["EWalletViewModel"] = viewModel;
                }
                else
                {
                    TempData.Keep("EWalletViewModel");
                }

                return View(invoice);
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
        private string GenerateBarcode(long paymentid)
        {
            string barcodeImageUrl = "";
            try
            {
                BarcodeLib.Barcode barcode = new BarcodeLib.Barcode(paymentid.ToString(), TYPE.CODE39);
                Image image = barcode.Encode(TYPE.CODE39, paymentid.ToString());
                byte[] imageByteData = imageToByteArray(image);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);

                barcodeImageUrl = imageDataURL;
            }
            catch (Exception)
            {
                throw;
            }

            return barcodeImageUrl;
        }
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        [AllowAnonymous]
        public ActionResult Receipt(long paymentId)
        {
            Receipt receipt = null;

            try
            {
                receipt = GetReceiptBy(paymentId);
                if (receipt == null)
                {
                    SetMessage("No receipt found!", Message.Category.Error);
                }
                else
                {
                    receipt.barcodeImageUrl = "https://students.fedpolyado.edu.ng/Applicant/EWallet/Receipt?paymentId=" + paymentId; ;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }
        public Receipt GetReceiptBy(long pmid)
        {
            Receipt receipt = null;
            PaymentLogic paymentLogic = new PaymentLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            try
            {
                Payment payment = paymentLogic.GetBy(pmid);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }

                PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(o => o.Payment_Id == payment.Id);
                if (paymentEtranzact != null)
                {
                    if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0 && payment.FeeType.Id != (int)FeeTypes.ShortFall)
                    {
                        throw new Exception("Fee Details for " + payment.FeeType.Name + " not set! please contact your system administrator.");
                    }

                    decimal amount = (decimal)paymentEtranzact.TransactionAmount;
                    receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, paymentEtranzact, amount, payment.FeeType.Name);
                }
                else
                {
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelsBy(o => o.Payment_Id == payment.Id).FirstOrDefault();
                    if (remitaPayment != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.ToLower().Contains("manual")))
                    {
                        decimal amount = remitaPayment.TransactionAmount;
                        Abundance_Nk.Model.Model.Student student = new Model.Model.Student();
                        StudentLogic studentLogic = new StudentLogic();
                        student = studentLogic.GetBy(payment.Person.Id);

                        ApplicationFormLogic formLogic = new ApplicationFormLogic();
                        ApplicationForm form = new ApplicationForm();

                        if (student == null)
                        {
                            form = formLogic.GetModelsBy(f => f.Person_Id == payment.Person.Id).LastOrDefault();

                            receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, "E-Wallet Deposit Receipt", form != null ? form.Number : "", "");
                        }
                        else
                        {
                            receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, "E-Wallet Deposit Receipt", student.MatricNumber, "");
                        }
                        
                        if (student == null)
                        {
                            if (form != null)
                            {
                                ApplicantLogic applicantLogic = new ApplicantLogic();
                                ApplicationFormView applicant = applicantLogic.GetBy(form.Id);
                                if (applicant != null)
                                {
                                    bool matricNoAssigned = studentLogic.AssignMatricNumber(applicant);
                                }
                            }
                        }

                    }
                }
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber)
        {
            try
            {
                Receipt receipt = new Receipt();
                receipt.Number = remitaPayment.OrderId;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = remitaPayment.RRR;
                receipt.Amount = remitaPayment.TransactionAmount;
                receipt.AmountInWords = "";
                receipt.Purpose = purpose;
                receipt.Date = DateTime.Now;
                receipt.ApplicationFormNumber = ApplicationFormNumber;
                receipt.MatricNumber = MatricNumber;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentEtranzact paymentEtranzact, decimal amount, string purpose)
        {
            try
            {
                Receipt receipt = new Receipt();
                receipt.Number = paymentEtranzact.ReceiptNo;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentEtranzact.ConfirmationNo;
                receipt.Amount = amount;
                receipt.AmountInWords = "";
                receipt.Purpose = purpose;
                receipt.Date = DateTime.Now;

                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult SessionReceipt(int sessionId, string personId)
        {
            Receipt receipt = null;

            try
            {
                long applicantId = Convert.ToInt64(Utility.Decrypt(personId));

                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                AdmissionList admissionList = admissionListLogic.GetModelsBy(s => s.APPLICATION_FORM.Person_Id == applicantId).LastOrDefault();

                PaymentLogic paymentLogic = new PaymentLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                _eWalletPaymentLogic = new EWalletPaymentLogic();
                
                List<EWalletPayment> eWalletPayments = _eWalletPaymentLogic.GetModelsBy(e => e.Person_Id == applicantId && e.Session_Id == sessionId);

                List<Payment> payments = eWalletPayments.Select(s => s.Payment).ToList();

                if (payments == null || payments.Count <= 0)
                {
                    SetMessage("No Payments found.", Message.Category.Error);
                    return View(receipt);
                }

                Payment mainPayment = new Payment();
                mainPayment.Person = admissionList.Form.Person;
                mainPayment.FeeDetails = new List<FeeDetail>();

                for (int i = 0; i < payments.Count; i++)
                {
                    Payment payment = payments[i];
                    RemitaPayment currentRemitaPayment = remitaPaymentLogic.GetModelsBy(o => o.Payment_Id == payment.Id).FirstOrDefault();

                    if (currentRemitaPayment != null && (currentRemitaPayment.Status.Contains("01") || currentRemitaPayment.Description.ToLower().Contains("manual")))
                    {
                        mainPayment.FeeDetails.Add(new FeeDetail { Fee = new Fee { Name = payment.InvoiceNumber, Amount = eWalletPayments.Where(p => p.Payment.Id == payment.Id).LastOrDefault().Amount } });
                    }
                }

                decimal amount = mainPayment.FeeDetails.Sum(f => f.Fee.Amount);
                RemitaPayment remitaPayment = new RemitaPayment { RRR = "E-Wallet", OrderId = "E-Wallet-" + applicantId, TransactionAmount = amount };

                receipt = BuildReceipt(mainPayment.Person.FullName, "", remitaPayment, amount, "E-Wallet Deposit Receipt", admissionList.Form.Number, "");

                receipt.barcodeImageUrl = "https://students.fedpolyado.edu.ng/Common/Credential/Receipt?pmid=" + applicantId;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }
    }
}