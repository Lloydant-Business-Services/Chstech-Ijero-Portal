using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{

    public class ECourseTranslator : TranslatorBase<ECourse, E_COURSE_CONTENT>
    {
        private CourseTranslator courseTranslator;
        private EContentTypeTranslator contentTypeTranslator;

        public ECourseTranslator()
        {
            courseTranslator = new CourseTranslator();
            contentTypeTranslator = new EContentTypeTranslator();
        }

        public override E_COURSE_CONTENT TranslateToEntity(ECourse model)
        {
            try
            {
                E_COURSE_CONTENT entity = null;
                if (model != null)
                {
                    entity = new E_COURSE_CONTENT();
                    entity.Course_Id = model.Course.Id;
                    entity.E_Content_Type_Id = model.EContentType.Id;
                    entity.Id = model.Id;
                    entity.Url = model.Url;
                    entity.Views = model.views;
                    entity.Active = model.Active;
                    entity.Video_Url = model.VideoUrl;
                    entity.EndDate = model.EndTime;
                    entity.StartDate = model.StartTime;
                    entity.IsDelete = model.IsDelete;
                    entity.LiveStream_Url = model.LiveStreamLink;

                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override ECourse TranslateToModel(E_COURSE_CONTENT entity)
        {
            try
            {
                ECourse model = null;
                if (entity != null)
                {
                    model = new ECourse();
                    model.Id = entity.Id;
                    model.Url = entity.Url;
                    model.views = entity.Views;
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.EContentType = contentTypeTranslator.Translate(entity.E_CONTENT_TYPE);
                    model.Active = entity.Active;
                    model.StartTime = entity.StartDate;
                    model.EndTime = entity.EndDate;
                    model.VideoUrl = entity.Video_Url;
                    model.IsDelete = entity.IsDelete;
                    model.LiveStreamLink = entity.LiveStream_Url;

                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
