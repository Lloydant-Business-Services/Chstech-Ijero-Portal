using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ScratchCardTypeTranslator : TranslatorBase<ScratchCardType, SCRATCH_CARD_TYPE>
    {
        private FeeTypeTranslator feeTypeTranslator;

        public ScratchCardTypeTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
        }

        public override ScratchCardType TranslateToModel(SCRATCH_CARD_TYPE entity)
        {
            try
            {
                ScratchCardType model = null;
                if (entity != null)
                {
                    model = new ScratchCardType();
                    model.Id = entity.Scratch_Card_Type_Id;
                    model.Name = entity.Scratch_Card_Type_Name;
                    model.Description = entity.Scratch_Card_Type_Description;
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SCRATCH_CARD_TYPE TranslateToEntity(ScratchCardType model)
        {
            try
            {
                SCRATCH_CARD_TYPE entity = null;
                if (model != null)
                {
                    entity = new SCRATCH_CARD_TYPE();
                    entity.Scratch_Card_Type_Id = model.Id;
                    entity.Scratch_Card_Type_Name = model.Name;
                    entity.Scratch_Card_Type_Description = model.Description;
                    entity.Fee_Type_Id = model.FeeType.Id;
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
