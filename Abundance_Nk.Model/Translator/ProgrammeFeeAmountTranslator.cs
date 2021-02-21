using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ProgrammeFeeAmountTranslator : TranslatorBase<ProgrammeFeeAmount, PROGRAMME_FEE_AMOUNT>
    {
        private ProgrammeTranslator programmeTranslator;
        private SessionTranslator sessionTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private LevelTranslator levelTranslator;

        public ProgrammeFeeAmountTranslator()
        {
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            levelTranslator = new LevelTranslator();
        }

        public override ProgrammeFeeAmount TranslateToModel(PROGRAMME_FEE_AMOUNT entity)
        {
            try
            {
                ProgrammeFeeAmount model = null;
                if (entity != null)
                {
                    model = new ProgrammeFeeAmount();
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Amount = entity.Amount;
                    model.Id = entity.Programme_Fee_Amount_Id;
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.PaymentMode = paymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PROGRAMME_FEE_AMOUNT TranslateToEntity(ProgrammeFeeAmount model)
        {
            try
            {
                PROGRAMME_FEE_AMOUNT entity = null;
                if (model != null)
                {
                    entity = new PROGRAMME_FEE_AMOUNT();
                    entity.Programme_Fee_Amount_Id = model.Id;
                    entity.Amount = model.Amount;
                    entity.Level_Id = model.Level.Id;
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                    entity.Programme_Id = model.Programme.Id;
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
