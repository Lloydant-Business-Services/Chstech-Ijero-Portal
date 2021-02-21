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
   public class HostelAllocationCriteriaLogic:BusinessBaseLogic<HostelAllocationCriteria, HOSTEL_ALLOCATION_CRITERIA>
    {
       public HostelAllocationCriteriaLogic()
       {
           translator = new HostelAllocationCriteriaTranslator();
       }
       public bool Modify(HostelAllocationCriteria hostelAllocationCriteria)
       {
           try
           {
               HOSTEL_ALLOCATION_CRITERIA entity = GetEntityBy(x => x.Id == hostelAllocationCriteria.Id);
               if (entity != null)
               {
                   entity.Id = hostelAllocationCriteria.Id;
                   if (hostelAllocationCriteria.Corner != null)
                   {
                       entity.Corner_Id = hostelAllocationCriteria.Corner.Id;
                   }
                   
                   if (hostelAllocationCriteria.Hostel != null)
                   {
                       entity.Hostel_Id = hostelAllocationCriteria.Hostel.Id;
                   }
                   if (hostelAllocationCriteria.Level != null)
                   {
                       entity.Level_Id = hostelAllocationCriteria.Level.Id;
                   }
                   
                   if (hostelAllocationCriteria.Room != null)
                   {
                       entity.Room_Id = hostelAllocationCriteria.Room.Id;
                   }
                   if (hostelAllocationCriteria.Series != null)
                   {
                       entity.Series_Id = hostelAllocationCriteria.Series.Id;
                   }
                   Save();
                   return true;
               }
               return false;
           }
           catch (Exception ex)
           {

               throw ex;
           }
       }
    }
}
