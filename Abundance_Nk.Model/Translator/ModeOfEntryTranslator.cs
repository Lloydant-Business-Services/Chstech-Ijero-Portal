using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ModeOfEntryTranslator : TranslatorBase<ModeOfEntry, MODE_OF_ENTRY>
    {
        public override ModeOfEntry TranslateToModel(MODE_OF_ENTRY entity)
        {
            try
            {
                ModeOfEntry model = null;
                if (entity != null)
                {
                    model = new ModeOfEntry();
                    model.Id = entity.Mode_Of_Entry_Id;
                    model.Name = entity.Mode_Of_Entry_Name;
                    model.Description = entity.Mode_Of_Entry_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MODE_OF_ENTRY TranslateToEntity(ModeOfEntry model)
        {
            try
            {
                MODE_OF_ENTRY entity = null;
                if (model != null)
                {
                    entity = new MODE_OF_ENTRY();
                    entity.Mode_Of_Entry_Id = model.Id;
                    entity.Mode_Of_Entry_Name = model.Name;
                    entity.Mode_Of_Entry_Description = model.Description;
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
