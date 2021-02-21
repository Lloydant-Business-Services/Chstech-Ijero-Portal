using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Transactions;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class StudentResultLogic : BusinessBaseLogic<StudentResult, STUDENT_RESULT>
    {
        private StudentResultDetailLogic studentResultDetailLogic;

        public StudentResultLogic()
        {
            translator = new StudentResultTranslator();
            studentResultDetailLogic = new StudentResultDetailLogic();
        }

        public List<StudentResult> GetBy(Level level, Programme programme, Department department, SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<STUDENT_RESULT, bool>> selector = sr => sr.Level_Id == level.Id && sr.Programme_Id == programme.Id && sr.Department_Id == department.Id && sr.Session_Semester_Id == sessionSemester.Id;
                return GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetTotalMaximumObtainableScore(Level level, Programme programme, Department department, SessionSemester sessionSemester)
        {
            try
            {
                //List<int?> result = (from sr in repository.GetBy<VW_MAXIMUM_OBTAINABLE_SCORE>(x => x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                //                     select sr.Maximum_Score_Obtainable.Value);


                Expression<Func<STUDENT_RESULT, bool>> selector = sr => sr.Level_Id == level.Id && sr.Programme_Id == programme.Id && sr.Department_Id == department.Id && sr.Session_Semester_Id == sessionSemester.Id;
                List<StudentResult> studentResults = GetModelsBy(selector);
                return studentResults.Sum(s => s.MaximumObtainableScore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Save(StudentResult resultHeader)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    if (resultHeader.Results != null && resultHeader.Results.Count > 0)
                    {
                        Add(resultHeader);
                        transaction.Complete();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StudentResult Add(StudentResult resultHeader, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                StudentResult newResultHeader = base.Create(resultHeader);
                if (newResultHeader == null || newResultHeader.Id == 0)
                {
                    throw new Exception("Result Header add opeartion failed! Please try again");
                }

                resultHeader.Id = newResultHeader.Id;
                //List<StudentResultDetail> results = SetHeader(resultHeader, newResultHeader);

                List<StudentResultDetail> results = SetHeader(resultHeader);
                int rowsAdded = studentResultDetailLogic.Create(results);
                if (rowsAdded == 0)
                {
                    throw new Exception("Result Header was succesfully added, but Result Detail Add operation failed! Please try again");
                }

                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (courseRegistrationDetailLogic.UpdateCourseRegistrationScore(results, courseRegistrationDetailAudit))
                {
                    //return newResultHeader;
                    return resultHeader;
                }
                else
                {
                    throw new Exception("Registered course failed on update! Please try again");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<StudentResultDetail> SetHeader(StudentResult resultHeader)
        {
            try
            {
                foreach (StudentResultDetail result in resultHeader.Results)
                {
                    result.Header = resultHeader;
                    //result.Header.Id = newResultHeader.Id;
                }

                return resultHeader.Results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetTranscriptBy(Student student)
        {
            try
            {
                if (student == null || student.Id <= 0)
                {
                    throw new Exception("Student not set! Please select student and try again.");
                }

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Person_Id == student.Id)
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
                                            FacultyName = sr.Faculty_Name,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,

                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            ProgrammeName = sr.Programme_Name,
                                            DepartmentName = sr.Department_Name,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id,
                                            SemesterId = sr.Semester_Id,
                                            LevelId = sr.Level_Id,
                                            ProgrammeId = sr.Programme_Id,
                                            DepartmentId = sr.Department_Id
                                        }).ToList();

                List<int> distinctSessions = results.Select(s => s.SessionId).Distinct().ToList();

                List<int> distinctLevels = results.Select(s => s.LevelId).Distinct().ToList();

                Semester firstSemester = new Semester() { Id = 1 };
                Semester secondSemester = new Semester() { Id = 2 };

                decimal firstSemesterCGPA = 0M;
                decimal secondSemesterCGPA = 0M;

                string remark = "";

                SessionSemester ss = null;

                for (int i = 0; i < distinctSessions.Count; i++)
                {
                    int currentSessonId = distinctSessions[i];

                    List<Result> currentSessionFirstSemesterResults = results.Where(s => s.SessionId == currentSessonId && s.SemesterId == firstSemester.Id).ToList();
                    if (currentSessionFirstSemesterResults.Count > 0)
                    {
                        firstSemesterCGPA = Math.Round(Convert.ToDecimal(currentSessionFirstSemesterResults.Sum(s => s.GPCU) / currentSessionFirstSemesterResults.Sum(s => s.CourseUnit)), 2);

                        List<string> carryOverCourses = new List<string>();

                        int levelId = results.Where(l => l.SessionId == currentSessonId).LastOrDefault().LevelId;
                        if (levelId == (int)Levels.NDII || levelId == (int)Levels.HNDII)
                        {
                            ss = new SessionSemester() { Session = new Session() { Id = currentSessonId }, Semester = firstSemester };

                            carryOverCourses = GetSecondYearCarryOverCourses(ss, new Level() { Id = levelId }, new Programme() { Id = results.LastOrDefault().ProgrammeId ?? 0 }, new Department() { Id = results.LastOrDefault().DepartmentId }, student);

                        }
                        else
                        {
                            ss = new SessionSemester() { Session = new Session() { Id = currentSessonId }, Semester = firstSemester };

                            carryOverCourses = GetFirstYearCarryOverCourses(ss, new Level() { Id = levelId }, new Programme() { Id = results.LastOrDefault().ProgrammeId ?? 0 }, new Department() { Id = results.LastOrDefault().DepartmentId }, student);

                        }

                        remark = GetGraduationStatus(firstSemesterCGPA, carryOverCourses);

                        for (int j = 0; j < currentSessionFirstSemesterResults.Count; j++)
                        {
                            currentSessionFirstSemesterResults[j].Remark = remark;
                        }
                    }

                    List<Result> currentSessionSecondSemesterResults = results.Where(s => s.SessionId == currentSessonId && s.SemesterId == secondSemester.Id).ToList();
                    if (currentSessionSecondSemesterResults.Count > 0)
                    {
                        secondSemesterCGPA = Math.Round(Convert.ToDecimal(currentSessionSecondSemesterResults.Sum(s => s.GPCU) / currentSessionSecondSemesterResults.Sum(s => s.CourseUnit)), 2);

                        List<string> carryOverCourses = new List<string>();

                        int levelId = results.Where(l => l.SessionId == currentSessonId).LastOrDefault().LevelId;
                        if (levelId == (int)Levels.NDII || levelId == (int)Levels.HNDII)
                        {
                            ss = new SessionSemester() { Session = new Session() { Id = currentSessonId }, Semester = secondSemester };

                            carryOverCourses = GetSecondYearCarryOverCourses(ss, new Level() { Id = levelId }, new Programme() { Id = results.LastOrDefault().ProgrammeId ?? 0 }, new Department() { Id = results.LastOrDefault().DepartmentId }, student);

                        }
                        else
                        {
                            ss = new SessionSemester() { Session = new Session() { Id = currentSessonId }, Semester = secondSemester };

                            carryOverCourses = GetFirstYearCarryOverCourses(ss, new Level() { Id = levelId }, new Programme() { Id = results.LastOrDefault().ProgrammeId ?? 0 }, new Department() { Id = results.LastOrDefault().DepartmentId }, student);

                        }

                        remark = GetGraduationStatus(secondSemesterCGPA, carryOverCourses);

                        for (int j = 0; j < currentSessionSecondSemesterResults.Count; j++)
                        {
                            currentSessionSecondSemesterResults[j].Remark = remark;
                        }
                    }
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StatementOfResultSummary> GetStatementOfResultSummaryBy(SessionSemester sessionSemester, Level level, Programme programme, Department department, Student student)
        {
            try
            {
                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Master Result Sheet not set! Please check your input criteria selection and try again.");
                }


                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_SUMMARY>(x => x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Person_Id == student.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseUnit = (int)sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Session_Semester_Sequence_Number,
                                            GradePoint = sr.Grade_Point,
                                            GPA = sr.GPA,
                                            WGP = sr.WGP,
                                            UnitPassed = sr.Unit_Passed,
                                            UnitOutstanding = sr.Unit_Outstanding,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            SemesterId = sr.Semester_Id
                                        }).ToList();


                List<StatementOfResultSummary> resultSummaries = new List<StatementOfResultSummary>();
                if (results != null && results.Count > 0)
                {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                    Result currentSemesterResult = results.Where(r => r.SessionSemesterId == sessionSemester.Id).SingleOrDefault();
                    Result previousSemesterResult = new Result();

                    if (ss.Semester != null && ss.Semester.Id == 2)
                    {
                        previousSemesterResult = results.Where(r => r.SemesterId == 1).LastOrDefault();
                    }
                    else
                    {
                        previousSemesterResult = null;
                    }

                    StatementOfResultSummary unitsAttempted = new StatementOfResultSummary();
                    StatementOfResultSummary wgp = new StatementOfResultSummary();
                    StatementOfResultSummary gpa = new StatementOfResultSummary();
                    StatementOfResultSummary unitPassed = new StatementOfResultSummary();
                    StatementOfResultSummary unitsOutstanding = new StatementOfResultSummary();

                    unitsAttempted.Item = "UNITS ATTEMPTED";
                    wgp.Item = "WEIGHT GRADE POINTS";
                    gpa.Item = "GRADE POINT AVERAGE";
                    unitPassed.Item = "UNITS PASSED";
                    unitsOutstanding.Item = "UNITS OUTSTANDING";

                    if (previousSemesterResult != null)
                    {
                        unitsAttempted.PreviousSemester = previousSemesterResult.CourseUnit.ToString();
                        wgp.PreviousSemester = previousSemesterResult.WGP.ToString();
                        previousSemesterResult.GPA = previousSemesterResult.GPA ?? 0;
                        gpa.PreviousSemester = Math.Round((decimal)previousSemesterResult.GPA, 2).ToString();

                        unitPassed.PreviousSemester = previousSemesterResult.UnitPassed.ToString();
                        unitsOutstanding.PreviousSemester = previousSemesterResult.UnitOutstanding.ToString();
                    }

                    if (currentSemesterResult != null)
                    {
                        unitsAttempted.CurrentSemester = currentSemesterResult.CourseUnit.ToString();
                        wgp.CurrentSemester = currentSemesterResult.WGP.ToString();

                        gpa.CurrentSemester = Math.Round((decimal)currentSemesterResult.GPA, 2).ToString();
                        unitPassed.CurrentSemester = currentSemesterResult.UnitPassed.ToString();
                        unitsOutstanding.CurrentSemester = currentSemesterResult.UnitOutstanding.ToString();
                    }

                    unitsAttempted.AllSemester = results.Sum(r => r.CourseUnit).ToString();
                    wgp.AllSemester = results.Sum(r => r.WGP).ToString();

                    gpa.AllSemester = Math.Round((decimal)results.Sum(r => r.GPA), 2).ToString();
                    unitPassed.AllSemester = results.Sum(r => r.UnitPassed).ToString();
                    unitsOutstanding.AllSemester = results.Sum(r => r.UnitOutstanding).ToString();

                    resultSummaries.Add(unitsAttempted);
                    resultSummaries.Add(wgp);
                    resultSummaries.Add(gpa);
                    resultSummaries.Add(unitPassed);
                    resultSummaries.Add(unitsOutstanding);
                }

                return resultSummaries;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private decimal getCGPA(long studentId, int levelId, int departmentId, int programmeId, int semesterId, int sessionId)
        {
            Result overallResult = new Result();
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();

                Session session = new Session() { Id = sessionId };
                Programme programme = new Programme() { Id = programmeId };
                Department department = new Department() { Id = departmentId };
                Level level = new Level() { Id = levelId };

                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == studentId);

                if (levelId == 1 || levelId == 3)
                {
                    if (semesterId == (int)Semesters.FirstSemester)
                    {
                        Semester firstSemester = new Semester() { Id = (int)Semesters.FirstSemester };
                        List<Result> result = null;
                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        else
                        {
                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        List<Result> modifiedResultList = new List<Result>();

                        int totalSemesterCourseUnit = 0;
                        foreach (Result resultItem in result)
                        {
                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {
                                resultItem.GPCU = 0;
                                if (totalSemesterCourseUnit == 0)
                                {
                                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                            }
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }

                            modifiedResultList.Add(resultItem);
                        }

                        decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                        int? firstSemesterTotalSemesterCourseUnit = 0;
                        overallResult = modifiedResultList.FirstOrDefault();
                        firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        decimal? firstSemesterGPA = 0M;
                        if (firstSemesterGPCUSum != null && firstSemesterGPCUSum > 0)
                        {
                            firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                        }

                        if (firstSemesterGPA != null && firstSemesterGPA > 0)
                        {
                            overallResult.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                        }
                        if (firstSemesterGPA != null && firstSemesterGPA > 0)
                        {
                            overallResult.CGPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                        }
                    }
                    else
                    {
                        List<Result> result = null;
                        Semester firstSemester = new Semester() { Id = (int)Semesters.FirstSemester };
                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        else
                        {
                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        List<Result> firstSemesterModifiedResultList = new List<Result>();

                        int totalFirstSemesterCourseUnit = 0;
                        foreach (Result resultItem in result)
                        {
                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {
                                resultItem.GPCU = 0;
                                if (totalFirstSemesterCourseUnit == 0)
                                {
                                    totalFirstSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalFirstSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }

                            }
                            if (totalFirstSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                            }
                            firstSemesterModifiedResultList.Add(resultItem);
                        }

                        decimal? firstSemesterGPCUSum = firstSemesterModifiedResultList.Sum(p => p.GPCU);
                        int? firstSemesterTotalSemesterCourseUnit = 0;
                        overallResult = firstSemesterModifiedResultList.FirstOrDefault();
                        firstSemesterTotalSemesterCourseUnit = firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        decimal? firstSemesterGPA = 0M;
                        if (firstSemesterGPCUSum != null && firstSemesterGPCUSum > 0)
                        {
                            firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                        }

                        if (firstSemesterGPA != null && firstSemesterGPA > 0)
                        {
                            overallResult.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                        }

                        Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                        List<Result> secondSemesterResult = null;
                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                        }
                        else
                        {
                            secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                        }
                        List<Result> secondSemesterModifiedResultList = new List<Result>();

                        int totalSecondSemesterCourseUnit = 0;
                        foreach (Result resultItem in secondSemesterResult)
                        {

                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {

                                resultItem.GPCU = 0;
                                if (totalSecondSemesterCourseUnit == 0)
                                {
                                    totalSecondSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalSecondSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }

                            }
                            if (totalSecondSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                            }
                            secondSemesterModifiedResultList.Add(resultItem);
                        }
                        decimal? secondSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                        Result secondSemesterStudentResult = secondSemesterModifiedResultList.FirstOrDefault();
                        overallResult = secondSemesterStudentResult;
                        if (secondSemesterGPCUSum != null && secondSemesterGPCUSum > 0)
                        {
                            overallResult.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / secondSemesterStudentResult.TotalSemesterCourseUnit), 2);
                        }
                        if (firstSemesterGPCUSum > 0 || secondSemesterGPCUSum > 0)
                        {
                            overallResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum) / (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) + firstSemesterTotalSemesterCourseUnit)), 2);
                        }
                    }
                }
                else
                {
                    decimal firstYearFirstSemesterGPCUSum = 0;
                    int firstYearFirstSemesterTotalCourseUnit = 0;
                    decimal firstYearSecondSemesterGPCUSum = 0;
                    int firstYearSecondSemesterTotalCourseUnit = 0;
                    decimal secondYearFirstSemesterGPCUSum = 0;
                    int secondYearFirstSemesterTotalCourseUnit = 0;
                    decimal secondYearSecondSemesterGPCUSum = 0;
                    int secondYearSecondSemesterTotalCourseUnit = 0;

                    Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(level.Id, department, programme, studentCheck);
                    Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(level.Id, department, programme, studentCheck);
                    if (semesterId == 1)
                    {
                        List<Result> result = null;
                        Semester firstSemester = new Semester() { Id = 1 };

                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        else
                        {
                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;
                        foreach (Result resultItem in result)
                        {

                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {

                                resultItem.GPCU = 0;
                                if (totalSemesterCourseUnit == 0)
                                {
                                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }

                            }
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }
                        Result firstYearFirstSemesterResult = new Result();
                        decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                        int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        firstYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                        decimal? firstSemesterGPA = 0M;
                        if (firstSemesterGPCUSum != null && firstSemesterGPCUSum > 0)
                        {
                            firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }

                        overallResult = modifiedResultList.FirstOrDefault();
                        if (firstSemesterGPA != null && firstSemesterGPA > 0)
                        {
                            overallResult.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                        }
                        if (firstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                        {
                            if ((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) > 0 && firstYearSecondSemester.TotalSemesterCourseUnit != null && firstYearFirstSemester.TotalSemesterCourseUnit != null && secondYearfirstSemesterTotalSemesterCourseUnit != null)
                            {
                                firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                                firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                                overallResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                            }
                        }
                    }
                    else if (semesterId == 2)
                    {

                        List<Result> result = null;
                        Semester firstSemester = new Semester() { Id = 1 };

                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        else
                        {
                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        }
                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;
                        foreach (Result resultItem in result)
                        {

                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {

                                resultItem.GPCU = 0;
                                if (totalSemesterCourseUnit == 0)
                                {
                                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }

                            }
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }
                        Result secondYearFirstSemesterResult = new Result();
                        decimal? secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                        int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                        decimal? firstSemesterGPA = 0M;
                        if (secondYearfirstSemesterGPCUSum != null && secondYearfirstSemesterGPCUSum > 0)
                        {
                            firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }



                        //Second semester second year

                        List<Result> secondSemesterResult = null;



                        Semester secondSemester = new Semester() { Id = 2 };

                        if (studentCheck.Activated == true || studentCheck.Activated == null)
                        {
                            secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                        }
                        else
                        {
                            secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                        }
                        List<Result> secondSemesterModifiedResultList = new List<Result>();
                        int totalSecondSemesterCourseUnit = 0;
                        foreach (Result resultItem in secondSemesterResult)
                        {

                            decimal WGP = 0;

                            if (resultItem.SpecialCase != null)
                            {

                                resultItem.GPCU = 0;
                                if (totalSecondSemesterCourseUnit == 0)
                                {
                                    totalSecondSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }
                                else
                                {
                                    totalSecondSemesterCourseUnit -= resultItem.CourseUnit;
                                    resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    resultItem.Grade = "-";
                                }

                            }
                            if (totalSecondSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                            }
                            secondSemesterModifiedResultList.Add(resultItem);
                        }
                        Result secondYearSecondtSemesterResult = new Result();
                        decimal? secondYearSecondtSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                        int? secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                        secondYearSecondSemesterTotalSemesterCourseUnit = secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                        secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                        decimal? secondYearSecondSmesterGPA = 0M;
                        if (secondYearSecondtSemesterGPCUSum != null && secondYearSecondtSemesterGPCUSum > 0)
                        {
                            secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                        }

                        overallResult = secondSemesterModifiedResultList.FirstOrDefault();
                        if (secondYearSecondSmesterGPA != null && secondYearSecondSmesterGPA > 0)
                        {
                            overallResult.GPA = Decimal.Round((decimal)secondYearSecondSmesterGPA, 2);
                        }
                        if (secondYearfirstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                        {
                            firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                            firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                            overallResult.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Convert.ToDecimal(overallResult.CGPA);
        }
        public Result GetFirstYearSecondSemesterResultInfo(int levelId, Department department, Programme programme, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                Semester semester = new Semester() { Id = 2 };
                Level level = null;
                if (levelId == 2)
                {
                    level = new Level() { Id = 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == studentCheck.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();

                // if student level is null create it for the student

                if (studentCheck.Activated == true || studentCheck.Activated == null)
                {
                    result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, studentCheck, semester, studentLevel.Programme);
                }
                else
                {
                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, studentCheck, semester, studentLevel.Programme);
                }
                List<Result> modifiedResultList = new List<Result>();
                int totalSemesterCourseUnit = 0;
                foreach (Result resultItem in result)
                {

                    decimal WGP = 0;

                    if (resultItem.SpecialCase != null)
                    {

                        resultItem.GPCU = 0;
                        if (totalSemesterCourseUnit == 0)
                        {
                            totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }
                        else
                        {
                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }

                    }
                    if (totalSemesterCourseUnit > 0)
                    {
                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    }
                    modifiedResultList.Add(resultItem);
                }
                Result firstYearFirstSemesterResult = new Result();
                decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                int? firstSemesterTotalSemesterCourseUnit = 0;
                firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                return firstYearFirstSemesterResult;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public Result GetFirstYearFirstSemesterResultInfo(int levelId, Department department, Programme programme, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);


                Semester semester = new Semester() { Id = 1 };
                Level level = null;
                if (levelId == 2)
                {
                    level = new Level() { Id = 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                //StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == studentCheck.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == studentCheck.Id && p.Level_Id == levelId && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                if (studentCheck.Activated == true || studentCheck.Activated == null)
                {
                    result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, studentCheck, semester, studentLevel.Programme);
                }
                else
                {
                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, studentCheck, semester, studentLevel.Programme);
                }
                List<Result> modifiedResultList = new List<Result>();
                int totalSemesterCourseUnit = 0;
                foreach (Result resultItem in result)
                {

                    decimal WGP = 0;

                    if (resultItem.SpecialCase != null)
                    {

                        resultItem.GPCU = 0;
                        if (totalSemesterCourseUnit == 0)
                        {
                            totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }
                        else
                        {
                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }

                    }
                    if (totalSemesterCourseUnit > 0)
                    {
                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    }
                    modifiedResultList.Add(resultItem);
                }
                Result firstYearFirstSemesterResult = new Result();
                decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                int? firstSemesterTotalSemesterCourseUnit = 0;
                firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                return firstYearFirstSemesterResult;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Result> GetProcessedResutBy(Session session, Semester semester, Level level, Department department, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt >= 2015)
                {

                    List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && p.Activated != false)
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
                                                FacultyName = sr.Faculty_Name,
                                                TestScore = sr.Test_Score,
                                                ExamScore = sr.Exam_Score,
                                                Score = sr.Total_Score,
                                                Grade = sr.Grade,
                                                GradePoint = sr.Grade_Point,
                                                Email = sr.Email,
                                                SpecialCase = sr.Special_Case,
                                                Address = sr.Contact_Address,
                                                MobilePhone = sr.Mobile_Phone,
                                                PassportUrl = sr.Image_File_Url,
                                                GPCU = sr.Grade_Point * sr.Course_Unit,
                                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit ?? 0,
                                                Student_Type_Id = sr.Student_Type_Id,
                                                SessionName = sr.Session_Name,
                                                Semestername = sr.Semester_Name,
                                                LevelName = sr.Level_Name,
                                                WGP = sr.WGP,
                                                Activated = sr.Activated,
                                                Reason = sr.Reason,
                                                RejectCategory = sr.Reject_Category,
                                                firstname_middle = sr.Othernames,
                                                ProgrammeName = sr.Programme_Name,
                                                Surname = sr.Last_Name,
                                                Firstname = sr.First_Name,
                                                Othername = sr.Other_Name,
                                                TotalScore = sr.Total_Score,
                                                SessionSemesterId = sr.Session_Semester_Id,
                                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            }).ToList();

                    return results;
                }
                else
                {

                    List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && p.Activated != false)
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
                                                FacultyName = sr.Faculty_Name,
                                                TestScore = sr.Test_Score,
                                                ExamScore = sr.Exam_Score,
                                                Score = sr.Total_Score,
                                                Grade = sr.Grade,
                                                GradePoint = sr.Grade_Point,
                                                Email = sr.Email,
                                                SpecialCase = sr.Special_Case,
                                                Address = sr.Contact_Address,
                                                MobilePhone = sr.Mobile_Phone,
                                                PassportUrl = sr.Image_File_Url,
                                                GPCU = sr.Grade_Point * sr.Course_Unit,
                                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                                Student_Type_Id = sr.Student_Type_Id,
                                                SessionName = sr.Session_Name,
                                                Semestername = sr.Semester_Name,
                                                LevelName = sr.Level_Name,
                                                WGP = sr.WGP,
                                                Activated = sr.Activated,
                                                Reason = sr.Reason,
                                                RejectCategory = sr.Reject_Category,
                                                firstname_middle = sr.Othernames,
                                                ProgrammeName = sr.Programme_Name,
                                                Surname = sr.Last_Name,
                                                Firstname = sr.First_Name,
                                                Othername = sr.Other_Name,
                                                TotalScore = sr.Total_Score,
                                                SessionSemesterId = sr.Session_Semester_Id,
                                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            }).ToList();

                    return results;
                }




            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public List<Result> GetStudentProcessedResultBy(Session session, Level level, Department department, Student student, Semester semester, Programme programme)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Person_Id == student.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && (p.Activated == true))
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
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();

                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentProcessedDepartmentOptionResultBy(Session session, Level level, Department department, Student student, Semester semester, Programme programme, DepartmentOption option)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(p => p.Person_Id == student.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Department_Option_Id==option.Id && p.Grade_Point != null)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = (int)sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();

                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentPreviousProcessedResultBy(Session session, Level level, Department department, Student student, Semester semester, Programme programme)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0 || session == null || session.Id <= 0 || semester == null || semester.Id<=0 || student==null || student.Id<=0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Programme_Id == programme.Id && p.Person_Id==student.Id && p.Semester_Id==semester.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && (p.Activated == true))
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            PreviousCourseUnit = sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            PreviousTestScore = sr.Test_Score,
                                            PreviousExamScore = sr.Exam_Score,
                                            PreviousScore = sr.Total_Score,
                                            PreviousGrade = sr.Grade,
                                            PreviousGradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            PreviousGPCU = sr.Grade_Point * sr.Course_Unit,
                                            PreviousTotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            PreviousWGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            PreviousTotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();

                if (results.Count > 0)
                {

                    decimal? GPCUSum = results.Sum(x => x.PreviousGPCU);
                    decimal? TWGP = results.Sum(x => x.PreviousWGP);
                    decimal? TotalUnit = results.FirstOrDefault().PreviousTotalSemesterCourseUnit;
                    int? UnitPassed = results.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                    int? UnitOutStanding = results.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                    results.FirstOrDefault().PreviousGPCU = GPCUSum;
                    results.FirstOrDefault().UnitPassed = UnitPassed;
                    results.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                    results.FirstOrDefault().PreviousWGP = TWGP;
                    results.FirstOrDefault().PreviousGPA = (GPCUSum / TotalUnit);

                }
                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentAllPreviousProcessedResultBy(Session session, Level level, Department department, Programme programme)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0 || session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && (p.Activated == true))
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            PreviousCourseUnit = sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            PreviousTestScore = sr.Test_Score,
                                            PreviousExamScore = sr.Exam_Score,
                                            PreviousScore = sr.Total_Score,
                                            PreviousGrade = sr.Grade,
                                            PreviousGradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            PreviousGPCU = sr.Grade_Point * sr.Course_Unit,
                                            PreviousTotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            PreviousWGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            PreviousTotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id,
                                            SemesterId = sr.Semester_Id
                                        }).ToList();


                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentAllPreviousProcessedDepartmentOptionResultBy(Session session, Level level, Department department, Programme programme, DepartmentOption departmentOption)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0 || session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && (p.Activated == true) && p.Department_Option_Id == departmentOption.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            PreviousCourseUnit = (int)sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            PreviousTestScore = sr.Test_Score,
                                            PreviousExamScore = sr.Exam_Score,
                                            PreviousScore = sr.Total_Score,
                                            PreviousGrade = sr.Grade,
                                            PreviousGradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            PreviousGPCU = sr.Grade_Point * sr.Course_Unit,
                                            PreviousTotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            PreviousWGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            PreviousTotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id,
                                            SemesterId = sr.Semester_Id
                                        }).ToList();


                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentPreviousProcessedDepartmentOptionResultBy(Session session, Level level, Department department, Student student, Semester semester, Programme programme,DepartmentOption departmentOption)
        {
            try
            {

                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(p => p.Person_Id == student.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Department_Option_Id==departmentOption.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            PreviousCourseUnit = (int)sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            PreviousTestScore = sr.Test_Score,
                                            PreviousExamScore = sr.Exam_Score,
                                            PreviousScore = sr.Total_Score,
                                            PreviousGrade = sr.Grade,
                                            PreviousGradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            PreviousGPCU = sr.Grade_Point * sr.Course_Unit,
                                            PreviousTotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            PreviousWGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            PreviousTotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();
                if (results.Count > 0)
                {
                    decimal? GPCUSum = results.Sum(x => x.PreviousGPCU);
                    decimal? TWGP = results.Sum(x => x.PreviousWGP);
                    int? UnitPassed = results.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                    int? UnitOutStanding = results.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                    results.FirstOrDefault().PreviousGPCU = GPCUSum;
                    results.FirstOrDefault().UnitPassed = UnitPassed;
                    results.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                    results.FirstOrDefault().PreviousWGP = TWGP;

                }

                return results;


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetDeactivatedProcessedResutBy(Session session, Semester semester, Level level, Department department, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);


                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && p.Activated == false)
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
                                            FacultyName = sr.Faculty_Name,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            SpecialCase = sr.Special_Case,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                        }).ToList();

                return results;



            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetDeactivatedStudentProcessedResultBy(Session session, Level level, Department department, Student student, Semester semester, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Person_Id == student.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && p.Activated == false)
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
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                        }).ToList();

                return results;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentResultBy(SessionSemester sessionSemester, Level level, Programme programme, Department department, Student student)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Person_Id == student.Id)
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
                                            FacultyName = sr.Faculty_Name,
                                            ProgrammeName = sr.Programme_Name,
                                            DepartmentName = sr.Department_Name,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            LevelName = sr.Level_Name,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,

                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,

                                        }).ToList();

                List<string> carryOverCourses = new List<string>();
                if (level.Id == (int)Levels.NDII || level.Id == (int)Levels.HNDII)
                {
                    carryOverCourses = GetSecondYearCarryOverCourses(ss, level, programme, department, student);
                }
                else
                {
                    carryOverCourses = GetFirstYearCarryOverCourses(ss, level, programme, department, student);
                }

                decimal CGPA = getCGPA(student.Id, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);

                string remark = GetGraduationStatus(CGPA, carryOverCourses);

                for (int i = 0; i < results.Count; i++)
                {
                    results[i].CGPA = CGPA;
                    results[i].Identifier = identifier;
                    results[i].Remark = remark;
                }

                return results;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetStudentResultByCourse(SessionSemester sessionSemester, Level level, Programme programme, Department department, Course course)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || course == null || course.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Course_Id == course.Id)
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
                                            FacultyName = sr.Faculty_Name,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,

                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                        }).ToList();

                return results;




            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetMaterSheetDetailsBy(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                           }).ToList();

                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                List<Result> masterSheetResult = new List<Result>();
                for (int i = 0; i < results.Count; i++)
                {
                    Result resultItem = results[i];

                    resultItem.Identifier = identifier;
                    Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                    masterSheetResult.Add(result);
                }

                for (int i = 0; i < masterSheetResult.Count; i++)
                {
                    Result result = masterSheetResult[i];

                    List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                    for (int j = 0; j < studentResults.Count; j++)
                    {
                        Result resultItem = studentResults[j];

                        resultItem.Identifier = identifier;
                        resultItem.CGPA = result.CGPA;
                        resultItem.Remark = result.Remark;
                        resultItem.GPA = result.GPA;
                    }
                }

                for (int i = 0; i < results.Count; i++)
                {
                    results[i].LevelName = levels;
                }

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetMaterSheetDetailsByMode(SessionSemester sessionSemester, Level level, Programme programme, Department department, CourseMode courseMode)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                if (courseMode == null)
                {
                    if (ss.Session.Id == (int)Sessions._20152016)
                    {
                        results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                                       SessionName = sr.Session_Name
                                   }).ToList();

                    }
                    else
                    {
                        if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                        {
                            results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null) && x.Course_Mode_Id == (int)CourseModes.FirstAttempt)
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
                                           SessionName = sr.Session_Name
                                       }).ToList();

                            //List<Result> resultList = new List<Result>();

                            //for (int i = 0; i < results.Count; i++)
                            //{
                            //    if (results[i].MatricNumber.Contains("/16/"))
                            //    {
                            //        resultList.Add(results[i]);
                            //    }
                            //    else
                            //    {
                            //        //Do Nothing
                            //    }
                            //}

                            //results = new List<Result>();
                            //results = resultList;
                        }
                        else
                        {
                            results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                                           SessionName = sr.Session_Name
                                       }).ToList();

                            List<Result> resultList = new List<Result>();

                            for (int i = 0; i < results.Count; i++)
                            {
                                if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                                {
                                    //Do Nothing
                                }
                                else
                                {
                                    resultList.Add(results[i]);
                                }
                            }

                            results = new List<Result>();
                            results = resultList;
                        }
                    }


                    //sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    //List<Result> masterSheetResult = new List<Result>();
                    //for (int i = 0; i < results.Count; i++)
                    //{
                    //    Result resultItem = results[i];
                    //    resultItem.Identifier = identifier;
                    //    TotalUnitsOmitted = 0;
                    //    Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                    //    result.UnitOutstanding = TotalUnitsOmitted;

                    //    masterSheetResult.Add(result);
                    //   // masterSheetResult.Add(resultItem);
                    //}

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    List<long> students = results.Select(s => s.StudentId).Distinct().ToList();

                    List<Result> masterSheetResult = new List<Result>();

                    for (int i = 0; i < students.Count; i++)
                    {
                        //Result resultItem = results[i];
                        //resultItem.Identifier = identifier;
                        long studentId = students[i]; 
                         TotalUnitsOmitted = 0;
                        //Result result = ViewProcessedStudentResult(studentId, sessionSemester, level, programme, department);
                        Result result = ViewProcessedStudentResultForAggregate(studentId, sessionSemester, level, programme, department);
                        result.UnitOutstanding = TotalUnitsOmitted;

                        AssignAndAddToMasterSheet(identifier, result, results.Where(s => s.StudentId == studentId).ToList(), masterSheetResult);

                        //masterSheetResult.Add(result);
                    }

                    StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
                    List<long> extraYear = extraYearLogic.GetEntitiesBy(e => e.Session_Id == ss.Session.Id).Select(e => e.Person_Id).ToList();

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];

                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];

                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                            resultItem.UnitOutstanding = result.UnitOutstanding;

                            resultItem.SessionId = ss.Session.Id;

                            int totalSemesterCourseUnit = 0;
                            CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            //if (totalSemesterCourseUnit > 0)
                            //{
                            //    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            //}
                            if (extraYear.Contains(resultItem.StudentId))
                            {
                                resultItem.Remark = "";
                            }
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Extensive";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.FirstAttempt)
                {

                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null) && x.Course_Mode_Id == courseMode.Id)
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
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    if (level.Id == (int)Levels.HNDI || level.Id == (int)Levels.NDI)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].MatricNumber.Contains("/16/"))
                            {
                                resultList.Add(results[i]);
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].MatricNumber.Contains("/15/"))
                            {
                                resultList.Add(results[i]);
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];

                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];

                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];

                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "First Attempt";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.CarryOver)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null) && x.Course_Mode_Id == courseMode.Id)
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
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains("/15/"))
                        {
                            resultList.Add(results[i]);
                        }
                        else
                        {
                            //Do Nothing
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];

                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];

                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];

                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Carry Over";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.ExtraYear)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains("/15/") || results[i].MatricNumber.Contains("/16/"))
                        {
                            //Do Nothing
                        }
                        else
                        {
                            resultList.Add(results[i]);
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];

                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];

                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];

                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Extra Year";
                    }
                }

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AssignAndAddToMasterSheet(string identifier, Result result, List<Result> results, List<Result> masterSheetResult)
        {
            try
            {
                if (result != null)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].CGPA = result.CGPA;
                        //results[i].FirstSemesterGPA = result.FirstSemesterGPA;
                        results[i].GPA = result.GPA;
                        //results[i].GPCU = result.GPCU;
                        results[i].Identifier = identifier;
                        results[i].TotalSemesterCourseUnit = result.TotalSemesterCourseUnit;
                        results[i].Remark = result.Remark;
                        results[i].UnitOutstanding = result.UnitOutstanding;
                        results[i].UnitPassed = result.UnitPassed;

                        masterSheetResult.Add(results[i]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetMaterSheetDetailsByOptions(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || departmentOption == null || departmentOption.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;




                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id == departmentOption.Id && (x.Activated != false || x.Activated == null))
                           select new Result
                           {
                               StudentId = sr.Person_Id,
                               Sex = sr.Sex_Name,
                               Name = sr.Name,
                               MatricNumber = sr.Matric_Number,
                               CourseId = sr.Course_Id,
                               CourseCode = sr.Course_Code,
                               CourseName = sr.Course_Name,
                               CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                           }).ToList();



                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                //  List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList(); 
                List<Result> masterSheetResult = new List<Result>();
                for (int i = 0; i < results.Count; i++)
                {
                    Result resultItem = results[i];
                    resultItem.Identifier = identifier;
                    Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                    masterSheetResult.Add(result);
                }
                //foreach (Result resultItem in results)
                //{
                //    resultItem.Identifier = identifier;
                //    Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                //    masterSheetResult.Add(result);
                //}

                for (int i = 0; i < masterSheetResult.Count; i++)
                {
                    Result result = masterSheetResult[i];
                    List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                    for (int j = 0; j < studentResults.Count; j++)
                    {
                        Result resultItem = studentResults[j];
                        resultItem.Identifier = identifier;
                        resultItem.CGPA = result.CGPA;
                        resultItem.Remark = result.Remark;
                        resultItem.GPA = result.GPA;
                    }
                }
                //foreach (Result result in masterSheetResult)
                //{
                //    List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                //    foreach (Result resultItem in studentResults)
                //    {
                //        resultItem.Identifier = identifier;
                //        resultItem.CGPA = result.CGPA;
                //        resultItem.Remark = result.Remark;
                //        resultItem.GPA = result.GPA;
                //    }

                //}

                for (int i = 0; i < results.Count; i++)
                {
                    results[i].LevelName = levels;
                }

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetMaterSheetDetailsByOptionsAndMode(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption, CourseMode courseMode)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || departmentOption == null || departmentOption.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;

                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                DepartmentOption option = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOption.Id);

                if (courseMode == null)
                {
                    if (ss.Session.Id == (int)Sessions._20152016)
                    {
                        results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                     x =>
                                         x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                         x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                         x.Department_Id == department.Id &&
                                         x.Department_Option_Id == departmentOption.Id &&
                                         (x.Activated != false || x.Activated == null))
                                   select new Result
                                   {
                                       StudentId = sr.Person_Id,
                                       Sex = sr.Sex_Name,
                                       Name = sr.Name,
                                       MatricNumber = sr.Matric_Number,
                                       CourseId = sr.Course_Id,
                                       CourseCode = sr.Course_Code,
                                       CourseName = sr.Course_Name,
                                       CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                       DepartmentOptionId = sr.Department_Option_Id,
                                       DepartmentOptionName = sr.Department_Option_Name,
                                       SessionName = sr.Session_Name
                                   }).ToList();
                    }
                    else
                    {
                        if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                        {
                            results =
                                (from sr in
                                    repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                        x =>
                                            x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                            x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                            x.Department_Id == department.Id &&
                                            x.Department_Option_Id == departmentOption.Id &&
                                            (x.Activated != false || x.Activated == null))
                                 select new Result
                                 {
                                     StudentId = sr.Person_Id,
                                     Sex = sr.Sex_Name,
                                     Name = sr.Name,
                                     MatricNumber = sr.Matric_Number,
                                     CourseId = sr.Course_Id,
                                     CourseCode = sr.Course_Code,
                                     CourseName = sr.Course_Name,
                                     CourseUnit = Convert.ToInt32(sr.Course_Unit),
                                     SpecialCase = sr.Special_Case,
                                     TestScore = sr.Test_Score,
                                     ExamScore = sr.Exam_Score,
                                     Score = sr.Total_Score,
                                     Grade = sr.Grade,
                                     GradePoint = sr.Grade_Point,
                                     DepartmentName = sr.Department_Name + " (" + sr.Department_Option_Name + ")",
                                     ProgrammeName = sr.Programme_Name,
                                     LevelName = sr.Level_Name,
                                     Semestername = sr.Semester_Name,
                                     GPCU = sr.Grade_Point * sr.Course_Unit,
                                     TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                     DepartmentOptionId = sr.Department_Option_Id,
                                     DepartmentOptionName = sr.Department_Option_Name,
                                     SessionName = sr.Session_Name
                                 }).ToList();

                            List<Result> resultList = new List<Result>();

                            for (int i = 0; i < results.Count; i++)
                            {
                                if (results[i].MatricNumber.Contains("/16/"))
                                {
                                    resultList.Add(results[i]);
                                }
                                else
                                {
                                    //Do Nothing
                                }
                            }

                            results = new List<Result>();
                            results = resultList;
                        }
                        else
                        {
                            results =
                                (from sr in
                                    repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                        x =>
                                            x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                            x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                            x.Department_Option_Id == departmentOption.Id &&
                                            (x.Activated != false || x.Activated == null))
                                 select new Result
                                 {
                                     StudentId = sr.Person_Id,
                                     Sex = sr.Sex_Name,
                                     Name = sr.Name,
                                     MatricNumber = sr.Matric_Number,
                                     CourseId = sr.Course_Id,
                                     CourseCode = sr.Course_Code,
                                     CourseName = sr.Course_Name,
                                     CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                     DepartmentOptionId = sr.Department_Option_Id,
                                     DepartmentOptionName = sr.Department_Option_Name,
                                     SessionName = sr.Session_Name
                                 }).ToList();

                            List<Result> resultList = new List<Result>();

                            for (int i = 0; i < results.Count; i++)
                            {
                                if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                                {
                                    //Do Nothing
                                }
                                else
                                {
                                    resultList.Add(results[i]);
                                }
                            }

                            results = new List<Result>();
                            results = resultList;
                        }

                    }
                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    //  List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList(); 
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];
                        resultItem.Identifier = identifier;
                        TotalUnitsOmitted = 0;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        result.UnitOutstanding = TotalUnitsOmitted;
                        masterSheetResult.Add(result);
                    }

                    StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
                    List<long> extraYear = extraYearLogic.GetEntitiesBy(e => e.Session_Id == ss.Session.Id).Select(e => e.Person_Id).ToList();

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];
                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                            resultItem.UnitOutstanding = result.UnitOutstanding;

                            resultItem.SessionId = ss.Session.Id;

                            int totalSemesterCourseUnit = 0;
                            CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            //if (totalSemesterCourseUnit > 0)
                            //{
                            //    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            //}
                            if (extraYear.Contains(resultItem.StudentId))
                            {
                                resultItem.Remark = "";
                            }

                            resultItem.DepartmentOptionName = option.Name;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Extensive";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.FirstAttempt)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id == departmentOption.Id && (x.Activated != false || x.Activated == null) && x.Course_Mode_Id == courseMode.Id)
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode = sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                   SessionName = sr.Session_Name
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].MatricNumber.Contains("/16/"))
                            {
                                resultList.Add(results[i]);
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].MatricNumber.Contains("/15/"))
                            {
                                resultList.Add(results[i]);
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    //  List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList(); 
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];
                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];
                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "First Attempt";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.CarryOver)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id == departmentOption.Id && (x.Activated != false || x.Activated == null) && x.Course_Mode_Id == courseMode.Id)
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode = sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                   SessionName = sr.Session_Name
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains("/15/"))
                        {
                            resultList.Add(results[i]);
                        }
                        else
                        {
                            //Do Nothing
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    //  List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList(); 
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];
                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];
                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Carry Over";
                    }
                }
                else if (courseMode.Id == (int)CourseModes.ExtraYear)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id == departmentOption.Id && (x.Activated != false || x.Activated == null))
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode = sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                   SessionName = sr.Session_Name
                               }).ToList();

                    List<Result> resultList = new List<Result>();

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains("/15/") || results[i].MatricNumber.Contains("/16/"))
                        {
                            //Do Nothing
                        }
                        else
                        {
                            resultList.Add(results[i]);
                        }
                    }

                    results = new List<Result>();
                    results = resultList;

                    sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                    //  List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList(); 
                    List<Result> masterSheetResult = new List<Result>();
                    for (int i = 0; i < results.Count; i++)
                    {
                        Result resultItem = results[i];
                        resultItem.Identifier = identifier;
                        Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                        masterSheetResult.Add(result);
                    }

                    for (int i = 0; i < masterSheetResult.Count; i++)
                    {
                        Result result = masterSheetResult[i];
                        List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                        for (int j = 0; j < studentResults.Count; j++)
                        {
                            Result resultItem = studentResults[j];
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = result.CGPA;
                            resultItem.Remark = result.Remark;
                            resultItem.GPA = result.GPA;
                        }
                    }

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].LevelName = levels;
                        results[i].CourseMode = "Extra Year";
                    }
                }

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> AggregateScore(SessionSemester sessionSemester, Level level, Programme programme, Department department, CourseMode courseMode)
        {
            List<Result> processedResult = new List<Result>();
            List<Result> returnResult = new List<Result>();
            List<Result> firstYearFirstSemesterresult = new List<Result>();
            List<Result> firstYearSecondSemesterresult = new List<Result>();
            
            
            List<Result> FirstYearCumulative = new List<Result>();
            int count = 0;
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                decimal FirstYearFirstSemesterGPCU = 0M;
                List<int> firstYearTotalCourseUnit =new List<int>();

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;

                //List<int> firstYearSecondSemesterTotalCourseUnit = 0;
                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id==ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
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
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper(),
                               SemesterId=sr.Semester_Id
                               

                           }).ToList();
                
                Session previoussession = new Session();
                Level previousLevel = new Level();
                if (level.Id == 2 || level.Id==4 || level.Id==12)
                {
                    if (level.Id == 2)
                    {
                        previousLevel = new Level() { Id = 1 };
                    }
                    else if (level.Id == 4)
                    {
                        previousLevel = new Level() { Id = 3 };
                    }
                    else if (level.Id == 12)
                    {
                        previousLevel = new Level() { Id = 11 };
                    }
                    
                    previoussession = new Session() { Id = ss.Session.Id - 1 };
                    Semester firstSemester = new Semester { Id = 1 };
                    Semester secondSemester = new Semester { Id = 2 };
                    List<long> studentIds = results.Select(r => r.StudentId).Distinct().ToList();
                    count =studentIds.Count();
                    List<Result> previousResults = GetStudentAllPreviousProcessedResultBy(previoussession, previousLevel, department, programme);
                    
                    foreach (var firstResultItem in studentIds)
                    {
                        Student student = new Student { Id = firstResultItem };
                        List<Result> firstSemesterresult = previousResults.Where(x=>x.StudentId==student.Id && x.SemesterId==firstSemester.Id).ToList();
                        if (firstSemesterresult.Count > 0)
                        {

                                decimal? GPCUSum = firstSemesterresult.Sum(x => x.PreviousGPCU);
                                decimal? TWGP = firstSemesterresult.Sum(x => x.PreviousWGP);
                                decimal? TotalUnit = firstSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit;
                                int? UnitPassed = firstSemesterresult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                                int? UnitOutStanding = firstSemesterresult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                                firstSemesterresult.FirstOrDefault().PreviousGPCU = GPCUSum;
                                firstSemesterresult.FirstOrDefault().PreviousTUP = UnitPassed;
                                firstSemesterresult.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                                firstSemesterresult.FirstOrDefault().PreviousWGP = TWGP;
                                firstSemesterresult.FirstOrDefault().PreviousGPA = (GPCUSum / TotalUnit);
                                firstSemesterresult.FirstOrDefault().PreviousTUA = (int)TotalUnit;

                        }

                        //firstYearFirstSemesterresult.AddRange(firstSemesterresult);
                        //Second Semester Result
                        List<Result> SecondSemesterresult = previousResults.Where(x => x.StudentId == student.Id && x.SemesterId == secondSemester.Id).ToList();
                        if (SecondSemesterresult.Count > 0)
                        {
                            decimal? GPCUSum = SecondSemesterresult.Sum(x => x.PreviousGPCU);
                            decimal? TWGP = SecondSemesterresult.Sum(x => x.PreviousWGP);
                            decimal? TotalUnit = SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit;
                            int? UnitPassed = SecondSemesterresult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                            int? UnitOutStanding = SecondSemesterresult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                            SecondSemesterresult.FirstOrDefault().PreviousGPCU = GPCUSum;
                            SecondSemesterresult.FirstOrDefault().PreviousTUP = UnitPassed;
                            SecondSemesterresult.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                            SecondSemesterresult.FirstOrDefault().PreviousWGP = TWGP;
                            SecondSemesterresult.FirstOrDefault().PreviousGPA = (GPCUSum / TotalUnit);
                            SecondSemesterresult.FirstOrDefault().PreviousTUA = (int)TotalUnit;
                        }
                        if (SecondSemesterresult.Count>0 && firstSemesterresult.Count>0)
                        {
                            var totalFirstYearGPCU = (decimal)(firstSemesterresult.FirstOrDefault().PreviousGPCU + SecondSemesterresult.FirstOrDefault().PreviousGPCU);
                            int secondTotalUnit = (int)(SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit);
                            int firstTotalUnit = (int)(firstSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit);
                            //Get total unit passed for both semesters
                            int firstUnitPassed = (int)firstSemesterresult.FirstOrDefault().PreviousTUP;
                            int secondUnitPassed = (int)SecondSemesterresult.FirstOrDefault().PreviousTUP;
                            SecondSemesterresult.FirstOrDefault().UnitPassed = firstUnitPassed + secondUnitPassed;
                            //Get total unit Outstanding for both semesters
                            int firstUnitOutstanding = (int)firstSemesterresult.FirstOrDefault().UnitOutstanding;
                            int secondUnitOutstanding = (int)SecondSemesterresult.FirstOrDefault().UnitOutstanding;
                            SecondSemesterresult.FirstOrDefault().UnitOutstanding = firstUnitOutstanding + secondUnitOutstanding;
                            /// this should be CGPA
                            FirstYearFirstSemesterGPCU = Decimal.Round((decimal)(totalFirstYearGPCU / (secondTotalUnit + firstTotalUnit)),2);
                            var averageTWGP = Decimal.Round(((decimal)(firstSemesterresult.FirstOrDefault().PreviousWGP + SecondSemesterresult.FirstOrDefault().PreviousWGP) / 2),2);
                            var averageGPA = Decimal.Round(((decimal)(firstSemesterresult.FirstOrDefault().PreviousGPA + SecondSemesterresult.FirstOrDefault().PreviousGPA) / 2), 2);
                            SecondSemesterresult.FirstOrDefault().PreviousWGP = averageTWGP;
                            SecondSemesterresult.FirstOrDefault().PreviousGPA = averageGPA;
                            SecondSemesterresult.FirstOrDefault().PreviousGPCU = totalFirstYearGPCU;
                            SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit = secondTotalUnit + firstTotalUnit;
                            SecondSemesterresult.FirstOrDefault().PreviousTUA = secondTotalUnit + firstTotalUnit;



                        }
                        //else
                        //{
                        //    SecondSemesterresult.FirstOrDefault().PreviousWGP = 0;
                        //    SecondSemesterresult.FirstOrDefault().PreviousGPCU = 0;
                        //    SecondSemesterresult.FirstOrDefault().PreviousGPA = 0;
                        //    SecondSemesterresult.FirstOrDefault().UnitPassed = 0;
                        //}

                        FirstYearCumulative.AddRange(SecondSemesterresult);
                    }
                }
                List<long> secondYearstudentIds = results.Select(r => r.StudentId).Distinct().ToList();
                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                foreach (var result in secondYearstudentIds)
                {
                    //if (result == 12261 || result == 12231 || result == 12203)
                    //{
                    //    var alertme = 0;
                    //}
                    List<Result> secondYearFirstSemesterresult = new List<Result>();
                    List<Result> secondYearSecondSemesterresult = new List<Result>();
                    if (sessionSemester.Semester.Id == 1)
                    {
                        Student student = new Student { Id = result};
                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };
                        //List<Result> secondFirstResult = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                        List<Result> secondFirstResult=results.Where(x => x.StudentId == student.Id && x.SemesterId == semester.Id).ToList();
                        secondYearFirstSemesterresult.AddRange(secondFirstResult);
                        //modify the secondyear result list
                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;

                        for (int i = 0; i < secondYearFirstSemesterresult.Count; i++)
                        {
                            Result resultItem = secondYearFirstSemesterresult[i];
                            decimal WGP = 0;
                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }

                        Result secondYearFirstSemesterResult = new Result();
                        decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                        decimal firstSemesterGPA = 0M;

                        if (firstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                        {
                            firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }

                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                        var hasPreviousResult = FirstYearCumulative.Where(x => x.StudentId == result);
                        int firstYearCourseUnit = 0;
                        decimal firstYearGPCU = 0M;
                        if (hasPreviousResult.Count() > 0)
                        {
                            firstYearCourseUnit = (int)FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousTotalSemesterCourseUnit).FirstOrDefault();
                            firstYearGPCU = (decimal)(FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousGPCU).FirstOrDefault()) * 2;
                        }

                        // if (firstYearFirstSemesterresult != null && firstYearFirstSemesterGPCU != null && firstYearSecondSemesterresult != null && firstYearSecondSemesterGPCU != null)
                        // {
                        //secondYearFirstSemesterResult.CGPA = Decimal.Round(((firstSemesterGPCUSum +  firstYearGPCU / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                        secondYearFirstSemesterResult.CGPA = Decimal.Round((firstSemesterGPCUSum + firstYearGPCU) / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit),2);
                        //}
                        //else
                        //{
                        //    secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
                        //}

                        List<string> secondYearFirstSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);

                        decimal firstYearCGPA =firstYearCourseUnit > 0 ? Convert.ToDecimal(firstYearGPCU  / firstYearCourseUnit):0;
                        var roundedfirstYearCGPA= firstYearCGPA!=0 ? Decimal.Round(firstYearCGPA, 2):0;
                        //this is to state only the status without the carryover in the graduand list 
                        secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatusForGraduands(roundedfirstYearCGPA, secondYearFirstSemesterResult.CGPA);
                        processedResult.Add(secondYearFirstSemesterResult);

                    }
                    else if (sessionSemester.Semester.Id == (int)Semesters.SecondSemester)
                    {

                        //List<Result> firstSemesterresult = null;
                        
                        Student student = new Student { Id = result};
                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };
                        //List<Result> firstSemester = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                        List<Result> firstSemester = results.Where(x => x.StudentId == student.Id && x.SemesterId == semester.Id).ToList();
                        secondYearFirstSemesterresult.AddRange(firstSemester);

                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;
                        for (int i = 0; i < secondYearFirstSemesterresult.Count; i++)
                        {
                            Result resultItem = secondYearFirstSemesterresult[i];
                            decimal WGP = 0;
                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }
                        Result secondYearFirstSemesterResult = new Result();
                        decimal secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                        decimal firstSemesterGPA = 0M;
                        if (secondYearfirstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                        {
                            firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }

                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                        //List<Result> secondSemesterResult = null;
                        Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                        //List<Result> secondsemester = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                        List<Result> secondsemester = results.Where(x => x.StudentId == student.Id && x.SemesterId == secondSemester.Id).ToList();
                        //Ensure the student registered for second semester courses
                        if (secondsemester.Count > 0)
                        {
                            secondYearSecondSemesterresult.AddRange(secondsemester);
                            List<Result> modifiedSecondResultList = new List<Result>();
                            for (int i = 0; i < secondYearSecondSemesterresult.Count; i++)
                            {
                                Result resultItem = secondYearSecondSemesterresult[i];
                                decimal WGP = 0;
                                totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.SecondSemester);
                                if (totalSemesterCourseUnit > 0)
                                {
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                }
                                modifiedSecondResultList.Add(resultItem);
                            }
                            Result secondYearSecondtSemesterResult = new Result();
                            decimal secondYearSecondtSemesterGPCUSum = modifiedSecondResultList.Sum(p => p.GPCU) ?? 0;
                            int secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                            secondYearSecondSemesterTotalSemesterCourseUnit = modifiedSecondResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                            secondYearSecondtSemesterResult = modifiedSecondResultList.FirstOrDefault();
                            secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                            secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                            decimal secondYearSecondSmesterGPA = 0M;
                            if (secondYearSecondtSemesterGPCUSum > 0 && secondYearSecondSemesterTotalSemesterCourseUnit > 0)
                            {
                                secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                            }
                            secondYearSecondtSemesterResult.GPA = Decimal.Round(secondYearSecondSmesterGPA, 2);
                            var hasPreviousResult = FirstYearCumulative.Where(x => x.StudentId == result);
                            int firstYearCourseUnit = 0;
                            decimal firstYearGPCU = 0M;
                            if (hasPreviousResult.Count() > 0)
                            {
                                firstYearCourseUnit = (int)FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousTotalSemesterCourseUnit).FirstOrDefault();
                                firstYearGPCU = (decimal)(FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousGPCU).FirstOrDefault()) * 2;
                            }



                            decimal previousCGPA = 0M;
                            secondYearSecondtSemesterResult.CGPA = Decimal.Round((secondYearSecondtSemesterGPCUSum + firstYearGPCU) / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit), 2);

                            List<string> secondYearSecondSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                            if (secondYearSecondtSemesterResult.CGPA < (decimal)2.0)
                            {
                                secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, previousCGPA, secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                            }
                            else
                            {
                                //this is to state only the status without the carryover in the graduand list 
                                secondYearSecondtSemesterResult.Remark = GetGraduationStatusForGranduands(secondYearSecondtSemesterResult.CGPA);
                                //secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                            }

                            processedResult.Add(secondYearSecondtSemesterResult);
                        }
                        

                    }
                }
                var allstudentIds = results.Select(x => x.StudentId).Distinct();
                
                
                foreach (var studentId in allstudentIds)
                {
                    //if (studentId == 12261 || studentId == 12231 || studentId == 12203)
                    //{
                    //    var alertme = 0;
                    //}

                    var stusentProcessedResult = processedResult.Where(x => x.StudentId == studentId).FirstOrDefault();
                    if (stusentProcessedResult != null)
                    {
                        

                        var previousResult = FirstYearCumulative.Where(x => x.StudentId == studentId).ToList();
                        var studentAllResult = results.Where(x => x.StudentId == studentId).ToList();
                        int? UnitPassed = studentAllResult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                        int? UnitOutStanding = studentAllResult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                        foreach (var resultItem in studentAllResult)
                        {
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = stusentProcessedResult.CGPA;
                            resultItem.Remark = stusentProcessedResult.GPA != 0 ? stusentProcessedResult.Remark : "";
                            resultItem.GPA = stusentProcessedResult.GPA != null ? Decimal.Round((decimal)stusentProcessedResult.GPA, 2) : 0;
                            resultItem.PreviousTotalSemesterCourseUnit = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTotalSemesterCourseUnit : 0;
                            resultItem.PreviousGPCU = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousGPCU : 0;
                            resultItem.UnitOutstanding = UnitOutStanding;
                            resultItem.UnitPassed = UnitPassed;
                            resultItem.PreviousWGP = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousWGP : 0;
                            resultItem.PreviousTUP = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTUP : 0;
                            resultItem.PreviousGPA = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousGPA : 0;
                            resultItem.PreviousTUA= previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTUA : 0;

                            //resultItem.PreviousGPA = result.PreviousGPA != null ? Decimal.Round((decimal)result.PreviousGPA, 2) : 0;
                            resultItem.Count = count;

                        }
                        returnResult.AddRange(studentAllResult);
                    }
                    
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return returnResult;
        }

        public List<Result> AggregateScoreByDepartmentOption(SessionSemester sessionSemester, Level level, Programme programme, Department department, CourseMode courseMode, DepartmentOption departmentOption)
        {
            List<Result> processedResult = new List<Result>();
            List<Result> returnResult = new List<Result>();
            List<Result> firstYearFirstSemesterresult = new List<Result>();
            List<Result> firstYearSecondSemesterresult = new List<Result>();


            List<Result> FirstYearCumulative = new List<Result>();
            int count = 0;
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || departmentOption==null || departmentOption.Id<=0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                decimal FirstYearFirstSemesterGPCU = 0M;
                List<int> firstYearTotalCourseUnit = new List<int>();

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                DepartmentOption option = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOption.Id);

                //List<int> firstYearSecondSemesterTotalCourseUnit = 0;
                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Option_Id == departmentOption.Id && x.Department_Id == department.Id)
                           select new Result
                           {
                               StudentId = sr.Person_Id,
                               Sex = sr.Sex_Name,
                               Name = sr.Name,
                               MatricNumber = sr.Matric_Number,
                               CourseId = sr.Course_Id,
                               CourseCode = sr.Course_Code,
                               CourseName = sr.Course_Name,
                               CourseUnit = (int)sr.Course_Unit,
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
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper(),
                               SemesterId = sr.Semester_Id


                           }).ToList();

                Session previoussession = new Session();
                Level previousLevel = new Level();
                if (level.Id == 2 || level.Id == 4)
                {
                    previousLevel = new Level() { Id = 1 };
                    previoussession = new Session() { Id = ss.Session.Id - 1 };
                    Semester firstSemester = new Semester { Id = 1 };
                    Semester secondSemester = new Semester { Id = 2 };
                    List<long> studentIds = results.Select(r => r.StudentId).Distinct().ToList();
                    count = studentIds.Count();
                    List<Result> previousResults = GetStudentAllPreviousProcessedDepartmentOptionResultBy(previoussession, previousLevel, department, programme,departmentOption);

                    foreach (var firstResultItem in studentIds)
                    {
                        Student student = new Student { Id = firstResultItem };
                        List<Result> firstSemesterresult = previousResults.Where(x => x.StudentId == student.Id && x.SemesterId == firstSemester.Id).ToList();
                        if (firstSemesterresult.Count > 0)
                        {

                            decimal? GPCUSum = firstSemesterresult.Sum(x => x.PreviousGPCU);
                            decimal? TWGP = firstSemesterresult.Sum(x => x.PreviousWGP);
                            decimal? TotalUnit = firstSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit;
                            int? UnitPassed = firstSemesterresult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                            int? UnitOutStanding = firstSemesterresult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                            firstSemesterresult.FirstOrDefault().PreviousGPCU = GPCUSum;
                            firstSemesterresult.FirstOrDefault().PreviousTUP = UnitPassed;
                            firstSemesterresult.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                            firstSemesterresult.FirstOrDefault().PreviousWGP = TWGP;
                            firstSemesterresult.FirstOrDefault().PreviousGPA = (GPCUSum / TotalUnit);

                        }

                        //firstYearFirstSemesterresult.AddRange(firstSemesterresult);
                        //Second Semester Result
                        List<Result> SecondSemesterresult = previousResults.Where(x => x.StudentId == student.Id && x.SemesterId == secondSemester.Id).ToList();
                        if (SecondSemesterresult.Count > 0)
                        {
                            decimal? GPCUSum = SecondSemesterresult.Sum(x => x.PreviousGPCU);
                            decimal? TWGP = SecondSemesterresult.Sum(x => x.PreviousWGP);
                            decimal? TotalUnit = SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit;
                            int? UnitPassed = SecondSemesterresult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                            int? UnitOutStanding = SecondSemesterresult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                            SecondSemesterresult.FirstOrDefault().PreviousGPCU = GPCUSum;
                            SecondSemesterresult.FirstOrDefault().PreviousTUP = UnitPassed;
                            SecondSemesterresult.FirstOrDefault().UnitOutstanding = UnitOutStanding;
                            SecondSemesterresult.FirstOrDefault().PreviousWGP = TWGP;
                            SecondSemesterresult.FirstOrDefault().PreviousGPA = (GPCUSum / TotalUnit);
                        }
                        if (SecondSemesterresult.Count > 0 && firstSemesterresult.Count > 0)
                        {
                            var totalFirstYearGPCU = (decimal)(firstSemesterresult.FirstOrDefault().PreviousGPCU + SecondSemesterresult.FirstOrDefault().PreviousGPCU);
                            int secondTotalUnit = (int)(SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit);
                            int firstTotalUnit = (int)(firstSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit);
                            //Get total unit passed for both semesters
                            int firstUnitPassed = (int)firstSemesterresult.FirstOrDefault().PreviousTUP;
                            int secondUnitPassed = (int)SecondSemesterresult.FirstOrDefault().PreviousTUP;
                            SecondSemesterresult.FirstOrDefault().UnitPassed = firstUnitPassed + secondUnitPassed;
                            //Get total unit Outstanding for both semesters
                            int firstUnitOutstanding = (int)firstSemesterresult.FirstOrDefault().UnitOutstanding;
                            int secondUnitOutstanding = (int)SecondSemesterresult.FirstOrDefault().UnitOutstanding;
                            SecondSemesterresult.FirstOrDefault().UnitOutstanding = firstUnitOutstanding + secondUnitOutstanding;
                            /// this should be CGPA
                            FirstYearFirstSemesterGPCU = Decimal.Round((decimal)(totalFirstYearGPCU / (secondTotalUnit + firstTotalUnit)), 2);
                            var averageTWGP = Decimal.Round(((decimal)(firstSemesterresult.FirstOrDefault().PreviousWGP + SecondSemesterresult.FirstOrDefault().PreviousWGP) / 2), 2);
                            var averageGPA = Decimal.Round(((decimal)(firstSemesterresult.FirstOrDefault().PreviousGPA + SecondSemesterresult.FirstOrDefault().PreviousGPA) / 2), 2);
                            SecondSemesterresult.FirstOrDefault().PreviousWGP = averageTWGP;
                            SecondSemesterresult.FirstOrDefault().PreviousGPA = averageGPA;
                            SecondSemesterresult.FirstOrDefault().PreviousGPCU = totalFirstYearGPCU;
                            SecondSemesterresult.FirstOrDefault().PreviousTotalSemesterCourseUnit = secondTotalUnit + firstTotalUnit;


                        }
                        //else
                        //{
                        //    SecondSemesterresult.FirstOrDefault().PreviousWGP = 0;
                        //    SecondSemesterresult.FirstOrDefault().PreviousGPCU = 0;
                        //    SecondSemesterresult.FirstOrDefault().PreviousGPA = 0;
                        //    SecondSemesterresult.FirstOrDefault().UnitPassed = 0;
                        //}

                        FirstYearCumulative.AddRange(SecondSemesterresult);
                    }
                }
                List<long> secondYearstudentIds = results.Select(r => r.StudentId).Distinct().ToList();
                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                foreach (var result in secondYearstudentIds)
                {
                    //if (result == 12261 || result == 12231 || result == 12203)
                    //{
                    //    var alertme = 0;
                    //}
                    List<Result> secondYearFirstSemesterresult = new List<Result>();
                    List<Result> secondYearSecondSemesterresult = new List<Result>();
                    if (sessionSemester.Semester.Id == 1)
                    {
                        Student student = new Student { Id = result };
                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };
                        //List<Result> secondFirstResult = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                        List<Result> secondFirstResult = results.Where(x => x.StudentId == student.Id && x.SemesterId == semester.Id).ToList();
                        secondYearFirstSemesterresult.AddRange(secondFirstResult);
                        //modify the secondyear result list
                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;

                        for (int i = 0; i < secondYearFirstSemesterresult.Count; i++)
                        {
                            Result resultItem = secondYearFirstSemesterresult[i];
                            decimal WGP = 0;
                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }

                        Result secondYearFirstSemesterResult = new Result();
                        decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                        decimal firstSemesterGPA = 0M;

                        if (firstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                        {
                            firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }

                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                        var hasPreviousResult = FirstYearCumulative.Where(x => x.StudentId == result);
                        int firstYearCourseUnit = 0;
                        decimal firstYearGPCU = 0M;
                        if (hasPreviousResult.Count() > 0)
                        {
                            firstYearCourseUnit = (int)FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousTotalSemesterCourseUnit).FirstOrDefault();
                            firstYearGPCU = (decimal)(FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousGPCU).FirstOrDefault()) * 2;
                        }

                        // if (firstYearFirstSemesterresult != null && firstYearFirstSemesterGPCU != null && firstYearSecondSemesterresult != null && firstYearSecondSemesterGPCU != null)
                        // {
                        //secondYearFirstSemesterResult.CGPA = Decimal.Round(((firstSemesterGPCUSum +  firstYearGPCU / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                        secondYearFirstSemesterResult.CGPA = Decimal.Round((firstSemesterGPCUSum + firstYearGPCU) / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit), 2);
                        //}
                        //else
                        //{
                        //    secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
                        //}

                        List<string> secondYearFirstSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);

                        decimal firstYearCGPA = firstYearCourseUnit > 0 ? Convert.ToDecimal(firstYearGPCU / firstYearCourseUnit) : 0;
                        var roundedfirstYearCGPA = firstYearCGPA != 0 ? Decimal.Round(firstYearCGPA, 2) : 0;
                        //this is to state only the status without the carryover in the graduand list 
                        secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatusForGraduands(roundedfirstYearCGPA, secondYearFirstSemesterResult.CGPA);
                        processedResult.Add(secondYearFirstSemesterResult);

                    }
                    else if (sessionSemester.Semester.Id == (int)Semesters.SecondSemester)
                    {

                        //List<Result> firstSemesterresult = null;

                        Student student = new Student { Id = result };
                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };
                        //List<Result> firstSemester = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                        List<Result> firstSemester = results.Where(x => x.StudentId == student.Id && x.SemesterId == semester.Id).ToList();
                        secondYearFirstSemesterresult.AddRange(firstSemester);

                        List<Result> modifiedResultList = new List<Result>();
                        int totalSemesterCourseUnit = 0;
                        for (int i = 0; i < secondYearFirstSemesterresult.Count; i++)
                        {
                            Result resultItem = secondYearFirstSemesterresult[i];
                            decimal WGP = 0;
                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                            if (totalSemesterCourseUnit > 0)
                            {
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            }
                            modifiedResultList.Add(resultItem);
                        }
                        Result secondYearFirstSemesterResult = new Result();
                        decimal secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                        secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                        decimal firstSemesterGPA = 0M;
                        if (secondYearfirstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                        {
                            firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                        }

                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                        //List<Result> secondSemesterResult = null;
                        Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                        //List<Result> secondsemester = GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                        List<Result> secondsemester = results.Where(x => x.StudentId == student.Id && x.SemesterId == secondSemester.Id).ToList();
                        //Ensure the student registered for second semester courses
                        if (secondsemester.Count > 0)
                        {
                            secondYearSecondSemesterresult.AddRange(secondsemester);
                            List<Result> modifiedSecondResultList = new List<Result>();
                            for (int i = 0; i < secondYearSecondSemesterresult.Count; i++)
                            {
                                Result resultItem = secondYearSecondSemesterresult[i];
                                decimal WGP = 0;
                                totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.SecondSemester);
                                if (totalSemesterCourseUnit > 0)
                                {
                                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                }
                                modifiedSecondResultList.Add(resultItem);
                            }
                            Result secondYearSecondtSemesterResult = new Result();
                            decimal secondYearSecondtSemesterGPCUSum = modifiedSecondResultList.Sum(p => p.GPCU) ?? 0;
                            int secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                            secondYearSecondSemesterTotalSemesterCourseUnit = modifiedSecondResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                            secondYearSecondtSemesterResult = modifiedSecondResultList.FirstOrDefault();
                            secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                            secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                            decimal secondYearSecondSmesterGPA = 0M;
                            if (secondYearSecondtSemesterGPCUSum > 0 && secondYearSecondSemesterTotalSemesterCourseUnit > 0)
                            {
                                secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                            }
                            secondYearSecondtSemesterResult.GPA = Decimal.Round(secondYearSecondSmesterGPA, 2);
                            var hasPreviousResult = FirstYearCumulative.Where(x => x.StudentId == result);
                            int firstYearCourseUnit = 0;
                            decimal firstYearGPCU = 0M;
                            if (hasPreviousResult.Count() > 0)
                            {
                                firstYearCourseUnit = (int)FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousTotalSemesterCourseUnit).FirstOrDefault();
                                firstYearGPCU = (decimal)(FirstYearCumulative.Where(x => x.StudentId == result).Select(x => x.PreviousGPCU).FirstOrDefault()) * 2;
                            }



                            decimal previousCGPA = 0M;
                            secondYearSecondtSemesterResult.CGPA = Decimal.Round((secondYearSecondtSemesterGPCUSum + firstYearGPCU) / (firstYearCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit), 2);

                            List<string> secondYearSecondSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                            if (secondYearSecondtSemesterResult.CGPA < (decimal)2.0)
                            {
                                secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, previousCGPA, secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                            }
                            else
                            {
                                //this is to state only the status without the carryover in the graduand list 
                                secondYearSecondtSemesterResult.Remark = GetGraduationStatusForGranduands(secondYearSecondtSemesterResult.CGPA);
                                //secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                            }

                            processedResult.Add(secondYearSecondtSemesterResult);
                        }


                    }
                }
                var allstudentIds = results.Select(x => x.StudentId).Distinct();


                foreach (var studentId in allstudentIds)
                {
                    //if (studentId == 12261 || studentId == 12231 || studentId == 12203)
                    //{
                    //    var alertme = 0;
                    //}

                    var stusentProcessedResult = processedResult.Where(x => x.StudentId == studentId).FirstOrDefault();
                    if (stusentProcessedResult != null)
                    {


                        var previousResult = FirstYearCumulative.Where(x => x.StudentId == studentId).ToList();
                        var studentAllResult = results.Where(x => x.StudentId == studentId).ToList();
                        int? UnitPassed = studentAllResult.Where(x => x.PreviousTotalScore >= 40).Sum(x => x.PreviousCourseUnit);
                        int? UnitOutStanding = studentAllResult.Where(x => x.PreviousTotalScore <= 39).Sum(x => x.PreviousCourseUnit);
                        foreach (var resultItem in studentAllResult)
                        {
                            resultItem.Identifier = identifier;
                            resultItem.CGPA = stusentProcessedResult.CGPA;
                            resultItem.Remark = stusentProcessedResult.GPA != 0 ? stusentProcessedResult.Remark : "";
                            resultItem.GPA = stusentProcessedResult.GPA != null ? Decimal.Round((decimal)stusentProcessedResult.GPA, 2) : 0;
                            resultItem.PreviousTotalSemesterCourseUnit = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTotalSemesterCourseUnit : 0;
                            resultItem.PreviousGPCU = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousGPCU : 0;
                            resultItem.UnitOutstanding = UnitOutStanding;
                            resultItem.UnitPassed = UnitPassed;
                            resultItem.PreviousWGP = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousWGP : 0;
                            resultItem.PreviousTUP = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTUP : 0;
                            resultItem.PreviousGPA = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousGPA : 0;
                            resultItem.PreviousTUA = previousResult.Count > 0 ? previousResult.FirstOrDefault().PreviousTotalSemesterCourseUnit : 0;

                            //resultItem.PreviousGPA = result.PreviousGPA != null ? Decimal.Round((decimal)result.PreviousGPA, 2) : 0;
                            resultItem.Count = count;

                        }
                        returnResult.AddRange(studentAllResult);
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnResult;
        }
       
        public List<Result> GetMaterSheetDetailsAltBy(SessionSemester sessionSemester, Level level, Programme programme, Department department, CourseMode courseMode)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || courseMode == null || courseMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;




                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Grade_Point!=null)
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
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper()
                           }).ToList();



                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                // List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList();
                List<Result> masterSheetResult = new List<Result>();
                foreach (Result resultItem in results)
                {
                    resultItem.Identifier = identifier;
                    //if (resultItem.StudentId == 12255)
                    //{
                    //    int i = 0;
                    //}
                     //Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
                     Result result = ViewProcessedStudentResultForAggregate(resultItem.StudentId, sessionSemester, level, programme, department);
                    masterSheetResult.Add(result);
                }


                foreach (Result result in masterSheetResult)
                {
                    List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                    foreach (Result resultItem in studentResults)
                    {
                        resultItem.Identifier = identifier;
                        resultItem.CGPA = result.CGPA;
                        resultItem.Remark = result.Remark;
                        resultItem.GPA = result.GPA!=null? Decimal.Round((decimal)result.GPA,2):0;
                        resultItem.PreviousTotalSemesterCourseUnit = result.PreviousTotalSemesterCourseUnit;
                        resultItem.PreviousCGPA = result.PreviousCGPA;
                        resultItem.PreviousCourseUnit = result.PreviousCourseUnit;
                        resultItem.PreviousGPA = result.PreviousGPA!=null? Decimal.Round((decimal)result.PreviousGPA,2):0;
                        

                    }

                }

                List<Result> resultsMode = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Course_Mode_Id == courseMode.Id)
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
                                                CourseModeId = sr.Course_Mode_Id,
                                                CourseModeName = sr.Course_Mode_Name,
                                                Surname = sr.Last_Name.ToUpper(),
                                                FacultyName = sr.Faculty_Name,
                                                WGP = sr.WGP,
                                                Othername = sr.Othernames.ToUpper()
                                            }).ToList();

                List<Result> ResultsAlt = new List<Result>();
                List<string> RegNumbers = resultsMode.Select(r => r.MatricNumber).Distinct().ToList();

                for (int i = 0; i < RegNumbers.Count; i++)
                {
                    List<Result> thisResult = results.Where(r => r.MatricNumber == RegNumbers[i]).ToList();
                    for (int j = 0; j < thisResult.Count; j++)
                    {
                        ResultsAlt.Add(thisResult[j]);
                    }
                }

                return ResultsAlt.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<Result> GetResultList(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            try
            {
                List<Result> filteredResult = new List<Result>();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<string> resultList = studentResultLogic.GetProcessedResutBy(sessionSemester.Session, sessionSemester.Semester, level, department, programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Result> result = studentResultLogic.GetProcessedResutBy(sessionSemester.Session, sessionSemester.Semester, level, department, programme);
                foreach (string item in resultList)
                {
                    Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }
                return filteredResult.ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }
        //public Result ViewProcessedStudentResult(long PersonId, SessionSemester sessionSemester, Level level, Programme programme, Department department)
        //{
        //    Result ProcessedResult = new Result();
        //    string Remark = null;
        //    try
        //    {

        //        if (PersonId > 0)
        //        {
        //            Abundance_Nk.Model.Model.Student student = new Model.Model.Student() { Id = PersonId };
        //            StudentLogic studentLogic = new StudentLogic();
        //            StudentResultLogic studentResultLogic = new StudentResultLogic();
        //            if (sessionSemester.Semester != null && sessionSemester.Session != null && programme != null && department != null && level != null)
        //            {
        //                if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
        //                {
        //                    Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == PersonId);
        //                    if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
        //                    {
        //                        List<Result> result = null;
        //                        if (studentCheck.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, sessionSemester.Semester, programme);
        //                        }

        //                        List<Result> modifiedResultList = new List<Result>();
        //                        int totalSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in result)
        //                        {
        //                            decimal WGP = 0;
        //                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit);
        //                            if (totalSemesterCourseUnit > 0)
        //                            {
        //                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
        //                            }
        //                            modifiedResultList.Add(resultItem);
        //                        }
        //                        decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
        //                        int? firstSemesterTotalSemesterCourseUnit = 0;
        //                        Result firstYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
        //                        firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
        //                        decimal firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
        //                        firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
        //                        firstYearFirstSemesterResult.CGPA = Decimal.Round(firstSemesterGPA, 2);
        //                        firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
        //                        firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
        //                        Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
        //                        firstYearFirstSemesterResult.Remark = Remark;
        //                        ProcessedResult = firstYearFirstSemesterResult;

        //                    }
        //                    else
        //                    {
        //                        List<Result> result = null;
        //                        Semester firstSemester = new Semester() { Id = (int)Semesters.FirstSemester };
        //                        if (studentCheck.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
        //                        }
        //                        else
        //                        {
        //                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
        //                        }
        //                        List<Result> firstSemesterModifiedResultList = new List<Result>();
        //                        int totalFirstSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in result)
        //                        {
        //                           decimal WGP = 0;
        //                           totalFirstSemesterCourseUnit = CheckForSpecialCase(resultItem, totalFirstSemesterCourseUnit);
        //                           if (totalFirstSemesterCourseUnit > 0)
        //                            {
        //                                resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
        //                            }
        //                            firstSemesterModifiedResultList.Add(resultItem);
        //                        }


        //                        decimal firstSemesterGPCUSum = firstSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
        //                        int? firstSemesterTotalSemesterCourseUnit = 0;
        //                        Result firstYearFirstSemesterResult = firstSemesterModifiedResultList.FirstOrDefault() ;
        //                        firstSemesterTotalSemesterCourseUnit = firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
        //                        decimal firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
        //                        firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA,2);

        //                        Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
        //                        List<Result> secondSemesterResult = null;
        //                        if (studentCheck.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
        //                        }
        //                        else
        //                        {
        //                            secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
        //                        }
        //                        List<Result> secondSemesterModifiedResultList = new List<Result>();

        //                        int totalSecondSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in secondSemesterResult)
        //                        {

        //                            decimal WGP = 0;

        //                            totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit);

        //                            if (totalSecondSemesterCourseUnit > 0)
        //                            {
        //                                resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
        //                            }
        //                            secondSemesterModifiedResultList.Add(resultItem);
        //                        }
        //                        decimal? secondSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
        //                        Result secondSemesterStudentResult = secondSemesterModifiedResultList.FirstOrDefault();

        //                        secondSemesterStudentResult.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / (decimal)(secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit))), 2);
        //                        secondSemesterStudentResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum) / (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) + firstSemesterTotalSemesterCourseUnit)), 2);
        //                        if (secondSemesterStudentResult.GPA < (decimal)2.0 && firstYearFirstSemesterResult.GPA < (decimal)2.0)
        //                        {
        //                            Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA, firstYearFirstSemesterResult.GPA, secondSemesterStudentResult.GPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
        //                        }
        //                        else
        //                        {
        //                            Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
        //                        }
        //                        secondSemesterStudentResult.Remark = Remark;
        //                        ProcessedResult = secondSemesterStudentResult;

        //                    }

        //                }
        //                else
        //                {
        //                    decimal firstYearFirstSemesterGPCUSum = 0;
        //                    int firstYearFirstSemesterTotalCourseUnit = 0;
        //                    decimal firstYearSecondSemesterGPCUSum = 0;
        //                    int firstYearSecondSemesterTotalCourseUnit = 0;
        //                    decimal secondYearFirstSemesterGPCUSum = 0;
        //                    int secondYearFirstSemesterTotalCourseUnit = 0;
        //                    decimal secondYearSecondSemesterGPCUSum = 0;
        //                    int secondYearSecondSemesterTotalCourseUnit = 0;

        //                    Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(sessionSemester,  level, programme,  department, student);
        //                    Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(sessionSemester, level, programme, department, student);
        //                    if (sessionSemester.Semester.Id == 1)
        //                    {
        //                        List<Result> result = null;
        //                        Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
        //                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };
        //                        if (student.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
        //                        }
        //                        else
        //                        {
        //                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
        //                        }
        //                        List<Result> modifiedResultList = new List<Result>();
        //                        int totalSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in result)
        //                        {
        //                            decimal WGP = 0;
        //                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit);
        //                            if (totalSemesterCourseUnit > 0)
        //                            {
        //                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
        //                            }
        //                            modifiedResultList.Add(resultItem);
        //                        }
        //                        Result secondYearFirstSemesterResult = new Result();
        //                        decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
        //                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
        //                        secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
        //                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
        //                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
        //                        secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
        //                        decimal firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
        //                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA,2);
        //                        if (firstYearFirstSemester != null && firstYearFirstSemester.GPCU != null && firstYearSecondSemester != null && firstYearSecondSemester.GPCU != null)
        //                        {
        //                           secondYearFirstSemesterResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
        //                        }
        //                        else
        //                        {
        //                            secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
        //                        }
        //                        List<string> secondYearFirstSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
        //                        secondYearFirstSemesterResult.Remark = GetGraduationStatus(secondYearFirstSemesterResult.CGPA, secondYearFirstSemetserCarryOverCourses);

        //                        ProcessedResult = secondYearFirstSemesterResult;

        //                    }
        //                    else if (sessionSemester.Semester.Id == (int)Semesters.SecondSemester)
        //                    {

        //                        List<Result> result = null;
        //                        Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
        //                        Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };

        //                        if (student.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
        //                        }
        //                        else
        //                        {
        //                            result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
        //                        }
        //                        List<Result> modifiedResultList = new List<Result>();
        //                        int totalSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in result)
        //                        {
        //                            decimal WGP = 0;
        //                            totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit);
        //                            if (totalSemesterCourseUnit > 0)
        //                            {
        //                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
        //                            }
        //                            modifiedResultList.Add(resultItem);
        //                        }
        //                        Result secondYearFirstSemesterResult = new Result();
        //                        decimal secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
        //                        int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
        //                        secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
        //                        secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
        //                        secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
        //                        decimal firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
        //                        secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

        //                        //Second semester second year

        //                        List<Result> secondSemesterResult = null;
        //                        Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
        //                        if (student.Activated == true || studentCheck.Activated == null)
        //                        {
        //                            secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
        //                        }
        //                        else
        //                        {
        //                            secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
        //                        }
        //                        List<Result> secondSemesterModifiedResultList = new List<Result>();
        //                        int totalSecondSemesterCourseUnit = 0;
        //                        foreach (Result resultItem in secondSemesterResult)
        //                        {
        //                           decimal WGP = 0;
        //                           totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit);
        //                            if (totalSecondSemesterCourseUnit > 0)
        //                            {
        //                               resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
        //                            }
        //                            secondSemesterModifiedResultList.Add(resultItem);
        //                        }
        //                        Result secondYearSecondtSemesterResult = new Result();
        //                        decimal secondYearSecondtSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
        //                        int secondYearSecondSemesterTotalSemesterCourseUnit = 0;
        //                        secondYearSecondSemesterTotalSemesterCourseUnit = secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
        //                        secondYearSecondtSemesterResult = secondSemesterModifiedResultList.FirstOrDefault();
        //                        secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
        //                        secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
        //                        decimal secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
        //                        secondYearSecondtSemesterResult.GPA = Decimal.Round(secondYearSecondSmesterGPA, 2);
        //                        secondYearSecondtSemesterResult.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
        //                        List<string> secondYearSecondSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
        //                        if (secondYearSecondtSemesterResult.GPA < (decimal)2.0 && secondYearFirstSemesterResult.GPA < (decimal)2.0)
        //                        {
        //                            secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearFirstSemesterResult.CGPA, secondYearFirstSemesterResult.GPA, secondYearSecondtSemesterResult.GPA, secondYearSecondSemetserCarryOverCourses);
        //                        }
        //                        else
        //                        {
        //                            secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearFirstSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
        //                        }

        //                        ProcessedResult = secondYearSecondtSemesterResult;
        //                    }



        //                }
        //            }
        //        }

        //    }
        //    catch (Exception )
        //    {


        //    }
        //    return ProcessedResult;
        //}
        public Result ViewProcessedStudentResult(long PersonId, SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            Result ProcessedResult = new Result();
            string Remark = null;
            try
            {
                if (PersonId > 0)
                {
                    Abundance_Nk.Model.Model.Student student = new Model.Model.Student() { Id = PersonId };
                    StudentLogic studentLogic = new StudentLogic();

                    GetStudent(studentLogic, student);

                    StudentResultLogic studentResultLogic = new StudentResultLogic();
                    if (sessionSemester.Semester != null && sessionSemester.Session != null && programme != null && department != null && level != null)
                    {
                        if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                        {
                            Abundance_Nk.Model.Model.Student studentCheck = new Student() { Id = PersonId };
                            GetStudent(studentLogic, studentCheck);
                            //Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == PersonId);
                            if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
                            {
                                List<Result> result = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, sessionSemester.Semester, programme);
                                }

                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }

                                decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                decimal firstSemesterGPA = 0M;
                                if (firstSemesterTotalSemesterCourseUnit != 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
                                }
                                firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.CGPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                                firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                                if (firstYearFirstSemesterResult.CGPA != null)
                                {
                                    Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                }
                                else
                                {
                                    Remark = "";
                                }
                                
                                firstYearFirstSemesterResult.Remark = Remark;
                                ProcessedResult = firstYearFirstSemesterResult;

                            }
                            else
                            {
                                List<Result> result = null;
                                Semester firstSemester = new Semester() { Id = (int)Semesters.FirstSemester };
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
                                }
                                List<Result> firstSemesterModifiedResultList = new List<Result>();
                                int totalFirstSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalFirstSemesterCourseUnit = CheckForSpecialCase(resultItem, totalFirstSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalFirstSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                    }
                                    firstSemesterModifiedResultList.Add(resultItem);

                                }


                                decimal firstSemesterGPCUSum = firstSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = firstSemesterModifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                decimal firstSemesterGPA = 0M;
                                if (firstSemesterGPCUSum > 0 && firstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
                                }

                                firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.CGPA = Decimal.Round(firstSemesterGPA, 2);

                                Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                                List<Result> secondSemesterResult = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                List<Result> secondSemesterModifiedResultList = new List<Result>();

                                int totalSecondSemesterCourseUnit = 0;
                                for (int i = 0; i < secondSemesterResult.Count; i++)
                                {
                                    Result resultItem = secondSemesterResult[i];
                                    decimal WGP = 0;

                                    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);

                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in secondSemesterResult)
                                //{

                                //    decimal WGP = 0;

                                //    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);

                                //    if (totalSecondSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                //    }
                                //    secondSemesterModifiedResultList.Add(resultItem);
                                //}
                                decimal? secondSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                                Result secondSemesterStudentResult = secondSemesterModifiedResultList.FirstOrDefault();

                                if (secondSemesterGPCUSum > 0)
                                {
                                    secondSemesterStudentResult.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / (decimal)(secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit))), 2);
                                }

                                if (firstSemesterGPCUSum > 0 || secondSemesterGPCUSum > 0)
                                {
                                    secondSemesterStudentResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum) / (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) + firstSemesterTotalSemesterCourseUnit)), 2);

                                }
                                else
                                {
                                    secondSemesterStudentResult.CGPA = secondSemesterStudentResult.GPA;
                                }

                                if (secondSemesterStudentResult.CGPA == 0)
                                {
                                    Remark = "";
                                }
                                else if (secondSemesterStudentResult.CGPA < (decimal)2.0 && firstYearFirstSemesterResult.GPA < (decimal)2.0)
                                {
                                    
                                    Remark = GetGraduationStatus(secondSemesterStudentResult.CGPA, firstYearFirstSemesterResult.CGPA ?? 0, secondSemesterStudentResult.CGPA ?? 0, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                }
                                else
                                {
                                    Remark = GetGraduationStatus(secondSemesterStudentResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                }

                                secondSemesterStudentResult.Remark = Remark;
                                ProcessedResult = secondSemesterStudentResult;

                            }

                        }
                        else
                        {
                            decimal firstYearFirstSemesterGPCUSum = 0;
                            int firstYearFirstSemesterTotalCourseUnit = 0;
                            decimal firstYearSecondSemesterGPCUSum = 0;
                            int firstYearSecondSemesterTotalCourseUnit = 0;
                            decimal secondYearFirstSemesterGPCUSum = 0;
                            int secondYearFirstSemesterTotalCourseUnit = 0;
                            decimal secondYearSecondSemesterGPCUSum = 0;
                            int secondYearSecondSemesterTotalCourseUnit = 0;

                            Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(sessionSemester, level, programme, department, student);
                            Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(sessionSemester, level, programme, department, student);
                            if (sessionSemester.Semester.Id == 1)
                            {
                                List<Result> result = null;
                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }

                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;

                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }

                                Result secondYearFirstSemesterResult = new Result();
                                decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                                decimal firstSemesterGPA = 0M;

                                if (firstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                                }

                                secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                                if (firstYearFirstSemester != null && firstYearFirstSemester.GPCU != null && firstYearSecondSemester != null && firstYearSecondSemester.GPCU != null)
                                {
                                    secondYearFirstSemesterResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                    //secondYearFirstSemesterResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                }
                                else
                                {
                                    secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
                                }

                                List<string> secondYearFirstSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                //secondYearFirstSemesterResult.Remark = GetGraduationStatus(secondYearFirstSemesterResult.CGPA, secondYearFirstSemetserCarryOverCourses);

                                decimal firstYearCGPA = Convert.ToDecimal((firstYearSecondSemester.GPCU + firstYearFirstSemester.GPCU) / (firstYearFirstSemester.TotalSemesterCourseUnit + firstYearSecondSemester.TotalSemesterCourseUnit));
                                //this is to state only the status without the carryover in the graduand list 
                                secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatusForGraduands(firstYearCGPA, secondYearFirstSemesterResult.CGPA);
                                //secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatus(firstYearCGPA, secondYearFirstSemesterResult.CGPA, secondYearFirstSemetserCarryOverCourses);

                                ProcessedResult = secondYearFirstSemesterResult;

                            }
                            else if (sessionSemester.Semester.Id == (int)Semesters.SecondSemester)
                            {

                                List<Result> result = null;
                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in result)
                                //{
                                //    decimal WGP = 0;
                                //    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                //    if (totalSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                //    }
                                //    modifiedResultList.Add(resultItem);
                                //}
                                Result secondYearFirstSemesterResult = new Result();
                                decimal secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                                decimal firstSemesterGPA = 0M;
                                if (secondYearfirstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                                }

                                secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                                //Second semester second year

                                List<Result> secondSemesterResult = null;
                                Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                List<Result> secondSemesterModifiedResultList = new List<Result>();
                                int totalSecondSemesterCourseUnit = 0;
                                for (int i = 0; i < secondSemesterResult.Count; i++)
                                {
                                    Result resultItem = secondSemesterResult[i];
                                    decimal WGP = 0;
                                    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);
                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in secondSemesterResult)
                                //{
                                //    decimal WGP = 0;
                                //    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);
                                //    if (totalSecondSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                //    }
                                //    secondSemesterModifiedResultList.Add(resultItem);
                                //}
                                Result secondYearSecondtSemesterResult = new Result();
                                decimal secondYearSecondtSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                                secondYearSecondSemesterTotalSemesterCourseUnit = secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearSecondtSemesterResult = secondSemesterModifiedResultList.FirstOrDefault();
                                secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                                secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                                decimal secondYearSecondSmesterGPA = 0M;
                                if (secondYearSecondtSemesterGPCUSum > 0 && secondYearSecondSemesterTotalSemesterCourseUnit > 0)
                                {
                                    secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                                }
                                secondYearSecondtSemesterResult.GPA = Decimal.Round(secondYearSecondSmesterGPA, 2);

                                firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                                firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;

                                decimal previousCGPA = 0M;
                                var secondSemesterCGPA=Convert.ToDecimal((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit));
                                secondYearSecondtSemesterResult.CGPA = Decimal.Round(secondSemesterCGPA, 2);
                                var previousCGPAConvert = Convert.ToDecimal((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit));
                                //secondYearSecondtSemesterResult.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                                //previousCGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                previousCGPA = Decimal.Round(previousCGPAConvert, 2);
                                List<string> secondYearSecondSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                if (secondYearSecondtSemesterResult.CGPA < (decimal)2.0 && previousCGPA < (decimal)2.0)
                                {
                                    secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, previousCGPA, secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                                }
                                else
                                {
                                    //this is to state only the status without the carryover in the graduand list 
                                    secondYearSecondtSemesterResult.Remark=GetGraduationStatusForGranduands(secondYearSecondtSemesterResult.CGPA);
                                    //secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                                }

                                ProcessedResult = secondYearSecondtSemesterResult;
                            }



                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return ProcessedResult;
        }
        public Result ViewProcessedStudentResultForAggregate(long PersonId, SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            Result ProcessedResult = new Result();
            string Remark = null;
            try
            {
                if (PersonId > 0)
                {
                    Abundance_Nk.Model.Model.Student student = new Model.Model.Student() { Id = PersonId };
                    StudentLogic studentLogic = new StudentLogic();

                    GetStudent(studentLogic, student);

                    StudentResultLogic studentResultLogic = new StudentResultLogic();
                    if (sessionSemester.Semester != null && sessionSemester.Session != null && programme != null && department != null && level != null)
                    {
                        if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                        {
                            Abundance_Nk.Model.Model.Student studentCheck = new Student() { Id = PersonId };
                            GetStudent(studentLogic, studentCheck);
                            //Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == PersonId);
                            if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
                            {
                                List<Result> result = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, sessionSemester.Semester, programme);
                                }

                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }

                                decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                decimal firstSemesterGPA = 0M;
                                if (firstSemesterTotalSemesterCourseUnit != 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
                                }
                                firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.CGPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                                firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                                Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                firstYearFirstSemesterResult.Remark = Remark;
                                ProcessedResult = firstYearFirstSemesterResult;

                            }
                            else
                            {
                                List<Result> result = null;
                                Semester firstSemester = new Semester() { Id = (int)Semesters.FirstSemester };
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, firstSemester, programme);
                                }
                                List<Result> firstSemesterModifiedResultList = new List<Result>();
                                int totalFirstSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalFirstSemesterCourseUnit = CheckForSpecialCase(resultItem, totalFirstSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalFirstSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                    }
                                    firstSemesterModifiedResultList.Add(resultItem);

                                }


                                decimal firstSemesterGPCUSum = firstSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = firstSemesterModifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                decimal firstSemesterGPA = 0M;
                                if (firstSemesterGPCUSum > 0 && firstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit ?? 0;
                                }

                                firstYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.CGPA = Decimal.Round(firstSemesterGPA, 2);

                                Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                                List<Result> secondSemesterResult = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                List<Result> secondSemesterModifiedResultList = new List<Result>();

                                int totalSecondSemesterCourseUnit = 0;
                                for (int i = 0; i < secondSemesterResult.Count; i++)
                                {
                                    Result resultItem = secondSemesterResult[i];
                                    decimal WGP = 0;

                                    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);

                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in secondSemesterResult)
                                //{

                                //    decimal WGP = 0;

                                //    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);

                                //    if (totalSecondSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                //    }
                                //    secondSemesterModifiedResultList.Add(resultItem);
                                //}
                                decimal? secondSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                                Result secondSemesterStudentResult = secondSemesterModifiedResultList.FirstOrDefault();

                                if (secondSemesterGPCUSum > 0)
                                {
                                    secondSemesterStudentResult.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / (decimal)(secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit))), 2);
                                }

                                if (firstSemesterGPCUSum > 0 || secondSemesterGPCUSum > 0)
                                {
                                    secondSemesterStudentResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum) / (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) + firstSemesterTotalSemesterCourseUnit)), 2);

                                }
                                else
                                {
                                    secondSemesterStudentResult.CGPA = secondSemesterStudentResult.GPA;
                                }

                                if (secondSemesterStudentResult.CGPA < (decimal)2.0 && firstYearFirstSemesterResult.GPA < (decimal)2.0)
                                {
                                    Remark = GetGraduationStatus(secondSemesterStudentResult.CGPA, firstYearFirstSemesterResult.CGPA ?? 0, secondSemesterStudentResult.CGPA ?? 0, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                }
                                else
                                {
                                    Remark = GetGraduationStatus(secondSemesterStudentResult.CGPA, GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                }

                                secondSemesterStudentResult.Remark = Remark;
                                ProcessedResult = secondSemesterStudentResult;

                            }

                        }
                        else
                        {
                            decimal firstYearFirstSemesterGPCUSum = 0;
                            int firstYearFirstSemesterTotalCourseUnit = 0;
                            decimal firstYearSecondSemesterGPCUSum = 0;
                            int firstYearSecondSemesterTotalCourseUnit = 0;
                            decimal secondYearFirstSemesterGPCUSum = 0;
                            int secondYearFirstSemesterTotalCourseUnit = 0;
                            decimal secondYearSecondSemesterGPCUSum = 0;
                            int secondYearSecondSemesterTotalCourseUnit = 0;

                            Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfoPrevious(sessionSemester, level, programme, department, student);
                            Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(sessionSemester, level, programme, department, student);
                            if (sessionSemester.Semester.Id == 1)
                            {
                                List<Result> result = null;
                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }

                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;

                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }

                                Result secondYearFirstSemesterResult = new Result();
                                decimal firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                                decimal firstSemesterGPA = 0M;

                                if (firstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                                }

                                secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                                if (firstYearFirstSemester != null && firstYearFirstSemester.GPCU != null && firstYearSecondSemester != null && firstYearSecondSemester.GPCU != null)
                                {
                                    secondYearFirstSemesterResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                }
                                else
                                {
                                    secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
                                }

                                List<string> secondYearFirstSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                //secondYearFirstSemesterResult.Remark = GetGraduationStatus(secondYearFirstSemesterResult.CGPA, secondYearFirstSemetserCarryOverCourses);

                                decimal firstYearCGPA = Convert.ToDecimal((firstYearSecondSemester.GPCU + firstYearFirstSemester.GPCU) / (firstYearFirstSemester.TotalSemesterCourseUnit + firstYearSecondSemester.TotalSemesterCourseUnit));
                                //this is to state only the status without the carryover in the graduand list 
                                secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatusForGraduands(firstYearCGPA, secondYearFirstSemesterResult.CGPA);
                                //secondYearFirstSemesterResult.Remark = GetSecondYearFirstSemeterGraduationStatus(firstYearCGPA, secondYearFirstSemesterResult.CGPA, secondYearFirstSemetserCarryOverCourses);

                                ProcessedResult = secondYearFirstSemesterResult;

                            }
                            else if (sessionSemester.Semester.Id == (int)Semesters.SecondSemester)
                            {

                                List<Result> result = null;
                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = (int)Semesters.FirstSemester };

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, semester, programme);
                                }
                                List<Result> modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    Result resultItem = result[i];
                                    decimal WGP = 0;
                                    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in result)
                                //{
                                //    decimal WGP = 0;
                                //    totalSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSemesterCourseUnit, (int)Semesters.FirstSemester);
                                //    if (totalSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                //    }
                                //    modifiedResultList.Add(resultItem);
                                //}
                                Result secondYearFirstSemesterResult = new Result();
                                decimal secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                                decimal firstSemesterGPA = 0M;
                                if (secondYearfirstSemesterGPCUSum > 0 && secondYearfirstSemesterTotalSemesterCourseUnit > 0)
                                {
                                    firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                                }

                                secondYearFirstSemesterResult.GPA = Decimal.Round(firstSemesterGPA, 2);

                                //Second semester second year

                                List<Result> secondSemesterResult = null;
                                Semester secondSemester = new Semester() { Id = (int)Semesters.SecondSemester };
                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, secondSemester, programme);
                                }
                                List<Result> secondSemesterModifiedResultList = new List<Result>();
                                int totalSecondSemesterCourseUnit = 0;
                                for (int i = 0; i < secondSemesterResult.Count; i++)
                                {
                                    Result resultItem = secondSemesterResult[i];
                                    decimal WGP = 0;
                                    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);
                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                //foreach (Result resultItem in secondSemesterResult)
                                //{
                                //    decimal WGP = 0;
                                //    totalSecondSemesterCourseUnit = CheckForSpecialCase(resultItem, totalSecondSemesterCourseUnit, (int)Semesters.SecondSemester);
                                //    if (totalSecondSemesterCourseUnit > 0)
                                //    {
                                //        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                //    }
                                //    secondSemesterModifiedResultList.Add(resultItem);
                                //}
                                Result secondYearSecondtSemesterResult = new Result();
                                decimal secondYearSecondtSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU) ?? 0;
                                int secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                                secondYearSecondSemesterTotalSemesterCourseUnit = secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) ?? 0;
                                secondYearSecondtSemesterResult = secondSemesterModifiedResultList.FirstOrDefault();
                                secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                                secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                                decimal secondYearSecondSmesterGPA = 0M;
                                if (secondYearSecondtSemesterGPCUSum > 0 && secondYearSecondSemesterTotalSemesterCourseUnit > 0)
                                {
                                    secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                                }
                                secondYearSecondtSemesterResult.GPA = Decimal.Round(secondYearSecondSmesterGPA, 2);

                                firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                                firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;

                                decimal previousCGPA = 0M;
                                var secondSemesterCGPA = Convert.ToDecimal((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit));
                                secondYearSecondtSemesterResult.CGPA = Decimal.Round(secondSemesterCGPA, 2);
                                var previousCGPAConvert = Convert.ToDecimal((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit));
                                //secondYearSecondtSemesterResult.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                                //previousCGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                previousCGPA = Decimal.Round(previousCGPAConvert, 2);
                                List<string> secondYearSecondSemetserCarryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                if (secondYearSecondtSemesterResult.CGPA < (decimal)2.0 && previousCGPA < (decimal)2.0)
                                {
                                    secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, previousCGPA, secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                                }
                                else
                                {
                                    //this is to state only the status without the carryover in the graduand list 
                                    secondYearSecondtSemesterResult.Remark = GetGraduationStatusForGranduands(secondYearSecondtSemesterResult.CGPA);
                                    //secondYearSecondtSemesterResult.Remark = GetGraduationStatus(secondYearSecondtSemesterResult.CGPA, secondYearSecondSemetserCarryOverCourses);
                                }

                                ProcessedResult = secondYearSecondtSemesterResult;
                            }



                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return ProcessedResult;
        }

        private void GetStudent(StudentLogic studentLogic, Student student)
        {
            try
            {
                STUDENT studentEntity = studentLogic.GetEntityBy(s => s.Person_Id == student.Id);
                student.Activated = studentEntity.Activated;
                student.ApplicationForm = new ApplicationForm() { Id = studentEntity.Application_Form_Id ?? 0 };
                student.BloodGroup = new BloodGroup() { Id = studentEntity.Blood_Group_Id ?? 0 };
                student.Category = new StudentCategory() { Id = studentEntity.Student_Category_Id };
                student.Genotype = new Genotype() { Id = studentEntity.Genotype_Id ?? 0 };
                student.MaritalStatus = new MaritalStatus() { Id = studentEntity.Marital_Status_Id ?? 0 };
                student.MatricNumber = studentEntity.Matric_Number;
                student.Number = studentEntity.Student_Number;
                student.Reason = studentEntity.Reason;
                student.RejectCategory = studentEntity.Reject_Category;
                student.SchoolContactAddress = studentEntity.School_Contact_Address;
                student.Status = new StudentStatus() { Id = studentEntity.Student_Status_Id };
                student.Title = new Title() { Id = studentEntity.Title_Id ?? 0 };
                student.Type = new StudentType() { Id = studentEntity.Student_Type_Id };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static int CheckForSpecialCase(Result resultItem, int totalSemesterCourseUnit, int semesterId)
        {
            if (resultItem.SpecialCase != null)
            {
                resultItem.GPCU = 0;
                if (totalSemesterCourseUnit == 0)
                {
                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    //resultItem.Grade = "-";
                    if (resultItem.SpecialCase == "SICK")
                    {
                        resultItem.Grade = "S";
                    }
                    if (resultItem.SpecialCase == "ABSENT")
                    {
                        resultItem.Grade = "ABS";
                    }
                }
                else
                {
                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    //resultItem.Grade = "-";
                    if (resultItem.SpecialCase == "SICK")
                    {
                        resultItem.Grade = "S";
                    }
                    if (resultItem.SpecialCase == "ABSENT")
                    {
                        resultItem.Grade = "ABS";
                    }
                }

            }

            //Check for deferment
            StudentDefermentLogic defermentLogic = new StudentDefermentLogic();
            if (defermentLogic.isStudentDefered(resultItem))
            {
                resultItem.GPCU = 0;
                if (totalSemesterCourseUnit == 0)
                {
                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    resultItem.Grade = "-";
                    resultItem.GPA = null;
                    resultItem.CGPA = null;
                    //resultItem.CourseUnit = 0;
                    resultItem.Remark = "DEFERMENT";
                }
                else
                {
                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    resultItem.Grade = "-";
                    resultItem.GPA = null;
                    resultItem.CGPA = null;
                    //resultItem.CourseUnit = 0;
                    resultItem.Remark = "DEFERMENT";
                }
            }

            //Check for rustication
            if (defermentLogic.isStudentRusticated(resultItem, semesterId))
            {
                resultItem.GPCU = 0;
                if (totalSemesterCourseUnit == 0)
                {
                    totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    resultItem.Grade = "-";
                    resultItem.GPA = null;
                    resultItem.CGPA = null;
                    //resultItem.CourseUnit = 0;
                    resultItem.Remark = "RUSTICATION";
                }
                else
                {
                    totalSemesterCourseUnit -= resultItem.CourseUnit;
                    resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    resultItem.Grade = "-";
                    resultItem.GPA = null;
                    resultItem.CGPA = null;
                    //resultItem.CourseUnit = 0;
                    resultItem.Remark = "RUSTICATION";
                }
            }

            return totalSemesterCourseUnit;
        }

        public int TotalUnitsOmitted { get; set; }
        public List<string> GetFirstYearCarryOverCourses(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Student student)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationdetails = new List<CourseRegistrationDetail>();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<string> courseCodes = new List<string>();
                if (lvl.Id == (int)Levels.NDI || lvl.Id == (int)Levels.HNDI)
                {
                    List<CourseRegistrationDetail> nullCourseRegistrationDetails = new List<CourseRegistrationDetail>();
                    List<CourseRegistrationDetail> nullCourseRegistrationDetailsLessThanPassMark = new List<CourseRegistrationDetail>();

                    nullCourseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score == null || crd.Exam_Score == null) && crd.Special_Case == null);
                    if (nullCourseRegistrationDetails.Count > 0)
                    {
                        for (int i = 0; i < nullCourseRegistrationDetails.Count; i++)
                        {
                            if (nullCourseRegistrationDetails[i].ExamScore == null && nullCourseRegistrationDetails[i].TestScore < (int)Grades.PassMark)
                            {
                                nullCourseRegistrationDetailsLessThanPassMark.Add(nullCourseRegistrationDetails[i]);
                            }
                            else if (nullCourseRegistrationDetails[i].TestScore == null && nullCourseRegistrationDetails[i].ExamScore < (int)Grades.PassMark)
                            {
                                nullCourseRegistrationDetailsLessThanPassMark.Add(nullCourseRegistrationDetails[i]);
                            }
                        }
                    }

                    courseRegistrationdetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score + crd.Exam_Score) < (int)Grades.PassMark && crd.Special_Case == null);
                    if (nullCourseRegistrationDetailsLessThanPassMark.Count > 0)
                    {
                        courseRegistrationdetails.AddRange(nullCourseRegistrationDetailsLessThanPassMark);
                    }

                    if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == (int)Semesters.FirstSemester).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            for (int i = 0; i < courseRegistrationdetails.Count; i++)
                            {
                                CourseRegistrationDetail courseRegistrationDetail = courseRegistrationdetails[i];
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                    TotalUnitsOmitted += courseRegistrationDetail.Course.Unit;
                                }
                            }
                            //foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            //{
                            //    if (courseRegistrationDetail.SpecialCase == null)
                            //    {

                            //        courseCodes.Add(courseRegistrationDetail.Course.Code);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            for (int i = 0; i < courseRegistrationdetails.Count; i++)
                            {
                                CourseRegistrationDetail courseRegistrationDetail = courseRegistrationdetails[i];
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                    TotalUnitsOmitted += courseRegistrationDetail.Course.Unit;
                                }
                            }
                            //foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            //{
                            //    if (courseRegistrationDetail.SpecialCase == null)
                            //    {

                            //        courseCodes.Add(courseRegistrationDetail.Course.Code);
                            //    }
                            //}
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
        public List<string> GetSecondYearCarryOverCourses(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Student student)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationdetails = new List<CourseRegistrationDetail>();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<string> courseCodes = courseCodes = new List<string>();
                List<string> firstYearCarryOverCourseCodes = null;
                StudentLevel studentLevel = null;
                if (lvl.Id == 2)
                {
                    //studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == 1 && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();//ND1
                    //GetStudentLevel(studentLevelLogic, studentLevel, student, new Level(){Id = 1}, department, programme );
                    STUDENT_LEVEL studentLevelEntity = studentLevelLogic.GetEntitiesBy(p => p.Person_Id == student.Id && p.Level_Id == 1 && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                    if (studentLevelEntity != null)
                    {
                        studentLevel = new StudentLevel();
                        studentLevel.Id = studentLevelEntity.Student_Level_Id;
                        studentLevel.Department = department;
                        if (studentLevelEntity.Department_Option_Id != null && studentLevelEntity.Department_Option_Id > 0)
                        {
                            studentLevel.DepartmentOption = new DepartmentOption() { Id = studentLevelEntity.Department_Option_Id ?? 0 };
                        }
                        else
                        {
                            studentLevel.DepartmentOption = null;
                        }

                        studentLevel.Level = new Level() { Id = 1 };
                        studentLevel.Programme = programme;
                        studentLevel.Session = new Session() { Id = studentLevelEntity.Session_Id };
                        studentLevel.Student = student;
                    }
                    if (studentLevel != null)
                    {
                        SessionSemester ss = new SessionSemester();
                        ss.Session = studentLevel.Session;
                        ss.Semester = new Semester() { Id = 2 };// Second semester to get all carry over for first year
                        firstYearCarryOverCourseCodes = GetFirstYearCarryOverCourses(ss, studentLevel.Level, studentLevel.Programme, studentLevel.Department, studentLevel.Student);
                    }
                }
                else if (lvl.Id == 4)
                {
                    //studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == 3 && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault(); //HND1
                    STUDENT_LEVEL studentLevelEntity = studentLevelLogic.GetEntitiesBy(p => p.Person_Id == student.Id && p.Level_Id == 3 && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                    if (studentLevelEntity != null)
                    {
                        studentLevel = new StudentLevel();
                        studentLevel.Id = studentLevelEntity.Student_Level_Id;
                        studentLevel.Department = department;
                        if (studentLevelEntity.Department_Option_Id != null && studentLevelEntity.Department_Option_Id > 0)
                        {
                            studentLevel.DepartmentOption = new DepartmentOption() { Id = studentLevelEntity.Department_Option_Id ?? 0 };
                        }
                        else
                        {
                            studentLevel.DepartmentOption = null;
                        }

                        studentLevel.Level = new Level() { Id = 3 };
                        studentLevel.Programme = programme;
                        studentLevel.Session = new Session() { Id = studentLevelEntity.Session_Id };
                        studentLevel.Student = student;
                    }
                    if (studentLevel != null)
                    {
                        SessionSemester ss = new SessionSemester();
                        ss.Session = studentLevel.Session;
                        ss.Semester = new Semester() { Id = 2 }; // Second semester to get all carry over for first year
                        firstYearCarryOverCourseCodes = GetFirstYearCarryOverCourses(ss, studentLevel.Level, studentLevel.Programme, studentLevel.Department, studentLevel.Student);
                    }
                }

                if (lvl.Id == 2 || lvl.Id == 4)
                {
                    List<CourseRegistrationDetail> nullCourseRegistrationDetails = new List<CourseRegistrationDetail>();
                    List<CourseRegistrationDetail> nullCourseRegistrationDetailsLessThanPassMark = new List<CourseRegistrationDetail>();

                    nullCourseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score == null || crd.Exam_Score == null) && crd.Special_Case == null);
                    if (nullCourseRegistrationDetails.Count > 0)
                    {
                        for (int i = 0; i < nullCourseRegistrationDetails.Count; i++)
                        {
                            if (nullCourseRegistrationDetails[i].ExamScore == null && nullCourseRegistrationDetails[i].TestScore < (int)Grades.PassMark)
                            {
                                nullCourseRegistrationDetailsLessThanPassMark.Add(nullCourseRegistrationDetails[i]);
                            }
                            else if (nullCourseRegistrationDetails[i].TestScore == null && nullCourseRegistrationDetails[i].ExamScore < (int)Grades.PassMark)
                            {
                                nullCourseRegistrationDetailsLessThanPassMark.Add(nullCourseRegistrationDetails[i]);
                            }
                        }
                    }

                    courseRegistrationdetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score + crd.Exam_Score) < (int)Grades.PassMark && crd.Special_Case == null);
                    if (nullCourseRegistrationDetailsLessThanPassMark.Count > 0)
                    {
                        courseRegistrationdetails.AddRange(nullCourseRegistrationDetailsLessThanPassMark);
                    }

                    if (sessionSemester.Semester.Id == 1)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == 1).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            for (int i = 0; i < courseRegistrationdetails.Count; i++)
                            {
                                CourseRegistrationDetail courseRegistrationDetail = courseRegistrationdetails[i];
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                    TotalUnitsOmitted += courseRegistrationDetail.Course.Unit;
                                }
                            }
                            //foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            //{
                            //    if (courseRegistrationDetail.SpecialCase == null)
                            //    {
                            //        courseCodes.Add(courseRegistrationDetail.Course.Code);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            for (int i = 0; i < courseRegistrationdetails.Count; i++)
                            {
                                CourseRegistrationDetail courseRegistrationDetail = courseRegistrationdetails[i];
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                    TotalUnitsOmitted += courseRegistrationDetail.Course.Unit;
                                }
                            }
                            //foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            //{
                            //    if (courseRegistrationDetail.SpecialCase == null)
                            //    {

                            //        courseCodes.Add(courseRegistrationDetail.Course.Code);
                            //    }
                            //}
                        }
                    }

                }
                //compare courses
                courseCodes = CompareCourses(courseCodes, firstYearCarryOverCourseCodes, sessionSemester, lvl, programme, department, student);
                return courseCodes;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<string> CompareCourses(List<string> courseCodes, List<string> firstYearCarryOverCourseCodes, SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Student student)
        {

            try
            {
                CourseRegistrationDetailLogic courseRegistrationDetail = new CourseRegistrationDetailLogic();

                if (courseCodes != null && firstYearCarryOverCourseCodes != null)
                {
                    int firstYearCount = firstYearCarryOverCourseCodes.Count;
                    for (int i = 0; i < firstYearCount; i++)
                    {
                        if (courseCodes.Contains(firstYearCarryOverCourseCodes[i]))
                        {
                            string code = firstYearCarryOverCourseCodes[i];
                            //courseCodes.Add(firstYearCarryOverCourseCodes[i]);
                            //firstYearCarryOverCourseCodes.RemoveAt(i);
                            STUDENT_COURSE_REGISTRATION_DETAIL course = courseRegistrationDetail.GetEntitiesBy(p => p.COURSE.Course_Code == code && p.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id).LastOrDefault();
                            if (course != null)
                            {
                                TotalUnitsOmitted -= course.Course_Unit ?? 0;
                            }
                        }
                        else
                        {
                            string Coursecode = firstYearCarryOverCourseCodes[i];
                            CourseRegistrationDetail course = courseRegistrationDetail.GetModelsBy(p => p.COURSE.Course_Code == Coursecode && p.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id).LastOrDefault();
                            if (course != null)
                            {
                                int courseUnitToRemove = course.CourseUnit ?? 0;
                                TotalUnitsOmitted -= courseUnitToRemove;
                                //firstYearCarryOverCourseCodes.RemoveAt(i);
                                TotalUnitsOmitted = TotalUnitsOmitted < 0 ? 0 : TotalUnitsOmitted;
                            }
                            else
                            {
                                courseCodes.Add(firstYearCarryOverCourseCodes[i]);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return courseCodes;
        }
        private Result GetFirstYearFirstSemesterResultInfo(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                Result firstYearFirstSemesterResult = new Result();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = new Student() { Id = student.Id };
                //Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                GetStudent(studentLogic, studentCheck);

                Semester semester = new Semester() { Id = 1 };
                Level level = null;
                Session session = null;
                if (lvl.Id == 2)
                {
                    level = new Level() { Id = 1 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = null;
                //StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                studentLevel = GetStudentLevel(studentLevelLogic, studentLevel, student, level, department, programme);
                //if (studentLevel != null && studentLevel.Session != null)
                //{
                    if (student.Activated == true || studentCheck.Activated == null)
                    {
                        result = studentResultLogic.GetStudentProcessedResultBy(session, level,
                            department, student, semester, programme);
                    }
                    else
                    {
                        result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level,
                            studentLevel.Department, student, semester, studentLevel.Programme);
                    }


                    List<Result> modifiedResultList = new List<Result>();
                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {
                        decimal WGP = 0;
                        if (resultItem.SpecialCase != null)
                        {

                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }

                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }
                        modifiedResultList.Add(resultItem);
                    //}

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {

                throw;
            }

        }
        private Result GetFirstYearFirstSemesterResultInfoPrevious(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                Result firstYearFirstSemesterResult = new Result();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = new Student() { Id = student.Id };
                //Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                GetStudent(studentLogic, studentCheck);

                Semester semester = new Semester() { Id = 1 };
                Level level = null;
                Session session = null;
                if (lvl.Id == 2)
                {
                    level = new Level() { Id = 1 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = null;
                //StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                studentLevel = GetStudentLevel(studentLevelLogic, studentLevel, student, level, department, programme);
                //if (studentLevel != null && studentLevel.Session != null)
                //{
                if (student.Activated == true || studentCheck.Activated == null)
                {
                    result = studentResultLogic.GetStudentPreviousProcessedResultBy(session, level,
                        department, student, semester, programme);
                }
                else
                {
                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level,
                        studentLevel.Department, student, semester, studentLevel.Programme);
                }


                List<Result> modifiedResultList = new List<Result>();
                int totalSemesterCourseUnit = 0;
                foreach (Result resultItem in result)
                {
                    decimal WGP = 0;
                    if (resultItem.SpecialCase != null)
                    {

                        resultItem.GPCU = 0;
                        if (totalSemesterCourseUnit == 0)
                        {
                            totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }
                        else
                        {
                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }

                    }
                    if (totalSemesterCourseUnit > 0)
                    {
                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    }
                    modifiedResultList.Add(resultItem);
                    //}

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.PreviousGPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.PreviousTotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private StudentLevel GetStudentLevel(StudentLevelLogic studentLevelLogic, StudentLevel studentLevel, Student student, Level level, Department department, Programme programme)
        {
            try
            {
                STUDENT_LEVEL studentLevelEntity = studentLevelLogic.GetEntitiesBy(p => p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                if (studentLevelEntity != null)
                {
                    studentLevel = new StudentLevel();
                    studentLevel.Id = studentLevelEntity.Student_Level_Id;
                    studentLevel.Department = department;

                    if (studentLevelEntity.Department_Option_Id != null && studentLevelEntity.Department_Option_Id > 0)
                    {
                        studentLevel.DepartmentOption = new DepartmentOption() { Id = studentLevelEntity.Department_Option_Id ?? 0 };
                    }
                    else
                    {
                        studentLevel.DepartmentOption = null;
                    }

                    studentLevel.Level = level;
                    studentLevel.Programme = programme;
                    studentLevel.Session = new Session() { Id = studentLevelEntity.Session_Id };
                    studentLevel.Student = student;
                }
                else
                {
                    studentLevel = null;
                }

                return studentLevel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Result GetFirstYearSecondSemesterResultInfo(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Model.Model.Student student)
        {
            try
            {
                Result firstYearFirstSemesterResult = new Result();
                List<Result> modifiedResultList = new List<Result>();
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                Semester semester = new Semester() { Id = 2 };
                Level level = null;
                Session session = null;
                if (lvl.Id == 2 || lvl.Id == 4)
                {
                    level = new Level() { Id = 1 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = null;
                studentLevel = GetStudentLevel(studentLevelLogic, studentLevel, student, level, department, programme);
                //StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                //if (studentLevel != null && studentLevel.Session != null)
                //{

                    if (student.Activated == true || studentCheck.Activated == null)
                    {
                    //result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, student, semester, studentLevel.Programme);
                    result = studentResultLogic.GetStudentPreviousProcessedResultBy(session, level, department, student, semester, programme);
                }
                    else
                    {
                    //result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, student, semester, studentLevel.Programme);
                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, student, semester, programme);
                }

                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {

                        decimal WGP = 0;

                        if (resultItem.SpecialCase != null)
                        {

                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }
                        modifiedResultList.Add(resultItem);
                    //}

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private Result GetFirstYearSecondSemesterResultInfoPrevious(SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Model.Model.Student student)
        {
            try
            {
                Result firstYearFirstSemesterResult = new Result();
                List<Result> modifiedResultList = new List<Result>();
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                Semester semester = new Semester() { Id = 2 };
                Level level = null;
                Session session = null;
                if (lvl.Id == 2)
                {
                    level = new Level() { Id = 1 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                    session = new Session() { Id = sessionSemester.Session.Id - 1 };
                }

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = null;
                studentLevel = GetStudentLevel(studentLevelLogic, studentLevel, student, level, department, programme);
                //StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id).FirstOrDefault();
                //if (studentLevel != null && studentLevel.Session != null)
                //{

                if (student.Activated == true || studentCheck.Activated == null)
                {
                    //result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, student, semester, studentLevel.Programme);
                    result = studentResultLogic.GetStudentPreviousProcessedResultBy(session, level, department, student, semester, programme);
                }
                else
                {
                    //result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level, studentLevel.Department, student, semester, studentLevel.Programme);
                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, student, semester, programme);
                }

                int totalSemesterCourseUnit = 0;
                foreach (Result resultItem in result)
                {

                    decimal WGP = 0;

                    if (resultItem.SpecialCase != null)
                    {

                        resultItem.GPCU = 0;
                        if (totalSemesterCourseUnit == 0)
                        {
                            totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }
                        else
                        {
                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                            resultItem.Grade = "-";
                        }
                    }
                    if (totalSemesterCourseUnit > 0)
                    {
                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                    }
                    modifiedResultList.Add(resultItem);
                    //}

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.PreviousGPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.PreviousTotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;

                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<string> GetCarryOverCourseCodes(SessionSemester ss, Level lvl, Programme programme, Department department, Model.Model.Student student)
        {
            try
            {
                List<string> courseCodes = new List<string>();
                List<CourseRegistrationDetail> courseRegistrationdetails = new List<CourseRegistrationDetail>();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();


                Level level = null;
                if (lvl.Id == 1 || lvl.Id == 3)
                {
                    level = new Level() { Id = 1 };
                    courseCodes = GetFirstYearCarryOverCourses(ss, lvl, programme, department, student);

                }
                else
                {
                    level = new Level() { Id = 3 };
                }
                courseRegistrationdetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == ss.Session.Id && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && (crd.Test_Score + crd.Exam_Score) < 40 && crd.Special_Case == null);

                if (courseRegistrationdetails != null)
                {
                    if (ss.Semester.Id == 1)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == 1).ToList();
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
        private string GetGraduationStatus(decimal? CGPA, List<string> courseCodes)
        {
            string remark = null;
            try
            {
                if (courseCodes.Count == 0 && CGPA != null)
                {
                    if (CGPA >= (decimal)3.5 && CGPA <= (decimal)4.0)
                    {
                        remark = "RHL; PASSED: DISTINCTION";
                        //remark = "DISTINCTION";
                    }
                    else if (CGPA >= (decimal)3.25 && CGPA <= (decimal)3.49)
                    {
                        remark = "DHL; PASSED: UPPER CREDIT";
                        //remark = "UPPER CREDIT";
                    }
                    else if (CGPA >= (decimal)3.0 && CGPA < (decimal)3.25)
                    {
                        remark = "PAS; PASSED: UPPER CREDIT";
                        //remark = "UPPER CREDIT";
                    }
                    else if (CGPA >= (decimal)2.5 && CGPA <= (decimal)2.99)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";
                        //remark = "LOWER CREDIT";
                    }
                    else if (CGPA >= (decimal)2.0 && CGPA <= (decimal)2.49)
                    {
                        remark = "PAS; PASSED: PASS";
                        //remark = "PASS";
                    }
                    else if (CGPA < (decimal)2.0)
                    {
                        remark = "PROBATION";
                    }
                }
                else
                {
                    if (CGPA < (decimal)2.0)
                    {
                        remark = "PROBATION / CO-";
                        for (int i = 0; i < courseCodes.Count(); i++)
                        {
                            remark += ("|" + courseCodes[i]);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remark;
        }
        private string GetGraduationStatusForGranduands(decimal? CGPA)
        {
            string remark = null;
            try
            {
                    if (CGPA >= (decimal)3.5 && CGPA <= (decimal)4.0)
                    {
                        remark = "DISTINCTION";
                    }
                    else if (CGPA >= (decimal)3.25 && CGPA <= (decimal)3.49)
                    {
                        remark = "UPPER CREDIT";
                    }
                    else if (CGPA >= (decimal)3.0 && CGPA < (decimal)3.25)
                    {

                        remark = "UPPER CREDIT";
                    }
                    else if (CGPA >= (decimal)2.5 && CGPA <= (decimal)2.99)
                    {
                        remark = "LOWER CREDIT";
                    }
                    else if (CGPA >= (decimal)2.0 && CGPA <= (decimal)2.49)
                    {
                        remark = "PASS";
                    }
                    else if (CGPA < (decimal)2.0)
                    {
                        remark = "PROBATION";
                    }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remark;
        }
        private string GetGraduationStatus(decimal? CGPA, decimal? previousCGPA, decimal? currentCGPA, List<string> courseCodes)
        {
            string remark = null;
            try
            {
                if (previousCGPA != null && currentCGPA != null)
                {
                    if (previousCGPA < (decimal)2.0 && currentCGPA < (decimal)2.0)
                    {
                        remark = "WITHRADWN ";
                    }
                    else if (previousCGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }
                    else if (currentCGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }
                    if (courseCodes.Count != 0)
                    {
                        remark += "CO-";
                        for (int i = 0; i < courseCodes.Count(); i++)
                        {
                            remark += ("|" + courseCodes[i]);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return remark;
        }
        private string GetSecondYearFirstSemeterGraduationStatus(decimal? firstSemesterGPA, decimal? secondSemesterGPA, List<string> courseCodes)
        {
            string remark = null;
            try
            {
                if (firstSemesterGPA != null && secondSemesterGPA != null)
                {
                    if (firstSemesterGPA < (decimal)2.0 && secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "WITHRADWN ";
                    }
                    //else if (firstSemesterGPA < (decimal)2.0)
                    //{
                    //    remark = "PROBATION ";
                    //}
                    //else if (secondSemesterGPA < (decimal)2.0)
                    //{
                    //    remark = "PROBATION ";
                    //}
                    else if (secondSemesterGPA >= (decimal)3.5 && secondSemesterGPA <= (decimal)4.0)
                    {
                        remark = "RHL; PASSED: DISTINCTION";
                    }
                    else if (secondSemesterGPA >= (decimal)3.25 && secondSemesterGPA <= (decimal)3.49)
                    {
                        remark = "DHL; PASSED: UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)3.0 && secondSemesterGPA < (decimal)3.25)
                    {
                        remark = "PAS; PASSED: UPPER CREDIT";
                        //remark = "UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.5 && secondSemesterGPA <= (decimal)2.99)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";
                        //remark = "LOWER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.0 && secondSemesterGPA <= (decimal)2.49)
                    {
                        remark = "PAS; PASSED: PASS";
                        //remark = "PASS";
                    }
                    else if (secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }

                    if (courseCodes.Count != 0)
                    {
                        remark += "CO-";
                        for (int i = 0; i < courseCodes.Count(); i++)
                        {
                            remark += ("|" + courseCodes[i]);
                        }
                    }
                }
                else if (firstSemesterGPA == null && secondSemesterGPA != null)
                {
                    if (secondSemesterGPA >= (decimal)3.5 && secondSemesterGPA <= (decimal)4.0)
                    {
                        remark = "RHL; PASSED: DISTINCTION";
                    }
                    else if (secondSemesterGPA >= (decimal)3.25 && secondSemesterGPA <= (decimal)3.49)
                    {
                        remark = "DHL; PASSED: UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)3.0 && secondSemesterGPA < (decimal)3.25)
                    {
                        remark = "PAS; PASSED: UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.5 && secondSemesterGPA <= (decimal)2.99)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.0 && secondSemesterGPA <= (decimal)2.49)
                    {
                        remark = "PAS; PASSED: PASS";
                    }
                    else if (secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }

                    if (courseCodes.Count != 0)
                    {
                        remark += "CO-";
                        for (int i = 0; i < courseCodes.Count(); i++)
                        {
                            remark += ("|" + courseCodes[i]);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return remark;
        }
        private string GetSecondYearFirstSemeterGraduationStatusForGraduands(decimal? firstyearCGPA, decimal? secondSemesterGPA)
        {
            string remark = null;
            try
            {
                if (firstyearCGPA != null && secondSemesterGPA != null)
                {
                    if (firstyearCGPA < (decimal)2.0 && secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "WITHRADWN ";
                    }
                    //else if (firstSemesterGPA < (decimal)2.0)
                    //{
                    //    remark = "PROBATION ";
                    //}
                    //else if (secondSemesterGPA < (decimal)2.0)
                    //{
                    //    remark = "PROBATION ";
                    //}
                    else if (secondSemesterGPA >= (decimal)3.5 && secondSemesterGPA <= (decimal)4.0)
                    {
                        remark = "DISTINCTION";
                    }
                    else if (secondSemesterGPA >= (decimal)3.25 && secondSemesterGPA <= (decimal)3.49)
                    {
                        remark = "UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)3.0 && secondSemesterGPA < (decimal)3.25)
                    {
                        remark = "UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.5 && secondSemesterGPA <= (decimal)2.99)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";

                    }
                    else if (secondSemesterGPA >= (decimal)2.0 && secondSemesterGPA <= (decimal)2.49)
                    {

                        remark = "PASS";
                    }
                    else if (secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }


                }
                else
                {
                    if (secondSemesterGPA >= (decimal)3.5 && secondSemesterGPA <= (decimal)4.0)
                    {
                        remark = "DISTINCTION";
                    }
                    else if (secondSemesterGPA >= (decimal)3.25 && secondSemesterGPA <= (decimal)3.49)
                    {
                        remark = "UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)3.0 && secondSemesterGPA < (decimal)3.25)
                    {
                        remark = "UPPER CREDIT";
                    }
                    else if (secondSemesterGPA >= (decimal)2.5 && secondSemesterGPA <= (decimal)2.99)
                    {
                        remark = "PAS; PASSED: LOWER CREDIT";

                    }
                    else if (secondSemesterGPA >= (decimal)2.0 && secondSemesterGPA <= (decimal)2.49)
                    {

                        remark = "PASS";
                    }
                    else if (secondSemesterGPA < (decimal)2.0)
                    {
                        remark = "PROBATION ";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remark;
        }
        public List<UploadedCourseFormat> GetUploadedCourses(Session session, Semester semester, Programme programme, Department department, Level level)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || level == null || level.Id <= 0)
                {
                    throw new Exception("One or more criteria to get the uploaded courses is not set! Please check your input criteria selection and try again.");
                }

                List<UploadedCourseFormat> uploadedCourses = (from uc in repository.GetBy<VW_UPLOADED_COURSES>(x => x.Session_Id == session.Id && x.Semester_Id == semester.Id && x.Department_Id == department.Id && x.Level_Id == level.Id)
                                                              select new UploadedCourseFormat
                                                              {
                                                                  Level = uc.Level_Name,
                                                                  Department = uc.Department_Name,
                                                                  CourseCode = uc.Course_Code,
                                                                  CourseTitle = uc.Course_Name,
                                                                  DepartmentId = uc.Department_Id,
                                                                  SessionId = uc.Session_Id,
                                                                  SemesterId = uc.Semester_Id,
                                                                  LevelId = uc.Level_Id,
                                                                  CourseId = uc.Course_Id,
                                                                  
                                                              }).ToList();

                StudentExamRawScoreSheetResultLogic rawScoreLogic = new StudentExamRawScoreSheetResultLogic();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                List<UploadedCourseFormat> masterUploadedCourseFormats = new List<UploadedCourseFormat>();

                for (int i = 0; i < uploadedCourses.Count; i++)
                {
                    UploadedCourseFormat currentUploadedCourseFormat = uploadedCourses[i];

                    CourseAllocation courseAllocation = courseAllocationLogic.GetModelsBy(c => c.Session_Id == currentUploadedCourseFormat.SessionId && c.Semester_Id == currentUploadedCourseFormat.SemesterId && c.Course_Id == currentUploadedCourseFormat.CourseId && c.Department_Id == currentUploadedCourseFormat.DepartmentId && c.Level_Id == currentUploadedCourseFormat.LevelId && c.Programme_Id == programme.Id).LastOrDefault();

                    if (courseAllocation != null)
                    {
                        List<UploadedCourseFormat> specifiCourseFormats = uploadedCourses.Where(u => u.CourseId == currentUploadedCourseFormat.CourseId && u.DepartmentId == currentUploadedCourseFormat.DepartmentId && u.LevelId == currentUploadedCourseFormat.LevelId && u.SemesterId == currentUploadedCourseFormat.SemesterId && u.SessionId == currentUploadedCourseFormat.SessionId).ToList();
                        for (int k = 0; k < specifiCourseFormats.Count; k++)
                        {
                            specifiCourseFormats[k].Programme = courseAllocation.Programme.Name;
                            specifiCourseFormats[k].LecturerName = courseAllocation.User.Username;
                            specifiCourseFormats[k].ProgrammeId = courseAllocation.Programme.Id;
                            specifiCourseFormats[k].CourseAllocationId = courseAllocation.Id;
                        }
                    }
                }

                masterUploadedCourseFormats = uploadedCourses.Where(u => u.ProgrammeId == programme.Id).ToList();

                return masterUploadedCourseFormats.OrderBy(uc => uc.Department).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<UploadedCourseFormat> GetUploadedAlternateCourses(Session session, Semester semester)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0)
                {
                    throw new Exception("One or more criteria to get the uploaded courses is not set! Please check your input criteria selection and try again.");
                }
                List<UploadedCourseFormat> uploadedCourses = (from uc in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(x => x.Session_Id == session.Id && x.Semester_Id == semester.Id)
                                                              select new UploadedCourseFormat
                                                              {
                                                                  Programme = uc.Programme_Name,
                                                                  Level = uc.Level_Name,
                                                                  Department = uc.Department_Name,
                                                                  CourseCode = uc.Course_Code,
                                                                  CourseTitle = uc.Course_Name,
                                                                  LecturerName = "",
                                                                  ProgrammeId = uc.Programme_Id,
                                                                  DepartmentId = 1,
                                                                  SessionId = uc.Session_Id,
                                                                  SemesterId = uc.Semester_Id,
                                                                  LevelId = uc.Level_Id,
                                                                  CourseId = uc.Course_Id
                                                              }).ToList();

                return uploadedCourses.OrderBy(uc => uc.Programme).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetIdentifierBy(STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED rawscoresheetItem)
        {
            try
            {
                string identifier = null;
                string departmentCode = rawscoresheetItem.COURSE.DEPARTMENT.Department_Code;
                string level = rawscoresheetItem.LEVEL.Level_Name;
                string semesterCode = GetSemesterCodeBy(rawscoresheetItem.Semester_Id);
                string sessionCode = GetSessionCodeBy(rawscoresheetItem.SESSION.Session_Name);
                identifier = departmentCode + level + semesterCode + sessionCode;
                return identifier;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetSessionCodeBy(string sessionName)
        {
            try
            {
                string sessionCode = null;
                string[] sessionArray = sessionName.Split('/');
                string sessionYear = sessionArray[1];
                sessionCode = sessionYear.Substring(2, 2);
                return sessionCode;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetSemesterCodeBy(int semesterId)
        {
            try
            {
                if (semesterId == 1)
                {
                    return "F";
                }
                else
                {
                    return "S";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetDepartmentCode(int departmentid)
        {
            string code = "";
            try
            {
                DepartmentLogic departmentLogic = new DepartmentLogic();
                code = departmentLogic.GetModelBy(m => m.Department_Id == departmentid).Code;
            }
            catch (Exception)
            {

                throw;
            }
            return code;
        }
        private string GetLevelName(int levelId)
        {
            string code = "";
            try
            {
                LevelLogic levelLogic = new LevelLogic();
                code = levelLogic.GetModelBy(m => m.Level_Id == levelId).Name;
            }
            catch (Exception)
            {

                throw;
            }
            return code;
        }
        public List<ResultUpdateModel> GetResultUpdates(Session session, Semester semester, Programme programme, Department department, Level level, Course course)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || level == null || level.Id <= 0 || course == null || course.Id <= 0)
                {
                    throw new Exception("One or more criteria to get the uploaded courses is not set! Please check your input criteria selection and try again.");
                }
                List<ResultUpdateModel> resultUpdates = (from uc in repository.GetBy<VW_RESULT_UPDATES>(x => x.Session_Id == session.Id && x.Semester_Id == semester.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Course_Id == course.Id)
                                                         select new ResultUpdateModel
                                                         {
                                                             Programme = uc.Programme_Name,
                                                             Level = uc.Level_Name,
                                                             Department = uc.Department_Name,
                                                             CourseCode = uc.Course_Code,
                                                             CourseTitle = uc.Course_Name,
                                                             StaffName = uc.User_Name,
                                                             Session = uc.Session_Name,
                                                             Semester = uc.Semester_Name,
                                                             MatricNumber = uc.Matric_Number,
                                                             LastModifiedDate = uc.Date_Uploaded.ToLongDateString(),
                                                             UserId = uc.User_Id
                                                         }).ToList();

                StaffLogic staffLogic = new StaffLogic();
                for (int i = 0; i < resultUpdates.Count; i++)
                {
                    ResultUpdateModel currentModel = resultUpdates[i];
                    Staff staff = staffLogic.GetModelBy(s => s.User_Id == currentModel.UserId);
                    if (staff != null)
                    {
                        resultUpdates[i].StaffName = staff.FullName;
                    }
                }

                return resultUpdates.OrderBy(r => r.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetGraduationList(SessionSemester sessionSemester, Level level, Programme programme, Department department, int type)
        {
            List<Result> masterSheetResult = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string currentSessionSuffix = sessionNameStr.Substring(2, 2);
                string yearTwoSessionSuffix = Convert.ToString((Convert.ToInt32(currentSessionSuffix) - 1));

                currentSessionSuffix = "/" + currentSessionSuffix + "/";
                yearTwoSessionSuffix = "/" + yearTwoSessionSuffix + "/";

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                if (ss.Session.Id == (int)Sessions._20152016)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                                   SessionName = sr.Session_Name,
                                   Surname = sr.Last_Name.ToUpper(),
                                   FacultyName = sr.Faculty_Name,
                                   WGP=sr.WGP,
                                   Othername = sr.Othernames.ToUpper()
                                   
                               }).ToList();

                }
                else
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && (x.Activated == true))
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name.ToUpper(),
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
                                   SessionName = sr.Session_Name,
                                   Surname = sr.Last_Name.ToUpper(),
                                   FacultyName = sr.Faculty_Name,
                                   WGP = sr.WGP,
                                   Othername = sr.Othernames.ToUpper()
                                   
                               }).ToList();
                    List<Result> resultList = new List<Result>();

                    //for (int i = 0; i < results.Count; i++)
                    //{
                    //    if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                    //    {
                    //        //Do Nothing
                    //    }
                    //    else
                    //    {
                    //        resultList.Add(results[i]);
                    //    }
                    //}

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains(yearTwoSessionSuffix))
                        {
                            resultList.Add(results[i]);
                        }
                        else
                        {
                            //Do Nothing
                        }

                    }


                    results = new List<Result>();
                    results = resultList;
                }

                List<Result> classResult = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                                  (x.Activated == true))
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
                                                SessionName = sr.Session_Name,
                                                CourseModeId = sr.Course_Mode_Id
                                            }).ToList();
                List<long> distinctStudents = results.Select(r => r.StudentId).Distinct().ToList();
                for (int i = 0; i < distinctStudents.Count; i++)
                {
                    long currentStudentId = distinctStudents[i];
                    Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);

                    Result confirmedResult = ViewProcessedStudentResult(currentStudentId, ss, level, programme, department);
                    //confirmedResult.UnitOutstanding = TotalUnitsOmitted;

                    bool hasSpecialCaseOutstanding = CheckSpecialCase(classResult, currentStudentId);




                    //long currentStudentId = distinctStudents[i];
                    //Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);
                    if (confirmedResult != null)
                    {
                        currentStudentResult.CGPA = confirmedResult.CGPA;
                        currentStudentResult.Remark = confirmedResult.Remark;
                        currentStudentResult.Identifier = identifier;
                        currentStudentResult.Count = 1;
                        masterSheetResult.Add(currentStudentResult);
                    }

                }

                switch (type)
                {
                    //case 1:
                    //    return masterSheetResult.OrderBy(a => a.Remark).ToList();
                    //    break;
                    case 1:
                        return masterSheetResult.OrderByDescending(a => a.CGPA).ToList();
                        break;
                    case 2:
                        return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
                        break;
                    case 3:
                        return masterSheetResult.OrderBy(a => a.Name).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
        }
        private bool CheckSpecialCase(List<Result> classResult, long studentId)
        {
            try
            {
                List<Result> specialCases = classResult.Where(r => r.StudentId == studentId && r.SpecialCase != null).ToList();
                if (specialCases == null || specialCases.Count <= 0)
                    return false;

                bool hasSpecialCase = false;

                for (int i = 0; i < specialCases.Count; i++)
                {
                    Result specialCase = specialCases[i];

                    Result hasPassedSpecialCase = classResult.LastOrDefault(r => r.StudentId == studentId && r.CourseId == specialCase.CourseId &&
                                                                            ((r.TestScore ?? 0) + (r.ExamScore ?? 0) >= 40));
                    if (hasPassedSpecialCase == null)
                    {
                        hasSpecialCase = true;
                    }
                }

                return hasSpecialCase;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetDiplomaClassList(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            List<Result> masterSheetResult = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                if (ss.Session.Id == (int)Sessions._20152016)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && (x.Activated == true || x.Activated == null))
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
                                   SessionName = sr.Session_Name
                               }).ToList();

                }
                else
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && (x.Activated == true || x.Activated == null))
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name.ToUpper(),
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
                                   SessionName = sr.Session_Name
                               }).ToList();

                    //List<Result> resultList = new List<Result>();

                    //for (int i = 0; i < results.Count; i++)
                    //{
                    //    if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                    //    {
                    //        //Do Nothing
                    //    }
                    //    else
                    //    {
                    //        resultList.Add(results[i]);
                    //    }
                    //}

                    //results = new List<Result>();
                    // results = resultList;
                }


                List<long> distinctStudents = results.Select(r => r.StudentId).Distinct().ToList();
                for (int i = 0; i < distinctStudents.Count; i++)
                {
                    long currentStudentId = distinctStudents[i];
                    Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);
                    Result confirmedResult = GetStudentResultBy(ss, level, programme, department, new Student() { Id = currentStudentId }).LastOrDefault();
                    if (confirmedResult != null)
                    {
                        currentStudentResult.CGPA = confirmedResult.CGPA;
                        currentStudentResult.Remark = confirmedResult.Remark;
                        currentStudentResult.Identifier = identifier;
                        currentStudentResult.Count = 1;
                        masterSheetResult.Add(currentStudentResult);
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
        }

        public List<Result> GetGraduationListByOption(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption, int type)
        {
            List<Result> masterSheetResult = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string currentSessionSuffix = sessionNameStr.Substring(2, 2);
                string yearTwoSessionSuffix = Convert.ToString((Convert.ToInt32(currentSessionSuffix) - 1));

                currentSessionSuffix = "/" + currentSessionSuffix + "/";
                yearTwoSessionSuffix = "/" + yearTwoSessionSuffix + "/";

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                DepartmentOption option = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOption.Id);

                if (ss.Session.Id == (int)Sessions._20152016)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                 x =>
                                     x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                     x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                     x.Department_Id == department.Id &&
                                     x.Department_Option_Id == departmentOption.Id &&
                                     (x.Activated != false || x.Activated == null))
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode = sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                   DepartmentOptionId = sr.Department_Option_Id,
                                   DepartmentOptionName = sr.Department_Option_Name,
                                   SessionName = sr.Session_Name,
                                   Surname = sr.Last_Name.ToUpper(),
                                   FacultyName = sr.Faculty_Name,
                                   WGP = sr.WGP,
                                   Othername = sr.Othernames.ToUpper()
                               }).ToList();
                }
                else
                {
                    results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                x =>
                                    x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                    x.Level_Id == level.Id &&
                                    x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                    x.Department_Option_Id == departmentOption.Id &&
                                    (x.Activated != false || x.Activated == null))
                         select new Result
                         {
                             StudentId = sr.Person_Id,
                             Sex = sr.Sex_Name,
                             Name = sr.Name.ToUpper(),
                             MatricNumber = sr.Matric_Number,
                             CourseId = sr.Course_Id,
                             CourseCode = sr.Course_Code,
                             CourseName = sr.Course_Name,
                             CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                             DepartmentOptionId = sr.Department_Option_Id,
                             DepartmentOptionName = sr.Department_Option_Name,
                             SessionName = sr.Session_Name,
                             Surname = sr.Last_Name.ToUpper(),
                             FacultyName = sr.Faculty_Name,
                             WGP = sr.WGP,
                             Othername = sr.Othernames.ToUpper()
                         }).ToList();

                    List<Result> resultList = new List<Result>();

                    //for (int i = 0; i < results.Count; i++)
                    //{
                    //    if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                    //    {
                    //        //Do Nothing
                    //    }
                    //    else
                    //    {
                    //        resultList.Add(results[i]);
                    //    }
                    //}

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains(yearTwoSessionSuffix))
                        {
                            resultList.Add(results[i]);
                        }
                        else
                        {
                            //Do Nothing
                        }

                    }


                    results = new List<Result>();
                    results = resultList;
                }

                List<Result> classResult = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                                  (x.Activated == true))
                                            select new Result
                                            {
                                                StudentId = sr.Person_Id,
                                                Sex = sr.Sex_Name,
                                                Name = sr.Name,
                                                MatricNumber = sr.Matric_Number,
                                                CourseId = sr.Course_Id,
                                                CourseCode = sr.Course_Code,
                                                CourseName = sr.Course_Name,
                                                CourseUnit = (int)sr.Course_Unit,
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
                                                SessionName = sr.Session_Name,
                                                CourseModeId = sr.Course_Mode_Id
                                            }).ToList();
                List<long> distinctStudents = results.Select(r => r.StudentId).Distinct().ToList();
                for (int i = 0; i < distinctStudents.Count; i++)
                {
                    //long currentStudentId = distinctStudents[i];
                    //Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);


                    long currentStudentId = distinctStudents[i];
                    Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);

                    Result confirmedResult = ViewProcessedStudentResult(currentStudentId, ss, level, programme, department);
                    //confirmedResult.UnitOutstanding = TotalUnitsOmitted;

                    bool hasSpecialCaseOutstanding = CheckSpecialCase(classResult, currentStudentId);

                    if (confirmedResult != null)
                    {
                        currentStudentResult.CGPA = confirmedResult.CGPA;
                        currentStudentResult.Remark = confirmedResult.Remark;
                        currentStudentResult.Identifier = identifier;
                        currentStudentResult.Count = 1;
                        masterSheetResult.Add(currentStudentResult);
                    }

                }

                switch (type)
                {
                    case 1:
                        return masterSheetResult.OrderByDescending(a => a.CGPA).ToList();
                        break;
                    case 2:
                        return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
                        break;
                    case 3:
                        return masterSheetResult.OrderBy(a => a.Name).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
        }
        public List<Result> GetDiplomaClassListByOption(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption)
        {
            List<Result> masterSheetResult = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                SessionLogic sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                DepartmentOption option = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOption.Id);

                if (ss.Session.Id == (int)Sessions._20152016)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                 x =>
                                     x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                     x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                     x.Department_Id == department.Id &&
                                     x.Department_Option_Id == departmentOption.Id &&
                                     (x.Activated != false || x.Activated == null))
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   Name = sr.Name,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode = sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                                   DepartmentOptionId = sr.Department_Option_Id,
                                   DepartmentOptionName = sr.Department_Option_Name,
                                   SessionName = sr.Session_Name
                               }).ToList();
                }
                else
                {
                    results =
                        (from sr in
                             repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(
                                 x =>
                                     x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                     x.Level_Id == level.Id &&
                                     x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                     x.Department_Option_Id == departmentOption.Id &&
                                     (x.Activated != false || x.Activated == null))
                         select new Result
                         {
                             StudentId = sr.Person_Id,
                             Sex = sr.Sex_Name,
                             Name = sr.Name.ToUpper(),
                             MatricNumber = sr.Matric_Number,
                             CourseId = sr.Course_Id,
                             CourseCode = sr.Course_Code,
                             CourseName = sr.Course_Name,
                             CourseUnit = Convert.ToInt32(sr.Course_Unit),
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
                             DepartmentOptionId = sr.Department_Option_Id,
                             DepartmentOptionName = sr.Department_Option_Name,
                             SessionName = sr.Session_Name
                         }).ToList();

                    List<Result> resultList = new List<Result>();

                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i].MatricNumber.Contains("/16/") || results[i].MatricNumber.Contains("/13/") || results[i].MatricNumber.Contains("/14/"))
                        {
                            //Do Nothing
                        }
                        else
                        {
                            resultList.Add(results[i]);
                        }
                    }

                    results = new List<Result>();
                    results = resultList;
                }


                List<long> distinctStudents = results.Select(r => r.StudentId).Distinct().ToList();
                for (int i = 0; i < distinctStudents.Count; i++)
                {
                    long currentStudentId = distinctStudents[i];
                    Result currentStudentResult = results.Where(r => r.StudentId == currentStudentId).LastOrDefault();
                    //Result confirmedResult = GetStudentResultDetails(currentStudentId, level.Id, department.Id, programme.Id, ss.Semester.Id, ss.Session.Id);
                    Result confirmedResult = GetStudentResultBy(ss, level, programme, department, new Student() { Id = currentStudentId }).LastOrDefault();
                    if (confirmedResult != null)
                    {
                        currentStudentResult.CGPA = confirmedResult.CGPA;
                        currentStudentResult.Remark = confirmedResult.Remark;
                        currentStudentResult.Identifier = identifier;

                        currentStudentResult.Count = 1;

                        masterSheetResult.Add(currentStudentResult);
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return masterSheetResult.OrderBy(a => a.MatricNumber).ToList();
        }
        private Result GetStudentResultDetails(long studentId, int levelId, int departmentId, int programmeId, int semesterId, int sessionId)
        {
            Result overallResult = new Result();
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                Session session = new Session() { Id = sessionId };
                Programme programme = new Programme() { Id = programmeId };
                Department department = new Department() { Id = departmentId };
                Level level = new Level() { Id = levelId };
                SessionSemester sessionSemester = new SessionSemester();
                List<string> carryOverCourses = new List<string>();

                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == studentId);

                decimal firstYearFirstSemesterGPCUSum = 0;
                int firstYearFirstSemesterTotalCourseUnit = 0;
                decimal firstYearSecondSemesterGPCUSum = 0;
                int firstYearSecondSemesterTotalCourseUnit = 0;
                decimal secondYearFirstSemesterGPCUSum = 0;
                int secondYearFirstSemesterTotalCourseUnit = 0;
                decimal secondYearSecondSemesterGPCUSum = 0;
                int secondYearSecondSemesterTotalCourseUnit = 0;

                Result firstYearFirstSemester = new Result();
                Result firstYearSecondSemester = new Result();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                STUDENT_LEVEL studentLevelEntity = new STUDENT_LEVEL();
                if (programmeId > 2)
                {
                    studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == 3 && s.Person_Id == studentId && s.Department_Id == departmentId && s.Programme_Id == programmeId).LastOrDefault();
                }
                else
                {
                    studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == level.Id && s.Person_Id == studentId && s.Department_Id == departmentId && s.Programme_Id == programmeId).LastOrDefault();
                }

                if (studentLevelEntity != null)
                {
                    firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(level.Id, department, programme, studentCheck);
                    firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(level.Id, department, programme, studentCheck);
                }
                else
                {
                    return null;
                }

                if (semesterId == 1)
                {
                    List<Result> result = null;
                    Semester firstSemester = new Semester() { Id = 1 };
                    sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Id == sessionId && s.Semester_Id == firstSemester.Id);

                    carryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, studentCheck);
                    if (carryOverCourses.Count > 0)
                    {
                        return null;
                    }

                    if (studentCheck.Activated == true || studentCheck.Activated == null)
                    {
                        result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                    }
                    else
                    {
                        // result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        return null;
                    }
                    List<Result> modifiedResultList = new List<Result>();
                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {
                        decimal WGP = 0;

                        if (resultItem.SpecialCase != null)
                        {
                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }

                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }

                        modifiedResultList.Add(resultItem);
                    }

                    Result firstYearFirstSemesterResult = new Result();
                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                    secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                    decimal? firstSemesterGPA = 0M;
                    if (firstSemesterGPCUSum != null && firstSemesterGPCUSum > 0)
                    {
                        firstSemesterGPA = firstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                    }

                    overallResult = modifiedResultList.FirstOrDefault();
                    if (firstSemesterGPA != null && firstSemesterGPA > 0)
                    {
                        overallResult.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                    }
                    if (firstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                    {
                        if ((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) > 0 && firstYearSecondSemester.TotalSemesterCourseUnit != null && firstYearFirstSemester.TotalSemesterCourseUnit != null && secondYearfirstSemesterTotalSemesterCourseUnit != null)
                        {
                            firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                            firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                            overallResult.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                        }
                    }

                    overallResult.Remark = GetGraduationStatus(overallResult.CGPA, carryOverCourses);
                }
                else if (semesterId == 2)
                {

                    List<Result> result = null;
                    Semester firstSemester = new Semester() { Id = 1 };


                    if (studentCheck.Activated == true || studentCheck.Activated == null)
                    {
                        result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                    }
                    else
                    {
                        //result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, firstSemester, programme);
                        return null;
                    }
                    List<Result> modifiedResultList = new List<Result>();
                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {

                        decimal WGP = 0;

                        if (resultItem.SpecialCase != null)
                        {

                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }

                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }
                        modifiedResultList.Add(resultItem);
                    }
                    Result secondYearFirstSemesterResult = new Result();
                    decimal? secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                    secondYearfirstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    secondYearFirstSemesterResult.TotalSemesterCourseUnit = secondYearfirstSemesterTotalSemesterCourseUnit;
                    secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                    decimal? firstSemesterGPA = 0M;
                    if (secondYearfirstSemesterGPCUSum != null && secondYearfirstSemesterGPCUSum > 0)
                    {
                        firstSemesterGPA = secondYearfirstSemesterGPCUSum / secondYearfirstSemesterTotalSemesterCourseUnit;
                    }



                    //Second semester second year

                    List<Result> secondSemesterResult = null;


                    Semester secondSemester = new Semester() { Id = 2 };

                    sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Id == sessionId && s.Semester_Id == secondSemester.Id);

                    carryOverCourses = GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, studentCheck);
                    if (carryOverCourses.Count > 0)
                    {
                        return null;
                    }

                    if (studentCheck.Activated == true || studentCheck.Activated == null)
                    {
                        secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                    }
                    else
                    {
                        //secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(session, level, department, studentCheck, secondSemester, programme);
                        return null;
                    }
                    List<Result> secondSemesterModifiedResultList = new List<Result>();
                    int totalSecondSemesterCourseUnit = 0;
                    foreach (Result resultItem in secondSemesterResult)
                    {

                        decimal WGP = 0;

                        if (resultItem.SpecialCase != null)
                        {

                            resultItem.GPCU = 0;
                            if (totalSecondSemesterCourseUnit == 0)
                            {
                                totalSecondSemesterCourseUnit = (int)resultItem.TotalSemesterCourseUnit - resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSecondSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }

                        }
                        if (totalSecondSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                        }
                        secondSemesterModifiedResultList.Add(resultItem);
                    }
                    Result secondYearSecondtSemesterResult = new Result();
                    decimal? secondYearSecondtSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                    int? secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                    secondYearSecondSemesterTotalSemesterCourseUnit = secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    secondYearSecondtSemesterResult.TotalSemesterCourseUnit = secondYearSecondSemesterTotalSemesterCourseUnit;
                    secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                    decimal? secondYearSecondSmesterGPA = 0M;
                    if (secondYearSecondtSemesterGPCUSum != null && secondYearSecondtSemesterGPCUSum > 0)
                    {
                        secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum / secondYearSecondSemesterTotalSemesterCourseUnit;
                    }

                    overallResult = secondSemesterModifiedResultList.FirstOrDefault();
                    if (secondYearSecondSmesterGPA != null && secondYearSecondSmesterGPA > 0)
                    {
                        overallResult.GPA = Decimal.Round((decimal)secondYearSecondSmesterGPA, 2);
                    }
                    if (secondYearfirstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                    {
                        firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                        firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                        firstYearFirstSemester.GPCU = firstYearFirstSemester.GPCU ?? 0;
                        firstYearSecondSemester.GPCU = firstYearSecondSemester.GPCU ?? 0;
                        overallResult.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                    }

                    overallResult.Remark = GetGraduationStatus(overallResult.CGPA, carryOverCourses);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return overallResult;
        }
        public List<Result>ResultSummaryReport(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            List<Result> returnResult = new List<Result>();
            List<Result> processedResults = new List<Result>();
            List<Result> studentResult = new List<Result>();
            List<GradingCount> allGrade = new List<GradingCount>();
            List<GradingCount> AllStudentCGPA = new List<GradingCount>();
            
            List<StudentDeferementLog> deferement = new List<StudentDeferementLog>();
            int Expelled = 0; int ResultWithheld = 0; int VoluntaryWithdrew = 0;int AcademicWithdrew = 0; int warning = 0; int probation = 0; int oneCarriedOverCourse = 0; int moreThanOneCarriedOverCourse = 0; int noCarryOver = 0;
            
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }
                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;
                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
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
                               TotalScore = sr.Total_Score,
                               Grade = sr.Grade,
                               GradePoint = sr.Grade_Point,
                               DepartmentName = sr.Department_Name,
                               ProgrammeName = sr.Programme_Name,
                               LevelName = sr.Level_Name,
                               Semestername = sr.Semester_Name,
                               GPCU = sr.Grade_Point * sr.Course_Unit,
                               TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper()

                           }).ToList();
                List<long> studentIds = results.Select(x => x.StudentId).Distinct().ToList();
                foreach(var studentId in studentIds)
                {
                    Student student = new Student { Id = studentId };
                     List<Result> detailResult=GetStudentResult(ss.Semester, level, programme, department, ss.Session, student);
                    if (detailResult.Count > 0)
                    {
                        GradingCount studentGrading = new GradingCount();
                        studentGrading.CGPA = (double)detailResult.FirstOrDefault().DoubleCGPA;
                        AllStudentCGPA.Add(studentGrading);
                    }
                    var requiredRecord=detailResult.FirstOrDefault();
                    studentResult.Add(requiredRecord);
                    
                    List<CarryOverCount> carriedOverStudent=StudentsWithOutstandingCourse(ss.Semester, level, programme, department, ss.Session, student);
                    if (carriedOverStudent.Count > 0)
                    {
                        foreach(var number in carriedOverStudent)
                        {
                            if (number.OneCourse > 0)
                            {
                                oneCarriedOverCourse += 1;
                            }
                            else if (number.MoreThanOneCourse > 0){
                                moreThanOneCarriedOverCourse += 1;
                            }
                            else
                            {
                                noCarryOver += 1;
                            }
                        }
                    }
                }
                List<long> courseIds = results.Select(r => r.CourseId).Distinct().ToList();

                var MaxGPA = studentResult.Where(x=>x.GPA==studentResult.Max(s=>s.GPA)).FirstOrDefault();
                var MaxCGPA = studentResult.Where(x => x.CGPA == studentResult.Max(s => s.CGPA)).FirstOrDefault();
                int RectorHonourCount = studentResult.Where(x => (double)x.GPA >= 3.50).ToList().Count();
                int DeanHonourCount = studentResult.Where(x => (double)x.GPA >= 3.25).ToList().Count();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<long> studentListIds=studentLevelLogic.GetModelsBy(x => x.Session_Id == ss.Session.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Department_Id == department.Id).Select(x => x.Student.Id).Distinct().ToList();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                FeeType feeType = new FeeType { Id = 2 };
                var registeredStudentIds = courseRegistrationLogic.GetModelsBy(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Session_Id == ss.Session.Id).Select(x => x.Student.Id).Distinct().ToList();
                var studentRoll=remitaPaymentLogic.NumberofStudentThatPaidSchoolFees(registeredStudentIds, ss.Session, feeType, programme);
                var registeredStudent=registeredStudentIds.Count();
                //var registeredStudent=courseRegistrationLogic.GetModelsBy(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Session_Id == ss.Session.Id).Select(x=>x.Student.Id).Distinct().Count();
                foreach (var courseId in courseIds)
                {
                    Course course = new Course { Id = courseId };
                    List<Result> coursRresults=GetCourseSummaryBreakdown(ss.Semester, level, programme, department, course, ss.Session);
                    

                    var specialcases=coursRresults.Where(x => x.SpecialCase != null).Select(x=>x.SpecialCase).ToList();
                    foreach(var sp in specialcases)
                    {
                        switch (sp)
                        {
                            case "Withheld":
                                ResultWithheld += 1;
                                break;
                            case "Probation":
                                probation += 1;
                                break;
                            case "Expelled":
                                Expelled += 1;
                                break;
                            case "AcademicWithdrawal":
                                AcademicWithdrew += 1;
                                break;
                            case "VoluntaryWithdrawal":
                                VoluntaryWithdrew += 1;
                                break;
                            case "Warning":
                                warning += 1;
                                break;
                        }
                    }

                    var gradedResults=StudentGradePerCourse(ss.Semester, level, programme, department, ss.Session, course);
                    if (gradedResults.Count > 0)
                    {
                        
                        foreach(var gradedResult in gradedResults)
                        {
                            GradingCount studentGrading = new GradingCount();
                            var examScore = gradedResult.ExamScore !=null ? gradedResult.ExamScore : 0;
                            var testScore = gradedResult.TestScore != null ? gradedResult.TestScore : 0;
                            decimal totalScore = (decimal)(examScore + testScore);
                            if (totalScore >= 75)
                            {
                                //AA += 1;
                                studentGrading.AA += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;


                            }
                            else if(totalScore>=70 && totalScore <= 74)
                            {
                                studentGrading.A += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 65 && totalScore <= 69)
                            {
                                studentGrading.BB += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 60 && totalScore <= 64)
                            {
                                studentGrading.B += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 55 && totalScore <= 59)
                            {
                                studentGrading.CC += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 50 && totalScore <= 54)
                            {
                                studentGrading.C += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 45 && totalScore <= 49)
                            {
                                studentGrading.DD += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 40 && totalScore <= 44)
                            {
                                studentGrading.D += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 30 && totalScore <= 39)
                            {
                                studentGrading.F1 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 20 && totalScore <= 29)
                            {
                                studentGrading.F2 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 0 && totalScore <= 19)
                            {
                                studentGrading.F2 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            if (gradedResult.SpecialCase != null)
                            {
                                if (gradedResult.SpecialCase == "Sick")
                                {
                                    studentGrading.SickCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Incomplete")
                                {
                                    studentGrading.IncompleteCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Warning")
                                {
                                    studentGrading.WarningCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Deferred")
                                {
                                    studentGrading.DefCount += 1;
                                }
                            }
                           
                            allGrade.Add(studentGrading);
                        }
                    }
                    processedResults.AddRange(coursRresults);

                    
                }
                var returncourseIds = results.Select(x => x.CourseId).Distinct();
                foreach (var Id in returncourseIds)
                {
                    //List<Result> courseResults = results.Where(x => x.CourseId == Id).ToList();
                    var courseResult = results.Where(x => x.CourseId == Id).FirstOrDefault();
                   var sumGrading= allGrade.Where(x => x.CourseId == Id);
                        courseResult.BestGPAStudent = Decimal.Round((decimal)MaxGPA.GPA, 2)>0? Decimal.Round((decimal)MaxGPA.GPA, 2) + "   " + MaxGPA.MatricNumber: Convert.ToString(Decimal.Round((decimal)MaxGPA.GPA, 2));
                        courseResult.BestCGPAStudent = Decimal.Round((decimal)MaxCGPA.CGPA, 2) > 0 ? Decimal.Round((decimal)MaxCGPA.CGPA, 2) + "   " + MaxCGPA.MatricNumber : Convert.ToString(Decimal.Round((decimal)MaxCGPA.CGPA, 2));
                    courseResult.WithHeldResults = ResultWithheld;
                        courseResult.Warning = warning;
                        courseResult.VoluntaryWithdrawal = VoluntaryWithdrew;
                        courseResult.TotalProbation = probation;
                        courseResult.AcademicWithdrawal = AcademicWithdrew;
                        courseResult.RegisteredStudent = registeredStudent;
                        courseResult.RectorHonourCount = RectorHonourCount;
                        courseResult.DeanHonourCount = DeanHonourCount;
                        courseResult.RusticatedStudents = Expelled;
                        courseResult.Identifier = identifier;
                        courseResult.StudentRoll = studentRoll;
                        courseResult.OneOutstandingCourse = oneCarriedOverCourse;
                        courseResult.MoreOutstandingCourse = moreThanOneCarriedOverCourse;
                        courseResult.studentWithOutOutstanding = noCarryOver;
                        courseResult.studentWithOutstanding = (studentIds.Count() - noCarryOver);
                    courseResult.AACount = sumGrading.Sum(x => x.AA);
                    courseResult.ACount = sumGrading.Sum(x => x.A);
                    courseResult.BBCount = sumGrading.Sum(x => x.BB);
                    courseResult.BCount = sumGrading.Sum(x => x.B);
                    courseResult.CCCount = sumGrading.Sum(x => x.CC);
                    courseResult.CCount = sumGrading.Sum(x => x.C);
                    courseResult.DCount = sumGrading.Sum(x => x.D);
                    courseResult.DDCount = sumGrading.Sum(x => x.DD);
                    courseResult.F1Count = sumGrading.Sum(x => x.F1);
                    courseResult.F2Count = sumGrading.Sum(x => x.F2);
                    courseResult.F3Count = sumGrading.Sum(x => x.F3);
                    courseResult.SickCount = sumGrading.Sum(x => x.SickCount);
                    courseResult.IncompleteCount = sumGrading.Sum(x => x.IncompleteCount);
                    courseResult.DefCount = sumGrading.Sum(x => x.DefCount);
                    courseResult.WarningCount = sumGrading.Sum(x => x.WarningCount);
                    courseResult.CGPA1 = AllStudentCGPA.Where(x => x.CGPA >= 0.0 && x.CGPA <= 0.49).Count();
                    courseResult.CGPA2 = AllStudentCGPA.Where(x => x.CGPA >= 0.5 && x.CGPA <= 0.99).Count();
                    courseResult.CGPA3 = AllStudentCGPA.Where(x => x.CGPA >= 1.0 && x.CGPA <= 1.49).Count();
                    courseResult.CGPA4 = AllStudentCGPA.Where(x => x.CGPA >= 1.5 && x.CGPA <= 1.99).Count();
                    courseResult.CGPA5 = AllStudentCGPA.Where(x => x.CGPA >= 2.0 && x.CGPA <= 2.49).Count();
                    courseResult.CGPA6 = AllStudentCGPA.Where(x => x.CGPA >= 2.5 && x.CGPA <= 2.99).Count();
                    courseResult.CGPA7 = AllStudentCGPA.Where(x => x.CGPA >= 3.0 && x.CGPA <= 3.49).Count();
                    courseResult.CGPA8 = AllStudentCGPA.Where(x => x.CGPA >= 3.5 && x.CGPA <= 4.0).Count();
                    courseResult.TotalCount = sumGrading.Sum(x => x.AA) + sumGrading.Sum(x => x.A) + sumGrading.Sum(x => x.BB) + sumGrading.Sum(x => x.B) + sumGrading.Sum(x=>x.CC) + sumGrading.Sum(x => x.C) + sumGrading.Sum(x => x.D) + sumGrading.Sum(x => x.DD)
                        + sumGrading.Sum(x => x.F1) + sumGrading.Sum(x => x.F2) + sumGrading.Sum(x => x.F3) + sumGrading.Sum(x => x.SickCount) + sumGrading.Sum(x => x.IncompleteCount) + sumGrading.Sum(x => x.DefCount) + sumGrading.Sum(x => x.WarningCount);
                    courseResult.PassCount = sumGrading.Sum(x => x.AA)+ sumGrading.Sum(x => x.A)+ sumGrading.Sum(x => x.BB)+ sumGrading.Sum(x => x.B) + sumGrading.Sum(x => x.CC) + sumGrading.Sum(x => x.C) + sumGrading.Sum(x => x.D)+ sumGrading.Sum(x => x.DD);
                    returnResult.Add(courseResult);
                    //}
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return returnResult;

        }
        public List<Result> ResultSummaryReportByOption(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption)
        {
            List<Result> returnResult = new List<Result>();
            List<Result> processedResults = new List<Result>();
            List<Result> studentResult = new List<Result>();
            List<GradingCount> allGrade = new List<GradingCount>();
            List<GradingCount> AllStudentCGPA = new List<GradingCount>();

            List<StudentDeferementLog> deferement = new List<StudentDeferementLog>();
            int Expelled = 0; int ResultWithheld = 0; int VoluntaryWithdrew = 0; int AcademicWithdrew = 0; int warning = 0; int probation = 0; int oneCarriedOverCourse = 0; int moreThanOneCarriedOverCourse = 0; int noCarryOver = 0;

            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }
                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                DepartmentOption option = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOption.Id);
                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;
                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id==departmentOption.Id)
                           select new Result
                           {
                               StudentId = sr.Person_Id,
                               Sex = sr.Sex_Name,
                               Name = sr.Name,
                               MatricNumber = sr.Matric_Number,
                               CourseId = sr.Course_Id,
                               CourseCode = sr.Course_Code,
                               CourseName = sr.Course_Name,
                               CourseUnit = (int)sr.Course_Unit,
                               SpecialCase = sr.Special_Case,
                               TestScore = sr.Test_Score,
                               ExamScore = sr.Exam_Score,
                               TotalScore = sr.Total_Score,
                               Grade = sr.Grade,
                               GradePoint = sr.Grade_Point,
                               DepartmentName = sr.Department_Name,
                               ProgrammeName = sr.Programme_Name,
                               LevelName = sr.Level_Name,
                               Semestername = sr.Semester_Name,
                               GPCU = sr.Grade_Point * sr.Course_Unit,
                               TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper()

                           }).ToList();
                List<long> studentIds = results.Select(x => x.StudentId).Distinct().ToList();
                foreach (var studentId in studentIds)
                {
                    Student student = new Student { Id = studentId };
                    List<Result> detailResult = GetStudentResultByOption(ss.Semester, level, programme, department, ss.Session, student,departmentOption);
                    if (detailResult.Count > 0)
                    {
                        GradingCount studentGrading = new GradingCount();
                        studentGrading.CGPA = (double)detailResult.FirstOrDefault().DoubleCGPA;
                        AllStudentCGPA.Add(studentGrading);
                    }
                    var requiredRecord = detailResult.FirstOrDefault();
                    studentResult.Add(requiredRecord);
                    List<CarryOverCount> carriedOverStudent = StudentsWithOutstandingCourseByOption(ss.Semester, level, programme, department, ss.Session, student,departmentOption);
                    if (carriedOverStudent.Count > 0)
                    {
                        foreach (var number in carriedOverStudent)
                        {
                            if (number.OneCourse > 0)
                            {
                                oneCarriedOverCourse += 1;
                            }
                            else if (number.MoreThanOneCourse > 0)
                            {
                                moreThanOneCarriedOverCourse += 1;
                            }
                            else
                            {
                                noCarryOver += 1;
                            }
                        }
                    }
                }
                List<long> courseIds = results.Select(r => r.CourseId).Distinct().ToList();

                var MaxGPA = studentResult.Where(x => x.GPA == studentResult.Max(s => s.GPA)).FirstOrDefault();
                var MaxCGPA = studentResult.Where(x => x.CGPA == studentResult.Max(s => s.CGPA)).FirstOrDefault();
                int RectorHonourCount = studentResult.Where(x => (double)x.GPA >= 3.50).ToList().Count();
                int DeanHonourCount = studentResult.Where(x => (double)x.GPA >= 3.25).ToList().Count();

                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<long> studentListIds = studentLevelLogic.GetModelsBy(x => x.Session_Id == ss.Session.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Department_Option_Id == departmentOption.Id).Select(x => x.Student.Id).ToList();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                FeeType feeType = new FeeType { Id = 2 };
                var studentRoll = remitaPaymentLogic.NumberofStudentThatPaidSchoolFees(studentListIds, ss.Session, feeType, programme);
                var registeredStudent = courseRegistrationLogic.GetModelsBy(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Session_Id == ss.Session.Id).Select(x => x.Student.Id).Distinct().Count();
                foreach (var courseId in courseIds)
                {
                    Course course = new Course { Id = courseId };
                    List<Result> coursRresults = GetCourseSummaryBreakdownByOption(ss.Semester, level, programme, department, course, ss.Session, departmentOption);
                    var specialcases = coursRresults.Where(x => x.SpecialCase != null).Select(x => x.SpecialCase).ToList();
                    foreach (var sp in specialcases)
                    {
                        switch (sp)
                        {
                            case "Withheld":
                                ResultWithheld += 1;
                                break;
                            case "Probation":
                                probation += 1;
                                break;
                            case "Expelled":
                                Expelled += 1;
                                break;
                            case "AcademicWithdrawal":
                                AcademicWithdrew += 1;
                                break;
                            case "VoluntaryWithdrawal":
                                VoluntaryWithdrew += 1;
                                break;
                            case "Warning":
                                warning += 1;
                                break;
                        }
                    }

                    var gradedResults = StudentGradePerCourse(ss.Semester, level, programme, department, ss.Session, course);
                    if (gradedResults.Count > 0)
                    {

                        foreach (var gradedResult in gradedResults)
                        {
                            GradingCount studentGrading = new GradingCount();
                            var examScore = gradedResult.ExamScore != null ? gradedResult.ExamScore : 0;
                            var testScore = gradedResult.TestScore != null ? gradedResult.TestScore : 0;
                            decimal totalScore = (decimal)(examScore + testScore);
                            if (totalScore >= 75)
                            {
                                //AA += 1;
                                studentGrading.AA += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;


                            }
                            else if (totalScore >= 70 && totalScore <= 74)
                            {
                                studentGrading.A += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 65 && totalScore <= 69)
                            {
                                studentGrading.BB += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 60 && totalScore <= 64)
                            {
                                studentGrading.B += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 55 && totalScore <= 59)
                            {
                                studentGrading.CC += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 50 && totalScore <= 54)
                            {
                                studentGrading.C += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 45 && totalScore <= 49)
                            {
                                studentGrading.DD += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 40 && totalScore <= 44)
                            {
                                studentGrading.D += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 30 && totalScore <= 39)
                            {
                                studentGrading.F1 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 20 && totalScore <= 29)
                            {
                                studentGrading.F2 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            else if (totalScore >= 0 && totalScore <= 19)
                            {
                                studentGrading.F2 += 1;
                                studentGrading.CourseId = gradedResult.Course.Id;
                            }
                            if(gradedResult.SpecialCase != null)
                            {
                                if (gradedResult.SpecialCase == "Sick")
                                {
                                    studentGrading.SickCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Incomplete")
                                {
                                    studentGrading.IncompleteCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Warning")
                                {
                                    studentGrading.WarningCount += 1;
                                }
                                else if (gradedResult.SpecialCase == "Deferred")
                                {
                                    studentGrading.DefCount += 1;
                                }
                            }
                            allGrade.Add(studentGrading);
                        }
                    }
                    processedResults.AddRange(coursRresults);


                }
                var returncourseIds = results.Select(x => x.CourseId).Distinct();
                foreach (var Id in returncourseIds)
                {
                    //List<Result> courseResults = results.Where(x => x.CourseId == Id).ToList();
                    var courseResult = results.Where(x => x.CourseId == Id).FirstOrDefault();
                    var sumGrading = allGrade.Where(x => x.CourseId == Id);
                    courseResult.BestGPAStudent = Decimal.Round((decimal)MaxGPA.GPA, 2) > 0 ? Decimal.Round((decimal)MaxGPA.GPA, 2) + "   " + MaxGPA.MatricNumber : Convert.ToString(Decimal.Round((decimal)MaxGPA.GPA, 2));
                    courseResult.BestCGPAStudent = Decimal.Round((decimal)MaxCGPA.CGPA, 2) > 0 ? Decimal.Round((decimal)MaxCGPA.CGPA, 2) + "   " + MaxCGPA.MatricNumber : Convert.ToString(Decimal.Round((decimal)MaxCGPA.CGPA, 2));
                    courseResult.WithHeldResults = ResultWithheld;
                    courseResult.Warning = warning;
                    courseResult.VoluntaryWithdrawal = VoluntaryWithdrew;
                    courseResult.TotalProbation = probation;
                    courseResult.AcademicWithdrawal = AcademicWithdrew;
                    courseResult.RegisteredStudent = registeredStudent;
                    courseResult.RectorHonourCount = RectorHonourCount;
                    courseResult.DeanHonourCount = DeanHonourCount;
                    courseResult.RusticatedStudents = Expelled;
                    courseResult.Identifier = identifier;
                    courseResult.StudentRoll = studentIds.Count();
                    courseResult.OneOutstandingCourse = oneCarriedOverCourse;
                    courseResult.MoreOutstandingCourse = moreThanOneCarriedOverCourse;
                    courseResult.studentWithOutOutstanding = noCarryOver;
                    courseResult.studentWithOutstanding = (studentIds.Count() - noCarryOver);
                    courseResult.AACount = sumGrading.Sum(x => x.AA);
                    courseResult.ACount = sumGrading.Sum(x => x.A);
                    courseResult.BBCount = sumGrading.Sum(x => x.BB);
                    courseResult.BCount = sumGrading.Sum(x => x.B);
                    courseResult.CCCount = sumGrading.Sum(x => x.CC);
                    courseResult.CCount = sumGrading.Sum(x => x.C);
                    courseResult.DCount = sumGrading.Sum(x => x.D);
                    courseResult.DDCount = sumGrading.Sum(x => x.DD);
                    courseResult.F1Count = sumGrading.Sum(x => x.F1);
                    courseResult.F2Count = sumGrading.Sum(x => x.F2);
                    courseResult.F3Count = sumGrading.Sum(x => x.F3);
                    courseResult.SickCount = sumGrading.Sum(x => x.SickCount);
                    courseResult.IncompleteCount = sumGrading.Sum(x => x.IncompleteCount);
                    courseResult.DefCount = sumGrading.Sum(x => x.DefCount);
                    courseResult.WarningCount = sumGrading.Sum(x => x.WarningCount);

                    courseResult.CGPA1 = AllStudentCGPA.Where(x => x.CGPA >= 0.0 && x.CGPA <= 0.49).Count();
                    courseResult.CGPA2 = AllStudentCGPA.Where(x => x.CGPA >= 0.5 && x.CGPA <= 0.99).Count();
                    courseResult.CGPA3 = AllStudentCGPA.Where(x => x.CGPA >= 1.0 && x.CGPA <= 1.49).Count();
                    courseResult.CGPA4 = AllStudentCGPA.Where(x => x.CGPA >= 1.5 && x.CGPA <= 1.99).Count();
                    courseResult.CGPA5 = AllStudentCGPA.Where(x => x.CGPA >= 2.0 && x.CGPA <= 2.49).Count();
                    courseResult.CGPA6 = AllStudentCGPA.Where(x => x.CGPA >= 2.5 && x.CGPA <= 2.99).Count();
                    courseResult.CGPA7 = AllStudentCGPA.Where(x => x.CGPA >= 3.0 && x.CGPA <= 3.49).Count();
                    courseResult.CGPA8 = AllStudentCGPA.Where(x => x.CGPA >= 3.5 && x.CGPA <= 4.0).Count();
                    courseResult.TotalCount = sumGrading.Sum(x => x.AA) + sumGrading.Sum(x => x.A) + sumGrading.Sum(x => x.BB) + sumGrading.Sum(x => x.B) + sumGrading.Sum(x => x.CC) + sumGrading.Sum(x => x.C) + sumGrading.Sum(x => x.D) + sumGrading.Sum(x => x.DD)
                        + sumGrading.Sum(x => x.F1) + sumGrading.Sum(x => x.F2) + sumGrading.Sum(x => x.F3) + sumGrading.Sum(x => x.SickCount) + sumGrading.Sum(x => x.IncompleteCount) + sumGrading.Sum(x => x.DefCount) + sumGrading.Sum(x => x.WarningCount);
                    courseResult.PassCount = sumGrading.Sum(x => x.AA) + sumGrading.Sum(x => x.A) + sumGrading.Sum(x => x.BB) + sumGrading.Sum(x => x.B) + sumGrading.Sum(x => x.CC) + sumGrading.Sum(x => x.C) + sumGrading.Sum(x => x.D) + sumGrading.Sum(x => x.DD);
                    returnResult.Add(courseResult);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnResult;

        }
        public List<Result>GetCourseSummaryBreakdown(Semester semester, Level level, Programme programme, Department department, Course course, Session session)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0||course==null||course.Id<=0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Course_Id==course.Id)
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
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            //Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();

                return results;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<Result> GetCourseSummaryBreakdownByOption(Semester semester, Level level, Programme programme, Department department, Course course, Session session, DepartmentOption departmentOption)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || course == null || course.Id <= 0 || departmentOption==null || departmentOption.Id<=0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Department_Option_Id==departmentOption.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Course_Id == course.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            Sex = sr.Sex_Name,
                                            Name = sr.Name,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = (int)sr.Course_Unit,
                                            FacultyName = sr.Faculty_Name,
                                            DepartmentName = sr.Department_Name,
                                            ProgrammeId = sr.Programme_Id,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            //Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                            SpecialCase = sr.Special_Case,
                                            Activated = sr.Activated,
                                            Reason = sr.Reason,
                                            RejectCategory = sr.Reject_Category,
                                            firstname_middle = sr.Othernames,
                                            ProgrammeName = sr.Programme_Name,
                                            Surname = sr.Last_Name,
                                            Firstname = sr.First_Name,
                                            Othername = sr.Other_Name,
                                            TotalScore = sr.Total_Score,
                                            SessionSemesterId = sr.Session_Semester_Id,
                                            SessionSemesterSequenceNumber = sr.Sequence_Number,
                                            SessionId = sr.Session_Id
                                        }).ToList();
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Result> GetStudentResult(Semester semester, Level level, Programme programme, Department department, Session session ,Student student)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Person_Id == student.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = sr.Course_Unit,
                                            SpecialCase=sr.Special_Case,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            //Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                        }).ToList();
                var totalWGP=results.Sum(x => x.WGP);
                var totalCourseUnit = results.FirstOrDefault().TotalSemesterCourseUnit;
                var totalGPCU = results.Sum(x => x.GPCU);
                var GPA = (totalGPCU / totalCourseUnit) != null ? (decimal)(totalGPCU / totalCourseUnit) : 0;
                var CGPA = (totalWGP / totalCourseUnit)!=null?(double)(totalWGP / totalCourseUnit):0;
                results.FirstOrDefault().GPA = GPA;
                results.FirstOrDefault().CGPA = (decimal)CGPA;
                results.FirstOrDefault().DoubleCGPA = CGPA;
                
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Result> GetStudentResultByOption(Semester semester, Level level, Programme programme, Department department, Session session, Student student, DepartmentOption departmentOption)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                SessionLogic sessionLogic = new SessionLogic();
                SESSION sessions = sessionLogic.GetEntityBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Session_Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(p => p.Programme_Id == programme.Id && p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Department_Option_Id==departmentOption.Id && p.Semester_Id == semester.Id && (p.Activated == true) && p.Person_Id == student.Id)
                                        select new Result
                                        {
                                            StudentId = sr.Person_Id,
                                            MatricNumber = sr.Matric_Number,
                                            CourseId = sr.Course_Id,
                                            CourseCode = sr.Course_Code,
                                            CourseName = sr.Course_Name,
                                            CourseUnit = (int)sr.Course_Unit,
                                            SpecialCase = sr.Special_Case,
                                            TestScore = sr.Test_Score,
                                            ExamScore = sr.Exam_Score,
                                            //Score = sr.Total_Score,
                                            Grade = sr.Grade,
                                            GradePoint = sr.Grade_Point,
                                            Email = sr.Email,
                                            Address = sr.Contact_Address,
                                            MobilePhone = sr.Mobile_Phone,
                                            PassportUrl = sr.Image_File_Url,
                                            GPCU = sr.Grade_Point * sr.Course_Unit,
                                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                            Student_Type_Id = sr.Student_Type_Id,
                                            SessionName = sr.Session_Name,
                                            Semestername = sr.Semester_Name,
                                            LevelName = sr.Level_Name,
                                            WGP = sr.WGP,
                                        }).ToList();
                var totalWGP = results.Sum(x => x.WGP);
                var totalCourseUnit = results.FirstOrDefault().TotalSemesterCourseUnit;
                var totalGPCU = results.Sum(x => x.GPCU);
                var GPA = (totalGPCU / totalCourseUnit) != null ? (decimal)(totalGPCU / totalCourseUnit) : 0;
                var CGPA = (totalWGP / totalCourseUnit) != null ? (double)(totalWGP / totalCourseUnit) : 0;
                results.FirstOrDefault().GPA = GPA;
                results.FirstOrDefault().CGPA = (decimal)CGPA;
                results.FirstOrDefault().DoubleCGPA = CGPA;

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CarryOverCount> StudentsWithOutstandingCourse(Semester semester, Level level, Programme programme, Department department, Session session, Student student)
        {
            List<CarryOverCount> carriedOver = new List<CarryOverCount>();
            try
            { CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                
                var CarriedOverCourses = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id 
                && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id 
                && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && ((crd.Test_Score + crd.Exam_Score) < (int)Grades.PassMark || crd.Test_Score==null) && crd.Special_Case == null);
                if (CarriedOverCourses.Count > 0)
                {
                    if (CarriedOverCourses.Count == 1)
                    {
                        CarryOverCount oneCarriedOver = new CarryOverCount();
                        oneCarriedOver.OneCourse = 1;
                        carriedOver.Add(oneCarriedOver);
                    }
                    else
                    {
                        CarryOverCount moreCarriedOver = new CarryOverCount();
                        moreCarriedOver.MoreThanOneCourse = 1;
                        carriedOver.Add(moreCarriedOver);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return carriedOver;
        }
        public List<CarryOverCount> StudentsWithOutstandingCourseByOption(Semester semester, Level level, Programme programme, Department department, Session session, Student student,DepartmentOption departmentOption)
        {
            List<CarryOverCount> carriedOver = new List<CarryOverCount>();
            try
            {
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || student == null || student.Id <= 0|| departmentOption==null || departmentOption.Id<=0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }

                var CarriedOverCourses = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id
                && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id
                && crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && ((crd.Test_Score + crd.Exam_Score) < (int)Grades.PassMark || crd.Test_Score == null) && crd.Special_Case == null);
                if (CarriedOverCourses.Count > 0)
                {
                    if (CarriedOverCourses.Count == 1)
                    {
                        CarryOverCount oneCarriedOver = new CarryOverCount();
                        oneCarriedOver.OneCourse = 1;
                        carriedOver.Add(oneCarriedOver);
                    }
                    else
                    {
                        CarryOverCount moreCarriedOver = new CarryOverCount();
                        moreCarriedOver.MoreThanOneCourse = 1;
                        carriedOver.Add(moreCarriedOver);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return carriedOver;
        }
        public List<CourseRegistrationDetail> StudentGradePerCourse(Semester semester, Level level, Programme programme, Department department, Session session, Course course)
        {
            List<CourseRegistrationDetail> courseGrading = new List<CourseRegistrationDetail>();
            try
            {
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0 || programme == null || programme.Id <= 0 || course == null || course.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }

                courseGrading = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id
                && crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id
                && crd.Course_Id==course.Id);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return courseGrading;
        }
        public List<Result> GetExamBroadSheet(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        {
            List<Result> broadScoreSheet = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                List<int> firstYearTotalCourseUnit = new List<int>();

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;

                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
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
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper(),
                               SemesterId = sr.Semester_Id,
                               SessionName=sr.Session_Name


                           }).ToList();
                broadScoreSheet.AddRange(results);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return broadScoreSheet.OrderBy(x => x.MatricNumber).ToList();
        }
        public List<Result> GetExamBroadSheetByOption(SessionSemester sessionSemester, Level level, Programme programme, Department department, DepartmentOption departmentOption)
        {
            List<Result> broadScoreSheet = new List<Result>();
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || departmentOption==null || departmentOption.Id<=0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                List<int> firstYearTotalCourseUnit = new List<int>();

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;

                List<Result> results = null;
                results = (from sr in repository.GetBy<VW_STUDENT_RESULT_WITH_OPTIONS>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Department_Option_Id==departmentOption.Id)
                           select new Result
                           {
                               StudentId = sr.Person_Id,
                               Sex = sr.Sex_Name,
                               Name = sr.Name,
                               MatricNumber = sr.Matric_Number,
                               CourseId = sr.Course_Id,
                               CourseCode = sr.Course_Code,
                               CourseName = sr.Course_Name,
                               CourseUnit = (int)sr.Course_Unit,
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
                               CourseModeId = sr.Course_Mode_Id,
                               CourseModeName = sr.Course_Mode_Name,
                               Surname = sr.Last_Name.ToUpper(),
                               FacultyName = sr.Faculty_Name,
                               WGP = sr.WGP,
                               Othername = sr.Othernames.ToUpper(),
                               SemesterId = sr.Semester_Id,
                               SessionName = sr.Session_Name


                           }).ToList();
                broadScoreSheet.AddRange(results);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return broadScoreSheet.OrderBy(x => x.MatricNumber).ToList();
        }
    }
    public class CarryOverCount
    {
        public int OneCourse { get; set; }
        public int MoreThanOneCourse { get; set; }
    }
    public class GradingCount
    {
        public long CourseId { get; set; }
        public int AA { get; set; }
        public int A { get; set; }
        public int BB { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int CC { get; set; }
        public int DD { get; set; }
        public int D { get; set; }
        public int F1 { get; set; }
        public int F2 { get; set; }
        public int F3 { get; set; }
        public int SickCount { get; set; }
        public int IncompleteCount { get; set; }
        public int DefCount { get; set; }
        public int WarningCount { get; set; }
        public int PassCount { get; set; }
        public int TotalCount { get; set; }
        public double CGPA { get; set; }
    }

}
