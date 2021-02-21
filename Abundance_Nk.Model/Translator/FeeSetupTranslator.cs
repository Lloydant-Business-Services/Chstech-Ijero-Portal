using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class FeeSetupTranslator : TranslatorBase<FeeSetup, FEE_SETUP>
    {
        private UserTranslator userTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private SessionTranslator sessionTranslator;
        public FeeSetupTranslator()
        {
            userTranslator = new UserTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            sessionTranslator = new SessionTranslator();

        }
        public override FeeSetup TranslateToModel(FEE_SETUP entity)
        {
            try
            {
                FeeSetup model = null;
                if (entity != null)
                {
                    model = new FeeSetup();
                    model.FeeSetupId = entity.FeeSetup_Id;
                    model.FeeSetUpName = entity.FeeSetUp_Name;
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.PaymentMode = paymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Activated = entity.Activated;
                    model.Amount = entity.Amount;
                    model.DateCreated = entity.Date_Created;
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override FEE_SETUP TranslateToEntity(FeeSetup model)
        {
            try
            {
                FEE_SETUP entity = null;
                if (model != null)
                {
                    entity = new FEE_SETUP();
                    entity.FeeSetup_Id = model.FeeSetupId;
                    entity.FeeSetUp_Name = model.FeeSetUpName;
                    entity.FeeType_Id = model.FeeType.Id;
                    entity.PaymentMode_Id = model.PaymentMode.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Amount = model.Amount;
                    entity.Activated = model.Activated;
                    entity.User_Id = model.User.Id;
                    entity.Date_Created = DateTime.Now;

                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
