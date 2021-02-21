using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantStatusLogic : BusinessBaseLogic<ApplicantStatus, APPLICANT_STATUS>
    {
        public ApplicantStatusLogic()
        {
            translator = new ApplicantStatusTranslator();
        }

        public void UpdateStatus(ApplicationForm applicationForm, int offeredAdmission)
        {
            throw new NotImplementedException();
        }
    }
}
