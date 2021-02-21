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
   public class HostelRoomCornerLogic:BusinessBaseLogic<HostelRoomCorner,HOSTEL_ROOM_CORNER>
    {
       public HostelRoomCornerLogic()
       {
           translator = new HostelRoomCornerTranslator();
       }
       public bool Modify(HostelRoomCorner model)
       {
           try
           { 
               int modified = 0;
               Expression<Func<HOSTEL_ROOM_CORNER, bool>> selector = c => c.Corner_Id == model.Id;
               HOSTEL_ROOM_CORNER cornerEntity = GetEntityBy(selector);
               if (cornerEntity != null)
               {
                   cornerEntity.Corner_Id = model.Id;
                   cornerEntity.Activated = model.Activated;
                   if (model.Name != null)
                   {
                       cornerEntity.Corner_Name = model.Name;
                   }
                   if (model.Room != null)
                   {
                       cornerEntity.Room_Id = model.Room.Id;
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
