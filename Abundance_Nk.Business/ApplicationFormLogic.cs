using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class ApplicationFormLogic : BusinessBaseLogic<ApplicationForm, APPLICATION_FORM>
    {
        private PaymentLogic paymentLogic;
        private CardPaymentLogic cardPaymentLogic;
        private ScratchCardLogic scratchCardLogic;
        private PaymentEtranzactLogic paymentEtranzactLogic;
        public ApplicationFormLogic()
        {
            translator = new ApplicationFormTranslator();
            scratchCardLogic = new ScratchCardLogic();
            cardPaymentLogic = new CardPaymentLogic();
            paymentLogic = new PaymentLogic();
            paymentEtranzactLogic = new PaymentEtranzactLogic();
        }
        public bool IsValidApplicationNumberAndPin(string applicationNumber, string pin)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = af => af.Application_Form_Number == applicationNumber;
                ApplicationForm applicationForm = GetModelBy(selector);
                if (applicationForm == null)
                {
                    throw new Exception("Invalid Application Number!");
                }

                ScratchCard card = scratchCardLogic.GetBy(pin);
                if (card == null)
                {
                    throw new Exception("Invalid Pin!");
                }
                //card has been used
                if (scratchCardLogic.IsPinUsed(pin, applicationForm.Person.Id))
                {
                    throw new Exception("Pin entered does not belong to applicant '" + applicationForm.Person.FullName + "' !");

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

        public bool IsValidApplicationNumberAndEtranzactPin(string applicationNumber, string pin)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = af => af.Application_Form_Number == applicationNumber;
                ApplicationForm applicationForm = GetModelBy(selector);
                if (applicationForm == null)
                {
                    throw new Exception("Invalid Application Number!");
                }

                PaymentEtranzact pinEtranzact = paymentEtranzactLogic.GetBy(applicationForm.Payment);
                if (pinEtranzact == null)
                {
                    throw new Exception("Invalid Pin!");
                }
                //card has been used
                if (pinEtranzact.ConfirmationNo != pin)
                {
                    throw new Exception("Pin entered does not belong to applicant '" + applicationForm.Person.FullName + "' !");

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

        public ApplicationForm Create(ApplicationForm form, AppliedCourse appliedCourse)
        {
            try
            {
                ApplicationForm newForm = base.Create(form);
                if (newForm == null || newForm.Id <= 0)
                {
                    throw new Exception("Application Form creation failed!");
                }

                newForm.Setting = form.Setting;
                newForm.ProgrammeFee = form.ProgrammeFee;
                newForm = SetNextApplicationFormNumber(newForm);
                newForm = SetNextExamNumber(newForm, appliedCourse);

                SetApplicationAndExamNumber(newForm);

                return newForm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public ApplicationForm Create(ApplicationForm form, AppliedCourse appliedCourse)
        //{
        //    try
        //    {
        //        form = SetNextApplicationFormNumber(form);
        //        form = SetNextExamNumber(form, appliedCourse);

        //        return base.Create(form);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public bool SetRejectReason(ApplicationForm form)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == form.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                entity.Reject_Reason = form.RejectReason;
                entity.Rejected = form.Rejected;
                entity.Release = form.Release;

                int modifiedRecordCount = Save();

                return modifiedRecordCount > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool AcceptOrReject(List<ApplicationForm> applications, bool accept)
        {
            try
            {
                if (applications == null || applications.Count <= 0)
                {
                    throw new Exception("No aplication found to Accept!");
                }

                bool done = false;
                foreach (ApplicationForm application in applications)
                {
                    Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == application.Id;
                    APPLICATION_FORM entity = GetEntityBy(selector);

                    if (entity != null)
                    {
                        entity.Rejected = accept;
                    }

                    done = repository.Save() > 0 ? true : false;
                }

                return done;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public ApplicationForm SetNextApplicationFormNumber(ApplicationForm form)
        //{
        //    try
        //    {
        //        long newSerialNumber = 0;
        //        Func<APPLICATION_FORM, long> selector = s => s.Serial_Number;
        //        long rawMaxSerialNumber = repository.GetMaxValueBy(selector);
        //        if (rawMaxSerialNumber > 0)
        //        {
        //            newSerialNumber = rawMaxSerialNumber + 1;
        //        }
        //        else
        //        {
        //            newSerialNumber = 1;
        //        }

        //        form.SerialNumber = newSerialNumber;
        //        form.Number = "FPI/" + form.ProgrammeFee.Programme.ShortName + "/" + DateTime.Now.ToString("yy") + "/" + PaddNumber(newSerialNumber, 10);

        //        return form;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public ApplicationForm SetNextApplicationFormNumber(ApplicationForm form)
        {
            try
            {
                form.SerialNumber = form.Id;
                form.Number = "CHSTECH/" + form.ProgrammeFee.Programme.ShortName + "/" + DateTime.Now.ToString("yy") + "/" + PaddNumber(form.Id, 10);

                return form;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ApplicationForm SetNextExamNumber(ApplicationForm form, AppliedCourse appliedCourse)
        {
            try
            {
                //Func<VW_APPLICATION_EXAM_NUMBER, int> selector = s => s.Application_Exam_Serial_Number.Value;
                //int rawMaxExamSerialNumber = repository.GetMaxValueBy(selector);

                int newExamSerialNumber = 0;
                List<ApplicationForm> applicationForms = (from a in repository.GetBy<VW_APPLICATION_EXAM_NUMBER>(a => a.Application_Form_Setting_Id == form.Setting.Id && a.Programme_Id == appliedCourse.Programme.Id && a.Department_Id == appliedCourse.Department.Id)
                                                          select new ApplicationForm
                                                          {
                                                              Id = a.Application_Form_Id,
                                                              SerialNumber = a.Serial_Number,
                                                              ExamSerialNumber = a.Application_Exam_Serial_Number,
                                                              ExamNumber = a.Application_Exam_Number,
                                                          }).ToList();

                if (applicationForms != null && applicationForms.Count > 0)
                {
                    int rawMaxExamSerialNumber = applicationForms.Max(a => a.ExamSerialNumber.Value);
                    newExamSerialNumber = rawMaxExamSerialNumber + 1;
                }
                else
                {
                    newExamSerialNumber = 1;
                }

                form.ExamSerialNumber = newExamSerialNumber;
                form.ExamNumber = appliedCourse.Department.Code + PaddNumber(newExamSerialNumber, 5);

                return form;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ApplicationForm> GetAllHndApplicants()
        {
            try
            {

                List<ApplicationForm> applicationForms = (from a in repository.GetBy<APPLICATION_FORM>(a => a.PERSON.APPLICANT_APPLIED_COURSE.Programme_Id == 3 || a.PERSON.APPLICANT_APPLIED_COURSE.Programme_Id == 4)
                                                          select new ApplicationForm
                                                          {
                                                              Id = a.Application_Form_Id,
                                                              SerialNumber = a.Serial_Number,
                                                              ExamSerialNumber = a.Application_Exam_Serial_Number,
                                                              ExamNumber = a.Application_Exam_Number,
                                                              Number = a.Application_Form_Number,

                                                          }).ToList();
                return applicationForms;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    StringBuilder builder = new StringBuilder();
                    for (int counter = 0; counter < zeroCount; counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhotoCard> GetPostJAMBApplications(Session session)
        {
            try
            {
                var applications = from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id)
                                   select new PhotoCard
                                   {
                                       PersonId = a.Person_Id,
                                       Name = a.Name,
                                       AplicationNumber = a.Application_Form_Id,
                                       PaymentNumber = a.Payment_Id,
                                       FirstChoiceDepartment = a.First_Choice_Department_Name,
                                       MobilePhone = a.Mobile_Phone,
                                       AppliedProgrammeName = a.Programme_Name,
                                       PassportUrl = a.Image_File_Url,
                                       AplicationFormNumber = a.Application_Form_Number,
                                   };

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhotoCard> GetPostJAMBApplicationsBy(Session session, Programme programme, SortOption sortOpton)
        {
            try
            {
                var applications = from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id)
                                   select new PhotoCard
                                   {
                                       PersonId = a.Person_Id,
                                       Name = a.Name,
                                       AplicationNumber = a.Application_Form_Id,
                                       PaymentNumber = a.Payment_Id,
                                       FirstChoiceDepartment = a.First_Choice_Department_Name,
                                       MobilePhone = a.Mobile_Phone,
                                       AppliedProgrammeName = a.Programme_Name,
                                       PassportUrl = a.Image_File_Url,
                                       AplicationFormNumber = a.Application_Form_Number,
                                       SessionName = a.Session_Name,
                                       AplicationSerialNumber = a.Serial_Number,
                                       ExamNumber = a.Application_Exam_Number,
                                       ExamSerialNumber = a.Application_Exam_Serial_Number,
                                   };

                applications = SortApplicantList(sortOpton, applications);

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhotoCard> GetPostJAMBApplicationsBy(Session session, Programme programme, Department department, SortOption sortOpton)
        {
            SessionLogic sessionLogic = new SessionLogic();
            try
            {
                var applications = from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Name == session.Name && a.Programme_Id == programme.Id && a.Department_Id == department.Id)
                                   select new PhotoCard
                                   {
                                       PersonId = a.Person_Id,
                                       Name = a.Name,
                                       AplicationNumber = a.Application_Form_Id,
                                       PaymentNumber = a.Payment_Id,
                                       FirstChoiceDepartment = a.First_Choice_Department_Name,
                                       MobilePhone = a.Mobile_Phone,
                                       AppliedProgrammeName = a.Programme_Name,
                                       PassportUrl = a.Image_File_Url,
                                       AplicationFormNumber = a.Application_Form_Number,
                                       SessionName = a.Session_Name,
                                       AplicationSerialNumber = a.Serial_Number,
                                       ExamNumber = a.Application_Exam_Number,
                                       ExamSerialNumber = a.Application_Exam_Serial_Number,
                                   };

                applications = SortApplicantList(sortOpton, applications);

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoCard> GetStudentExamPermitBy(Session session, Programme programme, Department department, Level level, SortOption sortOpton)
        {
            SessionLogic sessionLogic = new SessionLogic();
            List<PhotoCard> photoCardList = new List<PhotoCard>();
            try
            {
                var applications = (from a in repository.GetBy<VW_COMPREHENSIVE_SCHOOL_FEES_PAYMENT_REPORT>(a => a.Session_Name == session.Name && a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Level_Id == level.Id)
                                   select new PhotoCard
                                   {
                                       PersonId = a.Person_Id,
                                       Name = a.Name,
                                       MatriculationNumber = a.Matric_Number,
                                       DepartmentName = a.Department_Name,
                                       AppliedProgrammeName = a.Programme_Name,
                                       PassportUrl = a.Image_File_Url,
                                       SessionName = a.Session_Name,
                                       TrasactionAmount = (decimal)a.Transaction_Amount
                                   }).ToList();

                //applications = SortApplicantList(sortOpton, applications);

                //applications = applications.ToList();
                if (applications!=null)
                {
                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    var TotalSchoolFeeAmount = feeDetailLogic.GetModelsBy(f => f.Department_Id == department.Id && f.Programme_Id == programme.Id && f.Session_Id == session.Id && f.Level_Id == level.Id && f.Fee_Type_Id == (int)FeeTypes.SchoolFees).Sum(f => f.Fee.Amount);
                    var groupedList = applications.GroupBy(f => f.PersonId).ToList();
                    for (int i = 0; i < groupedList.Count; i++)
                    {
                        var personId = groupedList[i].Key;
                        var response = applications.Where(f => f.PersonId == personId).ToList();
                        if (response?.Count > 0)
                        {
                            var amountPaid = response.Sum(f => f.TrasactionAmount);
                            if (HasPaidSchoolFees(programme, amountPaid, TotalSchoolFeeAmount))
                            {
                                photoCardList.Add(response[0]);
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return photoCardList.OrderBy(f => f.MatriculationNumber).ToList();
        }
        public List<Slug> GetPostJAMBSlugDataBy(Session session, Programme programme, Department department)
        {
            try
            {
                if (programme.Id == 1)
                {
                    var applications = from a in repository.GetBy<VW_POST_JAMP_APPLICATION_ND_REGULAR>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id)
                                       select new Slug
                                       {
                                           PersonId = a.Person_Id,
                                           Name = a.Name,
                                           AplicationNumber = a.Application_Form_Id,
                                           PaymentNumber = a.Payment_Id,
                                           FirstChoiceDepartment = a.First_Choice_Department_Name,
                                           MobilePhone = a.Mobile_Phone,
                                           AppliedProgrammeName = a.Programme_Name,
                                           PassportUrl = a.Image_File_Url,
                                           AplicationFormNumber = a.Application_Form_Number,
                                           SessionName = a.Session_Name,
                                           ExamNumber = a.Application_Exam_Number,
                                           JambNumber = a.Applicant_Jamb_Registration_Number,
                                           JambScore = a.Applicant_Jamb_Score,
                                       };
                    if (applications != null && applications.Count() > 0)
                    {
                        applications = applications.OrderBy(a => a.ExamNumber);
                    }
                    return applications.ToList();
                }
                else
                {
                    var applications = from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id)
                                       select new Slug
                                       {
                                           PersonId = a.Person_Id,
                                           Name = a.Name,
                                           AplicationNumber = a.Application_Form_Id,
                                           PaymentNumber = a.Payment_Id,
                                           FirstChoiceDepartment = a.First_Choice_Department_Name,
                                           MobilePhone = a.Mobile_Phone,
                                           AppliedProgrammeName = a.Programme_Name,
                                           PassportUrl = a.Image_File_Url,
                                           AplicationFormNumber = a.Application_Form_Number,
                                           SessionName = a.Session_Name,
                                           ExamNumber = a.Application_Exam_Number

                                       };
                    if (applications != null && applications.Count() > 0)
                    {
                        applications = applications.OrderBy(a => a.ExamNumber);
                    }
                    return applications.ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        private IEnumerable<PhotoCard> SortApplicantList(SortOption sortOpton, IEnumerable<PhotoCard> applications)
        {
            try
            {
                if (applications != null && applications.Count() > 0)
                {
                    switch (sortOpton)
                    {
                        case SortOption.Name:
                            {
                                applications = applications.OrderBy(a => a.Name);
                                //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.Name);
                                break;
                            }
                        case SortOption.ExamNo:
                            {
                                applications = applications.OrderBy(a => a.ExamSerialNumber);
                                //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.ExamSerialNumber);
                                break;
                            }
                        case SortOption.ApplicationNo:
                            {
                                applications = applications.OrderBy(a => a.AplicationSerialNumber);
                                //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.AplicationSerialNumber);
                                break;
                            }
                    }
                }

                return applications;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private APPLICATION_FORM GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = s => s.Person_Id == person.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SetApplicationAndExamNumber(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = af => af.Application_Form_Id == applicationForm.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Application_Form_Number = applicationForm.Number;
                entity.Serial_Number = applicationForm.SerialNumber;
                entity.Application_Exam_Number = applicationForm.ExamNumber;
                entity.Application_Exam_Serial_Number = applicationForm.ExamSerialNumber;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(ApplicationForm applicationForm)
        {
            try
            {
                APPLICATION_FORM entity = GetEntityBy(applicationForm.Person);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Application_Form_Id = applicationForm.Id;
                entity.Application_Form_Setting_Id = applicationForm.Setting.Id;
                entity.Application_Programme_Fee_Id = applicationForm.ProgrammeFee.Id;
                entity.Payment_Id = applicationForm.Payment.Id;
                entity.Person_Id = applicationForm.Person.Id;
                entity.Date_Submitted = DateTime.Now;
                entity.Release = applicationForm.Release;
                entity.Rejected = applicationForm.Rejected;
                entity.Remarks = applicationForm.Remarks;
                entity.Application_Form_Number = applicationForm.Number;
                entity.Application_Exam_Number = applicationForm.ExamNumber;
                entity.Application_Exam_Serial_Number = applicationForm.ExamSerialNumber;
                entity.Serial_Number = applicationForm.SerialNumber;
                entity.Reject_Reason = applicationForm.RejectReason;
                entity.Verification_Comment = applicationForm.VerificationComment;
                entity.Verification_Status = applicationForm.VerificationStatus;
                entity.Cleard_For_Acceptance = applicationForm.CleardForAcceptance;
                if (applicationForm.VerificationOfficer != null)
                {
                    entity.Verification_Officer = applicationForm.VerificationOfficer.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ApplicationFormSummary> GetSummary(Session session)
        {
            try
            {
                List<ApplicationFormSummary> applicationForms = (from a in repository.GetBy<VW_APPLICATION_FORM_SUMMARY>(a => a.Session_Id == session.Id)
                                                                 select new ApplicationFormSummary
                                                                 {
                                                                     ProgrammeId = a.Programme_Id,
                                                                     ProgrammeName = a.Programme_Name,
                                                                     DepartmentName = a.Department_Name,
                                                                     SessionName = a.Session_Name,
                                                                     FormCount = (int)a.Application_Form_Count,
                                                                 }).ToList();

                return applicationForms;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ApplicationForm GetBy(long applicationFormId)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == applicationFormId;

                ApplicationForm applicationForm = GetModelBy(selector);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    applicationForm.Payment = paymentLogic.GetBy(applicationForm.Payment.Id);
                }

                return applicationForm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ApplicationForm GetBy(Person person)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Person_Id == person.Id;

                ApplicationForm applicationForm = GetModelBy(selector);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    applicationForm.Payment = paymentLogic.GetBy(applicationForm.Payment.Id);
                }

                return applicationForm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DuplicateApplicationNumber> GetDuplicateApplicationForm()
        {
            try
            {
                List<DuplicateApplicationNumber> applicationForms = (from a in repository.GetBy<VW_DUPLICATE_APPLICATION_NUMBER>(a => a.Application_Form_Id > 0)
                                                                     select new DuplicateApplicationNumber
                                                                     {
                                                                         ApplicationId = a.Application_Form_Id,
                                                                         SerialNumber = a.Serial_Number,
                                                                         Number = a.Application_Form_Number,
                                                                         ApplicationFormSetting = a.Application_Form_Setting_Id,
                                                                         ApplicationProgrammeFee = a.Application_Programme_Fee_Id,
                                                                         PersonId = a.Person_Id,
                                                                         PaymentId = a.Payment_Id,
                                                                         Rejected = a.Rejected,
                                                                         ExamSerialNumber = a.Application_Exam_Serial_Number,
                                                                         ExamNumber = a.Application_Exam_Number,
                                                                         RejectReason = a.Reject_Reason,
                                                                         AdmissionListId = a.Admission_List_Id,
                                                                         AdmissionListBatchId = a.Admission_List_Batch_Id,
                                                                         DeprtmentId = a.Department_Id,
                                                                         DepartmentOptionId = a.Department_Option_Id,
                                                                         SessionId = a.Session_Id,

                                                                     }).ToList();

                return applicationForms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool HasPaidSchoolFees(Programme programme, decimal AmountPaid,decimal AmountToPay)
        {
            if (programme.Id == (int)Programmes.HNDPartTime || programme.Id == (int)Programmes.NDPartTime)
            {
                decimal eightyPercentOfFirstInstallment = AmountToPay * (80M / 100M);
                if (AmountPaid >= eightyPercentOfFirstInstallment)
                    return true;
                return false;
            }
            else if (programme.Id == (int)Programmes.HNDEvening || programme.Id == (int)Programmes.NDEveningFullTime)
            {
                decimal fiftyPercentOfFullPayment = AmountToPay * (45M / 100M);
                if (AmountPaid >= fiftyPercentOfFullPayment)
                    return true;
                return false;
            }
            else
            {
                if (AmountPaid >= AmountToPay)
                    return true;
                return false;
            }
        }


    }

}
