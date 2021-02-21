using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelResultAuditTranslator:TranslatorBase<OlevelResultAudit,APPLICANT_O_LEVEL_RESULT_AUDIT>
    {
        private PersonTranslator personTranslator;
        private OLevelExamSittingTranslator oLevelExamSittingTranslator;
        private OLevelTypeTranslator oLevelTypeTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private OLevelResultTranslator oLevelResultTranslator;
        private UserTranslator userTranslator;
        public OLevelResultAuditTranslator()
        {
            personTranslator = new PersonTranslator();
            oLevelExamSittingTranslator = new OLevelExamSittingTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            oLevelTypeTranslator = new OLevelTypeTranslator();
            oLevelResultTranslator = new OLevelResultTranslator();
            userTranslator = new UserTranslator();
        }
        public override OlevelResultAudit TranslateToModel(APPLICANT_O_LEVEL_RESULT_AUDIT entity)
        {
            try
            {
                OlevelResultAudit model = null;
                if (entity != null)
                {
                    model = new OlevelResultAudit();
                    model.Id = entity.Applicant_O_Level_Result_Audit_Id;
                    model.OLevelResult = oLevelResultTranslator.Translate(entity.APPLICANT_O_LEVEL_RESULT);
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Type = oLevelTypeTranslator.Translate(entity.O_LEVEL_TYPE);
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Sitting = oLevelExamSittingTranslator.Translate(entity.O_LEVEL_EXAM_SITTING);
                    model.ExamNumber = entity.Exam_Number;
                    model.ExamYear = entity.Exam_Year;
                    model.ScannedCopyUrl = entity.Scanned_Copy_Url;
                    model.OldExamNumber = entity.Old_Exam_Number;
                    model.OldExamYear = entity.Old_Exam_Year;
                    model.OldScannedCopyUrl = entity.Old_Scanned_Copy_Url;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override APPLICANT_O_LEVEL_RESULT_AUDIT TranslateToEntity(OlevelResultAudit model)
        {
            try
            {
                APPLICANT_O_LEVEL_RESULT_AUDIT entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_O_LEVEL_RESULT_AUDIT();
                    entity.Applicant_O_Level_Result_Audit_Id = model.Id;
                    entity.Applicant_O_Level_Result_Id = model.OLevelResult.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Exam_Year = model.ExamYear;
                    entity.Exam_Number = model.ExamNumber;
                    entity.Scanned_Copy_Url = model.ScannedCopyUrl;
                    entity.Old_Exam_Number = model.OldExamNumber;
                    entity.Old_Exam_Year = model.OldExamYear;
                    entity.Old_Scanned_Copy_Url = model.OldScannedCopyUrl;
                    entity.O_Level_Type_Id = model.Type.Id;
                    entity.O_Level_Exam_Sitting_Id = model.Sitting.Id;
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
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
