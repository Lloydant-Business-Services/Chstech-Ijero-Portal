
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class DisplayMessageLogic : BusinessBaseLogic<DisplayMessage, DISPLAY_MESSAGE>
    {
        public DisplayMessageLogic()
        {
            translator = new DisplayMessageTranslator();

        }

        public bool Modify(DisplayMessage displayMessage)
        {
            try
            {
                DISPLAY_MESSAGE entity = GetEntityBy(ds => ds.Id == displayMessage.Id);
                entity.Id = displayMessage.Id;
                if (displayMessage.Message != null)
                {
                    entity.Message = displayMessage.Message;
                }
               
                entity.Activated = displayMessage.Activated;
                
                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
