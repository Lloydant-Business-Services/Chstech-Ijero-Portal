using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class ElearningController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";

        private string FileUploadURL;

        private readonly CourseAllocationLogic courseAllocationLogic;
        private readonly ECourseLogic eCourseLogic;
        private readonly EAssignmentLogic eAssignmentLogic;
        private readonly EAssignmentSubmissionLogic eAssignmentSubmissionLogic;
        private readonly EContentTypeLogic eContentTypeLogic;
        private readonly EChatTopicLogic eChatTopicLogic;
        private readonly EChatResponseLogic eChatResponseLogic;
        private ELearningViewModel viewModel;
        public ElearningController()
        {
            courseAllocationLogic = new CourseAllocationLogic();
            eCourseLogic = new ECourseLogic();
            eAssignmentLogic = new EAssignmentLogic();
            eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();
            viewModel = new ELearningViewModel();
            eContentTypeLogic = new EContentTypeLogic();
            eChatTopicLogic = new EChatTopicLogic();
            eChatResponseLogic = new EChatResponseLogic();
        }

        public void KeepDropDownState(ELearningViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.ActiveSessionList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                //ViewBag.Level = new SelectList(viewModel.CourseAllocationLevel, ID, NAME);
                ViewBag.Level = viewModel.CourseAllocationLevel;
                ViewBag.User = viewModel.UserSelectList;

                if (viewModel.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.Active==true);

                    var semesters = new List<Semester>();
                    foreach (SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters, ID, NAME, viewModel.Semester.Id);
                }
                if (viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, ID, NAME, viewModel.Department.Id);
                }
                if (viewModel.Course != null && viewModel.Course.Id > 0)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Programme, viewModel.Level, viewModel.Department,
                        viewModel.Semester);

                    ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Course.Id);
                }
                if (viewModel.Semester?.Id > 0 && viewModel.Session?.Id > 0 && viewModel.Level?.Id > 0 && viewModel.CourseAllocationId > 0)
                {
                    var allocatedCourse = courseAllocationLogic.GetModelsBy(g => g.USER.User_Name == User.Identity.Name && g.Level_Id == viewModel.Level.Id && g.Semester_Id == viewModel.Semester.Id && g.Session_Id == viewModel.Session.Id)
                        .Select(m => new CourseAllocated
                        {
                            Id = m.Id,
                            Name = m.Course.Code + "-" + m.Course.Name + " - " + m.Programme.Name + " " + "(" + m.Department.Name + ")"
                        });
                    ViewBag.AllocatedCourse = new SelectList(allocatedCourse, ID, NAME, viewModel.CourseAllocationId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Admin/Elearning
        public ActionResult Index()
        {
            ViewBag.Session = viewModel.ActiveSessionList;
            ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
            //ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            ViewBag.Level = viewModel.CourseAllocationLevel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(ELearningViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name).OrderBy(f=>f.Level.Id).ToList();
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult ViewContent(string cid)
        {
            try
            {

                long courseId = Convert.ToInt64(Utility.Decrypt(cid));
                var courseAllocation = new CourseAllocation();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == courseId);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("Index");
                }

                viewModel.CourseAllocation = courseAllocation;
                viewModel.cid = courseId;
                viewModel.eCourse = new ECourse();
                viewModel.eCourse.Course = courseAllocation.Course;
                //=viewModel.ECourseList = eCourseLogic.getBy(courseAllocation.Course.Id);
                viewModel.ECourseList = eCourseLogic.GetModelsBy(f => f.E_CONTENT_TYPE.Course_Allocation_Id == courseAllocation.Id && f.E_CONTENT_TYPE.IsDelete == false && f.IsDelete == false);
                ViewBag.Topics = Utility.PopulateEContentTypeSelectListItemByCourseAllocation(courseAllocation.Id);


            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddContent(ELearningViewModel viewModel)
        {
            try
            {
                string Topic = "";
                var courseAllocation = new CourseAllocation();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.CourseAllocation.Id);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("Index");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file];
                        if (hpf.ContentLength > 0 && hpf.FileName != "")
                        {
                            string pathForSaving = Server.MapPath("/Content/ELearning");
                            if (!Directory.Exists(pathForSaving))
                            {
                                Directory.CreateDirectory(pathForSaving);
                            }

                            string extension = Path.GetExtension(hpf.FileName);
                            string newFilename = courseAllocation.Course.Code.Replace("/", "_").Replace(" ", "_") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + extension;
                            string savedFileName = Path.Combine(pathForSaving, newFilename);
                            FileUploadURL = savedFileName;
                            hpf.SaveAs(savedFileName);

                            viewModel.eCourse.Url = "~/Content/ELearning/" + newFilename;

                        }
                        var isCreatedEcourse = eCourseLogic.Create(viewModel.eCourse);
                        var courseRegistrationDetail = courseRegistrationDetailLogic.GetModelsBy(f => f.STUDENT_COURSE_REGISTRATION.Session_Id == courseAllocation.Session.Id && f.STUDENT_COURSE_REGISTRATION.Programme_Id == courseAllocation.Programme.Id
                                                                     && f.STUDENT_COURSE_REGISTRATION.Department_Id == courseAllocation.Department.Id && f.STUDENT_COURSE_REGISTRATION.Level_Id == courseAllocation.Level.Id
                                                                     && f.Semester_Id == courseAllocation.Semester.Id && f.Course_Id == courseAllocation.Course.Id).Select(f => f.CourseRegistration);
                        if (isCreatedEcourse != null)
                        {
                            SetMessage("Added Content Succesfully", Message.Category.Information);
                            scope.Complete();
                        }
                        else
                        {
                            SetMessage("Error Occured while Processing Your Request", Message.Category.Error);
                        }



                    }
                }
                List<Model.Model.Student> studentList = new List<Model.Model.Student>();
                //studentList = GetClassList(courseAllocation);
                //StudentLogic studentLogic = new StudentLogic();
                //var student=studentLogic.GetModelsBy(f => f.Person_Id == 98712).FirstOrDefault();
                //studentList.Add(student);
                var contentType = eContentTypeLogic.GetModelBy(f => f.Id == viewModel.eCourse.EContentType.Id);
                if (CheckForInternetConnection())
                {
                    Topic = contentType != null ? contentType.Name : " ";
                    var courseCode = courseAllocation.Course.Code + "-" + courseAllocation.Course.Name;
                    //SendMail(studentList, 2, courseCode, null, Topic);
                }

                return RedirectToAction("ViewContent", "Elearning", new { area = "Admin", cid = Utility.Encrypt(viewModel.CourseAllocation.Id.ToString()) });



            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("Index");
            }
            //return Redirect("Index");
        }

        public ActionResult AssignmentIndex()
        {
            ViewBag.Session = viewModel.ActiveSessionList;
            ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
            //ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            ViewBag.Level = viewModel.CourseAllocationLevel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AssignmentIndex(ELearningViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult AssignmentViewContent(string cid)
        {
            try
            {
                ViewBag.MaxGrade = Utility.GradeGuideSelectListItem(100);
                long courseId = Convert.ToInt64(Utility.Decrypt(cid));
                var courseAllocation = new CourseAllocation();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == courseId);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("AssignmentIndex");
                }

                viewModel.CourseAllocation = courseAllocation;
                viewModel.cid = courseId;
                viewModel.eAssignment = new EAssignment();
                viewModel.eAssignment.Course = courseAllocation.Course;
                //viewModel.EAssignmentList = eAssignmentLogic.getBy(courseAllocation.Course.Id);
                viewModel.EAssignmentList = eAssignmentLogic.GetModelsBy(f => f.Course_Allocation_Id == courseAllocation.Id && f.IsDelete == false);
                ViewBag.ContentType = Utility.PopulateEContentTypeSelectListItemByCourseAllocation(courseAllocation.Id);


            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("AssignmentIndex");
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AssignmentAddContent(ELearningViewModel viewModel)
        {
            try
            {

                var courseAllocation = new CourseAllocation();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.CourseAllocation.Id);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("AssignmentIndex");
                }

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("/Content/ELearning/Assignment");
                    if (!Directory.Exists(pathForSaving))
                    {
                        Directory.CreateDirectory(pathForSaving);
                    }

                    string extension = Path.GetExtension(hpf.FileName);
                    string newFilename = courseAllocation.Course.Code.Replace("/", "_").Replace(" ", "_") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + extension;
                    string savedFileName = Path.Combine(pathForSaving, newFilename);
                    FileUploadURL = savedFileName;
                    hpf.SaveAs(savedFileName);

                    viewModel.eAssignment.URL = "~/Content/ELearning/Assignment/" + newFilename;
                    viewModel.eAssignment.DateSet = DateTime.Now;
                    viewModel.eAssignment.CourseAllocation = courseAllocation;
                    viewModel.eAssignment.DueDate = viewModel.eAssignment.DueDate.Add(viewModel.startTime);
                    eAssignmentLogic.Create(viewModel.eAssignment);
                    SetMessage("Added Assignemnt Succesfully", Message.Category.Information);

                    List<Model.Model.Student> studentList = new List<Model.Model.Student>();
                    //studentList = GetClassList(courseAllocation);
                    //StudentLogic studentLogic = new StudentLogic();
                    //var student = studentLogic.GetModelsBy(f => f.Person_Id == 98712).FirstOrDefault();
                    //studentList.Add(student);
                    if (CheckForInternetConnection())
                    {
                        var courseCode = courseAllocation.Course.Code + "-" + courseAllocation.Course.Name;
                        //SendMail(studentList, 1, courseCode, viewModel.eAssignment.DueDate.ToString(), viewModel.eAssignment.Assignment);
                        //SendMail(studentList, 1, courseAllocation.Course.Code);
                    }

                    return Redirect("AssignmentIndex");


                }


            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("AssignmentIndex");
            }
            return Redirect("AssignmentIndex");
        }


        public ActionResult AssignmentSubmission(string AssignmentId)
        {
            try
            {
                if (!String.IsNullOrEmpty(AssignmentId))
                {
                    long Id = Convert.ToInt64(Utility.Decrypt(AssignmentId));
                    viewModel.eAssignment = eAssignmentLogic.GetAssignment(Id);
                    viewModel.EAssignmentSubmissionList = eAssignmentSubmissionLogic.GetBy(Id);
                    if (viewModel.eAssignment?.Id > 0)
                    {
                        ViewBag.MaxGrade = Utility.GradeGuideSelectListItem(viewModel.eAssignment.MaxScore);
                    }
                    else
                    {
                        ViewBag.MaxGrade = Utility.GradeGuideSelectListItem(100);
                    }

                }
                else
                {
                    Redirect("AssignmentIndex");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(viewModel);
        }
        public ActionResult EditAssignment(string eAssignmentId)
        {
            try
            {
                if (!String.IsNullOrEmpty(eAssignmentId))
                {
                    long Id = Convert.ToInt64(Utility.Decrypt(eAssignmentId));
                    viewModel.eAssignment = eAssignmentLogic.GetAssignment(Id);
                }
                else
                {
                    Redirect("AssignmentIndex");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditAssignment(ELearningViewModel viewModel)
        {
            try
            {
                if (viewModel.eAssignment?.Id > 0)
                {
                    EAssignmentLogic eAssignmentLogic = new EAssignmentLogic();
                    var eAssignment = eAssignmentLogic.GetModelBy(f => f.Id == viewModel.eAssignment.Id);
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file];
                        if (hpf.ContentLength > 0 && hpf.FileName != "")
                        {
                            string pathForSaving = Server.MapPath("/Content/ELearning/Assignment");
                            if (!Directory.Exists(pathForSaving))
                            {
                                Directory.CreateDirectory(pathForSaving);
                            }

                            string extension = Path.GetExtension(hpf.FileName);
                            string newFilename = eAssignment.CourseAllocation.Course.Code.Replace("/", "_").Replace(" ", "_") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + extension;
                            string savedFileName = Path.Combine(pathForSaving, newFilename);
                            FileUploadURL = savedFileName;
                            hpf.SaveAs(savedFileName);

                            viewModel.eAssignment.URL = "~/Content/ELearning/Assignment/" + newFilename;
                        }

                        viewModel.eAssignment.DateSet = DateTime.Now;
                        viewModel.eAssignment.DueDate = viewModel.eAssignment.DueDate;

                        eAssignmentLogic.Modify(viewModel.eAssignment);
                        SetMessage("Assignemnt Modified Succesfully", Message.Category.Information);

                        List<Model.Model.Student> studentList = new List<Model.Model.Student>();
                        //studentList=GetClassList(eAssignment.CourseAllocation);
                        //StudentLogic studentLogic = new StudentLogic();
                        //var student = studentLogic.GetModelsBy(f => f.Person_Id == 98712).FirstOrDefault();
                        //studentList.Add(student);
                        if (CheckForInternetConnection())
                        {

                            //SendMail(studentList, 1, eAssignment.CourseAllocation.Course.Code);
                        }

                        return Redirect("AssignmentIndex");


                    }
                }



            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("AssignmentIndex");
            }
            return Redirect("AssignmentIndex");
        }
        public ActionResult EditEContentType(string eContentTypeId)
        {
            try
            {
                if (!String.IsNullOrEmpty(eContentTypeId))
                {
                    long Id = Convert.ToInt64(Utility.Decrypt(eContentTypeId));
                    viewModel.eContentType = eContentTypeLogic.GetModelBy(f => f.Id == Id);
                }
                else
                {
                    return RedirectToAction("ManageCourseContent");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditEContentType(ELearningViewModel viewModel)
        {
            try
            {
                if (viewModel.eContentType?.Id > 0)
                {

                    eContentTypeLogic.Modify(viewModel.eContentType);
                    SetMessage("Updated Successfully", Message.Category.Information);
                    return RedirectToAction("ManageCourseContent");
                }
                else
                {
                    return RedirectToAction("ManageCourseContent");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(viewModel);
        }
        public JsonResult CreateTopic(string topic, string coursedescription, bool active, string to, string from, string courseAllocationId, string fromtime, string totime)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                EContentType eContentType = new EContentType();
                EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                DateTime ToDate = Convert.ToDateTime(to);
                DateTime FromDate = Convert.ToDateTime(from);
                var spanFromTime = TimeSpan.Parse(fromtime);
                var spanToTime = TimeSpan.Parse(totime);
                DateTime To = ToDate.Add(spanToTime);
                DateTime From = FromDate.Add(spanFromTime);
                var Id = Convert.ToInt64(courseAllocationId);
                eContentType.Name = topic;
                eContentType.Description = coursedescription;
                eContentType.Active = true;
                eContentType.StartTime = From;
                eContentType.EndTime = To;
                eContentType.IsDelete = false;
                eContentType.CourseAllocation = new CourseAllocation() { Id = Id };
                var checkIfExisting = eContentTypeLogic.GetModelsBy(s => s.Name.Contains(topic) && s.Description.Contains(coursedescription) && s.IsDelete == false && s.Course_Allocation_Id == Id);

                if (checkIfExisting.Count <= 0)
                {

                    var createdEContentType = eContentTypeLogic.Create(eContentType);
                    if (createdEContentType?.Id > 0)
                    {
                        var existingChatTopic = eChatTopicLogic.GetModelsBy(g => g.Course_Allocation_Id == createdEContentType.Id).FirstOrDefault();
                        if (existingChatTopic == null)
                        {
                            EChatTopic eChatTopic = new EChatTopic()
                            {
                                CourseAllocation = new CourseAllocation() { Id = Id },
                                Active = true,

                            };
                            eChatTopicLogic.Create(eChatTopic);
                        }

                    }
                    result.IsError = false;
                    result.Message = "Topic Created Successfully";
                    result.EContentType = eContentTypeLogic.GetModelsBy(f => f.Course_Allocation_Id == Id && f.IsDelete == false);
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Topic Already Exist";
                    result.EContentType = eContentTypeLogic.GetModelsBy(f => f.Course_Allocation_Id == Id && f.IsDelete == false);
                }





            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SendMail(List<Model.Model.Student> studentList, int type, string CourseCode, string dueDate, string topic)
        {
            try
            {
                if (studentList?.Count > 0)
                {
                    foreach (var item in studentList)
                    {
                        ELearningEmail eLearningEmail = new ELearningEmail();
                        eLearningEmail.Name = item.FirstName;
                        EmailMessage message = new EmailMessage();

                        message.Email = item.Email ?? "support@lloydant.com";
                        message.Subject = type == 1 ? "FPI E-LEARNING:ASSIGNMENT ALERT" : "FPI E-LEARNING:NEW LECTURE MATERIAL";
                        eLearningEmail.header = message.Subject;
                        eLearningEmail.footer = "https://applications.federalpolyilaro.edu.ng/Security/Account/Login";
                        if (type == 1)
                        {
                            message.Body = "An assignment has just been added for" + " " + CourseCode +
                                Environment.NewLine + " " +
                               "Topic:" + topic +
                               Environment.NewLine + " " +
                               "Due Date:" + dueDate;
                            eLearningEmail.message = message.Body;
                        }
                        else
                        {
                            message.Body = "New Lecture material has just been added for" + " " + CourseCode + Environment.NewLine +
                               "Topic:" + topic;
                            eLearningEmail.message = message.Body;
                        }
                        var template = Server.MapPath("/Areas/Common/Views/Credential/ELearningEmailTemplate.cshtml");
                        EmailSenderLogic<ELearningEmail> receiptEmailSenderLogic = new EmailSenderLogic<ELearningEmail>(template, eLearningEmail);

                        receiptEmailSenderLogic.Send(message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DownloadElearningGuide()
        {
            try
            {

                string fileRootPath = Server.MapPath("/Content/ELearning/Manual");
                string fileName = "ELearningGuide.pdf";

                string fullPart = Path.Combine(fileRootPath, fileName);
                if (!Directory.Exists(fullPart))
                {
                    SetMessage("No Guide yet", Message.Category.Information);
                    return View();

                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPart);
                if (!string.IsNullOrEmpty(fileName))
                {
                    return File(fileBytes, "application/force-download", fileName);
                }

                return null;
            }
            catch (Exception ex) { throw ex; }
        }
        public ActionResult GetClassAttendanceList(string eContentId)
        {
            try
            {
                EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                ECourseLogic eCourseLogic = new ECourseLogic();
                ECourseContentDownloadLogic eCourseContentDownloadLogic = new ECourseContentDownloadLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                if (!String.IsNullOrEmpty(eContentId))
                {
                    var id = Convert.ToInt64(Utility.Decrypt(eContentId));
                    viewModel.eCourse = eCourseLogic.GetModelBy(f => f.Id == id);
                    if (viewModel.eCourse?.Id > 0)
                    {
                        var studentPresent = eCourseContentDownloadLogic.GetModelsBy(f => f.E_Course_Content_Id == viewModel.eCourse.Id);
                        viewModel.AttendanceClassList = new List<AttendanceClassList>();
                        var classList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == viewModel.eCourse.EContentType.CourseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == viewModel.eCourse.EContentType.CourseAllocation.Session.Id);
                        if (classList?.Count > 0)
                        {
                            foreach (var item in classList)
                            {
                                AttendanceClassList attendanceClassList = new AttendanceClassList();
                                var isPresent = studentPresent.Where(f => f.Person.Id == item.CourseRegistration.Student.Id).FirstOrDefault();
                                if (isPresent?.ECourseDownloadId > 0)
                                {
                                    attendanceClassList.IsPresent = true;
                                }
                                attendanceClassList.CourseRegistrationDetail = item;
                                viewModel.AttendanceClassList.Add(attendanceClassList);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public ActionResult GetClassAttendanceForAllTopics(string courseAllocationId)
        {
            try
            {
                EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                ECourseLogic eCourseLogic = new ECourseLogic();
                ECourseContentDownloadLogic eCourseContentDownloadLogic = new ECourseContentDownloadLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                if (!String.IsNullOrEmpty(courseAllocationId))
                {
                    viewModel.GeneralAttendanceList = new List<GeneralAttendance>();

                    var id = Convert.ToInt64(Utility.Decrypt(courseAllocationId));
                    var allCourseTopic = eContentTypeLogic.GetModelsBy(g => g.Course_Allocation_Id == id && g.IsDelete == false);

                    if (allCourseTopic?.Count > 0)
                    {

                        var getCourse = allCourseTopic.FirstOrDefault();
                        viewModel.eCourse = eCourseLogic.GetModelsBy(f => f.E_Content_Type_Id == getCourse.Id).FirstOrDefault();
                        var classList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == getCourse.CourseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == getCourse.CourseAllocation.Session.Id && f.STUDENT_COURSE_REGISTRATION.Level_Id == getCourse.CourseAllocation.Course.Level.Id);
                        foreach (var item in allCourseTopic)
                        {
                            GeneralAttendance generalAttendance = new GeneralAttendance();
                            var studentPresent = eCourseContentDownloadLogic.GetModelsBy(f => f.E_COURSE_CONTENT.E_Content_Type_Id == item.Id);

                            generalAttendance.Topics = item.Name;

                            List<TopicAttendance> topicAttendanceList = new List<TopicAttendance>();
                            foreach (var classitem in classList)
                            {
                                TopicAttendance topicAttendance = new TopicAttendance();
                                topicAttendance.CourseRegistrationDetail = classitem;
                                var isPresent = studentPresent.Where(f => f.Person.Id == classitem.CourseRegistration.Student.Id).FirstOrDefault();
                                if (isPresent?.ECourseDownloadId > 0)
                                {
                                    topicAttendance.IsPresent = true;
                                }
                                topicAttendanceList.Add(topicAttendance);

                            }
                            generalAttendance.AttendanceClassLists = topicAttendanceList;
                            generalAttendance.CourseRegistrationDetailList = classList;
                            viewModel.GeneralAttendanceList.Add(generalAttendance);
                        }



                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public ActionResult GetAssignmentList(string eAssignmentId)
        {
            try
            {
                EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                EAssignmentLogic eAssignmentLogic = new EAssignmentLogic();

                viewModel.EAssignmentClassList = new List<EAssignmentClassList>();

                if (!String.IsNullOrEmpty(eAssignmentId))
                {
                    var id = Convert.ToInt64(Utility.Decrypt(eAssignmentId));
                    viewModel.eAssignment = eAssignmentLogic.GetModelBy(f => f.Id == id && f.IsDelete == false);
                    if (viewModel.eAssignment?.Id > 0)
                    {
                        var classList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == viewModel.eAssignment.CourseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == viewModel.eAssignment.CourseAllocation.Session.Id);
                        viewModel.EAssignmentSubmissionList = eAssignmentSubmissionLogic.GetModelsBy(f => f.Assignment_Id == id);
                        foreach (var item in classList)
                        {
                            EAssignmentClassList eAssignmentClass = new EAssignmentClassList();
                            var didAssignment = viewModel.EAssignmentSubmissionList.Where(f => f.Student.Id == item.CourseRegistration.Student.Id).FirstOrDefault();
                            if (didAssignment?.Id > 0)
                            {

                                eAssignmentClass.IsSubmission = true;
                            }
                            eAssignmentClass.CourseRegistrationDetail = item;
                            eAssignmentClass.EAssignmentSubmission = didAssignment;
                            viewModel.EAssignmentClassList.Add(eAssignmentClass);

                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public JsonResult ScoreAssignment(string eAssignmentSubmissionId, string score, string remarks)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(eAssignmentSubmissionId) && !String.IsNullOrEmpty(score))
                {
                    EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();
                    var id = Convert.ToInt64(eAssignmentSubmissionId);
                    var givenScore = Convert.ToDecimal(score);
                    var eAssignmentSubmission = eAssignmentSubmissionLogic.GetModelBy(g => g.Id == id);
                    if (eAssignmentSubmission?.Id > 0)
                    {
                        eAssignmentSubmission.Score = givenScore;
                        eAssignmentSubmission.Remarks = remarks;
                        eAssignmentSubmissionLogic.Modify(eAssignmentSubmission);
                        result.IsError = false;
                        result.Message = "You have successfully Scored  " + eAssignmentSubmission.Student.FullName + "  Test|Assignment";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<Model.Model.Student> GetClassList(CourseAllocation courseAllocation)
        {
            List<Model.Model.Student> studentList = new List<Model.Model.Student>();
            try
            {
                if (courseAllocation?.Id > 0)
                {
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    studentList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == courseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == courseAllocation.Session.Id).Select(d => d.CourseRegistration.Student).ToList();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return studentList;
        }
        public ActionResult AverageElearningTestResult(string cid)
        {
            try
            {
                if (!String.IsNullOrEmpty(cid))
                {
                    ViewBag.courseAllocationId = Convert.ToInt64(Utility.Decrypt(cid));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }
        public ActionResult ManageCourseContent()
        {
            try
            {
                ViewBag.Session = viewModel.ActiveSessionList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
               //ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Level = viewModel.CourseAllocationLevel;
                ViewBag.AllocatedCourse = new SelectList(new List<CourseAllocated>(), ID, NAME);
                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult ManageCourseContent(ELearningViewModel viewModel)
        {
            try
            {
                try
                {
                    EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                    viewModel.EContentTypeList = eContentTypeLogic.GetModelsBy(f => f.Course_Allocation_Id == viewModel.CourseAllocationId && f.IsDelete == false);

                    KeepDropDownState(viewModel);
                    //TempData["vModel"] = viewModel;
                }
                catch (Exception ex)
                {
                    SetMessage(ex.Message, Message.Category.Error);
                }
                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult GetAllocatedCourses(string sessionId, string levelId, string semesterId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(semesterId) && !String.IsNullOrEmpty(sessionId) && !String.IsNullOrEmpty(levelId))
                {
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    var sesid = Convert.ToInt32(sessionId);
                    var semid = Convert.ToInt32(semesterId);
                    var levid = Convert.ToInt32(levelId);

                    var allocatedCourse = courseAllocationLogic.GetModelsBy(g => g.USER.User_Name == User.Identity.Name && g.Level_Id == levid && g.Semester_Id == semid && g.Session_Id == sesid)
                        .Select(m => new CourseAllocated
                        {
                            Id = m.Id,
                            Name = m.Course.Code + "-" + m.Course.Name + " - " + m.Programme.Name + " " + "(" + m.Department.Name + ")"
                        });
                    return Json(new SelectList(allocatedCourse, ID, NAME), JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTopic(string id)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                    var topicId = Convert.ToInt32(id);
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    var econtentType = eContentTypeLogic.GetModelBy(f => f.Id == topicId);
                    if (econtentType?.Id > 0)
                    {
                        econtentType.IsDelete = true;
                        eContentTypeLogic.Modify(econtentType);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);
                        //    var ecourses = eCourseLogic.GetEntitiesBy(f => f.E_Content_Type_Id == econtentType.Id);
                        //    if (ecourses?.Count > 0)
                        //    {
                        //        foreach (var item in ecourses)
                        //        {
                        //            var downloads = eCourseContentDownloadLogic.GetEntitiesBy(g => g.E_Course_Content_Id == item.Id);
                        //            if (downloads?.Count > 0)
                        //            {
                        //                foreach(var item2 in downloads)
                        //                {
                        //                    eCourseContentDownloadLogic.Delete(item2);
                        //                }

                        //            }
                        //        var ecourse=eCourseLogic.GetEntityBy(g => g.Id == item.Id);
                        //            eCourseLogic.Delete(ecourse);
                        //        }

                        //    }
                        //var EcourseType=eContentTypeLogic.GetEntityBy(g => g.Id == econtentType.Id);
                        //    eContentTypeLogic.Delete(EcourseType);
                    }

                    // }

                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEContent(string id)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var ecourseId = Convert.ToInt32(id);

                    var ecourseContent = eCourseLogic.GetModelBy(f => f.Id == ecourseId);
                    if (ecourseContent?.Id > 0)
                    {
                        ecourseContent.IsDelete = true;
                        eCourseLogic.Modify(ecourseContent);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }



                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EnterChatRoom(string courseAllocationId)
        {
            try
            {
                if (!String.IsNullOrEmpty(courseAllocationId))
                {
                    UserLogic userLogic = new UserLogic();
                    viewModel.User = userLogic.GetModelsBy(f => f.User_Name == User.Identity.Name).FirstOrDefault();
                    long id = Convert.ToInt64(Utility.Decrypt(courseAllocationId));
                    viewModel.CourseAllocation = courseAllocationLogic.GetModelBy(f => f.Course_Allocation_Id == id);
                    var existingChatTopic = eChatTopicLogic.GetModelsBy(f => f.Course_Allocation_Id == id).LastOrDefault();
                    if (existingChatTopic == null)
                    {
                        EChatTopic eChatTopic = new EChatTopic()
                        {
                            CourseAllocation = viewModel.CourseAllocation,
                            Active = true
                        };
                        eChatTopicLogic.Create(eChatTopic);
                    }
                    viewModel.EChatResponses = eChatResponseLogic.GetModelsBy(f => f.E_CHAT_TOPIC.Course_Allocation_Id == id);

                }



            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public JsonResult AjaxLoadChatBoard(long courseAllocationId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                UserLogic userLogic = new UserLogic();
                viewModel.User = userLogic.GetModelsBy(f => f.User_Name == User.Identity.Name).FirstOrDefault();
                if (viewModel.User != null && courseAllocationId > 0)
                {

                    result.EChatBoards = eChatResponseLogic.LoadChatBoard(null, viewModel.User, courseAllocationId);
                    result.IsError = false;

                }

            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        public JsonResult SaveChatResponse(long courseAllocationId, string response)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                UserLogic userLogic = new UserLogic();
                viewModel.User = userLogic.GetModelsBy(f => f.User_Name == User.Identity.Name).FirstOrDefault();
                if (viewModel.User != null && courseAllocationId > 0 && response != null && response != " ")
                {

                    eChatResponseLogic.SaveChatResponse(courseAllocationId, null, response, viewModel.User);
                    result.IsError = false;

                }
            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        public ActionResult GetGeneralAttendanceByPercentage(string courseAllocationId)
        {
            try
            {
                EContentTypeLogic eContentTypeLogic = new EContentTypeLogic();
                ECourseLogic eCourseLogic = new ECourseLogic();
                ECourseContentDownloadLogic eCourseContentDownloadLogic = new ECourseContentDownloadLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<TopicAttendance> topicAttendanceList = new List<TopicAttendance>();
                if (!String.IsNullOrEmpty(courseAllocationId))
                {
                    viewModel.GeneralAttendanceList = new List<GeneralAttendance>();

                    var id = Convert.ToInt64(Utility.Decrypt(courseAllocationId));
                    var allCourseTopic = eContentTypeLogic.GetModelsBy(g => g.Course_Allocation_Id == id && g.IsDelete == false);

                    if (allCourseTopic?.Count > 0)
                    {

                        var getCourse = allCourseTopic.FirstOrDefault();
                        var noOfTopics=allCourseTopic.Count();
                        viewModel.eCourse = eCourseLogic.GetModelsBy(f => f.E_Content_Type_Id == getCourse.Id).FirstOrDefault();
                        var classList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == getCourse.CourseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == getCourse.CourseAllocation.Session.Id && f.STUDENT_COURSE_REGISTRATION.Level_Id==getCourse.CourseAllocation.Course.Level.Id);
                        var generalStudentPresent = eCourseContentDownloadLogic.GetModelsBy(f => f.E_COURSE_CONTENT.E_CONTENT_TYPE.Course_Allocation_Id == id && f.E_COURSE_CONTENT.E_CONTENT_TYPE.IsDelete==false);
                        if (classList?.Count > 0)
                        {
                            foreach (var classitem in classList)
                            {
                                if(classitem.CourseRegistration.Student.Id== 33738)
                                {
                                    var me = 0;
                                }
                                TopicAttendance topicAttendance = new TopicAttendance();
                                topicAttendance.CourseRegistrationDetail = classitem;
                                var presentCount=generalStudentPresent.Where(f => f.Person.Id == classitem.CourseRegistration.Student.Id).ToList().Count();
                                topicAttendance.PresentCount = presentCount + "/" + noOfTopics;
                                
                                topicAttendance.PresentPercentage = ((presentCount * 100) / noOfTopics);
                                topicAttendanceList.Add(topicAttendance);

                            }
                        }
                        viewModel.TopicAttendanceList = topicAttendanceList;



                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public JsonResult PublishEAssignmentResult(string id, bool status)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var eAssignmentId = Convert.ToInt32(id);

                    var eAssignment = eAssignmentLogic.GetModelBy(f => f.Id == eAssignmentId);
                    if (eAssignment?.Id > 0)
                    {
                        eAssignment.Publish = status;
                        eAssignmentLogic.Modify(eAssignment);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }



                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActiveSemester(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Session session = new Session() { Id = Convert.ToInt32(id) };
                SemesterLogic semesterLogic = new SemesterLogic();
                List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id && p.Active==true);

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




    }
    public class CourseAllocated
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}