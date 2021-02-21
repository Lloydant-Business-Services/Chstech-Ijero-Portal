using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ECourseContentDownloadTranslator : TranslatorBase<ECourseContentDownload, E_COURSE_CONTENT_DOWNLOAD>
    {
        private PersonTranslator personTranslator;
        private ECourseTranslator eCourseTranslator;


        public ECourseContentDownloadTranslator()
        {
            personTranslator = new PersonTranslator();
            eCourseTranslator = new ECourseTranslator();

        }

        public override ECourseContentDownload TranslateToModel(E_COURSE_CONTENT_DOWNLOAD entity)
        {
            try
            {
                ECourseContentDownload model = null;
                if (entity != null)
                {
                    model = new ECourseContentDownload();
                    model.ECourseDownloadId = entity.E_Course_Content_Download_Id;
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.ECourse = eCourseTranslator.Translate(entity.E_COURSE_CONTENT);

                    model.DateViewed = entity.Date_Viewed;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override E_COURSE_CONTENT_DOWNLOAD TranslateToEntity(ECourseContentDownload model)
        {
            try
            {
                E_COURSE_CONTENT_DOWNLOAD entity = null;
                if (model != null)
                {
                    entity = new E_COURSE_CONTENT_DOWNLOAD();
                    entity.E_Course_Content_Download_Id = model.ECourseDownloadId;
                    entity.Person_Id = model.Person.Id;
                    entity.E_Course_Content_Id = model.ECourse.Id;
                    entity.Date_Viewed = model.DateViewed;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
