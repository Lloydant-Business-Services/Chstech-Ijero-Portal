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
    public class PostUtmeResultAuditLogic : BusinessBaseLogic<PutmeResultAudit, PUTME_RESULT_AUDIT>
    {
        public PostUtmeResultAuditLogic()
        {
            translator = new PutmeResultAuditTranslator ();
        }
    }
}
