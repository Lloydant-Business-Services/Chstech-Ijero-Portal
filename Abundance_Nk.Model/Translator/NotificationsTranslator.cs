using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class NotificationsTranslator : TranslatorBase<Notifications, NOTIFICATIONS>
    {       

        public override Notifications TranslateToModel(NOTIFICATIONS entity)
        {
            try
            {
                Notifications model = null;
                if (entity != null)
                {
                    model = new Notifications();
                    model.Id = entity.Id;
                    model.Message = entity.Message;
                    model.Active = entity.Active;
                    model.IsDelete = entity.IsDelete;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override NOTIFICATIONS TranslateToEntity(Notifications model)
        {
            try
            {
                NOTIFICATIONS entity = null;
                if (model != null)
                {
                    entity = new NOTIFICATIONS();
                    entity.Id = model.Id;
                    entity.Message = model.Message;
                    entity.Active = model.Active;
                    entity.IsDelete = model.IsDelete;
                  

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
