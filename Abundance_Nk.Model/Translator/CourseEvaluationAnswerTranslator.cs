using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseEvaluationAnswerTranslator: TranslatorBase<CourseEvaluationAnswer, COURSE_EVALUATION_ANSWER>
    {
        private StudentTranslator studentTranslator;
        private SessionTranslator sessionTranslator;
        private CourseTranslator courseTranslator;
        private SemesterTranslator semesterTranslator;
        private CourseEvaluationQuestionTranslator courseEvaluationQuestionTranslator;
        public CourseEvaluationAnswerTranslator()
        {
            studentTranslator = new StudentTranslator();
            semesterTranslator = new SemesterTranslator();
            sessionTranslator = new SessionTranslator();
            courseTranslator = new CourseTranslator();
            courseEvaluationQuestionTranslator = new CourseEvaluationQuestionTranslator();
        }
        public override CourseEvaluationAnswer TranslateToModel(COURSE_EVALUATION_ANSWER entity)
        {
            try
            {
                CourseEvaluationAnswer model = null;
                if (entity != null)
                {
                    model = new CourseEvaluationAnswer();
                    model.Id = entity.Id;
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.Score = entity.Score;
                    model.CourseEvaluationQuestion = courseEvaluationQuestionTranslator.Translate(entity.COURSE_EVALUATION_QUESTION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override COURSE_EVALUATION_ANSWER TranslateToEntity(CourseEvaluationAnswer model)
        {
            try
            {
                COURSE_EVALUATION_ANSWER entity = null;
                if (model != null)
                {
                    entity = new COURSE_EVALUATION_ANSWER();
                    entity.Id = model.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.Score = model.Score;
                    entity.Semester_Id  = model.Semester.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Question_Id = model.CourseEvaluationQuestion.Id;
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
