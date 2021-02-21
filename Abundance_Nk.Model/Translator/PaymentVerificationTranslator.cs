using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentVerificationTranslator:TranslatorBase<PaymentVerification,PAYMENT_VERIFICATION>
    {
        public PaymentTranslator PaymentTranslator;
        public UserTranslator UserTranslator;

        public PaymentVerificationTranslator()
        {
            PaymentTranslator = new PaymentTranslator();
            UserTranslator = new UserTranslator();
        }
        public override PAYMENT_VERIFICATION TranslateToEntity(PaymentVerification model)
        {
            PAYMENT_VERIFICATION entity = null;
            if (model != null)
            {
                entity = new PAYMENT_VERIFICATION();
                entity.Comments = model.Comment;
                entity.DateVerified = model.DateVerified;
                entity.Payment_Id = model.Payment.Id;
                entity.User_Id = model.User.Id;
                 
            }
            return entity;
        }

        public override PaymentVerification TranslateToModel(PAYMENT_VERIFICATION entity)
        {
            PaymentVerification model = null;
            if (entity != null)
            {
                model = new PaymentVerification();
                model.Comment = entity.Comments;
                model.DateVerified = entity.DateVerified;
                model.Payment = PaymentTranslator.Translate(entity.PAYMENT);
                model.User = UserTranslator.Translate(entity.USER);
            }
            return model;
        }
    }
}
