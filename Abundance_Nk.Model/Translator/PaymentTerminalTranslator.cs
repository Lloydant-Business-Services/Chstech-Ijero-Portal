using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class PaymentTerminalTranslator : TranslatorBase<PaymentTerminal, PAYMENT_TERMINAL>
    {
        private FeeTypeTranslator feeTypeTranslator;
        private SessionTranslator sessionTranslator;

        public PaymentTerminalTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override PaymentTerminal TranslateToModel(PAYMENT_TERMINAL entity)
        {
            try
            {
                PaymentTerminal model = null;
                if (entity != null)
                {
                    model = new PaymentTerminal();
                    model.Id = entity.Payment_Terminal_Id;
                    model.TerminalId = entity.Terminal_Id;
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_TERMINAL TranslateToEntity(PaymentTerminal model)
        {
            try
            {
                PAYMENT_TERMINAL entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_TERMINAL();
                    entity.Payment_Terminal_Id = model.Id;
                    entity.Terminal_Id = model.TerminalId;
                    entity.Fee_Type_Id = model.FeeType.Id;
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
