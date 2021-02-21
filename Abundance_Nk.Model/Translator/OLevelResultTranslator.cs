using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelResultTranslator : TranslatorBase<OLevelResult, APPLICANT_O_LEVEL_RESULT>
    {
        private PersonTranslator personTranslator;
        private OLevelExamSittingTranslator oLevelExamSittingTranslator;
        private OLevelTypeTranslator oLevelTypeTranslator;
        private ApplicationFormTranslator applicationFormTranslator;

        public OLevelResultTranslator()
        {
            personTranslator = new PersonTranslator();
            oLevelExamSittingTranslator = new OLevelExamSittingTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            oLevelTypeTranslator = new OLevelTypeTranslator();
        }

        public override OLevelResult TranslateToModel(APPLICANT_O_LEVEL_RESULT entity)
        {
            try
            {
                OLevelResult oLevelResult = null;
                if (entity != null)
                {
                    oLevelResult = new OLevelResult();
                    oLevelResult.Id = entity.Applicant_O_Level_Result_Id;
                    oLevelResult.Person = personTranslator.Translate(entity.PERSON);
                    oLevelResult.ExamNumber = entity.Exam_Number;
                    oLevelResult.ExamYear = entity.Exam_Year;
                    oLevelResult.ScannedCopyUrl = entity.Scanned_Copy_Url;
                    oLevelResult.Sitting = oLevelExamSittingTranslator.Translate(entity.O_LEVEL_EXAM_SITTING);
                    oLevelResult.Type = oLevelTypeTranslator.Translate(entity.O_LEVEL_TYPE);
                    oLevelResult.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    oLevelResult.ScratchCard = entity.Scratch_Card_Pin;
                    oLevelResult.ScratchCardSerialNo = entity.Scratch_Card_SerialNo;
                }

                return oLevelResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_O_LEVEL_RESULT TranslateToEntity(OLevelResult oLevelResult)
        {
            try
            {
                APPLICANT_O_LEVEL_RESULT entity = null;
                if (oLevelResult != null)
                {
                    entity = new APPLICANT_O_LEVEL_RESULT();
                    entity.Applicant_O_Level_Result_Id = oLevelResult.Id;
                    entity.Person_Id = oLevelResult.Person.Id;
                    entity.Exam_Number = oLevelResult.ExamNumber;
                    entity.Scanned_Copy_Url = oLevelResult.ScannedCopyUrl;
                    entity.Exam_Year = oLevelResult.ExamYear;
                    entity.O_Level_Exam_Sitting_Id = oLevelResult.Sitting.Id;
                    entity.O_Level_Type_Id = oLevelResult.Type.Id;
                    entity.Scratch_Card_Pin = oLevelResult.ScratchCard;
                    entity.Scratch_Card_SerialNo = oLevelResult.ScratchCardSerialNo;
                    if (oLevelResult.ApplicationForm != null && oLevelResult.ApplicationForm.Id > 0)
                    {
                        entity.Application_Form_Id = oLevelResult.ApplicationForm.Id;
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
