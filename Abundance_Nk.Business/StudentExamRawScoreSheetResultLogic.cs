using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class StudentExamRawScoreSheetResultLogic : BusinessBaseLogic<StudentExamRawScoreSheet, STUDENT_EXAM_RAW_SCORE_SHEET_RESULT>
    {
        public StudentExamRawScoreSheetResultLogic()
        {
            translator = new StudentExamRawScoreSheetTranslator();
        }

        public List<ExamRawScoreSheetReport> GetScoreSheetBy(Session session, Semester semester, Course course, Department department, Level level, Programme programme)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetReportList = new List<ExamRawScoreSheetReport>();
            try
            {
               
                 if (session != null && semester != null && course != null && department != null && level != null && programme != null)
                {
                    if (programme.Id == 1 || programme.Id == 2)
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
                            repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(
                                a =>
                                    a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                                    a.Course_Id == course.Id && a.Programme_Id == programme.Id )
                        select new ExamRawScoreSheetReport{};
                   
                    var passedStudents =
                        from a in
                            repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(
                                a =>
                                    a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                                    a.Course_Id == course.Id && a.Programme_Id == programme.Id && a.EX_CA >= benchMarkScore)
                        select new ExamRawScoreSheetReport{};

                    double PassStudentCount = Convert.ToDouble(passedStudents.Count());
                    double StudentCount = Convert.ToDouble(TotalClassCount.Count());
                    double successRate = (PassStudentCount / StudentCount) * (100);

                    List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id)
                                                                           select new ExamRawScoreSheetReport
                                                                           {
                                                                               CourseCode = a.Course_Code,
                                                                               CourseTitle = a.Course_Name,
                                                                               CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                                               Department = a.Department_Name,
                                                                               Programme = programme.Name,
                                                                               MatricNumber = a.Matric_Number,
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
                                                                               Level = a.Level_Name,
                                                                               UserId = a.Uploader_Id,
                                                                               Fullname=a.Name,
                                                                               SpecialCase=a.Special_Case,
                                                                               UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                                           }).ToList();

                    ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                    long staffUserId = 0;
                    if (examRawScoreSheetReport != null)
                    {
                        staffUserId = examRawScoreSheetReport.UserId; 
                    }  
                    StaffLogic staffLogic = new StaffLogic();
                    Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                   // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                    if (staff != null)
                    {
                        for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                        {
                            studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                        } 
                    }
                    else
                    {
                        UserLogic userLogic = new UserLogic();
                        User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                        if (user != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                            } 
                        }
                    }
                    
                    studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a=> a.MatricNumber).ToList();
                    
                }

            }
            catch (Exception)
            {

                throw;
            }

            return studentExamRawScoreSheetReportList;
        }

        public List<ExamRawScoreSheetReport> GetCAScoreSheetBy(Session session, Semester semester, Course course, Department department, Level level, Programme programme)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetReportList = new List<ExamRawScoreSheetReport>();
            try
            {

                if (session != null && semester != null && course != null && department != null && level != null && programme != null)
                {
                    if (programme.Id == 1 || programme.Id == 2)
                    {
                        programme.Name = "NATIONAL DIPLOMA";
                    }
                    else
                    {
                        programme.Name = "HIGHER NATIONAL DIPLOMA";
                    }
                    //double benchMarkScore = 30.0;
                    //var TotalClassCount =
                    //  from a in
                    //      repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(
                    //          a =>
                    //              a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                    //              a.Course_Id == course.Id && a.Programme_Id == programme.Id)
                    //  select new ExamRawScoreSheetReport { };

                    //var passedStudents =
                    //    from a in
                    //        repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(
                    //            a =>
                    //                a.Session_Id == session.Id && a.Semester_Id == semester.Id &&
                    //                a.Course_Id == course.Id && a.Programme_Id == programme.Id && a.EX_CA >= benchMarkScore)
                    //    select new ExamRawScoreSheetReport { };

                    //double PassStudentCount = Convert.ToDouble(passedStudents.Count());
                    //double StudentCount = Convert.ToDouble(TotalClassCount.Count());
                    //double successRate = (PassStudentCount / StudentCount) * (100);

                    List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id )
                                                                                     select new ExamRawScoreSheetReport
                                                                                     {
                                                                                         CourseCode = a.Course_Code,
                                                                                         CourseTitle = a.Course_Name,
                                                                                         CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                                                         Department = a.Department_Name,
                                                                                         Programme = programme.Name,
                                                                                         MatricNumber = a.Matric_Number,
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
                                                                                         //SuccessRate = successRate,
                                                                                         Level = a.Level_Name,
                                                                                         UserId = a.Uploader_Id,

                                                                                         UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                                                     }).ToList();


                    ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                    long staffUserId = 0;
                    if (examRawScoreSheetReport != null)
                    {
                        staffUserId = examRawScoreSheetReport.UserId;
                    }
                    StaffLogic staffLogic = new StaffLogic();
                    Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                    // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                    if (staff != null)
                    {
                        for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                        {
                            studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                        }
                    }
                    else
                    {
                        UserLogic userLogic = new UserLogic();
                        User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                        if (user != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                            }
                        }
                    }

                    studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a => a.MatricNumber).ToList();

                }

            }
            catch (Exception)
            {

                throw;
            }

            return studentExamRawScoreSheetReportList;
        }
        public List<ExamRawScoreSheetReport> GetCBEScoreSheetBy(Session session, Semester semester, Course course, Department department, Level level, Programme programme)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetReportList = new List<ExamRawScoreSheetReport>();
            try
            {

                if (session != null && semester != null && course != null && department != null && level != null && programme != null)
                {
                    if (programme.Id == 1 || programme.Id == 2)
                    {
                        programme.Name = "NATIONAL DIPLOMA";
                    }
                    else
                    {
                        programme.Name = "HIGHER NATIONAL DIPLOMA";
                    }

                    List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id)
                                                                                     select new ExamRawScoreSheetReport
                                                                                     {
                                                                                         CourseCode = a.Course_Code,
                                                                                         CourseTitle = a.Course_Name,
                                                                                         CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                                                         Department = a.Department_Name,
                                                                                         Programme = programme.Name,
                                                                                         MatricNumber = a.Matric_Number,
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
                                                                                         //SuccessRate = successRate,
                                                                                         Level = a.Level_Name,
                                                                                         UserId = a.Uploader_Id,

                                                                                         UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                                                     }).ToList();


                    ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                    long staffUserId = 0;
                    if (examRawScoreSheetReport != null)
                    {
                        staffUserId = examRawScoreSheetReport.UserId;
                    }
                    StaffLogic staffLogic = new StaffLogic();
                    Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                    // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                    if (staff != null)
                    {
                        for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                        {
                            studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                        }
                    }
                    else
                    {
                        UserLogic userLogic = new UserLogic();
                        User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                        if (user != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                            }
                        }
                    }

                    studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a => a.MatricNumber).ToList();

                }

            }
            catch (Exception)
            {

                throw;
            }

            return studentExamRawScoreSheetReportList;
        }
        public List<ExamRawScoreSheetReport> GetScoreSheetAltBy(Session session, Semester semester, Course course, Department department, Level level, Programme programme, CourseMode courseMode)
        {
            List<ExamRawScoreSheetReport> studentExamRawScoreSheetReportList = new List<ExamRawScoreSheetReport>();
            try
            { 
                if (session != null && semester != null && course != null && department != null && level != null && programme != null)
                {
                    if (programme.Id == 1 || programme.Id == 2)
                    {
                        programme.Name = "NATIONAL DIPLOMA";
                    }
                    else
                    {
                        programme.Name = "HIGHER NATIONAL DIPLOMA";
                    }
                    double benchMarkScore = 40.0;
                    var TotalClassCount = from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id )
                      select new ExamRawScoreSheetReport { };

                    var passedStudents = from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id && a.EX_CA >= benchMarkScore)
                        select new ExamRawScoreSheetReport { };

                    double PassStudentCount = Convert.ToDouble(passedStudents.Count());
                    double StudentCount = Convert.ToDouble(TotalClassCount.Count());
                    double successRate = (PassStudentCount / StudentCount) * (100);

                    List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetails = new List<ExamRawScoreSheetReport>();
                    List<ExamRawScoreSheetReport> studentExamRawScoreSheetDetailsCarryOver = new List<ExamRawScoreSheetReport>();

                    if (courseMode.Id == (int)CourseModes.FirstAttempt || courseMode.Id == (int)CourseModes.CarryOver)
                    {
                        if (courseMode.Id == (int)CourseModes.FirstAttempt)
                        {
                            studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id && a.Course_Mode_Id == courseMode.Id)
                                                               select new ExamRawScoreSheetReport
                                                               {
                                                                   CourseCode = a.Course_Code,
                                                                   CourseTitle = a.Course_Name,
                                                                   CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                                   Department = a.Department_Name,
                                                                   Programme = programme.Name,
                                                                   MatricNumber = a.Matric_Number,
                                                                   Date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year,
                                                                   Identifier = a.Department_Code + a.Level_Name + GetSemesterCodeBy(a.Semester_Id) + GetSessionCodeBy(a.Session_Name),
                                                                   Session = a.Session_Name,
                                                                   Semester = a.Semester_Name,
                                                                   Faculty = a.Faculty_Name,
                                                                   //QU1 = a.QU1.Value,
                                                                   //QU2 = a.QU2.Value,
                                                                   //QU3 = a.QU3.Value,
                                                                   //QU4 = a.QU4.Value,
                                                                   //QU5 = a.QU5.Value,
                                                                   //QU6 = a.QU6.Value,
                                                                   //QU7 = a.QU7.Value,
                                                                   //QU8 = a.QU8.Value,
                                                                   //QU9 = a.QU9.Value,
                                                                   T_CA = a.T_CA.Value,
                                                                   T_EX = a.T_EX.Value,
                                                                   EX_CA = a.EX_CA.Value,
                                                                   Remark = a.Remark,
                                                                   SuccessRate = successRate,
                                                                   Level = a.Level_Name,
                                                                   CourseModeId = a.Course_Mode_Id,
                                                                   CourseModeName = a.Course_Mode_Name,
                                                                   UserId = a.Uploader_Id,
                                                                   Fullname=a.Name,
                                                                   SpecialCase=a.Special_Case,
                                                                   UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                               }).ToList();
                        }
                        else
                        {
                            studentExamRawScoreSheetDetailsCarryOver = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id && a.Course_Mode_Id == courseMode.Id)
                                                               select new ExamRawScoreSheetReport
                                                               {
                                                                   CourseCode = a.Course_Code,
                                                                   CourseTitle = a.Course_Name,
                                                                   CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                                   Department = a.Department_Name,
                                                                   Programme = programme.Name,
                                                                   MatricNumber = a.Matric_Number,
                                                                   Date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year,
                                                                   Identifier = a.Department_Code + a.Level_Name + GetSemesterCodeBy(a.Semester_Id) + GetSessionCodeBy(a.Session_Name),
                                                                   Session = a.Session_Name,
                                                                   Semester = a.Semester_Name,
                                                                   Faculty = a.Faculty_Name,
                                                                   //QU1 = a.QU1.Value,
                                                                   //QU2 = a.QU2.Value,
                                                                   //QU3 = a.QU3.Value,
                                                                   //QU4 = a.QU4.Value,
                                                                   //QU5 = a.QU5.Value,
                                                                   //QU6 = a.QU6.Value,
                                                                   //QU7 = a.QU7.Value,
                                                                   //QU8 = a.QU8.Value,
                                                                   //QU9 = a.QU9.Value,
                                                                   T_CA = a.T_CA.Value,
                                                                   T_EX = a.T_EX.Value,
                                                                   EX_CA = a.EX_CA.Value,
                                                                   Remark = a.Remark,
                                                                   SuccessRate = successRate,
                                                                   Level = a.Level_Name,
                                                                   CourseModeId = a.Course_Mode_Id,
                                                                   CourseModeName = a.Course_Mode_Name,
                                                                   UserId = a.Uploader_Id,
                                                                   UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                               }).ToList();


                            if (session.Id == 7)
                            {
                                for (int i = 0; i < studentExamRawScoreSheetDetailsCarryOver.Count; i++)
                                {
                                    if (studentExamRawScoreSheetDetailsCarryOver[i].MatricNumber.Contains("/15/") || studentExamRawScoreSheetDetailsCarryOver[i].MatricNumber.Contains("/16/"))
                                    {
                                        studentExamRawScoreSheetDetails.Add(studentExamRawScoreSheetDetailsCarryOver[i]);
                                    }
                                    else
                                    {
                                        //Do Nothing
                                    }
                                }
                            }
                            else if(session.Id == 1)
                            {
                                for (int i = 0; i < studentExamRawScoreSheetDetailsCarryOver.Count; i++)
                                {
                                    if (studentExamRawScoreSheetDetailsCarryOver[i].MatricNumber.Contains("/14/") || studentExamRawScoreSheetDetailsCarryOver[i].MatricNumber.Contains("/15/"))
                                    {
                                        studentExamRawScoreSheetDetails.Add(studentExamRawScoreSheetDetailsCarryOver[i]);
                                    }
                                    else
                                    {
                                        //Do Nothing
                                    }
                                }   
                            }
                        }
                        


                        ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                        long staffUserId = 0;
                        if (examRawScoreSheetReport != null)
                        {
                            staffUserId = examRawScoreSheetReport.UserId;
                        }
                        StaffLogic staffLogic = new StaffLogic();
                        Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                        // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                        if (staff != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                            }
                        }
                        else
                        {
                            UserLogic userLogic = new UserLogic();
                            User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                            if (user != null)
                            {
                                for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                                {
                                    studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                                }
                            }
                        }

                        studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a => a.MatricNumber).ToList();
                    }
                    else if (courseMode.Id == 3)
                    {
                        studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id)
                                                           select new ExamRawScoreSheetReport
                                                           {
                                                               CourseCode = a.Course_Code,
                                                               CourseTitle = a.Course_Name,
                                                               CourseUnit = Convert.ToInt32(a.Course_Unit),
                                                               Department = a.Department_Name,
                                                               Programme = programme.Name,
                                                               MatricNumber = a.Matric_Number,
                                                               Date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year,
                                                               Identifier = a.Department_Code + a.Level_Name + GetSemesterCodeBy(a.Semester_Id) + GetSessionCodeBy(a.Session_Name),
                                                               Session = a.Session_Name,
                                                               Semester = a.Semester_Name,
                                                               Faculty = a.Faculty_Name,
                                                               //QU1 = a.QU1.Value,
                                                               //QU2 = a.QU2.Value,
                                                               //QU3 = a.QU3.Value,
                                                               //QU4 = a.QU4.Value,
                                                               //QU5 = a.QU5.Value,
                                                               //QU6 = a.QU6.Value,
                                                               //QU7 = a.QU7.Value,
                                                               //QU8 = a.QU8.Value,
                                                               //QU9 = a.QU9.Value,
                                                               T_CA = a.T_CA.Value,
                                                               T_EX = a.T_EX.Value,
                                                               EX_CA = a.EX_CA.Value,
                                                               Remark = a.Remark,
                                                               SuccessRate = successRate,
                                                               Level = a.Level_Name,
                                                               CourseModeId = a.Course_Mode_Id,
                                                               CourseModeName = a.Course_Mode_Name,
                                                               UserId = a.Uploader_Id,
                                                               UploadDate = a.Upload_Date.Value.ToLongDateString()
                                                           }).ToList();


                        ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                        long staffUserId = 0;
                        if (examRawScoreSheetReport != null)
                        {
                            staffUserId = examRawScoreSheetReport.UserId;
                        }
                        StaffLogic staffLogic = new StaffLogic();
                        Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                        // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                        if (staff != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                                studentExamRawScoreSheetDetails[i].CourseModeName = "Extra Year";
                            }
                        }
                        else
                        {
                            UserLogic userLogic = new UserLogic();
                            User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                            if (user != null)
                            {
                                for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                                {
                                    studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                                    studentExamRawScoreSheetDetails[i].CourseModeName = "Extra Year";
                                }
                            }
                        }

                        if (session.Id == 7)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                if (studentExamRawScoreSheetDetails[i].MatricNumber.Contains("/15/") || studentExamRawScoreSheetDetails[i].MatricNumber.Contains("/16/"))
                                {
                                    //Do Nothing
                                }
                                else
                                {
                                    studentExamRawScoreSheetReportList.Add(studentExamRawScoreSheetDetails[i]);
                                }
                            }
                        }
                        else if(session.Id == 1)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                if (studentExamRawScoreSheetDetails[i].MatricNumber.Contains("/14/") || studentExamRawScoreSheetDetails[i].MatricNumber.Contains("/15/"))
                                {
                                    //Do Nothing
                                }
                                else
                                {
                                    studentExamRawScoreSheetReportList.Add(studentExamRawScoreSheetDetails[i]);
                                }
                            }
                        }
                        

                        studentExamRawScoreSheetReportList = studentExamRawScoreSheetReportList.OrderBy(a => a.MatricNumber).ToList();
                    }
                    else if (courseMode.Id == 4)
                    {
                        studentExamRawScoreSheetDetails = (from a in repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(a => a.Session_Id == session.Id && a.Semester_Id == semester.Id && a.Course_Id == course.Id && a.Programme_Id == programme.Id)
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
                                                              //QU1 = a.QU1.Value,
                                                              //QU2 = a.QU2.Value,
                                                              //QU3 = a.QU3.Value,
                                                              //QU4 = a.QU4.Value,
                                                              //QU5 = a.QU5.Value,
                                                              //QU6 = a.QU6.Value,
                                                              //QU7 = a.QU7.Value,
                                                              //QU8 = a.QU8.Value,
                                                              //QU9 = a.QU9.Value,
                                                              T_CA = a.T_CA.Value,
                                                              T_EX = a.T_EX.Value,
                                                              EX_CA = a.EX_CA.Value,
                                                              Remark = a.Remark,
                                                              SuccessRate = successRate,
                                                              Level = a.Level_Name,
                                                              UserId = a.Uploader_Id
                                                          }).ToList();

                        ExamRawScoreSheetReport examRawScoreSheetReport = studentExamRawScoreSheetDetails.FirstOrDefault();
                        long staffUserId = 0;
                        if (examRawScoreSheetReport != null)
                        {
                            staffUserId = examRawScoreSheetReport.UserId;
                        }
                        StaffLogic staffLogic = new StaffLogic();
                        Staff staff = staffLogic.GetModelBy(s => s.User_Id == staffUserId);
                        // string staffName = staff.LastName + " " + staff.FirstName + " " + staff.OtherName;
                        if (staff != null)
                        {
                            for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                            {
                                studentExamRawScoreSheetDetails[i].StaffName = staff.FullName;
                                studentExamRawScoreSheetDetails[i].CourseModeName = "Alternate";
                            }
                        }
                        else
                        {
                            UserLogic userLogic = new UserLogic();
                            User user = userLogic.GetModelBy(u => u.User_Id == staffUserId);
                            if (user != null)
                            {
                                for (int i = 0; i < studentExamRawScoreSheetDetails.Count; i++)
                                {
                                    studentExamRawScoreSheetDetails[i].StaffName = user.Username;
                                    studentExamRawScoreSheetDetails[i].CourseModeName = "Alternate";
                                }
                            }
                        }

                        studentExamRawScoreSheetReportList = studentExamRawScoreSheetDetails.OrderBy(a => a.MatricNumber).ToList();
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            CourseLogic courseLogic = new CourseLogic();
            course = courseLogic.GetModelBy(c => c.Course_Id == course.Id);
            if (course.DepartmentOption != null && course.DepartmentOption.Id > 0)
            {
                for (int i = 0; i < studentExamRawScoreSheetReportList.Count; i++)
                {
                    studentExamRawScoreSheetReportList[i].Department =  studentExamRawScoreSheetReportList[i].Department + " (" + course.DepartmentOption.Name +")";
                }
            }
            
            return studentExamRawScoreSheetReportList;
        }
        private string GetIdentifierBy(STUDENT_EXAM_RAW_SCORE_SHEET_RESULT rawscoresheetItem)
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

        private string GetLevelBy(int levelId, int programmeId)
        {
            string level = null;
            string progType = null;
            try
            {
                if (levelId == 1)
                {
                    level = "100 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                else if (levelId == 2)
                {
                    level = "200 LEVEL";
                    progType = GetProgramme(programmeId);
                    level = level + " " + progType;
                }
                else if (levelId == 3)
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
            catch (Exception)
            {
                
                throw;
            }
        }

        private string GetProgramme(int programmeId)
        {
            try
            {
                if (programmeId == 1 || programmeId == 3)
                {
                    return "[FULL TIME]";
                }
                else
                {
                    return "[PART TIME]";
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private string GetRemark(double? totalExam, double? totalTest)
        {
            string remark = null;
            try
            {
                if (totalExam == 101)
                {
                    remark = "SICK: EXAM";
                    return remark;
                }
                else if (totalTest == 101)
                {
                    remark = "SICK: TEST";
                    return remark;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return remark;
        }

        public bool Modify(StudentExamRawScoreSheet studentExamRawScoreSheet)
        {
            try
            {
                if (studentExamRawScoreSheet != null)
                {
                    Expression<Func<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT, bool>> selector = p => p.Student_Result_Id == studentExamRawScoreSheet.Id;
                    STUDENT_EXAM_RAW_SCORE_SHEET_RESULT entity = GetEntityBy(selector);

                    if (studentExamRawScoreSheet.EX_CA != null)
                    {
                        entity.EX_CA = (double)studentExamRawScoreSheet.EX_CA;
                    }
                    if (studentExamRawScoreSheet.T_CA != null)
                    {
                        entity.T_CA = (double)studentExamRawScoreSheet.T_CA;
                    }
                    if (studentExamRawScoreSheet.T_EX != null)
                    {
                        entity.T_EX = (double)studentExamRawScoreSheet.T_EX;
                    }
                    if (studentExamRawScoreSheet.QU1 != null)
                    {
                        entity.QU1 = (double)studentExamRawScoreSheet.QU1;
                    }
                    if (studentExamRawScoreSheet.QU2 != null)
                    {
                        entity.QU2 = (double)studentExamRawScoreSheet.QU2;
                    }
                    if (studentExamRawScoreSheet.QU3 != null)
                    {
                        entity.QU3 = (double)studentExamRawScoreSheet.QU3;
                    }
                    if (studentExamRawScoreSheet.QU4 != null)
                    {
                        entity.QU4 = (double)studentExamRawScoreSheet.QU4;
                    }
                    if (studentExamRawScoreSheet.QU5 != null)
                    {
                        entity.QU5 = (double)studentExamRawScoreSheet.QU5;
                    }
                    if (studentExamRawScoreSheet.QU6 != null)
                    {
                        entity.QU6 = (double)studentExamRawScoreSheet.QU6;
                    }
                    if (studentExamRawScoreSheet.QU7 != null)
                    {
                        entity.QU7 = (double)studentExamRawScoreSheet.QU7;
                    }
                    if (studentExamRawScoreSheet.QU8 != null)
                    {
                        entity.QU8 = (double)studentExamRawScoreSheet.QU8;
                    }
                    if (studentExamRawScoreSheet.QU9 != null)
                    {
                        entity.QU9 = (double)studentExamRawScoreSheet.QU9;
                    }
                    
                    entity.Special_Case = studentExamRawScoreSheet.Special_Case;
                    entity.Remark = studentExamRawScoreSheet.Special_Case;

                    if (studentExamRawScoreSheet.Uploader != null)
                    {
                        entity.Uploader_Id = studentExamRawScoreSheet.Uploader.Id;
                    }
                    
                    if (studentExamRawScoreSheet.Course != null)
                    {
                        entity.Course_Id = studentExamRawScoreSheet.Course.Id;
                    }
                    if (studentExamRawScoreSheet.Student != null)
                    {
                        entity.Student_Id = studentExamRawScoreSheet.Student.Id;
                    }

                    int modifyCount = Save();
                   
                }
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }
        public List<ExamRawScoreSheetReport> GetStudentsWithoutResult(Session session, Semester semester, Department department, Level level, Programme programme)
        {
            List<ExamRawScoreSheetReport> studentList = new List<ExamRawScoreSheetReport>();
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception("One or more criteria to get this report is not set! Please check your input criteria selection and try again.");
                }

                List<ExamRawScoreSheetReport> results = (from sr in repository.GetBy<VW_STUDENT_WITHOUT_RESULT>(x => x.Session_Id == session.Id && x.Semester_Id == semester.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id)
                                                         select new ExamRawScoreSheetReport
                                                         {
                                                             Fullname = sr.NAME,
                                                             MatricNumber = sr.Matric_Number,
                                                             Programme = sr.Programme_Name,
                                                             Department = sr.Department_Name,
                                                             Level = sr.Level_Name,
                                                             CourseCode = sr.Course_Code,
                                                             CourseModeName = sr.Course_Mode_Name,
                                                             Session = sr.Session_Name,
                                                             Semester = sr.Semester_Name

                                                         }).ToList();
                List<string> regNumbers = results.Select(r => r.MatricNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ExamRawScoreSheetReport> currentStudent = results.Where(r => r.MatricNumber == regNumbers[i]).ToList();
                    ExamRawScoreSheetReport examRawScoreSheetReport = new ExamRawScoreSheetReport();
                    examRawScoreSheetReport.MatricNumber = currentStudent.FirstOrDefault().MatricNumber;
                    examRawScoreSheetReport.Fullname = currentStudent.FirstOrDefault().Fullname;
                    examRawScoreSheetReport.Programme = currentStudent.FirstOrDefault().Programme;
                    examRawScoreSheetReport.Department = currentStudent.FirstOrDefault().Department;
                    examRawScoreSheetReport.Level = currentStudent.FirstOrDefault().Level;
                    examRawScoreSheetReport.Session = currentStudent.FirstOrDefault().Session;
                    examRawScoreSheetReport.Semester = currentStudent.FirstOrDefault().Semester;
                    examRawScoreSheetReport.CourseCode = "";
                    for (int j = 0; j < currentStudent.Count; j++)
                    {
                        examRawScoreSheetReport.CourseCode += " | " + currentStudent[j].CourseCode;
                    }

                    studentList.Add(examRawScoreSheetReport);
                }

                return studentList.OrderBy(r => r.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
