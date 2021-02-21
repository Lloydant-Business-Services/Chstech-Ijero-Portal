using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CourseRegistrationDetailTranslator : TranslatorBase<CourseRegistrationDetail, STUDENT_COURSE_REGISTRATION_DETAIL>
    {
        private CourseTranslator courseTranslator;
        private CourseModeTranslator courseModeTranslator;
        private SemesterTranslator semesterTranslator;
        private CourseRegistrationTranslator courseRegistrationTranslator;

        public CourseRegistrationDetailTranslator()
        {
            courseTranslator = new CourseTranslator();
            courseModeTranslator = new CourseModeTranslator();
            semesterTranslator = new SemesterTranslator();
            courseRegistrationTranslator = new CourseRegistrationTranslator();
        }
        public override CourseRegistrationDetail TranslateToModel(STUDENT_COURSE_REGISTRATION_DETAIL entity)
        {
            try
            {
                CourseRegistrationDetail model = null;
                if (entity != null)
                {
                    model = new CourseRegistrationDetail();
                    model.Id = entity.Student_Course_Registration_Detail_Id;
                    model.CourseRegistration = courseRegistrationTranslator.Translate(entity.STUDENT_COURSE_REGISTRATION);
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.Mode = courseModeTranslator.Translate(entity.COURSE_MODE);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.TestScore = entity.Test_Score;
                    model.ExamScore = entity.Exam_Score;
                    model.CourseUnit = entity.Course_Unit;
                    model.SpecialCase = entity.Special_Case;


                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override STUDENT_COURSE_REGISTRATION_DETAIL TranslateToEntity(CourseRegistrationDetail model)
        {
            try
            {
                STUDENT_COURSE_REGISTRATION_DETAIL entity = null;
                if (model != null)
                {
                    entity = new STUDENT_COURSE_REGISTRATION_DETAIL();
                    entity.Student_Course_Registration_Detail_Id = model.Id;
                    entity.Student_Course_Registration_Id = model.CourseRegistration.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.Course_Mode_Id = model.Mode.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Test_Score = model.TestScore;
                    entity.Exam_Score = model.ExamScore;
                    entity.Course_Unit = model.CourseUnit;
                    entity.Special_Case = model.SpecialCase;
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
