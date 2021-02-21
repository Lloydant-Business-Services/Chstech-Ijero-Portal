
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class FeeTranslator : TranslatorBase<Fee, FEE>
    {
        public override Fee TranslateToModel(FEE entity)
        {
            try
            {
                Fee model = null;
                if (entity != null)
                {
                    model = new Fee();
                    model.Id = entity.Fee_Id;
                    model.Name = entity.Fee_Name;
                    model.Description = entity.Fee_Description;
                    model.Amount = Math.Round(entity.Amount, 2);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override FEE TranslateToEntity(Fee model)
        {
            try
            {
                FEE entity = null;
                if (model != null)
                {
                    entity = new FEE();
                    entity.Fee_Id = model.Id;
                    entity.Fee_Name = model.Name;
                    entity.Fee_Description = model.Description;
                    entity.Amount = model.Amount;
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
