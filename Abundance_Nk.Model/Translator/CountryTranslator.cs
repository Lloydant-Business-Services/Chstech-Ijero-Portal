using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CountryTranslator : TranslatorBase<Country, COUNTRY>
    {
        public override Country TranslateToModel(COUNTRY entity)
        {
            try
            {
                Country model = null;
                if (entity != null)
                {
                    model = new Country();
                    model.Id = entity.Country_Id;
                    model.CountryName = entity.Country_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COUNTRY TranslateToEntity(Country model)
        {
            try
            {
                COUNTRY entity = null;
                if (model != null)
                {
                    entity = new COUNTRY();
                    entity.Country_Id = model.Id;
                    entity.Country_Name = model.CountryName;
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
