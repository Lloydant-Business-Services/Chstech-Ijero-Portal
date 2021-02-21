using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class CardPaymentLogic : BusinessBaseLogic<CardPayment, CARD_PAYMENT>
    {
        public CardPaymentLogic()
        {
            translator = new CardPaymentTranslator();
        }

        public CardPayment GetBy(ScratchCard card, Payment payment)
        {
            try
            {
                Expression<Func<CARD_PAYMENT, bool>> selector = sc => sc.SCRATCH_CARD.Pin == card.Pin && sc.Payment_Id == payment.Id;
                return GetModelBy(selector);
            }
            catch(Exception)
            {
                throw;
            }
        }



    }




}
