using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;


namespace Abundance_Nk.Model.Translator
{
    public class DepartmentOptionTranslator : TranslatorBase<DepartmentOption, DEPARTMENT_OPTION>
    {
        public DepartmentTranslator departmentTranslator;

        public DepartmentOptionTranslator()
        {
            departmentTranslator = new DepartmentTranslator();
        }
        
        public override DepartmentOption TranslateToModel(DEPARTMENT_OPTION entity)
        {
            try
            {
                DepartmentOption departmentOption = null;
                if (entity != null)
                {
                    departmentOption = new DepartmentOption();
                    departmentOption.Id = entity.Department_Option_Id;
                    departmentOption.Name = entity.Department_Option_Name;
                    departmentOption.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    departmentOption.Activated = entity.Activated;

                }

                return departmentOption;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DEPARTMENT_OPTION TranslateToEntity(DepartmentOption departmentOption)
        {
            try
            {
                DEPARTMENT_OPTION entity = null;
                if (departmentOption != null)
                {
                    entity = new DEPARTMENT_OPTION();
                    entity.Department_Option_Id = departmentOption.Id;
                    entity.Department_Option_Name = departmentOption.Name;
                    entity.Activated = departmentOption.Activated;
                    entity.Department_Id = departmentOption.Department.Id;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}
