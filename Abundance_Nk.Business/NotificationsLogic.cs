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
    public class NotificationsLogic : BusinessBaseLogic<Notifications, NOTIFICATIONS>
    {
        public NotificationsLogic()
        {
            translator = new NotificationsTranslator();
        }

        public bool Modify(Notifications Notifications)
        {
            try
            {
                Expression<Func<NOTIFICATIONS, bool>> selector = n => n.Id == Notifications.Id;
                NOTIFICATIONS entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Message = Notifications.Message;
                entity.Active = Notifications.Active;
                entity.IsDelete = Notifications.IsDelete;

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
