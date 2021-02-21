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
    public class OLevelResultAuditLogic:BusinessBaseLogic<OlevelResultAudit,APPLICANT_O_LEVEL_RESULT_AUDIT>
    {
          public OLevelResultAuditLogic()
        {
            translator = new OLevelResultAuditTranslator();
        }
    }
}
