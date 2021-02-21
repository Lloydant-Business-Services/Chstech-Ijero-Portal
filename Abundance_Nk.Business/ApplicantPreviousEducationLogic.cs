using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class ApplicantPreviousEducationLogic: BusinessBaseLogic<ApplicantPreviousEducation, APPLICANT_PREVIOUS_EDUCATION>
    {
        public ApplicantPreviousEducationLogic()
        {
            translator = new ApplicantPreviousEducationTranslator();
        }

        public bool Modify(ApplicantPreviousEducation applicantPreviousEducation)
        {
            try
            {
                ApplicantPreviousEducation model = GetModelsBy(ap => ap.Applicant_Previous_Education_Id == applicantPreviousEducation.Applicant_Previous_Education_Id).FirstOrDefault();
                Expression<Func<APPLICANT_PREVIOUS_EDUCATION, bool>> selector = ap => ap.Applicant_Previous_Education_Id == applicantPreviousEducation.Applicant_Previous_Education_Id;
                APPLICANT_PREVIOUS_EDUCATION entity = GetEntityBy(selector);
                if (!string.IsNullOrEmpty(applicantPreviousEducation.Previous_School_Name))
                {
                    entity.Previous_School_Name = applicantPreviousEducation.Previous_School_Name;
                }
                
                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
