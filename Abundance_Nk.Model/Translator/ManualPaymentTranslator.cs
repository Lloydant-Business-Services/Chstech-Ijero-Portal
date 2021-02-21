using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ManualPaymentTranslator : TranslatorBase<ManualPayment, MANUAL_PAYMENT>
    {
        private readonly FeeTypeTranslator feeTypeTranslator;
        private readonly PaymentModeTranslator paymentModeTranslator;
        private readonly PaymentTypeTranslator paymentTypeTranslator;
        private readonly PersonTranslator personTranslator;
        private readonly PersonTypeTranslator personTypeTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly UserTranslator userTranslator;

        public ManualPaymentTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            personTypeTranslator = new PersonTypeTranslator();
            paymentTypeTranslator = new PaymentTypeTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
            userTranslator = new UserTranslator();

        }

        public override ManualPayment TranslateToModel(MANUAL_PAYMENT entity)
        {
            try
            {
                ManualPayment model = null;
                if (entity != null)
                {
                    model = new ManualPayment();
                    model.ManualPayment_Id = entity.Manual_Payment_Id;
                    model.InvoiceNumber = entity.Invoice_Number;
                    model.Amount = entity.Amount;
                    model.DateApproved = entity.Date_Approved;
                    model.FeeType = feeTypeTranslator.TranslateToModel(entity.FEE_TYPE);
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MANUAL_PAYMENT TranslateToEntity(ManualPayment model)
        {
            try
            {
                MANUAL_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new MANUAL_PAYMENT();
                    entity.Manual_Payment_Id = model.ManualPayment_Id;
                    entity.Amount = model.Amount;
                    entity.Approver_Officer_Id = model.User.Id;
                    entity.Date_Approved = DateTime.Now; ;
                    entity.FeeType_Id = model.FeeType.Id;
                    entity.Invoice_Number = model.InvoiceNumber;
                    entity.Person_Id = model.Person.Id;
                    entity.Session_Id = model.Session.Id;

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
