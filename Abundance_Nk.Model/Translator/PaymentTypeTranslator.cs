using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentTypeTranslator : TranslatorBase<PaymentType, PAYMENT_TYPE>
    {
        public override PaymentType TranslateToModel(PAYMENT_TYPE entity)
        {
            try
            {
                PaymentType paymentType = null;
                if (entity != null)
                {
                    paymentType = new PaymentType();
                    paymentType.Id = entity.Payment_Type_Id;
                    paymentType.Name = entity.Payment_Type_Name;
                    paymentType.Description = entity.Payment_Type_Description;
                }

                return paymentType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_TYPE TranslateToEntity(PaymentType paymentType)
        {
            try
            {
                PAYMENT_TYPE entity = null;
                if (paymentType != null)
                {
                    entity = new PAYMENT_TYPE();
                    entity.Payment_Type_Id = paymentType.Id;
                    entity.Payment_Type_Name = paymentType.Name;
                    entity.Payment_Type_Description = paymentType.Description;
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
