using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentResultDetailTranslator : TranslatorBase<StudentResultDetail, STUDENT_RESULT_DETAIL>
    {
        private CourseTranslator courseTranslator;
        private StudentTranslator studentTranslator;
        private StudentResultTranslator studentResultTranslator;

        public StudentResultDetailTranslator()
        {
            courseTranslator = new CourseTranslator();
            studentTranslator = new StudentTranslator();
            studentResultTranslator = new StudentResultTranslator();
        }

        public override StudentResultDetail TranslateToModel(STUDENT_RESULT_DETAIL entity)
        {
            try
            {
                StudentResultDetail model = null;
                if (entity != null)
                {
                    model = new StudentResultDetail();
                    model.Header = studentResultTranslator.Translate(entity.STUDENT_RESULT);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.Score = entity.Score;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_RESULT_DETAIL TranslateToEntity(StudentResultDetail model)
        {
            try
            {
                STUDENT_RESULT_DETAIL entity = null;
                if (model != null)
                {
                    entity = new STUDENT_RESULT_DETAIL();
                    entity.Student_Result_Id = model.Header.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.Score = model.Score;
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
