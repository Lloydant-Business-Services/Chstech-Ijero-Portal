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
    public class ProposeAdmissionLogic: BusinessBaseLogic<ProposeAdmission, PROPOSE_ADMISSION>
    {
        public ProposeAdmissionLogic()
        {
            translator = new ProposeAdmissionTranslator();
        }

        public bool Modify(ProposeAdmission proposeAdmission)
        {
            try
            {
                Expression<Func<PROPOSE_ADMISSION, bool>> selector = n => n.Application_Form_Id == proposeAdmission.Id;
                PROPOSE_ADMISSION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Active = proposeAdmission.Active;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
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
