using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ProgrammeDepartmentTranslator : TranslatorBase<ProgrammeDepartment, PROGRAMME_DEPARTMENT>
    {
        private ProgrammeTranslator programmeTranslator;
        private DepartmentTranslator departmentTranslator;

        public ProgrammeDepartmentTranslator()
        {
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
        }

        public override ProgrammeDepartment TranslateToModel(PROGRAMME_DEPARTMENT entity)
        {
            try
            {
                ProgrammeDepartment model = null;
                if (entity != null)
                {
                    model = new ProgrammeDepartment();
                    model.Id = entity.Department_Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PROGRAMME_DEPARTMENT TranslateToEntity(ProgrammeDepartment model)
        {
            try
            {
                PROGRAMME_DEPARTMENT entity = null;
                if (model != null)
                {
                    entity = new PROGRAMME_DEPARTMENT();
                    entity.Department_Id = model.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
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
