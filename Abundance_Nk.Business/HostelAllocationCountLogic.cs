using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class HostelAllocationCountLogic : BusinessBaseLogic<HostelAllocationCount, HOSTEL_ALLOCATION_COUNT>
    {
        public HostelAllocationCountLogic()
        {
            translator = new HostelAllocationCountTranslator();
        }
        public bool Modify(HostelAllocationCount model)
        {
            try
            {
                Expression<Func<HOSTEL_ALLOCATION_COUNT, bool>> selector = p => p.Hostel_Allocation_Count_Id == model.Id;
                HOSTEL_ALLOCATION_COUNT entity = GetEntityBy(selector);

                if (entity == null || entity.Hostel_Allocation_Count_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Activated = model.Activated;
                entity.Date_Set = model.DateSet;
                entity.Last_Modified = model.LastModified;
                entity.Free = model.Free;
                entity.Reserved = model.Reserved;
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
