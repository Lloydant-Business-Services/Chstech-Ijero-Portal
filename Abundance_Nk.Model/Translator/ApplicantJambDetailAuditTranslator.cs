using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantJambDetailAuditTranslator : TranslatorBase<ApplicantJambDetailAudit, APPLICANT_JAMB_DETAIL_AUDIT>
    {
        public override ApplicantJambDetailAudit TranslateToModel(APPLICANT_JAMB_DETAIL_AUDIT entity)
        {
            try
            {
                ApplicantJambDetailAudit model = null;
                if (entity != null)
                {
                    model = new ApplicantJambDetailAudit();
                    model.ApplicantJambRegistrationNumber = entity.Applicant_Jamb_Registration_Number;
                    model.PersonId = entity.Person_Id;
                    model.ApplicantJambScore = entity.Applicant_Jamb_Score;
                    model.ApplicationFormId = entity.Application_Form_Id;
                    model.InstitutionChoiceId = entity.Institution_Choice_Id;
                    model.Subject1 = entity.Subject1;
                    model.Subject2 = entity.Subject2;
                    model.Subject3 = entity.Subject3;
                    model.Subject4 = entity.Subject4;
                    model.Subject1Score = entity.Subject1_Score;
                    model.Subject2Score = entity.Subject2_Score;
                    model.Subject3Score = entity.Subject3_Score;
                    model.Subject4Score = entity.Subject4_Score;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;
                    model.UserId = entity.User_Id;

                }
                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override APPLICANT_JAMB_DETAIL_AUDIT TranslateToEntity(ApplicantJambDetailAudit model)
        {
            try
            {
                APPLICANT_JAMB_DETAIL_AUDIT entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_JAMB_DETAIL_AUDIT();
                    entity.Applicant_Jamb_Registration_Number = model.ApplicantJambRegistrationNumber;
                    entity.Person_Id = model.PersonId;
                    entity.Applicant_Jamb_Score = model.ApplicantJambScore;
                    entity.Application_Form_Id = model.ApplicationFormId;
                    entity.Institution_Choice_Id = model.InstitutionChoiceId;
                    entity.Subject1 = model.Subject1;
                    entity.Subject2 = model.Subject2;
                    entity.Subject3 = model.Subject3;
                    entity.Subject4 = model.Subject4;
                    entity.Subject1_Score = model.Subject1Score;
                    entity.Subject2_Score = model.Subject2Score;
                    entity.Subject3_Score = model.Subject3Score;
                    entity.Subject4_Score = model.Subject4Score;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
                    entity.User_Id = model.UserId;
                }
                return entity;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
