using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelGradeTranslator : TranslatorBase<OLevelGrade, O_LEVEL_GRADE>
    {
        public override OLevelGrade TranslateToModel(O_LEVEL_GRADE entity)
        {
            try
            {
                OLevelGrade oLevelGrade = null;
                if (entity != null)
                {
                    oLevelGrade = new OLevelGrade();
                    oLevelGrade.Id = entity.O_Level_Grade_Id;
                    oLevelGrade.Name = entity.O_Level_Grade_Name;
                    oLevelGrade.Description = entity.O_Level_Grade_Description;
                }

                return oLevelGrade;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override O_LEVEL_GRADE TranslateToEntity(OLevelGrade oLevelGrade)
        {
            try
            {
                O_LEVEL_GRADE entity = null;
                if (oLevelGrade != null)
                {
                    entity = new O_LEVEL_GRADE();
                    entity.O_Level_Grade_Id = oLevelGrade.Id;
                    entity.O_Level_Grade_Name = oLevelGrade.Name;
                    entity.O_Level_Grade_Description = oLevelGrade.Description;
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
