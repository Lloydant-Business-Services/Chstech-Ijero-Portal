
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
    public class DepartmentLogic : BusinessBaseLogic<Department, DEPARTMENT>
    {
        public DepartmentLogic()
        {
            translator = new DepartmentTranslator();
        }

        public List<Department> GetBy(Programme programme)
        {
            try
            {
                repository = new Repository();
                var departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                                   where d.Programme_Id == programme.Id && d.Active == true
                                   select new Department
                                   {
                                       Id = d.Department_Id,
                                       Name = d.Department_Name
                                   }
                                       ).ToList();

                return departments.OrderBy(d => d.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Department> GetBy(Programme programme, Faculty faculty)
        {
            try
            {
                repository = new Repository();
                var departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                                   where d.Programme_Id == programme.Id && d.Faculty_Id == faculty.Id && d.Active == true
                                   select new Department
                                   {
                                       Id = d.Department_Id,
                                       Name = d.Department_Name
                                   }
                                       ).ToList();

                return departments.OrderBy(d => d.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(Department department)
        {
            try
            {
                Expression<Func<DEPARTMENT, bool>> selector = f => f.Department_Id == department.Id;
                DEPARTMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Department_Name = department.Name;
                entity.Department_Code = department.Code;
                entity.Faculty_Id = department.Faculty.Id;
                entity.Active = department.Active;

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
        public List<Department> GetBy(Programme programme, Department department)
        {
            try
            {
                repository = new Repository();
                var departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                                   where d.Programme_Id == programme.Id && d.Active == true && d.Department_Id==department.Id
                                   select new Department
                                   {
                                       Id = d.Department_Id,
                                       Name = d.Department_Name
                                   }
                                       ).ToList();

                return departments.OrderBy(d => d.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }



    }

}
