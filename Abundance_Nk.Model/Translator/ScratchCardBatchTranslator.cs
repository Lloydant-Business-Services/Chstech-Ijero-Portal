using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ScratchCardBatchTranslator : TranslatorBase<ScratchCardBatch, SCRATCH_CARD_BATCH>
    {
        private UserTranslator userTranslator;
        private ScratchCardTypeTranslator scratchCardTypeTranslator;

        public ScratchCardBatchTranslator()
        {
            userTranslator = new UserTranslator();
            scratchCardTypeTranslator = new ScratchCardTypeTranslator();
        }

        public override ScratchCardBatch TranslateToModel(SCRATCH_CARD_BATCH entity)
        {
            try
            {
                ScratchCardBatch model = null;
                if (entity != null)
                {
                    model = new ScratchCardBatch();
                    model.Id = entity.Scratch_Card_Batch_Id;
                    model.CardType = scratchCardTypeTranslator.Translate(entity.SCRATCH_CARD_TYPE);
                    model.EnteredBy = userTranslator.Translate(entity.USER);
                    model.DateGenerated = entity.Date_Generated;
                    model.Quantity = entity.Quantity;
                    model.ExpiryDate = entity.Expiry_Date;
                    model.UsageCountLimit = entity.Usage_Count_Limit;
                    model.Price = entity.Price;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SCRATCH_CARD_BATCH TranslateToEntity(ScratchCardBatch model)
        {
            try
            {
                SCRATCH_CARD_BATCH entity = null;
                if (model != null)
                {
                    entity = new SCRATCH_CARD_BATCH();
                    entity.Scratch_Card_Batch_Id = model.Id;
                    entity.Scratch_Card_Type_Id = model.CardType.Id;
                    entity.User_Id = model.EnteredBy.Id;
                    entity.Date_Generated = model.DateGenerated;
                    entity.Quantity = model.Quantity;
                    entity.Expiry_Date = model.ExpiryDate;
                    entity.Usage_Count_Limit = model.UsageCountLimit;
                    entity.Price = model.Price;
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
