using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ScratchCardTranslator : TranslatorBase<ScratchCard, SCRATCH_CARD>
    {
        private ScratchCardBatchTranslator scratchCardBatchTranslator;
        private PersonTranslator personTranslator;
        public ScratchCardTranslator()
        {
            scratchCardBatchTranslator = new ScratchCardBatchTranslator();
            personTranslator = new PersonTranslator();
        }

        public override ScratchCard TranslateToModel(SCRATCH_CARD entity)
        {
            try
            {
                ScratchCard model = null;
                if (entity != null)
                {
                    model = new ScratchCard();
                    model.Id = entity.Scratch_Card_Id;
                    model.Batch = scratchCardBatchTranslator.Translate(entity.SCRATCH_CARD_BATCH);
                    model.SerialNumber = entity.Serial_Number;
                    model.Pin = entity.Pin;
                    model.UsageCount = entity.Usage_Count;
                    model.FirstUsedDate = entity.First_Used_Date.GetValueOrDefault();
                    model.person = personTranslator.Translate(entity.PERSON);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SCRATCH_CARD TranslateToEntity(ScratchCard model)
        {
            try
            {
                SCRATCH_CARD entity = null;
                if (model != null)
                {
                    entity = new SCRATCH_CARD();
                    entity.Scratch_Card_Id = model.Id;
                    entity.Scratch_Card_Batch_Id = model.Batch.Id;
                    entity.Serial_Number = model.SerialNumber;
                    entity.Pin = model.Pin;
                    entity.Usage_Count = model.UsageCount;
                    entity.First_Used_Date = entity.First_Used_Date;
                    entity.Person_Id = model.person.Id; 
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
