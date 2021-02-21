using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class BankBranchTranslator : TranslatorBase<BankBranch, BANK_BRANCH>
    {
        public override BankBranch TranslateToModel(BANK_BRANCH entity)
        {
            try
            {
                BankBranch model = null;
                if (entity != null)
                {
                    model = new BankBranch();
                    model.Code = entity.Branch_Code;
                    model.Name = entity.Branch_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override BANK_BRANCH TranslateToEntity(BankBranch model)
        {
            try
            {
                BANK_BRANCH entity = null;
                if (model != null)
                {
                    entity = new BANK_BRANCH();
                    entity.Branch_Code = model.Code;
                    entity.Branch_Name = model.Name;
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
