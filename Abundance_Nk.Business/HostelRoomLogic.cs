using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class HostelRoomLogic:BusinessBaseLogic<HostelRoom,HOSTEL_ROOM>
    {
        public HostelRoomLogic()
        {
            translator = new HostelRoomTranslator();
        }
        public bool Modify(HostelRoom model)
        {
            try
            { 
                int modified = 0;
                Expression<Func<HOSTEL_ROOM, bool>> selector = c => c.Room_Id == model.Id;
                HOSTEL_ROOM roomEntity = GetEntityBy(selector);
                if (roomEntity != null)
                {
                    roomEntity.Room_Id = model.Id;
                    roomEntity.Activated = model.Activated;
                    roomEntity.Reserved = model.Reserved;
                    roomEntity.Room_Number = model.Number;
                    if (model.Series != null)
                    {
                        roomEntity.Series_Id = model.Series.Id;
                    }
                    if (model.Hostel != null)
                    {
                        roomEntity.Hostel_Id = model.Hostel.Id;
                    }

                    modified = Save();

                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
    }
}
