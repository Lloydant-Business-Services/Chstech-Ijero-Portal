using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class DepartmentHeadTranslator: TranslatorBase<DepartmentHead, DEPARTMENT_HEAD>
    {
        private DepartmentTranslator DepartmentTranslator;
        private PersonTranslator PersonTranslator;

        public DepartmentHeadTranslator()
        {
            DepartmentTranslator = new DepartmentTranslator();
            PersonTranslator = new PersonTranslator();
        }

        public override DepartmentHead TranslateToModel(DEPARTMENT_HEAD entity)
        {
            try
            {
                DepartmentHead model = null;
                if (entity != null)
                {
                    model = new DepartmentHead();
                    model.DepartmentHeadId = entity.Department_Head_Id;
                    model.Date = entity.Date;
                    model.Department = DepartmentTranslator.Translate(entity.DEPARTMENT);
                    model.Person = PersonTranslator.Translate(entity.PERSON);
                }

                return model;
            }
            catch (Exception ex) { throw ex; }
        }

        public override DEPARTMENT_HEAD TranslateToEntity(DepartmentHead model)
        {
            try
            {
                DEPARTMENT_HEAD entity = null;
                if (model != null)
                {
                    entity = new DEPARTMENT_HEAD();
                    entity.Department_Id = model.Department.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Department_Head_Id = model.DepartmentHeadId;
                    entity.Date = model.Date;
                }

                return entity;
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
