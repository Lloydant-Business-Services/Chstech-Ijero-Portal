using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class FeeSetupAuditTranslator: TranslatorBase<FeeSetupAudit, FEE_SETUP_AUDIT>
    {
        private UserTranslator userTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private SessionTranslator sessionTranslator;
        private FeeSetupTranslator feeSetupTranslator;
        public FeeSetupAuditTranslator()
        {
            userTranslator = new UserTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            sessionTranslator = new SessionTranslator();
            feeSetupTranslator = new FeeSetupTranslator();

        }
        public override FeeSetupAudit TranslateToModel(FEE_SETUP_AUDIT entity)
        {
            try
            {
                FeeSetupAudit model = null;
                if (entity != null)
                {
                    model = new FeeSetupAudit();
                    model.FeeSetupAuditId = entity.Fee_Setup_Audit_Id;
                    model.FeeSetup = feeSetupTranslator.Translate(entity.FEE_SETUP);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.PaymentMode = paymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Client = entity.Client;
                    model.Action = entity.Action;
                    model.Amount = entity.Amount;
                    model.DateCreated = entity.DateCreated;
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override FEE_SETUP_AUDIT TranslateToEntity(FeeSetupAudit model)
        {
            try
            {
                FEE_SETUP_AUDIT entity = null;
                if (model != null)
                {
                    entity = new FEE_SETUP_AUDIT();
                    entity.Fee_Setup_Audit_Id = model.FeeSetupAuditId;
                    entity.Fee_SetUp_Id = model.FeeSetup.FeeSetupId;
                    entity.FeeType_Id = model.FeeType.Id;
                    entity.PaymentMode_Id = model.PaymentMode.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Amount = model.Amount;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.User_Id = model.User.Id;
                    entity.DateCreated = DateTime.Now;

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
