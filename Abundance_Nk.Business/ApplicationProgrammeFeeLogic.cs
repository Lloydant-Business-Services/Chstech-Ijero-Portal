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
    public class ApplicationProgrammeFeeLogic : BusinessBaseLogic<ApplicationProgrammeFee, APPLICATION_PROGRAMME_FEE>
    {
        public ApplicationProgrammeFeeLogic()
        {
            translator = new ApplicationProgrammeFeeTranslator();
        }

        public ApplicationProgrammeFee GetBy(Programme programme, Session session)
        {
            try
            {
                Expression<Func<APPLICATION_PROGRAMME_FEE, bool>> selector = m => m.Programme_Id == programme.Id && m.Session_Id == session.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicationProgrammeFee> GetListBy(Programme programme, Session session)
        {
            try
            {
                Expression<Func<APPLICATION_PROGRAMME_FEE, bool>> selector = m => m.Programme_Id == programme.Id && m.Session_Id == session.Id;
                return GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(ApplicationProgrammeFee model)
        {
            try
            {
                Expression<Func<APPLICATION_PROGRAMME_FEE, bool>> selector = s => s.Application_Programme_Fee_Id == model.Id;
                APPLICATION_PROGRAMME_FEE entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Session != null && model.Session.Id > 0)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.Programme != null && model.Programme.Id > 0)
                {
                    entity.Programme_Id = model.Programme.Id;
                }
                if (model.FeeType != null && model.FeeType.Id > 0)
                {
                    entity.Fee_Type_Id = model.FeeType.Id;
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
