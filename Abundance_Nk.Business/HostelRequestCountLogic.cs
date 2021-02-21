using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class HostelRequestCountLogic : BusinessBaseLogic<HostelRequestCount, HOSTEL_REQUEST_COUNT>
    {
        public HostelRequestCountLogic()
        {
            translator = new HostelRequestCountTranslator();
        }
        public bool Modify(HostelRequestCount model)
        {
            try
            {
                Expression<Func<HOSTEL_REQUEST_COUNT, bool>> selector = p => p.Hostel_Request_Count_Id == model.Id;
                HOSTEL_REQUEST_COUNT entity = GetEntityBy(selector);

                if (entity == null || entity.Hostel_Request_Count_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }


                entity.Date_Set = model.DateSet;
                entity.Last_Modified = model.LastModified;
                entity.Approved = model.Approved;
                entity.Total_Count = model.TotalCount;

                if (model.Level != null)
                {
                    entity.Level_Id = model.Level.Id;
                }
                if (model.Sex != null)
                {
                    entity.Sex_Id = model.Sex.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
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
