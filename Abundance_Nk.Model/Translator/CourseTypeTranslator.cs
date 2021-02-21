using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class CourseTypeTranslator : TranslatorBase<CourseType, COURSE_TYPE>
    {
        public override CourseType TranslateToModel(COURSE_TYPE entity)
        {
            try
            {
                CourseType model = null;
                if (entity != null)
                {
                    model = new CourseType();
                    model.Id = entity.Course_Type_Id;
                    model.Name = entity.Course_Type_Name;
                    model.Description = entity.Course_Type_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_TYPE TranslateToEntity(CourseType model)
        {
            try
            {
                COURSE_TYPE entity = null;
                if (model != null)
                {
                    entity = new COURSE_TYPE();
                    entity.Course_Type_Id = model.Id;
                    entity.Course_Type_Name = model.Name;
                    entity.Course_Type_Description = model.Description;
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
