using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionQuotaTranslator : TranslatorBase<AdmissionQuota, ADMISSION_QUOTA>
    {
        private PersonTranslator personTranslator;
        private DepartmentTranslator departmentTranslator;
        private ProgrammeTranslator programmeTranslator;
        private SessionTranslator sessionTranslator;
        private ApplicationFormTranslator applicationFormTranslator;
        private UserTranslator userTranslator;

        public AdmissionQuotaTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            userTranslator = new UserTranslator();

        }

        public override AdmissionQuota TranslateToModel(ADMISSION_QUOTA entity)
        {
            try
            {
                AdmissionQuota model = null;
                if (entity != null)
                {
                    model = new AdmissionQuota();
                    model.Id = entity.Quota_Id;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Quota = entity.Quota;
                    model.UnusedQuota = entity.UnusedQuota;
                    model.Active = entity.Active;
                    

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_QUOTA TranslateToEntity(AdmissionQuota model)
        {
            try
            {
                ADMISSION_QUOTA entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_QUOTA();
                    entity.Quota_Id = model.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.User_Id = model.User.Id;
                    entity.Quota = model.Quota;
                    entity.UnusedQuota = model.UnusedQuota;
                    entity.Active = model.Active;

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
