using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentExamRawScoreSheetNotRegisteredLogic:BusinessBaseLogic<StudentExamRawScoreSheet,STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED>
    {
        public StudentExamRawScoreSheetNotRegisteredLogic()
        {
            translator = new StudentExamRawScoreSheetNotRegisteredTranslator();
        }


        public List<ExamRawScoreSheetReport> GetScoreSheety(Session session,Semester semester,Course course,Department department,Level level,Programme programme)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetReportList = new List<ExamRawScoreSheetReport>();
            try
            {

                List<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED> studentExamRawScoreSheetList = new List<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED>();
                if(session != null && semester != null && course != null && department != null && level != null && programme != null)
                {
                    if(programme.Id == 1 || programme.Id == 2)
                    {
                        programme.Name = "NATIONAL DIPLOMA";
                    }
                    else
                    {
                        programme.Name = "HIGHER NATIONAL DIPLOMA";
                    }

                     double benchMarkScore = 40.0;
                      var TotalClassCount =
                        from a in
                            repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(
                                a =>
                                    a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                                    a.Course_Id == course.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id )
                        select new ExamRawScoreSheetReport{};
                   
                    var passedStudents =
                        from a in
                            repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(
                                a =>
                                    a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                                    a.Course_Id == course.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id && a.EX_CA >= benchMarkScore)
                        select new ExamRawScoreSheetReport{};

                    double PassStudentCount = Convert.ToDouble(passedStudents.Count());
                    double StudentCount = Convert.ToDouble(TotalClassCount.Count());
                    double successRate = (PassStudentCount / StudentCount) * (100);

                    var studentExamRawScoreSheetDetails = from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Level_Id == level.Id  && a.Programme_Id == programme.Id)
                                                                           select new ExamRawScoreSheetReport
                                                                           {
                                                                               CourseCode = a.Course_Code,
                                                                               CourseTitle = a.Course_Name,
                                                                               CourseUnit = a.Course_Unit,
                                                                               Department = a.Department_Name,
                                                                               Programme = programme.Name,
                                                                               MatricNumber = a.Student_Matric_Number,
                                                                               Date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year,
                                                                               Identifier = a.Department_Code+a.Level_Name+ GetSemesterCodeBy(a.Semester_Id) +GetSessionCodeBy(a.Session_Name),
                                                                               Session = a.Session_Name,
                                                                               Semester = a.Semester_Name,
                                                                               Faculty = a.Faculty_Name,
                                                                               QU1 = a.QU1.Value,
                                                                               QU2 = a.QU2.Value,
                                                                               QU3 = a.QU3.Value,
                                                                               QU4 = a.QU4.Value,
                                                                               QU5 = a.QU5.Value,
                                                                               QU6 = a.QU6.Value,
                                                                               QU7 = a.QU7.Value,
                                                                               QU8 = a.QU8.Value,
                                                                               QU9 = a.QU9.Value,
                                                                               T_CA = a.T_CA.Value,
                                                                               T_EX = a.T_EX.Value,
                                                                               EX_CA = a.EX_CA.Value,
                                                                               Remark = a.Remark,
                                                                               SuccessRate = successRate,
                                                                               Level = a.Level_Name
                                                                           };

                   studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a => a.MatricNumber).ToList();
                 
                }

            }
            catch(Exception)
            {

                throw;
            }

            return studentExamRawScoreSheetReportList.OrderBy(p => p.MatricNumber).ToList();
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
            catch(Exception)
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
                sessionCode = sessionYear.Substring(2,2);
                return sessionCode;
            }
            catch(Exception)
            {

                throw;
            }
        }

        private string GetSemesterCodeBy(int semesterId)
        {
            try
            {
                if(semesterId == 1)
                {
                    return "F";
                }
                else
                {
                    return "S";
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        private string GetLevelBy(int levelId,int programmeId)
        {
            string level = null;
            string progType = null;
            try
            {
                if(levelId == 1)
                {
                    level = "100 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                else if(levelId == 2)
                {
                    level = "200 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                else if(levelId == 3)
                {
                    level = "300 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                else
                {
                    level = "400 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                return level;

            }
            catch(Exception)
            {

                throw;
            }
        }

        private string GetProgramme(int programmeId)
        {
            try
            {
                if(programmeId == 1 || programmeId == 3)
                {
                    return "[FULL TIME]";
                }
                else
                {
                    return "[PART TIME]";
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        private string GetRemark(double? totalExam,double? totalTest)
        {
            string remark = null;
            try
            {
                if(totalExam == 101)
                {
                    remark = "SICK: EXAM";
                    return remark;
                }
                else if(totalTest == 101)
                {
                    remark = "SICK: TEST";
                    return remark;
                }
            }
            catch(Exception)
            {

                throw;
            }
            return remark;
        }

        public bool Modify(StudentExamRawScoreSheet studentExamRawScoreSheet)
        {
            try
            {
                if(studentExamRawScoreSheet != null)
                {
                    Expression<Func<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED,bool>> selector = p => p.Student_Result_Id == studentExamRawScoreSheet.Id;
                    STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED entity = GetEntityBy(selector);
                    entity.EX_CA = (double)studentExamRawScoreSheet.EX_CA;
                    entity.T_CA = (double)studentExamRawScoreSheet.T_CA;
                    entity.T_EX = (double)studentExamRawScoreSheet.T_EX;
                    entity.QU1 = (double)studentExamRawScoreSheet.QU1;
                    entity.QU2 = (double)studentExamRawScoreSheet.QU2;
                    entity.QU3 = (double)studentExamRawScoreSheet.QU3;
                    entity.QU4 = (double)studentExamRawScoreSheet.QU4;
                    entity.QU5 = (double)studentExamRawScoreSheet.QU5;
                    entity.QU6 = (double)studentExamRawScoreSheet.QU6;
                    entity.QU7 = (double)studentExamRawScoreSheet.QU7;
                    entity.QU8 = (double)studentExamRawScoreSheet.QU8;
                    entity.QU9 = (double)studentExamRawScoreSheet.QU9;
                    entity.Special_Case = studentExamRawScoreSheet.Special_Case;
                    entity.Remark = studentExamRawScoreSheet.Special_Case;
                    entity.Uploader_Id = studentExamRawScoreSheet.Uploader.Id;
                    entity.Upload_url = studentExamRawScoreSheet.FileUploadURL;
                    if (studentExamRawScoreSheet.Programme != null)
                    {
                        entity.Programme_Id = studentExamRawScoreSheet.Programme.Id;
                    }
                    int modifyCount = Save();

                }
            }
            catch(Exception)
            {

                throw;
            }
            return true;
        }
  
        public List<ResultFormat> GetDownloadResultFormats(long cid, int deptId,int progId, int levelId, int sessionId)
        {
            try
            {
                var resultFormatData = from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(a => a.Course_Id == cid && a.Programme_Id == progId && a.Level_Id == levelId&& a.Session_Id == sessionId)
                                           select new ResultFormat
                                           {
                                              
                                               MATRICNO = a.Student_Matric_Number,
                                               QU1 = (decimal)a.QU1,
                                               QU2 = (decimal)a.QU2,
                                               QU3 = (decimal)a.QU3,
                                               QU4 = (decimal)a.QU4,
                                               QU5 = (decimal)a.QU5,
                                               QU6 = (decimal)a.QU6,
                                               QU7 = (decimal)a.QU7,
                                               QU8 = (decimal)a.QU8,
                                               QU9 = (decimal)a.QU9,
                                               T_EX =(decimal)a.T_EX,
                                               T_CA =(decimal)a.T_CA,
                                               EX_CA =(decimal)a.EX_CA,
                                           };
                List<ResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                return resultFormatList;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<ExamRawScoreSheetReport> GetExamRawScoreSheetsToProcess(Session session, Semester semester, Level level, Programme programme, Department department)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetails = new List<ExamRawScoreSheetReport>();
            try
            {
                studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id && a.Department_Code == department.Code)
                                                      select new ExamRawScoreSheetReport
                                                      {
                                                          CourseCode = a.Course_Code,
                                                          CourseTitle = a.Course_Name,
                                                          CourseUnit = a.Course_Unit,
                                                          Department = a.Department_Name,
                                                          Programme = programme.Name,
                                                          MatricNumber = a.Student_Matric_Number,
                                                          Date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year,
                                                          Identifier = a.Department_Code + a.Level_Name + GetSemesterCodeBy(a.Semester_Id) + GetSessionCodeBy(a.Session_Name),
                                                          Session = a.Session_Name,
                                                          Semester = a.Semester_Name,
                                                          Faculty = a.Faculty_Name,
                                                          QU1 = a.QU1.Value,
                                                          QU2 = a.QU2.Value,
                                                          QU3 = a.QU3.Value,
                                                          QU4 = a.QU4.Value,
                                                          QU5 = a.QU5.Value,
                                                          QU6 = a.QU6.Value,
                                                          QU7 = a.QU7.Value,
                                                          QU8 = a.QU8.Value,
                                                          QU9 = a.QU9.Value,
                                                          T_CA = a.T_CA.Value,
                                                          T_EX = a.T_EX.Value,
                                                          EX_CA = a.EX_CA.Value,
                                                          Remark = a.Remark,
                                                          Level = a.Level_Name ,
                                                          ProgrammeId = a.Programme_Id,
                                                          LevelId = a.Level_Id,
                                                          SemesterId = a.Semester_Id,
                                                          SessionId = a.Session_Id,
                                                          CourseId = a.Course_Id,
                                                          SpecialCase = a.Special_Case,
                                                          StudentResultId = a.Student_Result_Id ,
                                                          UserId = a.Uploader_Id

                                                      }).ToList();
            }
            catch (Exception)
            {   
                throw;
            }

            return studentExamRawScoreSheetDetails;
        }
    }
}
