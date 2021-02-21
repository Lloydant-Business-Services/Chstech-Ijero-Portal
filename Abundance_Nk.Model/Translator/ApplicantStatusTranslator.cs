using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantStatusTranslator : TranslatorBase<ApplicantStatus, APPLICANT_STATUS>
    {
        public override ApplicantStatus TranslateToModel(APPLICANT_STATUS entity)
        {
            try
            {
                ApplicantStatus model = null;
                if (entity != null)
                {
                    model = new ApplicantStatus();
                    model.Id = entity.Applicant_Status_Id;
                    model.Name = entity.Applicant_Status_Name;
                    model.Description = entity.Applicant_Status_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_STATUS TranslateToEntity(ApplicantStatus model)
        {
            try
            {
                APPLICANT_STATUS entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_STATUS();
                    entity.Applicant_Status_Id = model.Id;
                    entity.Applicant_Status_Name = model.Name;
                    entity.Applicant_Status_Description = model.Description;
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
