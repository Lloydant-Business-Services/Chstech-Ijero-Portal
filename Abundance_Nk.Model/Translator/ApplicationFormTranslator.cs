using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicationFormTranslator : TranslatorBase<ApplicationForm, APPLICATION_FORM>
    {
        private PersonTranslator personTranslator;
        private PaymentTranslator paymentTranslator;
        private ApplicationFormSettingTranslator applicationFormSettingTranslator;
        private ApplicationProgrammeFeeTranslator applicationProgrammeFeeTranslator;
        private UserTranslator userTranslator;
        
        public ApplicationFormTranslator()
        {
            personTranslator = new PersonTranslator();
            paymentTranslator = new PaymentTranslator();
            applicationFormSettingTranslator = new ApplicationFormSettingTranslator();
            applicationProgrammeFeeTranslator = new ApplicationProgrammeFeeTranslator();
            userTranslator = new UserTranslator();
        }

        public override ApplicationForm TranslateToModel(APPLICATION_FORM entity)
        {
            try
            {
                ApplicationForm model = null;
                if (entity != null)
                {
                    model = new ApplicationForm();
                    model.Id = entity.Application_Form_Id;
                    model.SerialNumber = entity.Serial_Number;
                    model.Number = entity.Application_Form_Number;
                    model.Setting = applicationFormSettingTranslator.Translate(entity.APPLICATION_FORM_SETTING);
                    model.ProgrammeFee = applicationProgrammeFeeTranslator.Translate(entity.APPLICATION_PROGRAMME_FEE);
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.DateSubmitted = entity.Date_Submitted;
                    model.ExamSerialNumber = entity.Application_Exam_Serial_Number;
                    model.ExamNumber = entity.Application_Exam_Number;
                    model.Release = entity.Release;
                    model.Rejected = entity.Rejected;
                    model.RejectReason = entity.Reject_Reason;
                    model.Remarks = entity.Remarks;
                    model.VerificationComment = entity.Verification_Comment;
                    model.VerificationStatus = entity.Verification_Status;
                    model.VerificationOfficer = userTranslator.Translate(entity.USER);
                    model.CleardForAcceptance = entity.Cleard_For_Acceptance;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICATION_FORM TranslateToEntity(ApplicationForm model)
        {
            try
            {
                APPLICATION_FORM entity = null;
                if (model != null)
                {
                    entity = new APPLICATION_FORM();
                    entity.Application_Form_Id = model.Id;
                    entity.Serial_Number = model.SerialNumber;
                    entity.Application_Form_Number = model.Number;
                    entity.Application_Form_Setting_Id = model.Setting.Id;
                    entity.Application_Programme_Fee_Id = model.ProgrammeFee.Id;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Date_Submitted = model.DateSubmitted;
                    entity.Application_Exam_Serial_Number = model.ExamSerialNumber;
                    entity.Application_Exam_Number = model.ExamNumber;
                    entity.Release = model.Release;
                    entity.Rejected = model.Rejected;
                    entity.Reject_Reason = model.RejectReason;
                    entity.Remarks = model.Remarks;
                    entity.Verification_Comment = model.VerificationComment;
                    entity.Verification_Status = model.VerificationStatus;
                    entity.Cleard_For_Acceptance = model.CleardForAcceptance;
                    if (model.VerificationOfficer != null)
                    {
                        entity.Verification_Officer = model.VerificationOfficer.Id;
                    }
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }






    }

}
