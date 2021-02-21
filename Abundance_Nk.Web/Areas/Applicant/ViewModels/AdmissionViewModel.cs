using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class AdmissionViewModel : OLevelResultViewModel
    {
        private PaymentLogic paymentLogic;
        private PaymentEtranzactLogic paymentEtranzactLogic;
        private PaymentEtranzactTypeLogic paymentEtranzactTypeLogic;
        private ApplicantLogic applicantLogic;
        private AppliedCourseLogic appliedCourseLogic;
        private ApplicationFormLogic applicationFormLogic;
        private OnlinePaymentLogic onlinePaymentLogic;
        private AdmissionListLogic admissionListLogic;
        private RemitaPaymentLogic remitaPaymentLogic;
        public AdmissionViewModel()
        {
            ApplicationForm = new ApplicationForm();
            ApplicationForm.Person = new Person();

            Applicant = new Abundance_Nk.Model.Model.Applicant();
            Applicant.Status = new ApplicantStatus();
            
            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            admissionList = new AdmissionList();
            admissionList.Deprtment = new Department();
            Payment = new Payment();
            paymentEtranzactLogic = new PaymentEtranzactLogic();
            paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();

            Invoice = new Invoice();
            Invoice.Payment = new Payment();
            Invoice.Payment.FeeType = new FeeType();
            //Invoice.Payment.Fee.Type = new FeeType();
            Invoice.Person = new Person();

            paymentLogic = new PaymentLogic();
            appliedCourseLogic = new AppliedCourseLogic();
            applicationFormLogic = new ApplicationFormLogic();
            applicantLogic = new ApplicantLogic();
            onlinePaymentLogic = new OnlinePaymentLogic();
            admissionListLogic = new AdmissionListLogic();

            ScratchCard = new ScratchCard();
            ChangeOfCourseApplies = false;
        }

        public bool Loaded { get; set; }
        public ScratchCard ScratchCard { get; set; }
        public Remita remita { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public RemitaResponse remitaResponse { get; set; }
        public Receipt Receipt { get; set; }
        public Invoice Invoice { get; set; }
        public Abundance_Nk.Model.Model.Applicant Applicant { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public AdmissionList admissionList { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        
        public Payment Payment { get; set; }
        public bool IsAdmitted { get; set; }
        public int ApplicantStatusId { get; set; }

        [Display(Name = "Acceptance RRR")]
        public string AcceptanceConfirmationOrderNumber { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        public string RRRNumber { get; set; }

        [Display(Name = "School Fees RRR")]
        public string SchoolFeesConfirmationOrderNumber { get; set; }

        [Display(Name = "Acceptance Invoice Number")]
        public string AcceptanceInvoiceNumber { get; set; }

        public bool ChangeOfCourseApplies { get; set; }

        [Display(Name = "Acceptance Receipt Number")]
        public string AcceptanceReceiptNumber { get; set; }

        [Display(Name = "School Fees Invoice Number")]
        public string SchoolFeesInvoiceNumber { get; set; }

        [Display(Name = "School Fees Receipt Number")]
        public string SchoolFeesReceiptNumber { get; set; }

        public string ConfirmationOrderNumber { get; set; }
                
        public void GetApplicationBy(long formId)
        {
            try
            {
                remitaPayment = new RemitaPayment();
                remitaPaymentLogic = new RemitaPaymentLogic();
                ApplicationForm = applicationFormLogic.GetModelBy(a => a.Application_Form_Id == formId);
                if (ApplicationForm != null && ApplicationForm.Id > 0)
                {
                    AppliedCourse = appliedCourseLogic.GetModelBy(f => f.Application_Form_Id == ApplicationForm.Id);
                    admissionList = admissionListLogic.GetBy(ApplicationForm.Id);
                    Payment = paymentLogic.GetModelBy(p => p.Payment_Id == ApplicationForm.Payment.Id);
                    Applicant = applicantLogic.GetModelBy(a => a.Application_Form_Id == ApplicationForm.Id);
                    IsAdmitted = admissionListLogic.IsAdmitted(ApplicationForm);

                    if (Applicant != null && Applicant.Status != null)
                    {
                        ApplicantStatusId = Applicant.Status.Id;
                    }

                    if (admissionList != null && AppliedCourse.Department != admissionList.Deprtment)
                    {
                        ChangeOfCourseApplies = true;
                    }
                    
                    //update applicant status
                    if (IsAdmitted && ApplicantStatusId <= (int)ApplicantStatus.Status.SubmittedApplicationForm)
                        applicantLogic.UpdateStatus(ApplicationForm, ApplicantStatus.Status.OfferedAdmission);

                    //get acceptance payment
                    FeeType acceptanceFee = new FeeType() { Id = (int)FeeTypes.AcceptanceFee };
                    Payment acceptancePayment = paymentLogic.GetBy(ApplicationForm.Person, acceptanceFee);
                    if (acceptancePayment == null)
                    {
                        acceptanceFee = new FeeType() { Id = 9 };
                        acceptancePayment = paymentLogic.GetBy(ApplicationForm.Person, acceptanceFee);
                    }


                    if (acceptancePayment != null)
                    {
                        AcceptanceInvoiceNumber = acceptancePayment.InvoiceNumber;
                        remitaPayment = remitaPaymentLogic.GetBy(acceptancePayment.Id);

                        if (remitaPayment != null && (remitaPayment.Status.Contains("01:") || remitaPayment.Description.ToLower().Contains("manual")))
                        {
                            AcceptanceReceiptNumber = remitaPayment.OrderId;
                            if (ApplicantStatusId >= (int)ApplicantStatus.Status.ClearedAndAccepted || ApplicantStatusId == (int)ApplicantStatus.Status.GeneratedAcceptanceReceipt)
                            {
                                AcceptanceConfirmationOrderNumber = remitaPayment.RRR;
                            }
                        }
                    }


                    //get school fees payment
                    FeeType schoolFees = new FeeType() { Id = (int)FeeTypes.SchoolFees };
                    Payment schoolFeesPayment = paymentLogic.GetBy(ApplicationForm.Person, schoolFees);
                    if (schoolFeesPayment != null)
                    {
                        SchoolFeesInvoiceNumber = schoolFeesPayment.InvoiceNumber;
                        remitaPayment = remitaPaymentLogic.GetBy(schoolFeesPayment.Id);
                        if (remitaPayment != null && remitaPayment.Status.Contains("01:"))
                        {
                            SchoolFeesReceiptNumber = remitaPayment.OrderId;
                            if (ApplicantStatusId >= (int)ApplicantStatus.Status.GeneratedSchoolFeesReceipt)
                            {
                                SchoolFeesConfirmationOrderNumber = remitaPayment.RRR;
                            }
                        }
                    }

                    Loaded = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormBy(string formNumber)
        {
            try
            {
                return applicationFormLogic.GetModelsBy(f => f.Application_Form_Number == formNumber).LastOrDefault();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void GetInvoiceBy(string invoiceNumber)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic levelLogic = new StudentLevelLogic();
                Department department = new Department();
                if (payment.FeeType.Id == (int)FeeTypes.SchoolFees || payment.FeeType.Id == (int)FeeTypes.AcceptanceFee || payment.FeeType.Id == (int)FeeTypes.HNDAcceptance)
                { 
                    studentLevel = levelLogic.GetBy(payment.Person.Id);
                    if (studentLevel != null)
                    {
                        department = studentLevel.Department;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Level.Id, 1, studentLevel.Department.Id, studentLevel.Session.Id);
                    }
                    else
                    {
                        AdmissionList list = new AdmissionList();
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        list = listLogic.GetBy(payment.Person);
                        if (list != null)
                        {
                            Level level = new Level();
                            level = SetLevel(list.Programme);
                            department = list.Deprtment;
                            payment.FeeDetails = paymentLogic.SetFeeDetails(payment, list.Programme.Id, level.Id, 1, list.Deprtment.Id, list.Form.Setting.Session.Id);
                        }
                    }

                }
                else if (payment.FeeType.Id == (int)FeeTypes.ChangeOfCourseFees)
                {   
                    AppliedCourse appliedCourse = new AppliedCourse();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(payment.Person);

                    if (appliedCourse != null)
                    {
                        Level level = new Level();
                        level = SetLevel(appliedCourse.Programme);
                        department = appliedCourse.Department;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id, level.Id, 1, appliedCourse.Department.Id, appliedCourse.ApplicationForm.Setting.Session.Id);
                      
                    }
                }
               

                Invoice = new Invoice();
                Invoice.Payment = payment;
                Invoice.Person = payment.Person;
                Invoice.JambRegistrationNumber ="";
                Invoice.Department = department;

                remitaPayment = new RemitaPayment();
                remitaPaymentLogic = new RemitaPaymentLogic();
                remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                if (remitaPayment != null)
                {
                    Invoice.remitaPayment = remitaPayment;
                }

                studentLevel = levelLogic.GetBy(payment.Person.Id);
                if (payment.FeeType.Id == (int)FeeTypes.SchoolFees || payment.FeeType.Id == (int)FeeTypes.AcceptanceFee || payment.FeeType.Id == (int)FeeTypes.HNDAcceptance)
                {
                    AdmissionList admissionList = new AdmissionList();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    admissionList = admissionListLogic.GetBy(payment.Person);
                    if (admissionList != null && studentLevel == null)
                    {
                        Level thisLevel = new Level();
                        thisLevel = SetLevel(admissionList.Programme);
                        Invoice.paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Level_Id == thisLevel.Id && p.Programme_Id == admissionList.Form.ProgrammeFee.Programme.Id && p.Session_Id == payment.Session.Id);
                    }
                    if (studentLevel != null)
                    {
                        Invoice.paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Level_Id == studentLevel.Level.Id && p.Programme_Id == studentLevel.Programme.Id && p.Session_Id == payment.Session.Id);
                    }
                }
                else if (payment.FeeType.Id == (int)FeeTypes.ChangeOfCourseFees)
                {
                    Invoice.paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id);
                }
                else
                {
                    Invoice.paymentEtranzactType = paymentEtranzactTypeLogic.GetBy(payment.FeeType); 
                }   

                AcceptanceInvoiceNumber = payment.InvoiceNumber;
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
                            return level = new Level() { Id = 5 };

                        }
                    case 3:
                        {
                            return level = new Level() { Id = 3 };

                        }
                    case 4:
                        {
                            return level = new Level() { Id = 8 };

                        }
                    case 5:
                        {
                            return level = new Level() { Id = 11};

                        }
                    case 6:
                        {
                            return level = new Level() { Id = 13 };

                        }
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt GetReceiptBy(string invoiceNumber)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                if (payment == null ||payment.Id <= 0)
                {
                    return null;
                }
                var MatricNumber = "";
                        var ApplicationFormNumber = "";
                        var department = "";
                        var programme = "";
                 PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(o => o.Payment_Id == payment.Id);
                 if (paymentEtranzact != null)
                 {
                     if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                     {
                         throw new Exception("Fee Details for " + payment.FeeType.Name + " not set! please contact your system administrator.");
                     }
                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetBy(paymentEtranzact.Payment.Payment.Person.Id);
                    if (student != null)
                    {
                        
                        MatricNumber = student.MatricNumber;
                        ApplicationFormNumber = student.ApplicationForm.Number;
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
                    decimal amount = (decimal)paymentEtranzact.TransactionAmount;
                     Receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, paymentEtranzact, amount, payment.FeeType.Name,programme,department,payment.Session.Name);
                 }
                 else
                 {
                     remitaPayment = new RemitaPayment();
                     remitaPaymentLogic = new RemitaPaymentLogic();
                     remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                     if (remitaPayment != null && (remitaPayment.Status.Contains("021:") || remitaPayment.Description.ToLower().Contains("manual")))
                     {
                        //if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                        //{
                        //    throw new Exception("Fee Details for " + payment.FeeType.Name + " not set! please contact your system administrator.");
                        //}


                        decimal amountToPay = Utility.GetAmountToPay(remitaPayment.payment);

                        if (amountToPay > 0M && amountToPay > remitaPayment.TransactionAmount)
                        {
                            ShortFallLogic shortFallLogic = new ShortFallLogic();
                            ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.Fee_Type_Id == remitaPayment.payment.FeeType.Id &&
                                                  s.PAYMENT.Session_Id == remitaPayment.payment.Session.Id && s.PAYMENT.Person_Id == remitaPayment.payment.Person.Id).LastOrDefault();
                            if (shortFall != null)
                            {
                                RemitaPayment shortFallRemitaPayment = Utility.CheckShortFallRemita(remitaPayment, amountToPay);
                                if (shortFallRemitaPayment != null)
                                {
                                    remitaPayment.TransactionAmount += shortFallRemitaPayment.TransactionAmount;
                                    remitaPayment.RRR += ", " + shortFallRemitaPayment.RRR;
                                }
                            }
                        }
                        StudentLogic studentLogic = new StudentLogic();
                        Model.Model.Student student = studentLogic.GetBy(remitaPayment.payment.Person.Id);
                        if (student != null)
                        {

                            MatricNumber = student.MatricNumber;
                            ApplicationFormNumber = student.ApplicationForm.Number;
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
                            AdmissionListLogic appliedCourseLogic = new AdmissionListLogic();
                            AdmissionList admissionList = appliedCourseLogic.GetBy(remitaPayment.payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;

                            }
                        }
                        decimal amount = remitaPayment.TransactionAmount;
                         Receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, "", "",programme,department, payment.Session.Name);

                    }
                 }
                return Receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PaymentMode PaymentMode { get; set; }
        //public Payment GenerateInvoice(ApplicationForm applicationForm, ApplicantStatus.Status status, FeeType feeType)
        //{
        //    try
        //    {
        //        Payment payment = new Payment();
        //        payment.PaymentMode = new PaymentMode() { Id = applicationForm.Setting.PaymentMode.Id };
        //        payment.PaymentType = new PaymentType() { Id = applicationForm.Setting.PaymentType.Id };
        //        payment.PersonType = new PersonType() { Id = applicationForm.Setting.PersonType.Id };
        //        payment.Person = applicationForm.Person;
        //        payment.DatePaid = DateTime.Now;
        //        payment.FeeType = feeType;
        //        payment.Session = applicationForm.Setting.Session;

        //        if (paymentLogic.PaymentAlreadyMade(payment))
        //        {
        //            return paymentLogic.GetBy(applicationForm.Person, feeType);
        //        }
        //        else
        //        {
        //            Payment newPayment = null;
        //            OnlinePayment newOnlinePayment = null;
        //            using (TransactionScope transaction = new TransactionScope())
        //            {
        //                newPayment = paymentLogic.Create(payment);
        //                if (newPayment != null)
        //                {
        //                    if (feeType.Id == 3)
        //                    {

        //                        AdmissionList list = new AdmissionList();
        //                        AdmissionListLogic listLogic = new AdmissionListLogic();
        //                        list = listLogic.GetBy(applicationForm.Id);
        //                        int LevelId = GetLevel(applicationForm.ProgrammeFee.Programme.Id);
        //                        newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, applicationForm.ProgrammeFee.Programme.Id, LevelId, 1, list.Deprtment.Id, applicationForm.Setting.Session.Id);

        //                    }
        //                    else if (feeType.Id == 2 && applicationForm.ProgrammeFee.Programme.Id > 1)
        //                    {
        //                        feeType.Id = 9;
        //                        newPayment.FeeDetails = paymentLogic.SetFeeDetails(feeType);
        //                    }

        //                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
        //                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
        //                    OnlinePayment onlinePayment = new OnlinePayment();
        //                    onlinePayment.Channel = channel;
        //                    onlinePayment.Payment = newPayment;
        //                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
        //                }

        //                applicantLogic.UpdateStatus(applicationForm, status);
        //                transaction.Complete();
        //            }

        //            return newPayment;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public Payment GenerateInvoice(ApplicationForm applicationForm, ApplicantStatus.Status status, FeeType feeType)
        {
            try
            {
                Payment payment = new Payment();
                payment.PaymentMode = new PaymentMode() { Id = applicationForm.Setting.PaymentMode.Id };
                payment.PaymentType = new PaymentType() { Id = applicationForm.Setting.PaymentType.Id };
                payment.PersonType = new PersonType() { Id = applicationForm.Setting.PersonType.Id };
                payment.Person = applicationForm.Person;
                payment.DatePaid = DateTime.Now;
                payment.FeeType = feeType;
                payment.Session = applicationForm.Setting.Session;

                if (paymentLogic.PaymentAlreadyMade(payment))
                {
                    return paymentLogic.GetBy(applicationForm.Person, feeType);
                }
                else
                {
                    Payment newPayment = null;
                    OnlinePayment newOnlinePayment = null;
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        newPayment = paymentLogic.Create(payment);

                        AdmissionList list = new AdmissionList();
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        list = listLogic.GetBy(applicationForm.Id);
                        //Get the programme to which the student is admitted into
                        var admission=listLogic.GetModelsBy(f => f.Application_Form_Id == applicationForm.Id).FirstOrDefault();
                        int LevelId = 0;
                        if (admission != null)
                        {
                            LevelId = GetLevel(admission.Programme.Id);
                        }
                        else
                        {
                            LevelId = GetLevel(applicationForm.ProgrammeFee.Programme.Id);
                        }
                        

                        if (newPayment != null)
                        {
                            if(admission!=null && admission.Id > 0)
                            {
                                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, admission.Programme.Id, LevelId, 1, list.Deprtment.Id, applicationForm.Setting.Session.Id);
                            }
                            else
                            {
                                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, applicationForm.ProgrammeFee.Programme.Id, LevelId, 1, list.Deprtment.Id, applicationForm.Setting.Session.Id);
                            }

                            PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                            OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                            OnlinePayment onlinePayment = new OnlinePayment();
                            onlinePayment.Channel = channel;
                            onlinePayment.Payment = newPayment;
                            newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                        }

                        applicantLogic.UpdateStatus(applicationForm, status);
                        transaction.Complete();
                    }

                    return newPayment;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Payment GenerateInvoice(ApplicationForm applicationForm, ApplicantStatus.Status status, FeeType feeType, PaymentMode paymentMode)
        {
            try
            {
                Payment payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = new PaymentType() { Id = applicationForm.Setting.PaymentType.Id };
                payment.PersonType = new PersonType() { Id = applicationForm.Setting.PersonType.Id };
                payment.Person = applicationForm.Person;
                payment.DatePaid = DateTime.Now;
                payment.FeeType = feeType;
                payment.Session = applicationForm.Setting.Session;

                if (paymentLogic.PaymentAlreadyMade(payment))
                {
                    return paymentLogic.GetBy(applicationForm.Person, feeType, applicationForm.Setting.Session);
                }
                else
                {
                    Payment newPayment = null;
                    OnlinePayment newOnlinePayment = null;
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        newPayment = paymentLogic.Create(payment);

                        AdmissionList list = new AdmissionList();
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        list = listLogic.GetBy(applicationForm.Id);

                        int LevelId = GetLevel(applicationForm.ProgrammeFee.Programme.Id);

                        if (newPayment != null)
                        {
                            newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, applicationForm.ProgrammeFee.Programme.Id, LevelId, payment.PaymentMode.Id, list.Deprtment.Id, applicationForm.Setting.Session.Id);

                            PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                            OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                            OnlinePayment onlinePayment = new OnlinePayment();
                            onlinePayment.Channel = channel;
                            onlinePayment.Payment = newPayment;
                            newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                        }

                        applicantLogic.UpdateStatus(applicationForm, status);
                        transaction.Complete();
                    }

                    newPayment.PaymentMode = payment.PaymentMode;

                    return newPayment;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt GenerateReceipt(string invoiceNumber, long formId, ApplicantStatus.Status status)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }

                Receipt receipt = null;
                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    remitaPayment = new RemitaPayment();
                    remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                    if (remitaPayment != null && (remitaPayment.Status.Contains("021:") || remitaPayment.Description.ToLower().Contains("manual")))
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            bool updated = onlinePaymentLogic.UpdateTransactionNumber(payment, remitaPayment.RRR);
                            applicantLogic.UpdateStatus(applicationForm, status);

                            transaction.Complete();
                        }


                        decimal amountToPay = Utility.GetAmountToPay(remitaPayment.payment);

                        if (amountToPay > 0M && amountToPay > remitaPayment.TransactionAmount)
                        {
                            ShortFallLogic shortFallLogic = new ShortFallLogic();
                            ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.Fee_Type_Id == remitaPayment.payment.FeeType.Id &&
                                                  s.PAYMENT.Session_Id == remitaPayment.payment.Session.Id && s.PAYMENT.Person_Id == remitaPayment.payment.Person.Id).LastOrDefault();
                            if (shortFall != null)
                            {
                                RemitaPayment shortFallRemitaPayment = Utility.CheckShortFallRemita(remitaPayment, amountToPay);
                                if (shortFallRemitaPayment != null)
                                {
                                    remitaPayment.TransactionAmount += shortFallRemitaPayment.TransactionAmount;
                                    remitaPayment.RRR += ", " + shortFallRemitaPayment.RRR;
                                }
                            }
                        }

                        decimal amount = remitaPayment.TransactionAmount;
                        var GetProgrammeAndDept = admissionListLogic.GetModelsBy(d => d.Application_Form_Id == applicationForm.Id).FirstOrDefault();
                        receipt = BuildReceipt(applicationForm.Person.FullName, invoiceNumber, remitaPayment, amount, payment.FeeType.Name, "", applicationForm.Number, GetProgrammeAndDept.Programme.Name, GetProgrammeAndDept.Deprtment.Name, payment.Session.Name);
                    }
                    else
                    {
                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                        if (paymentEtranzact != null)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                bool updated = onlinePaymentLogic.UpdateTransactionNumber(payment, paymentEtranzact.ConfirmationNo);
                                applicantLogic.UpdateStatus(applicationForm, status);

                                transaction.Complete();
                            }

                            decimal amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                            var GetProgrammeAndDept=admissionListLogic.GetModelsBy(d => d.Application_Form_Id == applicationForm.Id).FirstOrDefault();
                            receipt = BuildReceipt(applicationForm.Person.FullName, invoiceNumber, paymentEtranzact, amount, payment.FeeType.Name, GetProgrammeAndDept.Programme.Name, GetProgrammeAndDept.Deprtment.Name, payment.Session.Name);
                        }
                    }
                }

                return receipt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string Programme, string Department, string session)
        {
            try
            {
                Receipt receipt = new Receipt();
                receipt.Number = remitaPayment.OrderId;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = remitaPayment.RRR;
                receipt.Amount = amount;
                receipt.AmountInWords = "";
                receipt.Purpose = purpose;
                receipt.Date = remitaPayment.TransactionDate;
                receipt.ApplicationFormNumber = ApplicationFormNumber;
                receipt.MatricNumber = MatricNumber;
                receipt.ProgrammeName = Programme;
                receipt.DepartmentName = Department;
                receipt.SessionName = session;
                return receipt;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentEtranzact paymentEtranzact, decimal amount, string purpose, string Programme, string Department, string session)
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

                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
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
        
    }


}