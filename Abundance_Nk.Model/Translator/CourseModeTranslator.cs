using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CourseModeTranslator : TranslatorBase<CourseMode, COURSE_MODE>
    {
        public override CourseMode TranslateToModel(COURSE_MODE entity)
        {
            try
            {
                CourseMode model = null;
                if (entity != null)
                {
                    model = new CourseMode();
                    model.Id = entity.Course_Mode_Id;
                    model.Name = entity.Course_Mode_Name;
                    model.Description = entity.Course_Mode_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_MODE TranslateToEntity(CourseMode model)
        {
            try
            {
                COURSE_MODE entity = null;
                if (model != null)
                {
                    entity = new COURSE_MODE();
                    entity.Course_Mode_Id = model.Id;
                    entity.Course_Mode_Name = model.Name;
                    entity.Course_Mode_Description = model.Description;
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
