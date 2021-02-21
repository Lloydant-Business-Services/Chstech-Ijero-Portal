using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public  class RemitaPaymentTranslator : TranslatorBase<RemitaPayment, REMITA_PAYMENT>
    {
        private PaymentTranslator paymentTranslator;

        public RemitaPaymentTranslator()
        {
            paymentTranslator = new PaymentTranslator();
        }
        public override RemitaPayment TranslateToModel(REMITA_PAYMENT entity)
        {
            try
            {
                RemitaPayment model = null;
                if (entity != null)
                {
                    model = new RemitaPayment();
                    model.payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.RRR = entity.RRR;
                    model.OrderId = entity.OrderId;
                    model.Status = entity.Status;
                    model.Receipt_No = entity.Receipt_No;

                    model.Description = entity.Description;
                    model.MerchantCode = entity.Merchant_Code;
                    model.TransactionAmount = entity.Transaction_Amount;
                    model.TransactionDate = entity.Transaction_Date;
                    model.ConfirmationNo = entity.Confirmation_No;
                    model.BankCode = entity.Bank_Code;
                    model.CustomerId = entity.Customer_Id;
                    model.BankCode = entity.Bank_Code;
                    model.CustomerName = entity.Customer_Name;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override REMITA_PAYMENT TranslateToEntity(RemitaPayment model)
        {
            try
            {
                REMITA_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new REMITA_PAYMENT();
                    entity.Payment_Id = model.payment.Id;
                    entity.RRR = model.RRR;
                    entity.OrderId = model.OrderId;
                    entity.Status = model.Status;
                    entity.Receipt_No = model.Receipt_No;

                    entity.Description = model.Description;
                    entity.Merchant_Code = model.MerchantCode;
                    entity.Transaction_Amount = model.TransactionAmount;
                    entity.Transaction_Date = model.TransactionDate;
                    entity.Confirmation_No = model.ConfirmationNo;
                    entity.Bank_Code = model.BankCode;
                     entity.Customer_Id = model.CustomerId;
                    entity.Bank_Code = model.BankCode;
                    entity.Customer_Name= model.CustomerName;
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
