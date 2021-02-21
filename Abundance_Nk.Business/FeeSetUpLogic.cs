using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class FeeSetUpLogic : BusinessBaseLogic<FeeSetup, FEE_SETUP>
    {
        public FeeSetUpLogic()
        {
            translator = new FeeSetupTranslator();
        }
        public List<FeeSetup> GetFeeSetup(Session session, FeeType feeType)
        {
            try
            {
                List<FeeSetup> feeSetup = GetModelsBy(x => x.FeeType_Id == feeType.Id && x.Session_Id == session.Id);
                return feeSetup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Modify(FeeSetup feeSetup)
        {
            try
            {
                int modified = 0;
                Expression<Func<FEE_SETUP, bool>> selector = c => c.FeeSetup_Id == feeSetup.FeeSetupId;
                FEE_SETUP entity = GetEntityBy(selector);
                if (entity != null)
                {
                    entity.FeeSetup_Id = feeSetup.FeeSetupId;
                    entity.Amount = feeSetup.Amount;
                    entity.Activated = feeSetup.Activated;
                    entity.FeeSetUp_Name = feeSetup.FeeSetUpName;
                    entity.FeeType_Id = feeSetup.FeeType.Id;
                    entity.PaymentMode_Id = feeSetup.PaymentMode.Id;
                    entity.Session_Id = feeSetup.Session.Id;

                    modified = Save();

                    return true;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}
