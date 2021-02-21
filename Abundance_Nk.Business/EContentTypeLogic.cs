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
    public class EContentTypeLogic : BusinessBaseLogic<EContentType, E_CONTENT_TYPE>
    {
        public EContentTypeLogic()
        {
            translator = new EContentTypeTranslator();
        }
        public bool Remove(Func<E_CONTENT_TYPE, bool> selector)
        {
            try
            {
                repository.Delete(selector);
                return Save() > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public bool Modify(EContentType model)
        {
            try
            {

                Expression<Func<E_CONTENT_TYPE, bool>> selector = a => a.Id == model.Id;
                E_CONTENT_TYPE entity = GetEntityBy(selector);
                if (entity == null || entity.Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Active = model.Active;
                entity.Description = model.Description;
                entity.EndDate = model.EndTime;
                entity.StartDate = model.StartTime;
                entity.Name = model.Name;
                entity.IsDelete = model.IsDelete;
                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
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
