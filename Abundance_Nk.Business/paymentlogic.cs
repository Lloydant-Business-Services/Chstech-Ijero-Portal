using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class PaymentLogic :BusinessBaseLogic<Payment,PAYMENT>
    {
        private FeeDetailLogic feeDetailLogic;

        public PaymentLogic()
        {
            feeDetailLogic = new FeeDetailLogic();
            translator = new PaymentTranslator();
        }

        public List<PaymentView> GetBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT>(p => p.Person_Id == person.Id && p.Confirmation_No != null)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Receipt_No,
                                                  ConfirmationOrderNumber = p.Confirmation_No,
                                                  BankCode = p.Bank_Code,
                                                  BankName = p.Bank_Name,
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = p.Branch_Name,
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                              }).ToList();
                return payments;

            }
            catch(Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetEtranzactPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Receipt_No,
                                                  ConfirmationOrderNumber = p.Confirmation_No,
                                                  BankCode = p.Bank_Code,
                                                  BankName = p.Bank_Name,
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = p.Branch_Name,
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                              }).ToList();
                return payments;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PaymentView> GetBy(RemitaPayment remitaPayment)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_REMITA_PAYMENT>(p => p.Person_Id == remitaPayment.payment.Person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.RRR,
                                                  BankCode = p.Bank_Code,
                                                  BankName = "",
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = "",
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                              }).ToList();
                return payments;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<PaymentView> GetRemitaPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_REMITA_PAYMENT>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.RRR,
                                                  BankCode = p.Bank_Code,
                                                  BankName = "",
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = "",
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                              }).ToList();
                return payments;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AcceptanceView> GetRemitaReportBy(Session session,Department department,Programme programme)
        {
            try
            {
                List<AcceptanceView> payments = (from p in repository.GetBy<ACCPTANCE__REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id)
                                                 select new AcceptanceView
                                                 {
                                                     Person_Id = Convert.ToInt64(p.Person_Id),
                                                     Application_Exam_Number = p.Application_Exam_Number,
                                                     Invoice_Number = p.Invoice_Number,
                                                     Application_Form_Number = p.Application_Form_Number,
                                                     First_Choice_Department_Name = p.Department_Name,
                                                     Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                                     RRR = p.Department_Option_Name,
                                                     Programme_Name = p.Programme_Name,
                                                     DepartmentOption = p.Department_Option_Name
                                                 }).OrderBy(b => b.Name).ToList();
                return payments;

            }
            catch(Exception)
            {
                throw;
            }
        }
        //public List<FeesPaymentReport> GetFeesPaymentBy(Session session,Programme programme,Department department,Level level)
        //{
        //    try
        //    {

        //        List<FeesPaymentReport> feesPaymentReportList = (from a in repository.GetBy<VW_ALL_PAYMENT_REPORT_NEW_STUDENTS>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Level_Id == level.Id)
        //                                     select new FeesPaymentReport
        //                                     {
        //                                         AcceptanceFeeInvoiceNumber = a.ACCEPTANCE_FEE_INVOICE.ToString(),
        //                                         AcceptanceTransactionAmount = a.ACCEPTANCE_FEE_AMOUNT.ToString(),
        //                                         ApplicationFormInvoiceNumber = a.APPLICATION_FORM_INVOICE.ToString(),
        //                                         ApplicationFormAmount = a.APPLICATION_FEE_AMOUNT.ToString(),
        //                                         FirstYearFeesTransactionAmount = a.FIRST_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
        //                                         FirstYearSchoolFeesInvoiceNumber = a.FIRST_YEAR_FEE_INVOICE.ToString(),
        //                                         SecondYearFeesTransactionAmount = a.SECOND_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
        //                                         SecondYearSchoolFeesInvoiceNumber = a.SECOND_YEAR_FEE_INVOICE.ToString(),
        //                                         ApplicationNumber = a.Application_Form_Number,
        //                                         MatricNumber = a.Matric_Number,
        //                                         Name = a.Last_Name.ToUpper() + " " + a.First_Name.ToUpper() + " " + a.Other_Name.ToUpper(),
        //                                         Session = a.Session_Name,
        //                                         Programme = a.Programme_Name,
        //                                         Department = a.Department_Name,
        //                                         Level = a.Last_Name

        //                                     }).ToList();



        //        return feesPaymentReportList.OrderBy(p => p.Name).ToList();
        //    }
        //    catch(Exception)
        //    {
        //        throw;
        //    }
        //}

        public Payment GetBy(Person person,FeeType feeType)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Person_Id == person.Id && p.Fee_Type_Id == feeType.Id;
                Payment payment = GetModelsBy(selector).FirstOrDefault();

                SetFeeDetails(payment);

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public Payment GetBy(Person person, FeeType feeType, Session session)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Person_Id == person.Id && p.Fee_Type_Id == feeType.Id && p.Session_Id == session.Id;
                Payment payment = GetModelsBy(selector).LastOrDefault();
                SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Payment GetBy(long id)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Payment_Id == id;
                Payment payment = GetModelBy(selector);

                SetFeeDetails(payment);

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Payment GetBy(FeeType feeType,Person person,Session session)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Fee_Type_Id == feeType.Id && p.Person_Id == person.Id && p.Session_Id == session.Id;
                Payment payment = GetModelsBy(selector).FirstOrDefault();

                SetFeeDetails(payment);

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public Payment GetBy(FeeType feeType, Person person, Session session, PaymentMode paymentMode)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Fee_Type_Id == feeType.Id && p.Person_Id == person.Id && p.Session_Id == session.Id && p.Payment_Mode_Id == paymentMode.Id;
                Payment payment = GetModelsBy(selector).FirstOrDefault();

                SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetFeeDetails(Payment payment)
        {
            try
            {
                if(payment != null && payment.Id > 0)
                {
                    payment.FeeDetails = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == payment.FeeType.Id);

                }
            }
            catch(Exception)
            {
                throw;
            }
        }
        public void SetFeeDetails(Payment payment,Int32? ProgrammeId,Int32? DepartmentId,Int32? SessionId)
        {
            try
            {
                if(payment != null && payment.Id > 0)
                {
                    payment.FeeDetails = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == 9 && f.Programme_Id == ProgrammeId && f.Department_Id == DepartmentId && f.Session_Id == SessionId);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public List<FeeDetail> SetFeeDetails(Payment payment,Int32? ProgrammeId,Int32? LevelId,Int32? PaymentModeId,Int32? DepartmentId,Int32? SessionId)
        {
            List<FeeDetail> feedetail = new List<FeeDetail>();
            try
            {
                if(payment != null && payment.Id > 0)
                {
                    feedetail = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == payment.FeeType.Id && f.Programme_Id == ProgrammeId && f.Level_Id == LevelId && f.Payment_Mode_Id == PaymentModeId && f.Department_Id == DepartmentId && f.Session_Id == SessionId);
                }
            }
            catch(Exception)
            {
                throw;
            }
            return feedetail;
        }

        public List<FeeDetail> SetFeeDetails(FeeType feeType)
        {
            List<FeeDetail> feedetail = new List<FeeDetail>();
            try
            {
                if(feeType != null && feeType.Id > 0)
                {
                    feedetail = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == feeType.Id);
                }
            }
            catch(Exception)
            {
                throw;
            }
            return feedetail;
        }

        public List<FeeDetail> SetFeeDetails(long FeeId)
        {
            List<FeeDetail> feedetail = new List<FeeDetail>();
            try
            {
                if(FeeId > 0)
                {
                    feedetail = feeDetailLogic.GetModelsBy(f => f.Fee_Id == FeeId);
                }
            }
            catch(Exception)
            {
                throw;
            }
            return feedetail;
        }

        public Payment GetBy(string invoiceNumber)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Invoice_Number == invoiceNumber;
                Payment payment = GetModelBy(selector);

                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = new StudentLevel();
                PaymentMode paymentMode = new PaymentMode(){ Id = 1};
                
                studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == payment.Session.Id).LastOrDefault();
                if (studentLevel == null)
                {
                    studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault(); 
                }
                
                AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == payment.Person.Id);
                Session session = payment.Session;

                if (studentLevel != null)
                {

                    if (studentLevel != null)
                    {
                        payment.FeeDetails = SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Level.Id, paymentMode.Id, studentLevel.Department.Id, payment.Session.Id);
                        //if (studentLevel.Programme.Id == 2 || studentLevel.Programme.Id == 3)
                        //{
                        //    SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Department.Id, session.Id);
                            
                        //}
                        //else
                        //{
                        //    SetFeeDetails(payment);
                        //}
                    }
                    
                } 
                else if (appliedCourse != null)
                {
                    Int32 levelId = 1;
                    if (appliedCourse.Programme.Id == 3)
                    {
                        levelId = 3;
                    }

                    payment.FeeDetails = SetFeeDetails(payment, appliedCourse.Programme.Id, levelId, paymentMode.Id, appliedCourse.Department.Id, payment.Session.Id);
                    //if (appliedCourse.Programme.Id == 2 || appliedCourse.Programme.Id == 3)
                    //{
                    //    SetFeeDetails(payment, appliedCourse.Programme.Id, appliedCourse.Department.Id, session.Id);
                    //}
                    //else
                    //{
                    //    SetFeeDetails(payment);
                    //} 
                }
                else
                {
                    SetFeeDetails(payment);
                } 

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool PaymentAlreadyMade(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Fee_Type_Id == payment.FeeType.Id && p.Payment_Mode_Id == payment.PaymentMode.Id && p.Payment_Type_Id == payment.PaymentType.Id && p.Person_Id == payment.Person.Id && p.Person_Type_Id == payment.PersonType.Id && p.Session_Id == payment.Session.Id;
                List<Payment> payments = GetModelsBy(selector);
                if(payments != null && payments.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool SetInvoiceNumber(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT entity = base.GetEntityBy(selector);

                if(entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Payment_Serial_Number = payment.SerialNumber;
                entity.Invoice_Number = payment.InvoiceNumber;

                int modifiedRecordCount = Save();
                if(modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public override Payment Create(Payment payment)
        {
            try
            {
                Payment newPayment = base.Create(payment);
                if(newPayment == null || newPayment.Id <= 0)
                {
                    throw new Exception("Payment ID not set!");
                }

                newPayment = SetNextPaymentNumber(newPayment);
                SetInvoiceNumber(newPayment);
                newPayment.FeeType = payment.FeeType;
                //SetFeeDetails(newPayment);

                return newPayment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Payment SetNextPaymentNumber(Payment payment)
        {
            try
            {
                payment.SerialNumber = payment.Id;
                payment.InvoiceNumber = "CHSTECH" + DateTime.Now.ToString("yy") + PaddNumber(payment.Id,10);

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static string PaddNumber(long id,int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if(idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    StringBuilder builder = new StringBuilder();
                    for(int counter = 0;counter < zeroCount;counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool InvalidConfirmationOrderNumber(string invoiceNo,string confirmationOrderNo)
        {
            try
            {
                List<PaymentEtranzact> payments = (from p in repository.GetBy<VW_PAYMENT>(p => p.Invoice_Number == invoiceNo)
                                                   select new PaymentEtranzact
                                                   {
                                                       ConfirmationNo = p.Confirmation_No,
                                                   }).ToList();

                if(payments != null)
                {
                    if(payments.Count > 1)
                    {
                        throw new Exception("Duplicate Invoice Number '" + invoiceNo + "' detected! Please contact your system administrator.");
                    }
                    else if(payments.Count == 1)
                    {
                        if(payments[0].ConfirmationNo == confirmationOrderNo)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber, int feeType)
        {
            try
            {
                Payment payment = new Payment();
                PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
                if(etranzactDetails == null || etranzactDetails.ReceiptNo == null)
                {
                    PaymentTerminal paymentTerminal = new PaymentTerminal();
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == feeType && p.Session_Id == 7);

                    etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber, paymentTerminal);
                    if(etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                    {
                        PaymentLogic paymentLogic = new PaymentLogic();
                        payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                        if(payment != null && payment.Id > 0)
                        {
                            decimal amountToPay = 0M;
                            if (payment.FeeType.Id == (int) FeeTypes.HostelFee)
                            {
                                HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                                HostelFee hostelFee = hostelFeeLogic.GetModelBy(h => h.Payment_Id == payment.Id);
                                amountToPay = Convert.ToDecimal(hostelFee.Amount);
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
                                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id, level.Id, 1, appliedCourse.Department.Id, appliedCourse.ApplicationForm.Setting.Session.Id);

                                    amountToPay = payment.FeeDetails.Sum(p => p.Fee.Amount);
                                }
                            }
                            else if (payment.FeeType.Id == (int)FeeTypes.ShortFall)
                            {
                                ShortFallLogic shortFallLogic = new ShortFallLogic();
                                ShortFall shortFall = shortFallLogic.GetModelBy(h => h.Payment_Id == payment.Id);
                                if (shortFall != null)
                                {
                                    amountToPay = Convert.ToDecimal(shortFall.Amount);
                                }
                            }
                            else
                            {
                                FeeDetail feeDetail = new FeeDetail();
                                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                                feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                                amountToPay = feeDetail.Fee.Amount;
                            }

                            if (!etranzactLogic.ValidatePin(etranzactDetails, payment, amountToPay))
                            {
                                throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com."); 
                            }
                        }
                        else
                        {
                            throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                        }
                    }
                    else
                    {
                        throw new Exception("Confirmation Order Number entered seems not to be valid! Please cross check and try again.");
                    }
                }
                else
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if(payment != null && payment.Id > 0)
                    {
                        decimal amountToPay = 0M;
                        if (payment.FeeType.Id == (int)FeeTypes.HostelFee)
                        {
                            HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                            HostelFee hostelFee = hostelFeeLogic.GetModelBy(h => h.Payment_Id == payment.Id);
                            amountToPay = Convert.ToDecimal(hostelFee.Amount);
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
                                payment.FeeDetails = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id, level.Id, 1, appliedCourse.Department.Id, appliedCourse.ApplicationForm.Setting.Session.Id);

                                amountToPay = payment.FeeDetails.Sum(p => p.Fee.Amount);
                            }
                        }
                        else if (payment.FeeType.Id == (int)FeeTypes.ShortFall)
                        {
                            ShortFallLogic shortFallLogic = new ShortFallLogic();
                            ShortFall shortFall = shortFallLogic.GetModelBy(h => h.Payment_Id == payment.Id);
                            if (shortFall != null)
                            {
                                amountToPay = Convert.ToDecimal(shortFall.Amount);
                            }
                        }
                        else
                        {
                            List<FeeDetail> feeDetails = new List<FeeDetail>();
                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                            feeDetails = feeDetailLogic.GetModelsBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                            amountToPay = feeDetails.Sum(a => a.Fee.Amount);;
                        }
                        if (!etranzactLogic.ValidatePin(etranzactDetails, payment, amountToPay))
                        {
                            throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                        }
                    }
                    else
                    {
                        throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                    }
                }

                return payment;
            }
            catch(Exception)
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
        public Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber,string ivn,int feeType)
        {
            try
            {
                Payment payment = new Payment();
                PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
                if(etranzactDetails == null || etranzactDetails.ReceiptNo == null)
                {
                    PaymentTerminal paymentTerminal = new PaymentTerminal();
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == feeType && p.Session_Id == 1);

                    etranzactDetails = etranzactLogic.RetrievePinsWithoutInvoice(confirmationOrderNumber,ivn,feeType,paymentTerminal);
                    if(etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                    {
                        PaymentLogic paymentLogic = new PaymentLogic();
                        payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                        if(payment != null && payment.Id > 0)
                        {
                            FeeDetail feeDetail = new FeeDetail();
                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                            feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                            if(!etranzactLogic.ValidatePin(etranzactDetails,payment,feeDetail.Fee.Amount))
                            {
                                throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");

                            }
                        }
                        else
                        {
                            throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                        }
                    }
                    else
                    {
                        throw new Exception("Confirmation Order Number entered seems not to be valid! Please cross check and try again.");
                    }
                }
                else
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if(payment != null && payment.Id > 0)
                    {
                        //FeeDetail feeDetail = new FeeDetail();
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        //feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                        List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                        decimal amount = feeDetails.Sum(a => a.Fee.Amount);
                        if(!etranzactLogic.ValidatePin(etranzactDetails,payment,amount))
                        {
                            throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                            //payment = null;
                            //return payment;
                        }
                    }
                    else
                    {
                        throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                    }
                }

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Gets all invoices generated by the student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public PaymentHistory GetStudentInvoices(Student student)
        {
            PaymentHistory paymentHistory = new PaymentHistory();
            try
            {
                if (student != null)
                {
                    StudentLogic studentLogic = new StudentLogic();

                    student = studentLogic.GetModelBy(s => s.Person_Id == student.Id);

                    paymentHistory.Student = student;
                    paymentHistory.Payments = new List<PaymentView>();

                    GetModelsBy(p => p.Person_Id == student.Id).ForEach(payment =>
                    {

                        if (ChechPaymentGateway(payment))
                            paymentHistory.Payments.Add(new PaymentView
                            {
                                PersonId = payment.Person.Id,
                                PaymentId = payment.Id,
                                InvoiceNumber = payment.InvoiceNumber,
                                PaymentModeName = payment.PaymentMode.Name,
                                FeeTypeName = payment.FeeType.Name,
                                SessionName = payment.Session.Name,
                                InvoiceGenerationDate = payment.DatePaid,
                                InvoiceGenerationDateStr = payment.DatePaid.ToLongDateString()
                            });
                    });

                    paymentHistory.Payments = paymentHistory.Payments.Count > 0 ? 
                                                paymentHistory.Payments.OrderBy(p => p.SessionName).ThenBy(p => p.FeeTypeName).ThenBy(p => p.PaymentModeName).ToList() : 
                                                paymentHistory.Payments;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return paymentHistory;
        }

        private bool ChechPaymentGateway(Payment payment)
        {
            try
            {
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(rp => rp.Payment_Id == payment.Id);

                return remitaPayment != null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber,Session session,FeeType feetype)
        {
            try
            {
                Payment payment = new Payment();
                PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
                if(etranzactDetails == null || etranzactDetails.ReceiptNo == null)
                {

                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    RemitaPayementProcessor r = new RemitaPayementProcessor("918567");

                    remitaPayment = remitaPaymentLogic.GetModelsBy(a => a.RRR == confirmationOrderNumber).FirstOrDefault();

                    if (remitaPayment != null)
                    {
                        if (remitaPayment != null && remitaPayment.Description.ToLower().Contains("manual"))
                        {
                            return remitaPayment.payment;
                        }

                        remitaPayment = r.GetStatus(remitaPayment.OrderId);
                        //remitaPayment.TransactionAmount == 2500 &
                        //if (remitaPayment.Status.Contains("01:") || remitaPayment.Status.Contains("00"))
                        if (remitaPayment.Status.Contains("021:") || remitaPayment.Status.Contains("00"))
                            {
                            payment = remitaPayment.payment;
                            return payment;
                        }
                        else
                        {
                            throw new Exception("Payment could not be verified, Try again in a few minuteds");
                        }
                    }

                    PaymentTerminal paymentTerminal = new PaymentTerminal();
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    paymentTerminal = paymentTerminalLogic.GetModelsBy(p => p.Fee_Type_Id == feetype.Id && p.Session_Id == session.Id).FirstOrDefault();

                    etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber,paymentTerminal);
                    if(etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                    {
                        PaymentLogic paymentLogic = new PaymentLogic();
                        payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                        if(payment != null && payment.Id > 0)
                        {
                            List<FeeDetail> feeDetail = new List<FeeDetail>();
                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();


                            StudentLevel studentLevel = new StudentLevel();
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            studentLevel = studentLevelLogic.GetBy(payment.Person.Id);

                            AdmissionList admissionList = new AdmissionList();
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            admissionList = admissionLogic.GetBy(payment.Person);

                            if(studentLevel != null)
                            {

                                if(studentLevel != null)
                                {
                                    decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department,studentLevel.Level,studentLevel.Programme,payment.FeeType,payment.Session,payment.PaymentMode);
                                    if(!etranzactLogic.ValidatePin(etranzactDetails,payment,AmountToPay))
                                    {
                                        if (CheckShortFall(etranzactDetails, payment, AmountToPay))
                                        {
                                            //paid shortfall
                                        }
                                        else
                                        {
                                            throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot retrieve amount for your department. Please contact support@lloydant.com.");

                                }

                            }
                            else
                            {
                                if(admissionList != null)
                                {
                                    Level level;
                                    if(admissionList.Form.ProgrammeFee.Programme.Id == 1 || admissionList.Form.ProgrammeFee.Programme.Id == 2 || admissionList.Form.ProgrammeFee.Programme.Id == 5)
                                    {
                                        level = new Level() { Id = 1 };
                                    }
                                    else
                                    {
                                        level = new Level() { Id = 3 };
                                    }

                                    decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment,level,admissionList.Form.ProgrammeFee.Programme,payment.FeeType,payment.Session,payment.PaymentMode);
                                    if(!etranzactLogic.ValidatePin(etranzactDetails,payment,AmountToPay))
                                    {
                                        if (CheckShortFall(etranzactDetails, payment, AmountToPay))
                                        {
                                            //paid shortfall
                                        }
                                        else
                                        {
                                            throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                        }
                    }
                    else
                    {
                        throw new Exception("Confirmation Order Number entered seems not to be valid! Please cross check and try again.");
                    }
                }
                else
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if(payment != null && payment.Id > 0)
                    {
                        List<FeeDetail> feeDetail = new List<FeeDetail>();
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();

                        if(payment.FeeType.Id == (int)FeeTypes.SchoolFees || payment.FeeType.Id == (int)FeeTypes.CarryOverSchoolFees)
                        {

                            AdmissionList admissionList = new AdmissionList();
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            admissionList = admissionLogic.GetBy(payment.Person);
                            
                            if(admissionList != null)
                            {
                                Level level;
                                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                                StudentLevel currentStudentLevel = studentLevelLogic.GetModelsBy(sl => sl.Person_Id == payment.Person.Id && sl.Session_Id == payment.Session.Id).LastOrDefault();
                                if (currentStudentLevel != null)
                                {
                                    level = currentStudentLevel.Level;
                                }
                                else if (admissionList.Session.Id != payment.Session.Id)
                                {
                                    if (admissionList.Form.ProgrammeFee.Programme.Id == 1 || admissionList.Form.ProgrammeFee.Programme.Id == 2 || admissionList.Form.ProgrammeFee.Programme.Id == 5)
                                    {
                                        level = new Level() { Id = 2 };
                                    }
                                    else
                                    {
                                        level = new Level() { Id = 4 };
                                    } 
                                }
                                else
                                {
                                    if (admissionList.Form.ProgrammeFee.Programme.Id == 1 || admissionList.Form.ProgrammeFee.Programme.Id == 2 || admissionList.Form.ProgrammeFee.Programme.Id == 5)
                                    {
                                        level = new Level() { Id = 1 };
                                    }
                                    else
                                    {
                                        level = new Level() { Id = 3 };
                                    }  
                                }

                                decimal AmountToPay = 0M;

                                if (payment.FeeType.Id == (int) FeeTypes.CarryOverSchoolFees)
                                {
                                    StudentExtraYearSession extraYear = new StudentExtraYearSession();
                                    StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
                                    extraYear = extraYearLogic.GetBy(payment.Person.Id, payment.Session.Id);

                                    if (extraYear != null)
                                    {
                                        int lastSession =
                                            Convert.ToInt32(extraYear.LastSessionRegistered.Name.Substring(0, 4));
                                        int currentSession = Convert.ToInt32(payment.Session.Name.Substring(0, 4));
                                        int NoOfOutstandingSession = currentSession - lastSession;
                                        if (NoOfOutstandingSession == 0)
                                        {
                                            NoOfOutstandingSession = 1;
                                        }

                                        AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment, level, admissionList.Form.ProgrammeFee.Programme, payment.FeeType, payment.Session, payment.PaymentMode) * NoOfOutstandingSession;
                                    } 
                                }
                                else
                                {
                                    AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment, level, admissionList.Form.ProgrammeFee.Programme, payment.FeeType, payment.Session, payment.PaymentMode); 
                                }
                                
                                if(!etranzactLogic.ValidatePin(etranzactDetails,payment,AmountToPay))
                                {
                                    if (CheckShortFall(etranzactDetails, payment, AmountToPay))
                                    {
                                        //paid shortfall
                                    }
                                    else
                                    {
                                        throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                    }
                                }
                            }
                            else
                            {
                                StudentLevel studentLevel = new StudentLevel();
                                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                                //studentLevel = studentLevelLogic.GetBy(payment.Person.Id);
                                //studentLevel = studentLevelLogic.GetExtraYearBy(payment.Person.Id);
                                studentLevel = studentLevelLogic.GetModelsBy(s => s.Session_Id == payment.Session.Id && s.Person_Id == payment.Person.Id).LastOrDefault();
                                if(studentLevel != null)
                                {
                                    //decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                                    //if (!etranzactLogic.ValidatePin(etranzactDetails, payment, AmountToPay))
                                    //{
                                    //    throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");

                                    //}
                                }
                                else
                                {
                                    throw new Exception("Cannot retrieve amount for your department. Please contact support@lloydant.com.");

                                }

                            }
                        }
                        else if(payment.FeeType.Id == (int)FeeTypes.AcceptanceFee || payment.FeeType.Id == (int)FeeTypes.HNDAcceptance)
                        {
                             AdmissionList admissionList = new AdmissionList();
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            admissionList = admissionLogic.GetBy(payment.Person);



                            if(admissionList != null)
                            {
                                Level level;
                                if(admissionList.Form.ProgrammeFee.Programme.Id == 1 || admissionList.Form.ProgrammeFee.Programme.Id == 2 || admissionList.Form.ProgrammeFee.Programme.Id == 5)
                                {
                                    level = new Level() { Id = 1 };
                                }
                                else
                                {
                                    level = new Level() { Id = 3 };
                                }

                                decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment,level,admissionList.Form.ProgrammeFee.Programme,payment.FeeType,payment.Session,payment.PaymentMode);
                                if(!etranzactLogic.ValidatePin(etranzactDetails,payment,AmountToPay))
                                {
                                    if (CheckShortFall(etranzactDetails, payment, AmountToPay))
                                    {
                                        //paid shortfall
                                    }
                                    else
                                    {
                                        throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            payment.FeeDetails = feeDetailLogic.GetModelsBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                            if(!etranzactLogic.ValidatePin(etranzactDetails,payment,payment.FeeDetails.Sum(p => p.Fee.Amount)))
                            {
                                if (CheckShortFall(etranzactDetails, payment, payment.FeeDetails.Sum(p => p.Fee.Amount)))
                                {
                                    //paid shortfall
                                }
                                else
                                {
                                    throw new Exception("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                }
                            }
                        }

                    }
                    else
                    {
                        throw new Exception("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                    }
                }

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool CheckShortFall(PaymentEtranzact etranzactDetails, Payment payment, decimal amountToPay)
        {
            bool paid = false;
            try
            {
                ShortFallLogic shortFallLogic = new ShortFallLogic();
                ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.Payment_Id == payment.Id).LastOrDefault();

                if (shortFall != null)
                {
                    paid = Convert.ToDecimal(shortFall.Amount) + etranzactDetails.TransactionAmount == amountToPay;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return paid;
        }

        public bool Modify(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT entity = GetEntityBy(selector);

                if(entity == null || entity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if(payment.Person != null)
                {
                    entity.Person_Id = payment.Person.Id;
                }

                entity.Payment_Serial_Number = payment.SerialNumber;
                entity.Invoice_Number = payment.InvoiceNumber;
                entity.Date_Paid = payment.DatePaid;

                if(payment.PaymentMode != null)
                {
                    entity.Payment_Mode_Id = payment.PaymentMode.Id;
                }
                if(payment.PaymentType != null)
                {
                    entity.Payment_Type_Id = payment.PaymentType.Id;
                }
                if(payment.PersonType != null)
                {
                    entity.Person_Type_Id = payment.PersonType.Id;
                }
                if(payment.FeeType != null)
                {
                    entity.Fee_Type_Id = payment.FeeType.Id;
                }
                if(payment.Session != null)
                {
                    entity.Session_Id = payment.Session.Id;
                }

                entity.Fee_Type_Id = payment.FeeType.Id;
                int modifiedRecordCount = Save();
                if(modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public decimal GetPaymentAmount(Payment payment)
        {
            decimal Amount = 0;
            try
            {
                FeeDetail feeDetail = new FeeDetail();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                feeDetail = feeDetailLogic.GetModelBy(f => f.Fee_Type_Id == payment.FeeType.Id);
                Amount = feeDetail.Fee.Amount;
            }
            catch(Exception ex)
            {

                throw;
            }
            return Amount;
        }

        public void DeleteBy(long PaymentID)
        {
            try
            {
                Expression<Func<PAYMENT,bool>> selector = a => a.Payment_Id == PaymentID;
                Delete(selector);
            }
            catch(Exception)
            {
                throw;
            }
        }
        //public List<PaymentReportModel> GetPaymentList()
        //{
        //    try
        //    {
        //        List<PaymentReportModel> PaymentReportList = new List<PaymentReportModel>();
        //        List<Payment> paymentList = new List<Payment>();

        //        PaymentEtranzact paymentEtransact = new PaymentEtranzact();
        //        StudentLevel studentLevel = new StudentLevel();

        //        PaymentEtranzactLogic paymentEtransactLogic = new PaymentEtranzactLogic();
        //        PaymentLogic paymentLogic = new PaymentLogic();
        //        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

        //        paymentList = paymentLogic.GetModelsBy(p => p.Fee_Type_Id == 3 && p.Session_Id == 1);

        //        foreach (Payment payment in paymentList)
        //        {		            
        //            long personId = paymentEtransactLogic.GetModelBy(p => p.Payment_Id == payment.Id).Payment.Payment.Person.Id;
        //            if (personId > 0)
        //            {
        //                studentLevel = studentLevelLogic.GetModelBy(p => p.Person_Id == personId);
        //                PaymentReportModel model = new PaymentReportModel();
        //                model.Department = studentLevel.Department.Name;
        //                model.Programme = studentLevel.Programme.Name;
        //                model.Level = studentLevel.Level.Name;
        //                model.Session = payment.Session.Name;
        //                if (studentLevel.Level.Id == 1 && studentLevel.Programme.Id == 1)
        //                {
        //                    model.ND1 += 1;
        //                }
        //                if (studentLevel.Level.Id == 2 && studentLevel.Programme.Id == 1)
        //                {
        //                    model.ND2 += 1;
        //                }
        //                if (studentLevel.Level.Id == 1 && studentLevel.Programme.Id == 2)
        //                {
        //                    model.PT1 += 1;
        //                }
        //                if (studentLevel.Level.Id == 2 && studentLevel.Programme.Id == 2)
        //                {
        //                    model.PT2 += 2;
        //                }
        //                if (studentLevel.Level.Id == 3 && studentLevel.Programme.Id == 3)
        //                {
        //                    model.HND1 += 1;
        //                }
        //                if (studentLevel.Level.Id == 4 && studentLevel.Programme.Id == 3)
        //                {
        //                    model.HND2 += 1;
        //                }

        //                PaymentReportList.Add(model);
        //            }

        //        }

        //        return PaymentReportList.OrderBy(p => p.Department).ToList();
        //    }
        //    catch (Exception)
        //    {                
        //        throw;
        //    }
        //}
        public List<RegistrationBalanceReport> GetRegistrationBalanceList(Session session,Semester semester)
        {
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            List<RegistrationBalanceReport> RegistrationBalanceList = new List<RegistrationBalanceReport>();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            STUDENT_LEVEL studentLevel = new STUDENT_LEVEL();

            CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
            CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            STUDENT_COURSE_REGISTRATION courseRegistration = new STUDENT_COURSE_REGISTRATION();
            STUDENT_COURSE_REGISTRATION_DETAIL courseRegistrationDetail = new STUDENT_COURSE_REGISTRATION_DETAIL();

            try
            {
                if(session != null)
                {
                    List<PAYMENT> payments = GetEntitiesBy(p => p.Fee_Type_Id == 3 && p.Session_Id == session.Id).Take(3000).ToList();
                    foreach(PAYMENT payment in payments)
                    {
                        int studentNumberPay = 0;
                        int studentNumberReg = 0;
                        PAYMENT_ETRANZACT paymentEtranzact = new PAYMENT_ETRANZACT();
                        paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                        if(paymentEtranzact != null)
                        {
                            RegistrationBalanceReport registrationBalanceReport = new RegistrationBalanceReport();
                            studentLevel = studentLevelLogic.GetEntityBy(p => p.Person_Id == payment.Person_Id && p.Session_Id == session.Id);

                            if(session != null && semester != null)
                            {
                                courseRegistration = courseRegistrationLogic.GetEntityBy(p => p.Session_Id == session.Id && p.Person_Id == payment.Person_Id && p.Level_Id == studentLevel.Level_Id && p.Department_Id == studentLevel.Department_Id && p.Programme_Id == studentLevel.Programme_Id);
                                if(courseRegistration != null)
                                {
                                    courseRegistrationDetail = courseRegistrationDetailLogic.GetEntitiesBy(p => p.Semester_Id == semester.Id && p.Student_Course_Registration_Id == courseRegistration.Student_Course_Registration_Id).FirstOrDefault();
                                }
                            }

                            registrationBalanceReport.Department = studentLevel.DEPARTMENT.Department_Name;

                            if(studentLevel.Level_Id == 1 && studentLevel.Programme_Id == 2)
                            {
                                registrationBalanceReport.ProgrammePayment = "PART TIME 1 (PAY)";

                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if(courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = " PART TIME 1 (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }
                            else if(studentLevel.Level_Id == 2 && studentLevel.Programme_Id == 2)
                            {
                                registrationBalanceReport.ProgrammePayment = "PART TIME 2 (PAY)";
                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if(courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = "PART TIME 2 (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }
                            else
                            {
                                registrationBalanceReport.ProgrammePayment = studentLevel.LEVEL.Level_Name + " (PAY)";
                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if(courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = studentLevel.LEVEL.Level_Name + " (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }

                        }

                    }
                }

            }
            catch(Exception)
            {
                throw;
            }
            return RegistrationBalanceList;
        }
        public List<PaymentReport> GetPaymentsBy(Session session)
        {
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            List<PaymentReport> PaymentReportList = new List<PaymentReport>();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            STUDENT_LEVEL studentLevel = new STUDENT_LEVEL();

            try
            {
                if(session != null)
                {
                    List<REMITA_PAYMENT> remita = (from a in repository.GetBy<VW_SCHOOL_FEES_PAYMENT>(s => s.Fee_Type_Id == (int) FeeTypes.SchoolFees && s.Session_Id == session.Id)
                                    select new REMITA_PAYMENT()
                                    {
                                        ONLINE_PAYMENT = new ONLINE_PAYMENT() { PAYMENT = new PAYMENT() { Person_Id = a.Person_Id} }
                                    }).ToList();

                    for (int i = 0; i < remita.Count; i++)
                    {
                        REMITA_PAYMENT remitas = remita[i];

                        int studentNumber = 0;
                        PaymentReport paymentReport = new PaymentReport();
                        studentLevel = studentLevelLogic.GetEntitiesBy(p => p.Person_Id == remitas.ONLINE_PAYMENT.PAYMENT.Person_Id && p.Session_Id == session.Id).LastOrDefault();

                        if (studentLevel != null)
                        {
                            paymentReport.Department = studentLevel.DEPARTMENT.Department_Name;
                            if (studentLevel.Level_Id == (int)LevelList.ND1 && studentLevel.Programme_Id == (int)Programmes.NDPartTime)
                            {
                                paymentReport.Programme = "PART TIME 1";

                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else if (studentLevel.Level_Id == (int)LevelList.ND2 && studentLevel.Programme_Id == (int)Programmes.NDPartTime)
                            {
                                paymentReport.Programme = "PART TIME 2";
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else
                            {
                                paymentReport.Programme = studentLevel.LEVEL.Level_Name;
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                        }
                    }

                   
                }

            }
            catch(Exception ex)
            {

                throw ex;
            }
            return PaymentReportList;
        }



        public List<DuplicateMatricNumberFix> GetPartTimeGuys()
        {
            List<DuplicateMatricNumberFix> dupList = new List<DuplicateMatricNumberFix>();
            List<PAYMENT> paymentList = GetEntitiesBy(p => p.Fee_Type_Id == 6 && p.Session_Id == 1);
            PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
            foreach(PAYMENT paymentItem in paymentList)
            {
                PAYMENT_ETRANZACT paymentEtranzact = etranzactLogic.GetEntityBy(p => p.Payment_Id == paymentItem.Payment_Id);
                if(paymentEtranzact != null)
                {
                    DuplicateMatricNumberFix dup = new DuplicateMatricNumberFix();
                    dup.Fullname = paymentItem.PERSON.Last_Name + " " + paymentItem.PERSON.First_Name + " " + paymentItem.PERSON.Other_Name;
                    dup.ConfirmationOrder = paymentEtranzact.Confirmation_No;
                    dup.ReceiptNumber = paymentEtranzact.Receipt_No;
                    dupList.Add(dup);
                }
            }
            return dupList.OrderBy(p => p.Fullname).ToList();
        }
        public List<FeesPaymentReport> GetFeesPaymentBy(Session session,Programme programme,Department department,Level level)
        {
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            List<FeesPaymentReport> feesPaymentReportList = new List<FeesPaymentReport>();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            List<STUDENT_LEVEL> studentLevelList = new List<STUDENT_LEVEL>();
            List<ADMISSION_LIST> admissionList = new List<ADMISSION_LIST>();
            List<PAYMENT> payments = new List<PAYMENT>();
            PAYMENT_ETRANZACT paymentEtranzact = new PAYMENT_ETRANZACT();
            REMITA_PAYMENT remitaPayment = new REMITA_PAYMENT();
            PAYMENT_ETRANZACT thisPaymentEtranzact = new PAYMENT_ETRANZACT();
            REMITA_PAYMENT thisRemitaPayment = new REMITA_PAYMENT();

            try
            {
                if(session != null && programme != null && department != null && level != null)
                {
                    studentLevelList = studentLevelLogic.GetEntitiesBy(p => p.Department_Id == department.Id && p.Level_Id == level.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id);

                    foreach(STUDENT_LEVEL studentLevel in studentLevelList)
                    {
                        List<PAYMENT> confirmedPayments = new List<PAYMENT>();
                        payments = GetEntitiesBy(p => p.Person_Id == studentLevel.Person_Id && (p.Fee_Type_Id == 3 || p.Fee_Type_Id == 2));
                        foreach(PAYMENT payment in payments)
                        {
                            paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                            if(paymentEtranzact != null)
                            {
                                confirmedPayments.Add(payment);
                            }
                            else
                            {
                                remitaPayment = remitaPaymentLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                                if(remitaPayment != null)
                                {
                                    confirmedPayments.Add(payment);
                                }

                            }
                        }

                        FeesPaymentReport feesPaymentReport = new FeesPaymentReport();
                        foreach(PAYMENT confirmedPayment in confirmedPayments)
                        {
                            thisPaymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == confirmedPayment.Payment_Id);
                            thisRemitaPayment = remitaPaymentLogic.GetEntityBy(p => p.Payment_Id == confirmedPayment.Payment_Id);
                            if(confirmedPayment.Fee_Type_Id == 2)
                            {

                                if(thisPaymentEtranzact != null)
                                {
                                    feesPaymentReport.AcceptanceTransactionAmount = "N" + paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                    feesPaymentReport.AcceptanceFeeInvoiceNumber = thisPaymentEtranzact.Confirmation_No;
                                }
                                else if(thisRemitaPayment != null)
                                {
                                    feesPaymentReport.AcceptanceTransactionAmount = "N" + remitaPaymentLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                    feesPaymentReport.AcceptanceFeeInvoiceNumber = thisRemitaPayment.RRR;
                                }
                            }
                            else if(confirmedPayment.Fee_Type_Id == 3 && confirmedPayment.Session_Id == session.Id)
                            {
                                if(studentLevel.Level_Id == 1 || studentLevel.Level_Id == 3)
                                {
                                    if(thisPaymentEtranzact != null)
                                    {
                                        feesPaymentReport.FirstYearFeesTransactionAmount = "N" + paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                        feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = thisPaymentEtranzact.Confirmation_No;
                                    }
                                    else if(thisRemitaPayment != null)
                                    {
                                        feesPaymentReport.FirstYearFeesTransactionAmount = "N" + remitaPaymentLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                        feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = thisRemitaPayment.RRR;
                                    }
                                }
                                if(studentLevel.Level_Id == 2 || studentLevel.Level_Id == 4)
                                {
                                    if(thisPaymentEtranzact != null)
                                    {
                                        feesPaymentReport.SecondYearFeesTransactionAmount = "N" + paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                        feesPaymentReport.SecondYearSchoolFeesInvoiceNumber = thisPaymentEtranzact.Confirmation_No;
                                    }
                                    else if(thisRemitaPayment != null)
                                    {
                                        feesPaymentReport.SecondYearFeesTransactionAmount = "N" + remitaPaymentLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                        feesPaymentReport.SecondYearSchoolFeesInvoiceNumber = thisRemitaPayment.RRR;
                                    }
                                }
                            }
                            else if(confirmedPayment.Fee_Type_Id == 3 && confirmedPayment.Session_Id != session.Id)
                            {
                                if(thisPaymentEtranzact != null)
                                {
                                    feesPaymentReport.FirstYearFeesTransactionAmount = "N" + paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                    feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = thisPaymentEtranzact.Confirmation_No;
                                }
                                else if(thisRemitaPayment != null)
                                {
                                    feesPaymentReport.FirstYearFeesTransactionAmount = "N" + remitaPaymentLogic.GetModelBy(p => p.Payment_Id == confirmedPayment.Payment_Id).TransactionAmount.ToString();
                                    feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = thisRemitaPayment.RRR;
                                }
                            }
                        }


                        feesPaymentReport.Department = department.Name;
                        feesPaymentReport.Level = level.Name;
                        feesPaymentReport.MatricNumber = studentLevel.STUDENT.Matric_Number;
                        if(studentLevel.STUDENT.APPLICATION_FORM != null)
                        {
                            feesPaymentReport.ApplicationNumber = studentLevel.STUDENT.APPLICATION_FORM.Application_Form_Number;
                        }
                        feesPaymentReport.Programme = programme.Name;
                        feesPaymentReport.Session = session.Name;
                        feesPaymentReport.Name = studentLevel.STUDENT.PERSON.Last_Name + " " + studentLevel.STUDENT.PERSON.First_Name;

                        feesPaymentReportList.Add(feesPaymentReport);
                    }

                }
            }

            catch(Exception)
            {
                throw;
            }

            return feesPaymentReportList.OrderBy(p => p.Name).ToList();
        }
        public List<FeesPaymentReport> GetAllFeesPaymentBy(Session session,Programme programme,Department department,Level level)
        {
            List<FeesPaymentReport> feesPaymentReportList = new List<FeesPaymentReport>();

            try
            {
                if(session != null && programme != null && department != null && level != null)
                {

                    if(level.Id == 1 || level.Id == 3)
                    {
                        var paymentReport = (from a in repository.GetBy<VW_ALL_PAYMENT_REPORT_NEW_STUDENTS>(a => a.Department_Id == department.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id && a.Session_Id == session.Id)
                                             select new FeesPaymentReport
                                             {
                                                 Name = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                                                 ApplicationNumber = a.Application_Form_Number,
                                                 MatricNumber = a.Matric_Number,
                                                 AcceptanceFeeInvoiceNumber = a.ACCEPTANCE_FEE_INVOICE,
                                                 AcceptanceTransactionAmount = a.ACCEPTANCE_FEE_AMOUNT.ToString(),
                                                 FirstYearSchoolFeesInvoiceNumber = a.FIRST_YEAR_FEE_INVOICE,
                                                 FirstYearFeesTransactionAmount = a.FIRST_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
                                                 SecondYearSchoolFeesInvoiceNumber = a.SECOND_YEAR_FEE_INVOICE,
                                                 SecondYearFeesTransactionAmount = a.SECOND_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
                                                 Session = a.Session_Name,
                                                 Programme = a.Programme_Name,
                                                 Department = a.Department_Name,
                                                 Level = a.Level_Name,
                                             }).ToList();
                        feesPaymentReportList = paymentReport;
                    }
                    else
                    {
                        feesPaymentReportList = GetFeesPaymentBy(session,programme,department,level);
                    }



                }
            }

            catch(Exception)
            {
                throw;
            }
            return feesPaymentReportList.OrderBy(p => p.Name).ToList();

        }


        public List<AcceptanceView> GetAcceptanceCount(Session session, string dateFrom, string dateTo)
        {
            try
            {
                List<AcceptanceView> payments = new List<AcceptanceView>();
                string[] ndProgrammeNames = { "ND Full Time", "ND Part Time" };
                string[] hndProgrammeNames = { "HND Full Time", "HND Part Time" };
                int ndCount = 0;
                int hndCount = 0;
                int otherCount = 0;

                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime applicationFrom = ConvertToDate(dateFrom);
                    DateTime applicationTo = ConvertToDate(dateTo);

                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Session_Id == session.Id && (p.Transaction_Date.Value >= applicationFrom && p.Transaction_Date.Value <= applicationTo))
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name,
                                    InvoiceDate = p.Transaction_Date == null ? p.Date_Paid.ToLongDateString() : p.Transaction_Date.Value.ToLongDateString(),
                                    Count = 1
                                }).OrderBy(b => b.Name).ToList();

                    ndCount = payments.Count(p => ndProgrammeNames.Contains(p.Programme_Name));
                    hndCount = payments.Count(p => hndProgrammeNames.Contains(p.Programme_Name));
                }
                else
                {
                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Session_Id == session.Id)
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name,
                                    InvoiceDate = p.Transaction_Date == null ? p.Date_Paid.ToLongDateString() : p.Transaction_Date.Value.ToLongDateString(),
                                    Count = 1
                                }).OrderBy(b => b.Name).ToList();

                    ndCount = payments.Count(p => ndProgrammeNames.Contains(p.Programme_Name));
                    hndCount = payments.Count(p => hndProgrammeNames.Contains(p.Programme_Name));
                }

                for (int i = 0; i < payments.Count; i++)
                {
                    payments[i].HNDCount = hndCount;
                    payments[i].NDCount = ndCount;
                    payments[i].OTHERCount = otherCount;
                    payments[i].TotalCount = payments.Count;
                }

                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                string[] dateSplit = date.Split('/');
                newDate = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }

        public List<AcceptanceView> GetAcceptanceReport(Session session, Department department, Programme programme, string dateFrom, string dateTo)
        {

            try
            {
                List<AcceptanceView> payments = new List<AcceptanceView>();

                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime applicationFrom = ConvertToDate(dateFrom);
                    DateTime applicationTo = ConvertToDate(dateTo);

                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && (p.Transaction_Date.Value >= applicationFrom && p.Transaction_Date.Value <= applicationTo))
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Confirmation_No,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Confirmation_No,
                                    Programme_Name = p.Programme_Name,
                                    InvoiceDate = p.Transaction_Date == null ? p.Date_Paid.ToLongDateString() : p.Transaction_Date.Value.ToLongDateString(),
                                }).OrderBy(b => b.Name).ToList();

                }
                else
                {
                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id)
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Confirmation_No,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Confirmation_No,
                                    Programme_Name = p.Programme_Name,
                                    InvoiceDate = p.Transaction_Date == null ? p.Date_Paid.ToLongDateString() : p.Transaction_Date.Value.ToLongDateString(),
                                }).OrderBy(b => b.Name).ToList();

                }

                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetFirstInstallment(Payment payment)
        {
            try
            {
                return
                    GetModelBy(
                        p =>
                            p.Person_Id == payment.Person.Id && p.Session_Id == payment.Session.Id &&
                            p.Payment_Mode_Id == (int)PaymentModes.FirstInstallment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Payment GetSecondInstallment(Payment payment)
        {
            try
            {
                return
                    GetModelBy(
                        p =>
                            p.Person_Id == payment.Person.Id && p.Session_Id == payment.Session.Id &&
                            p.Payment_Mode_Id == (int)PaymentModes.SecondInstallment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Payment Update(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT paymentEntity = GetEntityBy(selector);

                if (paymentEntity == null || paymentEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                paymentEntity.Payment_Serial_Number = payment.SerialNumber;
                paymentEntity.Invoice_Number = payment.InvoiceNumber;
                paymentEntity.Date_Paid = payment.DatePaid;

                if (payment.PaymentMode != null)
                {
                    paymentEntity.Payment_Mode_Id = payment.PaymentMode.Id;
                }
                if (payment.Person != null)
                {
                    paymentEntity.Person_Id = payment.Person.Id;
                }
                if (payment.PaymentType != null)
                {
                    paymentEntity.Payment_Type_Id = payment.PaymentType.Id;
                }
                if (payment.PersonType != null)
                {
                    paymentEntity.Person_Type_Id = payment.PersonType.Id;
                }
                if (payment.FeeType != null)
                {
                    paymentEntity.Fee_Type_Id = payment.FeeType.Id;
                }
                if (payment.Session != null)
                {
                    paymentEntity.Session_Id = payment.Session.Id;
                }


                int modifiedRecordCount = Save();

                payment = GetModelBy(p => p.Payment_Id == payment.Id);
                return payment;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<PaymentSummary> GetPaymentSummary(DateTime dateFrom, DateTime dateTo, List<int> programmeIds)
        {
           
            try
            {
                List<PaymentSummary> payments = (from p in repository.GetBy<VW_PAYMENT_SUMMARY>(p => p.Transaction_Date != null && p.Transaction_Date >= dateFrom && p.Transaction_Date <= dateTo && programmeIds.Contains((int)p.Programme_Id))
                                                 select new PaymentSummary
                                                 {
                                                     PersonId = p.Person_Id,
                                                     Name = p.Name,
                                                     MatricNumber = p.Matric_Number,
                                                     SessionId = p.Session_Id,
                                                     SessionName = p.Session_Name,
                                                     FeeTypeId = p.Fee_Type_Id,
                                                     FeeTypeName = p.Fee_Type_Name,
                                                     LevelId = p.Level_Id,
                                                     LevelName = p.Level_Name,
                                                     ProgrammeId = p.Programme_Id,
                                                     ProgrammeName = p.Programme_Name,
                                                     DepartmentId = p.Department_Id,
                                                     DepartmentName = p.Department_Name,
                                                     FacultyId = p.Faculty_Id,
                                                     FacultyName = p.Faculty_Name,
                                                     TransactionDate = p.Transaction_Date,
                                                     TransactionAmount = p.Transaction_Amount,
                                                     RRR = p.RRR,
                                                     Status = p.Status,
                                                     PaymentEtranzactId = p.Payment_Etranzact_Id,
                                                     InvoiceNumber = p.Invoice_Number,
                                                     ConfirmationNumber = p.Confirmation_Number
                                                 }).ToList();
                return payments;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetThirdInstallment(Payment payment)
        {
            try
            {
                return
                    GetModelBy(
                        p =>
                            p.Person_Id == payment.Person.Id && p.Session_Id == payment.Session.Id &&
                            p.Payment_Mode_Id == (int)PaymentModes.ThirdInstallment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsPaid(long paymentId)
        {
            try
            {
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                return remitaPaymentLogic.GetModelBy(r => r.Payment_Id == paymentId && r.Status.Contains("01") || r.Status.Contains("00")) != null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdatePaymentRecord(long paymentId, DateTime transactionDate)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == paymentId;
                PAYMENT paymentEntity = GetEntityBy(selector);

                if (paymentEntity == null || paymentEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                paymentEntity.Date_Paid = transactionDate;

                int modifiedRecordCount = Save();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

