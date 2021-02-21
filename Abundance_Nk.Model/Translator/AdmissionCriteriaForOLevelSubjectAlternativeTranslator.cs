using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;


namespace Abundance_Nk.Model.Translator
{
    public class AdmissionCriteriaForOLevelSubjectAlternativeTranslator : TranslatorBase<AdmissionCriteriaForOLevelSubjectAlternative, ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE>
    {
        private OLevelSubjectTranslator oLevelSubjectTranslator;
        private AdmissionCriteriaForOLevelSubjectTranslator admissionCriteriaForOLevelSubjectTranslator;

        public AdmissionCriteriaForOLevelSubjectAlternativeTranslator()
        {
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
            admissionCriteriaForOLevelSubjectTranslator = new AdmissionCriteriaForOLevelSubjectTranslator();
        }

        public override AdmissionCriteriaForOLevelSubjectAlternative TranslateToModel(ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE entity)
        {
            try
            {
                AdmissionCriteriaForOLevelSubjectAlternative model = null;
                if (entity != null)
                {
                    model = new AdmissionCriteriaForOLevelSubjectAlternative();
                    model.Id = entity.Admission_Criteria_For_O_Level_Subject_Alternative_Id;
                    model.Alternative = admissionCriteriaForOLevelSubjectTranslator.Translate(entity.ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT);
                    model.OLevelSubject = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE TranslateToEntity(AdmissionCriteriaForOLevelSubjectAlternative model)
        {
            try
            {
                ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE();
                    entity.Admission_Criteria_For_O_Level_Subject_Alternative_Id = model.Id;
                    entity.Admission_Criteria_For_O_Level_Subject_Id =model.Alternative.Id;
                    entity.O_Level_Subject_Id = model.OLevelSubject.Id;
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
