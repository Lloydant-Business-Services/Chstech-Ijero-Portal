using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ShortFallTranslator:TranslatorBase<ShortFall,SHORT_FALL>
    {
        private PaymentTranslator paymentTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private UserTranslator userTranslator;
        public ShortFallTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            userTranslator = new UserTranslator();
        }
        public override ShortFall TranslateToModel(SHORT_FALL entity)
        {
            try
            {
                ShortFall model = null;
                if (entity != null)
                {
                    model = new ShortFall();
                    model.Amount = entity.Amount;
                    model.Id = entity.Short_Fall_Id;
                    model.FeeReference = entity.Fee_Reference;
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.User = userTranslator.Translate(entity.USER);
                }
                return model;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public override SHORT_FALL TranslateToEntity(ShortFall model)
        {
            try
            {
                SHORT_FALL entity = null;
                if (model != null)
                {
                    entity = new SHORT_FALL();
                    entity.Short_Fall_Id = model.Id;
                    entity.Amount = model.Amount;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Fee_Reference = model.FeeReference;
                    if (model.FeeType != null)
                    {
                        entity.Fee_Type_Id = model.FeeType.Id;
                    }
                    if (model.User != null)
                    {
                        entity.User_Id = model.User.Id;
                    }
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
