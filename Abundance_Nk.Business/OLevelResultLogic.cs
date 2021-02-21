using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class OLevelResultLogic : BusinessBaseLogic<OLevelResult, APPLICANT_O_LEVEL_RESULT>
    {
        public OLevelResultLogic()
        {
            translator = new OLevelResultTranslator();
        }

        public bool Modify(OLevelResult oLevelResult)
        {
            try
            {
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> selector = o => o.Applicant_O_Level_Result_Id == oLevelResult.Id;
                APPLICANT_O_LEVEL_RESULT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (oLevelResult.Person != null && oLevelResult.Person.Id > 0)
                {
                    entity.Person_Id = oLevelResult.Person.Id;
                }
                if (oLevelResult.ExamNumber != null)
                {
                    entity.Exam_Number = oLevelResult.ExamNumber;
                }
                if (oLevelResult.ExamYear != null)
                {
                    entity.Exam_Year = oLevelResult.ExamYear;
                }
                if (oLevelResult.Sitting != null && oLevelResult.Sitting.Id > 0)
                {
                    entity.O_Level_Exam_Sitting_Id = oLevelResult.Sitting.Id;
                }
                if (oLevelResult.Type != null && oLevelResult.Type.Id > 0)
                {
                    entity.O_Level_Type_Id = oLevelResult.Type.Id;
                }
                if (oLevelResult.ApplicationForm != null && oLevelResult.ApplicationForm.Id > 0)
                {
                    entity.Application_Form_Id = oLevelResult.ApplicationForm.Id;
                }
                entity.Scanned_Copy_Url = oLevelResult.ScannedCopyUrl;
                entity.Scratch_Card_Pin = oLevelResult.ScratchCard;

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



    }




}
