using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class GenotypeTranslator : TranslatorBase<Genotype, GENOTYPE>
    {
        public override Genotype TranslateToModel(GENOTYPE entity)
        {
            try
            {
                Genotype model = null;
                if (entity != null)
                {
                    model = new Genotype();
                    model.Id = entity.Genotype_Id;
                    model.Name = entity.Genotype_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override GENOTYPE TranslateToEntity(Genotype model)
        {
            try
            {
                GENOTYPE entity = null;
                if (model != null)
                {
                    entity = new GENOTYPE();
                    entity.Genotype_Id = model.Id;
                    entity.Genotype_Name = model.Name;
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
