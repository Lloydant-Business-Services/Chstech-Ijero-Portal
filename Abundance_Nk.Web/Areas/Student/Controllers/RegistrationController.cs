using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Transactions;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Web.Script.Serialization;
using System.Web.Security;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : BaseController
    {
        private RegistrationViewModel viewModel;
        private RegistrationIndexViewModel indexViewModel;
        private RegistrationLogonViewModel logonViewModel;

        private const string FIRST_SITTING = "FIRST SITTING";
        private const string SECOND_SITTING = "SECOND SITTING";

        private PaymentLogic paymentLogic;
        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];

        public RegistrationController()
        {
            paymentLogic = new PaymentLogic();
            indexViewModel = new RegistrationIndexViewModel();
            logonViewModel = new RegistrationLogonViewModel();
        }

        public ActionResult Logon()
        {
            logonViewModel = new RegistrationLogonViewModel();
            ViewBag.Session = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }

        [HttpPost]
        public ActionResult Logon(RegistrationLogonViewModel viewModel)
        {
            Payment payment = new Payment();

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
                    payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber, session,
                        feetype);
                    if (payment != null && payment.Id > 0)
                    {
                        if (payment.FeeType.Id != (int)FeeTypes.SchoolFees &&
                            payment.FeeType.Id != (int)FeeTypes.CarryOverSchoolFees)
                        {
                            SetMessage(
                                "Confirmation Order Number (" + viewModel.ConfirmationOrderNumber +
                                ") entered is not for School Fees payment! Please enter your School Fees Confirmation Order Number.",
                                Message.Category.Error);
                            ViewBag.Session = logonViewModel.SessionSelectListItem;
                            return View(logonViewModel);
                        }
                    }
                }
                else
                {
                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment =
                        remitaPaymentLogic.GetModelsBy(a => a.RRR == viewModel.ConfirmationOrderNumber).FirstOrDefault();
                    if (remitaPayment != null)
                    {
                        //Get status of transaction
                        RemitaSettings settings = new RemitaSettings();
                        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                        settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 2);
                        string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                        RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                        remitaPayementProcessor.GetTransactionStatus(remitaPayment.RRR, remitaVerifyUrl, 2);
                        remitaPayment =
                            remitaPaymentLogic.GetModelsBy(a => a.RRR == viewModel.ConfirmationOrderNumber)
                                .FirstOrDefault();
                        if (remitaPayment != null && remitaPayment.Status.Contains("01:") &&
                            remitaPayment.payment.FeeType.Id == (int)FeeTypes.SchoolFees)
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
                    string checkMessage = checkSchoolFeeShortFall(viewModel.ConfirmationOrderNumber, viewModel.Session,
                        new FeeType() { Id = (int)FeeTypes.SchoolFees });
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

            TempData["Payment"] = payment;
            return RedirectToAction("Index", "Registration",
                new
                {
                    Area = "Student",
                    sid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Person.Id.ToString()),
                    sesId = viewModel.Session.Id
                });
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

                PaymentTerminal paymentTerminal =
                    paymentTerminalLogic.GetModelBy(p => p.Session_Id == session.Id && p.Fee_Type_Id == feeType.Id);
                PaymentEtranzact etranzact =
                    paymentEtranzactLogic.RetrieveEtranzactWebServicePinDetails(ConfirmationNumber, paymentTerminal);

                if (etranzact != null)
                {
                    Payment payment =
                        paymentLogic.GetModelBy(p => p.Invoice_Number == etranzact.CustomerID.ToUpper().Trim());
                    StudentLevel studentLevel =
                        studentLevelLogic.GetModelsBy(
                            s => s.Person_Id == payment.Person.Id && s.Session_Id == session.Id).LastOrDefault();

                    if (studentLevel != null)
                    {
                        decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department,
                            studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session,
                            payment.PaymentMode);
                        if (etranzact.TransactionAmount < AmountToPay)
                        {
                            ShortFall shortFall =
                                shortFallLogic.GetModelsBy(
                                    s =>
                                        s.PAYMENT.Person_Id == payment.Person.Id &&
                                        s.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall &&
                                        s.PAYMENT.Session_Id == session.Id).LastOrDefault();

                            if (shortFall != null &&
                                (Convert.ToDecimal(shortFall.Amount) + etranzact.TransactionAmount == AmountToPay))
                            {
                                PaymentEtranzact etranzactShortFall =
                                    paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == shortFall.Payment.Id);
                                if (etranzactShortFall != null)
                                {
                                    PaymentEtranzact etranzactToModify =
                                        paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == payment.Id);
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
                            PaymentEtranzact etranzactToModify =
                                paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == payment.Id);
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

        public ActionResult LogonCarryOver()
        {
            return View(logonViewModel);
        }

        public ActionResult Index(string sid, int sesId)
        {

            try
            {
                long stId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(sid));
                StudentLogic studentLogic = new StudentLogic();
                indexViewModel.Student = studentLogic.GetBy(stId);
                indexViewModel.Session = new Session() { Id = sesId };
                indexViewModel.Payment = (Payment)TempData.Peek("Payment");

                if (indexViewModel.Student != null && indexViewModel.Student.Id > 0)
                {
                    indexViewModel.isExtraYearStudent = false;
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    //indexViewModel.StudentLevel = studentLevelLogic.GetBy(indexViewModel.Student, indexViewModel.Session);
                    //indexViewModel.StudentLevel = studentLevelLogic.GetExtraYearBy(indexViewModel.Student.Id);
                    indexViewModel.StudentLevel =
                        studentLevelLogic.GetModelsBy(
                            s => s.Person_Id == indexViewModel.Student.Id && s.Session_Id == sesId).LastOrDefault();

                    if (indexViewModel.StudentLevel != null &&
                        indexViewModel.StudentLevel.Session.Id != indexViewModel.Session.Id)
                    {
                        List<StudentLevel> studentLevels =
                            studentLevelLogic.GetModelsBy(s => s.Person_Id == indexViewModel.Student.Id);
                        StudentLevel currentSessionLevel =
                            studentLevels.LastOrDefault(s => s.Session.Id == indexViewModel.Session.Id);
                        if (currentSessionLevel != null)
                        {
                            indexViewModel.StudentLevel = currentSessionLevel;
                        }
                        else
                        {
                            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                            PaymentEtranzact paymentEtranzact =
                                paymentEtranzactLogic.GetModelBy(
                                    p =>
                                        p.ONLINE_PAYMENT.PAYMENT.Session_Id == indexViewModel.Session.Id &&
                                        p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees &&
                                        p.ONLINE_PAYMENT.PAYMENT.Person_Id == indexViewModel.Student.Id);
                            if (paymentEtranzact != null)
                            {
                                StudentLevel newStudentLevel = indexViewModel.StudentLevel;
                                newStudentLevel.Session = indexViewModel.Session;
                                if (newStudentLevel.Level.Id == 1)
                                {
                                    newStudentLevel.Level = new Level() { Id = 2 };
                                }
                                else if (newStudentLevel.Level.Id == 3)
                                {
                                    newStudentLevel.Level = new Level() { Id = 4 };
                                }
                                else
                                {
                                    newStudentLevel.Level = indexViewModel.StudentLevel.Level;
                                }

                                StudentLevel createdStudentLevel = studentLevelLogic.Create(newStudentLevel);
                                indexViewModel.StudentLevel =
                                    studentLevelLogic.GetModelBy(s => s.Student_Level_Id == createdStudentLevel.Id);
                            }
                        }
                    }

                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment =
                        remitaPaymentLogic.GetModelsBy(
                            a =>
                                a.PAYMENT.Person_Id == indexViewModel.Student.Id && a.PAYMENT.Fee_Type_Id == 3 &&
                                a.Status.Contains("01:")).FirstOrDefault();

                    PaymentHistory paymentHistory = new PaymentHistory();
                    List<PaymentView> pastPaymentsRemita = new List<PaymentView>();
                    List<PaymentView> pastPaymentsEtranzact = new List<PaymentView>();

                    if (remitaPayment != null && remitaPayment.payment != null)
                    {
                        pastPaymentsRemita = paymentLogic.GetBy(remitaPayment);
                    }
                    pastPaymentsEtranzact = paymentLogic.GetBy(indexViewModel.Payment.Person);
                    //paymentHistory.Payments = paymentLogic.GetBy(remitaPayment);
                    paymentHistory.Payments = new List<PaymentView>();
                    if (pastPaymentsRemita.Count > 0)
                    {
                        paymentHistory.Payments.AddRange(pastPaymentsRemita);
                    }
                    if (pastPaymentsEtranzact.Count > 0)
                    {
                        paymentHistory.Payments.AddRange(pastPaymentsEtranzact);
                    }

                    paymentHistory.Student = indexViewModel.Student;

                    indexViewModel.PaymentHistory = paymentHistory;
                    if (paymentHistory.Payments == null || paymentHistory.Payments.Count <= 0)
                    {
                        SetMessage("No payment made yet! Kindly generate invoice, go to bank and make your payments",
                            Message.Category.Error);
                    }

                    StudentExtraYearSession extraYear = new StudentExtraYearSession();
                    StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
                    extraYear = extraYearLogic.GetBy(indexViewModel.Payment.Person.Id, indexViewModel.Payment.Session.Id);
                    if (extraYear != null)
                    {
                        indexViewModel.isExtraYearStudent = true;
                    }
                    else
                    {
                        indexViewModel.isExtraYearStudent = false;
                    }
                }
                else
                {
                    SetMessage("Student details not found in the system! Please contact your system administrator.",
                        Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["viewModel"] = indexViewModel;

            return View(indexViewModel);
        }

        public ActionResult SelectCourseRegistrationSession()
        {
            RegistrationIndexViewModel viewModel = (RegistrationIndexViewModel)TempData["viewModel"];
            try
            {
                if (viewModel == null && System.Web.HttpContext.Current.Session["student"] != null)
                {
                    var studentLogic = new StudentLogic();
                    Model.Model.Student student =
                        System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;

                    if (student == null)
                    {
                        return RedirectToAction("Login", "Account", new { Arear = "Security" });
                    }

                    student = studentLogic.GetBy(student.Id);

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);

                    viewModel = new RegistrationIndexViewModel();
                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevel;
                }

                indexViewModel.CourseRegistrationSession = new Session();
                ViewBag.Session = indexViewModel.SessionSelectListItem;
                ViewBag.Semesters = indexViewModel.SemesterSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData.Keep("viewModel");
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectCourseRegistrationSession(RegistrationIndexViewModel viewModel)
        {
            RegistrationIndexViewModel regIndexViewModel = (RegistrationIndexViewModel)TempData["viewModel"];

            try
            {
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                SessionSemester sessionSemester =
                    sessionSemesterLogic.GetModelsBy(
                        s =>
                            s.Session_Id == viewModel.CourseRegistrationSession.Id &&
                            s.Semester_Id == viewModel.Semester.Id).LastOrDefault();

                Model.Model.Student student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;

                if (student == null)
                {
                    return RedirectToAction("Login", "Account", new { Area = "Security" });
                }

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);

                regIndexViewModel = regIndexViewModel ?? new RegistrationIndexViewModel();

                regIndexViewModel.Student = student;
                regIndexViewModel.StudentLevel = studentLevel;

                if (sessionSemester == null)
                {
                    SetMessage("Sessionsemester was not set kindly contact your system administrator!",
                        Message.Category.Error);

                    ViewBag.Session = regIndexViewModel.SessionSelectListItem;
                    ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
                    return View(regIndexViewModel);
                }
                if (sessionSemester.RegistrationEnded == true)
                {
                    SetMessage("Course registration for this session and semester has been closed!",
                        Message.Category.Error);

                    ViewBag.Session = regIndexViewModel.SessionSelectListItem;
                    ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
                    return View(regIndexViewModel);
                }


                if (student.MaritalStatus == null || student.Title == null || student.BloodGroup == null)
                {
                    SetMessage(
                        "You have not filled/completed your student form, kindly fill your student form before registering for your courses!",
                        Message.Category.Error);

                    ViewBag.Session = regIndexViewModel.SessionSelectListItem;
                    ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
                    return View(regIndexViewModel);
                }

                //get payment status
                bool hasPaidFees = GetPaymentStatus(student, studentLevel, viewModel.CourseRegistrationSession, viewModel.Semester);
                if (!hasPaidFees)
                {
                    SetMessage("You have not paid school fee for this session. ", Message.Category.Error);
                    ViewBag.Session = regIndexViewModel.SessionSelectListItem;
                    ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
                    return View(regIndexViewModel);
                }

                //PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                //PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id && p.ONLINE_PAYMENT.PAYMENT.Session_Id 
                //                            == viewModel.CourseRegistrationSession.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees).LastOrDefault();
                //if (paymentEtranzact == null)
                //{
                //    SetMessage("You have not paid school fee for this session. ", Message.Category.Error);

                //    ViewBag.Session = regIndexViewModel.SessionSelectListItem;
                //    ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
                //    return View(regIndexViewModel);
                //}

                return RedirectToAction("Form", "CourseRegistration",
                    new { sid = Utility.Encrypt(viewModel.Student.Id.ToString()), ssid = sessionSemester.Id });

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = regIndexViewModel.SessionSelectListItem;
            ViewBag.Semesters = regIndexViewModel.SemesterSelectListItem;
            return View(regIndexViewModel);
        }

        private bool GetPaymentStatus(Model.Model.Student student, StudentLevel studentLevel, Session session, Semester semester)
        {
            bool hasPaidFees = false;
            try
            {
                if (session.Id == (int)Sessions._20172018)
                    return true;

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id && s.Session_Id == session.Id).LastOrDefault();
                if (studentLevel == null)
                {
                    return false;
                }

                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == studentLevel.Department.Id && f.Fee_Type_Id == (int)FeeTypes.SchoolFees && f.Level_Id == studentLevel.Level.Id
                                                                        && f.Programme_Id == studentLevel.Programme.Id && f.Session_Id == session.Id);
                decimal fullPayment = feeDetails.Where(f => f.PaymentMode.Id == (int)PaymentModes.Full).Sum(f => f.Fee.Amount);
                decimal firstInstallmentPayment = feeDetails.Where(f => f.PaymentMode.Id == (int)PaymentModes.FirstInstallment).Sum(f => f.Fee.Amount);
                decimal secondInstallmentPayment = feeDetails.Where(f => f.PaymentMode.Id == (int)PaymentModes.SecondInstallment).Sum(f => f.Fee.Amount);

                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                RemitaPayment remitaPaymentFull = remitaPaymentLogic.GetModelsBy(s => s.PAYMENT.Person_Id == student.Id && s.PAYMENT.Session_Id == session.Id && s.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees
                                                    && s.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Full && (s.Status.Contains("021") || s.Description.ToLower().Contains("manual"))).LastOrDefault();
                RemitaPayment remitaPaymentFirst = remitaPaymentLogic.GetModelsBy(s => s.PAYMENT.Person_Id == student.Id && s.PAYMENT.Session_Id == session.Id && s.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees
                                                    && s.PAYMENT.Payment_Mode_Id == (int)PaymentModes.FirstInstallment && (s.Status.Contains("021") || s.Description.ToLower().Contains("manual"))).LastOrDefault();
                RemitaPayment remitaPaymentSecond = remitaPaymentLogic.GetModelsBy(s => s.PAYMENT.Person_Id == student.Id && s.PAYMENT.Session_Id == session.Id && s.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees
                                                    && s.PAYMENT.Payment_Mode_Id == (int)PaymentModes.SecondInstallment && (s.Status.Contains("021") || s.Description.ToLower().Contains("manual"))).LastOrDefault();
                //Get school fees paid as Short fall
                List<RemitaPayment> remitaPaymentShortFalls = remitaPaymentLogic.GetModelsBy(s => s.PAYMENT.Person_Id == student.Id && s.PAYMENT.Session_Id == session.Id && s.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall
                                    && s.Description.Contains("School Fees") && s.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Full && (s.Status.Contains("021") || s.Description.ToLower().Contains("manual")));

                EWalletPaymentLogic walletPaymentLogic = new EWalletPaymentLogic();
                decimal totalWalletAmount = 0M;
                bool hasWalletPayment = false;
                List<EWalletPayment> walletPayments = walletPaymentLogic.GetModelsBy(w => (w.Student_Id == student.Id || w.Person_Id == student.Id) && w.Session_Id == session.Id && (w.Fee_Type_Id == (int)FeeTypes.SchoolFees || w.Fee_Type_Id == (int)FeeTypes.ShortFall));
                if (walletPayments.Count > 0)
                    hasWalletPayment = true;

                for (int i = 0; i < walletPayments.Count; i++)
                {
                    long currentPaymentId = walletPayments[i].Payment.Id;
                    RemitaPayment walletRemitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == currentPaymentId && (r.Status.Contains("021") || r.Description.ToLower().Contains("manual")));
                    if (walletRemitaPayment != null)
                    {
                        hasWalletPayment = true;
                        totalWalletAmount += walletRemitaPayment.TransactionAmount;
                    }
                }

                if (remitaPaymentFull == null && remitaPaymentFirst == null && remitaPaymentSecond == null && hasWalletPayment == false)
                {
                    return false;
                }

                if (hasWalletPayment)
                {
                    if (semester != null && semester.Id == (int)Semesters.FirstSemester)
                    {
                        if (studentLevel.Programme.Id == (int)Programmes.HNDPartTime || studentLevel.Programme.Id == (int)Programmes.NDPartTime)
                        {
                            decimal eightyPercentOfFirstInstallment = firstInstallmentPayment * (80M / 100M);
                            return totalWalletAmount >= eightyPercentOfFirstInstallment;
                        }
                        else if (studentLevel.Programme.Id == (int)Programmes.HNDEvening || studentLevel.Programme.Id == (int)Programmes.NDEveningFullTime)
                        {
                            decimal fiftyPercentOfFullPayment = fullPayment * (45M / 100M);
                            return totalWalletAmount >= fiftyPercentOfFullPayment;
                        }
                    }
                    if (semester != null && semester.Id == (int)Semesters.SecondSemester)
                    {
                        if (studentLevel.Programme.Id == (int)Programmes.HNDPartTime || studentLevel.Programme.Id == (int)Programmes.NDPartTime)
                        {
                            var amount = remitaPaymentShortFalls.Count > 0 ? remitaPaymentShortFalls.Sum(f => f.TransactionAmount) : 0;
                            var fullSchoolFeesPayment = remitaPaymentFull != null ? remitaPaymentFull.TransactionAmount : 0;
                            if (amount >= fullPayment)
                            {
                                return amount >= fullPayment;
                            }
                            else if (fullSchoolFeesPayment >= fullPayment)
                            {
                                return fullSchoolFeesPayment >= fullPayment;
                            }
                            else
                            {
                                return totalWalletAmount >= fullPayment;
                            }


                        }
                        else if (studentLevel.Programme.Id == (int)Programmes.HNDEvening || studentLevel.Programme.Id == (int)Programmes.NDEveningFullTime)
                        {
                            var amount = remitaPaymentShortFalls.Count > 0 ? remitaPaymentShortFalls.Sum(f => f.TransactionAmount) : 0;
                            var fullSchoolFeesPayment = remitaPaymentFull != null ? remitaPaymentFull.TransactionAmount : 0;
                            if (amount >= fullPayment)
                            {
                                return amount >= fullPayment;
                            }
                            else if (fullSchoolFeesPayment >= fullPayment)
                            {
                                return fullSchoolFeesPayment >= fullPayment;
                            }
                            else
                            {
                                return totalWalletAmount >= fullPayment;
                            }
                            //return totalWalletAmount >= fullPayment;
                        }
                    }
                }
                else
                {
                    if (remitaPaymentFull != null)
                        return true;

                    if (semester != null && semester.Id == (int)Semesters.FirstSemester)
                    {
                        if (remitaPaymentFirst != null)
                            return true;
                    }
                    if (semester != null && semester.Id == (int)Semesters.SecondSemester)
                    {
                        if (remitaPaymentFirst != null && remitaPaymentSecond != null)
                            return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return hasPaidFees;
        }

        public ActionResult SelectECourseRegistrationSession()
        {
            RegistrationIndexViewModel viewModel = (RegistrationIndexViewModel)TempData["viewModel"];
            try
            {
                if (viewModel == null && System.Web.HttpContext.Current.Session["student"] != null)
                {
                    var studentLogic = new StudentLogic();
                    Model.Model.Student student =
                        System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
                    student = studentLogic.GetBy(student.Id);

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);

                    viewModel = new RegistrationIndexViewModel();
                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevel;
                }

                indexViewModel.CourseRegistrationSession = new Session();
                ViewBag.Session = indexViewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData.Keep("viewModel");
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectECourseRegistrationSession(RegistrationIndexViewModel viewModel)
        {
            try
            {
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<StudentLevel> studentLevels =
                    studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Student.Id);
                SessionSemester sessionSemester =
                    sessionSemesterLogic.GetModelsBy(s => s.Session_Id == viewModel.CourseRegistrationSession.Id)
                        .LastOrDefault();

                bool inRegisteredSession = false;
                for (int i = 0; i < studentLevels.Count; i++)
                {
                    if (viewModel.CourseRegistrationSession.Id == studentLevels[i].Session.Id)
                    {
                        inRegisteredSession = true;
                    }
                }

                if (inRegisteredSession)
                {
                    return RedirectToAction("ELearning", "CourseRegistration",
                        new { sid = Utility.Encrypt(viewModel.Student.Id.ToString()), ssid = sessionSemester.Id });

                }
                else
                {
                    SetMessage("You have not registered for this session!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            RegistrationIndexViewModel regIndexViewModel = (RegistrationIndexViewModel)TempData["viewModel"];
            ViewBag.Session = regIndexViewModel.SessionSelectListItem;
            return View(regIndexViewModel);
        }


        public ActionResult Form(long sid, int pid)
        {
            RegistrationViewModel existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                PopulateAllDropDowns(pid);
                if (existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }

                viewModel.LoadStudentInformationFormBy(sid);
                if (viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    if (viewModel.Payment == null)
                    {
                        viewModel.Payment = (Payment)TempData.Peek("Payment");
                    }

                    SetSelectedSittingSubjectAndGrade(viewModel);
                    SetLgaIfExist(viewModel);
                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                    SetEntryAndStudyMode(viewModel);
                    SetDateOfBirth();

                    viewModel.Student.Type = new StudentType() { Id = (int)StudentType.EnumName.Returning };
                    if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                    {
                        //SetPreviousEducationStartDate();
                        //SetPreviousEducationEndDate();

                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }
                    else
                    {
                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }

                    //SetLastEmploymentStartDate();
                    //SetLastEmploymentEndDate();
                    //SetNdResultDateAwarded();
                    SetStudentAcademicInformationLevel();
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

            return View(viewModel);
        }

        public ActionResult FormAlt(long sid, int pid)
        {
            RegistrationViewModel existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                PopulateAllDropDowns(pid);
                if (existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }

                viewModel.LoadStudentInformationFormBy(sid);
                if (viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    if (viewModel.Payment == null)
                    {
                        viewModel.Payment = (Payment)TempData.Peek("Payment");
                    }

                    SetSelectedSittingSubjectAndGrade(viewModel);
                    SetLgaIfExist(viewModel);
                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                    SetEntryAndStudyMode(viewModel);
                    SetDateOfBirth();

                    viewModel.Student.Type = new StudentType() { Id = (int)StudentType.EnumName.Returning };
                    if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                    {
                        //SetPreviousEducationStartDate();
                        //SetPreviousEducationEndDate();

                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }
                    else
                    {
                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }

                    //SetLastEmploymentStartDate();
                    //SetLastEmploymentEndDate();
                    //SetNdResultDateAwarded();
                    SetStudentAcademicInformationLevel();
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetStudentUploadedPassport(viewModel);

            TempData["RegistrationViewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;
            //TempData.Remove("imageUrl");

            return View(viewModel);
        }

        private void SetStudentAcademicInformationLevel()
        {
            try
            {
                if (viewModel.StudentAcademicInformation.Level == null ||
                    viewModel.StudentAcademicInformation.Level.Id <= 0)
                {
                    if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                    {
                        viewModel.StudentAcademicInformation.Level = viewModel.StudentLevel.Level;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetNdResultDateAwarded()
        {
            try
            {
                //if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != null)
                if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != DateTime.MinValue)
                {
                    if (viewModel.StudentNdResult.YearAwarded.Id > 0 && viewModel.StudentNdResult.MonthAwarded.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentNdResult.YearAwarded.Id,
                            viewModel.StudentNdResult.MonthAwarded.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value() { Name = "--DD--" });
                        }

                        if (viewModel.StudentNdResult.DayAwarded != null && viewModel.StudentNdResult.DayAwarded.Id > 0)
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentNdResult.DayAwarded.Id);
                        }
                        else
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLastEmploymentStartDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.StartDate != null)
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.StartDate != DateTime.MinValue)
                {
                    if (viewModel.StudentEmploymentInformation.StartYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.StartMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.StartYear.Id,
                            viewModel.StudentEmploymentInformation.StartMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value() { Name = "--DD--" });
                        }

                        if (viewModel.StudentEmploymentInformation.StartDay != null &&
                            viewModel.StudentEmploymentInformation.StartDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.StartDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLastEmploymentEndDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.EndDate != null)
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.EndDate != DateTime.MinValue)
                {
                    if (viewModel.StudentEmploymentInformation.EndYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.EndMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.EndYear.Id,
                            viewModel.StudentEmploymentInformation.EndMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value() { Name = "--DD--" });
                        }

                        if (viewModel.StudentEmploymentInformation.EndDay != null &&
                            viewModel.StudentEmploymentInformation.EndDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.EndDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetEntryAndStudyMode(RegistrationViewModel vModel)
        {
            try
            {
                //set mode of entry

                switch (vModel.StudentLevel.Programme.Id)
                {
                    case 1:
                        {
                            vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = 3 };
                            break;
                        }
                    case 2:
                        {
                            vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = 2 };
                            break;
                        }
                    case 3:
                        {
                            vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = 4 };
                            break;
                        }
                    case 4:
                        {
                            vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = 1 };

                            break;
                        }
                }

                //set mode of study
                switch (vModel.StudentLevel.Programme.Id)
                {
                    case 1:
                    case 3:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy() { Id = 1 };
                            break;
                        }
                    case 2:
                    case 4:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy() { Id = 2 };
                            break;
                        }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Form(RegistrationViewModel viewModel)
        {
            try
            {
                //SetStudentUploadedPassport(viewModel);
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Payment.Id");



                if (ModelState.IsValid)
                {
                    //if (string.IsNullOrEmpty(viewModel.Person.ImageFileUrl) || viewModel.Person.ImageFileUrl == Utility.DEFAULT_AVATAR)
                    //{
                    //    SetMessage("No Passport uploaded! Please upload your passport to continue.", Message.Category.Error);
                    //    SetStateVariables(viewModel);
                    //    return View(viewModel);
                    //}

                    TempData["RegistrationViewModel"] = viewModel;
                    return RedirectToAction("FormPreview", "Registration");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetStateVariables(viewModel);
            return View(viewModel);
        }

        private void SetStateVariables(RegistrationViewModel viewModel)
        {
            try
            {
                TempData["RegistrationViewModel"] = viewModel;
                TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

                PopulateAllDropDowns(viewModel.StudentLevel.Programme.Id);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        public ActionResult FormPreview()
        {
            RegistrationViewModel viewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                if (viewModel != null)
                {
                    viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                        viewModel.Person.MonthOfBirth.Id, viewModel.Person.DayOfBirth.Id);
                    viewModel.Person.State =
                        viewModel.States.Where(m => m.Id == viewModel.Person.State.Id).SingleOrDefault();
                    viewModel.Person.LocalGovernment =
                        viewModel.Lgas.Where(m => m.Id == viewModel.Person.LocalGovernment.Id).SingleOrDefault();
                    viewModel.Person.Sex =
                        viewModel.Genders.Where(m => m.Id == viewModel.Person.Sex.Id).SingleOrDefault();
                    viewModel.NextOfKin.Relationship =
                        viewModel.Relationships.Where(m => m.Id == viewModel.NextOfKin.Relationship.Id)
                            .SingleOrDefault();
                    viewModel.Person.Religion =
                        viewModel.Religions.Where(m => m.Id == viewModel.Person.Religion.Id).SingleOrDefault();
                    viewModel.Student.Title =
                        viewModel.Titles.Where(m => m.Id == viewModel.Student.Title.Id).SingleOrDefault();
                    viewModel.Student.MaritalStatus =
                        viewModel.MaritalStatuses.Where(m => m.Id == viewModel.Student.MaritalStatus.Id)
                            .SingleOrDefault();

                    if (viewModel.Student.BloodGroup != null && viewModel.Student.BloodGroup.Id > 0)
                    {
                        viewModel.Student.BloodGroup =
                            viewModel.BloodGroups.Where(m => m.Id == viewModel.Student.BloodGroup.Id).SingleOrDefault();
                    }
                    if (viewModel.Student.Genotype != null && viewModel.Student.Genotype.Id > 0)
                    {
                        viewModel.Student.Genotype =
                            viewModel.Genotypes.Where(m => m.Id == viewModel.Student.Genotype.Id).SingleOrDefault();
                    }

                    viewModel.StudentAcademicInformation.ModeOfEntry =
                        viewModel.ModeOfEntries.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfEntry.Id)
                            .SingleOrDefault();
                    viewModel.StudentAcademicInformation.ModeOfStudy =
                        viewModel.ModeOfStudies.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfStudy.Id)
                            .SingleOrDefault();
                    viewModel.Student.Category =
                        viewModel.StudentCategories.Where(m => m.Id == viewModel.Student.Category.Id).SingleOrDefault();
                    viewModel.Student.Type =
                        viewModel.StudentTypes.Where(m => m.Id == viewModel.Student.Type.Id).SingleOrDefault();
                    viewModel.StudentAcademicInformation.Level =
                        viewModel.Levels.Where(m => m.Id == viewModel.StudentAcademicInformation.Level.Id)
                            .SingleOrDefault();
                    viewModel.StudentFinanceInformation.Mode =
                        viewModel.ModeOfFinances.Where(m => m.Id == viewModel.StudentFinanceInformation.Mode.Id)
                            .SingleOrDefault();
                    viewModel.StudentSponsor.Relationship =
                        viewModel.Relationships.Where(m => m.Id == viewModel.StudentSponsor.Relationship.Id)
                            .SingleOrDefault();

                    viewModel.FirstSittingOLevelResult.Type =
                        viewModel.OLevelTypes.Where(m => m.Id == viewModel.FirstSittingOLevelResult.Type.Id)
                            .SingleOrDefault();
                    if (viewModel.SecondSittingOLevelResult.Type != null)
                    {
                        viewModel.SecondSittingOLevelResult.Type =
                            viewModel.OLevelTypes.Where(m => m.Id == viewModel.SecondSittingOLevelResult.Type.Id)
                                .SingleOrDefault();
                    }

                    if (viewModel.StudentLevel.DepartmentOption == null ||
                        viewModel.StudentLevel.DepartmentOption.Id <= 0)
                    {
                        viewModel.StudentLevel.DepartmentOption = new DepartmentOption() { Id = 1 };
                        viewModel.StudentLevel.DepartmentOption.Name = viewModel.StudentLevel.Department.Name;
                    }

                    //if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                    //{
                    //    viewModel.PreviousEducation.StartDate = new DateTime(viewModel.PreviousEducation.StartYear.Id, viewModel.PreviousEducation.StartMonth.Id, viewModel.PreviousEducation.StartDay.Id);
                    //    viewModel.PreviousEducation.EndDate = new DateTime(viewModel.PreviousEducation.EndYear.Id, viewModel.PreviousEducation.EndMonth.Id, viewModel.PreviousEducation.EndDay.Id);
                    //    viewModel.StudentEmploymentInformation.StartDate = new DateTime(viewModel.StudentEmploymentInformation.StartYear.Id, viewModel.StudentEmploymentInformation.StartMonth.Id, viewModel.StudentEmploymentInformation.StartDay.Id);
                    //    viewModel.StudentEmploymentInformation.EndDate = new DateTime(viewModel.StudentEmploymentInformation.EndYear.Id, viewModel.StudentEmploymentInformation.EndMonth.Id, viewModel.StudentEmploymentInformation.EndDay.Id);
                    //    viewModel.StudentNdResult.DateAwarded = new DateTime(viewModel.StudentNdResult.YearAwarded.Id, viewModel.StudentNdResult.MonthAwarded.Id, viewModel.StudentNdResult.DayAwarded.Id);
                    //    viewModel.PreviousEducation.ResultGrade = viewModel.ResultGrades.Where(m => m.Id == viewModel.PreviousEducation.ResultGrade.Id).SingleOrDefault();
                    //}

                    UpdateOLevelResultDetail(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FormPreview(RegistrationViewModel vm)
        {
            Abundance_Nk.Model.Model.Student newStudent = null;
            RegistrationViewModel viewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];
            PersonType personType = new PersonType() { Id = (int)PersonType.EnumName.Student };

            try
            {
                if (viewModel.StudentAlreadyExist == false)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus() { Id = 1 };
                        StudentLogic studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);


                        viewModel.StudentSponsor.Student = newStudent;
                        StudentSponsorLogic sponsorLogic = new StudentSponsorLogic();
                        if (sponsorLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            sponsorLogic.Modify(viewModel.StudentSponsor);
                        }
                        else
                        {
                            sponsorLogic.Create(viewModel.StudentSponsor);
                        }



                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType() { Id = (int)PersonType.EnumName.Student };
                        NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                        if (nextOfKinLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            nextOfKinLogic.Modify(viewModel.NextOfKin);
                        }
                        else
                        {
                            nextOfKinLogic.Create(viewModel.NextOfKin);
                        }


                        if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            viewModel.FirstSittingOLevelResult.Person = viewModel.Person;
                            viewModel.FirstSittingOLevelResult.PersonType = personType;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
                        }

                        if (viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            viewModel.SecondSittingOLevelResult.Person = viewModel.Person;
                            viewModel.SecondSittingOLevelResult.PersonType = personType;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
                        }

                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult, viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);

                        viewModel.StudentAcademicInformation.Student = newStudent;
                        StudentAcademicInformationLogic academicInformationLogic = new StudentAcademicInformationLogic();
                        if (academicInformationLogic.GetModelBy(a => a.Person_Id == newStudent.Id) != null)
                        {
                            academicInformationLogic.Modify(viewModel.StudentAcademicInformation);
                        }
                        else
                        {
                            academicInformationLogic.Create(viewModel.StudentAcademicInformation);
                        }


                        viewModel.StudentFinanceInformation.Student = newStudent;
                        StudentFinanceInformationLogic financeInformationLogic = new StudentFinanceInformationLogic();
                        if (financeInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() !=
                            null)
                        {
                            financeInformationLogic.Modify(viewModel.StudentFinanceInformation);
                        }
                        else
                        {
                            financeInformationLogic.Create(viewModel.StudentFinanceInformation);
                        }



                        //if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                        //{
                        //    ITDuration duration = new ITDuration() { Id = 1 };
                        //    EducationalQualification qualification = new EducationalQualification() { Id = 45 };
                        //    viewModel.PreviousEducation.Person = newStudent;
                        //    viewModel.PreviousEducation.PersonType = personType;
                        //    viewModel.PreviousEducation.ITDuration = duration;
                        //    viewModel.PreviousEducation.Qualification = qualification;
                        //    PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
                        //    if (previousEducationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        //    {
                        //        previousEducationLogic.Modify(viewModel.PreviousEducation);
                        //    }
                        //    else
                        //    {
                        //        previousEducationLogic.Create(viewModel.PreviousEducation);
                        //    }


                        //    viewModel.StudentEmploymentInformation.Student = newStudent;
                        //    StudentEmploymentInformationLogic employmentInformationLogic = new StudentEmploymentInformationLogic();
                        //    if (employmentInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        //    {
                        //        employmentInformationLogic.Modify(viewModel.StudentEmploymentInformation);
                        //    }
                        //    else
                        //    {
                        //        employmentInformationLogic.Create(viewModel.StudentEmploymentInformation);

                        //    }

                        //    viewModel.StudentNdResult.Student = newStudent;
                        //    StudentNdResultLogic ndResultLogic = new StudentNdResultLogic();
                        //    if (ndResultLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        //    {
                        //        ndResultLogic.Modify(viewModel.StudentNdResult);
                        //    }
                        //    else
                        //    {
                        //        ndResultLogic.Create(viewModel.StudentNdResult);
                        //    }

                        //}


                        //string junkFilePath;
                        //string destinationFilePath;
                        //SetPersonPassportDestination(viewModel, out junkFilePath, out destinationFilePath);

                        PersonLogic personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);
                        if (personModified)
                        {
                            //SavePersonPassport(junkFilePath, destinationFilePath, viewModel.Person);
                            transaction.Complete();
                        }
                        else
                        {
                            throw new Exception("Passport save operation failed! Please try again.");
                        }

                        //transaction.Complete();
                    }
                }
                else
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus() { Id = 1 };
                        StudentLogic studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);


                        viewModel.StudentSponsor.Student = newStudent;
                        StudentSponsorLogic sponsorLogic = new StudentSponsorLogic();
                        sponsorLogic.Modify(viewModel.StudentSponsor);


                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType() { Id = (int)PersonType.EnumName.Student };
                        NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                        nextOfKinLogic.Modify(viewModel.NextOfKin);


                        if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            viewModel.FirstSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
                        }

                        if (viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            viewModel.SecondSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
                        }
                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult, viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);


                        viewModel.StudentAcademicInformation.Student = newStudent;
                        StudentAcademicInformationLogic academicInformationLogic = new StudentAcademicInformationLogic();
                        academicInformationLogic.Modify(viewModel.StudentAcademicInformation);

                        viewModel.StudentFinanceInformation.Student = newStudent;
                        StudentFinanceInformationLogic financeInformationLogic = new StudentFinanceInformationLogic();
                        financeInformationLogic.Modify(viewModel.StudentFinanceInformation);

                        //if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                        //{
                        //    viewModel.PreviousEducation.Person = newStudent;
                        //    viewModel.PreviousEducation.PersonType = personType;
                        //    PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
                        //    previousEducationLogic.Modify(viewModel.PreviousEducation);

                        //    viewModel.StudentEmploymentInformation.Student = newStudent;
                        //    StudentEmploymentInformationLogic employmentInformationLogic = new StudentEmploymentInformationLogic();
                        //    employmentInformationLogic.Modify(viewModel.StudentEmploymentInformation);

                        //}


                        PersonLogic personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);
                        transaction.Complete();
                    }
                }
                TempData["RegistrationViewModel"] = viewModel;
                return RedirectToAction("AcknowledgementSlip", "Registration");
            }
            catch (Exception ex)
            {
                newStudent = null;
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            return View(viewModel);
        }

        private void SaveOLevelResult(Person person, OLevelResult oLevelResult,
            List<OLevelResultDetail> oLevelResultDetails, PersonType personType)
        {
            try
            {
                OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    //oLevelResult.ApplicationForm = applicationForm;
                    oLevelResult.Person = person;
                    oLevelResult.PersonType = personType;
                    //oLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };

                    OLevelResult firstSittingOLevelResult = oLevelResultLogic.Create(oLevelResult);

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0 && firstSittingOLevelResult != null)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(m => m.Grade != null && m.Subject != null).ToList();
                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = firstSittingOLevelResult;
                        }

                        oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SavePersonPassport(string junkFilePath, string pathForSaving, Person person)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(pathForSaving);
                string mainFileName = person.Id.ToString() + "__";

                DeleteFileIfExist(folderPath, mainFileName);

                System.IO.File.Move(junkFilePath, pathForSaving);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPersonPassportDestination(RegistrationViewModel viewModel, out string junkFilePath,
            out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                string passportUrl = viewModel.Person.ImageFileUrl;
                junkFilePath = Server.MapPath(TILDA + viewModel.Person.ImageFileUrl);
                destinationFilePath = junkFilePath.Replace("Junk", "Student");
                viewModel.Person.ImageFileUrl = passportUrl.Replace("Junk", "Student");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public ActionResult AcknowledgementSlip()
        //{
        //    RegistrationViewModel existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

        //    TempData["RegistrationViewModel"] = existingViewModel;
        //    return View(existingViewModel);
        //}
        public ActionResult AcknowledgementSlip(long sId)
        {
            RegistrationViewModel existingViewModel = new RegistrationViewModel();
            try
            {
                //List<OLevelResultDetail> extractList = new List<OLevelResultDetail>();


                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PersonLogic personLogic = new PersonLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();
                NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                StudentSponsorLogic studentSponsorLogic = new StudentSponsorLogic();
                OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                OLevelResultDetail oLevelResultDetail = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail2 = new OLevelResultDetail();
                OLevelResult oLevelResult = new OLevelResult();
                OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                existingViewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == sId);
                //existingViewModel.StudentLevel = studentLevelLogic.GetModelBy(s => s.Person_Id == sId);
                existingViewModel.StudentLevel = studentLevelLogic.GetBy(sId);
                existingViewModel.Person = personLogic.GetModelBy(s => s.Person_Id == sId);
                existingViewModel.NextOfKin = nextOfKinLogic.GetModelBy(s => s.Person_Id == sId);
                existingViewModel.StudentAcademicInformation = studentAcademicInformationLogic.GetModelBy(s => s.Person_Id == sId);
                existingViewModel.StudentSponsor = studentSponsorLogic.GetModelBy(s => s.Person_Id == sId);
                var oLevelResultFirst = oLevelResultLogic.GetModelsBy(s => s.Person_Id == sId && s.O_Level_Exam_Sitting_Id == 1).LastOrDefault();
                var oLevelResultSecond = oLevelResultLogic.GetModelsBy(s => s.Person_Id == sId && s.O_Level_Exam_Sitting_Id == 2).LastOrDefault();

                if (oLevelResultFirst != null)
                {
                    existingViewModel.FirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(s => s.Applicant_O_Level_Result_Id == oLevelResultFirst.Id);
                }
                //existingViewModel.FirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelBy(s => s.Applicant_O_Level_Result_Id == oLevelResult.Id);

                //existingViewModel.FirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(s => s.APPLICANT_O_LEVEL_RESULT.Person_Id == sId && s.APPLICANT_O_LEVEL_RESULT.O_Level_Exam_Sitting_Id == 1);
                //existingViewModel.SecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(s => s.APPLICANT_O_LEVEL_RESULT.Person_Id == sId && s.APPLICANT_O_LEVEL_RESULT.O_Level_Exam_Sitting_Id == 2);

                if (oLevelResultSecond != null)
                {
                    existingViewModel.SecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(s => s.Applicant_O_Level_Result_Id == oLevelResultSecond.Id);
                }

                if (existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    existingViewModel.FirstSittingOLevelResult = existingViewModel.FirstSittingOLevelResultDetails[0].Header;
                }

                if (existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    existingViewModel.SecondSittingOLevelResult = existingViewModel.SecondSittingOLevelResultDetails[0].Header;
                }


            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = existingViewModel;
            return View(existingViewModel);
        }

        private void UpdateOLevelResultDetail(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null &&
                    viewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    foreach (
                        OLevelResultDetail firstSittingOLevelResultDetail in viewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null)
                        {
                            firstSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == firstSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if (firstSittingOLevelResultDetail.Grade != null)
                        {
                            firstSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == firstSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }

                if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null &&
                    viewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    foreach (
                        OLevelResultDetail secondSittingOLevelResultDetail in viewModel.SecondSittingOLevelResultDetails
                        )
                    {
                        if (secondSittingOLevelResultDetail.Subject != null)
                        {
                            secondSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == secondSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if (secondSittingOLevelResultDetail.Grade != null)
                        {
                            secondSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == secondSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetStudentUploadedPassport(RegistrationViewModel viewModel)
        {
            if (viewModel != null && viewModel.Person != null && !string.IsNullOrEmpty((string)TempData["imageUrl"]))
            {
                viewModel.Person.ImageFileUrl = (string)TempData["imageUrl"];
            }
            else if (viewModel != null && viewModel.Person != null &&
                     !string.IsNullOrEmpty(viewModel.Person.ImageFileUrl))
            {
                viewModel.Person.ImageFileUrl = viewModel.Person.ImageFileUrl;
            }
            else
            {
                viewModel.Person.ImageFileUrl = Utility.DEFAULT_AVATAR;
            }
        }

        private void PopulateAllDropDowns(int programmeId)
        {
            RegistrationViewModel existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                if (existingViewModel == null)
                {
                    viewModel = new RegistrationViewModel();

                    ViewBag.States = viewModel.StateSelectList;
                    ViewBag.Sexes = viewModel.SexSelectList;
                    ViewBag.FirstChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.SecondChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
                    ViewBag.Relationships = viewModel.RelationshipSelectList;
                    ViewBag.FirstSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.Religions = viewModel.ReligionSelectList;
                    ViewBag.Abilities = viewModel.AbilitySelectList;
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    ViewBag.MonthOfBirths = viewModel.MonthOfBirthSelectList;
                    ViewBag.YearOfBirths = viewModel.YearOfBirthSelectList;
                    ViewBag.Titles = viewModel.TitleSelectList;
                    ViewBag.MaritalStatuses = viewModel.MaritalStatusSelectList;
                    ViewBag.BloodGroups = viewModel.BloodGroupSelectList;
                    ViewBag.Genotypes = viewModel.GenotypeSelectList;
                    ViewBag.ModeOfEntries = viewModel.ModeOfEntrySelectList;
                    ViewBag.ModeOfStudies = viewModel.ModeOfStudySelectList;
                    ViewBag.StudentCategories = viewModel.StudentCategorySelectList;
                    ViewBag.StudentTypes = viewModel.StudentTypeSelectList;
                    ViewBag.Levels = viewModel.LevelSelectList;
                    ViewBag.ModeOfFinances = viewModel.ModeOfFinanceSelectList;
                    ViewBag.Relationships = viewModel.RelationshipSelectList;
                    ViewBag.Faculties = viewModel.FacultySelectList;
                    ViewBag.AdmissionYears = viewModel.AdmissionYearSelectList;
                    ViewBag.GraduationYears = viewModel.GraduationYearSelectList;
                    ViewBag.Programmes = viewModel.ProgrammeSelectList;



                    if (viewModel.DepartmentSelectList != null)
                    {
                        ViewBag.Departments = viewModel.DepartmentSelectList;
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    }

                    //if (programmeId == 3 || programmeId == 4)
                    //{
                    //    ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    //    ViewBag.StudentNdResultMonthAwardeds = viewModel.StudentNdResultMonthAwardedSelectList;
                    //    ViewBag.StudentNdResultYearAwardeds = viewModel.StudentNdResultYearAwardedSelectList;

                    //    ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    //    ViewBag.StudentLastEmploymentStartMonths = viewModel.StudentLastEmploymentStartMonthSelectList;
                    //    ViewBag.StudentLastEmploymentStartYears = viewModel.StudentLastEmploymentStartYearSelectList;

                    //    ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    //    ViewBag.StudentLastEmploymentEndMonths = viewModel.StudentLastEmploymentEndMonthSelectList;
                    //    ViewBag.StudentLastEmploymentEndYears = viewModel.StudentLastEmploymentEndYearSelectList;

                    //    ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    //    ViewBag.PreviousEducationStartMonths = viewModel.PreviousEducationStartMonthSelectList;
                    //    ViewBag.PreviousEducationStartYears = viewModel.PreviousEducationStartYearSelectList;

                    //    ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    //    ViewBag.PreviousEducationEndMonths = viewModel.PreviousEducationEndMonthSelectList;
                    //    ViewBag.PreviousEducationEndYears = viewModel.PreviousEducationEndYearSelectList;

                    //    ViewBag.ResultGrades = viewModel.ResultGradeSelectList;
                    //}

                    SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                }
                else
                {
                    if (existingViewModel.Student.Title == null)
                    {
                        existingViewModel.Student.Title = new Title();
                    }
                    if (existingViewModel.Person.Sex == null)
                    {
                        existingViewModel.Person.Sex = new Sex();
                    }
                    if (existingViewModel.Student.MaritalStatus == null)
                    {
                        existingViewModel.Student.MaritalStatus = new MaritalStatus();
                    }
                    if (existingViewModel.Person.Religion == null)
                    {
                        existingViewModel.Person.Religion = new Religion();
                    }
                    if (existingViewModel.Person.State == null)
                    {
                        existingViewModel.Person.State = new State();
                    }
                    if (existingViewModel.StudentLevel.Programme == null)
                    {
                        existingViewModel.StudentLevel.Programme = new Programme();
                    }
                    if (existingViewModel.NextOfKin.Relationship == null)
                    {
                        existingViewModel.NextOfKin.Relationship = new Relationship();
                    }
                    if (existingViewModel.StudentSponsor.Relationship == null)
                    {
                        existingViewModel.StudentSponsor.Relationship = new Relationship();
                    }
                    if (existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }
                    if (existingViewModel.Person.YearOfBirth == null)
                    {
                        existingViewModel.Person.YearOfBirth = new Value();
                    }
                    if (existingViewModel.Person.MonthOfBirth == null)
                    {
                        existingViewModel.Person.MonthOfBirth = new Value();
                    }
                    if (existingViewModel.Person.DayOfBirth == null)
                    {
                        existingViewModel.Person.DayOfBirth = new Value();
                    }
                    if (existingViewModel.StudentLevel.Department == null)
                    {
                        existingViewModel.StudentLevel.Department = new Department();
                    }
                    if (existingViewModel.Student.BloodGroup == null)
                    {
                        existingViewModel.Student.BloodGroup = new BloodGroup();
                    }
                    if (existingViewModel.Student.Genotype == null)
                    {
                        existingViewModel.Student.Genotype = new Genotype();
                    }
                    if (existingViewModel.StudentAcademicInformation.Level == null)
                    {
                        existingViewModel.StudentAcademicInformation.Level = new Level();
                    }
                    if (existingViewModel.StudentFinanceInformation.Mode == null)
                    {
                        existingViewModel.StudentFinanceInformation.Mode = new ModeOfFinance();
                    }


                    // PERSONAL INFORMATION
                    ViewBag.Titles = new SelectList(existingViewModel.TitleSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Title.Id);
                    ViewBag.Sexes = new SelectList(existingViewModel.SexSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.Sex.Id);
                    ViewBag.MaritalStatuses = new SelectList(existingViewModel.MaritalStatusSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.MaritalStatus.Id);
                    SetDateOfBirthDropDown(existingViewModel);
                    ViewBag.Religions = new SelectList(existingViewModel.ReligionSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.Religion.Id);
                    ViewBag.States = new SelectList(existingViewModel.StateSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.State.Id);

                    if (existingViewModel.Person.LocalGovernment != null &&
                        existingViewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(existingViewModel.LocalGovernmentSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.VALUE, Utility.TEXT);
                    }
                    ViewBag.BloodGroups = new SelectList(existingViewModel.BloodGroupSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.BloodGroup.Id);
                    ViewBag.Genotypes = new SelectList(existingViewModel.GenotypeSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Genotype.Id);



                    // ACADEMIC DETAILS
                    ViewBag.ModeOfEntries = new SelectList(existingViewModel.ModeOfEntrySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfEntry.Id);
                    ViewBag.ModeOfStudies = new SelectList(existingViewModel.ModeOfStudySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfStudy.Id);
                    ViewBag.Programmes = new SelectList(existingViewModel.ProgrammeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentLevel.Programme.Id);
                    ViewBag.Faculties = new SelectList(existingViewModel.FacultySelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentLevel.Department.Faculty.Id);

                    SetDepartmentIfExist(existingViewModel);
                    SetDepartmentOptionIfExist(existingViewModel);

                    ViewBag.StudentTypes = new SelectList(existingViewModel.StudentTypeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.Type.Id);
                    ViewBag.StudentCategories = new SelectList(existingViewModel.StudentCategorySelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.Student.Category.Id);
                    ViewBag.AdmissionYears = new SelectList(existingViewModel.AdmissionYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfAdmission);
                    ViewBag.GraduationYears = new SelectList(existingViewModel.GraduationYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfGraduation);

                    if (existingViewModel.StudentAcademicInformation.Level != null &&
                        existingViewModel.StudentAcademicInformation.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE, Utility.TEXT,
                            existingViewModel.StudentAcademicInformation.Level.Id);
                    }
                    else if (existingViewModel.StudentLevel.Level != null && existingViewModel.StudentLevel.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentLevel.Level.Id);
                    }

                    // FINANCE DETAILS
                    ViewBag.ModeOfFinances = new SelectList(existingViewModel.ModeOfFinanceSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentFinanceInformation.Mode.Id);

                    // NEXT OF KIN
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentSponsor.Relationship.Id);

                    //SPONSOR
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.NextOfKin.Relationship.Id);


                    ViewBag.FirstSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);

                    //if (programmeId == 3 || programmeId == 4)
                    //{
                    //    SetStudentNdResultDateAwardedDropDown(existingViewModel);
                    //    SetStudentLastEmploymentEndDateDropDown(existingViewModel);
                    //    SetStudentLastEmploymentStartDateDropDown(existingViewModel);
                    //    SetPreviousEducationEndDateDropDowns(existingViewModel);
                    //    SetPreviousEducationStartDateDropDowns(existingViewModel);

                    //    ViewBag.ResultGrades = new SelectList(existingViewModel.ResultGradeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.PreviousEducation.ResultGrade.Id);
                    //}

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        existingViewModel.SecondSittingOLevelResult.Type = new OLevelType() { Id = 0 };
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, 0);
                    }

                    SetSelectedSittingSubjectAndGrade(existingViewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetStudentLastEmploymentStartDateDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentStartMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentStartMonthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartMonth.Id);
                ViewBag.StudentLastEmploymentStartYears =
                    new SelectList(existingViewModel.StudentLastEmploymentStartYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartYear.Id);

                if ((existingViewModel.StudentLastEmploymentStartDaySelectList == null ||
                     existingViewModel.StudentLastEmploymentStartDaySelectList.Count == 0) &&
                    (existingViewModel.StudentEmploymentInformation.StartMonth.Id > 0 &&
                     existingViewModel.StudentEmploymentInformation.StartYear.Id > 0))
                {
                    existingViewModel.StudentLastEmploymentStartDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.StartMonth,
                            existingViewModel.StudentEmploymentInformation.StartYear);
                }
                else
                {
                    if (existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
                    }
                    else if (existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.StartDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            existingViewModel.StudentLastEmploymentStartDaySelectList;
                    }
                    else if (existingViewModel.StudentLastEmploymentStartDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentStartDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentStartDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentEmploymentInformation.StartDay != null &&
                    existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentStartDays =
                        new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = existingViewModel.StudentLastEmploymentStartDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetStudentLastEmploymentEndDateDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentEndMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentEndMonthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndMonth.Id);
                ViewBag.StudentLastEmploymentEndYears =
                    new SelectList(existingViewModel.StudentLastEmploymentEndYearSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentEmploymentInformation.EndYear.Id);

                if ((existingViewModel.StudentLastEmploymentEndDaySelectList == null ||
                     existingViewModel.StudentLastEmploymentEndDaySelectList.Count == 0) &&
                    (existingViewModel.StudentEmploymentInformation.EndMonth.Id > 0 &&
                     existingViewModel.StudentEmploymentInformation.EndYear.Id > 0))
                {
                    existingViewModel.StudentLastEmploymentEndDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.EndMonth,
                            existingViewModel.StudentEmploymentInformation.EndYear);
                }
                else
                {
                    if (existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
                    }
                    else if (existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.EndDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            existingViewModel.StudentLastEmploymentEndDaySelectList;
                    }
                    else if (existingViewModel.StudentLastEmploymentEndDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentEndDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentEndDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentEmploymentInformation.EndDay != null &&
                    existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentEndDays =
                        new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = existingViewModel.StudentLastEmploymentEndDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetStudentNdResultDateAwardedDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentNdResultMonthAwardeds =
                    new SelectList(existingViewModel.StudentNdResultMonthAwardedSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentNdResult.MonthAwarded.Id);
                ViewBag.StudentNdResultYearAwardeds =
                    new SelectList(existingViewModel.StudentNdResultYearAwardedSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentNdResult.YearAwarded.Id);
                if ((existingViewModel.StudentNdResultDayAwardedSelectList == null ||
                     existingViewModel.StudentNdResultDayAwardedSelectList.Count == 0) &&
                    (existingViewModel.StudentNdResult.MonthAwarded.Id > 0 &&
                     existingViewModel.StudentNdResult.YearAwarded.Id > 0))
                {
                    existingViewModel.StudentNdResultDayAwardedSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentNdResult.MonthAwarded,
                            existingViewModel.StudentNdResult.YearAwarded);
                }
                else
                {
                    if (existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                        existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds =
                            new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
                    }
                    else if (existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                             existingViewModel.StudentNdResult.DayAwarded.Id <= 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                    }
                    else if (existingViewModel.StudentNdResultDayAwardedSelectList == null)
                    {
                        existingViewModel.StudentNdResultDayAwardedSelectList = new List<SelectListItem>();
                        ViewBag.StudentNdResultDayAwardeds = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentNdResult.DayAwarded != null &&
                    existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                {
                    ViewBag.StudentNdResultDayAwardeds =
                        new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetDateOfBirthDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.MonthOfBirths = new SelectList(existingViewModel.MonthOfBirthSelectList, Utility.VALUE,
                    Utility.TEXT, existingViewModel.Person.MonthOfBirth.Id);
                ViewBag.YearOfBirths = new SelectList(existingViewModel.YearOfBirthSelectList, Utility.VALUE,
                    Utility.TEXT, existingViewModel.Person.YearOfBirth.Id);
                if ((existingViewModel.DayOfBirthSelectList == null || existingViewModel.DayOfBirthSelectList.Count == 0) &&
                    (existingViewModel.Person.MonthOfBirth.Id > 0 && existingViewModel.Person.YearOfBirth.Id > 0))
                {
                    existingViewModel.DayOfBirthSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.Person.MonthOfBirth,
                            existingViewModel.Person.YearOfBirth);
                }
                else
                {
                    if (existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id > 0)
                    {
                        ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                    }
                    else if (existingViewModel.DayOfBirthSelectList != null &&
                             existingViewModel.Person.DayOfBirth.Id <= 0)
                    {
                        ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                    }
                    else if (existingViewModel.DayOfBirthSelectList == null)
                    {
                        existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
                        ViewBag.DayOfBirths = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
                {
                    ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                }
                else
                {
                    ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationStartDateDropDowns(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationStartMonths =
                    new SelectList(existingViewModel.PreviousEducationStartMonthSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.StartMonth.Id);
                ViewBag.PreviousEducationStartYears =
                    new SelectList(existingViewModel.PreviousEducationStartYearSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.StartYear.Id);
                if ((existingViewModel.PreviousEducationStartDaySelectList == null ||
                     existingViewModel.PreviousEducationStartDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.StartMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.StartYear.Id > 0))
                {
                    existingViewModel.PreviousEducationStartDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.StartMonth,
                            existingViewModel.PreviousEducation.StartYear);
                }
                else
                {
                    if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                        existingViewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays =
                            new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.PreviousEducation.StartDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                             existingViewModel.PreviousEducation.StartDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationStartDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationStartDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.StartDay != null &&
                    existingViewModel.PreviousEducation.StartDay.Id > 0)
                {
                    ViewBag.PreviousEducationStartDays =
                        new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.PreviousEducation.StartDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationEndDateDropDowns(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationEndMonths =
                    new SelectList(existingViewModel.PreviousEducationEndMonthSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.EndMonth.Id);
                ViewBag.PreviousEducationEndYears = new SelectList(
                    existingViewModel.PreviousEducationEndYearSelectList, Utility.VALUE, Utility.TEXT,
                    existingViewModel.PreviousEducation.EndYear.Id);
                if ((existingViewModel.PreviousEducationEndDaySelectList == null ||
                     existingViewModel.PreviousEducationEndDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.EndMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.EndYear.Id > 0))
                {
                    existingViewModel.PreviousEducationEndDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.EndMonth,
                            existingViewModel.PreviousEducation.EndYear);
                }
                else
                {
                    if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                        existingViewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays =
                            new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.PreviousEducation.EndDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                             existingViewModel.PreviousEducation.EndDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationEndDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationEndDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.EndDay != null &&
                    existingViewModel.PreviousEducation.EndDay.Id > 0)
                {
                    ViewBag.PreviousEducationEndDays =
                        new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE, Utility.TEXT,
                            existingViewModel.PreviousEducation.EndDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetDefaultSelectedSittingSubjectAndGrade(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["FirstSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }

                if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["SecondSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLgaIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    LocalGovernmentLogic localGovernmentLogic = new LocalGovernmentLogic();
                    List<LocalGovernment> lgas =
                        localGovernmentLogic.GetModelsBy(l => l.State_Id == viewModel.Person.State.Id);
                    if (viewModel.Person.LocalGovernment != null && viewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(lgas, Utility.ID, Utility.NAME,
                            viewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(lgas, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    ProgrammeDepartmentLogic departmentLogic = new ProgrammeDepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                {
                    DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                    List<DepartmentOption> departmentOptions =
                        departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
                    if (viewModel.StudentLevel.DepartmentOption != null &&
                        viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptions = new SelectList(departmentOptions, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        List<DepartmentOption> options = new List<DepartmentOption>();
                        DepartmentOption option = new DepartmentOption()
                        {
                            Id = 0,
                            Name = viewModel.StudentLevel.Department.Name
                        };
                        options.Add(option);

                        ViewBag.DepartmentOptions = new SelectList(options, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetPreviousEducationStartDate()
        {
            try
            {
                if (viewModel.PreviousEducation != null && viewModel.PreviousEducation.StartDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.StartYear.Id,
                        viewModel.PreviousEducation.StartMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value() { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.StartDay != null && viewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.PreviousEducation.StartDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetPreviousEducationEndDate()
        {
            try
            {
                if (viewModel.PreviousEducation != null && viewModel.PreviousEducation.EndDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.EndYear.Id,
                        viewModel.PreviousEducation.EndMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value() { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.EndDay != null && viewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.PreviousEducation.EndDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDateOfBirth()
        {
            try
            {
                if (viewModel.Person.DateOfBirth.HasValue)
                {
                    if (viewModel.Person.YearOfBirth.Id > 0 && viewModel.Person.MonthOfBirth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.Person.YearOfBirth.Id,
                            viewModel.Person.MonthOfBirth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value() { Name = "--DD--" });
                        }

                        if (viewModel.Person.DayOfBirth != null && viewModel.Person.DayOfBirth.Id > 0)
                        {
                            ViewBag.DayOfBirths = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.Person.DayOfBirth.Id);
                        }
                        else
                        {
                            ViewBag.DayOfBirths = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetSelectedSittingSubjectAndGrade(RegistrationViewModel existingViewModel)
        {
            try
            {
                if (existingViewModel != null && existingViewModel.FirstSittingOLevelResultDetails != null && existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    if (existingViewModel.FirstSittingOLevelResultDetails.Count != 9 && existingViewModel.FirstSittingOLevelResultDetails.Count < 9)
                    {
                        int firstSittingCount = existingViewModel.FirstSittingOLevelResultDetails.Count;
                        for (int j = 0; j < 9 - firstSittingCount; j++)
                        {
                            OLevelResultDetail oLevelResultDetailFirstSitting = new OLevelResultDetail();
                            oLevelResultDetailFirstSitting.Header = existingViewModel.FirstSittingOLevelResultDetails[0].Header;
                            oLevelResultDetailFirstSitting.Grade = null;
                            oLevelResultDetailFirstSitting.Subject = null;

                            existingViewModel.FirstSittingOLevelResultDetails.Add(oLevelResultDetailFirstSitting);
                        }
                    }

                    int i = 0;
                    foreach (OLevelResultDetail firstSittingOLevelResultDetail in existingViewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null && firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }

                if (existingViewModel != null && existingViewModel.SecondSittingOLevelResultDetails != null && existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    if (existingViewModel.SecondSittingOLevelResultDetails.Count != 9 && existingViewModel.SecondSittingOLevelResultDetails.Count < 9)
                    {
                        int secondSittingCount = existingViewModel.SecondSittingOLevelResultDetails.Count;
                        for (int j = 0; j < 9 - secondSittingCount; j++)
                        {
                            OLevelResultDetail oLevelResultDetailSecondSitting = new OLevelResultDetail();
                            oLevelResultDetailSecondSitting.Header = existingViewModel.SecondSittingOLevelResultDetails[0].Header;
                            oLevelResultDetailSecondSitting.Grade = null;
                            oLevelResultDetailSecondSitting.Subject = null;

                            existingViewModel.SecondSittingOLevelResultDetails.Add(oLevelResultDetailSecondSitting);
                        }
                    }

                    int i = 0;
                    foreach (OLevelResultDetail secondSittingOLevelResultDetail in existingViewModel.SecondSittingOLevelResultDetails)
                    {
                        if (secondSittingOLevelResultDetail.Subject != null && secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                LocalGovernmentLogic lgaLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(l => l.State_Id == id);

                return Json(new SelectList(lgas, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDayOfBirthBy(string monthId, string yearId)
        {
            try
            {
                if (string.IsNullOrEmpty(monthId) || string.IsNullOrEmpty(yearId))
                {
                    return null;
                }

                Value month = new Value() { Id = Convert.ToInt32(monthId) };
                Value year = new Value() { Id = Convert.ToInt32(yearId) };
                List<Value> days = Utility.GetNumberOfDaysInMonth(month, year);

                return Json(new SelectList(days, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 20 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ModifyOlevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                OlevelResultdDetailsAudit olevelResultdDetailsAudit = new OlevelResultdDetailsAudit();
                olevelResultdDetailsAudit.Action = "Modify";
                olevelResultdDetailsAudit.Operation = "Modify O level (Registration Controller)";
                olevelResultdDetailsAudit.Client = Request.LogonUserIdentity.Name + " (" +
                                                   HttpContext.Request.UserHostAddress + ")";
                UserLogic loggeduser = new UserLogic();
                olevelResultdDetailsAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                if (olevelResultdDetailsAudit.User == null)
                {
                    olevelResultdDetailsAudit.User = new User() { Id = 1 };
                }

                OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    OLevelResult oLevel =
                        oLevelResultLogic.GetModelsBy(
                            p =>
                                p.Person_Id == oLevelResult.Person.Id && p.Exam_Number == oLevelResult.ExamNumber &&
                                p.Exam_Year == oLevelResult.ExamYear &&
                                p.O_Level_Exam_Sitting_Id == oLevelResult.Sitting.Id).FirstOrDefault();
                    if (oLevel != null)
                    {
                        oLevelResult.Id = oLevel.Id;
                    }

                    if (oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult, olevelResultdDetailsAudit);
                        oLevelResultLogic.Modify(oLevelResult);
                    }
                    else
                    {

                        OLevelResult newOLevelResult = oLevelResultLogic.Create(oLevelResult);
                        oLevelResult.Id = newOLevelResult.Id;
                    }

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(
                                m => m.Grade != null && m.Grade.Id > 0 && m.Subject != null && m.Subject.Id > 0)
                                .ToList();
                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetailLogic.Create(oLevelResultDetail);
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
        [HttpPost]
        public virtual ActionResult UploadPassport(FileUpload form)
        {
            //HttpPostedFileBase file = Request.Files["MyFile"];
            JsonResultObject jsonResultObject = new JsonResultObject();
            PersonLogic personLogic = new PersonLogic();
            Person person = new Person();

            var myfile = form.file;
            bool isUploaded = false;
            string personName = form.personName;
            long personId = form.personId;
            string passportUrl = form.personName;
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;
            string newFileName = "";
            string returnName = "";

            try
            {
                if (form != null && form.file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(form.file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__" + personName + "__";
                    newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;
                    returnName = newFileName;

                    string invalidFileMessage = InvalidFile(form.file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded = isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    //string junkPath = imageFileUrl.Split('?').FirstOrDefault();
                    //string studentPath = "/Content/Student/" + junkPath.Split('/').LastOrDefault();

                    string pathForSaving = Server.MapPath("~/Content/Student");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId.ToString());

                        form.file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "Saved!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Student/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            TempData["imageUrl"] = imageUrl;
                            person = personLogic.GetModelBy(p => p.Person_Id == personId);
                            if (person != null)
                            {
                                person.ImageFileUrl = imageUrl;
                                personLogic.Modify(person);

                                jsonResultObject.Message = imageUrl;
                                return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                            }


                        }
                    }
                }
                else
                {
                    jsonResultObject.Message = "No file Selected";
                    return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            //return Json(new { isUploaded = isUploaded, message = message, imageUrl = returnName }, "text/html", JsonRequestBehavior.AllowGet);
            jsonResultObject.Message = message;
            return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["PassportFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"].ToString();
            string passportUrl = form["Person.ImageFileUrl"].ToString();
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile +
                                         DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                                         fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(
                            new { isUploaded = isUploaded, message = invalidFileMessage, imageUrl = passportUrl },
                            "text/html", JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded = isUploaded, message = message, imageUrl = imageUrlDisplay }, "text/html",
                JsonRequestBehavior.AllowGet);
        }


        public JsonResult SavePassport(string imageFileUrl)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!string.IsNullOrEmpty(imageFileUrl))
                {
                    var studentUserName = User.Identity.Name;
                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == studentUserName).LastOrDefault();
                    //Model.Model.Student student =  System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;

                    if (student == null)
                    {
                        FormsAuthentication.SignOut();
                        System.Web.HttpContext.Current.Response.Redirect(Url.Action("Login", "Account",
                            new { Area = "Security" }));
                    }

                    string junkPath = imageFileUrl.Split('?').FirstOrDefault();
                    string studentPath = "/Content/Student/" + junkPath.Split('/').LastOrDefault();

                    if (System.IO.File.Exists(Server.MapPath("~" + junkPath)))
                    {
                        viewModel = new RegistrationViewModel() { Person = new Person() { Id = student.Id, ImageFileUrl = junkPath } };
                        SetPersonPassportDestination(viewModel, out junkPath, out studentPath);

                        SavePersonPassport(junkPath, studentPath, viewModel.Person);


                        //System.IO.File.Move(Server.MapPath(junkPath), Server.MapPath(studentPath));

                        PersonLogic personLogic = new PersonLogic();
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);

                        person.ImageFileUrl = viewModel.Person.ImageFileUrl;

                        personLogic.Modify(person);

                        result.IsError = false;
                        result.Message = "Passport has been saved.";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Initial filepath does not exist.";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Image Url not set.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormAltShowPreview(string dataArray, string firstSitting, string secondSitting)
        {
            viewModel = new RegistrationViewModel();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            ApplicationFormJsonModel applicationJsonData = serializer.Deserialize<ApplicationFormJsonModel>(dataArray);
            List<OLevelResultDetailJsonModel> firstSittingOLevelJsonData =
                serializer.Deserialize<List<OLevelResultDetailJsonModel>>(firstSitting);
            List<OLevelResultDetailJsonModel> secondSittingOLevelJsonData =
                serializer.Deserialize<List<OLevelResultDetailJsonModel>>(secondSitting);

            try
            {
                PopulateModelsFromJsonData(viewModel, applicationJsonData);
                PopulateOLevelResultDetailFromJsonData(viewModel, firstSittingOLevelJsonData, 1);
                PopulateOLevelResultDetailFromJsonData(viewModel, secondSittingOLevelJsonData, 2);

                if (InvalidDateOfBirth(viewModel))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                if (InvalidNumberOfOlevelSubject(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResultDetails))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                OLevelGradeLogic oLevelGradeLogic = new OLevelGradeLogic();
                OLevelSubjectLogic oLevelSubjectLogic = new OLevelSubjectLogic();

                viewModel.OLevelGrades = oLevelGradeLogic.GetAll();
                viewModel.OLevelSubjects = oLevelSubjectLogic.GetAll();

                if (InvalidOlevelSubjectOrGrade(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects,
                    viewModel.OLevelGrades, FIRST_SITTING))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                if (viewModel.SecondSittingOLevelResult != null)
                {
                    if (viewModel.SecondSittingOLevelResult.ExamNumber != null &&
                        viewModel.SecondSittingOLevelResult.Type != null &&
                        viewModel.SecondSittingOLevelResult.Type.Id > 0 &&
                        viewModel.SecondSittingOLevelResult.ExamYear > 0)
                    {
                        if (InvalidOlevelSubjectOrGrade(viewModel.SecondSittingOLevelResultDetails,
                            viewModel.OLevelSubjects, viewModel.OLevelGrades, SECOND_SITTING))
                        {
                            applicationJsonData.IsError = true;
                            Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                            applicationJsonData.Message = msg.Description;
                            return
                                Json(
                                    new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                                    JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.FirstSittingOLevelResult, FIRST_SITTING))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.SecondSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResult, SECOND_SITTING))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                if (NoOlevelSubjectSpecified(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.FirstSittingOLevelResult, FIRST_SITTING))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }
                if (NoOlevelSubjectSpecified(viewModel.SecondSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResult, SECOND_SITTING))
                {
                    applicationJsonData.IsError = true;
                    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                    applicationJsonData.Message = msg.Description;
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                //if (InvalidOlevelType(viewModel.FirstSittingOLevelResult.Type, viewModel.SecondSittingOLevelResult.Type))
                //{
                //    applicationJsonData.IsError = true;
                //    Message msg = (Abundance_Nk.Model.Model.Message)TempData["Message"];
                //    applicationJsonData.Message = msg.Description;
                //    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData }, JsonRequestBehavior.AllowGet);
                //}

                if (string.IsNullOrEmpty(viewModel.Person.ImageFileUrl))
                {
                    applicationJsonData.IsError = true;
                    applicationJsonData.Message = "No Passport uploaded! Please upload your passport to continue.";
                    return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                        JsonRequestBehavior.AllowGet);
                }

                applicationJsonData.IsError = false;

                return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        var objRef = "Please Kindly Verify that the fileds were properly filled";
                        applicationJsonData.IsError = true;
                        applicationJsonData.Message = "Error! " + objRef;
                    }
                }
                if (string.IsNullOrEmpty(applicationJsonData.Message))
                {
                    applicationJsonData.IsError = true;
                    applicationJsonData.Message = "Error! " + ex.Message;
                }

            }

            return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData },
                JsonRequestBehavior.AllowGet);
        }

        private bool NoOlevelSubjectSpecified(List<OLevelResultDetail> oLevelResultDetails, OLevelResult oLevelResult,
            string sitting)
        {
            try
            {
                if (!string.IsNullOrEmpty(oLevelResult.ExamNumber) ||
                    (oLevelResult.Type != null && oLevelResult.Type.Id > 0) || (oLevelResult.ExamYear > 0))
                {
                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> oLevelResultDetailsEntered =
                            oLevelResultDetails.Where(r => r.Subject.Id > 0).ToList();
                        if (oLevelResultDetailsEntered.Count <= 0)
                        {
                            SetMessage(
                                "No O-Level Subject specified for " + sitting +
                                "! At least one subject must be specified when Exam Number, O-Level Type and Year are all specified for the sitting.",
                                Message.Category.Error);
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

        private bool InvalidNumberOfOlevelSubject(List<OLevelResultDetail> firstSittingResultDetails,
            List<OLevelResultDetail> secondSittingResultDetails)
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
                if (totalNoOfSubjects < FIVE)
                {
                    SetMessage("O-Level Result cannot be less than " + FIVE + " subjects in both sittings!",
                        Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void PopulateOLevelResultDetailFromJsonData(RegistrationViewModel viewModel,
            List<OLevelResultDetailJsonModel> oLevelResultDetailJsonModels, int type)
        {
            List<OLevelResultDetail> oLevelResultDetails = new List<OLevelResultDetail>();
            try
            {
                OLevelGradeLogic oLevelGradeLogic = new OLevelGradeLogic();
                OLevelSubjectLogic oLevelSubjectLogic = new OLevelSubjectLogic();

                List<OLevelResultDetailJsonModel> myOLevelModels =
                    oLevelResultDetailJsonModels.Where(o => o.GradeId != "0" && o.SubjectId != "0").ToList();

                if (myOLevelModels.Count > 0)
                {
                    for (int i = 0; i < myOLevelModels.Count; i++)
                    {
                        OLevelResultDetail oLevelResultDetail = new OLevelResultDetail();
                        int oLevelSubjectId = Convert.ToInt32(myOLevelModels[i].SubjectId);
                        O_LEVEL_SUBJECT oLevelSubject =
                            oLevelSubjectLogic.GetEntityBy(o => o.O_Level_Subject_Id == oLevelSubjectId);
                        int oLevelGradeId = Convert.ToInt32(myOLevelModels[i].GradeId);
                        O_LEVEL_GRADE oLevelGrade =
                            oLevelGradeLogic.GetEntityBy(o => o.O_Level_Grade_Id == oLevelGradeId);

                        oLevelResultDetail.Grade = new OLevelGrade()
                        {
                            Id = oLevelGradeId,
                            Name = oLevelGrade.O_Level_Grade_Name
                        };
                        oLevelResultDetail.Subject = new OLevelSubject()
                        {
                            Id = oLevelSubjectId,
                            Name = oLevelSubject.O_Level_Subject_Name
                        };

                        oLevelResultDetails.Add(oLevelResultDetail);

                        myOLevelModels[i].SubjectName = oLevelSubject.O_Level_Subject_Name;
                        myOLevelModels[i].GradeName = oLevelGrade.O_Level_Grade_Name;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (type == 1)
            {
                viewModel.FirstSittingOLevelResultDetails = oLevelResultDetails;
            }
            else
            {
                viewModel.SecondSittingOLevelResultDetails = oLevelResultDetails;
            }
        }

        private bool InvalidDateOfBirth(RegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.YearOfBirth == null || viewModel.Person.YearOfBirth.Id <= 0)
                {
                    SetMessage("Please select Year of Birth!", Message.Category.Error);
                    return true;
                }
                if (viewModel.Person.MonthOfBirth == null || viewModel.Person.MonthOfBirth.Id <= 0)
                {
                    SetMessage("Please select Month of Birth!", Message.Category.Error);
                    return true;
                }
                if (viewModel.Person.DayOfBirth == null || viewModel.Person.DayOfBirth.Id <= 0)
                {
                    SetMessage("Please select Day of Birth!", Message.Category.Error);
                    return true;
                }

                viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                    viewModel.Person.MonthOfBirth.Id, viewModel.Person.DayOfBirth.Id);
                if (viewModel.Person.DateOfBirth == null)
                {
                    SetMessage("Please enter Date of Birth!", Message.Category.Error);
                    return true;
                }

                TimeSpan difference = DateTime.Now - (DateTime)viewModel.Person.DateOfBirth;
                if (difference.Days == 0)
                {
                    SetMessage("Date of Birth cannot be todays date!", Message.Category.Error);
                    return true;
                }
                if (difference.Days == -1)
                {
                    SetMessage("Date of Birth cannot be yesterdays date date!", Message.Category.Error);
                    return true;
                }

                if (difference.Days < 4380)
                {
                    SetMessage("Applicant cannot be less than twelve years!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateModelsFromJsonData(RegistrationViewModel viewModel,
            ApplicationFormJsonModel applicationJsonData)
        {
            viewModel.Person = new Person();
            viewModel.NextOfKin = new NextOfKin();
            viewModel.FirstSittingOLevelResult = new OLevelResult();
            viewModel.SecondSittingOLevelResult = new OLevelResult();
            viewModel.StudentSponsor = new StudentSponsor();
            viewModel.Student = new Model.Model.Student();
            viewModel.StudentAcademicInformation = new StudentAcademicInformation();
            viewModel.StudentFinanceInformation = new StudentFinanceInformation();
            viewModel.StudentLevel = new StudentLevel();
            try
            {
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                PersonLogic personLogic = new PersonLogic();
                ModeOfStudyLogic modeOfStudyLogic = new ModeOfStudyLogic();
                ModeOfEntryLogic modeOfEntryLogic = new ModeOfEntryLogic();
                LevelLogic levelLogic = new LevelLogic();

                viewModel.Session = new Model.Model.Session();
                viewModel.Session.Id = Convert.ToInt32(applicationJsonData.SessionId);
                viewModel.Session.Name = applicationJsonData.Session;

                viewModel.Session = new Session() { Id = viewModel.Session.Id, Name = viewModel.Session.Name };

                viewModel.Person.Id = Convert.ToInt64(applicationJsonData.PersonId);
                viewModel.Person.LastName = applicationJsonData.LastName;
                viewModel.Person.FirstName = applicationJsonData.FirstName;
                viewModel.Person.OtherName = applicationJsonData.OtherName;
                viewModel.Person.Sex = new Sex() { Id = Convert.ToByte(applicationJsonData.SexId) };
                viewModel.Person.YearOfBirth = new Value() { Id = Convert.ToInt32(applicationJsonData.YearOfBirthId) };
                viewModel.Person.MonthOfBirth = new Value() { Id = Convert.ToInt32(applicationJsonData.MonthOfBirthId) };
                viewModel.Person.DayOfBirth = new Value() { Id = Convert.ToInt32(applicationJsonData.DayOfBirthId) };
                viewModel.Person.DateOfBirth = new DateTime(Convert.ToInt32(applicationJsonData.YearOfBirthId),
                    Convert.ToInt32(applicationJsonData.MonthOfBirthId),
                    Convert.ToInt32(applicationJsonData.DayOfBirthId));
                viewModel.Person.State = new State() { Id = applicationJsonData.StateId };
                viewModel.Person.LocalGovernment = new LocalGovernment()
                {
                    Id = Convert.ToInt32(applicationJsonData.LocalGovernmentId)
                };
                if (viewModel.Person.LocalGovernment.Id <= 0)
                {
                    viewModel.Person.LocalGovernment = new LocalGovernment() { Id = 265 };
                }
                viewModel.Person.HomeTown = applicationJsonData.HomeTown;
                viewModel.Person.MobilePhone = applicationJsonData.MobilePhone;
                viewModel.Person.Email = applicationJsonData.Email;
                viewModel.Person.Religion = new Religion() { Id = Convert.ToInt32(applicationJsonData.ReligionId) };

                if (viewModel.Person.Id >= 0)
                {
                    viewModel.Person.ImageFileUrl =
                        personLogic.GetEntityBy(e => e.Person_Id == viewModel.Person.Id).Image_File_Url;
                }
                else
                {
                    viewModel.Person.ImageFileUrl = "/Content/Images/default_avatar.png";
                }

                //viewModel.Person.ImageFileUrl = applicationJsonData.ImageFileUrl;
                viewModel.Person.FullName = applicationJsonData.LastName + " " + applicationJsonData.FirstName + " " +
                                            applicationJsonData.OtherName;

                viewModel.Student.Id = viewModel.Person.Id;
                viewModel.Student.Title = new Title() { Id = Convert.ToInt32(applicationJsonData.TitleId) };
                viewModel.Student.MaritalStatus = new MaritalStatus()
                {
                    Id = Convert.ToInt32(applicationJsonData.MaritalStatusId)
                };
                viewModel.Student.Genotype = new Genotype() { Id = Convert.ToInt32(applicationJsonData.GenotypeId) };
                viewModel.Student.BloodGroup = new BloodGroup() { Id = Convert.ToInt32(applicationJsonData.BloodGroupId) };
                viewModel.Student.Category = new StudentCategory()
                {
                    Id = Convert.ToInt32(applicationJsonData.StudentCategoryId)
                };
                viewModel.Student.Type = new StudentType() { Id = Convert.ToInt32(applicationJsonData.StudentTypeId) };
                viewModel.Student.SchoolContactAddress = applicationJsonData.ContactAddress;

                viewModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry()
                {
                    Id = Convert.ToInt32(applicationJsonData.ModeOfEntryId)
                };
                viewModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy()
                {
                    Id = Convert.ToInt32(applicationJsonData.ModeOfStudyId)
                };

                viewModel.StudentFinanceInformation.Mode = new ModeOfFinance()
                {
                    Id = Convert.ToInt32(applicationJsonData.ModeOfFinanceId)
                };
                viewModel.StudentLevel.Programme = new Programme()
                {
                    Id = Convert.ToInt32(applicationJsonData.ProgrammeId)
                };
                viewModel.StudentLevel.Department = new Department()
                {
                    Id = Convert.ToInt32(applicationJsonData.DepartmentId)
                };

                viewModel.FirstSittingOLevelResult.Type = new OLevelType()
                {
                    Id = Convert.ToInt32(applicationJsonData.FirstSittingOLevelResultTypeId)
                };
                viewModel.FirstSittingOLevelResult.ExamNumber = applicationJsonData.FirstSittingOLevelResultExamNumber;
                viewModel.FirstSittingOLevelResult.ExamYear =
                    Convert.ToInt32(applicationJsonData.FirstSittingOLevelResultExamYear);

                viewModel.SecondSittingOLevelResult.Type = new OLevelType()
                {
                    Id = Convert.ToInt32(applicationJsonData.SecondSittingOLevelResultTypeId)
                };
                viewModel.SecondSittingOLevelResult.ExamNumber = applicationJsonData.SecondSittingOLevelResultExamNumber;
                viewModel.SecondSittingOLevelResult.ExamYear =
                    Convert.ToInt32(applicationJsonData.SecondSittingOLevelResultExamYear);

                viewModel.StudentSponsor.Name = applicationJsonData.SponsorName;
                viewModel.StudentSponsor.ContactAddress = applicationJsonData.SponsorContactAddress;
                viewModel.StudentSponsor.Relationship = new Relationship()
                {
                    Id = Convert.ToInt32(applicationJsonData.SponsorRelationshipId)
                };
                viewModel.StudentSponsor.MobilePhone = applicationJsonData.SponsorMobilePhone;

                viewModel.NextOfKin.Name = applicationJsonData.NextOfKinName;
                viewModel.NextOfKin.ContactAddress = applicationJsonData.NextOfKinContactAddress;
                viewModel.NextOfKin.Relationship = new Relationship()
                {
                    Id = Convert.ToInt32(applicationJsonData.NextOfKinRelationshipId)
                };
                viewModel.NextOfKin.MobilePhone = applicationJsonData.NextOfKinMobilePhone;


                viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                    viewModel.Person.MonthOfBirth.Id, viewModel.Person.DayOfBirth.Id);
                viewModel.Person.State =
                    viewModel.States.Where(m => m.Id == viewModel.Person.State.Id).SingleOrDefault();
                viewModel.Person.LocalGovernment =
                    viewModel.Lgas.Where(m => m.Id == viewModel.Person.LocalGovernment.Id).SingleOrDefault();
                viewModel.Person.Sex = viewModel.Genders.Where(m => m.Id == viewModel.Person.Sex.Id).SingleOrDefault();
                viewModel.NextOfKin.Relationship =
                    viewModel.Relationships.Where(m => m.Id == viewModel.NextOfKin.Relationship.Id).SingleOrDefault();
                viewModel.Person.Religion =
                    viewModel.Religions.Where(m => m.Id == viewModel.Person.Religion.Id).SingleOrDefault();
                viewModel.Student.Title =
                    viewModel.Titles.Where(m => m.Id == viewModel.Student.Title.Id).SingleOrDefault();
                viewModel.Student.MaritalStatus =
                    viewModel.MaritalStatuses.Where(m => m.Id == viewModel.Student.MaritalStatus.Id).SingleOrDefault();

                if (viewModel.Student.BloodGroup != null && viewModel.Student.BloodGroup.Id > 0)
                {
                    viewModel.Student.BloodGroup =
                        viewModel.BloodGroups.Where(m => m.Id == viewModel.Student.BloodGroup.Id).SingleOrDefault();
                }
                if (viewModel.Student.Genotype != null && viewModel.Student.Genotype.Id > 0)
                {
                    viewModel.Student.Genotype =
                        viewModel.Genotypes.Where(m => m.Id == viewModel.Student.Genotype.Id).SingleOrDefault();
                }

                viewModel.StudentAcademicInformation.ModeOfEntry =
                    viewModel.ModeOfEntries.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfEntry.Id)
                        .SingleOrDefault();
                viewModel.StudentAcademicInformation.ModeOfStudy =
                    viewModel.ModeOfStudies.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfStudy.Id)
                        .SingleOrDefault();
                viewModel.Student.Category =
                    viewModel.StudentCategories.Where(m => m.Id == viewModel.Student.Category.Id).SingleOrDefault();
                viewModel.Student.Type =
                    viewModel.StudentTypes.Where(m => m.Id == viewModel.Student.Type.Id).SingleOrDefault();
                viewModel.StudentAcademicInformation.Level = new Level()
                {
                    Id = Convert.ToInt32(applicationJsonData.LevelId)
                };
                viewModel.StudentAcademicInformation.Level =
                    viewModel.Levels.Where(m => m.Id == viewModel.StudentAcademicInformation.Level.Id).SingleOrDefault();
                viewModel.StudentFinanceInformation.Mode =
                    viewModel.ModeOfFinances.Where(m => m.Id == viewModel.StudentFinanceInformation.Mode.Id)
                        .SingleOrDefault();
                viewModel.StudentSponsor.Relationship =
                    viewModel.Relationships.Where(m => m.Id == viewModel.StudentSponsor.Relationship.Id)
                        .SingleOrDefault();

                viewModel.FirstSittingOLevelResult.Type =
                    viewModel.OLevelTypes.Where(m => m.Id == viewModel.FirstSittingOLevelResult.Type.Id)
                        .SingleOrDefault();
                if (viewModel.SecondSittingOLevelResult.Type != null)
                {
                    viewModel.SecondSittingOLevelResult.Type =
                        viewModel.OLevelTypes.Where(m => m.Id == viewModel.SecondSittingOLevelResult.Type.Id)
                            .SingleOrDefault();
                }


                applicationJsonData.Title = viewModel.Student.Title.Name;
                applicationJsonData.StudentCategory = viewModel.Student.Category.Name;
                applicationJsonData.StudentType = viewModel.Student.Type.Name;
                applicationJsonData.ModeOfFinance = viewModel.StudentFinanceInformation.Mode.Name;
                applicationJsonData.MaritalStatus = viewModel.Student.MaritalStatus.Name;
                applicationJsonData.Sex = viewModel.Person.Sex.Name;
                applicationJsonData.Genotype = viewModel.Student.Genotype.Name;
                applicationJsonData.BloodGroup = viewModel.Student.BloodGroup.Name;
                applicationJsonData.State = viewModel.Person.State.Name;
                applicationJsonData.LocalGovernment = viewModel.Person.LocalGovernment.Name;
                applicationJsonData.Religion = viewModel.Person.Religion.Name;
                applicationJsonData.SponsorRelationship = viewModel.StudentSponsor.Relationship.Name;
                applicationJsonData.Programme =
                    programmeLogic.GetEntityBy(p => p.Programme_Id == viewModel.StudentLevel.Programme.Id)
                        .Programme_Name;
                applicationJsonData.Level =
                    levelLogic.GetEntityBy(p => p.Level_Id == viewModel.StudentAcademicInformation.Level.Id).Level_Name;
                applicationJsonData.Department =
                    departmentLogic.GetEntityBy(d => d.Department_Id == viewModel.StudentLevel.Department.Id)
                        .Department_Name;
                applicationJsonData.Faculty =
                    departmentLogic.GetEntityBy(d => d.Department_Id == viewModel.StudentLevel.Department.Id)
                        .FACULTY.Faculty_Name;

                applicationJsonData.NextOfKinRelationship = viewModel.NextOfKin.Relationship.Name;
                applicationJsonData.SponsorRelationship = viewModel.StudentSponsor.Relationship.Name;
                applicationJsonData.ImageFileUrl = viewModel.Person.ImageFileUrl;

                int modeOfEntryId = Convert.ToInt32(applicationJsonData.ModeOfEntryId);
                int modeOfStudyId = Convert.ToInt32(applicationJsonData.ModeOfStudyId);
                if (modeOfEntryId > 0 && modeOfStudyId > 0)
                {
                    applicationJsonData.ModeOfEntry =
                        modeOfEntryLogic.GetEntityBy(m => m.Mode_Of_Entry_Id == modeOfEntryId).Mode_Of_Entry_Name;
                    applicationJsonData.ModeOfStudy =
                        modeOfStudyLogic.GetEntityBy(m => m.Mode_Of_Study_Id == modeOfStudyId).Mode_Of_Study_Name;
                }


                DateTime dateOfBirth = viewModel.Person.DateOfBirth.Value;
                applicationJsonData.DateOfBirth = dateOfBirth.ToLongDateString();

                applicationJsonData.FirstSittingOLevelResultType = viewModel.FirstSittingOLevelResult.Type.Name;
                if (viewModel.SecondSittingOLevelResult.Type != null && viewModel.SecondSittingOLevelResult.Type.Id > 0)
                {
                    applicationJsonData.SecondSittingOLevelResultType = viewModel.SecondSittingOLevelResult.Type.Name;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelResultHeaderInformation(List<OLevelResultDetail> resultDetails,
            OLevelResult oLevelResult, string sitting)
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
                            SetMessage("O-Level Exam Number not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.Type == null || oLevelResult.Type.Id <= 0)
                        {
                            SetMessage("O-Level Exam Type not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.ExamYear <= 0)
                        {
                            SetMessage("O-Level Exam Year not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
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

        private bool InvalidOlevelSubjectOrGrade(List<OLevelResultDetail> oLevelResultDetails,
            List<OLevelSubject> subjects, List<OLevelGrade> grades, string sitting)
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

                    List<OLevelResultDetail> results =
                        subjectList.Where(o => o.Subject.Id == oLevelResultDetail.Subject.Id).ToList();
                    if (results != null && results.Count > 1)
                    {
                        SetMessage(
                            "Duplicate " + subject.Name.ToUpper() + " Subject detected in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id > 0 && oLevelResultDetail.Grade.Id <= 0)
                    {
                        SetMessage(
                            "No Grade specified for Subject " + subject.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    {
                        SetMessage(
                            "No Subject specified for Grade" + grade.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }

                    //else if (oLevelResultDetail.Grade.Id > 0 && oLevelResultDetail.Subject.Id <= 0)
                    //{
                    //    SetMessage("No Grade specified for " + subject.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
                    //else if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    //{
                    //    SetMessage("No Subject specified for " + grade.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
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
                    if ((firstSittingOlevelType.Id != secondSittingOlevelType.Id) && firstSittingOlevelType.Id > 0 &&
                        secondSittingOlevelType.Id > 0)
                    {
                        if (firstSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage(
                                "NABTEB O-Level Type in " + FIRST_SITTING +
                                " cannot be combined with any other O-Level Type! Please modify.",
                                Message.Category.Error);
                            return true;
                        }
                        if (secondSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage(
                                "NABTEB O-Level Type in " + SECOND_SITTING +
                                " cannot be combined with any other O-Level Type! Please modify.",
                                Message.Category.Error);
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

        public JsonResult FormAltPreviewPost(string dataArray, string firstSitting, string secondSitting)
        {
            var viewModel = new RegistrationViewModel();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ApplicationFormJsonModel applicationJsonData =
                serializer.Deserialize<ApplicationFormJsonModel>(dataArray);
            List<OLevelResultDetailJsonModel> firstSittingOLevelJsonData =
                serializer.Deserialize<List<OLevelResultDetailJsonModel>>(firstSitting);
            List<OLevelResultDetailJsonModel> secondSittingOLevelJsonData =
                serializer.Deserialize<List<OLevelResultDetailJsonModel>>(secondSitting);

            try
            {
                StudentSponsorLogic sponsorLogic = new StudentSponsorLogic();

                PopulateModelsFromJsonData(viewModel, applicationJsonData);
                PopulateOLevelResultDetailFromJsonData(viewModel, firstSittingOLevelJsonData, 1);
                PopulateOLevelResultDetailFromJsonData(viewModel, secondSittingOLevelJsonData, 2);

                StudentSponsor studentSponsor =
                    sponsorLogic.GetModelsBy(s => s.Person_Id == viewModel.Person.Id).LastOrDefault();

                Model.Model.Student newStudent = new Model.Model.Student();
                PersonType personType = new PersonType() { Id = (int)PersonType.EnumName.Student };

                if (studentSponsor != null)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus() { Id = 1 };
                        StudentLogic studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);


                        viewModel.StudentSponsor.Student = newStudent;

                        if (sponsorLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            sponsorLogic.Modify(viewModel.StudentSponsor);
                        }
                        else
                        {
                            sponsorLogic.Create(viewModel.StudentSponsor);
                        }


                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType() { Id = (int)PersonType.EnumName.Student };
                        NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                        if (nextOfKinLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            nextOfKinLogic.Modify(viewModel.NextOfKin);
                        }
                        else
                        {
                            nextOfKinLogic.Create(viewModel.NextOfKin);
                        }


                        if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            if (viewModel.FirstSittingOLevelResult == null)
                            {
                                viewModel.FirstSittingOLevelResult = new OLevelResult();
                            }

                            viewModel.FirstSittingOLevelResult.Person = viewModel.Person;
                            viewModel.FirstSittingOLevelResult.PersonType = personType;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
                        }

                        if (viewModel.SecondSittingOLevelResult == null ||
                            viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            if (viewModel.SecondSittingOLevelResult == null)
                            {
                                viewModel.SecondSittingOLevelResult = new OLevelResult();
                            }
                            viewModel.SecondSittingOLevelResult.Person = viewModel.Person;
                            viewModel.SecondSittingOLevelResult.PersonType = personType;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
                        }

                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult,
                            viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);

                        viewModel.StudentAcademicInformation.Student = newStudent;
                        StudentAcademicInformationLogic academicInformationLogic =
                            new StudentAcademicInformationLogic();
                        viewModel.StudentAcademicInformation.YearOfAdmission = Convert.ToInt32(applicationJsonData.YearOfAdmission);
                        viewModel.StudentAcademicInformation.YearOfGraduation = Convert.ToInt32(applicationJsonData.YearOfGraduation);
                        if (academicInformationLogic.GetModelBy(a => a.Person_Id == newStudent.Id) != null)
                        {
                            academicInformationLogic.Modify(viewModel.StudentAcademicInformation);
                        }
                        else
                        {
                            academicInformationLogic.Create(viewModel.StudentAcademicInformation);
                        }


                        viewModel.StudentFinanceInformation.Student = newStudent;
                        StudentFinanceInformationLogic financeInformationLogic =
                            new StudentFinanceInformationLogic();

                        if (
                            financeInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() !=
                            null)
                        {
                            financeInformationLogic.Modify(viewModel.StudentFinanceInformation);
                        }
                        else
                        {
                            financeInformationLogic.Create(viewModel.StudentFinanceInformation);
                        }

                        PersonLogic personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);

                        transaction.Complete();
                    }
                }
                else
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus() { Id = 1 };
                        StudentLogic studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);


                        viewModel.StudentSponsor.Student = newStudent;
                        if (sponsorLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            sponsorLogic.Modify(viewModel.StudentSponsor);
                        }
                        else
                        {
                            sponsorLogic.Create(viewModel.StudentSponsor);
                        }


                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType() { Id = (int)PersonType.EnumName.Student };
                        NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                        if (nextOfKinLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            nextOfKinLogic.Modify(viewModel.NextOfKin);
                        }
                        else
                        {
                            nextOfKinLogic.Create(viewModel.NextOfKin);
                        }


                        if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            if (viewModel.FirstSittingOLevelResult == null)
                            {
                                viewModel.FirstSittingOLevelResult = new OLevelResult();
                            }
                            viewModel.FirstSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
                        }

                        if (viewModel.SecondSittingOLevelResult == null ||
                            viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            if (viewModel.SecondSittingOLevelResult == null)
                            {
                                viewModel.SecondSittingOLevelResult = new OLevelResult();
                            }
                            viewModel.SecondSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
                        }
                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult,
                            viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);


                        viewModel.StudentAcademicInformation.Student = newStudent;
                        StudentAcademicInformationLogic academicInformationLogic =
                            new StudentAcademicInformationLogic();
                        viewModel.StudentAcademicInformation.YearOfAdmission = Convert.ToInt32(applicationJsonData.YearOfAdmission);
                        viewModel.StudentAcademicInformation.YearOfGraduation = Convert.ToInt32(applicationJsonData.YearOfGraduation);
                        if (academicInformationLogic.GetModelBy(a => a.Person_Id == newStudent.Id) != null)
                        {
                            academicInformationLogic.Modify(viewModel.StudentAcademicInformation);
                        }
                        else
                        {
                            academicInformationLogic.Create(viewModel.StudentAcademicInformation);
                        }

                        viewModel.StudentFinanceInformation.Student = newStudent;
                        StudentFinanceInformationLogic financeInformationLogic =
                            new StudentFinanceInformationLogic();
                        if (
                            financeInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() !=
                            null)
                        {
                            financeInformationLogic.Modify(viewModel.StudentFinanceInformation);
                        }
                        else
                        {
                            financeInformationLogic.Create(viewModel.StudentFinanceInformation);
                        }

                        PersonLogic personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);
                        transaction.Complete();
                    }
                }

                applicationJsonData.IsError = false;
                return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                applicationJsonData.IsError = true;
                applicationJsonData.Message = ex.Message;
                return Json(new { applicationJsonData, firstSittingOLevelJsonData, secondSittingOLevelJsonData }, JsonRequestBehavior.AllowGet);
            }
        }
    }
    public class FileUpload
    {
        public HttpPostedFileBase file { get; set; }
        //public int my { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string personName { get; set; }
        public long personId { get; set; }
    }
}