using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class DisplayMessageTranslator : TranslatorBase<DisplayMessage, DISPLAY_MESSAGE>
    {
        public override DisplayMessage TranslateToModel(DISPLAY_MESSAGE entity)
        {

            try
            {
                DisplayMessage model = null;
                if (entity != null)
                {
                    model = new DisplayMessage();
                    model.Id = entity.Id;
                    model.Message = entity.Message;
                    model.Activated = entity.Activated;

                }
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override DISPLAY_MESSAGE TranslateToEntity(DisplayMessage model)
        {
            try
            {
                DISPLAY_MESSAGE entity = null;
                if (model != null)
                {
                    entity = new DISPLAY_MESSAGE();
                    entity.Id = model.Id;
                    entity.Message = model.Message;
                    entity.Activated = model.Activated;
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
