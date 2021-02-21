using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class MaritalStatusTranslator : TranslatorBase<MaritalStatus, MARITAL_STATUS>
    {
        public override MaritalStatus TranslateToModel(MARITAL_STATUS entity)
        {
            try
            {
                MaritalStatus model = null;
                if (entity != null)
                {
                    model = new MaritalStatus();
                    model.Id = entity.Marital_Status_Id;
                    model.Name = entity.Marital_Status_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MARITAL_STATUS TranslateToEntity(MaritalStatus model)
        {
            try
            {
                MARITAL_STATUS entity = null;
                if (model != null)
                {
                    entity = new MARITAL_STATUS();
                    entity.Marital_Status_Id = model.Id;
                    entity.Marital_Status_Name = model.Name;
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
