using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ITDurationTranslator : TranslatorBase<ITDuration, IT_DURATION>
    {
        public override ITDuration TranslateToModel(IT_DURATION entity)
        {
            try
            {
                ITDuration model = null;
                if (entity != null)
                {
                    model = new ITDuration();
                    model.Id = entity.IT_Duration_Id;
                    model.Name = entity.IT_Duration_Name;
                    model.Description = entity.IT_Duration_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override IT_DURATION TranslateToEntity(ITDuration model)
        {
            try
            {
                IT_DURATION entity = null;
                if (model != null)
                {
                    entity = new IT_DURATION();
                    entity.IT_Duration_Id = model.Id;
                    entity.IT_Duration_Name = model.Name;
                    entity.IT_Duration_Description = model.Description;
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
