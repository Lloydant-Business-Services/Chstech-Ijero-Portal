using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
   public class ProposeAdmissionTranslator : TranslatorBase<ProposeAdmission, PROPOSE_ADMISSION>
    {
        private PersonTranslator personTranslator;
        private DepartmentTranslator departmentTranslator;
        private ProgrammeTranslator programmeTranslator;
        private SessionTranslator sessionTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private UserTranslator userTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;

        public ProposeAdmissionTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            userTranslator = new UserTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();

        }
        public override ProposeAdmission TranslateToModel(PROPOSE_ADMISSION entity)
        {
            try
            {
                ProposeAdmission model = null;
                if (entity != null)
                {
                    model = new ProposeAdmission();
                    model.Id = entity.Application_Form_Id;
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);                   
                    model.Active = entity.Active;
                   
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override PROPOSE_ADMISSION TranslateToEntity(ProposeAdmission model)
        {
            try
            {
                PROPOSE_ADMISSION entity = null;
                if (model != null)
                {
                    entity = new PROPOSE_ADMISSION();
                    entity.Propose_Admission_Id = model.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.User_Id = model.User.Id;
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                    entity.Active = model.Active;
                    if(model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }

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
