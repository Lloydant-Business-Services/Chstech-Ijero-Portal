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

    public class ECourseLogic : BusinessBaseLogic<ECourse, E_COURSE_CONTENT>
    {
        public ECourseLogic()
        {
            translator = new ECourseTranslator();
        }

        public List<ECourse> getBy(long courseId)
        {
            try
            {
                return GetModelsBy(a => a.Course_Id == courseId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Modify(ECourse model)
        {
            try
            {

                Expression<Func<E_COURSE_CONTENT, bool>> selector = a => a.Id == model.Id;
                E_COURSE_CONTENT entity = GetEntityBy(selector);
                if (entity == null || entity.Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.views > 0)
                {
                    entity.Views = model.views;
                }
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
        public bool Remove(Func<E_COURSE_CONTENT, bool> selector)
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

    }
}
