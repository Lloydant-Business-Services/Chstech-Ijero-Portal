using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{

    public class ECourseContentDownloadLogic : BusinessBaseLogic<ECourseContentDownload, E_COURSE_CONTENT_DOWNLOAD>
    {
        public ECourseContentDownloadLogic()
        {
            translator = new ECourseContentDownloadTranslator();
        }

        public List<ECourseContentDownload> getBy(long Id, long personId)
        {
            try
            {
                return GetModelsBy(a => a.E_Course_Content_Download_Id == Id && a.Person_Id == personId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Remove(Func<E_COURSE_CONTENT_DOWNLOAD, bool> selector)
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
