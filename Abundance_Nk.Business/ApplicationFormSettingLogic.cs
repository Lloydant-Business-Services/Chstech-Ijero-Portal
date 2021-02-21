using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicationFormSettingLogic : BusinessBaseLogic<ApplicationFormSetting, APPLICATION_FORM_SETTING>
    {
        public ApplicationFormSettingLogic()
        {
            translator = new ApplicationFormSettingTranslator();
        }

        public ApplicationFormSetting GetBy(Session session, FeeType feeType)
        {
            try
            {
               return GetModelsBy(m => m.Session_Id == session.Id && m.Fee_Type_Id == feeType.Id).LastOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(ApplicationFormSetting model)
        {
            try
            {
                Expression<Func<APPLICATION_FORM_SETTING, bool>> selector = s => s.Application_Form_Setting_Id == model.Id;
                APPLICATION_FORM_SETTING entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Session != null && model.Session.Id > 0)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.PaymentMode != null && model.PaymentMode.Id > 0)
                {
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
