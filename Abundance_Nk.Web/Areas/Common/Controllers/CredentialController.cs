using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using System.IO;
using System.Drawing;
using BarcodeLib;
using static Abundance_Nk.Web.Areas.Admin.Views.SupportController;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    [AllowAnonymous]
    public class CredentialController : BaseController
    {
        public ActionResult ApplicationForm(long fid)
        {
            try
            {
                ApplicationFormViewModel applicationFormViewModel = new ApplicationFormViewModel();
                ApplicationForm form = new ApplicationForm() { Id = fid };

                applicationFormViewModel.GetApplicationFormBy(form);
                if (applicationFormViewModel.Person != null && applicationFormViewModel.Person.Id > 0)
                {
                    applicationFormViewModel.SetApplicantAppliedCourse(applicationFormViewModel.Person);
                }

                return View(applicationFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult StudentForm(string fid)
        {
            try
            {
                Int64 formId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(fid));

                ApplicationForm form = new ApplicationForm() { Id = formId };
                StudentFormViewModel studentFormViewModel = new StudentFormViewModel();

                studentFormViewModel.LoadApplicantionFormBy(formId);
                if (studentFormViewModel.ApplicationForm.Person != null && studentFormViewModel.ApplicationForm.Person.Id > 0)
                {
                    studentFormViewModel.LoadStudentInformationFormBy(studentFormViewModel.ApplicationForm.Person.Id);
                }

                return View(studentFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult PostUtmeResult(string jn)
        {
            PostUtmeResult result = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(jn))
                {
                    PostUtmeResultLogic postUtmeResultLogic = new PostUtmeResultLogic();
                    result = postUtmeResultLogic.GetModelBy(m => m.REGNO == jn);
                    if (result == null || result.Id <= 0)
                    {
                        //SetMessage("Registration Number / Jamb No was not found! Please check that you have typed in the correct detail", Message.Category.Error);
                        return View(result);
                    }
                    else
                    {
                        result.Fullname = result.Fullname;
                        result.Regno = result.Regno;
                        result.Eng = result.Eng;
                        result.Sub2 = result.Sub2;
                        result.Sub3 = result.Sub3;
                        result.Sub4 = result.Sub4;
                        result.Scr2 = result.Scr2;
                        result.Scr3 = result.Scr3;
                        result.Scr4 = result.Scr4;
                        result.Total = result.Total;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return View(result);
        }
        public ActionResult GenerateReciept()
        {
            AdmissionViewModel admissionViewModel = new AdmissionViewModel();

            return View();
        }
        [HttpPost]
        public ActionResult GenerateReciept(string remitaCode)
        {

          
                RemitaPayment remitaPayment = new RemitaPayment();
                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                remitaPayment = remitaPaymentLogic.GetModelsBy(x => x.RRR == remitaCode).LastOrDefault();
        

            return RedirectToAction("Receipt", new { pmid = remitaPayment.payment.Id });

        }

        public ActionResult Receipt(long pmid)
        {
            Payment payment = new Payment();
            PaymentLogic paymentLogic = new PaymentLogic();
            RemitaPayment remitaPayment = new RemitaPayment();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            Receipt receipt = null;

            try
            {
                receipt = GetReceiptBy(pmid);
                if (receipt == null)
                {
                    SetMessage("No receipt found!", Message.Category.Error);
                }
                else
                {
                    //http://localhost:2720/

                    //receipt.barcodeImageUrl = "https://students.fedpolyado.edu.ng/Common/Credential/Receipt?pmid=" + pmid; ;
                    receipt.barcodeImageUrl = "Name:" + receipt.Name + "  " + "Amount:" + receipt.Amount + " " +  "RRR:" + receipt.ConfirmationOrderNumber + "Date Paid:" + receipt.Date + " " + "Fee Type:" + receipt.Purpose;
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
            var MatricNumber = "";
            var ApplicationFormNumber = "";
            var department = "";
            var programme = "";
            
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
                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetBy(paymentEtranzact.Payment.Payment.Person.Id);
                    if (student != null)
                    {

                        MatricNumber = student.MatricNumber;
                        ApplicationFormNumber = student.ApplicationForm != null ? student.ApplicationForm.Number : null;
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel != null)
                        {
                            department = studentLevel.Department.Name;
                            programme = studentLevel.Programme.Name;
                        }
                        else
                        {
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                            }
                        }
                    }
                    else
                    {
                        AdmissionListLogic appliedCourseLogic = new AdmissionListLogic();
                        AdmissionList admissionList = appliedCourseLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                        if (admissionList != null)
                        {
                            department = admissionList.Deprtment.Name;
                            programme = admissionList.Programme.Name;

                        }
                    }
                    decimal amount =(decimal) paymentEtranzact.TransactionAmount;
                     receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, paymentEtranzact, amount, payment.FeeType.Name,programme,department,payment.Session.Name,payment.PaymentMode.Name);
                 }
                 else
                 {
                     RemitaPayment remitaPayment = remitaPaymentLogic.GetModelsBy(o => o.Payment_Id == payment.Id).FirstOrDefault();
                     if (remitaPayment != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")))
                     {
                         if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                         {
                             //throw new Exception("Fee Details for " + payment.FeeType.Name + " not set! Please contact your system administrator.");
                         }

                         decimal amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                         Abundance_Nk.Model.Model.Student student = new Model.Model.Student();
                         StudentLogic studentLogic = new StudentLogic();
                         student = studentLogic.GetBy(payment.Person.Id);                       
                         PaymentScholarship scholarship = new PaymentScholarship();
                         PaymentScholarshipLogic scholarshipLogic = new PaymentScholarshipLogic();
                         if (scholarshipLogic.IsStudentOnScholarship(payment.Person, payment.Session))
                         {
                             scholarship = scholarshipLogic.GetBy(payment.Person);
                             amount = payment.FeeDetails.Sum(p => p.Fee.Amount) - scholarship.Amount;

                         }

                        decimal amountToPay = GetAmountToPay(remitaPayment.payment);
                        var confirmation = HasPaidSchoolFees(payment, amountToPay);

                        if (amountToPay > 0M && amountToPay > remitaPayment.TransactionAmount)
                        {
                            ShortFallLogic shortFallLogic = new ShortFallLogic();
                            ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.Fee_Type_Id == remitaPayment.payment.FeeType.Id &&
                                                  s.PAYMENT.Session_Id == remitaPayment.payment.Session.Id && s.PAYMENT.Person_Id == remitaPayment.payment.Person.Id).LastOrDefault();
                            if (shortFall != null)
                            {
                                RemitaPayment shortFallRemitaPayment = CheckShortFallRemita(remitaPayment, amountToPay);
                                if(shortFallRemitaPayment != null)
                                {
                                    remitaPayment.TransactionAmount += shortFallRemitaPayment.TransactionAmount;
                                    remitaPayment.RRR += ", " + shortFallRemitaPayment.RRR;
                                }
                            }
                        }
                        Model.Model.Student getstudent = studentLogic.GetBy(remitaPayment.payment.Person.Id);
                        if (getstudent != null)
                        {

                            MatricNumber = getstudent.MatricNumber;
                            ApplicationFormNumber = getstudent.ApplicationForm!=null?getstudent.ApplicationForm.Number:null;
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            StudentLevel studentLevel = new StudentLevel();
                            studentLevel = studentLevelLogic.GetBy(student.Id);
                            var doesExit = studentLevelLogic.GetModelBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == payment.Session.Id);

                            if (studentLevel != null)
                            {
                                department = studentLevel.Department.Name;
                                programme = studentLevel.Programme.Name;
                                if(doesExit == null && confirmation && studentLevel.Level.Id != 2 && studentLevel.Level.Id != 4 && studentLevel.Level.Id != 7 && studentLevel.Level.Id != 10 && studentLevel.Level.Id != 12 && studentLevel.Level.Id != 14)
                                {
                                    studentLevel.Student = getstudent;
                                    studentLevel.Programme = studentLevel.Programme;
                                    studentLevel.Session = payment.Session;
                                    studentLevel.Department = studentLevel.Department;
                                    studentLevel.Level.Id = studentLevel.Level.Id + 1;

                                    studentLevelLogic.Create(studentLevel);
                                }

                            }
                            else
                            {
                                AdmissionListLogic admissionLogic = new AdmissionListLogic();
                                AdmissionList admissionList = admissionLogic.GetBy(remitaPayment.payment.Person);
                                if (admissionList != null)
                                {
                                    department = admissionList.Deprtment.Name;
                                    programme = admissionList.Programme.Name;
                                }
                            }
                        }
                        else
                        {
                            //AdmissionListLogic appliedCourseLogic = new AdmissionListLogic();
                            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                            AppliedCourse appliedCourse = appliedCourseLogic.GetBy(remitaPayment.payment.Person);


                            //AdmissionList admissionList = appliedCourseLogic.GetBy(remitaPayment.payment.Person);
                            if (appliedCourse != null)
                            {
                                department = appliedCourse.Department.Name;
                                programme = appliedCourse.Programme.Name;

                            }
                        }
                       
                        receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, "",programme,department,payment.Session.Name, payment.PaymentMode.Name);
                     }
                 }
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal GetAmountToPay(Payment payment)
        {
            decimal amt = 0M;
            try
            {
                if (payment != null)
                {
                    long progId = 0;
                    long deptId = 0;
                    long levelId = 0;
                    long nextLevelId = 0;

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == payment.Session.Id).LastOrDefault();
                    if (studentLevel != null)
                    {
                        progId = studentLevel.Programme.Id;
                        deptId = studentLevel.Department.Id;
                        levelId = studentLevel.Level.Id;
                        nextLevelId = studentLevel.Level.Id + 1;
                    }
                    else
                    {
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        AdmissionList list = listLogic.GetModelsBy(s => s.APPLICATION_FORM.Person_Id == payment.Person.Id && s.Activated).LastOrDefault();
                        if (list != null)
                        {
                            progId = list.Programme.Id;
                            deptId = list.Deprtment.Id;
                            levelId = GetLevel(list.Programme.Id);
                        }
                    }

                    if (progId > 0 && deptId > 0 && levelId > 0)
                    {
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == deptId && f.Fee_Type_Id == payment.FeeType.Id &&
                                                        f.Payment_Mode_Id == payment.PaymentMode.Id && f.Level_Id == nextLevelId && f.Programme_Id == progId && f.Session_Id == payment.Session.Id);

                        if (feeDetails != null && feeDetails.Count > 0)
                        {
                            return feeDetails.Sum(f => f.Fee.Amount);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return amt;
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
        private RemitaPayment CheckShortFallRemita(RemitaPayment remitaPayment, decimal amountToPay)
        {
            try
            {
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                RemitaPayment remitaPaymentShortfall = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == remitaPayment.payment.Person.Id && r.PAYMENT.Fee_Type_Id == (int)FeeTypes.ShortFall &&
                                                                                        r.PAYMENT.Session_Id == remitaPayment.payment.Session.Id && (r.Status.Contains("01") || r.Description.ToLower().Contains("manual"))).LastOrDefault();

                if (remitaPaymentShortfall != null)
                {
                    ShortFallLogic shortFallLogic = new ShortFallLogic();

                    ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.Payment_Id == remitaPaymentShortfall.payment.Id).LastOrDefault();
                    if (shortFall != null && shortFall.FeeType.Id == remitaPayment.payment.FeeType.Id && (remitaPayment.TransactionAmount + remitaPaymentShortfall.TransactionAmount) >= amountToPay)
                    {
                        return remitaPaymentShortfall;
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose,/* string MatricNumber,*/ string ApplicationFormNumber, string Programme, string Department, string session, string paymentMode)
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
                receipt.Date = remitaPayment.TransactionDate;
                receipt.ApplicationFormNumber = ApplicationFormNumber;
                //receipt.MatricNumber = MatricNumber;
                receipt.ProgrammeName = Programme;
                receipt.DepartmentName = Department;
                receipt.SessionName = session;
                receipt.Mode = paymentMode;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentEtranzact paymentEtranzact, decimal amount, string purpose, string Programme, string Department, string session, string paymentMode)
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
                receipt.ProgrammeName = Programme;
                receipt.DepartmentName = Department;
                receipt.SessionName = session;
                receipt.Mode = paymentMode;

                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AdmissionLetter(string fid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(fid));
            try
            {
                admissionLetter = GetAdmissionLetterBy(formId);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(admissionLetter);
        }

        public AdmissionLetter GetAdmissionLetterBy(long formId)
        {
            try
            {
                AdmissionLetter admissionLetter = null;
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);
                  

                if (applicationForm != null && applicationForm.Id > 0)
                {
                    AdmissionList list = new AdmissionList();
                    AdmissionListLogic listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(applicationForm.Id);

                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == (int)FeeTypes.SchoolFees);

                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == applicationForm.Person.Id);
                    if (appliedCourse == null)
                    {
                        throw new Exception("Applicant Applied Course cannot be found! Please contact your system administrator.");
                    }

                    admissionLetter = new AdmissionLetter();
                    admissionLetter.Person = applicationForm.Person;
                    admissionLetter.Session = applicationForm.Payment.Session;
                    admissionLetter.FeeDetails = feeDetails;
                    admissionLetter.Programme = list.Programme;
                    admissionLetter.Department = list.Deprtment;
                    admissionLetter.RegistrationEndDate = applicationForm.Setting.RegistrationEndDate;
                    admissionLetter.RegistrationEndTime = applicationForm.Setting.RegistrationEndTime;
                    admissionLetter.RegistrationEndTimeString = applicationForm.Setting.RegistrationEndTimeString;

                    if (admissionLetter.Session == null || admissionLetter.Session.Id <= 0)
                    {
                        throw new Exception("Session not set for this admission period! Please contact your system administrator.");
                    }
                    else if (!admissionLetter.RegistrationEndDate.HasValue)
                    {
                        throw new Exception("Registration End Date not set for this admission period! Please contact your system administrator.");
                    }
                    else if (!admissionLetter.RegistrationEndTime.HasValue)
                    {
                        throw new Exception("Registration End Time not set for this admission period! Please contact your system administrator.");
                    }

                    string programmeType = "National Diploma";
                    if (appliedCourse.Programme.Id == 1 || appliedCourse.Programme.Id == 2)
                    {
                        admissionLetter.ProgrammeType = programmeType;
                    }
                    else
                    {
                        admissionLetter.ProgrammeType = "Higher " + programmeType;
                    }
                }

                return admissionLetter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AdmissionSlip(string fid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(fid));
            try
            {
                admissionLetter = GetAdmissionLetterBy(formId);
            }
            catch(Exception)
            {
                throw;
            }

            return View(admissionLetter);
        }

        public ActionResult FinancialClearanceSlip(string pid)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(pid));
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetBy(paymentid);
                                

                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentHistory paymentHistory = new PaymentHistory();
                paymentHistory.Payments = paymentLogic.GetBy(student);
                paymentHistory.Student = student;

                return View(paymentHistory);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult Invoice(string pmid)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(pmid));
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(paymentid);
                //if (payment.FeeType.Id == (int)FeeTypes.SchoolFees || payment.FeeType.Id == (int)FeeTypes.CarryOverSchoolFees || payment.FeeType.Id == (int)FeeTypes.ConvocationFee)
                //{

                    Invoice invoice = new Invoice();
                    invoice.Person = payment.Person;
                    invoice.Payment = payment;

                    invoice.barcodeImageUrl = GenerateBarcode(paymentid);

                    Model.Model.Student student = new Model.Model.Student();
                    StudentLogic studentLogic = new StudentLogic();
                    student = studentLogic.GetBy(payment.Person.Id);

                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    PaymentEtranzactTypeLogic PaymentEtranzactTypeLogic = new Business.PaymentEtranzactTypeLogic();

                    StudentLevel studentLevel = null;//new StudentLevel();
                    StudentLevelLogic levelLogic = new StudentLevelLogic();
                    if(student != null)
                    {
                        studentLevel = levelLogic.GetModelsBy(sl => sl.Person_Id == student.Id && sl.Session_Id == payment.Session.Id).LastOrDefault();
                    }
                    if (studentLevel == null && student != null)
                    {
                        studentLevel = levelLogic.GetBy(student.Id); 
                    }

                    if (studentLevel != null && studentLevel.Id > 0)
                    {
                        invoice.MatricNumber = student.MatricNumber;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Level.Id, payment.PaymentMode.Id, studentLevel.Department.Id, payment.Session.Id);
                       // paymentEtranzactType = PaymentEtranzactTypeLogic.GetModelBy(p => p.Level_Id == studentLevel.Level.Id && p.Payment_Mode_Id == payment.PaymentMode.Id && p.Fee_Type_Id == payment.FeeType.Id && p.Programme_Id == studentLevel.Programme.Id && p.Session_Id == payment.Session.Id);
                       // invoice.paymentEtranzactType = paymentEtranzactType;

                        invoice.Department = studentLevel.Department;
                    }
                    else
                    {
                        AdmissionList list = new AdmissionList();
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        list = listLogic.GetBy(payment.Person);
                        if (list != null)
                        {
                            Level level = new Level();
                            level = SetLevel(list.Form.ProgrammeFee.Programme);
                            payment.FeeDetails = paymentLogic.SetFeeDetails(payment, list.Form.ProgrammeFee.Programme.Id, level.Id, payment.PaymentMode.Id, list.Deprtment.Id, payment.Session.Id);
                            paymentEtranzactType = PaymentEtranzactTypeLogic.GetModelBy(p => p.Level_Id == level.Id && p.Payment_Mode_Id == payment.PaymentMode.Id && p.Fee_Type_Id == payment.FeeType.Id && p.Programme_Id == list.Form.ProgrammeFee.Programme.Id && p.Session_Id == payment.Session.Id);
                            invoice.paymentEtranzactType = paymentEtranzactType;
                            invoice.Department = list.Deprtment;

                        }
                    }


                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                    if (paymentEtranzact != null)
                    {
                        invoice.Paid = true;
                    }

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
                        payment.FeeDetails.Add(new FeeDetail { Fee = new Fee { Amount = remitaPayment.TransactionAmount } });
                        invoice.Payment = payment;
                    }
                        

                        invoice.Amount = remitaPayment.TransactionAmount;
                    }


                PaymentScholarship scholarship = new PaymentScholarship();
                    PaymentScholarshipLogic scholarshipLogic = new PaymentScholarshipLogic();
                    if (scholarshipLogic.IsStudentOnScholarship(payment.Person, payment.Session))
                    {
                        scholarship = scholarshipLogic.GetBy(payment.Person);
                        invoice.paymentScholarship = scholarship;
                        invoice.Amount = payment.FeeDetails.Sum(p => p.Fee.Amount) - scholarship.Amount;

                    }


                    return View(invoice);
                //}
                           
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
        public ActionResult ShortFallInvoice(string pmid, string amount)
        {
            try
            {

                int paymentid = Convert.ToInt32(pmid);
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(paymentid);

                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                if ((payment.FeeType.Id == (int)FeeTypes.ShortFall) || (payment.FeeType.Id == (int)FeeTypes.Ewallet_Shortfall))
                {
                    if (TempData["FeeDetail"] != null)
                    {
                        payment.FeeDetails = (List<FeeDetail>)TempData["FeeDetail"];
                    }

                    if (TempData["ChangeCourseViewModel"] != null)
                    {
                        ChangeCourseViewModel changeCourseViewModel = (ChangeCourseViewModel)TempData["ChangeCourseViewModel"];
                        TempData["ChangeCourseViewModel"] = changeCourseViewModel;
                    }

                    Invoice invoice = new Invoice();
                    invoice.Person = payment.Person;
                    invoice.Payment = payment;

                    //PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    //PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();

                    //paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id);
                    //invoice.paymentEtranzactType = paymentEtranzactType;

                    //invoice.Amount = Convert.ToDecimal(amount);

                    //return View(invoice);
                    RemitaPayment remitaPayment = new RemitaPayment();
                    remitaPayment = remitaPaymentLogic.GetBy(payment.Id);

                    if (remitaPayment != null)
                    {
                        invoice.remitaPayment = remitaPayment;
                        if (remitaPayment.Status.Contains("01") || remitaPayment.Description.ToLower().Contains("manual"))
                        {
                            invoice.Paid = true;
                        }
                    }

                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetBy(payment.Person.Id);
                    if (student != null && student.MatricNumber != null)
                    {
                        invoice.MatricNumber = student.MatricNumber;
                    }
                    else
                    {
                        ApplicationFormLogic formLogic = new ApplicationFormLogic();
                        ApplicationForm form = formLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault();
                        if (form != null)
                        {
                            invoice.MatricNumber = form.Number;
                        }
                    }

                    invoice.barcodeImageUrl = GenerateBarcode(paymentid);
                    invoice.Amount = Convert.ToDecimal(amount);

                    TempData.Keep("ChangeCourseViewModel");

                    return View(invoice);
                }
                //else
                //{
                //    Person oldPerson = new Person();
                //    oldPerson = (Person)TempData["OldPerson"];
                //    PostJambViewModel viewModel = new PostJambViewModel();
                //    AppliedCourse appliedCourse = new AppliedCourse();
                //    ApplicationForm applicationForm = new Model.Model.ApplicationForm();
                //    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                //    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                //    Session session = new Model.Model.Session() { Id = 7 };
                //    SessionLogic sessionLogic = new SessionLogic();
                //    ApplicationFormSetting applicationFormSetting = new ApplicationFormSetting();
                //    ApplicantJambDetail applicantJambDetail = new ApplicantJambDetail();
                //    ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
                //    applicantJambDetail = applicantJambDetailLogic.GetModelBy(p => p.Person_Id == oldPerson.Id);
                //    ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();
                //    applicationFormSetting = applicationFormSettingLogic.GetModelBy(p => p.Application_Form_Setting_Id == 2);
                //    //session = sessionLogic.GetModelBy(p=>p.Activated == true);
                //    appliedCourse = appliedCourseLogic.GetModelBy(p => p.Person_Id == payment.Person.Id);
                //    applicationForm = applicationFormLogic.GetModelBy(p => p.Person_Id == payment.Person.Id);
                //    viewModel.AppliedCourse = appliedCourse;
                //    viewModel.Person = payment.Person;
                //    viewModel.Session = session;
                //    viewModel.Programme = appliedCourse.Programme;
                //    viewModel.ApplicationFormNumber = applicationForm.Number;
                //    viewModel.ApplicationFormSetting = applicationFormSetting;
                //    viewModel.ApplicantJambDetail = applicantJambDetail;
                //    viewModel.ApplicationForm = applicationForm;
                //    TempData["viewModel"] = viewModel;
                //    return RedirectToAction("PostJAMBSlip", new { controller = "Form", area = "Applicant" });
                //}
                return View();
            }
            catch (Exception)
            {
                throw;
            }

        }
        
        private Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch (programme.Id)
                {
                    case 1:
                        {
                            return level = new Level() { Id = 1 };

                        }
                    case 2:
                        {
                            return level = new Level() { Id = 1 };
                          
                        }
                    case 3:
                        {
                            return level = new Level() { Id = 3 };

                        }
                    case 4:
                        {
                            return level = new Level() { Id = 3 };

                        }
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult TranscriptInvoice(string pmid)
        {
            try
            {
                string type = Convert.ToString(TempData["RequestType"]);
                if (type == "")
                {
                    type = null;
                }
                TempData.Keep("RequestType");

                int paymentid = Convert.ToInt32(pmid);
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(paymentid);

                Invoice invoice = new Invoice();
                invoice.Person = payment.Person;
                invoice.Payment = payment;

                if (type == null || type == "Transcript Verification")
                {
                    invoice.Payment.FeeType.Name = "Transcript";
                }
                if (type == "Certificate Collection" || type == "Certificate Verification")
                {
                    invoice.Payment.FeeType.Name = "Certificate";
                }
                if (type == "Wes")
                {
                    invoice.Payment.FeeType.Name = "WES Verification";
                }

                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                if (remitaPayment != null)
                {
                    invoice.remitaPayment = remitaPayment;
                    invoice.Amount = remitaPayment.TransactionAmount;
                }

               TranscriptViewModel viewModel = new TranscriptViewModel();
                string hash = "538661740" + remitaPayment.RRR + "918567";
                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(hash);
                viewModel.Hash = remitaProcessor.HashPaymentDetailToSHA512(hash);
                viewModel.RemitaPayment = remitaPayment;
                viewModel.RemitaPayment.MerchantCode = "538661740";
                viewModel.RemitaPayment.RRR = remitaPayment.RRR;
               TempData["TranscriptViewModel"] = viewModel;
                return View(invoice);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult CardPayment()
        {
            TranscriptViewModel viewModel = (TranscriptViewModel)TempData["TranscriptViewModel"];
            TempData.Keep("TranscriptViewModel");

            return View(viewModel);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public ActionResult VerifySchoolFees()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifySchoolFees(Receipt model)
        {
            if (model.barcodeImageUrl != null)
            {
                int startIndex = model.barcodeImageUrl.IndexOf("pmid=");
                int pmid = Convert.ToInt32(model.barcodeImageUrl.Substring(startIndex).Split('=')[1]);
                if (pmid > 0)
                {
                    
                    Payment payment = new Payment() { Id = pmid };
                    var loggeduser = new UserLogic();
                    var paymentEtranzactLogic = new PaymentEtranzactLogic();
                    var paymentVerificationLogic = new PaymentVerificationLogic();
                    var paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                    RemitaPayment remitaPayment = new RemitaPayment();

                    if (paymentEtranzact != null)
                    {
                        var paymentVerification = paymentVerificationLogic.GetBy(pmid);
                        if (paymentVerification == null)
                        {
                            string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                            paymentVerification = new PaymentVerification();
                            paymentVerification.Payment = payment;
                            paymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                            paymentVerification.DateVerified = DateTime.Now;
                            paymentVerification.Comment = client;
                            paymentVerification = paymentVerificationLogic.Create(paymentVerification);
                            paymentVerification.Payment = payment;
                            paymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                            paymentVerification.DateVerified = DateTime.Now;
                            paymentVerification.Comment = client;
                        }
                    }
                    else
                    {
                        RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                        remitaPayment = remitaPaymentLogic.GetModelBy(p => p.Payment_Id == pmid && p.Status.Contains("01:"));
                        if (remitaPayment != null)
                        {
                            var paymentVerification = paymentVerificationLogic.GetBy(pmid);
                            if (paymentVerification == null)
                            {
                                string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                                paymentVerification = new PaymentVerification();
                                paymentVerification.Payment = payment;
                                paymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                paymentVerification.DateVerified = DateTime.Now;
                                paymentVerification.Comment = client;
                                paymentVerification = paymentVerificationLogic.Create(paymentVerification);
                                paymentVerification.Payment = payment;
                                paymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                paymentVerification.DateVerified = DateTime.Now;
                                paymentVerification.Comment = client;
                            }
                        }
                    }

                    if (paymentEtranzact == null && remitaPayment == null)
                    {
                        SetMessage("Payment has not been made.", Message.Category.Warning);
                    }
                    else
                    {
                        return RedirectToAction("Receipt", "Credential", new { pmid = pmid });
                    }
                }
                else
                {
                    SetMessage("Payment Could not be verified! Please ensure that student has made payment", Message.Category.Warning);
                }
            }

            return View();
        }
        public ActionResult FeeVerificationReport()
        {
            return View();
        }
        public bool HasPaidSchoolFees(Payment payment, decimal AmountToPay)
        {
            long progId = 0;
            long deptId = 0;
            long levelId = 0;
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == payment.Session.Id).LastOrDefault();
            RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == payment.Id && r.Status.Contains("01:"));
            if (studentLevel != null)
            {
                progId = studentLevel.Programme.Id;
                deptId = studentLevel.Department.Id;
                levelId = studentLevel.Level.Id;
            }
            if (progId == (int)Programmes.HNDPartTime || progId == (int)Programmes.NDPartTime)
            {
                decimal eightyPercentOfFirstInstallment = AmountToPay * (80M / 100M);
                if (remitaPayment != null && remitaPayment.TransactionAmount >= eightyPercentOfFirstInstallment)
                    return true;
                return false;
            }
            else if (progId == (int)Programmes.HNDEvening || progId == (int)Programmes.NDEveningFullTime)
            {
                decimal fiftyPercentOfFullPayment = AmountToPay * (45M / 100M);
                if (remitaPayment != null && remitaPayment.TransactionAmount >= fiftyPercentOfFullPayment)
                    return true;
                return false;
            }
            else
            {
                if (remitaPayment.TransactionAmount >= AmountToPay)
                    return true;
                return false;
            }
        }
        public ActionResult TranscriptReciept(long pmid)
        {
            Payment payment = new Payment();
            PaymentLogic paymentLogic = new PaymentLogic();
            RemitaPayment remitaPayment = new RemitaPayment();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            Receipt receipt = null;

            try
            {
                receipt = GetTranscriptReceiptBy(pmid);
                if (receipt == null)
                {
                    SetMessage("No receipt found!", Message.Category.Error);
                }
                else
                {
                    //http://localhost:2720/

                    //receipt.barcodeImageUrl = "https://students.fedpolyado.edu.ng/Common/Credential/TranscriptReciept?pmid=" + pmid; ;
                    receipt.barcodeImageUrl = "Name:" + receipt.Name + "  " + "Amount:" + receipt.Amount + " " + "RRR:" + receipt.ConfirmationOrderNumber + "Date Paid:" + receipt.Date + " " + "Fee Type:" + receipt.Purpose;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }
        public Receipt GetTranscriptReceiptBy(long pmid)
        {
            Receipt receipt = null;
            PaymentLogic paymentLogic = new PaymentLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
            var department = "";
            var programme = "";

            try
            {
                Payment payment = paymentLogic.GetBy(pmid);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }

               
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelsBy(o => o.Payment_Id == payment.Id).FirstOrDefault();
                    if (remitaPayment != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")))
                    {
                    var request=transcriptRequestLogic.GetModelsBy(f => f.Payment_Id == remitaPayment.payment.Id).FirstOrDefault();
                    if (request?.Id > 0)
                    {
                        decimal amount = remitaPayment.TransactionAmount;
                        Abundance_Nk.Model.Model.Student student = new Model.Model.Student();
                        StudentLogic studentLogic = new StudentLogic();
                        student = studentLogic.GetBy(payment.Person.Id);
                       
                        Model.Model.Student getstudent = studentLogic.GetBy(remitaPayment.payment.Person.Id);
                        
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            StudentLevel studentLevel = new StudentLevel();
                            studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel?.Id > 0)
                        {
                            programme = studentLevel.Programme.Name;
                            department = studentLevel.Department.Name;
                        }

                        receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, "", programme, department, payment.Session.Name, payment.PaymentMode.Name);
                    }
                }
                        
               
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}