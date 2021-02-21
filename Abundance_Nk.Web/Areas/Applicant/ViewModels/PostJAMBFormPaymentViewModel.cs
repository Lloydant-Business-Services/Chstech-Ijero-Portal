using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using System.ComponentModel.DataAnnotations;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Models;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostJAMBFormPaymentViewModel
    {
        public PostJAMBFormPaymentViewModel()
        {
            Programme = new Programme();
            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            remitaPayment = new RemitaPayment();

            Person = new Person();
            Person.State = new State();
            StateSelectList = Utility.PopulateStateSelectListItem();
            //ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateApplicationProgrammeSelectListItem();
           
        }

        public AppliedCourse AppliedCourse { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public Department Department { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        [Display(Name = "JAMB Reg. No")]
        public string JambRegistrationNumber { get; set; }
        public Programme Programme { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public FeeType FeeType { get; set; }
        public Person Person { get; set; }
        public decimal Amount { get; set; }
        public Session CurrentSession { get; set; }
        public PaymentType PaymentType { get; set; }
        public Payment Payment { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public PaymentEtranzactType PaymentEtranzactType { get; set; }
        public Level Level { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public string Hash { get; internal set; }

        public void Initialise()
        {
            try
            {
                CurrentSession = GetApplicationSession();

                if (CurrentSession != null && Programme.Id > 0)
                {
                    FeeType = GetFeeTypeBy(CurrentSession, Programme);
                    ApplicationFormSetting = GetApplicationFormSettingBy(CurrentSession, FeeType);

                    ProgrammeLevelLogic programmeLevelLogic = new ProgrammeLevelLogic();
                    Level = programmeLevelLogic.GetModelsBy(p => p.Programme_Id == Programme.Id).FirstOrDefault().Level;
                    
                    PaymentMode paymentMode = new PaymentMode(){Id = (int)PaymentModes.Full};

                    PaymentEtranzactType = GetPaymentTypeBy(FeeType, Programme, Level, paymentMode, CurrentSession);
                    ApplicationProgrammeFee = GetApplicationProgrammeFeeBy(Programme, FeeType, CurrentSession);
                    if (ApplicationFormSetting != null)
                    {
                        PaymentType = ApplicationFormSetting.PaymentType;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ApplicationProgrammeFee GetApplicationProgrammeFeeBy(Programme Programme, FeeType FeeType, Session CurrentSession)
        {
            try
            {
                ApplicationProgrammeFeeLogic applicationProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();
                ApplicationProgrammeFee applicationProgrammeFee = applicationProgrammeFeeLogic.GetModelBy(p => p.Fee_Type_Id == FeeType.Id && p.Programme_Id == Programme.Id && p.Session_Id == CurrentSession.Id);
                return applicationProgrammeFee;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public Session GetApplicationSession()
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                Session session = sessionLogic.GetModelsBy(a => a.Active_For_Application.Value == true).LastOrDefault();
                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Session GetCurrentSession()
        {
            try
            {
                //CurrentSessionSemesterLogic currentSessionLogic = new CurrentSessionSemesterLogic();
                //CurrentSessionSemester currentSessionSemester = currentSessionLogic.GetCurrentSessionTerm();
                //if (currentSessionSemester != null && currentSessionSemester.SessionSemester != null)
                //{
                //    return currentSessionSemester.SessionSemester.Session;
                //}

                SessionLogic sessionLogic = new SessionLogic();
                Session session = sessionLogic.GetModelBy(a => a.Session_Id == 7);
                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public FeeType GetFeeTypeBy(Session session, Programme programme)
        //{
        //    try
        //    {
        //        ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
        //        ApplicationProgrammeFee = programmeFeeLogic.GetBy(programme, session);

        //        if (ApplicationProgrammeFee != null)
        //        {
        //            return ApplicationProgrammeFee.FeeType;
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public FeeType GetFeeTypeBy(Session session, Programme programme)
        {
            try
            {
                ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                List<ApplicationProgrammeFee> applicationProgrammeFess = programmeFeeLogic.GetListBy(programme, session);
                foreach (ApplicationProgrammeFee item in applicationProgrammeFess)
                {
                    if (item.FeeType.Id <= 21)
                    {
                        return item.FeeType;
                    }
                }
              
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public PaymentEtranzactType GetPaymentTypeBy(FeeType feeType, Programme programme, Level level, PaymentMode paymentMode, Session session)
        {
            PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
            //PaymentEtranzactType = paymentEtranzactTypeLogic.GetBy(feeType);
            PaymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == feeType.Id && p.Programme_Id == programme.Id && p.Level_Id == level.Id && p.Payment_Mode_Id == paymentMode.Id && p.Session_Id == session.Id).LastOrDefault();

            if (PaymentEtranzactType != null)
            {
                return PaymentEtranzactType;
            }

            return null;
        }

        public ApplicationFormSetting GetApplicationFormSettingBy(Session session, FeeType feeType)
        {
            try
            {
                ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();
                return applicationFormSettingLogic.GetBy(session, feeType);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ResponseUrl { get; set; }
    }
}