using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class TertiaryInstitutionTranslator : TranslatorBase<TertiaryInstitution, TERTIARY_INSTITUTION>
    {
        public override TertiaryInstitution TranslateToModel(TERTIARY_INSTITUTION entity)
        {
            try
            {
                TertiaryInstitution model = null;
                if (entity != null)
                {
                    model = new TertiaryInstitution();
                    model.Id = entity.Tertiary_Institution_Id;
                    model.Activated = entity.Activated;
                    model.Description = entity.Description;
                    model.Name = entity.Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TERTIARY_INSTITUTION TranslateToEntity(TertiaryInstitution model)
        {
            try
            {
                TERTIARY_INSTITUTION entity = null;
                if (model != null)
                {
                    entity = new TERTIARY_INSTITUTION();
                    entity.Description = model.Description;
                    entity.Activated = model.Activated;
                    entity.Name = model.Name;
                    entity.Tertiary_Institution_Id = model.Id;
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
