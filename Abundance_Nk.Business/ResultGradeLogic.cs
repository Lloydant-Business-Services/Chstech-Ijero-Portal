
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
    public class ResultGradeLogic : BusinessBaseLogic<ResultGrade, RESULT_GRADE>
    {
        public ResultGradeLogic()
        {
            translator = new ResultGradeTranslator();
        }

        public bool Modify(ResultGrade resultGrade)
        {
            try
            {
                Expression<Func<RESULT_GRADE, bool>> selector = r => r.Result_Grade_Id == resultGrade.Id;
                RESULT_GRADE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Level_of_Pass = resultGrade.LevelOfPass;

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
