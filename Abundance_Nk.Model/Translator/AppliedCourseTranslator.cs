using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AppliedCourseTranslator : TranslatorBase<AppliedCourse, APPLICANT_APPLIED_COURSE>
    {
        private PersonTranslator personTranslator;
        private DepartmentTranslator departmentTranslator;
        private ProgrammeTranslator programmeTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;

        public AppliedCourseTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
        }

        public override AppliedCourse TranslateToModel(APPLICANT_APPLIED_COURSE entity)
        {
            try
            {
                AppliedCourse appliedCourse = null;
                if (entity != null)
                {
                    appliedCourse = new AppliedCourse();
                    appliedCourse.Person = personTranslator.Translate(entity.PERSON);
                    appliedCourse.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    appliedCourse.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    appliedCourse.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    appliedCourse.Option = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    appliedCourse.OptionSecondChoice = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION1);
                    //appliedCourse.Active = true;
                }

                return appliedCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_APPLIED_COURSE TranslateToEntity(AppliedCourse appliedCourse)
        {
            try
            {
                APPLICANT_APPLIED_COURSE entity = null;
                if (appliedCourse != null)
                {
                    entity = new APPLICANT_APPLIED_COURSE();
                    entity.Person_Id = appliedCourse.Person.Id;
                    entity.Programme_Id = appliedCourse.Programme.Id;
                    entity.Department_Id = appliedCourse.Department.Id;
                    //entity.Active = true;
                    if (appliedCourse.Option != null && appliedCourse.Option.Id > 0)
                    {
                        entity.Department_Option_Id = appliedCourse.Option.Id;
                    }
                    if (appliedCourse.OptionSecondChoice != null && appliedCourse.OptionSecondChoice.Id > 0)
                    {
                        entity.Option_Second_Choice_Id = appliedCourse.OptionSecondChoice.Id;
                    }

                    if (appliedCourse.ApplicationForm != null && appliedCourse.ApplicationForm.Id > 0)
                    {
                        entity.Application_Form_Id = appliedCourse.ApplicationForm.Id;
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
