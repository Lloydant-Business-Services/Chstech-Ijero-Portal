using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseRegistrationDetailAuditTranslator: TranslatorBase<CourseRegistrationDetailAudit, STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT>
    {
        private CourseTranslator courseTranslator;
        private CourseModeTranslator courseModeTranslator;
        private SemesterTranslator semesterTranslator;
        private CourseRegistrationTranslator courseRegistrationTranslator;
        private CourseRegistrationDetailTranslator courseRegistrationDetailTranslator;
        private UserTranslator userTranslator;
        private StudentTranslator studentTranslator;

        public CourseRegistrationDetailAuditTranslator()
        {
            courseTranslator = new CourseTranslator();
            courseModeTranslator = new CourseModeTranslator();
            semesterTranslator = new SemesterTranslator();
            courseRegistrationTranslator = new CourseRegistrationTranslator();
            courseRegistrationDetailTranslator = new CourseRegistrationDetailTranslator();
            userTranslator = new UserTranslator();
            studentTranslator = new StudentTranslator();
        }
        public override CourseRegistrationDetailAudit TranslateToModel(STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT entity)
        {
            try
            {
                CourseRegistrationDetailAudit model = null;
                if (entity != null)
                {
                    model = new CourseRegistrationDetailAudit();
                    model.Id = entity.Student_Course_Registration_Detail_Audit_Id;
                    model.CourseRegistration = courseRegistrationTranslator.Translate(entity.STUDENT_COURSE_REGISTRATION);
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.Mode = courseModeTranslator.Translate(entity.COURSE_MODE);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.TestScore = entity.Test_Score;
                    model.ExamScore = entity.Exam_Score;
                    model.CourseUnit = entity.Course_Unit;
                    model.SpecialCase = entity.Special_Case;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;
                    model.User = userTranslator.Translate(entity.USER);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT TranslateToEntity(CourseRegistrationDetailAudit model)
        {
            try
            {
                STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT();
                    entity.Student_Course_Registration_Detail_Audit_Id = model.Id;
                    if (model.CourseRegistration != null)
                    {
                        entity.Student_Course_Registration_Id = model.CourseRegistration.Id;
                    }
                    if (model.CourseRegistrationDetail != null)
                    {
                        entity.Student_Course_Registration_Detail_Id = model.CourseRegistrationDetail.Id;
                    }
                    
                    entity.Course_Id = model.Course.Id;
                    entity.Course_Mode_Id = model.Mode.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Test_Score = model.TestScore;
                    entity.Exam_Score = model.ExamScore;
                    entity.Course_Unit = model.CourseUnit;
                    entity.Special_Case = model.SpecialCase;
                    entity.Action = model.Action;
                    entity.Time = model.Time;
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    if (model.User != null)
                    {
                        entity.User_Id = model.User.Id; 
                    }
                    if (model.Student != null)
                    {
                        entity.Person_Id = model.Student.Id;
                    }
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
