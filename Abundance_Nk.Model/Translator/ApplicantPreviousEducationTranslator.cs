using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantPreviousEducationTranslator: TranslatorBase<ApplicantPreviousEducation, APPLICANT_PREVIOUS_EDUCATION>
    {
        private ApplicationFormTranslator ApplicationFormTranslator;
        private PersonTranslator PersonTranslator;
        private ResultGradeTranslator ResultGradeTranslator;
        private TertiaryInstitutionTranslator TertiaryInstitutionTranslator;
        private EducationalQualificationTranslator EducationalQualificationTranslator;
        private ITDurationTranslator ITDurationTranslator;

        public ApplicantPreviousEducationTranslator()
        {
            ApplicationFormTranslator = new ApplicationFormTranslator();
            PersonTranslator = new PersonTranslator();
            ResultGradeTranslator = new ResultGradeTranslator();
            TertiaryInstitutionTranslator = new TertiaryInstitutionTranslator();
            EducationalQualificationTranslator = new EducationalQualificationTranslator();
            ITDurationTranslator = new ITDurationTranslator();
        }

        public override APPLICANT_PREVIOUS_EDUCATION TranslateToEntity(ApplicantPreviousEducation model)
        {
            try
            {
                APPLICANT_PREVIOUS_EDUCATION entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_PREVIOUS_EDUCATION();
                    entity.Applicant_Previous_Education_Id = model.Applicant_Previous_Education_Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Previous_Course = model.Previous_Course;
                    entity.Previous_School_Name = model.Previous_School_Name;
                    entity.Previous_Education_End_Date = model.Previous_Education_End_Date;
                    entity.Previous_Education_Start_Date = model.Previous_Education_Start_Date;
                    entity.Educational_Qualification_Id = model.Educational_Qualification_Id;
                    entity.Result_Grade_Id = model.Result_Grade_Id;
                    entity.IT_Duration_Id = model.IT_Duration_Id;
                    entity.Application_Form_Id = model.Application_Form_Id;
                    entity.Previous_School_Id = model.Previous_School_Id;
                    entity.IT_Start_Date = model.IT_Start_Date;
                    entity.IT_End_Date = model.IT_End_Date;
                }

                return entity;
            }
            catch(Exception ex) { throw ex; }
        }

        public override ApplicantPreviousEducation TranslateToModel(APPLICANT_PREVIOUS_EDUCATION entity)
        {
            try
            {
                ApplicantPreviousEducation model = null;
                if (entity != null)
                {
                    model = new ApplicantPreviousEducation();
                    model.Applicant_Previous_Education_Id = entity.Applicant_Previous_Education_Id;
                    model.Person = PersonTranslator.Translate(entity.PERSON);
                    model.Previous_School_Name = entity.Previous_School_Name;
                    model.Previous_Course = entity.Previous_Course;
                    model.Previous_Education_Start_Date = entity?.Previous_Education_Start_Date;
                    model.Previous_Education_End_Date = entity.Previous_Education_End_Date;
                    model.EducationalQualification = EducationalQualificationTranslator.Translate(entity.EDUCATIONAL_QUALIFICATION);
                    model.ResultGrade = ResultGradeTranslator.Translate(entity.RESULT_GRADE);
                    model.ITDuration = ITDurationTranslator.Translate(entity.IT_DURATION);
                    model.ApplicationForm = ApplicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Previous_School_Id = entity?.Previous_School_Id;
                    model.IT_Start_Date = entity.IT_Start_Date;
                    model.IT_End_Date = entity.IT_End_Date;
                }

                return model;
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
