using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class AdmissionCriteriaForOLevelSubjectTranslator : TranslatorBase<AdmissionCriteriaForOLevelSubject, ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT>
    {
        private OLevelSubjectTranslator oLevelSubjectTranslator;
        private AdmissionCriteriaTranslator admissionCriteriaTranslator;
        private OLevelGradeTranslator oLevelGradeTranslator;
        //private AdmissionCriteriaForOLevelSubjectAlternativeTranslator admissionCriteriaForOLevelSubjectAlternativeTranslator;

        public AdmissionCriteriaForOLevelSubjectTranslator()
        {
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
            admissionCriteriaTranslator = new AdmissionCriteriaTranslator();
            //admissionCriteriaForOLevelSubjectAlternativeTranslator = new AdmissionCriteriaForOLevelSubjectAlternativeTranslator();
            oLevelGradeTranslator = new OLevelGradeTranslator();
        }

        public override AdmissionCriteriaForOLevelSubject TranslateToModel(ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT entity)
        {
            try
            {
                AdmissionCriteriaForOLevelSubject model = null;
                if (entity != null)
                {
                    model = new AdmissionCriteriaForOLevelSubject();
                    model.Id = entity.Admission_Criteria_For_O_Level_Subject_Id;
                    model.MainCriteria = admissionCriteriaTranslator.Translate(entity.ADMISSION_CRITERIA);
                    model.Subject = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT);
                    model.MinimumGrade = oLevelGradeTranslator.Translate(entity.O_LEVEL_GRADE);
                    model.IsCompulsory = entity.Is_Compulsory;

                    //admissionCriteriaForOLevelSubjectAlternativeTranslator.Translate(entity.ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT TranslateToEntity(AdmissionCriteriaForOLevelSubject model)
        {
            try
            {
                ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT();
                    entity.Admission_Criteria_For_O_Level_Subject_Id = model.Id;
                    entity.Admission_Criteria_Id = model.MainCriteria.Id;
                    entity.O_Level_Subject_Id = model.Subject.Id;
                    entity.Minimum_O_Level_Grade_Id = model.MinimumGrade.Id;
                    entity.Is_Compulsory = model.IsCompulsory;
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
