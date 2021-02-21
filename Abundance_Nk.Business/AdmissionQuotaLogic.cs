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
    public class AdmissionQuotaLogic : BusinessBaseLogic<AdmissionQuota, ADMISSION_QUOTA>
    {
        public AdmissionQuotaLogic()
        {
            translator = new AdmissionQuotaTranslator();
        }

        public bool Modify(AdmissionQuota AdmissionQuota)
        {
            try
            {
                Expression<Func<ADMISSION_QUOTA, bool>> selector = n => n.Quota_Id == AdmissionQuota.Id;
                ADMISSION_QUOTA entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Active = AdmissionQuota.Active;
                entity.UnusedQuota = AdmissionQuota.UnusedQuota;
                entity.Quota = AdmissionQuota.Quota;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    //throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
