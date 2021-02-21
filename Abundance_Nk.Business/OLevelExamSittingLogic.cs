
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
    public class OLevelExamSittingLogic : BusinessBaseLogic<OLevelExamSitting, O_LEVEL_EXAM_SITTING>
    {
        public OLevelExamSittingLogic()
        {
            translator = new OLevelExamSittingTranslator();
        }

        public bool Modify(OLevelExamSitting oLevelExamSitting)
        {
            try
            {
                Expression<Func<O_LEVEL_EXAM_SITTING, bool>> selector = o => o.O_Level_Exam_Sitting_Id == oLevelExamSitting.Id;
                O_LEVEL_EXAM_SITTING entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.O_Level_Exam_Sitting_Name = oLevelExamSitting.Name;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
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
