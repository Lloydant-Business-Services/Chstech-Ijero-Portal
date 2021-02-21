using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class FeeSetupFeeTranslator : TranslatorBase<FeeSetupFee, FEESETUP_FEE>
    {
        FeeTranslator feeTranslator;
        FeeSetupTranslator feeSetupTranslator;
        public FeeSetupFeeTranslator()
        {
            feeTranslator = new FeeTranslator();
            feeSetupTranslator = new FeeSetupTranslator();

        }
        public override FeeSetupFee TranslateToModel(FEESETUP_FEE entity)
        {
            try
            {
                FeeSetupFee model = null;
                if (entity != null)
                {
                    model = new FeeSetupFee();
                    model.FeeSetUpFeeId = entity.FeeSetUpFee_Id;
                    model.FeeSetUp = feeSetupTranslator.Translate(entity.FEE_SETUP);
                    model.Activated = entity.Activated;
                    model.Fee = feeTranslator.Translate(entity.FEE);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override FEESETUP_FEE TranslateToEntity(FeeSetupFee model)
        {
            try
            {
                FEESETUP_FEE entity = null;
                if (model != null)
                {
                    entity = new FEESETUP_FEE();
                    entity.FeeSetUpFee_Id = model.FeeSetUpFeeId;
                    entity.FeeSetUp_Id = model.FeeSetUp.FeeSetupId;
                    entity.Fee_Id = model.Fee.Id;
                    entity.Activated = model.Activated;

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
