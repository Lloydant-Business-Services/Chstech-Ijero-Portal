using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using Abundance_Nk.Data;

namespace Abundance_Nk.Business
{
    public class DepartmentOptionLogic : BusinessBaseLogic<DepartmentOption, DEPARTMENT_OPTION>
    {
        public DepartmentOptionLogic()
        {
            translator = new DepartmentOptionTranslator();
        }

        public List<DepartmentOption> GetBy(Department department, Programme programme)
        {
            try
            {
                List<DepartmentOption> departmentOptions = new List<DepartmentOption>();

                if (programme.Id == 3 || programme.Id == 4 || programme.Id == 5)
                {
                    repository = new Repository();
                    departmentOptions = (from d in repository.GetBy<VW_DEPARTMENT_OPTION>()
                                         where d.Department_Id == department.Id && d.Programme_Id == programme.Id
                                         select new DepartmentOption
                                         {
                                             Id = d.Department_Option_Id,
                                             Name = d.Department_Option_Name
                                         }).ToList();
                }
                

                return departmentOptions;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DepartmentOption> GetDeptOption(int departmentId, int programmeId)
        {
            try
            {
                List<DepartmentOption> departmentOptions = new List<DepartmentOption>();

                if (programmeId == 3 || programmeId == 4 || programmeId == 5)
                {
                    repository = new Repository();
                    departmentOptions = (from d in repository.GetBy<VW_DEPARTMENT_OPTION>()
                                         where d.Department_Id == departmentId && d.Programme_Id == programmeId
                                         select new DepartmentOption
                                         {
                                             Id = d.Department_Option_Id,
                                             Name = d.Department_Option_Name
                                         }).ToList();
                }


                return departmentOptions;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(DepartmentOption option)
        {
            try
            {
                Expression<Func<DEPARTMENT_OPTION, bool>> selector = d => d.Department_Option_Id == option.Id;
                DEPARTMENT_OPTION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Department_Option_Name = option.Name;
                entity.DEPARTMENT.Department_Id = option.Department.Id;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
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
