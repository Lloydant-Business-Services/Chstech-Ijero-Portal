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
    public class ApplicantSponsorLogic: BusinessBaseLogic<ApplicantSponsor, APPLICANT_SPONSOR>
    {
        public ApplicantSponsorLogic()
        {
            translator = new ApplicantSponsorTranslator();
        }

        public bool Modify(ApplicantSponsor applicantSponsor)
        {
            try
            {
                ApplicantSponsor model = GetModelsBy(ap => ap.Person_Id == applicantSponsor.Person.Id).FirstOrDefault();
                Expression<Func<APPLICANT_SPONSOR, bool>> selector = ap => ap.Person_Id == applicantSponsor.Person.Id;
                APPLICANT_SPONSOR entity = GetEntityBy(selector);
                if (!string.IsNullOrEmpty(applicantSponsor.Sponsor_Name))
                {
                    entity.Sponsor_Name = applicantSponsor.Sponsor_Name;
                }
                if (applicantSponsor.Relationship_Id > 0)
                {
                    entity.Relationship_Id = applicantSponsor.Relationship.Id;
                }

                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}