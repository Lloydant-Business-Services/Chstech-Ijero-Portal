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
    public class StaffDepartmentLogic : BusinessBaseLogic<StaffDepartment, STAFF_DEPARTMENT>
    {
        public StaffDepartmentLogic()
        {
            translator = new StaffDepartmentTranslator();
        }
        public bool Modify(StaffDepartment model)
        {
            try
            {
                Expression<Func<STAFF_DEPARTMENT, bool>> selector = a => a.Staff_Department_Id == model.Id;
                STAFF_DEPARTMENT entity = GetEntitiesBy(selector).LastOrDefault();

                if (entity != null)
                {
                    entity.IsHead = model.IsHead;
                    entity.IsExamOfficer = model.IsExamOfficer;

                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.SessionSemester != null)
                    {
                        entity.Session_Semester_Id = model.SessionSemester.Id;
                    }
                    if (model.Staff != null)
                    {
                        entity.Staff_Id = model.Staff.Id;
                    }

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
