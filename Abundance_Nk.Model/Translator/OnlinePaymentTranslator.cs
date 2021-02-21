using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OnlinePaymentTranslator : TranslatorBase<OnlinePayment, ONLINE_PAYMENT>
    {
        private PaymentTranslator paymentTranslator;
        private PaymentChannelTranslator paymentChannelTranslator;

        public OnlinePaymentTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            paymentChannelTranslator = new PaymentChannelTranslator();
        }

        public override OnlinePayment TranslateToModel(ONLINE_PAYMENT entity)
        {
            try
            {
                OnlinePayment model = null;
                if (entity != null)
                {
                    model = new OnlinePayment();
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Channel = paymentChannelTranslator.Translate(entity.PAYMENT_CHANNEL);
                    model.TransactionNumber = entity.Transaction_Number;
                    model.TransactionDate = entity.Transaction_Date;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ONLINE_PAYMENT TranslateToEntity(OnlinePayment model)
        {
            try
            {
                ONLINE_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new ONLINE_PAYMENT();
                    entity.Payment_Id = model.Payment.Id;
                    entity.Payment_Channnel_Id = model.Channel.Id;
                    entity.Transaction_Number = model.TransactionNumber;
                    entity.Transaction_Date = model.TransactionDate;
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
