using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class ApplicantClearanceLogic : BusinessBaseLogic<ApplicantClearance, APPLICANT_CLEARANCE>
    {
        public ApplicantClearanceLogic()
        {
            translator = new ApplicantClearanceTranslator();
        }

        public ApplicantClearance GetBy(ApplicationForm form)
        {
            try
            {
                Expression<Func<APPLICANT_CLEARANCE, bool>> selector = ac => ac.Application_Form_Id == form.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(ApplicantClearance clearance)
        {
            try
            {
                Expression<Func<APPLICANT_CLEARANCE, bool>> selector = ac => ac.Application_Form_Id == clearance.ApplicationForm.Id;
                APPLICANT_CLEARANCE entity = GetEntityBy(selector);

                entity.Cleared = clearance.Cleared;
                entity.Date_Cleared = clearance.DateCleared;

                if (clearance.User != null)
                    entity.User_Id = clearance.User.Id;

                int modifiedRecordCount = Save();

                return modifiedRecordCount > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsCleared(ApplicationForm form)
        {
            try
            {
                ApplicantClearance applicantClearance = GetBy(form);
                return applicantClearance == null ? false : true;
            }
            catch(Exception)
            {
                throw;
            }
        }


    }





}
