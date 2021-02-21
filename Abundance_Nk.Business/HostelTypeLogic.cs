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
   public class HostelTypeLogic:BusinessBaseLogic<HostelType,HOSTEL_TYPE>
    {
       public HostelTypeLogic()
       {
           translator = new HostelTypeTranslator();
       }
       public bool Modify(HostelType model)
       {
           try
           {
               int modified = 0;
               Expression<Func<HOSTEL_TYPE, bool>> selector = c => c.Hostel_Type_Id == model.Hostel_Type_Id;
               HOSTEL_TYPE entity = GetEntityBy(selector);
               if (entity != null)
               {
                   entity.Hostel_Type_Id = model.Hostel_Type_Id;
                   entity.Hostel_Type_Name = model.Hostel_Type_Name;
                   entity.Hostel_Type_Description = model.Hostel_Type_Description;

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
