using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class EContentTypeTranslator : TranslatorBase<EContentType, E_CONTENT_TYPE>
    {
        private CourseAllocationTranslator courseAllocationTranslator;
        public EContentTypeTranslator()
        {
            courseAllocationTranslator = new CourseAllocationTranslator();
        }
        public override E_CONTENT_TYPE TranslateToEntity(EContentType model)
        {
            try
            {
                E_CONTENT_TYPE entity = null;
                if (model != null)
                {
                    entity = new E_CONTENT_TYPE();
                    entity.Id = model.Id;
                    entity.Active = model.Active;
                    entity.Description = model.Description;
                    entity.Name = model.Name;
                    entity.StartDate = model.StartTime;
                    entity.EndDate = model.EndTime;
                    entity.IsDelete = model.IsDelete;
                    if (model.CourseAllocation?.Id > 0)
                    {
                        entity.Course_Allocation_Id = model.CourseAllocation.Id;
                    }

                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override EContentType TranslateToModel(E_CONTENT_TYPE entity)
        {
            try
            {
                EContentType model = null;
                if (entity != null)
                {
                    model = new EContentType();
                    model.Id = entity.Id;
                    model.Name = entity.Name;
                    model.Description = entity.Description;
                    model.Active = entity.Active;
                    model.StartTime = entity.StartDate;
                    model.EndTime = entity.EndDate;
                    model.IsDelete = entity.IsDelete;
                    model.CourseAllocation = courseAllocationTranslator.Translate(entity.COURSE_ALLOCATION);

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
