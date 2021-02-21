using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentModeTranslator : TranslatorBase<PaymentMode, PAYMENT_MODE>
    {
        public override PaymentMode TranslateToModel(PAYMENT_MODE paymentModeEntity)
        {
            try
            {
                PaymentMode paymentMode = null;
                if (paymentModeEntity != null)
                {
                    paymentMode = new PaymentMode();
                    paymentMode.Id = paymentModeEntity.Payment_Mode_Id;
                    paymentMode.Name = paymentModeEntity.Payment_Mode_Name;
                    paymentMode.Description = paymentModeEntity.Payment_Mode_Description;
                }

                return paymentMode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_MODE TranslateToEntity(PaymentMode paymentMode)
        {
            try
            {
                PAYMENT_MODE paymentModeEntity = null;
                if (paymentMode != null)
                {
                    paymentModeEntity = new PAYMENT_MODE();
                    paymentModeEntity.Payment_Mode_Name = paymentMode.Name;
                    paymentModeEntity.Payment_Mode_Description = paymentMode.Description;
                }

                return paymentModeEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
