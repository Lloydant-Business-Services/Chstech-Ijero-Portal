using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class RemitaSplitItemsTranslator : TranslatorBase<RemitaSplitItems, REMITA_SPLIT_DETAILS>
    {

        public override RemitaSplitItems TranslateToModel(REMITA_SPLIT_DETAILS entity)
        {
            try
            {
                RemitaSplitItems model = null;
                if (entity != null)
                {
                    model = new RemitaSplitItems();
                    model.lineItemsId = entity.Id.ToString();
                    model.bankCode = entity.Bank_Code;
                    model.beneficiaryAccount = entity.Beneficiary_Account;
                    model.beneficiaryAmount = entity.Beneficiary_Amount.ToString();
                    model.beneficiaryName = entity.Beneficiary_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override REMITA_SPLIT_DETAILS TranslateToEntity(RemitaSplitItems model)
        {
            try
            {
                REMITA_SPLIT_DETAILS entity = null;
                if (model != null)
                {
                    entity = new REMITA_SPLIT_DETAILS();
                    entity.Id = Convert.ToInt32(model.lineItemsId);
                    entity.Bank_Code = model.bankCode;
                    entity.Beneficiary_Account = model.beneficiaryAccount;
                    entity.Beneficiary_Amount = Convert.ToDecimal(model.beneficiaryAmount);
                    entity.Beneficiary_Name = model.beneficiaryName;
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
