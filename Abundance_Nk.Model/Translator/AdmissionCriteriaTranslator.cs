using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class AdmissionCriteriaTranslator : TranslatorBase<AdmissionCriteria, ADMISSION_CRITERIA>
    {
        private ProgrammeTranslator programmeTranslator;
        private DepartmentTranslator departmentTranslator;

        public AdmissionCriteriaTranslator()
        {
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
        }

        public override AdmissionCriteria TranslateToModel(ADMISSION_CRITERIA entity)
        {
            try
            {
                AdmissionCriteria model = null;
                if (entity != null)
                {
                    model = new AdmissionCriteria();
                    model.Id = entity.Admission_Criteria_Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.MinimumRequiredNumberOfSubject = entity.Minimum_Required_Number_Of_Subject;
                    model.DateEntered = entity.Date_Entered;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_CRITERIA TranslateToEntity(AdmissionCriteria model)
        {
            try
            {
                ADMISSION_CRITERIA entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_CRITERIA();
                    entity.Admission_Criteria_Id = model.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Minimum_Required_Number_Of_Subject = model.MinimumRequiredNumberOfSubject;
                    entity.Date_Entered = model.DateEntered;
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
