using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OLevelResultDetailAuditLogic:BusinessBaseLogic<OlevelResultdDetailsAudit,APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT>
    {
        public OLevelResultDetailAuditLogic()
        {
            translator = new OlevelResultdDetailAuditTranslator();
        }
    }
}
