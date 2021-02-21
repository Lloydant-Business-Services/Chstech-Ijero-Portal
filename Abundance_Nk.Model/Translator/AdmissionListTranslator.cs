using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionListTranslator : TranslatorBase<AdmissionList, ADMISSION_LIST>
    {
        private ApplicationFormTranslator applicationFormTranslator;
        private AdmissionListBatchTranslator admissionListBatchTranslator;
        private DepartmentTranslator departmentTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;
        private SessionTranslator sessionTranslator;
        private ProgrammeTranslator programmeTranslator;

        public AdmissionListTranslator()
        {
            applicationFormTranslator = new ApplicationFormTranslator();
            admissionListBatchTranslator = new AdmissionListBatchTranslator();
            departmentTranslator = new DepartmentTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            sessionTranslator = new SessionTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }

        public override AdmissionList TranslateToModel(ADMISSION_LIST entity)
        {
            try
            {
                AdmissionList model = null;
                if (entity != null)
                {
                    model = new AdmissionList();
                    model.Id = entity.Admission_List_Id;
                    model.Form = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Batch = admissionListBatchTranslator.Translate(entity.ADMISSION_LIST_BATCH);
                    model.Deprtment = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Activated = entity.Activated;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_LIST TranslateToEntity(AdmissionList model)
        {
            try
            {
                ADMISSION_LIST entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_LIST();
                    entity.Admission_List_Id = model.Id;
                    entity.Application_Form_Id = model.Form.Id;
                    entity.Admission_List_Batch_Id = model.Batch.Id;
                    entity.Department_Id = model.Deprtment.Id;
                    if (model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    entity.Session_Id = model.Session.Id;
                    entity.Activated = model.Activated;
                    entity.Programme_Id = model.Programme.Id;
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
