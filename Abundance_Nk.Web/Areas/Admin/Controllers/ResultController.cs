using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Data;
using System.Data.OleDb;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Models.Result;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Controllers;
using System.Web.UI.WebControls;
using System.IO;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    //[AllowAnonymous]
    public class ResultController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private ResultUploadViewModel viewModel;

        public ResultController()
        {

        }

        public ActionResult Upload()
        {
            try
            {
                ResultUploadViewModel vm = (ResultUploadViewModel)TempData["ResultUploadViewModel"];
                PopulateDropDowns(vm);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            if (viewModel.StudentResult.Uploader == null || viewModel.StudentResult.Uploader.Id <= 0)
            {
                viewModel.StudentResult.Uploader = (User)TempData.Peek("User");
            }

            TempData["ResultUploadViewModel"] = viewModel;
            return View();
        }

        private void PopulateDropDowns(ResultUploadViewModel vm)
        {
            try
            {
                if (vm == null)
                {
                    viewModel = new ResultUploadViewModel();

                    ViewBag.Levels = viewModel.LevelSelectList;
                    ViewBag.Programmes = viewModel.ProgrammeSelectList;
                    ViewBag.ResultTypes = viewModel.StudentResultTypeSelectList;
                    ViewBag.SessionSemesters = viewModel.SessionSemesterSelectList;
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    ViewBag.MaximumObtainableScores = new SelectList(viewModel.MaximumObtainableScores, Utility.ID, Utility.NAME);
                }
                else
                {
                    //ViewBag.Levels = new SelectList(vm.LevelSelectList, Utility.VALUE, Utility.TEXT, vm.Level.Id);
                    //ViewBag.Programmes = new SelectList(vm.ProgrammeSelectList, Utility.VALUE, Utility.TEXT, vm.Programme.Id);
                    //ViewBag.ResultTypes = new SelectList(vm.StudentResultTypeSelectList, Utility.VALUE, Utility.TEXT, vm.StudentResultType.Id);
                    //ViewBag.SessionSemesters = new SelectList(vm.SessionSemesterSelectList, Utility.VALUE, Utility.TEXT, vm.SessionSemester.Id);
                    //ViewBag.MaximumObtainableScores = new SelectList(vm.MaximumObtainableScores, Utility.ID, Utility.NAME, vm.MaximumObtainableScore.Id);

                    ViewBag.Levels = new SelectList(vm.LevelSelectList, Utility.VALUE, Utility.TEXT, vm.StudentResult.Level.Id);
                    ViewBag.Programmes = new SelectList(vm.ProgrammeSelectList, Utility.VALUE, Utility.TEXT, vm.StudentResult.Programme.Id);
                    ViewBag.ResultTypes = new SelectList(vm.StudentResultTypeSelectList, Utility.VALUE, Utility.TEXT, vm.StudentResult.Type.Id);
                    ViewBag.SessionSemesters = new SelectList(vm.SessionSemesterSelectList, Utility.VALUE, Utility.TEXT, vm.StudentResult.SessionSemester.Id);
                    ViewBag.MaximumObtainableScores = new SelectList(vm.MaximumObtainableScores, Utility.ID, Utility.NAME, vm.StudentResult.MaximumObtainableScore);

                    KeepDepartmentDropDownState(vm);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
        }

        private void KeepDepartmentDropDownState(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentResult.Programme != null && viewModel.StudentResult.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectList = Utility.PopulateDepartmentSelectListItem(viewModel.StudentResult.Programme);
                    if (viewModel.StudentResult.Department != null && viewModel.StudentResult.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(viewModel.DepartmentSelectList, Utility.VALUE, Utility.TEXT, viewModel.StudentResult.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectList, Utility.VALUE, Utility.TEXT);
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult SaveUpload(ResultUploadViewModel viewModel)
        {
            try
            {
                List<StudentResultDetail> results = (List<StudentResultDetail>)TempData["StudentResultDetails"];

                if (results == null || results.Count <= 0)
                {
                    SetMessage("Uploaded Results not set!", Message.Category.Error);
                    return PartialView("_Message", TempData["Message"]);
                }

                //StudentResult studentResult = new StudentResult();
                //studentResult.Department = viewModel.Department;
                //studentResult.Type = viewModel.StudentResultType;
                //studentResult.Level = viewModel.Level;
                //studentResult.Programme = viewModel.Programme;
                //studentResult.SessionSemester = viewModel.SessionSemester;
                //studentResult.MaximumObtainableScore = viewModel.MaximumObtainableScore.Id;
                //studentResult.Uploader = new User() { Id = 1 };
                //studentResult.DateUploaded = DateTime.Now;
                //studentResult.UploadedFileUrl = "url";
                //studentResult.Results = results;
                //viewModel.StudentResult.Uploader = new User() { Id = 1 };
                //viewModel.StudentResult.Uploader = (User)TempData.Peek("User");
                //viewModel.StudentResult.UploadedFileUrl = "url";

                //User user = (User)TempData.Peek("User");
                viewModel.StudentResult.Results = results;
                viewModel.StudentResult.DateUploaded = DateTime.Now;
                viewModel.StudentResult.UploadedFileUrl = (string)TempData.Peek("FilePath");


                StudentResultLogic studentResultLogic = new StudentResultLogic();
                bool saved = studentResultLogic.Save(viewModel.StudentResult);
                if (saved)
                {
                    SetMessage("Uploaded Results has been successfully saved.", Message.Category.Information);
                }
                else
                {
                    SetMessage("Uploaded Results Save operation failed! Please try again.", Message.Category.Error);
                }

                return PartialView("_Message", TempData["Message"]);
            }
            catch (Exception ex)
            {
                DeleteFile(viewModel.StudentResult.UploadedFileUrl);
                SetMessage(ex.Message, Message.Category.Error);
            }

            return PartialView("_Message", TempData["Message"]);
        }

        public ActionResult TestView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, ResultUploadViewModel viewModel)
        {
            try
            {
                viewModel.StudentResult.Uploader = (User)TempData.Peek("User");

                if (ModelState.IsValid)
                {
                    DataTable excelData = ReadExcelFile(file, "~/Content/Result");
                    if (excelData != null)
                    {
                        viewModel.StudentResult.UploadedFileUrl = (string)TempData.Peek("FilePath");
                        ViewBag.TotalCount = "Total count of uploaded rows are: " + excelData.Rows.Count;

                        CourseLogic courseLogic = new CourseLogic();
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                        StudentResultTypeLogic studentResultTypeLogic = new StudentResultTypeLogic();


                        int maximumScoreObtainable = viewModel.StudentResult.MaximumObtainableScore;
                        SessionSemester sessionSemester = sessionSemesterLogic.GetBy(viewModel.StudentResult.SessionSemester.Id);
                        List<StudentLevel> studentLevels = studentLevelLogic.GetBy(viewModel.StudentResult.Level, sessionSemester.Session);
                        List<Course> courses = courseLogic.GetBy(viewModel.StudentResult.Department, viewModel.StudentResult.Level, sessionSemester.Semester);
                        StudentResultType resultType = studentResultTypeLogic.GetBy(viewModel.StudentResult.Type.Id);


                        //SessionSemester sessionSemester = sessionSemesterLogic.GetBy(viewModel.SessionSemester.Id);
                        //List<StudentLevel> studentLevels = studentLevelLogic.GetBy(viewModel.Level, sessionSemester.Session);
                        //List<Course> courses = courseLogic.GetBy(viewModel.Department, viewModel.Level, sessionSemester.Semester);
                        //StudentResultType resultType = studentResultTypeLogic.GetBy(viewModel.StudentResultType.Id);


                        if (AlreadyUploaded(studentLevels[0].Level, studentLevels[0].Programme, studentLevels[0].Department, sessionSemester))
                        {
                            PopulateDropDowns(viewModel);
                            return View(viewModel);
                        }


                        if (InvalidMaximumScoreObtainable(maximumScoreObtainable, studentLevels[0].Level, studentLevels[0].Programme, studentLevels[0].Department, sessionSemester))
                        {
                            PopulateDropDowns(viewModel);
                            return View(viewModel);
                        }


                        DataTableTranslatorBase<StudentResultDetail> dataTableTranslator = new StudentResultDataTableTranslator(studentLevels, resultType, courses, maximumScoreObtainable);
                        List<StudentResultDetail> results = dataTableTranslator.Translate(excelData);


                        viewModel.ExcelData = excelData;
                        viewModel.StudentResultDetails = results;
                        if (results == null || results.Count <= 0 && excelData == null || excelData.Rows.Count <= 0)
                        {
                            SetMessage("Selected Result File is empty!", Message.Category.Error);
                        }

                        //if (results != null && results.Count > 0 && excelData != null && excelData.Rows.Count > 0)
                        //{
                        //    SetMessage("Selected Result File has been successfully uploaded.", Message.Category.Information);
                        //}
                        //else
                        //{
                        //    SetMessage("Selected Result File is empty!", Message.Category.Error);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                //DeleteFile((string)TempData["FilePath"]);

                DeleteFile(viewModel.StudentResult.UploadedFileUrl);
                SetMessage(ex.Message, Message.Category.Error);
            }

            TempData["StudentResultDetails"] = viewModel.StudentResultDetails;
            PopulateDropDowns(viewModel);
            return View(viewModel);
        }

        private bool AlreadyUploaded(Level level, Programme programme, Department department, SessionSemester sessionSemester)
        {
            try
            {
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<StudentResult> studentResults = studentResultLogic.GetBy(level, programme, department, sessionSemester);

                if (studentResults != null && studentResults.Count > 0)
                {
                    SetMessage("Upload for the selected programme, level, department and session already exist! Operation was aborted to avoid duplicate. Kindly change your selection to do a fresh upload.", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidMaximumScoreObtainable(int selectedMaxObtainableScore, Level level, Programme programme, Department department, SessionSemester sessionSemester)
        {
            const int ZERO = 0;
            const int ONE_HNDRED = 100;

            try
            {
                if (selectedMaxObtainableScore <= 0)
                {
                    SetMessage("Please select Maximum Obtainable Score!", Message.Category.Error);
                    return true;
                }

                StudentResultLogic studentResultLogic = new StudentResultLogic();
                int maximumObtainableScore = studentResultLogic.GetTotalMaximumObtainableScore(level, programme, department, sessionSemester);
                int totalScore = maximumObtainableScore + selectedMaxObtainableScore;

                if (maximumObtainableScore > ZERO)
                {
                    if (totalScore != ONE_HNDRED)
                    {
                        SetMessage("Total Maximum Score Obtainable is currently " + totalScore + ", it must be equal to 100! Please correct and try again.", Message.Category.Error);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable ReadExcelFile(HttpPostedFileBase file, string folderPath)
        {
            try
            {
                DataTable excelData = null;
                if (file != null && file.ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string folderLocation = Server.MapPath(folderPath);
                        CreateDirectory(folderLocation);

                        string fileLocation = Server.MapPath(folderPath + "/") + file.FileName;
                        DeleteFile(fileLocation);

                        //TempData["FileUrl"] = imageUrl;
                        TempData["FilePath"] = fileLocation;
                        file.SaveAs(fileLocation);

                        ExcelReader excelReader = new ExcelReader();
                        excelData = excelReader.ReadExcel(fileLocation);
                    }
                }
                else
                {
                    SetMessage("No file found to upload!", Message.Category.Error);
                }

                return excelData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void DeleteFile(string fileLocation)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileLocation))
                {
                    return;
                }

                if (System.IO.File.Exists(fileLocation))
                {
                    System.IO.File.Delete(fileLocation);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void CreateDirectory(string folderLocation)
        {
            try
            {
                if (!System.IO.Directory.Exists(folderLocation))
                {
                    System.IO.Directory.CreateDirectory(folderLocation);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(departments, JsonRequestBehavior.AllowGet);

                //return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ExamSheet()
        {
            return View();
        }

        public ActionResult MasterSheet()
        {
            return View();
        }

        public ActionResult ResultByCourse()
        {
            return View();
        }

        public ActionResult Transcript()
        {
            return View();
        }

        public ActionResult EditResult()
        {
            ResultUploadViewModel viewModel = new ResultUploadViewModel();
            try
            {
                PopulateDropDowns(viewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditResult(ResultUploadViewModel viewModel)
        {
            try
            {
                CourseRegistrationDetailLogic courseRegistrationLogic = new CourseRegistrationDetailLogic();
                if (viewModel.student.MatricNumber != null && viewModel.StudentResult.SessionSemester.Id > 0)
                {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    SessionSemester sessionSemester = new SessionSemester();
                    sessionSemester = sessionSemesterLogic.GetBy(viewModel.StudentResult.SessionSemester.Id);
                    viewModel.StudentCourseRegistrationDetails = courseRegistrationLogic.GetResultsBy(viewModel.student, sessionSemester);
                }
                PopulateDropDowns(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateResult(ResultUploadViewModel viewModel)
        {
            try
            {
                string operation = "MODIFY";
                string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (ResultController)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                CourseRegistrationDetailLogic courseRegistrationLogic = new CourseRegistrationDetailLogic();
                if (courseRegistrationLogic.UpdateCourseRegistrationScore(viewModel.StudentCourseRegistrationDetails, courseRegistrationDetailAudit))
                {
                    SetMessage("Results were updated successfully", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);

            }
            return RedirectToAction("EditResult");
        }

        public ActionResult RemoveStudentCourse()
        {
            try
            {
                ResultUploadViewModel vModel = new ResultUploadViewModel();
                ViewBag.SessionId = vModel.SessionSelectList;
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public ActionResult RemoveStudentCourse(ResultUploadViewModel viewModel)
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                StudentLogic studentLogic = new StudentLogic();
                Abundance_Nk.Model.Model.Student student = new Model.Model.Student();
                student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.MatricNumber);
                deleteStudentCourse(student, viewModel.CourseCode, viewModel);
                ViewBag.SessionId = new SelectList(sessionLogic.GetAll(), ID, NAME, viewModel.Session.Id);
                TempData["Action"] = "Delete successful";
            }
            catch (Exception ex)
            {

                SetMessage(ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        private void deleteStudentCourse(Model.Model.Student student, string courseCode, ResultUploadViewModel viewModel)
        {
            try
            {
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                string operation = "DELETE";
                string action = "ADMIN :REMOVE STUDENT COURSE FROM ADMIN CONSOLE (ResultController)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistration courseRegistration = new CourseRegistration();
                Course course = new Course();
                CourseLogic courseLogic = new CourseLogic();
                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();
                course = courseLogic.GetModelBy(p => p.Course_Code == courseCode.Trim() && p.Department_Id == studentLevel.Department.Id);
                courseRegistration = courseRegistrationLogic.GetModelBy(p => p.Person_Id == student.Id && p.Session_Id == viewModel.Session.Id);

                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                //courseRegistrationDetail = courseRegistrationDetailLogic.GetModelBy(p => p.Course_Id == course.Id && p.Student_Course_Registration_Id == courseRegistration.Id);
                StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                StudentResultDetail studentResultDetail = new StudentResultDetail();
                //studentResultDetail = studentResultDetailLogic.GetModelBy(p => p.Course_Id == course.Id && p.Person_Id == student.Id);

                using (TransactionScope scope = new TransactionScope())
                {
                    courseRegistrationDetail = courseRegistrationDetailLogic.GetModelBy(p => p.Course_Id == course.Id && p.Student_Course_Registration_Id == courseRegistration.Id);

                    courseRegistrationDetailAudit.Course = course;
                    courseRegistrationDetailAudit.CourseRegistration = courseRegistration;
                    courseRegistrationDetailAudit.CourseUnit = course.Unit;

                    if (courseRegistrationDetail != null)
                    {
                        courseRegistrationDetailAudit.ExamScore = courseRegistrationDetail.ExamScore;
                        courseRegistrationDetailAudit.TestScore = courseRegistrationDetail.TestScore;
                        courseRegistrationDetailAudit.Mode = courseRegistrationDetail.Mode;
                        courseRegistrationDetailAudit.Semester = courseRegistrationDetail.Semester;
                        courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetail.SpecialCase;
                    }
                    courseRegistrationDetailAudit.Student = student;

                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                    bool deleteStudentResultDetail = studentResultDetailLogic.Delete(p => p.Course_Id == course.Id && p.Person_Id == student.Id);
                    bool deletecourseRegistrationDetail = courseRegistrationDetailLogic.Delete(p => p.Course_Id == course.Id && p.Student_Course_Registration_Id == courseRegistration.Id);

                    scope.Complete();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult DownloadResultSheet()
        {
            try
            {
                ResultUploadViewModel viewModel = new ResultUploadViewModel();
                ViewBag.AllSession = viewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);

            }
            return View();
        }
        [HttpPost]
        public ActionResult DownloadResultSheet(ResultUploadViewModel viewModel)
        {
            try
            {
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<ResultFormat> resultFormatList = new List<ResultFormat>();

                SessionLogic sessionLogic = new SessionLogic();
                //Session session = sessionLogic.GetModelBy(p => p.Session_Id == viewModel.Session.Id);
                StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<CourseRegistrationDetail> courseRegistrationDetailList = new List<CourseRegistrationDetail>();
                List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                courseRegistrationList = courseRegistrationLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id);
                if (viewModel.Course != null && viewModel.Semester != null && courseRegistrationList.Count > 0)
                {
                    int count = 1;
                    foreach (CourseRegistration courseRegistration in courseRegistrationList)
                    {
                        courseRegistrationDetailList = courseRegistrationDetailLogic.GetModelsBy(p => p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id && p.Student_Course_Registration_Id == courseRegistration.Id);
                        if (courseRegistrationDetailList.Count > 0)
                        {

                            foreach (CourseRegistrationDetail courseRegistrationDetailItem in courseRegistrationDetailList)
                            {
                                ResultFormat resultFormat = new ResultFormat();
                                //resultFormat.SN = count;
                                resultFormat.MATRICNO = courseRegistrationDetailItem.CourseRegistration.Student.MatricNumber.Trim();
                                StudentExamRawScoreSheet studentExamRawScoreSheet =
                                    studentExamRawScoreSheetResultLogic.GetModelBy(
                                        a =>
                                            a.Course_Id == courseRegistrationDetailItem.Course.Id &&
                                            a.Student_Id == courseRegistrationDetailItem.CourseRegistration.Student.Id);

                                if (studentExamRawScoreSheet != null && studentExamRawScoreSheet.Id > 0)
                                {
                                    resultFormat.QU1 = (decimal)studentExamRawScoreSheet.QU1;
                                    resultFormat.QU2 = (decimal)studentExamRawScoreSheet.QU2;
                                    resultFormat.QU3 = (decimal)studentExamRawScoreSheet.QU3;
                                    resultFormat.QU4 = (decimal)studentExamRawScoreSheet.QU4;
                                    resultFormat.QU5 = (decimal)studentExamRawScoreSheet.QU5;
                                    resultFormat.QU6 = (decimal)studentExamRawScoreSheet.QU6;
                                    resultFormat.QU7 = (decimal)studentExamRawScoreSheet.QU7;
                                    resultFormat.QU8 = (decimal)studentExamRawScoreSheet.QU8;
                                    resultFormat.QU9 = (decimal)studentExamRawScoreSheet.QU9;
                                    resultFormat.T_CA = (decimal)studentExamRawScoreSheet.T_CA;
                                    resultFormat.T_EX = (decimal)studentExamRawScoreSheet.T_EX;
                                    resultFormat.EX_CA = (decimal)studentExamRawScoreSheet.EX_CA;
                                }

                                resultFormatList.Add(resultFormat);

                                count++;
                            }

                        }
                    }

                }

                if (resultFormatList.Count > 0)
                {
                    List<ResultFormat> list = resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    List<ResultFormat> sort = new List<ResultFormat>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort;// resultFormatList.OrderBy(p => p.MATRICNO);
                    CourseLogic courseLogic = new CourseLogic();
                    Course course = courseLogic.GetModelBy(p => p.Course_Id == viewModel.Course.Id);
                    gv.Caption = course.Code + " " + " DEPARTMENT OF " + " " + course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                    gv.DataBind();


                    return new DownloadFileActionResult(gv, "ResultSheet.xls");
                }
                else
                {
                    Response.Write("No data available for download");
                    Response.End();
                    return new JavaScriptResult();
                }

            }
            catch (Exception ex)
            {

                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("DownloadResultSheet");
        }

        public ActionResult UploadResultSheet()
        {
            try
            {
                ResultUploadViewModel viewModel = new ResultUploadViewModel();
                ViewBag.AllSession = viewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);

            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadResultSheet(ResultUploadViewModel viewModel)
        {
            try
            {
                List<ResultFormat> resultFormatList = new List<ResultFormat>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if (studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 1; i < studentSet.Tables[0].Rows.Count; i++)
                        {
                            ResultFormat resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MATRICNO = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.QU1 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][2].ToString().Trim());
                            resultFormat.QU2 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][3].ToString().Trim());
                            resultFormat.QU3 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            resultFormat.QU4 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][5].ToString().Trim());
                            resultFormat.QU5 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][6].ToString().Trim());
                            resultFormat.QU6 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][7].ToString().Trim());
                            resultFormat.QU7 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][8].ToString().Trim());
                            resultFormat.QU8 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][9].ToString().Trim());
                            resultFormat.QU9 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][10].ToString().Trim());
                            resultFormat.T_EX = Convert.ToDecimal(studentSet.Tables[0].Rows[i][11].ToString().Trim());
                            resultFormat.T_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][12].ToString().Trim());
                            resultFormat.EX_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][13].ToString().Trim());

                            if (resultFormat.MATRICNO != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }

                        }
                        resultFormatList.OrderBy(p => p.MATRICNO);
                        viewModel.resultFormatList = resultFormatList;
                        TempData["resultUploadViewModel"] = viewModel;

                    }

                }
            }
            catch (Exception ex)
            {

                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }
        public ActionResult SaveUploadedResultSheet()
        {
            ResultUploadViewModel viewModel = (ResultUploadViewModel)TempData["resultUploadViewModel"];
            try
            {
                if (viewModel != null)
                {

                    int status = validateFields(viewModel.resultFormatList);

                    if (status > 0)
                    {
                        ResultFormat format = viewModel.resultFormatList.ElementAt((status - 1));
                        SetMessage("Validation Error for" + " " + format.MATRICNO, Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return RedirectToAction("UploadResultSheet");
                    }

                    bool resultAdditionStatus = addStudentResult(viewModel, User.Identity.Name);

                    SetMessage("Upload successful", Message.Category.Information);
                }


            }
            catch (Exception ex)
            {

                SetMessage("Error occured " + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View("UploadResultSheet");
        }
        public ActionResult ProcessResult()
        {
            try
            {
                ResultUploadViewModel viewModel = new ResultUploadViewModel();
                ViewBag.AllSession = viewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                // ViewBag.StudentType = viewModel.StudentTypeSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
        [HttpPost]
        public ActionResult ProcessResult(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Semester != null && viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                    {
                        GetResultList(viewModel);
                        TempData["ResultUploadViewModel"] = viewModel;
                        RetainDropdownState(viewModel);
                    }

                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        public ActionResult DownloadZip(string downloadName)
        {
            TempData["downloadName"] = downloadName + ".zip";
            TempData["view"] = "DownloadZip";
            return View();
        }
        public ActionResult DownloadStatementOfResultZip(string downloadName)
        {
            TempData["downloadName"] = downloadName + ".zip";
            TempData["view"] = "DownloadStatementOfResultZip";
            return View();
        }
        public ActionResult GetZip()
        {
            string callingView = "";
            try
            {
                string downloadName = (string)TempData["downloadName"];
                TempData.Keep("downloadName");
                callingView = (string)TempData["view"];
                TempData.Keep("view");
                string path = "~/Content/temp/" + downloadName;
                Response.Redirect(path, false);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }

            return View(callingView);
        }
        public ActionResult ProcessDeactivatedResult()
        {
            try
            {
                ResultUploadViewModel viewModel = new ResultUploadViewModel();
                ViewBag.AllSession = viewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                // ViewBag.StudentType = viewModel.StudentTypeSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public ActionResult NotificationOfResult(long id)
        {
            try
            {
                if (id > 0)
                {
                    ResultUploadViewModel viewModel = (ResultUploadViewModel)TempData["ResultUploadViewModel"];
                    if (viewModel.Semester != null && viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                    {
                        return RedirectToAction("NotificationOfResult", new { controller = "Report", area = "Admin", personId = id, semesterId = viewModel.Semester.Id, sessionid = viewModel.Session.Id, programmeId = viewModel.Programme.Id, departmentId = viewModel.Department.Id, levelId = viewModel.Level.Id });
                    }
                    else
                    {
                        SetMessage("Field not set", Message.Category.Error);
                        return RedirectToAction("ProcessResult");
                    }

                }

                else
                {
                    SetMessage("Student does not exist", Message.Category.Error);
                    return RedirectToAction("ProcessResult");
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        [HttpPost]
        public ActionResult ProcessDeactivatedResult(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Semester != null && viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                    {
                        GetDeactivatedResultList(viewModel);
                        TempData["ResultUploadViewModel"] = viewModel;
                        RetainDropdownState(viewModel);
                    }

                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);

        }
        public ActionResult ViewProcessedStudentResult(long id)
        {
            ResultUploadViewModel viewModel = (ResultUploadViewModel)TempData["ResultUploadViewModel"];
            try
            {
                if (id > 0)
                {
                    Abundance_Nk.Model.Model.Student student = new Model.Model.Student() { Id = id };
                    StudentLogic studentLogic = new StudentLogic();
                    StudentResultLogic studentResultLogic = new StudentResultLogic();


                    if (viewModel.Semester != null && viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                    {
                        if (viewModel.Level.Id == 1 || viewModel.Level.Id == 3)
                        {


                            Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == id);

                            if (viewModel.Semester.Id == 1)
                            {
                                List<Result> result = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, viewModel.Semester, viewModel.Programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, viewModel.Semester, viewModel.Programme);
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
                                viewModel.Result = modifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                decimal? firstSemesterGPA = 0M;
                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                                }

                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    viewModel.Result.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                                }
                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    viewModel.Result.CGPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                                }

                                viewModel.ResultList = modifiedResultList;
                            }
                            else
                            {
                                List<Result> result = null;
                                Semester firstSemester = new Semester() { Id = 1 };
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, firstSemester, viewModel.Programme);

                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, firstSemester, viewModel.Programme);
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
                                viewModel.Result = firstSemesterModifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit = firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                decimal? firstSemesterGPA = 0M;
                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                                }

                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    viewModel.Result.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                                }

                                Semester secondSemester = new Semester() { Id = 2 };
                                List<Result> secondSemesterResult = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, secondSemester, viewModel.Programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, student, secondSemester, viewModel.Programme);
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
                                viewModel.Result = secondSemesterStudentResult;
                                if (secondSemesterGPCUSum != null && secondSemesterGPCUSum > 0)
                                {
                                    viewModel.Result.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / secondSemesterStudentResult.TotalSemesterCourseUnit), 2);
                                }
                                if (firstSemesterGPCUSum > 0 || secondSemesterGPCUSum > 0)
                                {
                                    viewModel.Result.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum) / (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) + firstSemesterTotalSemesterCourseUnit)), 2);
                                }

                                viewModel.ResultList = secondSemesterModifiedResultList;
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

                            Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(viewModel, student);
                            Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(viewModel, student);
                            if (viewModel.Semester.Id == 1)
                            {

                                List<Result> result = null;


                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = 1 };

                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, semester, viewModel.Programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, semester, viewModel.Programme);
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

                                viewModel.Result = modifiedResultList.FirstOrDefault();
                                if (firstSemesterGPA != null && firstSemesterGPA > 0)
                                {
                                    viewModel.Result.GPA = Decimal.Round((decimal)firstSemesterGPA, 2);
                                }
                                if (firstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                                {
                                    if ((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) > 0 && firstYearSecondSemester.TotalSemesterCourseUnit != null && firstYearFirstSemester.TotalSemesterCourseUnit != null && secondYearfirstSemesterTotalSemesterCourseUnit != null)
                                    {
                                        firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                                        firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                                        viewModel.Result.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit)), 2);                               
                                    }
                                }
                                viewModel.ResultList = modifiedResultList;


                            }
                            else if (viewModel.Semester.Id == 2)
                            {

                                List<Result> result = null;


                                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                Semester semester = new Semester() { Id = 1 };

                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, semester, viewModel.Programme);
                                }
                                else
                                {
                                    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, semester, viewModel.Programme);
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

                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult = studentResultLogic.GetStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, secondSemester, viewModel.Programme);
                                }
                                else
                                {
                                    secondSemesterResult = studentResultLogic.GetDeactivatedStudentProcessedResultBy(viewModel.Session, viewModel.Level, viewModel.Department, studentCheck, secondSemester, viewModel.Programme);
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

                                viewModel.Result = secondSemesterModifiedResultList.FirstOrDefault();
                                if (secondYearSecondSmesterGPA != null && secondYearSecondSmesterGPA > 0)
                                {
                                    viewModel.Result.GPA = Decimal.Round((decimal)secondYearSecondSmesterGPA, 2);
                                }
                                if (secondYearfirstSemesterGPCUSum != null && firstYearFirstSemester != null && firstYearSecondSemester != null)
                                {
                                    firstYearFirstSemester.TotalSemesterCourseUnit = firstYearFirstSemester.TotalSemesterCourseUnit ?? 0;
                                    firstYearSecondSemester.TotalSemesterCourseUnit = firstYearSecondSemester.TotalSemesterCourseUnit ?? 0;
                                    viewModel.Result.CGPA = Decimal.Round((decimal)((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU + firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum) / (firstYearSecondSemester.TotalSemesterCourseUnit + firstYearFirstSemester.TotalSemesterCourseUnit + secondYearfirstSemesterTotalSemesterCourseUnit + secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                                }
                                //viewModel.ResultList = modifiedResultList;
                                viewModel.ResultList = secondSemesterModifiedResultList;

                            }



                        }
                    }
                }

            }
            catch (Exception ex)
            {

                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            TempData["ResultUploadViewModel"] = viewModel;
            return View(viewModel);
        }

        private Result GetFirstYearSecondSemesterResultInfo(ResultUploadViewModel viewModel, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                Semester semester = new Semester() { Id = 2 };
                Level level = null;
                if (viewModel.Level.Id == 2)
                {
                    level = new Level() { Id = 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == studentCheck.Id && p.Level_Id == level.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id).FirstOrDefault();
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
        private Result GetFirstYearFirstSemesterResultInfo(ResultUploadViewModel viewModel, Model.Model.Student student)
        {
            try
            {
                List<Result> result = null;
                StudentLogic studentLogic = new StudentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                Abundance_Nk.Model.Model.Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);


                Semester semester = new Semester() { Id = 1 };
                Level level = null;
                if (viewModel.Level.Id == 2)
                {
                    level = new Level() { Id = 1 };
                }
                else
                {
                    level = new Level() { Id = 3 };
                }
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == studentCheck.Id && p.Level_Id == level.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id).FirstOrDefault();
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
        [HttpPost]
        public ActionResult ViewProcessedStudentResult(ResultUploadViewModel viewModel)
        {
            ResultUploadViewModel vModel = (ResultUploadViewModel)TempData["ResultUploadViewModel"];

            try
            {
                if (viewModel.Result != null && viewModel.Enable != null)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    Abundance_Nk.Model.Model.Student student = new Model.Model.Student();
                    student.Id = viewModel.Result.StudentId;
                    student = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                    if (viewModel.Enable == "1")
                    {
                        student.Activated = true;
                    }
                    else if (viewModel.Enable == "0")
                    {
                        student.Activated = false;
                    }
                    else
                    {
                        student.Activated = true;
                    }
                    student.Reason = viewModel.Reason;
                    student.RejectCategory = viewModel.RejectCategory;
                    bool isModified = studentLogic.Modify(student);
                    if (isModified)
                    {

                        SetMessage("Result Processed Successfully", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("No changes made", Message.Category.Warning);
                    }
                }
                else
                {
                    SetMessage("No changes made", Message.Category.Warning);
                }
                GetResultList(vModel);
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            TempData["ResultUploadViewModel"] = vModel;
            RetainDropdownState(vModel);
            return View("ProcessResult", vModel);
        }
        public bool addStudentResult(ResultUploadViewModel viewModel, string userName)
        {
            try
            {
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = null;
                if (User == null)
                {
                    user = userLogic.GetModelBy(p => p.User_Name == userName);
                }
                else
                {
                    user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                }

                sessionSemester = sessionSemesterLogicc.GetModelBy(p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);

                string operation = "MODIFY";
                string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (ResultController)";
                //string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = viewModel.Client;
                courseRegistrationDetailAudit.User = user;
                courseRegistrationDetailAudit.Time = DateTime.Now;


                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    StudentResultDetail studentResultDetailTest;
                    StudentResultDetail studentResultDetailExam;
                    List<StudentResultDetail> studentResultDetailTestList;
                    List<StudentResultDetail> studentResultDetailExamList;
                    StudentResultLogic studentResultLogic;
                    StudentResult studentResultTest;
                    StudentResult studentResultExam;
                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic = new StudentExamRawScoreSheetResultLogic();

                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {


                            InitializeStudentResult(viewModel, sessionSemester, testResultType, examResultType, user, out studentResultDetailTest, out studentResultDetailExam, out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic, out studentResultTest, out studentResultExam);
                            studentResultDetailTest.Course = viewModel.Course;
                            studentResultDetailTest.Student = studentLogic.GetModelBy(p => p.Matric_Number == resultFormat.MATRICNO.Trim());
                            studentResultDetailTest.Score = resultFormat.T_CA;
                            studentResultDetailTest.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
                            studentResultDetailTestList.Add(studentResultDetailTest);
                            studentResultTest.Results = studentResultDetailTestList;
                            studentResultLogic.Add(studentResultTest, courseRegistrationDetailAudit);

                            studentResultDetailExam.Course = viewModel.Course;
                            studentResultDetailExam.Student = studentResultDetailTest.Student;
                            studentResultDetailExam.Score = resultFormat.T_EX;
                            studentResultDetailExam.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
                            studentResultDetailExamList.Add(studentResultDetailExam);
                            studentResultExam.Results = studentResultDetailExamList;
                            studentResultLogic.Add(studentResultExam, courseRegistrationDetailAudit);
                            studentExamRawScoreSheet = StudentExamRawScoreSheetLogic.GetModelBy(p => p.Student_Id == studentResultDetailExam.Student.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);
                            if (studentExamRawScoreSheet == null)
                            {
                                studentExamRawScoreSheet = CreateExamRawScoreSheet(viewModel, user, studentResultDetailTest, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
                            }
                            else
                            {
                                ModifyExamRawScoreSheet(viewModel, user, studentResultDetailTest, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
                            }

                        }
                        scope.Complete();
                    }


                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private StudentExamRawScoreSheet CreateExamRawScoreSheet(ResultUploadViewModel viewModel, Model.Model.User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
        {
            try
            {
                studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                studentExamRawScoreSheet.Course = viewModel.Course;
                studentExamRawScoreSheet.EX_CA = (double)resultFormat.EX_CA;
                studentExamRawScoreSheet.Level = viewModel.Level;
                studentExamRawScoreSheet.T_CA = (double)resultFormat.T_CA;
                studentExamRawScoreSheet.T_EX = (double)resultFormat.T_EX;
                studentExamRawScoreSheet.QU1 = (double)resultFormat.QU1;
                studentExamRawScoreSheet.QU2 = (double)resultFormat.QU2;
                studentExamRawScoreSheet.QU3 = (double)resultFormat.QU3;
                studentExamRawScoreSheet.QU4 = (double)resultFormat.QU4;
                studentExamRawScoreSheet.QU5 = (double)resultFormat.QU5;
                studentExamRawScoreSheet.QU6 = (double)resultFormat.QU6;
                studentExamRawScoreSheet.QU7 = (double)resultFormat.QU7;
                studentExamRawScoreSheet.QU8 = (double)resultFormat.QU8;
                studentExamRawScoreSheet.QU9 = (double)resultFormat.QU9;
                studentExamRawScoreSheet.Semester = viewModel.Semester;
                studentExamRawScoreSheet.Session = viewModel.Session;
                studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
                studentExamRawScoreSheet.Student = studentResultDetailTest.Student;
                studentExamRawScoreSheet.Uploader = user;
                StudentExamRawScoreSheetLogic.Create(studentExamRawScoreSheet);
                return studentExamRawScoreSheet;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ModifyExamRawScoreSheet(ResultUploadViewModel viewModel, Model.Model.User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
        {
            try
            {
                studentExamRawScoreSheet.Course = viewModel.Course;
                studentExamRawScoreSheet.EX_CA = (double)resultFormat.EX_CA;
                studentExamRawScoreSheet.Level = viewModel.Level;
                studentExamRawScoreSheet.T_CA = (double)resultFormat.T_CA;
                studentExamRawScoreSheet.T_EX = (double)resultFormat.T_EX;
                studentExamRawScoreSheet.QU1 = (double)resultFormat.QU1;
                studentExamRawScoreSheet.QU2 = (double)resultFormat.QU2;
                studentExamRawScoreSheet.QU3 = (double)resultFormat.QU3;
                studentExamRawScoreSheet.QU4 = (double)resultFormat.QU4;
                studentExamRawScoreSheet.QU5 = (double)resultFormat.QU5;
                studentExamRawScoreSheet.QU6 = (double)resultFormat.QU6;
                studentExamRawScoreSheet.QU7 = (double)resultFormat.QU7;
                studentExamRawScoreSheet.QU8 = (double)resultFormat.QU8;
                studentExamRawScoreSheet.QU9 = (double)resultFormat.QU9;
                studentExamRawScoreSheet.Semester = viewModel.Semester;
                studentExamRawScoreSheet.Session = viewModel.Session;
                studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
                studentExamRawScoreSheet.Student = studentResultDetailTest.Student;
                studentExamRawScoreSheet.Uploader = user;
                bool isScoreSheetModified = StudentExamRawScoreSheetLogic.Modify(studentExamRawScoreSheet);
                //CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                //List<CourseRegistrationDetail> courseRegistrationDetail = new List<CourseRegistrationDetail>();
                //courseRegistrationDetail = courseRegistrationDetailLogic.GetModelsBy(p => p.Course_Id == viewModel.Course.Id && p.STUDENT_COURSE_REGISTRATION.Person_Id == studentResultDetailTest.Student.Id && p.Semester_Id == viewModel.Semester.Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == viewModel.Session.Id);
                //courseRegistrationDetailLogic.UpdateCourseRegistrationScore(courseRegistrationDetail);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static void InitializeStudentResult(ResultUploadViewModel viewModel, SessionSemester sessionSemester, StudentResultType testResultType, StudentResultType examResultType, User user, out StudentResultDetail studentResultDetailTest, out StudentResultDetail studentResultDetailExam, out List<StudentResultDetail> studentResultDetailTestList, out List<StudentResultDetail> studentResultDetailExamList, out StudentResultLogic studentResultLogic, out StudentResult studentResultTest, out StudentResult studentResultExam)
        {
            studentResultDetailTest = new StudentResultDetail();
            studentResultDetailExam = new StudentResultDetail();
            studentResultDetailTestList = new List<StudentResultDetail>();
            studentResultDetailExamList = new List<StudentResultDetail>();
            studentResultLogic = new StudentResultLogic();
            studentResultTest = new StudentResult();
            studentResultExam = new StudentResult();

            studentResultTest.MaximumObtainableScore = 30;
            studentResultTest.DateUploaded = DateTime.Now;
            studentResultTest.Department = viewModel.Department;
            studentResultTest.Level = viewModel.Level;
            studentResultTest.Programme = viewModel.Programme;
            studentResultTest.SessionSemester = sessionSemester;
            studentResultTest.UploadedFileUrl = "STAFF UPLOAD";
            studentResultTest.Uploader = user;
            studentResultTest.Type = testResultType;

            studentResultExam.MaximumObtainableScore = 70;
            studentResultExam.DateUploaded = DateTime.Now;
            studentResultExam.Department = viewModel.Department;
            studentResultExam.Level = viewModel.Level;
            studentResultExam.Programme = viewModel.Programme;
            studentResultExam.SessionSemester = sessionSemester;
            studentResultExam.UploadedFileUrl = "STAFF UPLOAD";
            studentResultExam.Uploader = user;
            studentResultExam.Type = examResultType;
        }
        private List<ResultHolder> RetrieveCourseRegistrationInformation(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.resultFormatList != null)
                {
                    List<ResultHolder> results = new List<ResultHolder>();
                    foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                    {
                        List<Abundance_Nk.Model.Model.Student> students = new List<Model.Model.Student>();
                        StudentLogic studentLogic = new StudentLogic();
                        students = studentLogic.GetModelsBy(p => p.Matric_Number == resultFormat.MATRICNO);

                        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                        if (students.Count == 1)
                        {
                            CourseRegistration courseRegistration = new CourseRegistration();
                            ResultHolder result = new ResultHolder();
                            long studentId = students[0].Id;
                            courseRegistration = courseRegistrationLogic.GetModelBy(p => p.Person_Id == studentId && p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id && p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id);

                            if (courseRegistration != null)
                            {
                                result.CourseRegistration = courseRegistration;
                                result.ResultFormat = resultFormat;
                                results.Add(result);
                            }
                        }

                    }
                    return results;

                }
                else
                {
                    return new List<ResultHolder>();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void GetResultList(ResultUploadViewModel viewModel)
        {
            try
            {
                List<Result> filteredResult = new List<Result>();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<string> resultList = studentResultLogic.GetProcessedResutBy(viewModel.Session, viewModel.Semester, viewModel.Level, viewModel.Department, viewModel.Programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Result> result = studentResultLogic.GetProcessedResutBy(viewModel.Session, viewModel.Semester, viewModel.Level, viewModel.Department, viewModel.Programme);
                foreach (string item in resultList)
                {
                    Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }
                viewModel.ResultList = filteredResult.OrderBy(p => p.MatricNumber).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }
        private void GetDeactivatedResultList(ResultUploadViewModel viewModel)
        {
            try
            {
                List<Result> filteredResult = new List<Result>();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<string> resultList = studentResultLogic.GetDeactivatedProcessedResutBy(viewModel.Session, viewModel.Semester, viewModel.Level, viewModel.Department, viewModel.Programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Result> result = studentResultLogic.GetDeactivatedProcessedResutBy(viewModel.Session, viewModel.Semester, viewModel.Level, viewModel.Department, viewModel.Programme);
                foreach (string item in resultList)
                {
                    Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }
                viewModel.ResultList = filteredResult.OrderBy(p => p.MatricNumber).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }
        private int validateFields(List<ResultFormat> list)
        {
            try
            {
                int failedReason = 0;

                if (list != null && list.Count > 0)
                {

                    for (int i = 0; i < list.Count; i++)
                    {
                        bool testStatus;
                        bool examStatus;
                        bool totalStatus;
                        decimal testScore = list[i].T_CA;
                        decimal inputExamScore = list[i].T_EX;
                        decimal projectQuestionSUm = list[i].QU1 + list[i].QU2 + list[i].QU3 + list[i].QU4 + list[i].QU5 + list[i].QU6 + list[i].QU7 + list[i].QU8 + list[i].QU9;
                        if (testScore > 100 || inputExamScore > 100)
                        {
                            AssignSpecialCaseRemarks(list, i, testScore, inputExamScore);
                        }
                        else if (projectQuestionSUm == 0 && (inputExamScore > 0 || testScore > 0)) // for project and practical
                        {
                            decimal CalculatedTotal = inputExamScore + testScore;
                            decimal InputTotalScore = list[i].EX_CA;
                            if (CalculatedTotal != InputTotalScore)
                            {
                                failedReason = (i + 1);
                            }

                        }
                        else
                        {
                            decimal calculatedExamScore = list[i].QU1 + list[i].QU2 + list[i].QU3 + list[i].QU4 + list[i].QU5 + list[i].QU6 + list[i].QU7 + list[i].QU8 + list[i].QU9;
                            decimal inputTotalScore = list[i].EX_CA;
                            decimal calculatedTotalScore = list[i].T_CA + list[i].T_EX;
                            list[i].ResultSpecialCaseMessages.SpecialCaseMessage = null;
                            list[i].ResultSpecialCaseMessages.TestSpecialCaseMessage = null;
                            list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = null;
                            if (testScore >= 0 && testScore <= 30)
                            {
                                testStatus = true;

                            }
                            else
                            {
                                testStatus = false;
                                if (failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }
                            }

                            if ((inputExamScore >= 0 && inputExamScore <= 70) && (calculatedExamScore >= 0 && calculatedExamScore <= 70))
                            {
                                examStatus = true;

                            }
                            else
                            {
                                examStatus = false;
                                if (failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }

                            }

                            if ((calculatedTotalScore == inputTotalScore) && calculatedTotalScore <= 100)
                            {
                                totalStatus = true;
                            }
                            else
                            {
                                totalStatus = false;
                                if (failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }
                            }
                            if (calculatedExamScore != inputExamScore)
                            {
                                if (failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }
                            }


                        }

                    }

                }
                if (failedReason > 0)
                {
                    return failedReason;
                }
                else
                {
                    return 0;
                }



            }
            catch (Exception)
            {

                throw;
            }
        }
        private static void AssignSpecialCaseRemarks(List<ResultFormat> list, int i, decimal testScore, decimal inputExamScore)
        {
            try
            {
                if (testScore == (decimal)SpeicalCaseCodes.Sick)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "SICK";
                    list[i].ResultSpecialCaseMessages.TestSpecialCaseMessage = "SICK";
                    list[i].T_CA = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Sick)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "SICK";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "SICK";
                    list[i].T_EX = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Absent)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "ABSENT";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "ABSENT";
                    list[i].T_EX = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void RetainDropdownState(ResultUploadViewModel viewModel)
        {
            try
            {
                SemesterLogic semesterLogic = new SemesterLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                StudentTypeLogic studentTypeLogic = new StudentTypeLogic();
                if (viewModel != null)
                {
                    if (viewModel.Session != null)
                    {

                        ViewBag.Session = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true), ID, NAME, viewModel.Session.Id);
                        ViewBag.AllSession = new SelectList(sessionLogic.GetAll(), ID, NAME, viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                        ViewBag.AllSession = viewModel.AllSessionSelectList;
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
                        ViewBag.Programme = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true), ID, NAME, viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }
                    if (viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME, viewModel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                    }
                    if (viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                    }
                    if (viewModel.Course != null && viewModel.Level != null && viewModel.Semester != null && viewModel.Department != null)
                    {
                        List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level, viewModel.Department, viewModel.Semester);
                        ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                    }
                    if (viewModel.StudentType != null)
                    {
                        ViewBag.StudentType = new SelectList(studentTypeLogic.GetAll(), ID, NAME, viewModel.StudentType.Id);
                    }
                    else
                    {
                        ViewBag.StudentType = viewModel.StudentTypeSelectList;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public JsonResult GetDepartments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

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

                Session session = new Session() { Id = Convert.ToInt32(id) };
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
                Level level = new Level() { Id = Convert.ToInt32(ids[1]) };
                Department department = new Department() { Id = Convert.ToInt32(ids[0]) };
                Semester semester = new Semester() { Id = Convert.ToInt32(ids[1]) };
                List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level, department, semester);

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;"  + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                //string xConnStr = "Provider = Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";

                
                OleDbConnection connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString().Replace("'", "");
                    OleDbCommand command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                    // Create DbDataReader to Data Worksheet

                    OleDbDataAdapter MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    DataSet ds = new DataSet();
                    ds.Clear();
                    MyData.Fill(ds);
                    connection.Close();

                    Result = ds;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;

        }
        public enum SpeicalCaseCodes
        {
            Sick = 101,
            Absent = 201
        }
        public ActionResult AddGraduationDate()
        {
            try
            {
                ResultUploadViewModel viewModel = new ResultUploadViewModel();
                ViewBag.AllSession = viewModel.AllSessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
        [HttpPost]
        public ActionResult AddGraduationDate(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                    {
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();

                        viewModel.StudentLevels = studentLevelLogic.GetModelsBy( s => s.Department_Id == viewModel.Department.Id && s.Level_Id == viewModel.Level.Id &&
                                                                                        s.Programme_Id == viewModel.Programme.Id && s.Session_Id == viewModel.Session.Id);

                        for (int i = 0; i < viewModel.StudentLevels.Count; i++)
                        {
                            long studentId = viewModel.StudentLevels[i].Student.Id;

                            StudentAcademicInformation studentAcademicInformation = studentAcademicInformationLogic.GetModelsBy(s => s.Person_Id == studentId).LastOrDefault();

                            if (studentAcademicInformation != null && studentAcademicInformation.GraduationDate != null)
                            {
                                viewModel.StudentLevels[i].GraduationDate = studentAcademicInformation.GraduationDate.Value.ToLongDateString();
                            }
                            if (studentAcademicInformation != null && studentAcademicInformation.TranscriptDate != null)
                            {
                                viewModel.StudentLevels[i].TranscriptDate = studentAcademicInformation.TranscriptDate.Value.ToLongDateString();
                            }
                        }

                        if (viewModel.StudentLevels.Count > 0)
                        {
                            viewModel.StudentLevels = viewModel.StudentLevels.OrderBy(s => s.Student.MatricNumber).ToList();
                        }
                        
                        RetainDropdownState(viewModel);
                    }

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult SaveGraduationDate(ResultUploadViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();

                    viewModel.StudentLevels = viewModel.StudentLevels.Where(s => s.Active).ToList();

                    for (int i = 0; i < viewModel.StudentLevels.Count; i++)
                    {
                        long studentId = viewModel.StudentLevels[i].Student.Id;

                        StudentAcademicInformation studentAcademicInformation = studentAcademicInformationLogic.GetModelsBy(s => s.Person_Id == studentId).LastOrDefault();
                        if (studentAcademicInformation != null)
                        {
                            if (viewModel.IsGraduationDate)
                            {
                                studentAcademicInformation.GraduationDate = viewModel.Date;
                            }
                            else if (viewModel.IsTranscriptDate)
                            {
                                studentAcademicInformation.TranscriptDate = viewModel.Date;
                            }
                            
                            studentAcademicInformationLogic.Modify(studentAcademicInformation);
                        }
                    }

                    SetMessage("Operation Successful!", Message.Category.Information);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("AddGraduationDate");
        }
    }
}