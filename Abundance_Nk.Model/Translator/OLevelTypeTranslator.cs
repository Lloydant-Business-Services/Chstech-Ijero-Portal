using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelTypeTranslator : TranslatorBase<OLevelType, O_LEVEL_TYPE>
    {
        public override OLevelType TranslateToModel(O_LEVEL_TYPE entity)
        {
            try
            {
                OLevelType oLevelType = null;
                if (entity != null)
                {
                    oLevelType = new OLevelType();
                    oLevelType.Id = entity.O_Level_Type_Id;
                    oLevelType.Name = entity.O_Level_Type_Name;
                    oLevelType.ShortName = entity.O_Level_Type_Short_Name;
                    oLevelType.Description = entity.O_Level_Type_Description;
                }

                return oLevelType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override O_LEVEL_TYPE TranslateToEntity(OLevelType oLevelType)
        {
            try
            {
                O_LEVEL_TYPE entity = null;
                if (oLevelType != null)
                {
                    entity = new O_LEVEL_TYPE();
                    entity.O_Level_Type_Id = oLevelType.Id;
                    entity.O_Level_Type_Name = oLevelType.Name;
                    entity.O_Level_Type_Short_Name = oLevelType.ShortName;
                    entity.O_Level_Type_Description = oLevelType.Description;
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
