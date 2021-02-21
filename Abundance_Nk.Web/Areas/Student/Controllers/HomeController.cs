using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private Model.Model.Student _Student;
        private Model.Model.StudentLevel _StudentLevel;
        private StudentLevelLogic studentLevelLogic;
        private StudentLogic studentLogic;
        public HomeController()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["student"] != null)
                {
                    studentLogic = new StudentLogic();
                    _Student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
                    _Student = studentLogic.GetBy(_Student.Id);
                    studentLevelLogic = new StudentLevelLogic();
                    _StudentLevel = studentLevelLogic.GetBy(_Student.Id);
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                    //System.Web.HttpContext.Current.Response.Redirect("/Security/Account/Login");

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        // GET: Student/Home
        public ActionResult Index()
        {
            try
            {
                Model.Model.Student currentStudent = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
                currentStudent = studentLogic.GetBy(currentStudent.Id);
                UpdateStudentRRRPayments(currentStudent);
                CheckDuplicateCourseReg(currentStudent);
                ViewBag.Email = StudentEmail(currentStudent.Id);

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        private void CheckDuplicateCourseReg(Model.Model.Student student)
        {
            try
            {
                if (student != null)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                    List<CourseRegistration> courseRegistrations = courseRegistrationLogic.GetModelsBy(c => c.Person_Id == student.Id);

                    for (int i = 0; i < courseRegistrations.Count; i++)
                    {
                        CourseRegistration courseRegistration = courseRegistrations[i];
                        List<CourseRegistrationDetail> registrationDetails = courseRegistrationDetailLogic.GetModelsBy(c => c.Student_Course_Registration_Id == courseRegistration.Id);

                        List<long> existingCourses = new List<long>();

                        for (int j = 0; j < registrationDetails.Count; j++)
                        {
                            CourseRegistrationDetail courseRegistrationDetail = registrationDetails[j];

                            if (!existingCourses.Contains(courseRegistrationDetail.Course.Id))
                            {
                                existingCourses.Add(courseRegistrationDetail.Course.Id);
                            }
                            else
                            {
                                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();

                                using (TransactionScope scope = new TransactionScope())
                                {
                                    string operation = "DELETE";
                                    string action = "AUTOMATIC DUPLICATE FIX";
                                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();

                                    courseRegistrationDetailAudit.Action = action;
                                    courseRegistrationDetailAudit.Operation = operation;
                                    courseRegistrationDetailAudit.Client = client;
                                    courseRegistrationDetailAudit.User = new User(){ Id = 1 };
                                    courseRegistrationDetailAudit.Time = DateTime.Now;
                                    courseRegistrationDetailAudit.Course = courseRegistrationDetail.Course;
                                    courseRegistrationDetailAudit.CourseRegistration = courseRegistrationDetail.CourseRegistration;
                                    courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetail.CourseUnit;
                                    courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetail.SpecialCase;
                                    courseRegistrationDetailAudit.TestScore = courseRegistrationDetail.TestScore;
                                    courseRegistrationDetailAudit.ExamScore = courseRegistrationDetail.ExamScore;
                                    courseRegistrationDetailAudit.Mode = courseRegistrationDetail.Mode;
                                    courseRegistrationDetailAudit.Semester = courseRegistrationDetail.Semester;

                                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                                    courseRegistrationDetailLogic.Delete(c => c.Student_Course_Registration_Detail_Id == courseRegistrationDetail.Id);

                                    scope.Complete();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void UpdateStudentRRRPayments(Model.Model.Student student)
        {
            try
            {
                if (student != null)
                {
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    RemitaResponse remitaResponse = new RemitaResponse();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                    settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
                    string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);

                    List<RemitaPayment> remitaPayments = remitaPaymentLogic.GetModelsBy(m =>  m.PAYMENT.Person_Id == student.Id);

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

        public ActionResult Profile()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("FormAlt", "Registration", new { Area = "Student", sid = _Student.Id, pid = _StudentLevel.Programme.Id });
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Fees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Index", "Payment", new { Area = "Student", sid = _Student.Id });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult OtherFees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("OtherFees", "Payment", new { Area = "Student", sid = _Student.Id });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ExtraYearFees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Index", "ExtraYear", new { Area = "Student" });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult PaymentHistory()
        {
            var paymentHistory = new PaymentHistory();
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    var paymentLogic = new PaymentLogic();
                    paymentHistory.Payments = paymentLogic.GetBy(_Student);
                    List<Payment> List = new List<Payment>();

                    RemitaPayment remitaPayment = new RemitaPayment() { payment = new Payment() { Person = new Person() { Id = _Student.Id } } };
                    List<PaymentView> remitaPaymentViews = paymentLogic.GetBy(remitaPayment);

                    paymentHistory.Payments = paymentHistory.Payments ?? new List<PaymentView>();

                    if (remitaPaymentViews != null && remitaPaymentViews.Count > 0)
                    {
                        paymentHistory.Payments.AddRange(remitaPaymentViews);
                        if (paymentHistory.Payments.Count > 0)
                        {
                            for(int i = 0; i < paymentHistory.Payments.Count; i++)
                            {
                                var paymentId = paymentHistory.Payments[i].PaymentId;
                                SessionLogic sessionLogic = new SessionLogic();
                                var Payment=paymentLogic.GetModelsBy(f => f.Payment_Id == paymentId).FirstOrDefault();
                                List.Add(Payment);
                            }
                        }
                        paymentHistory.PaymentLists = new List<Payment>();
                        paymentHistory.PaymentLists = List;
                    }
                    
                    paymentHistory.Student = _Student;
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View(paymentHistory);
        }
        public ActionResult CourseRegistration()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("SelectCourseRegistrationSession", "Registration", new { Area = "Student" });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult PrintAdmissionLetter()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    ApplicationFormLogic formLogic = new ApplicationFormLogic();
                    ApplicationForm form = formLogic.GetModelsBy(f => f.Person_Id == _Student.Id).LastOrDefault();
                    if (form != null)
                    {
                        return RedirectToAction("AdmissionLetter", "Credential", new { Area = "Common", fid = Utility.Encrypt(form.Id.ToString()) });
                    }
                    else
                    {
                        SetMessage("Error! Application not found.", Message.Category.Error);
                        return View("Index");
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ELearning()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("SelectECourseRegistrationSession", "Registration", new { Area = "Student" });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ExtraYearRegistration()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Step_3", "ExtraYear", new { Area = "Student" });

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Result()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Check", "Result", new { Area = "Student"});

                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ChangePassword()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    StudentViewModel studentViewModel = new StudentViewModel();
                    studentViewModel.Student = _Student;
                    return View(studentViewModel);
                }
                else
                {
                    //FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(StudentViewModel studentViewModel)
        {
            try
            {
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.MobilePhone");
                if (ModelState.IsValid)
                {
                    var studentLogic = new StudentLogic();
                    var LoggedInUser = new Model.Model.Student();
                    LoggedInUser = studentLogic.GetModelBy(
                            u =>
                                u.Matric_Number == studentViewModel.Student.MatricNumber &&
                                u.Password_hash == studentViewModel.OldPassword);
                    if (LoggedInUser != null)
                    {
                        LoggedInUser.PasswordHash = studentViewModel.NewPassword;
                        studentLogic.ChangeUserPassword(LoggedInUser);
                        SetMessage("Password Changed successfully! Please keep password in a safe place", Message.Category.Information);
                        return RedirectToAction("Index", "Home", new { Area = "Student" });
                    }
                    SetMessage("Please log off and log in then try again.", Message.Category.Error);

                    return View(studentViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
        public ActionResult GenerateShortFallInvoice()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("GenerateShortFallInvoice", "Payment", new { Area = "Student"});
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult PayShortFallFee()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("PayShortFallFee", "Payment", new { Area = "Student" });
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Logon()
        {
            RegistrationLogonViewModel logonViewModel = new RegistrationLogonViewModel();
            ViewBag.Session = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }

        [HttpPost]
        public ActionResult Logon(RegistrationLogonViewModel viewModel)
        {
            Payment payment = new Payment();
            RegistrationLogonViewModel logonViewModel = new RegistrationLogonViewModel();

            try
            {
                if (viewModel.Session != null && viewModel.Session.Id != (int)Sessions._20172018)
                {
                    SetMessage("Registration has closed!", Message.Category.Error);
                    ViewBag.Session = viewModel.SessionSelectListItem;
                    return View(viewModel);
                }

                if (viewModel.ConfirmationOrderNumber.Length > 12)
                {
                    //Model.Model.Session session = new Model.Model.Session() { Id = 1 };
                    Model.Model.Session session = viewModel.Session;
                    FeeType feetype = new FeeType() { Id = (int)FeeTypes.SchoolFees };
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber, session, feetype);
                    if (payment != null && payment.Id > 0)
                    {
                        if (payment.FeeType.Id != (int)FeeTypes.SchoolFees && payment.FeeType.Id != (int)FeeTypes.CarryOverSchoolFees)
                        {
                            SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrderNumber + ") entered is not for School Fees payment! Please enter your School Fees Confirmation Order Number.", Message.Category.Error);
                            ViewBag.Session = logonViewModel.SessionSelectListItem;
                            return View(logonViewModel);
                        }
                    }
                }
                else
                {
                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment = remitaPaymentLogic.GetModelsBy(a => a.RRR == viewModel.ConfirmationOrderNumber).FirstOrDefault();
                    if (remitaPayment != null)
                    {
                        //Get status of transaction
                        RemitaSettings settings = new RemitaSettings();
                        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                        settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 2);
                        string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                        RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                        remitaPayementProcessor.GetTransactionStatus(remitaPayment.RRR, remitaVerifyUrl, 2);
                        remitaPayment = remitaPaymentLogic.GetModelsBy(a => a.RRR == viewModel.ConfirmationOrderNumber).FirstOrDefault();
                        if (remitaPayment != null && remitaPayment.Status.Contains("01:") && remitaPayment.payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                        {
                            payment = remitaPayment.payment;
                        }
                        else
                        {
                            SetMessage("Payment couldn't verified for this payment purpose!", Message.Category.Error);
                            ViewBag.Session = logonViewModel.SessionSelectListItem;
                            return View(logonViewModel);
                        }
                    }
                    else
                    {
                        SetMessage("Payment couldn't verified!", Message.Category.Error);
                        ViewBag.Session = logonViewModel.SessionSelectListItem;
                        return View(logonViewModel);
                    }
                }


            }
            catch (Exception ex)
            {
                if (ex.Message == "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.")
                {
                    string checkMessage = checkSchoolFeeShortFall(viewModel.ConfirmationOrderNumber, viewModel.Session, new FeeType() { Id = (int)FeeTypes.SchoolFees });
                    if (checkMessage == "True")
                    {
                        return RedirectToAction("GenerateShortFallInvoice", "Payment", new { area = "Student" });
                    }
                    else if (checkMessage == "False")
                    {
                        SetMessage("Kindly try again! ", Message.Category.Information);
                        ViewBag.Session = logonViewModel.SessionSelectListItem;
                        return View(logonViewModel);
                    }
                    else
                    {
                        SetMessage(checkMessage, Message.Category.Error);
                        ViewBag.Session = logonViewModel.SessionSelectListItem;
                        return View(logonViewModel);
                    }
                }

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                ViewBag.Session = logonViewModel.SessionSelectListItem;
                return View(logonViewModel);
            }

            SetMessage("School Fees payment has been confirmed, You can click on the 'Receipt' link to generate your receipt. ", Message.Category.Information);
            ViewBag.Session = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }
        private string checkSchoolFeeShortFall(string ConfirmationNumber, Model.Model.Session session, FeeType feeType)
        {
            try
            {
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                ShortFallLogic shortFallLogic = new ShortFallLogic();

                PaymentTerminal paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Session_Id == session.Id && p.Fee_Type_Id == feeType.Id);
                PaymentEtranzact etranzact = paymentEtranzactLogic.RetrieveEtranzactWebServicePinDetails(ConfirmationNumber, paymentTerminal);

                if (etranzact != null)
                {
                    Payment payment = paymentLogic.GetModelBy(p => p.Invoice_Number == etranzact.CustomerID.ToUpper().Trim());
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == session.Id).LastOrDefault();

                    if (studentLevel != null)
                    {
                        decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                        if (etranzact.TransactionAmount < AmountToPay)
                        {
                            ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.PAYMENT.Person_Id == payment.Person.Id && s.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall && s.PAYMENT.Session_Id == session.Id).LastOrDefault();

                            if (shortFall != null && (Convert.ToDecimal(shortFall.Amount) + etranzact.TransactionAmount == AmountToPay))
                            {
                                PaymentEtranzact etranzactShortFall = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == shortFall.Payment.Id);
                                if (etranzactShortFall != null)
                                {
                                    PaymentEtranzact etranzactToModify = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                                    if (etranzactToModify != null)
                                    {
                                        etranzactToModify.TransactionAmount = AmountToPay;
                                        paymentEtranzactLogic.Modify(etranzactToModify);

                                        return "False";
                                    }
                                }
                                else
                                {
                                    return "True";
                                }

                            }
                            else
                            {
                                return "True";
                            }
                        }
                        else if (etranzact.TransactionAmount == AmountToPay)
                        {
                            PaymentEtranzact etranzactToModify = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                            if (etranzactToModify != null)
                            {
                                etranzactToModify.TransactionAmount = AmountToPay;
                                paymentEtranzactLogic.Modify(etranzactToModify);

                                return "False";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "True";
        }
        public ActionResult Step_3()
        {
            RegistrationLogonViewModel logonViewModel = new RegistrationLogonViewModel();
            ViewBag.Session = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }
        //public ActionResult PayFees()
        //{
        //    try
        //    {
        //        if (_Student != null && _StudentLevel != null)
        //        {
        //            return RedirectToAction("Logon", "Registration", new { Area = "Student" });
        //        }
        //        else
        //        {
        //            FormsAuthentication.SignOut();
        //            RedirectToAction("Login", "Account", new { Area = "Security" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return RedirectToAction("Index");
        //}

        //public ActionResult OtherFees()
        //{
        //    try
        //    {
        //        if (_Student != null && _StudentLevel != null)
        //        {
        //            return RedirectToAction("OldFees", "Payment", new { Area = "Student", Detail = Utility.Encrypt(_Student.Id.ToString()) });

        //        }
        //        else
        //        {
        //            FormsAuthentication.SignOut();
        //            RedirectToAction("Login", "Account", new { Area = "Security" });
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return RedirectToAction("Index");

        //}

        //public ActionResult PaymentReceipt()
        //{
        //    try
        //    {
        //        if (_Student != null && _StudentLevel != null)
        //        {
        //            return RedirectToAction("PrintReceipt", "Registration", new { Area = "Student" });
        //        }
        //        else
        //        {
        //            FormsAuthentication.SignOut();
        //            RedirectToAction("Login", "Account", new { Area = "Security" });
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return RedirectToAction("Index");


        //}


        public ActionResult SaveModifiedPassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                _Student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;

                if (_Student == null)
                {
                    //FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }

                _Student = studentLogic.GetBy(_Student.Id);

                _Student.PasswordHash = newPassword;
                studentLogic.ChangeUserPassword(_Student);

                result.IsError = false;
                result.Message = "Password Changed successfully! Please keep password in a safe place.";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error! " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReprintInvoice()
        {
            var paymentHistory = new PaymentHistory();
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    var paymentLogic = new PaymentLogic();
                    paymentHistory = paymentLogic.GetStudentInvoices(_Student);
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("An Error occured. " + ex.Message, Message.Category.Error);
            }

            return View(paymentHistory);
        }
        public JsonResult GetSessionDetails()
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                Notifications notifications = new Notifications();
                NotificationsLogic notificationsLogic = new NotificationsLogic();
                notifications = notificationsLogic.GetModelsBy(n => n.Active).LastOrDefault();
                if (_Student != null && _StudentLevel != null)
                {
                    result.Session = _StudentLevel.Session.Name;
                    result.FullName = _Student.FullName;
                    result.Programme = _StudentLevel.Programme.Name;
                    result.Department = _StudentLevel.Department.Name;
                    result.Level = _StudentLevel.Level.Name;
                    result.Notification = notifications == null ? "Nothing to display at this time..." : notifications.Message;

                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    //List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == _StudentLevel.Department.Id && f.Fee_Type_Id == (int)FeeTypes.SchoolFees && f.Level_Id == _StudentLevel.Level.Id &&
                    //                                                        f.Payment_Mode_Id == (int)PaymentModes.Full && f.Programme_Id == _StudentLevel.Programme.Id && f.Session_Id == _StudentLevel.Session.Id);
                    List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == _StudentLevel.Department.Id && f.Fee_Type_Id == (int)FeeTypes.SchoolFees && f.Level_Id == _StudentLevel.Level.Id 
                                                                             && f.Programme_Id == _StudentLevel.Programme.Id && f.Session_Id == _StudentLevel.Session.Id);
                    if (feeDetails != null && feeDetails.Count > 0)
                        result.AmountDue = feeDetails.Sum(f => f.Fee.Amount).ToString();

                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    List<RemitaPayment> remitaPayments = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == _Student.Id && r.PAYMENT.Session_Id == _StudentLevel.Session.Id && r.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees &&
                                                                                    (r.Status.Contains("01") || r.Description.ToLower().Contains("manual")));
                    if (remitaPayments != null && remitaPayments.Count > 0)
                    {
                        List<RemitaPayment> walletPayments = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == _Student.Id && r.PAYMENT.Session_Id == _StudentLevel.Session.Id && (r.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall || r.PAYMENT.Fee_Type_Id==(int)FeeTypes.Ewallet_Shortfall) &&
                                                                                    (r.Status.Contains("01")||r.Description.ToLower().Contains("manual")) && r.Description.Contains("SCHOOL FEES - E-WALLET"));
                        result.AmountPaid = remitaPayments.Sum(r => r.TransactionAmount).ToString();
                        if(walletPayments!=null && walletPayments.Count > 0)
                        {
                            result.AmountPaid = (walletPayments.Sum(r => r.TransactionAmount) + remitaPayments.Sum(r => r.TransactionAmount)).ToString();
                        }
                        
                    }
                    else
                    {
                        List<RemitaPayment> walletPayments = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == _Student.Id && r.PAYMENT.Session_Id == _StudentLevel.Session.Id && (r.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall || r.PAYMENT.Fee_Type_Id == (int)FeeTypes.Ewallet_Shortfall) &&
                                                                                    r.Status.Contains("01") && r.Description.Contains("SCHOOL FEES - E-WALLET"));

                        if (walletPayments != null && walletPayments.Count > 0)
                            result.AmountPaid = walletPayments.Sum(r => r.TransactionAmount).ToString();
                    }

                    result.IsError = false;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Student is already logged out.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string StudentEmail(long studentId)
        {
            PersonLogic personLogic = new PersonLogic();
            if (studentId > 0)
            {
                var person = personLogic.GetModelsBy(f => f.Person_Id == studentId).FirstOrDefault();
                if (person?.Id > 0)
                {
                    return person.Email;
                }
            }

            return null;
        }
        public JsonResult UpdateEmail(string emailAddress)
        {
            JsonResultModel result = new JsonResultModel();
            Model.Model.Student currentStudent = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
            try
            {
                if (currentStudent?.Id > 0)
                {
                    PersonLogic personLogic = new PersonLogic();
                    var person = personLogic.GetModelsBy(f => f.Person_Id == currentStudent.Id).FirstOrDefault();
                    if (person?.Id > 0)
                    {
                        person.Email = emailAddress;
                        personLogic.Modify(person);
                        result.IsError = false;
                        result.Message = "You have successfully Updated your email address!";
                    }
                }

            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}