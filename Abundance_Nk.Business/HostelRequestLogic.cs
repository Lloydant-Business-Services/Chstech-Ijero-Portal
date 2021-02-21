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
    public class HostelRequestLogic : BusinessBaseLogic<HostelRequest, HOSTEL_REQUEST>
    {
        public HostelRequestLogic()
        {
            translator = new HostelRequestTranslator();
        }

        public bool Modify(HostelRequest model)
        {
            try
            { 
                int modified = 0;
                Expression<Func<HOSTEL_REQUEST, bool>> selector = c => c.Hostel_Request_Id == model.Id;
                HOSTEL_REQUEST entity = GetEntityBy(selector);
                if (entity != null)
                {
                    entity.Approved = model.Approved;
                    entity.Request_Date = model.RequestDate;
                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.Level != null)
                    {
                        entity.Level_Id = model.Level.Id;
                    }
                    if (model.Programme != null)
                    {
                        entity.Programme_Id = model.Programme.Id;
                    }
                    if (model.Student != null)
                    {
                        entity.Person_Id = model.Student.Id;
                    }
                    if (model.Person != null)
                    {
                        entity.Person_Id = model.Person.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
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
