using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionListAuditTranslator : TranslatorBase<AdmissionListAudit, ADMISSION_LIST_AUDIT>
    {
        private ApplicationFormTranslator applicationFormTranslator;
        private DepartmentTranslator departmentTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;
        private AdmissionListTranslator admissionListTranslator;
        private UserTranslator userTranslator;
        public AdmissionListAuditTranslator()
        {
            applicationFormTranslator = new ApplicationFormTranslator();
            departmentTranslator = new DepartmentTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            admissionListTranslator = new AdmissionListTranslator();
            userTranslator = new UserTranslator();
        }
        public override AdmissionListAudit TranslateToModel(ADMISSION_LIST_AUDIT entity)
        {
            try
            {
                AdmissionListAudit model = null;
                if (entity != null)
                {
                    model = new AdmissionListAudit();
                    model.Id = entity.Admission_List_Audit_Id;
                    model.AdmissionList = admissionListTranslator.Translate(entity.ADMISSION_LIST);
                    model.Form = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Deprtment = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;
                    model.User = userTranslator.Translate(entity.USER);

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_LIST_AUDIT TranslateToEntity(AdmissionListAudit model)
        {
            try
            {
                ADMISSION_LIST_AUDIT entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_LIST_AUDIT();
                    entity.Admission_List_Audit_Id = model.Id;
                    entity.Admission_List_Id = model.AdmissionList.Id;
                    entity.Application_Form_Id = model.Form.Id;
                    entity.Department_Id = model.Deprtment.Id;
                    if (model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
                    entity.User_Id = model.User.Id;
                    entity.Action = model.Action;
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
