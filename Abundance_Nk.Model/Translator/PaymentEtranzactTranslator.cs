using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentEtranzactTranslator: TranslatorBase<PaymentEtranzact, PAYMENT_ETRANZACT>
    {
        private OnlinePaymentTranslator onlinePaymentTranslator;
        private PaymentTerminalTranslator paymentTerminalTranslator;
        private PaymentEtranzactTypeTranslator paymentEtranzactTypeTranslator;

        public PaymentEtranzactTranslator()
        {
            onlinePaymentTranslator = new OnlinePaymentTranslator();
            paymentTerminalTranslator = new PaymentTerminalTranslator();
            paymentEtranzactTypeTranslator = new PaymentEtranzactTypeTranslator();
        }

        public override PaymentEtranzact TranslateToModel(PAYMENT_ETRANZACT entity)
        {
            try
            {
                PaymentEtranzact model = null;
                if (entity != null)
                {
                    model = new PaymentEtranzact();
                    model.Payment = onlinePaymentTranslator.Translate(entity.ONLINE_PAYMENT);
                    model.Terminal = paymentTerminalTranslator.Translate(entity.PAYMENT_TERMINAL);
                    model.EtranzactType = paymentEtranzactTypeTranslator.Translate(entity.PAYMENT_ETRANZACT_TYPE);
                    model.BankCode = entity.Bank_Code;
                    model.BranchCode = entity.Branch_Code;
                    model.ConfirmationNo = entity.Confirmation_No;
                    model.CustomerAddress = entity.Customer_Address;
                    model.CustomerID = entity.Customer_Id;
                    model.CustomerName = entity.Customer_Name;
                    model.MerchantCode = entity.Merchant_Code;
                    model.PaymentCode = entity.Payment_Code;
                    model.ReceiptNo = entity.Receipt_No;
                    model.TransactionAmount = entity.Transaction_Amount;
                    model.TransactionDate = entity.Transaction_Date;
                    model.TransactionDescription = entity.Transaction_Description;
                    model.Used = Convert.ToBoolean(entity.Used);

                    //model.UsedBy = Convert.ToInt64(entity.USED_BY_PERSON);
                    //model.SessionId = Convert.ToInt16(entity.SESSION_ID);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_ETRANZACT TranslateToEntity(PaymentEtranzact model)
        {
            try
            {
                PAYMENT_ETRANZACT entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_ETRANZACT();
                    entity.Payment_Id = model.Payment.Payment.Id;
                    entity.Payment_Terminal_Id = model.Terminal.Id;
                    entity.Payment_Etranzact_Type_Id = model.EtranzactType.Id;
                    entity.Bank_Code = model.BankCode;
                    entity.Branch_Code = model.BranchCode;
                    entity.Confirmation_No = model.ConfirmationNo;
                    entity.Customer_Address = model.CustomerAddress;
                    entity.Customer_Id = model.CustomerID;
                    entity.Customer_Name = model.CustomerName;
                    entity.Merchant_Code = model.MerchantCode;
                    entity.Payment_Code = model.PaymentCode;
                    entity.Receipt_No = model.ReceiptNo;
                    entity.Transaction_Amount = model.TransactionAmount;
                    entity.Transaction_Date = model.TransactionDate;
                    entity.Transaction_Description = model.TransactionDescription;
                    entity.Used = model.Used;
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
