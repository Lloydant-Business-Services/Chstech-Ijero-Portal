using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantJambDetailAuditLogic : BusinessBaseLogic<ApplicantJambDetailAudit, APPLICANT_JAMB_DETAIL_AUDIT>
    {
        public ApplicantJambDetailAuditLogic()
        {
            translator = new ApplicantJambDetailAuditTranslator();
        }
    }
}