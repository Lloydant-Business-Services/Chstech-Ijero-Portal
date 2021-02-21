using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class FeeDetailAuditLogic : BusinessBaseLogic<FeeDetailAudit, FEE_DETAIL_AUDIT>
    {
        public FeeDetailAuditLogic()
        {
            translator = new FeeDetailAuditTranslator();
        }
    }
}
