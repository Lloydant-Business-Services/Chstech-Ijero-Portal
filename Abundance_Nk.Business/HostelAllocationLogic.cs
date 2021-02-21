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
    public class HostelAllocationLogic: BusinessBaseLogic<HostelAllocation,HOSTEL_ALLOCATION>
    {
        public HostelAllocationLogic()
        {
            translator = new HostelAllocationTranslator();
        }
        public bool Modify(HostelAllocation model)
        {
            try
            {
                int modified = 0;
                Expression<Func<HOSTEL_ALLOCATION, bool>> selector = c => c.Id == model.Id;
                HOSTEL_ALLOCATION entity = GetEntityBy(selector);
                if (entity != null)
                {
                    entity.Id = model.Id;
                    entity.Occupied = model.Occupied;
                    if (model.Series != null)
                    {
                        entity.Series_Id = model.Series.Id;
                    }
                    if (model.Hostel != null)
                    {
                        entity.Hostel_Id = model.Hostel.Id;
                    }
                    if (model.Room != null)
                    {
                        entity.Room_Id = model.Room.Id;
                    }
                    if (model.Corner != null)
                    {
                        entity.Corner_Id = model.Corner.Id;
                    }
                    if (model.Student != null)
                    {
                        entity.Student_Id = model.Student.Id;
                    }
                    if (model.Person != null)
                    {
                        entity.Student_Id = model.Person.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    if (model.Payment != null)
                    {
                        entity.Payment_Id = model.Payment.Id;
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
        public List<HostelAllocation> GetVacantBedSpaces(Hostel hostel)
        {
            List<HostelAllocation> vancantBedSpaces = new List<HostelAllocation>();
            try
            {
                vancantBedSpaces = (from b in repository.GetBy<VW_VACANT_BEDSPACES>(h => h.Hostel_Id == hostel.Id)
                                    select new HostelAllocation
                                    {
                                        HostelId = b.Hostel_Id,
                                        HostelName = b.Hostel_Name,
                                        SeriesId = b.Series_Id,
                                        SeriesName = b.Series_Name,
                                        RoomId = b.Room_Id,
                                        RoomName = b.Room_Number,
                                        CornerId = b.Corner_Id,
                                        CornerName = b.Corner_Name,
                                        ReserveStatus = b.Reserved ? "Reserved" : "Free"
                                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return vancantBedSpaces;
        }
    }
}
