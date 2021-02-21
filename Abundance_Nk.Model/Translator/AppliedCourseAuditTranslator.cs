using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AppliedCourseAuditTranslator : TranslatorBase<AppliedCourseAudit, APPLICANT_APPLIED_COURSE_AUDIT>
    {
        private AppliedCourseTranslator appliedCourseTranslator;
        private PersonTranslator personTranslator;
        private DepartmentTranslator departmentTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;
        private ProgrammeTranslator programmeTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private UserTranslator userTranslator;

        public AppliedCourseAuditTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            programmeTranslator = new ProgrammeTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            appliedCourseTranslator = new AppliedCourseTranslator();
            userTranslator = new UserTranslator();
        }

        public override AppliedCourseAudit TranslateToModel(APPLICANT_APPLIED_COURSE_AUDIT entity)
        {
            try
            {
                AppliedCourseAudit appliedCourse = null;
                if (entity != null)
                {
                    appliedCourse = new AppliedCourseAudit();
                    appliedCourse.Id = entity.Applicant_Applied_Course_Audit_Id;
                    appliedCourse.AppliedCourse = appliedCourseTranslator.Translate(entity.APPLICANT_APPLIED_COURSE);
                    appliedCourse.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    appliedCourse.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    appliedCourse.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    appliedCourse.Option = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);

                    appliedCourse.OldProgramme = programmeTranslator.Translate(entity.PROGRAMME1);
                    appliedCourse.OldDepartment = departmentTranslator.Translate(entity.DEPARTMENT1);
                    appliedCourse.OldApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM1);
                    appliedCourse.OldOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION1);

                    appliedCourse.User = userTranslator.Translate(entity.USER);
                    appliedCourse.Operation = entity.Operation;
                    appliedCourse.Action = entity.Action;
                    appliedCourse.Time = entity.Time;
                    appliedCourse.Client = entity.Client;
                }

                return appliedCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_APPLIED_COURSE_AUDIT TranslateToEntity(AppliedCourseAudit model)
        {
            try
            {
                APPLICANT_APPLIED_COURSE_AUDIT entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_APPLIED_COURSE_AUDIT();
                    entity.Applicant_Applied_Course_Audit_Id = model.Id;
                    entity.Person_Id = model.AppliedCourse.Person.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    if (model.Option != null && model.Option.Id > 0)
                    {
                        entity.Department_Option_Id = model.Option.Id;
                    }
                    if (model.ApplicationForm != null && model.ApplicationForm.Id > 0)
                    {
                        entity.Application_Form_Id = model.ApplicationForm.Id;
                    }

                    entity.OLD_Programme_Id = model.OldProgramme.Id;
                    entity.OLD_Department_Id = model.OldDepartment.Id;
                    if (model.OldOption != null && model.OldOption.Id > 0)
                    {
                        entity.OLD_Department_Option_Id = model.OldOption.Id;
                    }

                    if (model.OldApplicationForm != null && model.OldApplicationForm.Id > 0)
                    {
                        entity.OLD_Application_Form_Id = model.OldApplicationForm.Id;
                    }

                    entity.User_Id = model.User.Id;
                    entity.Operation = model.Operation;
                    entity.Action = model.Action;
                    entity.Time = model.Time;
                    entity.Client = model.Client;
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
