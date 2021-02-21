using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Abundance_Nk.Data;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    public class ResultComputationLogic
    {
        protected IRepository repository = new Repository();
       
        //Get Aggregate Sheet
        public List<Result> GetMaterSheetBy(SessionSemester sessionSemester, Level level, Programme programme,Department department)
        {
            List<Result> masterSheetList = null;
            try
            {
                 if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet was not selected ! Please select al the necessary inputs and try again.");
                }

                if (programme.Id == 1 || programme.Id == 2)
                {
                        masterSheetList = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == sessionSemester.Session.Id && x.Semester_Id == sessionSemester.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = sr.Course_Unit,
                                            SpecialCase = sr.Special_Case,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeName = sr.Programme_Name,
                                            LevelName = sr.Level_Name,
                                            Semestername = sr.Semester_Name,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            //GPA = GetGPA(sr.Person_Id,sr.Semester_Id,sr.Level_Id),
                                            //FirstSemesterGPA = GetGPA(sr.Person_Id,1,3),
                                            //SecondSemesterGPA = GetGPA(sr.Person_Id,1,3),
                                            //ThirdSemesterGPA = GetGPA(sr.Person_Id,1,4),
                                            //FourthSemesterGPA = GetGPA(sr.Person_Id,1,4),
                                            //Remark = GetRemark(sr.Person_Id,sr.Semester_Id,sr.Level_Id,sr.Session_Id,sr.Programme_Id,sr.Department_Id),

                                        }).ToList();
                    List<Result> masteResult = new List<Result>();
                    foreach (Result result in masterSheetList)
                    {
                        result.CGPA = GetCGPA(result.StudentId);
                        result.Remark = GetRemark(result.StudentId, sessionSemester.Semester.Id, result.LevelId, result.SessionId,programme.Id, department.Id);
                        masteResult.Add(result);
                    }
                    return masteResult;
                }
                else
                {
                        masterSheetList = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == sessionSemester.Session.Id && x.Semester_Id == sessionSemester.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = sr.Course_Unit,
                                            SpecialCase = sr.Special_Case,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeName = sr.Programme_Name,
                                            LevelName = sr.Level_Name,
                                            Semestername = sr.Semester_Name,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            GPA = GetGPA(sr.Person_Id,sr.Semester_Id,sr.Level_Id),
                                            FirstSemesterGPA = GetGPA(sr.Person_Id,1,3),
                                            SecondSemesterGPA = GetGPA(sr.Person_Id,1,3),
                                            ThirdSemesterGPA = GetGPA(sr.Person_Id,1,4),
                                            FourthSemesterGPA = GetGPA(sr.Person_Id,1,4),
                                            Remark = GetRemark(sr.Person_Id,sr.Semester_Id,sr.Level_Id,sr.Session_Id,sr.Programme_Id,sr.Department_Id),

                                        }).ToList();
                }

            }
            catch (Exception)
            {
                
                throw;
            }
            return masterSheetList;
        }

        private decimal GetGPA(long PersonId,int semesterId,int levelId)
        {
            decimal cgpa = 0;
            try
            {

              var totalWgp =   repository.GetBy<VW_STUDENT_RESULT_2>(a => a.Person_Id == PersonId && a.Semester_Id == semesterId && a.Level_Id == levelId).Sum(k => k.WGP);
              var totalUnit =   repository.GetBy<VW_STUDENT_RESULT_2>(a => a.Person_Id == PersonId && a.Semester_Id == semesterId && a.Level_Id == levelId).Sum(k => k.Course_Unit);
              if (totalWgp != 0 && totalUnit != 0)
              {
                cgpa = Convert.ToDecimal(totalWgp/totalUnit);
              }

            }
            catch (Exception)
            {
                
                throw;
            }
            return cgpa;
        }
        private decimal GetCGPA(long PersonId)
        {
            decimal cgpa = 0;
            try
            {
                int semesterCount = 0;
                decimal firstSemesterGPA = GetGPA(PersonId, 1, 1);
                decimal secondSemesterGPA = GetGPA(PersonId, 2, 1);
                decimal thirdSemesterGPA = GetGPA(PersonId, 1, 2);
                decimal fourthSemesterGPA = GetGPA(PersonId, 1, 2);
               
                if (firstSemesterGPA != 0)
                {
                    semesterCount=semesterCount+1;
                }
                if (secondSemesterGPA != 0)
                {
                    semesterCount=semesterCount+1;
                }
                if (thirdSemesterGPA != 0)
                {
                     semesterCount=semesterCount+1;
                }
                if (fourthSemesterGPA != 0)
                {
                     semesterCount=semesterCount+1;
                }
                cgpa = ((firstSemesterGPA + secondSemesterGPA + thirdSemesterGPA + fourthSemesterGPA)/semesterCount);
               

            }
            catch (Exception)
            {
                
                throw;
            }
            return cgpa;
        }
        private string GetRemark(long PersonId,int semesterId,int levelId,int SessionId,int ProgrammeId, int DepartmentId)
        {
            string remark = null;
            try
            {

                SessionSemester sessionSemester = new SessionSemester(){Id = 1,Session = new Session(){Id = SessionId}};  
                Level level = new Level(){Id=levelId};    
                Programme programme = new Programme(){Id=ProgrammeId}; 
                Department department = new Department(){Id = DepartmentId }; 
                Student student = new Student(){Id = PersonId};
                List<string> courseCodes = GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student);
                decimal CGPA = GetCGPA(PersonId);
                if (courseCodes.Count == 0 && CGPA != null)
                {
                    if (CGPA >= 3.5M && CGPA <= 4.0M)
                    {
                        remark = "RHL; PASSED: DISTICTION";
                    }
                    else if (CGPA >= 3.25M && CGPA <= 3.49M)
                    {
                        remark = "DHL; PASSED: UPPER CREDIT";
                    }
                    else if (CGPA >= 3.0M && CGPA < 3.25M)
                    {
                        remark = "PAS; PASSED: UPPER CREDIT";
                    }
                    else if (CGPA >= 2.5M && CGPA <= 2.99M)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";
                    }
                    else if (CGPA >= 2.0M && CGPA <= 2.49M)
                    {
                        remark = "PAS; PASSED: PASS";
                    }
                    else if (CGPA < 2.0M)
                    {
                        remark = "FAIL";
                    }
                }
                else
                {
                    remark = "CO-";
                    for (int i = 0; i < courseCodes.Count(); i++)
                    {
                        remark += ("|" + courseCodes[i]);
                    }
                }
               

            }
            catch(Exception )
            { 
            }
            return remark;
        }
       
        
        private List<string> GetFirstYearCarryOverCourses(SessionSemester sessionSemester, Level level, Programme programme, Department department, Student student)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationdetails = new List<CourseRegistrationDetail>();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<string> courseCodes =  new List<string>();
                if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                {
                    courseRegistrationdetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score + crd.Exam_Score) < (int)Grades.PassMark && crd.Special_Case == null);
                    if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == (int)Semesters.FirstSemester).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                    
                }

                return courseCodes;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
      

    }
}
