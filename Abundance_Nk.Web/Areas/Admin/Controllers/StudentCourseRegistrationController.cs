using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StudentCourseRegistrationController : BaseController
    {
        public const string ID = "Id";
        public const string NAME = "Name";
        // GET: Admin/StudentCourseRegistration
        public ActionResult RegisterCourse()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
                return View();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }

        public void RetainDropdownState(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                SemesterLogic semesterLogic = new SemesterLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                if (viewModel != null)
                {
                    if (viewModel.Session != null)
                    {

                        ViewBag.Session = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true), ID, NAME,
                            viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                    }
                    if (viewModel.Semester != null)
                    {
                        ViewBag.Semester = new SelectList(semesterLogic.GetAll(), ID, NAME, viewModel.Semester.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                    }
                    if (viewModel.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true), ID,
                            NAME, viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }
                    if (viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME,
                            viewModel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                    }
                    if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0 && viewModel.Programme != null && viewModel.Department != null)
                    {
                        DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                        ViewBag.DepartmentOptions = new SelectList(departmentOptionLogic.GetBy(viewModel.Department, viewModel.Programme), ID, NAME, viewModel.DepartmentOption.Id);
                    }
                    else
                    {
                        ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
                    }
                    if (viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                    }
                    if (viewModel.Course != null && viewModel.Level != null && viewModel.Semester != null &&
                        viewModel.Department != null)
                    {
                        List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level,
                            viewModel.Department, viewModel.Semester);
                        ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult RegisterCourse(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                List<CourseRegistration> courseRegistrationListCount = new List<CourseRegistration>();
                int courseRegDetailCheckCount = 0;
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<StudentLevel> studentLevelList = new List<StudentLevel>();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseLogic courseLogic = new CourseLogic();
                DepartmentOptionLogic optionLogic = new DepartmentOptionLogic();

                string operation = "INSERT";
                string action = "ADMIN :REGISTRATION FROM ADMIN CONSOLE (StudentCourseRegistration)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                CourseMode courseMode = new CourseMode() {Id = 1};
                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    studentLevelList = studentLevelLogic.GetModelsBy(p => p.Department_Option_Id == viewModel.DepartmentOption.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);
                
                }
                else
                {
                    studentLevelList = studentLevelLogic.GetModelsBy(p => p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);
                
                }

                List<DepartmentOption> options = optionLogic.GetBy(viewModel.Department, viewModel.Programme);
                if (options != null && options.Count > 0 && (viewModel.DepartmentOption == null || viewModel.DepartmentOption.Id <= 0) && (viewModel.Programme.Id == (int)Programmes.HNDFullTime))
                {
                    SetMessage("Please select department option.", Message.Category.Error);
                    RetainDropdownState(viewModel);
                    return View(viewModel);
                }

                foreach (StudentLevel studentLevel in studentLevelList)
                {
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == studentLevel.Student.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                p.Department_Id == studentLevel.Department.Id && p.Level_Id == studentLevel.Level.Id &&
                                p.Session_Id == studentLevel.Session.Id);
                    if (courseRegistrationList != null && courseRegistrationList.Count() > 0)
                    {
                        foreach (CourseRegistration item in courseRegistrationList)
                        {
                            CourseRegistrationDetail courseRegistrationDetailCheck = new CourseRegistrationDetail();
                            courseRegistrationDetailCheck =
                                courseRegistrationDetailLogic.GetModelBy(
                                    p =>
                                        p.Student_Course_Registration_Id == item.Id &&
                                        p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id &&
                                        p.Course_Mode_Id == 1);
                            if (courseRegistrationDetailCheck != null)
                            {
                                courseRegDetailCheckCount += 1;
                            }
                        }
                    }

                    viewModel.Course = courseLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id);

                    if (courseRegDetailCheckCount == 0)
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            CourseRegistration courseRegistration = new CourseRegistration();
                            courseRegistration.Student = studentLevel.Student;
                            courseRegistration.Session = studentLevel.Session;
                            courseRegistration.Programme = studentLevel.Programme;
                            courseRegistration.Department = studentLevel.Department;
                            courseRegistration.Level = studentLevel.Level;
                            courseRegistration = courseRegistrationLogic.CreateCourseRegistration(courseRegistration);
                            courseRegistrationDetail.CourseRegistration = courseRegistration;
                            courseRegistrationDetail.Course = viewModel.Course;
                            courseRegistrationDetail.Mode = courseMode;
                            courseRegistrationDetail.Semester = viewModel.Semester;
                            courseRegistrationDetail.CourseUnit = viewModel.Course.Unit;
                            courseRegistrationDetail = courseRegistrationDetailLogic.Create(courseRegistrationDetail, courseRegistrationDetailAudit);
                            courseRegistrationListCount.Add(courseRegistration);
                            transaction.Complete();
                        }
                    }
                    courseRegDetailCheckCount = 0;
                }
                TempData["Action"] = courseRegistrationListCount.Count + " Students registered  successfully";
                return RedirectToAction("RegisterCourse", new {controller = "StudentCourseRegistration", area = "Admin"});
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }
        }

        public ActionResult UnRegisterCourse()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
                return View();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }

        [HttpPost]
        public ActionResult UnRegisterCourse(StudentCourseRegistrationViewModel viewModel)
        {

            try
            {
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                int courseRegistrationDeleteCount = 0;
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<StudentLevel> studentLevelList = new List<StudentLevel>();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseLogic courseLogic = new CourseLogic();


                string operation = "MODIFY";
                string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (CoursesController)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    studentLevelList = studentLevelLogic.GetModelsBy(p => p.Department_Option_Id == viewModel.DepartmentOption.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);

                }
                else
                {
                    studentLevelList = studentLevelLogic.GetModelsBy(p => p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);

                }

                viewModel.Course = courseLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id);

                foreach (StudentLevel studentLevel in studentLevelList)
                {
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    courseRegistrationList = courseRegistrationLogic.GetModelsBy( p =>
                                                                                        p.Person_Id == studentLevel.Student.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                                                                        p.Department_Id == studentLevel.Department.Id && p.Level_Id == studentLevel.Level.Id &&
                                                                                        p.Session_Id == studentLevel.Session.Id);

                    if (courseRegistrationList != null && courseRegistrationList.Count() > 0)
                    {
                        foreach (CourseRegistration item in courseRegistrationList)
                        {
                            //courseRegistrationDetailCheckList = courseRegistrationDetailLogic.GetModelsBy(p => p.Student_Course_Registration_Id == item.Id && p.Test_Score == 0.00M && p.Exam_Score == 0.00M);
                            CourseRegistrationDetail courseRegistrationDetailCheck = courseRegistrationDetailLogic.GetModelsBy(p => p.Student_Course_Registration_Id == item.Id && (p.Course_Id == viewModel.Course.Id || p.COURSE.Course_Code == viewModel.Course.Code) && p.Semester_Id == viewModel.Semester.Id && p.Course_Mode_Id == (int)CourseModes.FirstAttempt).LastOrDefault();

                            using (TransactionScope scope = new TransactionScope())
                            {
                                if (courseRegistrationDetailCheck != null)
                                {
                                    courseRegistrationDetailAudit.Course = courseRegistrationDetailCheck.Course;
                                    courseRegistrationDetailAudit.CourseRegistration = item;
                                    courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetailCheck.CourseUnit;
                                    courseRegistrationDetailAudit.Mode = new CourseMode(){Id = (int)CourseModes.FirstAttempt};
                                    courseRegistrationDetailAudit.Semester = viewModel.Semester;
                                    courseRegistrationDetailAudit.Time = DateTime.Now;
                                    courseRegistrationDetailAudit.ExamScore = courseRegistrationDetailCheck.ExamScore;
                                    courseRegistrationDetailAudit.TestScore = courseRegistrationDetailCheck.TestScore;
                                    courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetailCheck.SpecialCase;

                                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                                    //bool isCourseRegistrationDetailDeleted = courseRegistrationDetailLogic.Delete( p => p.Student_Course_Registration_Detail_Id == courseRegistrationDetailCheck.Id);
                                    bool isCourseRegistrationDetailDeleted = courseRegistrationDetailLogic.Delete(p => p.Student_Course_Registration_Id == item.Id && (p.Course_Id == viewModel.Course.Id || p.COURSE.Course_Code == viewModel.Course.Code) && p.Semester_Id == viewModel.Semester.Id && p.Course_Mode_Id == (int)CourseModes.FirstAttempt);

                                    if (isCourseRegistrationDetailDeleted)
                                    {
                                        courseRegistrationDeleteCount += 1;
                                    }

                                    scope.Complete();
                                }
                            }
                        }
                    }
                }

                TempData["Action"] = courseRegistrationDeleteCount + " Students unregistered  successfully";
                return RedirectToAction("UnRegisterCourse", new {controller = "StudentCourseRegistration", area = "Admin"});
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }

        public ActionResult AddExtraCourse()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddExtraCourse(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    CourseLogic courseLogic = new CourseLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                    List<Model.Model.Student> students =
                        studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1)
                    {
                        SetMessage("Duplicate Matric Number OR Matric Number does not exist!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View(viewModel);
                    }

                    Model.Model.Student student = students.FirstOrDefault();
                    List<StudentLevel> studentLevels =
                        studentLevelLogic.GetModelsBy(
                            sl =>
                                sl.Person_Id == student.Id && sl.Department_Id == viewModel.Department.Id &&
                                sl.Programme_Id == viewModel.Programme.Id);
                    if (studentLevels.Count <= 0)
                    {
                        SetMessage("Student is not in this Programme, Department!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View(viewModel);
                    }

                    viewModel.Courses =
                        courseLogic.GetModelsBy(c => c.Department_Id == viewModel.Department.Id && c.Activated == true);
                    viewModel.Courses.OrderBy(c => c.Level);

                    RetainDropdownState(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveAddedCourse(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseMode carryOverCourseMode = new CourseMode() {Id = 2};
                    CourseMode firstAttemprCourseMode = new CourseMode() {Id = 1};

                    List<StudentLevel> studentLevelList = new List<StudentLevel>();
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();

                    List<Model.Model.Student> students =
                        studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1)
                    {
                        SetMessage("Duplicate Matric Number OR Matric Number does not exist!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    Model.Model.Student student = students.FirstOrDefault();
                    studentLevelList =
                        studentLevelLogic.GetModelsBy(
                            p =>
                                p.Person_Id == student.Id && p.Department_Id == viewModel.Department.Id &&
                                p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id &&
                                p.Level_Id == viewModel.Level.Id);
                    StudentLevel studentLevel = studentLevelList.LastOrDefault();
                    if (studentLevelList.Count == 0)
                    {
                        SetMessage("Student has not been registered in this level for this session!",
                            Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == studentLevel.Student.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                p.Department_Id == studentLevel.Department.Id && p.Level_Id == studentLevel.Level.Id &&
                                p.Session_Id == studentLevel.Session.Id);

                    if (courseRegistrationList.Count() != 1)
                    {
                        SetMessage("Student has not registered course for this session!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    CourseRegistration courseRegistration = courseRegistrationList.FirstOrDefault();

                    for (int i = 0; i < viewModel.Courses.Count; i++)
                    {
                        long courseId = viewModel.Courses[i].Id;
                        CourseRegistrationDetail courseRegistrationDetailCheck =
                            courseRegistrationDetailLogic.GetModelsBy(
                                crd =>
                                    crd.Course_Id == courseId &&
                                    crd.Student_Course_Registration_Id == courseRegistration.Id).LastOrDefault();
                        if (courseRegistrationDetailCheck == null)
                        {
                            if (viewModel.Courses[i].IsRegistered || viewModel.Courses[i].isCarryOverCourse)
                            {
                                courseRegistrationDetail.CourseRegistration = courseRegistration;
                                courseRegistrationDetail.Course = viewModel.Courses[i];
                                if (viewModel.Courses[i].isCarryOverCourse)
                                {
                                    courseRegistrationDetail.Mode = carryOverCourseMode;
                                }
                                else
                                {
                                    courseRegistrationDetail.Mode = firstAttemprCourseMode;
                                }

                                courseRegistrationDetail.Semester = viewModel.Semester;
                                courseRegistrationDetail.CourseUnit = viewModel.Courses[i].Unit;
                                courseRegistrationDetail = courseRegistrationDetailLogic.Create(courseRegistrationDetail);

                            }
                        }


                    }

                    SetMessage("Operation Successful!", Message.Category.Information);
                    return RedirectToAction("AddExtraCourse");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            return View("AddExtraCourse", viewModel);
        }

        public JsonResult GetDepartments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() {Id = Convert.ToInt32(id)};
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetDepartmentOptions(string ProgId, string DeptId)
        {
            try
            {
                if (string.IsNullOrEmpty(ProgId) || string.IsNullOrEmpty(DeptId))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(ProgId) };
                Department department = new Department() { Id = Convert.ToInt32(DeptId) };
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = new List<DepartmentOption>();

                departmentOptions = departmentOptionLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetSemester(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Session session = new Session() {Id = Convert.ToInt32(id)};
                SemesterLogic semesterLogic = new SemesterLogic();
                List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                List<Semester> semesters = new List<Semester>();
                foreach (SessionSemester sessionSemester in sessionSemesterList)
                {
                    semesters.Add(sessionSemester.Semester);
                }

                return Json(new SelectList(semesters, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetCourses(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    return null;
                }

                DepartmentOption departmentOption = null;
                Level level = new Level() {Id = Convert.ToInt32(ids[1])};
                Department department = new Department() {Id = Convert.ToInt32(ids[0])};
                Semester semester = new Semester() {Id = Convert.ToInt32(ids[2])};
                if (ids.Count() >= 4)
                {
                    departmentOption = new DepartmentOption() { Id = Convert.ToInt32(ids[3]) };
                }
                
                List<Course> courseList = Utility.GetCoursesByOptionLevelDepartmentAndSemester(departmentOption, level, department, semester);

                for (int i = 0; i < courseList.Count; i++)
                {
                    courseList[i].Name = courseList[i].Name + ", " + courseList[i].Code;
                }

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RegisterAll()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View(viewModel);
            }

        }

        [HttpPost]
        public ActionResult RegisterAll(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                viewModel.CourseRegistrations = courseRegistrationLogic.GetUnregisteredStudents(viewModel.Session,
                    viewModel.Programme, viewModel.Department, viewModel.Level);
                TempData["viewModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveAllRegisteredStudents()
        {
            StudentCourseRegistrationViewModel viewModel = (StudentCourseRegistrationViewModel) TempData["viewModel"];
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                List<CourseRegistration> courseRegistrations = viewModel.CourseRegistrations;

                if (courseRegistrations != null && courseRegistrations.Count > 0)
                {
                    List<Course> SemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level,
                        viewModel.Semester);

                    List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

                    foreach (Course SemesterCourse in SemesterCourses)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                        courseRegistrationDetail.Course = SemesterCourse;
                        courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                        courseRegistrationDetail.Mode = new CourseMode() {Id = 1};
                        courseRegistrationDetail.Semester = viewModel.Semester;
                        courseRegistrationDetails.Add(courseRegistrationDetail);
                    }

                    string operation = "INSERT";
                    string action = "REGISTRATION :REGISTER ALL - ADMIN";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    UserLogic loggeduser = new UserLogic();
                    courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                    if (courseRegistrationDetails.Count > 0)
                    {
                        foreach (CourseRegistration courseRegistration in courseRegistrations)
                        {
                            CourseRegistration registeredCourse = new CourseRegistration();
                            registeredCourse = courseRegistrationLogic.GetBy(courseRegistration.Student, courseRegistration.Level, courseRegistration.Programme, courseRegistration.Department,
                                courseRegistration.Session);
                            if (registeredCourse == null)
                            {
                                courseRegistration.Details = courseRegistrationDetails;
                                registeredCourse = courseRegistrationLogic.Create(courseRegistration, courseRegistrationDetailAudit);

                            }
                        }
                    }

                    SetMessage("Operation Succesful!", Message.Category.Information);
                }

                RetainDropdownState(viewModel);
                return RedirectToAction("RegisterAll");
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return RedirectToAction("RegisterAll");
            }

        }

        public ActionResult StudentsToRegister()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StudentsToRegister(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    List<StudentLevel> studentLevelList = new List<StudentLevel>();

                    List<CourseRegistration> courseRegistrations =
                        courseRegistrationLogic.GetModelsBy(
                            s =>
                                s.Session_Id == viewModel.Session.Id && s.Department_Id == viewModel.Department.Id &&
                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id);

                    if (courseRegistrations.Count > 0)
                    {
                        courseRegistrations = CheckIfAllCoursesRegistered(courseRegistrations, viewModel);
                    }

                    List<StudentLevel> studentLevels =
                        studentLevelLogic.GetModelsBy(
                            s =>
                                s.Session_Id == viewModel.Session.Id && s.Department_Id == viewModel.Department.Id &&
                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id);

                    List<long> courseRegPersonIdList = courseRegistrations.Select(c => c.Student.Id).ToList();

                    for (int i = 0; i < studentLevels.Count; i++)
                    {
                        if (!courseRegPersonIdList.Contains(studentLevels[i].Student.Id))
                        {
                            string matricNumber = studentLevels[i].Student.MatricNumber;
                            string session = studentLevels[i].Session.Name;
                            string[] splitRegNumber = matricNumber.Split('/');
                            string matricYearNumber = splitRegNumber[2];
                            string[] sessionSplit = session.Split('/');
                            string sessionYear = sessionSplit[0].Substring(2, 2);
                            string prevSessionYear = (Convert.ToInt32(sessionYear) - 1).ToString();
                            string[] matricNumberYearsToPull = { sessionYear, prevSessionYear };
                            if (matricNumberYearsToPull.Contains(matricYearNumber))
                            {
                                studentLevelList.Add(studentLevels[i]);
                            }

                            //studentLevelList.Add(studentLevels[i]);
                        }
                    }

                    viewModel.StudentLevelList = studentLevelList;
                    TempData["viewModel"] = viewModel;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public List<CourseRegistration> CheckIfAllCoursesRegistered(List<CourseRegistration> courseRegistrations, StudentCourseRegistrationViewModel viewModel)
        {
            List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                Semester firstSemester = new Semester() { Id = 1 };
                Semester secondSemester = new Semester() { Id = 2 };

                List<Course> firstSemesterCourses = new List<Course>();
                List<Course> secondSemesterCourses = new List<Course>();
                
                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    firstSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.DepartmentOption, viewModel.Level, firstSemester, true);
                    secondSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.DepartmentOption, viewModel.Level, secondSemester, true);
                }
                else
                {
                    firstSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level, firstSemester, true);
                    secondSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level, secondSemester, true);
                }

                List<Course> courses = firstSemesterCourses;
                courses.AddRange(secondSemesterCourses);

                for (int i = 0; i < courseRegistrations.Count; i++)
                {
                    CourseRegistration currentCourseRegistration = courseRegistrations[i];

                    bool removeStatus = false;

                    List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(c => c.Student_Course_Registration_Id == currentCourseRegistration.Id);
                    for (int j = 0; j < courseRegistrationDetails.Count; j++)
                    {
                        if (!courses.Contains(courseRegistrationDetails[j].Course))
                        {
                            removeStatus = true;
                        }
                    }

                    if (!removeStatus)
                    {
                        courseRegistrationList.Add(currentCourseRegistration);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return courseRegistrationList;
        }

        public ActionResult GetPayments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                StudentCourseRegistrationViewModel viewModel =
                    (StudentCourseRegistrationViewModel)TempData["viewModel"];
                TempData.Keep("viewModel");
                long personId = Convert.ToInt64(id);

                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                List<Payment> paymentList = new List<Payment>();

                List<Payment> payments = paymentLogic.GetModelsBy(p => p.Person_Id == personId);

                for (int i = 0; i < payments.Count; i++)
                {
                    Payment currentPayment = payments[i];
                    PaymentEtranzact paymentEtranzact =
                        paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == currentPayment.Id);
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == currentPayment.Id);
                    if (paymentEtranzact != null)
                    {
                        paymentList.Add(currentPayment);
                    }
                    if (remitaPayment != null)
                    {
                        paymentList.Add(currentPayment);
                    }
                }

                viewModel.Payments = paymentList;
                return PartialView("_StudentPayment", viewModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult RegisteStudent(string id)
        {
            StudentCourseRegistrationViewModel viewModel = (StudentCourseRegistrationViewModel)TempData["viewModel"];
            TempData.Keep("viewModel");
            try
            {
                long personId = Convert.ToInt64(id);
                Model.Model.Student student = new Model.Model.Student() { Id = personId };

                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                Semester firstSemester = new Semester() { Id = 1 };
                Semester secondSemester = new Semester() { Id = 2 };

                List<Course> firstSemesterCourses = new List<Course>();
                List<Course> secondSemesterCourses = new List<Course>();

                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    firstSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.DepartmentOption, viewModel.Level, firstSemester, true);
                    secondSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.DepartmentOption, viewModel.Level, secondSemester, true);
                }
                else
                {
                    firstSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level, firstSemester, true);
                    secondSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level, secondSemester, true);
                }

                List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

                foreach (Course SemesterCourse in firstSemesterCourses)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    courseRegistrationDetail.Course = SemesterCourse;
                    courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                    courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                    courseRegistrationDetail.Semester = firstSemester;
                    courseRegistrationDetails.Add(courseRegistrationDetail);
                }

                foreach (Course SemesterCourse in secondSemesterCourses)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    courseRegistrationDetail.Course = SemesterCourse;
                    courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                    courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                    courseRegistrationDetail.Semester = secondSemester;
                    courseRegistrationDetails.Add(courseRegistrationDetail);
                }

                List<Course> courses = firstSemesterCourses;
                courses.AddRange(secondSemesterCourses);

                if (courseRegistrationDetails.Count > 0)
                {
                    CourseRegistration registeredCourse = new CourseRegistration();
                    registeredCourse = courseRegistrationLogic.GetBy(student, viewModel.Level, viewModel.Programme, viewModel.Department, viewModel.Session);
                    if (registeredCourse == null)
                    {
                        registeredCourse = new CourseRegistration();
                        registeredCourse.Student = student;
                        registeredCourse.Department = viewModel.Department;
                        registeredCourse.Details = courseRegistrationDetails;
                        registeredCourse.Level = viewModel.Level;
                        registeredCourse.Programme = viewModel.Programme;
                        registeredCourse.Session = viewModel.Session;
                        courseRegistrationLogic.Create(registeredCourse);
                    }
                    else
                    {
                        for (int i = 0; i < courses.Count; i++)
                        {
                            Course currentCourse = courses[i];
                            CourseRegistrationDetail detail = courseRegistrationDetailLogic.GetModelsBy(c => c.Student_Course_Registration_Id == registeredCourse.Id && c.Course_Id == currentCourse.Id).LastOrDefault();
                            if (detail == null)
                            {
                                CourseRegistrationDetail newDetail = new CourseRegistrationDetail();
                                newDetail.Course = currentCourse;
                                newDetail.CourseRegistration = registeredCourse;
                                newDetail.CourseUnit = currentCourse.Unit;
                                newDetail.Mode = new CourseMode(){Id = (int)CourseModes.FirstAttempt};
                                newDetail.Semester = currentCourse.Semester;
                                newDetail.SpecialCase = null;
                                
                                courseRegistrationDetailLogic.Create(newDetail);
                            }
                        }
                    }
                }

                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult StudentDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StudentDetails(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                List<Model.Model.Student> students = studentLogic.GetModelsBy(x => x.Matric_Number == viewModel.Student.MatricNumber || x.APPLICATION_FORM.Application_Form_Number == viewModel.Student.MatricNumber);
                Model.Model.Student myStudent = new Model.Model.Student();
                if (students.Count > 1)
                {
                    SetMessage("Matric Number is duplicate", Message.Category.Error);
                    return View();
                }
                if (students.Count == 0)
                {
                    Model.Model.Student appliedStudent = studentLogic.GetModelsBy(s => s.APPLICATION_FORM.Application_Form_Number == viewModel.Student.MatricNumber).LastOrDefault();
                    if (appliedStudent == null)
                    {
                        SetMessage("No record found", Message.Category.Error);
                        return View();
                    }
                    else
                    {
                        myStudent = appliedStudent;
                    }
                }

                if (students.Count == 1)
                {
                    myStudent = students.FirstOrDefault();
                }

                List<CourseRegistration> courseRegistrationlist = courseRegistrationLogic.GetModelsBy(x => x.Person_Id == myStudent.Id);

                for (int i = 0; i < courseRegistrationlist.Count; i++)
                {
                    long courseRegId = courseRegistrationlist[i].Id;
                    List<CourseRegistrationDetail> courseRegistrationDetaillist = courseRegistrationDetailLogic.GetModelsBy(x => x.Student_Course_Registration_Id == courseRegId);
                    if (courseRegistrationDetaillist.Count != 0)
                    {
                        courseRegistrationlist[i].Details = courseRegistrationDetaillist.OrderBy(c => c.Semester.Id).ToList();
                    }
                }

                List<Payment> paymentList = paymentLogic.GetModelsBy(x => x.Person_Id == myStudent.Id);
                List<Payment> ConfirmedPayments = new List<Payment>();

                for (int i = 0; i < paymentList.Count; i++)
                {
                    long PaymentId = paymentList[i].Id;
                    Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == PaymentId);

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(x => x.Payment_Id == PaymentId);
                    if (paymentEtranzact != null)
                    {
                        payment.ConfirmationNumber = paymentEtranzact.ConfirmationNo;
                        payment.Amount = paymentEtranzact.TransactionAmount.ToString();
                        payment.DatePaid = paymentEtranzact.TransactionDate.Value;

                        ConfirmedPayments.Add(payment);
                    }
                    else
                    {
                        RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                        string[] remitaPaymentStatus = { "00", "00:", "01", "01:", "01:approved" };

                        RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == PaymentId && remitaPaymentStatus.Contains(r.Status));
                        if (remitaPayment != null)
                        {
                            payment.ConfirmationNumber = remitaPayment.RRR;
                            payment.Amount = remitaPayment.TransactionAmount.ToString();
                            payment.DatePaid = remitaPayment.TransactionDate;

                            ConfirmedPayments.Add(payment);
                        }
                    }
                }

                viewModel.StudentLevelList = studentLevelLogic.GetModelsBy(s => s.STUDENT.Person_Id == myStudent.Id);

                viewModel.Student = myStudent;
                viewModel.CourseRegistrations = courseRegistrationlist;
                viewModel.Payments = ConfirmedPayments;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentDetails");
        }
        public JsonResult DeleteStudentLevel(string id)
        {
            JsonPostResult result = new JsonPostResult();
            try
            {
                long studentLevelId = Convert.ToInt64(id);
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                studentLevelLogic.Delete(s => s.Student_Level_Id == studentLevelId);

                result.IsError = false;
                result.SuccessMessage = "Operation Successful!";

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "Error! " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RemoveCourse(string id)
        {
            JsonPostResult result = new JsonPostResult();
            try
            {
                string postBackResult = "";
                long courseRegdetailId = Convert.ToInt64(id);
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                UserLogic userLogic = new UserLogic();
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();

                //courseRegistrationDetailLogic.Delete(s => s.Student_Course_Registration_Detail_Id == courseRegdetailId);
                //postBackResult = "Success";

                CourseRegistrationDetail RegDetailCheck = courseRegistrationDetailLogic.GetModelBy(cr => cr.Student_Course_Registration_Detail_Id == courseRegdetailId);
                if ((RegDetailCheck.TestScore == null && RegDetailCheck.ExamScore == null) || (RegDetailCheck.TestScore + RegDetailCheck.ExamScore <= 0))
                {
                    postBackResult = "Success";
                }
                else
                {
                    postBackResult = "HasResult";
                }

                if (postBackResult == "HasResult" && User.Identity.Name == "william")
                {
                    postBackResult = "Success";
                }

                if (postBackResult == "Success")
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        CourseRegistrationDetailAudit courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                        string operation = "REMOVED COURSE REGISTRATION";
                        string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StudentCourseRegistrationController)";
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                        courseRegistrationDetailAudit.Action = action;
                        courseRegistrationDetailAudit.Operation = operation;
                        courseRegistrationDetailAudit.Client = client;
                        courseRegistrationDetailAudit.User = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                        courseRegistrationDetailAudit.Time = DateTime.Now;

                        courseRegistrationDetailAudit.Course = RegDetailCheck.Course;
                        courseRegistrationDetailAudit.CourseUnit = RegDetailCheck.CourseUnit;
                        courseRegistrationDetailAudit.Mode = RegDetailCheck.Mode;
                        courseRegistrationDetailAudit.Semester = RegDetailCheck.Semester;
                        courseRegistrationDetailAudit.TestScore = RegDetailCheck.TestScore;
                        courseRegistrationDetailAudit.ExamScore = RegDetailCheck.ExamScore;
                        courseRegistrationDetailAudit.SpecialCase = RegDetailCheck.SpecialCase;
                        courseRegistrationDetailAudit.CourseRegistrationDetail = RegDetailCheck;
                        courseRegistrationDetailAudit.Student = RegDetailCheck.CourseRegistration.Student;

                        courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                        courseRegistrationDetailLogic.Delete(s => s.Student_Course_Registration_Detail_Id == courseRegdetailId);

                        scope.Complete();
                    }
                    
                }

                if (postBackResult == "HasResult")
                {
                    //  Send "false"
                    result.IsError = true;
                    result.ErrorMessage = "Course was not removed because it has score.";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (postBackResult == "Success")
                {
                    //  Send "Success"
                    result.IsError = false;
                    result.SuccessMessage = "Course was removed!";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "Error! " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStudentLevel(int sid)
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (sid > 0)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelBy(sl => sl.Student_Level_Id == sid);

                    viewModel.StudentLevel = studentLevel;
                    ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME, studentLevel.Level.Id);
                    ViewBag.Session = viewModel.SessionSelectList;
                    ViewBag.Programme = viewModel.ProgrammeSelectList;
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(studentLevel.Programme);
                    ViewBag.Department = new SelectList(departments, "Id", "Name", studentLevel.Department.Id);

                    if (studentLevel.DepartmentOption != null)
                    {
                        DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                        List<DepartmentOption> options = departmentOptionLogic.GetBy(studentLevel.Department, studentLevel.Programme);

                        ViewBag.DepartmentOption = new SelectList(options, "Id", "Name", studentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");
                    } 
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditStudentLevel(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel != null)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelBy(sl => sl.Student_Level_Id == viewModel.StudentLevel.Id);

                    studentLevel.Session = viewModel.StudentLevel.Session;
                    studentLevel.Level = viewModel.StudentLevel.Level;
                    if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                    {
                        studentLevel.Programme = viewModel.StudentLevel.Programme; 
                    }
                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        studentLevel.Department = viewModel.StudentLevel.Department;
                    } 
                    if (viewModel.OptionId > 0)
                    {
                        DepartmentOption departmentOption = new DepartmentOption() { Id = viewModel.OptionId };
                        studentLevel.DepartmentOption = departmentOption;
                    }   

                    studentLevelLogic.Modify(studentLevel, studentLevel.Id);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentDetails");
        }

        public JsonResult AddStudentLevel(string Id)
        {
            JsonPostResult result = new JsonPostResult();
            long PersonId = Convert.ToInt64(Id);
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (PersonId > 0)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    var studentLevels = studentLevelLogic.GetModelsBy(sl => sl.Person_Id == PersonId);
                    if (studentLevels != null && studentLevels.Count > 0)
                    {
                        if (studentLevels.Count <= 1)
                        {
                            StudentLevel newStudentLevel = new StudentLevel();
                            newStudentLevel = studentLevels[0];
                            newStudentLevel.Session.Id = 1;
                            studentLevelLogic.Create(newStudentLevel);

                            result.IsError = false;
                            result.SuccessMessage = "Level Added!";

                          
                        }
                        else
                        {
                            result.IsError = false;
                            result.SuccessMessage = "Two levels already exist!";
                        }
                    }
                    
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "Error! " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult ViewConfirmedpayments()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
               
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewConfirmedpayments(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                    Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber).LastOrDefault();
                    if (student == null)
                    {
                        SetMessage("Error! Student does not exist", Message.Category.Error); 
                    }

                    viewModel.PaymentEtranzacts = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult EditConfirmedPayment(int pid)
        {   
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (pid > 0)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    viewModel.PaymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == pid);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditConfirmedPayment(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.PaymentEtranzact != null)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    viewModel.PaymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.ONLINE_PAYMENT.PAYMENT.Payment_Id == viewModel.PaymentEtranzact.Payment.Payment.Id);
                    viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(viewModel.Amount);

                    paymentEtranzactLogic.Modify(viewModel.PaymentEtranzact);
                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewConfirmedpayments");
        }
        public ActionResult EditCourseRegistration(int cid)
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (cid > 0)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                    CourseRegistration courseRegistration = courseRegistrationLogic.GetModelBy(c => c.Student_Course_Registration_Id == cid);
                    List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(c => c.Student_Course_Registration_Id == cid);

                    viewModel.CourseRegistration = courseRegistration;
                    viewModel.CourseRegistration.Details = courseRegistrationDetails;

                    ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME, courseRegistration.Level.Id);
                    ViewBag.Session = viewModel.SessionSelectList;
                    ViewBag.Programme = viewModel.ProgrammeSelectList;
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(courseRegistration.Programme);
                    ViewBag.Department = new SelectList(departments, "Id", "Name", courseRegistration.Department.Id);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditCourseRegistration(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.CourseRegistration != null)
                {
                    string operation = "MODIFY";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StudentController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                     UserLogic loggeduser = new UserLogic();
                    courseRegistrationDetailAudit.User = loggeduser.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();

                    //CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                 
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistration courseRegistration = courseRegistrationLogic.GetModelBy(c => c.Student_Course_Registration_Id == viewModel.CourseRegistration.Id);

                    courseRegistration.Session = viewModel.CourseRegistration.Session;
                    courseRegistration.Level = viewModel.CourseRegistration.Level;
                    courseRegistration.Programme = viewModel.CourseRegistration.Programme;
                    courseRegistration.Department = viewModel.CourseRegistration.Department;

                    for (int i = 0; i < viewModel.CourseRegistration.Details.Count; i++)
                    {
                        CourseMode firstAttempt = new CourseMode() {Id = 1};
                        CourseMode carryOver = new CourseMode() {Id = 2};
                        if (viewModel.CourseRegistration.Details[i].Mode.Name == "First Attempt")
                        {
                            viewModel.CourseRegistration.Details[i].Mode = firstAttempt;
                        }
                        else if (viewModel.CourseRegistration.Details[i].Mode.Name == "Carry Over")
                        {
                            viewModel.CourseRegistration.Details[i].Mode = carryOver;
                        }
                    }

                    courseRegistration.Details = viewModel.CourseRegistration.Details;

                    courseRegistrationLogic.ModifyRegOnly(courseRegistration, courseRegistrationDetailAudit);
                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentDetails");
        }
        public JsonResult DeleteCourseRegistration(string id)
        {
            JsonPostResult result = new JsonPostResult();

            try
            {
                long courseRegId = Convert.ToInt64(id);
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                UserLogic userLogic = new UserLogic();

                User user = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);

                CourseRegistrationDetailAudit courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                string operation = "REMOVED COURSE REGISTRATION";
                string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StudentCourseRegistrationController)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                courseRegistrationDetailAudit.User = user;
                courseRegistrationDetailAudit.Time = DateTime.Now;

                List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(c => c.Student_Course_Registration_Id == courseRegId);

                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        CourseRegistrationDetail courseReg = courseRegistrationDetails[i];

                        courseRegistrationDetailAudit.Course = new Course() { Id = courseRegistrationDetails[i].Course.Id };
                        courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetails[i].CourseUnit;
                        courseRegistrationDetailAudit.Mode = new CourseMode() { Id = courseRegistrationDetails[i].Mode.Id };
                        courseRegistrationDetailAudit.Semester = new Semester() { Id = courseRegistrationDetails[i].Semester.Id };
                        courseRegistrationDetailAudit.TestScore = courseRegistrationDetails[i].TestScore;
                        courseRegistrationDetailAudit.ExamScore = courseRegistrationDetails[i].ExamScore;
                        courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetails[i].SpecialCase;
                        courseRegistrationDetailAudit.Student = courseRegistrationDetails[i].CourseRegistration.Student;

                        courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                        courseRegistrationDetailLogic.Delete(c => c.Student_Course_Registration_Detail_Id == courseReg.Id);
                    }

                    courseRegistrationDetailAuditLogic.Delete(c => c.Student_Course_Registration_Id == courseRegId);

                    courseRegistrationLogic.Delete(c => c.Student_Course_Registration_Id == courseRegId);

                    scope.Complete();

                    result.IsError = false;
                    result.SuccessMessage = "Course Registration was removed successfully!";
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "Error! " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ManageDeferment()
        {
             StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                viewModel.StudentDeferementLog = new StudentDeferementLog();
                
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewModel);
        }

         [HttpPost]
         public ActionResult ManageDeferment(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                viewModel.StudentDeferementLog = new StudentDeferementLog();
                viewModel.StudentDeferementLog.Student = studentLogic.GetBy(viewModel.Student.MatricNumber);
                ViewBag.SessionId = viewModel.SessionSelectList;
                ViewBag.SemesterId = viewModel.SemesterSelectList;
            }
            catch (Exception)
            {
                throw;
            }
            return View(viewModel);

        }
        [HttpPost]
        public ActionResult AddDeferment(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                 StudentDefermentLogic studentDefermentLogic = new StudentDefermentLogic();
                if (viewModel.StudentDeferementLog.Semester.Id == 0)
                {
                    viewModel.StudentDeferementLog.Semester.Id = 1;
                }
               viewModel.StudentDeferementLog =  studentDefermentLogic.Create(viewModel.StudentDeferementLog);
                if (viewModel.StudentDeferementLog.Id > 0)
                {
                    SetMessage("Student was added to list",Message.Category.Information);
                }
                else
                {
                    SetMessage("Student is already on the list for the selected session",Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
              SetMessage("An Error occured while adding to the list! Please try again " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("ManageDeferment");
        }
    
         public ActionResult ViewDeferment()
        {
             StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                viewModel.StudentDeferementLog = new StudentDeferementLog();
                viewModel.StudentDeferementLog.Semester = new Semester();
                viewModel.StudentDeferementLog.Session = new Session();
                viewModel.StudentDeferementLogs = new List<StudentDeferementLog>();
                ViewBag.SessionId = viewModel.SessionSelectList;
                ViewBag.SemesterId = viewModel.SemesterSelectList;
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewDeferment(StudentCourseRegistrationViewModel viewModel)
        {
            
            try
            {
                StudentDefermentLogic studentDefermentLogic = new StudentDefermentLogic();
                if (viewModel.StudentDeferementLog.Semester.Id == 0)
                {
                    viewModel.StudentDeferementLog.Semester.Id = 1;
                }
               viewModel.StudentDeferementLogs =  studentDefermentLogic.GetModelsBy(a => a.Semester_Id == viewModel.StudentDeferementLog.Semester.Id && a.Session_Id == viewModel.StudentDeferementLog.Session.Id);
                
                ViewBag.SessionId = viewModel.SessionSelectList;
                ViewBag.SemesterId = viewModel.SemesterSelectList;
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewModel);
        }

        public JsonResult Delete(string Id)
        {
            try
            {
                 if (string.IsNullOrEmpty(Id))
                {
                    return null;
                }
                 StudentDefermentLogic studentDefermentLogic = new StudentDefermentLogic();
                studentDefermentLogic.Delete(Id);
           
                return Json("Deleted", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult DeactivateStudent(string studentMatricNumber, int activeMode)
        {
            JsonPostResult result = new JsonPostResult();
            try
            {
                if (!string.IsNullOrEmpty(studentMatricNumber))
                {
                    StudentLogic studentLogic = new StudentLogic();

                    Model.Model.Student student = studentLogic.GetModelBy(s => s.Matric_Number == studentMatricNumber || s.APPLICATION_FORM.Application_Form_Number == studentMatricNumber);
                    if (student != null)
                    {
                        student.Activated = activeMode == 0 ? false : true;

                        studentLogic.Modify(student);

                        result.IsError = false;
                        result.SuccessMessage = "Operation Successful!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result.IsError = true;
                        result.ErrorMessage = "Student not found!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    result.IsError = true;
                    result.ErrorMessage = "Paramter not set! ";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "Error! " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}