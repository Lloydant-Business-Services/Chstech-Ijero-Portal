using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class ModeOfFinanceTranslator : TranslatorBase<ModeOfFinance, MODE_OF_FINANCE>
    {
        public override ModeOfFinance TranslateToModel(MODE_OF_FINANCE entity)
        {
            try
            {
                ModeOfFinance model = null;
                if (entity != null)
                {
                    model = new ModeOfFinance();
                    model.Id = entity.Mode_Of_Finance_Id;
                    model.Name = entity.Mode_Of_Finance_Name;
                    model.Description = entity.Mode_Of_Finance_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MODE_OF_FINANCE TranslateToEntity(ModeOfFinance model)
        {
            try
            {
                MODE_OF_FINANCE entity = null;
                if (model != null)
                {
                    entity = new MODE_OF_FINANCE();
                    entity.Mode_Of_Finance_Id = model.Id;
                    entity.Mode_Of_Finance_Name = model.Name;
                    entity.Mode_Of_Finance_Description = model.Description;
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
