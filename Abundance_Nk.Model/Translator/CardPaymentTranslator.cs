using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class CardPaymentTranslator : TranslatorBase<CardPayment, CARD_PAYMENT>
    {
        private PaymentTranslator paymentTranslator;
        private ScratchCardTranslator scratchCardTranslator;

        public CardPaymentTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            scratchCardTranslator = new ScratchCardTranslator();
        }

        public override CardPayment TranslateToModel(CARD_PAYMENT entity)
        {
            try
            {
                CardPayment model = null;
                if (entity != null)
                {
                    model = new CardPayment();
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Card = scratchCardTranslator.Translate(entity.SCRATCH_CARD);
                    model.TransactionDate = entity.Transaction_Date;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override CARD_PAYMENT TranslateToEntity(CardPayment model)
        {
            try
            {
                CARD_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new CARD_PAYMENT();
                    entity.Payment_Id = model.Payment.Id;
                    entity.Scratch_Card_Id = model.Card.Id;
                    model.TransactionDate = entity.Transaction_Date;
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
