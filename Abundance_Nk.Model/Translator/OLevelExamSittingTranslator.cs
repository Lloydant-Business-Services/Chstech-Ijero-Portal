using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelExamSittingTranslator : TranslatorBase<OLevelExamSitting, O_LEVEL_EXAM_SITTING>
    {
        public override OLevelExamSitting TranslateToModel(O_LEVEL_EXAM_SITTING entity)
        {
            try
            {
                OLevelExamSitting oLevelExamSitting = null;
                if (entity != null)
                {
                    oLevelExamSitting = new OLevelExamSitting();
                    oLevelExamSitting.Id = entity.O_Level_Exam_Sitting_Id;
                    oLevelExamSitting.Name = entity.O_Level_Exam_Sitting_Name;
                }

                return oLevelExamSitting;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override O_LEVEL_EXAM_SITTING TranslateToEntity(OLevelExamSitting oLevelExamSitting)
        {
            try
            {
                O_LEVEL_EXAM_SITTING entity = null;
                if (oLevelExamSitting != null)
                {
                    entity = new O_LEVEL_EXAM_SITTING();
                    entity.O_Level_Exam_Sitting_Id = oLevelExamSitting.Id;
                    entity.O_Level_Exam_Sitting_Name = oLevelExamSitting.Name;
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
