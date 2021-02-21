using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class BankTranslator : TranslatorBase<Bank, BANK>
    {
        public override Bank TranslateToModel(BANK entity)
        {
            try
            {
                Bank model = null;
                if (entity != null)
                {
                    model = new Bank();
                    model.Code = entity.Bank_Code;
                    model.Name = entity.Bank_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override BANK TranslateToEntity(Bank model)
        {
            try
            {
                BANK entity = null;
                if (model != null)
                {
                    entity = new BANK();
                    entity.Bank_Code = model.Code;
                    entity.Bank_Name = model.Name;
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
