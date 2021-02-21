using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class ProgrammeDepartmentLogic : BusinessBaseLogic<ProgrammeDepartment, PROGRAMME_DEPARTMENT>
    {
        public ProgrammeDepartmentLogic()
        {
            translator = new ProgrammeDepartmentTranslator();
        }

        public List<Department> GetBy(Programme programme)
        {
            try
            {
                Expression<Func<PROGRAMME_DEPARTMENT, bool>> selector = pd => pd.Programme_Id == programme.Id;
                List<ProgrammeDepartment> programmeDepartments = GetModelsBy(selector);

                List<Department> departments = (from d in programmeDepartments
                                                select new Department
                                                {
                                                    Id = d.Department.Id,
                                                    Name = d.Department.Name,
                                                }).ToList();

                return departments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(ProgrammeDepartment model)
        {
            try
            {
                Expression<Func<PROGRAMME_DEPARTMENT, bool>> selector = d => d.Programme_Department_Id == model.Id;
                PROGRAMME_DEPARTMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Department_Id = model.Department.Id;
                entity.Programme_Id = model.Programme.Id;


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
