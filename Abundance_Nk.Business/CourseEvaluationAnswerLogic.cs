using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseEvaluationAnswerLogic: BusinessBaseLogic<CourseEvaluationAnswer,COURSE_EVALUATION_ANSWER>
    {
        public CourseEvaluationAnswerLogic()
        {
            translator = new CourseEvaluationAnswerTranslator();
        }

        public List<CourseEvaluationReport> GetCourseEvaluationReport(Programme programme, Department department, Level level, SessionSemester sessionSemester)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get this Report is not set! Please check your input criteria selection and try again.");
                }

                List<CourseEvaluationReport> evaluationScores = new List<CourseEvaluationReport>();
                List<CourseEvaluationReport> masterList = new List<CourseEvaluationReport>();

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                evaluationScores = (from sr in repository.GetBy<VW_COURSE_EVALUATION_SCORES>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                                    select new CourseEvaluationReport
                                    {
                                        Programme = sr.Programme_Name,
                                        Department = sr.Department_Name,
                                        Level = sr.Level_Name,
                                        Faculty = sr.Faculty_Name,
                                        Session = sr.Session_Name,
                                        Semester = sr.Semester_Name,
                                        CourseCode = sr.Course_Code,
                                        CourseName = sr.Course_Name,
                                        Score = sr.Score,
                                        LecturerName = sr.User_Name,
                                        PersonId = sr.Person_Id
                                    }).ToList();

                List<string> distinctCourseCodes = evaluationScores.Select(e => e.CourseCode).Distinct().ToList();

                StaffLogic staffLogic = new StaffLogic();

                for (int i = 0; i < distinctCourseCodes.Count; i++)
                {
                    string courseCode = distinctCourseCodes[i];
                    List<CourseEvaluationReport> evaluationCourse = evaluationScores.Where(e => e.CourseCode == courseCode).ToList();

                    CourseEvaluationReport firstEvaluationCourse = evaluationCourse[0];

                    Staff staff = staffLogic.GetModelBy(s => s.USER.User_Name == firstEvaluationCourse.LecturerName);

                    int studentCount = evaluationCourse.Select(p => p.PersonId).Distinct().Count();
                    long? score = evaluationCourse.Where(e => e.Score != null).Sum(e => e.Score);

                    for (int j = 0; j < evaluationCourse.Count; j++)
                    {
                        evaluationCourse[j].NumberOfStudent = studentCount;
                        evaluationCourse[j].Score = score;

                        CourseEvaluationReport evaluation = evaluationCourse[j];

                        if (staff != null)
                        {
                            evaluation.LecturerName = staff.FullName.ToUpper();
                        }

                        //masterList.Add(evaluation);
                    }

                    masterList.Add(evaluationCourse.FirstOrDefault());
                } 

                //for (int i = 0; i < evaluationScores.Count; i++)
                //{
                //    CourseEvaluationReport currentEvaluation = evaluationScores[i];
                //    StaffLogic staffLogic = new StaffLogic();
                //    Staff staff = staffLogic.GetModelBy(s => s.USER.User_Name == currentEvaluation.LecturerName);
                //    if (staff != null)
                //    {
                //        currentEvaluation.LecturerName = staff.FullName;
                //    }

                //    masterList.Add(currentEvaluation);
                //}

                return masterList.OrderBy(a => a.CourseCode).ToList();
                //return evaluationScores.OrderBy(a => a.CourseCode).ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(CourseEvaluationAnswer courseEvaluationAnswer)
        {
            try
            {
                Expression<Func<COURSE_EVALUATION_ANSWER, bool>> selector = c => c.Id == courseEvaluationAnswer.Id;
                COURSE_EVALUATION_ANSWER entity = GetEntityBy(selector);

                if (courseEvaluationAnswer.Course != null)
                {
                    entity.Course_Id = courseEvaluationAnswer.Course.Id;
                }
                if (courseEvaluationAnswer.Student != null)
                {
                    entity.Person_Id = courseEvaluationAnswer.Student.Id;
                }

                int modifiedRecordCount = Save();

                return modifiedRecordCount > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
