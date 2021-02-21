using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentTranslator : TranslatorBase<Payment, PAYMENT>
    {
        private PaymentModeTranslator paymentModeTranslator;
        private PersonTypeTranslator personTypeTranslator;
        private PaymentTypeTranslator paymentTypeTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private PersonTranslator personTranslator;
        private SessionTranslator sessionTranslator;

        public PaymentTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            personTypeTranslator = new PersonTypeTranslator();
            paymentTypeTranslator = new PaymentTypeTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override Payment TranslateToModel(PAYMENT paymentEntity)
        {
            try
            {
                Payment payment = null;
                if (paymentEntity != null)
                {
                    payment = new Payment();
                    payment.Id = paymentEntity.Payment_Id;
                    payment.PaymentMode = paymentModeTranslator.TranslateToModel(paymentEntity.PAYMENT_MODE);
                    payment.PaymentType = paymentTypeTranslator.TranslateToModel(paymentEntity.PAYMENT_TYPE);
                    payment.PersonType = personTypeTranslator.TranslateToModel(paymentEntity.PERSON_TYPE);
                    payment.FeeType = feeTypeTranslator.TranslateToModel(paymentEntity.FEE_TYPE);
                    payment.DatePaid = paymentEntity.Date_Paid;
                    payment.Person = personTranslator.Translate(paymentEntity.PERSON);
                    payment.SerialNumber = paymentEntity.Payment_Serial_Number;
                    payment.InvoiceNumber = paymentEntity.Invoice_Number;
                    payment.Session = sessionTranslator.Translate(paymentEntity.SESSION);
                }

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT TranslateToEntity(Payment payment)
        {
            try
            {
                PAYMENT paymentEntity = null;
                if (payment != null)
                {
                    paymentEntity = new PAYMENT();
                    paymentEntity.Payment_Id = payment.Id;
                    paymentEntity.Payment_Mode_Id = payment.PaymentMode.Id;
                    paymentEntity.Payment_Type_Id = payment.PaymentType.Id;
                    paymentEntity.Person_Type_Id = payment.PersonType.Id;
                    paymentEntity.Fee_Type_Id = payment.FeeType.Id;
                    paymentEntity.Date_Paid = payment.DatePaid;
                    paymentEntity.Person_Id = payment.Person.Id;
                    paymentEntity.Payment_Serial_Number = payment.SerialNumber;
                    paymentEntity.Invoice_Number = payment.InvoiceNumber;
                    paymentEntity.Session_Id = payment.Session.Id;

                }

                return paymentEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
