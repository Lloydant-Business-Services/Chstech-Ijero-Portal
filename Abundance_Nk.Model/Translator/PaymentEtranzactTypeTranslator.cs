using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentEtranzactTypeTranslator : TranslatorBase<PaymentEtranzactType, PAYMENT_ETRANZACT_TYPE>
    {
        private FeeTypeTranslator feeTypeTranslator;
        private LevelTranslator levelTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private ProgrammeTranslator programmeTranslator;
        private SessionTranslator sessionTranslator;
        public PaymentEtranzactTypeTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            levelTranslator = new LevelTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override PaymentEtranzactType TranslateToModel(PAYMENT_ETRANZACT_TYPE entity)
        {
            try
            {
                PaymentEtranzactType model = null;
                if (entity != null)
                {
                    model = new PaymentEtranzactType();
                    model.Id = entity.Payment_Etranzact_Type_Id;
                    model.Name = entity.Payment_Etranzact_Type_Name;
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.PaymentMode = paymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_ETRANZACT_TYPE TranslateToEntity(PaymentEtranzactType model)
        {
            try
            {
                PAYMENT_ETRANZACT_TYPE entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_ETRANZACT_TYPE();
                    entity.Payment_Etranzact_Type_Id = model.Id;
                    entity.Payment_Etranzact_Type_Name = model.Name;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                    entity.Programme_Id = model.Programme.Id;
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
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
