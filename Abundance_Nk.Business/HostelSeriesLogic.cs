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
    public class HostelSeriesLogic: BusinessBaseLogic<HostelSeries,HOSTEL_SERIES>
    {
         public HostelSeriesLogic()
         {
             translator = new HostelSeriesTranslator();
         }
         public bool Modify(HostelSeries model)
         {
             try
             {
                 int modified = 0;
                 Expression<Func<HOSTEL_SERIES, bool>> selector = c => c.Series_Id == model.Id;
                 HOSTEL_SERIES entity = GetEntityBy(selector);
                 if (entity != null)
                 {
                     entity.Series_Name = model.Name;
                     if (model.Hostel != null && model.Hostel.Id > 0)
                     {
                         entity.Hostel_Id = model.Hostel.Id;
                     }

                     entity.Activated = model.Activated;

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
