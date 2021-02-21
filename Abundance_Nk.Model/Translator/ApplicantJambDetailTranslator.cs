using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class ApplicantJambDetailTranslator : TranslatorBase<ApplicantJambDetail, APPLICANT_JAMB_DETAIL>
    {
        private PersonTranslator personTranslator;
        private InstitutionChoiceTranslator institutionChoiceTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private OLevelSubjectTranslator oLevelSubjectTranslator;

        public ApplicantJambDetailTranslator()
        {
            personTranslator = new PersonTranslator();
            institutionChoiceTranslator = new InstitutionChoiceTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
        }

        public override ApplicantJambDetail TranslateToModel(APPLICANT_JAMB_DETAIL entity)
        {
            try
            {
                ApplicantJambDetail model = null;
                if (entity != null)
                {
                    model = new ApplicantJambDetail();
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.JambRegistrationNumber = entity.Applicant_Jamb_Registration_Number;
                    model.InstitutionChoice = institutionChoiceTranslator.Translate(entity.INSTITUTION_CHOICE);
                    model.JambScore = entity.Applicant_Jamb_Score;
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Subject1 = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT);
                    model.Subject2 = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT1);
                    model.Subject3 = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT2);
                    model.Subject4 = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT3);
                    model.Subject1Score = entity.Subject1_Score;
                    model.Subject2Score = entity.Subject2_Score;
                    model.Subject3Score = entity.Subject3_Score;
                    model.Subject4Score = entity.Subject4_Score;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_JAMB_DETAIL TranslateToEntity(ApplicantJambDetail model)
        {
            try
            {
                APPLICANT_JAMB_DETAIL entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_JAMB_DETAIL();
                    entity.Person_Id = model.Person.Id;
                    entity.Applicant_Jamb_Registration_Number = model.JambRegistrationNumber;
                    entity.Applicant_Jamb_Score = model.JambScore;
                    if (model.Subject1 != null)
                    {
                        entity.Subject1 = model.Subject1.Id; 
                    }
                    if (model.Subject2 != null)
                    {
                        entity.Subject2 = model.Subject2.Id;
                    }
                    if (model.Subject3 != null)
                    {
                        entity.Subject3 = model.Subject3.Id;
                    }
                    if (model.Subject4 != null)
                    {
                        entity.Subject4 = model.Subject4.Id;
                    }

                    if (model.InstitutionChoice != null)
                    {
                        entity.Institution_Choice_Id = model.InstitutionChoice.Id;
                    }

                    if (model.ApplicationForm != null)
                    {
                        entity.Application_Form_Id = model.ApplicationForm.Id;
                    }

                    entity.Subject1_Score = model.Subject1Score;
                    entity.Subject2_Score = model.Subject2Score;
                    entity.Subject3_Score = model.Subject3Score;
                    entity.Subject4_Score = model.Subject4Score;
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
