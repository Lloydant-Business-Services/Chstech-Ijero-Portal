using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class EWalletPaymentTranslator : TranslatorBase<EWalletPayment, E_WALLET_PAYMENT>
    {
        private PaymentTranslator paymentTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private SessionTranslator sessionTranslator;
        private StudentTranslator studentTranslator;
        private PersonTranslator personTranslator;
        public EWalletPaymentTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            sessionTranslator = new SessionTranslator();
            studentTranslator = new StudentTranslator();
            personTranslator = new PersonTranslator();
        }
        public override EWalletPayment TranslateToModel(E_WALLET_PAYMENT entity)
        {
            try
            {
                EWalletPayment model = null;
                if (entity != null)
                {
                    model = new EWalletPayment();
                    model.Amount = entity.Amount;
                    model.Id = entity.Id;
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Person = personTranslator.Translate(entity.PERSON);
                }
                return model;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public override E_WALLET_PAYMENT TranslateToEntity(EWalletPayment model)
        {
            try
            {
                E_WALLET_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new E_WALLET_PAYMENT();
                    entity.Id = model.Id;
                    entity.Amount = model.Amount;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Session_Id = model.Session.Id;
                    if (model.Person != null)
                    {
                        entity.Person_Id = model.Person.Id;
                    }
                    if (model.Student != null)
                    {
                        entity.Student_Id = model.Student.Id;
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
