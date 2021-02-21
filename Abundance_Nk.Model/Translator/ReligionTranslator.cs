using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ReligionTranslator : TranslatorBase<Religion, RELIGION>
    {
        public override Religion TranslateToModel(RELIGION religionEntity)
        {
            try
            {
                Religion religion = null;
                if (religionEntity != null)
                {
                    religion = new Religion();
                    religion.Id = religionEntity.Religion_Id;
                    religion.Name = religionEntity.Religion_Name;
                }

                return religion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override RELIGION TranslateToEntity(Religion religion)
        {
            try
            {
                RELIGION entity = null;
                if (religion != null)
                {
                    entity = new RELIGION();
                    entity.Religion_Id = religion.Id;
                    entity.Religion_Name = religion.Name;
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
