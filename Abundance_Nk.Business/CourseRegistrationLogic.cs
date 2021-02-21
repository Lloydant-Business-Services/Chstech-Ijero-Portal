using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Transactions;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class CourseRegistrationLogic : BusinessBaseLogic<CourseRegistration, STUDENT_COURSE_REGISTRATION>
    {
        private CourseRegistrationDetailLogic courseRegistrationDetailLogic;
        public CourseRegistrationLogic()
        {
            translator = new CourseRegistrationTranslator();
            courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
        }
        public CourseRegistration GetBy(Student student, Level level, Programme programme, Department department, Session session)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector = cr => cr.Person_Id == student.Id && cr.Level_Id == level.Id && cr.Programme_Id == programme.Id && cr.Department_Id == department.Id && cr.Session_Id == session.Id;
                CourseRegistration registeredCourse = new CourseRegistration();
                registeredCourse = GetModelBy(selector);
                if (registeredCourse != null && registeredCourse.Id > 0)
                {
                    registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                }

                return registeredCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CourseRegistration GetBy(Student student, Programme programme, Department department)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector = cr => cr.Person_Id == student.Id && cr.Programme_Id == programme.Id && cr.Department_Id == department.Id;
                CourseRegistration registeredCourse = new CourseRegistration();
                registeredCourse = GetModelBy(selector);
                if (registeredCourse != null && registeredCourse.Id > 0)
                {
                    registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                }

                return registeredCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CourseRegistration> GetListBy(Student student, Programme programme, Department department)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector = cr => cr.Person_Id == student.Id && cr.Programme_Id == programme.Id && cr.Department_Id == department.Id;
                List<CourseRegistration> registeredCourses = new List<CourseRegistration>();
                registeredCourses = GetModelsBy(selector);
                if (registeredCourses != null && registeredCourses.Count > 0)
                {
                    foreach (CourseRegistration registeredCourse in registeredCourses)
                    {
                        registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                    }

                }

                return registeredCourses;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Result> GetExamSheetBy(Level level, Programme programme, Department department, Session session, Semester semester)
        {
            try
            {
                if (session == null || session.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || semester == null || semester.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Exam Sheet not set! Please check your input criteria selection and try again.");
                }


                List<Result> results = (from sr in repository.GetBy<VW_REGISTERED_COURSES>(x => x.Session_Id == session.Id && x.Semester_Id == semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
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

                                        }).ToList();

                return results;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public CourseRegistration Create(CourseRegistration courseRegistration,CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                int rowAdded = 0;
                CourseRegistration newCourseRegistration = null;
                CourseLogic courseLogic = new CourseLogic();
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {
                    newCourseRegistration = GetModelBy(a => a.Person_Id == courseRegistration.Student.Id && a.Session_Id == courseRegistration.Session.Id && a.Programme_Id == courseRegistration.Programme.Id && a.Department_Id == courseRegistration.Department.Id && a.Level_Id == courseRegistration.Level.Id);
                    if (newCourseRegistration == null)
                    {
                        newCourseRegistration = base.Create(courseRegistration);
                        if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistration.Details)
                            {
                                courseRegistrationDetail.CourseRegistration = newCourseRegistration;
                                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseRegistrationDetail.Course.Id);
                                courseRegistrationDetail.CourseUnit = course.Unit;
                            }



                            rowAdded = courseRegistrationDetailLogic.Create(courseRegistration.Details,courseRegistrationDetailAudit);
                            //if (rowAdded > 0 && rowAdded == courseRegistration.Details.Count)
                            //{
                            //    transaction.Complete();
                            //}
                            if (rowAdded > 0)
                            {
                                transaction.Complete();
                            }
                        }
                    }

                }

                return rowAdded > 0 ? newCourseRegistration : null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(CourseRegistration courseRegistration,CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                string errorMessage = "Course Registration modification failed!";

                bool modified = false;
                if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        List<CourseRegistrationDetail> existingCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id > 0 && c.Course.IsRegistered == true).ToList();
                        List<CourseRegistrationDetail> removedCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id > 0 && c.Course.IsRegistered == false).ToList();
                        List<CourseRegistrationDetail> newCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id <= 0 && c.Course.IsRegistered == true).ToList();

                        int newCourseRegistrationDetailCount = newCourseRegistrationDetails.Count;
                        int removedCourseRegistrationDetailCount = removedCourseRegistrationDetails.Count;

                        if (newCourseRegistrationDetailCount <= 0 && removedCourseRegistrationDetailCount <= 0)
                        {
                            throw new Exception("No changes detected! You need to make some changes before cliking the Register Course button.");
                        }

                        if (newCourseRegistrationDetailCount == removedCourseRegistrationDetailCount) //straight modification
                        {
                            if (!courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetails, newCourseRegistrationDetails,courseRegistrationDetailAudit))
                            {
                                throw new Exception(errorMessage);
                            }
                        }
                        else if (newCourseRegistrationDetailCount > removedCourseRegistrationDetailCount)
                        {
                            int difference = newCourseRegistrationDetailCount - removedCourseRegistrationDetailCount;
                            List<CourseRegistrationDetail> removedCourseRegistrationDetailsToModify = removedCourseRegistrationDetails.Take(removedCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToModify = newCourseRegistrationDetails.Take(removedCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToAdd = newCourseRegistrationDetails.Skip(removedCourseRegistrationDetailCount).Take(difference).ToList();

                            if (removedCourseRegistrationDetailsToModify == null || removedCourseRegistrationDetailsToModify.Count <= 0 || removedCourseRegistrationDetailCount <= 0)
                            {
                                if (courseRegistrationDetailLogic.Create(newCourseRegistrationDetailsToAdd,courseRegistrationDetailAudit) > 0)
                                {
                                    modified = true;
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                            else if (removedCourseRegistrationDetailsToModify != null && removedCourseRegistrationDetailsToModify.Count > 0 && newCourseRegistrationDetailsToModify != null && newCourseRegistrationDetailsToModify.Count > 0 && newCourseRegistrationDetailsToModify.Count == removedCourseRegistrationDetailsToModify.Count)
                            {
                                modified = courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetailsToModify, newCourseRegistrationDetailsToModify,courseRegistrationDetailAudit);
                                if (modified)
                                {
                                    if (courseRegistrationDetailLogic.Create(newCourseRegistrationDetailsToAdd,courseRegistrationDetailAudit) > 0)
                                    {
                                        modified = true;
                                    }
                                    else
                                    {
                                        throw new Exception(errorMessage);
                                    }
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                        }
                        else if (newCourseRegistrationDetailCount < removedCourseRegistrationDetailCount)
                        {
                            int difference = removedCourseRegistrationDetailCount - newCourseRegistrationDetailCount;
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToModify = newCourseRegistrationDetails.Take(newCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> removedCourseRegistrationDetailsToModify = removedCourseRegistrationDetails.Take(newCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> courseRegistrationDetailsToDelete = removedCourseRegistrationDetails.Skip(newCourseRegistrationDetailCount).Take(difference).ToList();

                            if (newCourseRegistrationDetailCount <= 0)
                            {
                                if (courseRegistrationDetailLogic.Remove(courseRegistrationDetailsToDelete,courseRegistrationDetailAudit))
                                {
                                    modified = true;
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                            else
                            {
                                modified = courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetailsToModify, newCourseRegistrationDetailsToModify,courseRegistrationDetailAudit);
                                if (modified)
                                {
                                    if (!courseRegistrationDetailLogic.Remove(courseRegistrationDetailsToDelete,courseRegistrationDetailAudit))
                                    {
                                        throw new Exception(errorMessage);
                                    }
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                        }

                        transaction.Complete();
                    }
                }

                return modified;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ModifyRegOnly(CourseRegistration courseRegistration,CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector = s => s.Student_Course_Registration_Id == courseRegistration.Id;
                STUDENT_COURSE_REGISTRATION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (courseRegistration.Department != null)
                {
                    entity.Department_Id = courseRegistration.Department.Id;
                }
                if (courseRegistration.Programme != null)
                {
                    entity.Programme_Id = courseRegistration.Programme.Id;
                }
                if (courseRegistration.Level != null)
                {
                    entity.Level_Id = courseRegistration.Level.Id;
                }
                if (courseRegistration.Session != null)
                {
                    entity.Session_Id = courseRegistration.Session.Id;
                }
                if (courseRegistration.Student != null)
                {
                    entity.Person_Id = courseRegistration.Student.Id;
                }

                int modifiedRecordCount = Save();

                if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                {
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                    for (int i = 0; i < courseRegistration.Details.Count; i++)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = courseRegistration.Details[i];
                        CourseRegistrationDetail newCourseRegistrationDetail = courseRegistrationDetailLogic.GetModelBy(c => c.Student_Course_Registration_Detail_Id == courseRegistrationDetail.Id);

                        newCourseRegistrationDetail.CourseUnit = courseRegistrationDetail.CourseUnit;
                        newCourseRegistrationDetail.ExamScore = courseRegistrationDetail.ExamScore;
                        newCourseRegistrationDetail.TestScore = courseRegistrationDetail.TestScore;
                        newCourseRegistrationDetail.Mode = courseRegistrationDetail.Mode;

                        courseRegistrationDetailLogic.Modify(newCourseRegistrationDetail,courseRegistrationDetailAudit);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CourseRegistration CreateCourseRegistration(CourseRegistration courseRegistration)
        {
            try
            {
                CourseRegistration newCourseRegistration = null;
                newCourseRegistration = GetModelBy(a => a.Person_Id == courseRegistration.Student.Id && a.Session_Id == courseRegistration.Session.Id && a.Programme_Id == courseRegistration.Programme.Id && a.Department_Id == courseRegistration.Department.Id && a.Level_Id == courseRegistration.Level.Id);
                if (newCourseRegistration == null)
                {
                    newCourseRegistration = base.Create(courseRegistration);
                }
                return newCourseRegistration;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<CarryOverReportModel> GetCarryOverList(Department department, Programme programme, Session session, Level level, Semester semester)
        {
            try
            {
                List<CarryOverReportModel> CarryOverStudentList = new List<CarryOverReportModel>();

                StudentLogic studentLogic = new StudentLogic();
                CourseLogic courseLogic = new CourseLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                SessionLogic sessionLogic = new SessionLogic();
                SemesterLogic semesterLogic = new SemesterLogic();
                LevelLogic levelLogic = new LevelLogic();

                Department departmentNew = departmentLogic.GetModelBy(p => p.Department_Id == department.Id);
                Programme programmeNew = programmeLogic.GetModelBy(p => p.Programme_Id == programme.Id);
                Session sessionNew = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                Semester semesterNew = semesterLogic.GetModelBy(p => p.Semester_Id == semester.Id);
                Level levelNew = levelLogic.GetModelBy(p => p.Level_Id == level.Id);

                List<CourseRegistration> courseRegistration = GetModelsBy(p => p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id);

                foreach (CourseRegistration itemList in courseRegistration)
                {
                    List<CourseRegistrationDetail> courseRegistrationDetailList = courseRegistrationDetailLogic.GetModelsBy(p => p.Student_Course_Registration_Id == itemList.Id && p.Semester_Id == semester.Id && (p.Exam_Score + p.Test_Score) <= 39);
                    foreach (CourseRegistrationDetail courseRegDetail in courseRegistrationDetailList)
                    {
                        string name = courseRegistrationDetailLogic.GetModelBy(p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id).CourseRegistration.Student.FullName;
                        string matricNo = courseRegistrationDetailLogic.GetModelBy(p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id).CourseRegistration.Student.MatricNumber;
                        Course course = courseLogic.GetModelBy(p => p.Course_Id == courseRegDetail.Course.Id);
                        CarryOverStudentList.Add(new CarryOverReportModel()
                        {
                            CourseCode = course.Code,
                            CourseName = course.Name,
                            CourseUnit = course.Unit,
                            Department = departmentNew.Name,
                            Programme = programmeNew.Name,
                            Fullname = name,
                            MatricNo = matricNo,
                            Semester = semesterNew.Name,
                            Session = sessionNew.Name,
                            Level = levelNew.Name,
                        });
                    }
                }
                return CarryOverStudentList.OrderBy(p => p.Fullname).ToList();
            }

            catch (Exception)
            {

                throw;
            }
        }
        public List<CarryOverReportModel> GetCarryOverCourseList(Session session, Semester semester, Programme programme, Department department, Level level, Course course)
        {
            try
            {
                List<CarryOverReportModel> CarryOverCourseList = new List<CarryOverReportModel>();

                CourseLogic courseLogic = new CourseLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                SessionLogic sessionLogic = new SessionLogic();
                SemesterLogic semesterLogic = new SemesterLogic();
                LevelLogic levelLogic = new LevelLogic();

                Department departmentNew = departmentLogic.GetModelBy(p => p.Department_Id == department.Id);
                Programme programmeNew = programmeLogic.GetModelBy(p => p.Programme_Id == programme.Id);
                Course courseNew = courseLogic.GetModelBy(p => p.Course_Id == course.Id);
                Session sessionNew = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                Semester semesterNew = semesterLogic.GetModelBy(p => p.Semester_Id == semester.Id);
                Level levelNew = levelLogic.GetModelBy(p => p.Level_Id == level.Id);

                List<CourseRegistration> courseRegistration = GetModelsBy(p => p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id && p.Programme_Id == programme.Id);

                foreach (CourseRegistration itemList in courseRegistration)
                {
                    List<CourseRegistrationDetail> courseRegistrationDetailList = courseRegistrationDetailLogic.GetModelsBy(p => p.Student_Course_Registration_Id == itemList.Id && p.Course_Id == course.Id && p.Semester_Id == semester.Id && (p.Exam_Score + p.Test_Score) <= 39);
                    foreach (CourseRegistrationDetail courseRegDetail in courseRegistrationDetailList)
                    {
                        string name = courseRegistrationDetailLogic.GetModelBy(p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id).CourseRegistration.Student.FullName;
                        string matricNo = courseRegistrationDetailLogic.GetModelBy(p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id).CourseRegistration.Student.MatricNumber;
                        CarryOverCourseList.Add(new CarryOverReportModel()
                        {
                            CourseCode = courseNew.Code,
                            CourseName = courseNew.Name,
                            CourseUnit = courseNew.Unit,
                            Department = departmentNew.Name,
                            Programme = programmeNew.Name,
                            Fullname = name,
                            MatricNo = matricNo,
                            Semester = semesterNew.Name,
                            Session = sessionNew.Name,
                            Level = levelNew.Name,
                        });
                    }
                }
                return CarryOverCourseList.OrderBy(p => p.MatricNo).ToList();
            }

            catch (Exception)
            {

                throw;
            }
        }
        public List<PaymentReport> GetRegistrationBy(Session session, Semester semester)
        {
            CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
            CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            List<STUDENT_COURSE_REGISTRATION> courseRegistrationList = new List<STUDENT_COURSE_REGISTRATION>();
            STUDENT_COURSE_REGISTRATION_DETAIL courseRegistrationDetail = new STUDENT_COURSE_REGISTRATION_DETAIL();

            List<PaymentReport> PaymentReportList = new List<PaymentReport>();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();


            try
            {
                if (session != null && semester != null)
                {
                    courseRegistrationList = GetEntitiesBy(p => p.Session_Id == session.Id);
                    //List<Payment> payments = GetModelsBy(p => p.Fee_Type_Id == 3 && p.Session_Id == 1);
                    foreach (STUDENT_COURSE_REGISTRATION courseRegistration in courseRegistrationList)
                    {
                        int studentNumber = 0;

                        courseRegistrationDetail = courseRegistrationDetailLogic.GetEntitiesBy(p => p.Semester_Id == semester.Id && p.Student_Course_Registration_Id == courseRegistration.Student_Course_Registration_Id).FirstOrDefault();
                        if (courseRegistrationDetail != null)
                        {
                            PaymentReport paymentReport = new PaymentReport();
                            paymentReport.Department = courseRegistration.DEPARTMENT.Department_Name;
                            if (courseRegistration.Level_Id == 1 && courseRegistration.Programme_Id == 2)
                            {
                                paymentReport.Programme = "PART TIME 1";

                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else if (courseRegistration.Level_Id == 2 && courseRegistration.Programme_Id == 2)
                            {
                                paymentReport.Programme = "PART TIME 2";
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else
                            {
                                paymentReport.Programme = courseRegistration.LEVEL.Level_Name;
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }


                        }

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PaymentReportList;
        }
        public List<ResultFormat> GetDownloadResultFormats(CourseAllocation courseAllocation)
        {
            try
            {
                var resultFormatData = from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Level_Id == courseAllocation.Level.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id)
                                           select new ResultFormat
                                           {
                                               MATRICNO = a.Matric_Number,
                                               QU1 = 0,
                                               QU2 = 0,
                                               QU3 = 0,
                                               QU4 = 0,
                                               QU5 = 0,
                                               QU6 = 0,
                                               QU7 = 0,
                                               QU8 = 0,
                                               QU9 = 0,
                                               T_EX = 0,
                                               T_CA = 0,
                                               EX_CA = 0,
                                           };
                List<ResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                return resultFormatList;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public List<NewResultFormat> GetDownloadResultNewFormats(CourseAllocation courseAllocation)
        {
            try
            {
                var resultFormatData = from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Level_Id == courseAllocation.Level.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id)
                                       select new NewResultFormat
                                       {
                                           MATRICNO = a.Matric_Number,
                                           T_EX = 0,
                                           T_CA = 0,
                                           //EX_CA = 0,
                                       };
                List<NewResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                return resultFormatList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<GeneralResultFormat> GetDownloadResultFormatsForDeparment(Department department, Programme programme, Session session, Semester semester, Level level)
        {

            try
            {
                var resultFormatData = from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Department_Id == department.Id && a.Level_Id == level.Id && a.Session_Id == session.Id && a.Programme_Id == programme.Id)
                                       select new GeneralResultFormat
                                       {
                                           MATRICNO = a.Matric_Number,
                                           C1=0,
                                           C2=0,
                                           C3=0,
                                           C4=0,
                                           C5=0,
                                           C6 = 0,
                                           C7 = 0,
                                           C8 = 0,
                                           C9 = 0,
                                           C10 = 0,
                                           C11 = 0,
                                           C12 = 0,
                                           C13 = 0,
                                           C14 = 0,
                                           C15 = 0,
                                           C16 = 0,
                                           C17 = 0,
                                           C18 = 0,
                                           C19 = 0,
                                           C20 = 0,
                                           C21 = 0,
                                           C22 = 0,
                                           C23 = 0,
                                           C24 = 0


                                       };
                List<GeneralResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                return resultFormatList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<ResultFormat> GetDownloadResultFormats(CourseAllocation courseAllocation, int courseModeId)
        {
            try
            {
                if (courseModeId != 4)
                {
                    List<ResultFormat> resultFormatData = new List<ResultFormat>();

                    if (courseModeId == 3)
                    {
                        resultFormatData = (from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id)
                                               select new ResultFormat
                                               {
                                                   MATRICNO = a.Matric_Number,
                                                   QU1 = 0,
                                                   QU2 = 0,
                                                   QU3 = 0,
                                                   QU4 = 0,
                                                   QU5 = 0,
                                                   QU6 = 0,
                                                   QU7 = 0,
                                                   QU8 = 0,
                                                   QU9 = 0,
                                                   T_EX = 0,
                                                   T_CA = 0,
                                                   EX_CA = 0,
                                               }).ToList();

                        List<ResultFormat> resultFormatList = new List<ResultFormat>();

                        for (int i = 0; i < resultFormatData.Count; i++)
                        {
                            if (resultFormatData[i].MATRICNO.Contains("/15/") || resultFormatData[i].MATRICNO.Contains("/16/"))
                            {
                                //Do Nothing
                            }
                            else
                            {
                                resultFormatList.Add(resultFormatData[i]);
                            }
                        }

                        return resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    }
                    else if (courseModeId == 2)
                    {
                        resultFormatData = (from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id && a.Course_Mode_Id == courseModeId)
                                               select new ResultFormat
                                               {
                                                   MATRICNO = a.Matric_Number,
                                                   QU1 = 0,
                                                   QU2 = 0,
                                                   QU3 = 0,
                                                   QU4 = 0,
                                                   QU5 = 0,
                                                   QU6 = 0,
                                                   QU7 = 0,
                                                   QU8 = 0,
                                                   QU9 = 0,
                                                   T_EX = 0,
                                                   T_CA = 0,
                                                   EX_CA = 0,
                                               }).ToList();

                        List<ResultFormat> resultFormatList = new List<ResultFormat>();

                        for (int i = 0; i < resultFormatData.Count; i++)
                        {
                            if (resultFormatData[i].MATRICNO.Contains("/15/") || resultFormatData[i].MATRICNO.Contains("/16/"))
                            {
                                resultFormatList.Add(resultFormatData[i]);
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }

                        return resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    }
                    else
                    {
                        resultFormatData = (from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id && a.Course_Mode_Id == courseModeId)
                                            select new ResultFormat
                                            {
                                                MATRICNO = a.Matric_Number,
                                                QU1 = 0,
                                                QU2 = 0,
                                                QU3 = 0,
                                                QU4 = 0,
                                                QU5 = 0,
                                                QU6 = 0,
                                                QU7 = 0,
                                                QU8 = 0,
                                                QU9 = 0,
                                                T_EX = 0,
                                                T_CA = 0,
                                                EX_CA = 0,
                                            }).ToList();

                        List<ResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                        return resultFormatList;
                    }
                    
                }
                else
                {
                    List<ResultFormat> resultFormatList = new List<ResultFormat>();
                    ResultFormat sampleFormat = new ResultFormat();
                    sampleFormat.MATRICNO = "N/XX/15/12345";
                    resultFormatList.Add(sampleFormat);

                    return resultFormatList;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<CBTResultFormat> GetCBTDownloadResultFormats(CourseAllocation courseAllocation, int courseModeId)
        {
            try
            {
                List<CBTResultFormat> resultFormatData = new List<CBTResultFormat>();
                if (courseModeId == 0)
                {
                    resultFormatData = (from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id)
                                        select new CBTResultFormat
                                        {
                                            MATRICNO = a.Matric_Number,
                                            STUDENTNAME = a.Name,
                                            SCORE = 0
                                        }).ToList();
                }
                else
                {
                    resultFormatData = (from a in repository.GetBy<VW_REGISTERED_COURSES>(a => a.Course_Id == courseAllocation.Course.Id && a.Department_Id == courseAllocation.Department.Id && a.Level_Id == courseAllocation.Level.Id && a.Session_Id == courseAllocation.Session.Id && a.Programme_Id == courseAllocation.Programme.Id && a.Course_Mode_Id == courseModeId)
                                        select new CBTResultFormat
                                        {
                                            MATRICNO = a.Matric_Number,
                                            STUDENTNAME = a.Name,
                                            SCORE = 0
                                        }).ToList();
                }
                    

                List<CBTResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MATRICNO).ToList();
                return resultFormatList;
               

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<RegisteredCoursesReport> GetCourseRegistrationSummary(Department department, Programme programme, Level level, Session session, Semester semester)
        {
            try
            {
                var RegisteredCoursesReportList = from a in repository.GetBy<VW_STUDENT_REGISTERED_COURSES_REPORT>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Level_Id == level.Id && a.Semester_Id == semester.Id).OrderBy(p => p.Matric_Number)
                                                  select new RegisteredCoursesReport
                                                  {
                                                      Fullname = a.Name.ToUpper(),
                                                      SessionName = a.SESSION_NAME,
                                                      DepartmentName = a.DEPARTMENT_NAME,
                                                      ProgrammeName = a.PROGRAMME_NAME,
                                                      MatricNumber = a.Matric_Number,
                                                      LevelName = a.LEVEL_NAME,
                                                      SemesterName = a.SEMESTER_NAME,
                                                      CoursesRegistered = a.COURSES_REGISTERED,
                                                  };
                return RegisteredCoursesReportList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CourseRegistration> GetUnregisteredStudents(Session session, Programme programme, Department department, Level level)
        {

            try
            {

                List<CourseRegistration> courseRegistrations = (from a in repository.GetBy<VW_STUDENTS_PAID_BUT_UNREGISTERED>(a => a.Department_Id == department.Id && a.Programme_Id == programme.Id && a.Level_Id == level.Id && a.Session_Id == session.Id)
                                                                select new CourseRegistration
                                                                {
                                                                    Student = new Student() { Id = a.Person_Id, MatricNumber = a.Matric_Number, FullName = a.Name},
                                                                    Level = new Level() { Id = a.Level_Id },
                                                                    Programme = new Programme() { Id = a.Programme_Id },
                                                                    Department = new Department() { Id = a.Department_Id },
                                                                    Session = new Session() { Id = (int)a.Session_Id },
                                                                    Name = a.Name
                                                                }).ToList();

                return courseRegistrations;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<CourseRegistrationSummary> GetSummary(Session session, Semester semester)
        {
            try
            {
                List<CourseRegistrationSummary> courseRegistrationSummary = (from a in repository.GetBy<VW_COURSE_REGISTRATION_SUMMARY>(a => a.Session_Id == session.Id && a.Semester_Id==semester.Id)
                                                           select new CourseRegistrationSummary
                                                           {
                                                               Programme_Id = a.Programme_Id,
                                                               Programme_Name = a.Programme_Name,
                                                               Department_Name = a.Department_Name,
                                                               Session_Name = a.Session_Name,
                                                               RegistrationCount = (int)a.NUMBER_REGISTERED,
                                                               Semester_Name=a.Semester_Name
                                                           }).ToList();

                return courseRegistrationSummary;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }


}
