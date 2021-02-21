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
    public class HostelLogic : BusinessBaseLogic<Hostel,HOSTEL>
    {
        public HostelLogic()
        {
            translator = new HostelTranslator();
        }
        public bool Modify(Hostel model)
        {
            try
            {
                Expression<Func<HOSTEL, bool>> selector = s => s.Hostel_Id == model.Id;
                HOSTEL entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.HostelType != null && model.HostelType.Hostel_Type_Id > 0)
                {
                    entity.Hostel_Type_Id = model.HostelType.Hostel_Type_Id;
                }
                entity.Hostel_Name = model.Name;
                entity.Capacity = model.Capacity;
                entity.Description = model.Description;
                entity.Date_Entered = model.DateEntered;
                entity.Activated = model.Activated;

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
