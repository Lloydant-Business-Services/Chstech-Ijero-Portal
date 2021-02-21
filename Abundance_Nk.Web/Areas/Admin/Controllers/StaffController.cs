using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.IO;
using System.Data.OleDb;
using System.Transactions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StaffController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private StaffViewModel viewModel;
        private string FileUploadURL = null;

        protected const string ArgumentNullException = "Null object argument. Please contact your system administrator";
        protected const string UpdateException = "Operation failed due to update exception!";
        protected const string NoItemModified = "No item modified!";
        protected const string NoItemFound = "No item found to modified!";
        protected const string NoItemRemoved = "No item removed!";
        protected const string ErrowDuringProccesing = "Error Occurred During Processing.";
        protected const string ContainsDuplicate = "Error Occurred, the data contains duplicates, Please try again or contact ICT";

        // GET: Admin/Staff
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Download()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {

                ViewBag.Session = staffViewModel.SessionSelectList;
                ViewBag.CourseMode = staffViewModel.CourseModeSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult Download(StaffViewModel staffViewModel)
        {

            try
            {

                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                staffViewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Level_Id == staffViewModel.Level.Id && p.COURSE.Semester_Id == staffViewModel.Semester.Id && p.Session_Id == staffViewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(staffViewModel);
                TempData["vModel"] = staffViewModel;

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        public ActionResult ResultUploadSheet(long cid, int courseModeId)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == cid);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("Download");
                }
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<NewResultFormat> resultFormatList = new List<NewResultFormat>();
                List<NewResultFormat> resultFormatList2 = new List<NewResultFormat>();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                //resultFormatList2 = courseRegistrationLogic.GetDownloadResultFormats(courseAllocation, courseModeId);
                resultFormatList2 = courseRegistrationLogic.GetDownloadResultNewFormats(courseAllocation);

                if (resultFormatList2 != null && resultFormatList2.Count > 0)
                {
                    List<string> distinctMatricNumber = resultFormatList2.Select(r => r.MATRICNO).Distinct().ToList();

                    for (int i = 0; i < distinctMatricNumber.Count; i++)
                    {
                        resultFormatList.Add(resultFormatList2.Where(r => r.MATRICNO == distinctMatricNumber[i]).LastOrDefault());
                    }
                }

                if (resultFormatList.Count > 0)
                {
                    List<NewResultFormat> list = resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    List<NewResultFormat> sort = new List<NewResultFormat>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort;// resultFormatList.OrderBy(p => p.MATRICNO);
                    CourseLogic courseLogic = new CourseLogic();
                    Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                    gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " + course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";

                    gv.DataBind();

                    string filename = course.Code.Replace("/", "").Replace("\\", "") + course.Department.Code + ".xls";
                    return new DownloadFileActionResult(gv, filename);
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
            return RedirectToAction("Download");
        }

        public ActionResult ResultSheetDownload(long cid, int courseModeId)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == cid);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("Download");
                }
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<CBTResultFormat> resultFormatList = new List<CBTResultFormat>();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                resultFormatList = courseRegistrationLogic.GetCBTDownloadResultFormats(courseAllocation, courseModeId);
                if (resultFormatList.Count > 0)
                {
                    List<CBTResultFormat> list = resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    List<CBTResultFormat> sort = new List<CBTResultFormat>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort;// resultFormatList.OrderBy(p => p.MATRICNO);
                    CourseLogic courseLogic = new CourseLogic();
                    Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                    gv.DataBind();

                    string filename = course.Code.Replace("/", "").Replace("\\", "") + course.Department.Code + ".xls";
                    return new DownloadFileActionResult(gv, filename);
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
            return RedirectToAction("Download");
        }

        public ActionResult Upload()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {

                ViewBag.Session = staffViewModel.SessionSelectList;
                ViewBag.CourseMode = staffViewModel.CourseModeSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult Upload(StaffViewModel staffViewModel)
        {

            try
            {
                //SetMessage("Result upload disabled temporarily, kindly check back later.", Message.Category.Warning);
                //return RedirectToAction("Upload");

                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                staffViewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Level_Id == staffViewModel.Level.Id && p.COURSE.Semester_Id == staffViewModel.Semester.Id && p.Session_Id == staffViewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(staffViewModel);
                TempData["vModel"] = staffViewModel;

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        public ActionResult ProcessUpload()
        {
            StaffViewModel viewModel = (StaffViewModel)TempData["staffViewModel"];
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
                        return RedirectToAction("Upload");
                    }

                    if (viewModel.IsAlternate == false)
                    {
                        bool resultAdditionStatus = addStudentResult(viewModel);
                    }
                    else
                    {
                        bool resultAdditionStatus = AddUnregisteredStudentResult(viewModel);
                    }

                    SetMessage("Upload successful", Message.Category.Information);
                }


            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }
            RetainDropdownState(viewModel);
            return RedirectToAction("Upload");
        }
        [HttpPost]
        public ActionResult ProcessUpload(StaffViewModel staffViewModel)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == staffViewModel.Cid);
                CourseLogic courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("Upload");
                }
                List<ResultFormat> resultFormatList = new List<ResultFormat>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    FileUploadURL = savedFileName;
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
                            resultFormat.fileUploadUrl = savedFileName;
                            if (resultFormat.MATRICNO != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }

                        }
                        resultFormatList.OrderBy(p => p.MATRICNO);
                        staffViewModel.resultFormatList = resultFormatList;
                        staffViewModel.Course = course;
                        StaffViewModel vModel = (StaffViewModel)TempData["vModel"];
                        vModel.Programme = staffViewModel.Programme;
                        vModel.IsAlternate = staffViewModel.IsAlternate;
                        vModel.Course = course;
                        vModel.resultFormatList = resultFormatList;
                        vModel.CourseAllocation = courseAllocation;
                        TempData["staffViewModel"] = vModel;

                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }

            RetainDropdownState(staffViewModel);
            return View(staffViewModel);
        }
        public void KeepDropDownState(StaffViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.User = viewModel.UserSelectList;
                if (viewModel.Semester != null)
                {
                    List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

                    List<Semester> semesters = new List<Semester>();
                    foreach (SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters, ID, NAME, viewModel.Semester.Id);
                }
                if (viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, ID, NAME, viewModel.Department.Id);
                }
                if (viewModel.Course != null && viewModel.Course.Id > 0)
                {
                    List<Course> courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level, viewModel.Department, viewModel.Semester);

                    ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Course.Id);
                }

                if (viewModel.CourseMode != null && viewModel.CourseMode.Id > 0)
                {
                    ViewBag.CourseMode = new SelectList(viewModel.CourseModeSelectList, "Value", "Text", viewModel.CourseMode.Id);
                }
                else
                {
                    ViewBag.CourseMode = viewModel.CourseModeSelectList;
                }

                if (viewModel.StudentResultType != null && viewModel.StudentResultType.Id > 0)
                {
                    ViewBag.ResultType = new SelectList(viewModel.ResultTypeSelectList, "Value", "Text", viewModel.StudentResultType.Id);
                }
                else
                {
                    ViewBag.ResultType = viewModel.ResultTypeSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
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
        public void RetainDropdownState(StaffViewModel viewModel)
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

                        ViewBag.Session = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true), ID, NAME, viewModel.Session.Id);
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

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public enum SpeicalCaseCodes
        {
            Sick = 101,
            Absent = 201,
            Other = 301
        }
        private static void AssignSpecialCaseRemarks(List<ResultFormat> list, int i, decimal testScore, decimal inputExamScore)
        {
            try
            {
                if (testScore == (decimal)SpeicalCaseCodes.Sick)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "SICK: TEST";
                    list[i].ResultSpecialCaseMessages.TestSpecialCaseMessage = "SICK: TEST";
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
                        if (testScore > 100 || inputExamScore > 100)
                        {
                            AssignSpecialCaseRemarks(list, i, testScore, inputExamScore);
                        }
                        else
                        {
                            decimal calculatedExamScore = list[i].QU1 + list[i].QU2 + list[i].QU3 + list[i].QU4 + list[i].QU5 + list[i].QU6 + list[i].QU7 + list[i].QU8 + list[i].QU9;
                            decimal inputTotalScore = list[i].EX_CA;
                            decimal calculatedTotalScore = list[i].T_CA + list[i].T_EX;
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
        private bool addStudentResult(StaffViewModel viewModel)
        {
            try
            {
                viewModel = (StaffViewModel)TempData["staffViewModel"];
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester = sessionSemesterLogicc.GetModelBy(p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    CourseRegistration courseRegistration = new CourseRegistration();
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
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


                    string operation = "MODIFY";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    courseRegistrationDetailAudit.User = user;
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            InitializeStudentResult(viewModel, "ADMIN", sessionSemester, testResultType, examResultType, user, out studentResultDetailTest, out studentResultDetailExam, out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic, out studentResultTest, out studentResultExam);
                            studentResultDetailTest.Course = viewModel.Course;

                            courseRegistration = courseRegistrationLogic.GetModelsBy(c => c.STUDENT.Matric_Number == resultFormat.MATRICNO.Trim() && c.Session_Id == viewModel.Session.Id).LastOrDefault();

                            if (courseRegistration != null)
                            {
                                studentResultDetailTest.Student = studentLogic.GetModelBy(p => p.Matric_Number == resultFormat.MATRICNO.Trim() && p.Person_Id == courseRegistration.Student.Id);
                                if (studentResultDetailTest.Student != null)
                                {
                                    studentResultDetailTest.Score = resultFormat.T_CA;
                                    studentResultDetailTest.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage;
                                    studentResultDetailTestList.Add(studentResultDetailTest);
                                    studentResultTest.Results = studentResultDetailTestList;
                                    studentResultLogic.Add(studentResultTest, courseRegistrationDetailAudit);

                                    studentResultDetailExam.Course = viewModel.Course;
                                    studentResultDetailExam.Student = studentResultDetailTest.Student;
                                    studentResultDetailExam.Score = resultFormat.T_EX;
                                    studentResultDetailExam.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.ExamSpecialCaseMessage;
                                    studentResultDetailExamList.Add(studentResultDetailExam);
                                    studentResultExam.Results = studentResultDetailExamList;
                                    studentResultLogic.Add(studentResultExam, courseRegistrationDetailAudit);

                                    studentExamRawScoreSheet = StudentExamRawScoreSheetLogic.GetModelBy(p => p.Student_Id == studentResultDetailExam.Student.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);
                                    List<StudentExamRawScoreSheet> a = StudentExamRawScoreSheetLogic.GetModelsBy(p => p.Student_Id == studentResultDetailExam.Student.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);

                                    if (studentExamRawScoreSheet == null)
                                    {
                                        studentExamRawScoreSheet = CreateExamRawScoreSheet(viewModel, user, studentResultDetailTest, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
                                    }
                                    else
                                    {
                                        ModifyExamRawScoreSheet(viewModel, user, studentResultDetailTest, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
                                    }
                                }
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
        private static void InitializeStudentResult(StaffViewModel viewModel, string FileUploadURL, SessionSemester sessionSemester, StudentResultType testResultType, StudentResultType examResultType, User user, out StudentResultDetail studentResultDetailTest, out StudentResultDetail studentResultDetailExam, out List<StudentResultDetail> studentResultDetailTestList, out List<StudentResultDetail> studentResultDetailExamList, out StudentResultLogic studentResultLogic, out StudentResult studentResultTest, out StudentResult studentResultExam)
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
            studentResultTest.Department = viewModel.Course.Department;
            studentResultTest.Level = viewModel.CourseAllocation.Level;
            studentResultTest.Programme = viewModel.CourseAllocation.Programme;
            studentResultTest.SessionSemester = sessionSemester;
            studentResultTest.UploadedFileUrl = FileUploadURL;
            studentResultTest.Uploader = user;
            studentResultTest.Type = testResultType;

            studentResultExam.MaximumObtainableScore = 70;
            studentResultExam.DateUploaded = DateTime.Now;
            studentResultExam.Department = viewModel.Course.Department;
            studentResultExam.Level = viewModel.CourseAllocation.Level;
            studentResultExam.Programme = viewModel.CourseAllocation.Programme;
            studentResultExam.SessionSemester = sessionSemester;
            studentResultExam.UploadedFileUrl = FileUploadURL;
            studentResultExam.Uploader = user;
            studentResultExam.Type = examResultType;
        }
        private static void ModifyExamRawScoreSheet(StaffViewModel viewModel, User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
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

        }
        private static StudentExamRawScoreSheet CreateExamRawScoreSheet(StaffViewModel viewModel, User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
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
        private bool AddUnregisteredStudentResult(StaffViewModel viewModel)
        {
            try
            {
                viewModel = (StaffViewModel)TempData["staffViewModel"];
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester = sessionSemesterLogicc.GetModelBy(p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {

                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetNotRegisteredLogic StudentExamRawScoreSheetLogic = new StudentExamRawScoreSheetNotRegisteredLogic();

                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            studentExamRawScoreSheet = StudentExamRawScoreSheetLogic.GetModelBy(p => p.Student_Matric_Number == resultFormat.MATRICNO && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);
                            if (studentExamRawScoreSheet == null)
                            {
                                studentExamRawScoreSheet = CreateUnregisteredStudentExamRawScoreSheet(viewModel, user, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat, resultFormat.fileUploadUrl);
                            }
                            else
                            {
                                ModifyUnregisteredStudentExamRawScoreSheet(viewModel, user, studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat, resultFormat.fileUploadUrl);
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
        private static StudentExamRawScoreSheet CreateUnregisteredStudentExamRawScoreSheet(StaffViewModel viewModel, User user, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic, ResultFormat resultFormat, string fileURL)
        {
            //confirm that student exists on the portal
            StudentLevel studentLevel = new StudentLevel();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            //studentLevel = studentLevelLogic.GetBy(resultFormat.MATRICNO);
            studentLevel = studentLevelLogic.GetModelsBy(s => s.STUDENT.Matric_Number == resultFormat.MATRICNO && s.Session_Id == viewModel.Session.Id).LastOrDefault();

            if (studentLevel == null)
            {
                studentLevel = studentLevelLogic.GetModelsBy(s => s.STUDENT.Matric_Number == resultFormat.MATRICNO).LastOrDefault();
            }

            //confirm that student is in the department that is to be uploaded
            CourseLogic courseLogic = new CourseLogic();
            viewModel.Course = courseLogic.GetBy(viewModel.Course.Id);

            if (studentLevel != null && viewModel.Course != null)
            {
                if (viewModel.Course.Department.Id == studentLevel.Department.Id && viewModel.Programme.Id == studentLevel.Programme.Id)
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
                    studentExamRawScoreSheet.MatricNumber = resultFormat.MATRICNO;
                    studentExamRawScoreSheet.Uploader = user;
                    studentExamRawScoreSheet.FileUploadURL = fileURL;
                    studentExamRawScoreSheet.Programme = viewModel.Programme;
                    studentExamRawScoreSheetNotRegisteredLogic.Create(studentExamRawScoreSheet);
                    return studentExamRawScoreSheet;
                }

            }
            return null;


        }
        private static void ModifyUnregisteredStudentExamRawScoreSheet(StaffViewModel viewModel, User user, StudentExamRawScoreSheet studentExamRawScoreSheet, StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic, ResultFormat resultFormat, string fileURL)
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
            studentExamRawScoreSheet.MatricNumber = resultFormat.MATRICNO;
            studentExamRawScoreSheet.Uploader = user;
            studentExamRawScoreSheet.FileUploadURL = fileURL;
            bool isScoreSheetModified = studentExamRawScoreSheetNotRegisteredLogic.Modify(studentExamRawScoreSheet);
        }
        public ActionResult ViewResultSheet()
        {
            StaffViewModel viewModel = new StaffViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.CourseMode = viewModel.CourseModeSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewResultSheet(StaffViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);

                if (viewModel.CourseAllocations.Count <= 0)
                {
                    SetMessage("Sorry!, You were not allocated any course in this Level for the selected Session-Semester! ", Message.Category.Error);
                }

                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            KeepDropDownState(viewModel);
            return View(viewModel);
        }

        public ActionResult ResultSheetReport(int cid, int courseModeId)
        {
            StaffViewModel viewModel = (StaffViewModel)TempData["vModel"];
            try
            {
                if (viewModel != null)
                {
                    CourseAllocation courseAllocation = viewModel.CourseAllocations.Where(c => c.Id == cid).FirstOrDefault();
                    viewModel.CourseAllocation = courseAllocation;
                    viewModel.Cid = cid;
                    viewModel.courseModeId = courseModeId;

                    TempData["viewModel"] = viewModel;

                    return RedirectToAction("ResultSheetAlt", "Report");
                }

            }
            catch (Exception)
            {
                throw;
            }

            KeepDropDownState(viewModel);
            return View("ViewResultSheet", viewModel);
        }

        public ActionResult NewResultUpload()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {
                if (TempData["FailedResult"] != null)
                {
                    staffViewModel.FailedResultFormatList = (List<ResultFormat>)TempData["FailedResult"];
                }

                ViewBag.Session = staffViewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.CourseMode = staffViewModel.CourseModeSelectList;
                ViewBag.ResultType = staffViewModel.ResultTypeSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);
                ViewBag.Programme = staffViewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult NewResultUpload(StaffViewModel staffViewModel)
        {
            try
            {
                List<ResultFormat> resultFormatList = new List<ResultFormat>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    FileUploadURL = savedFileName;
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if (studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < studentSet.Tables[0].Rows.Count; i++)
                        {
                            ResultFormat resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MATRICNO = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.EX_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][3].ToString().Trim());
                            resultFormat.fileUploadUrl = savedFileName;
                            if (resultFormat.MATRICNO != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }

                        }

                        CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                        staffViewModel.CourseAllocation = courseAllocationLogic.GetModelsBy(c => c.Session_Id == staffViewModel.Session.Id && c.Course_Id == staffViewModel.Course.Id && c.Department_Id == staffViewModel.Department.Id && c.Level_Id == staffViewModel.Level.Id && c.Programme_Id == staffViewModel.Programme.Id && c.Semester_Id == staffViewModel.Semester.Id).LastOrDefault();

                        staffViewModel.resultFormatList = resultFormatList;

                        TempData["staffViewModel"] = staffViewModel;
                    }

                }

                //KeepDropDownState(staffViewModel);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }

            KeepDropDownState(staffViewModel);
            return View(staffViewModel);
        }
        public ActionResult SaveNewUploadedResult()
        {
            StaffViewModel viewModel = (StaffViewModel)TempData["staffViewModel"];
            try
            {
                if (viewModel != null)
                {
                    bool status = true;
                    //int status = validateFields(viewModel.resultFormatList);
                    for (int i = 0; i < viewModel.resultFormatList.Count; i++)
                    {
                        if (viewModel.StudentResultType != null && viewModel.StudentResultType.Id == 1)
                        {
                            if (viewModel.resultFormatList[i].EX_CA < 0 || viewModel.resultFormatList[i].EX_CA > 30)
                            {
                                status = false;
                            }
                        }
                        else if (viewModel.StudentResultType != null && viewModel.StudentResultType.Id == 2)
                        {
                            if (viewModel.resultFormatList[i].EX_CA < 0 || viewModel.resultFormatList[i].EX_CA > 70)
                            {
                                status = false;
                            }
                        }
                    }

                    if (!status)
                    {
                        SetMessage("Validation Error for one or more entries, cross check and re-upload!", Message.Category.Error);
                        KeepDropDownState(viewModel);
                        return RedirectToAction("NewResultUpload");
                    }


                    bool resultAdditionStatus = SaveNewStudentResult(viewModel);

                    SetMessage("Upload successful", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }

            KeepDropDownState(viewModel);
            return RedirectToAction("NewResultUpload");
        }
        private bool SaveNewStudentResult(StaffViewModel viewModel)
        {
            try
            {
                List<ResultFormat> failedResults = new List<ResultFormat>();
                UserLogic userLogic = new UserLogic();
                StudentLogic studentLogic = new StudentLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                CourseLogic courseLogic = new CourseLogic();

                viewModel.Course = courseLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id);
                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id && s.Semester_Id == viewModel.Semester.Id);
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);

                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };

                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    CourseRegistration courseRegistration = new CourseRegistration();
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetLogic = new StudentExamRawScoreSheetResultLogic();

                    string operation = "MODIFY";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    courseRegistrationDetailAudit.User = user;
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            courseRegistration = courseRegistrationLogic.GetModelsBy(c => c.STUDENT.Matric_Number == resultFormat.MATRICNO.Trim() && c.Session_Id == viewModel.Session.Id).LastOrDefault();
                            if (courseRegistration != null)
                            {
                                courseRegistrationDetail = courseRegistrationDetailLogic.GetModelsBy(c => c.Course_Id == viewModel.Course.Id && c.Student_Course_Registration_Id == courseRegistration.Id && c.Semester_Id == viewModel.Semester.Id).LastOrDefault();

                                if (courseRegistrationDetail != null)
                                {
                                    if (viewModel.StudentResultType.Id == 1)
                                    {
                                        courseRegistrationDetail.TestScore = resultFormat.EX_CA;
                                    }
                                    else if (viewModel.StudentResultType.Id == 2)
                                    {
                                        courseRegistrationDetail.ExamScore = resultFormat.EX_CA;
                                    }

                                    courseRegistrationDetailLogic.Modify(courseRegistrationDetail, courseRegistrationDetailAudit);

                                    studentExamRawScoreSheet = studentExamRawScoreSheetLogic.GetModelBy(p => p.Student_Id == courseRegistration.Student.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);

                                    if (studentExamRawScoreSheet == null)
                                    {
                                        studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                                        studentExamRawScoreSheet.Course = viewModel.Course;
                                        studentExamRawScoreSheet.EX_CA = Convert.ToDouble(resultFormat.EX_CA);
                                        studentExamRawScoreSheet.Level = viewModel.Level;
                                        studentExamRawScoreSheet.MatricNumber = resultFormat.MATRICNO.Trim();
                                        studentExamRawScoreSheet.Programme = viewModel.Programme;
                                        if (viewModel.StudentResultType.Id == 2)
                                        {
                                            studentExamRawScoreSheet.QU1 = Convert.ToDouble(resultFormat.EX_CA);
                                            studentExamRawScoreSheet.T_EX = Convert.ToDouble(resultFormat.EX_CA);
                                        }
                                        else if (viewModel.StudentResultType.Id == 1)
                                        {
                                            studentExamRawScoreSheet.T_CA = Convert.ToDouble(resultFormat.EX_CA);
                                        }
                                        studentExamRawScoreSheet.Remark = null;
                                        studentExamRawScoreSheet.Semester = viewModel.Semester;
                                        studentExamRawScoreSheet.Session = viewModel.Session;
                                        studentExamRawScoreSheet.Special_Case = null;
                                        studentExamRawScoreSheet.Student = courseRegistration.Student;
                                        studentExamRawScoreSheet.Uploader = user;

                                        studentExamRawScoreSheet.T_CA = studentExamRawScoreSheet.T_CA ?? Convert.ToDouble(resultFormat.T_CA);
                                        studentExamRawScoreSheet.T_EX = studentExamRawScoreSheet.T_EX ?? Convert.ToDouble(resultFormat.T_EX);
                                        studentExamRawScoreSheet.QU1 = studentExamRawScoreSheet.QU1 ?? Convert.ToDouble(resultFormat.QU1);
                                        studentExamRawScoreSheet.QU2 = studentExamRawScoreSheet.QU2 ?? Convert.ToDouble(resultFormat.QU2);
                                        studentExamRawScoreSheet.QU3 = studentExamRawScoreSheet.QU3 ?? Convert.ToDouble(resultFormat.QU3);
                                        studentExamRawScoreSheet.QU4 = studentExamRawScoreSheet.QU4 ?? Convert.ToDouble(resultFormat.QU4);
                                        studentExamRawScoreSheet.QU5 = studentExamRawScoreSheet.QU5 ?? Convert.ToDouble(resultFormat.QU5);
                                        studentExamRawScoreSheet.QU6 = studentExamRawScoreSheet.QU6 ?? Convert.ToDouble(resultFormat.QU6);
                                        studentExamRawScoreSheet.QU7 = studentExamRawScoreSheet.QU7 ?? Convert.ToDouble(resultFormat.QU7);
                                        studentExamRawScoreSheet.QU8 = studentExamRawScoreSheet.QU8 ?? Convert.ToDouble(resultFormat.QU8);
                                        studentExamRawScoreSheet.QU9 = studentExamRawScoreSheet.QU9 ?? Convert.ToDouble(resultFormat.QU9);

                                        studentExamRawScoreSheetLogic.Create(studentExamRawScoreSheet);


                                        StudentResultDetail studentResultDetailTest;
                                        StudentResultDetail studentResultDetailExam;
                                        List<StudentResultDetail> studentResultDetailTestList;
                                        List<StudentResultDetail> studentResultDetailExamList;
                                        StudentResultLogic studentResultLogic;
                                        StudentResult studentResultTest;
                                        StudentResult studentResultExam;

                                        InitializeStudentResult(viewModel, "ADMIN", sessionSemester, testResultType, examResultType, user, out studentResultDetailTest, out studentResultDetailExam, out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic, out studentResultTest, out studentResultExam);
                                        studentResultDetailTest.Course = viewModel.Course;

                                        studentResultDetailTest.Student = studentLogic.GetModelBy(p => p.Matric_Number == resultFormat.MATRICNO.Trim() && p.Person_Id == courseRegistration.Student.Id);

                                        if (studentResultDetailTest.Student != null)
                                        {
                                            studentResultDetailTest.Score = Convert.ToDecimal(studentExamRawScoreSheet.T_CA);
                                            if (resultFormat.ResultSpecialCaseMessages != null && resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage != null)
                                            {
                                                studentResultDetailTest.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage;
                                            }
                                            else
                                            {
                                                studentResultDetailTest.SpecialCaseMessage = null;
                                            }
                                            studentResultDetailTestList.Add(studentResultDetailTest);
                                            studentResultTest.Results = studentResultDetailTestList;
                                            if (viewModel.StudentResultType.Id == 1)
                                            {
                                                studentResultLogic.Add(studentResultTest, courseRegistrationDetailAudit);
                                            }

                                            studentResultDetailExam.Course = viewModel.Course;
                                            studentResultDetailExam.Student = studentResultDetailTest.Student;
                                            studentResultDetailExam.Score = Convert.ToDecimal(studentExamRawScoreSheet.T_EX);

                                            if (resultFormat.ResultSpecialCaseMessages != null && resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage != null)
                                            {
                                                studentResultDetailExam.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.ExamSpecialCaseMessage;
                                            }
                                            else
                                            {
                                                studentResultDetailExam.SpecialCaseMessage = null;
                                            }

                                            studentResultDetailExamList.Add(studentResultDetailExam);
                                            studentResultExam.Results = studentResultDetailExamList;
                                            if (viewModel.StudentResultType.Id == 2)
                                            {
                                                studentResultLogic.Add(studentResultExam, courseRegistrationDetailAudit);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (viewModel.StudentResultType.Id == 2)
                                        {
                                            studentExamRawScoreSheet.QU1 = Convert.ToDouble(resultFormat.EX_CA);
                                            studentExamRawScoreSheet.T_EX = Convert.ToDouble(resultFormat.EX_CA);
                                        }
                                        else if (viewModel.StudentResultType.Id == 1)
                                        {
                                            studentExamRawScoreSheet.T_CA = Convert.ToDouble(resultFormat.EX_CA);
                                        }

                                        if (studentExamRawScoreSheet.T_CA == null)
                                        {
                                            studentExamRawScoreSheet.EX_CA = 0 + studentExamRawScoreSheet.T_EX;
                                        }
                                        else
                                        {
                                            studentExamRawScoreSheet.EX_CA = studentExamRawScoreSheet.T_CA + studentExamRawScoreSheet.T_EX;
                                        }

                                        if (studentExamRawScoreSheet.QU2 == null)
                                        {
                                            studentExamRawScoreSheet.QU2 = Convert.ToDouble(resultFormat.QU2);
                                        }

                                        studentExamRawScoreSheet.T_CA = studentExamRawScoreSheet.T_CA ?? Convert.ToDouble(resultFormat.T_CA);
                                        studentExamRawScoreSheet.T_EX = studentExamRawScoreSheet.T_EX ?? Convert.ToDouble(resultFormat.T_EX);
                                        studentExamRawScoreSheet.QU1 = studentExamRawScoreSheet.QU1 ?? Convert.ToDouble(resultFormat.QU1);
                                        studentExamRawScoreSheet.QU2 = studentExamRawScoreSheet.QU2 ?? Convert.ToDouble(resultFormat.QU2);
                                        studentExamRawScoreSheet.QU3 = studentExamRawScoreSheet.QU3 ?? Convert.ToDouble(resultFormat.QU3);
                                        studentExamRawScoreSheet.QU4 = studentExamRawScoreSheet.QU4 ?? Convert.ToDouble(resultFormat.QU4);
                                        studentExamRawScoreSheet.QU5 = studentExamRawScoreSheet.QU5 ?? Convert.ToDouble(resultFormat.QU5);
                                        studentExamRawScoreSheet.QU6 = studentExamRawScoreSheet.QU6 ?? Convert.ToDouble(resultFormat.QU6);
                                        studentExamRawScoreSheet.QU7 = studentExamRawScoreSheet.QU7 ?? Convert.ToDouble(resultFormat.QU7);
                                        studentExamRawScoreSheet.QU8 = studentExamRawScoreSheet.QU8 ?? Convert.ToDouble(resultFormat.QU8);
                                        studentExamRawScoreSheet.QU9 = studentExamRawScoreSheet.QU9 ?? Convert.ToDouble(resultFormat.QU9);

                                        studentExamRawScoreSheetLogic.Modify(studentExamRawScoreSheet);

                                        StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                                        StudentResultDetail studentResultDetail = studentResultDetailLogic.GetModelsBy(s => s.Person_Id == courseRegistration.Student.Id && s.STUDENT_RESULT.Session_Semester_Id == sessionSemester.Id && s.Course_Id == viewModel.Course.Id).LastOrDefault();

                                        if (studentResultDetail == null)
                                        {
                                            StudentResultDetail studentResultDetailTest;
                                            StudentResultDetail studentResultDetailExam;
                                            List<StudentResultDetail> studentResultDetailTestList;
                                            List<StudentResultDetail> studentResultDetailExamList;
                                            StudentResultLogic studentResultLogic;
                                            StudentResult studentResultTest;
                                            StudentResult studentResultExam;

                                            InitializeStudentResult(viewModel, "ADMIN", sessionSemester, testResultType, examResultType, user, out studentResultDetailTest, out studentResultDetailExam, out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic, out studentResultTest, out studentResultExam);
                                            studentResultDetailTest.Course = viewModel.Course;

                                            studentResultDetailTest.Student = studentLogic.GetModelBy(p => p.Matric_Number == resultFormat.MATRICNO.Trim() && p.Person_Id == courseRegistration.Student.Id);

                                            if (studentResultDetailTest.Student != null)
                                            {
                                                studentResultDetailTest.Score = Convert.ToDecimal(studentExamRawScoreSheet.T_CA);
                                                if (resultFormat.ResultSpecialCaseMessages != null && resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage != null)
                                                {
                                                    studentResultDetailTest.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage;
                                                }
                                                else
                                                {
                                                    studentResultDetailTest.SpecialCaseMessage = null;
                                                }
                                                studentResultDetailTestList.Add(studentResultDetailTest);
                                                studentResultTest.Results = studentResultDetailTestList;
                                                if (viewModel.StudentResultType.Id == 1)
                                                {
                                                    studentResultLogic.Add(studentResultTest, courseRegistrationDetailAudit);
                                                }

                                                studentResultDetailExam.Course = viewModel.Course;
                                                studentResultDetailExam.Student = studentResultDetailTest.Student;
                                                studentResultDetailExam.Score = Convert.ToDecimal(studentExamRawScoreSheet.T_EX);

                                                if (resultFormat.ResultSpecialCaseMessages != null && resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage != null)
                                                {
                                                    studentResultDetailExam.SpecialCaseMessage = resultFormat.ResultSpecialCaseMessages.ExamSpecialCaseMessage;
                                                }
                                                else
                                                {
                                                    studentResultDetailExam.SpecialCaseMessage = null;
                                                }

                                                studentResultDetailExamList.Add(studentResultDetailExam);
                                                studentResultExam.Results = studentResultDetailExamList;
                                                if (viewModel.StudentResultType.Id == 2)
                                                {
                                                    studentResultLogic.Add(studentResultExam, courseRegistrationDetailAudit);
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    resultFormat.ResultSpecialCaseMessages = new ResultSpecialCaseMessages() { SpecialCaseMessage = "Student did not register for this course" };

                                    failedResults.Add(resultFormat);
                                }
                            }
                            else
                            {
                                StudentExamRawScoreSheetNotRegisteredLogic scoreSheetNotRegisteredLogic = new StudentExamRawScoreSheetNotRegisteredLogic();


                                studentExamRawScoreSheet = scoreSheetNotRegisteredLogic.GetModelBy(p => p.Student_Matric_Number == resultFormat.MATRICNO.Trim() && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.Course_Id == viewModel.Course.Id);

                                if (studentExamRawScoreSheet == null)
                                {
                                    studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                                    studentExamRawScoreSheet.Course = viewModel.Course;
                                    studentExamRawScoreSheet.EX_CA = Convert.ToDouble(resultFormat.EX_CA);
                                    studentExamRawScoreSheet.Level = viewModel.Level;
                                    studentExamRawScoreSheet.MatricNumber = resultFormat.MATRICNO.Trim();
                                    studentExamRawScoreSheet.Programme = viewModel.Programme;
                                    if (viewModel.StudentResultType.Id == 2)
                                    {
                                        studentExamRawScoreSheet.QU1 = Convert.ToDouble(resultFormat.EX_CA);
                                        studentExamRawScoreSheet.T_EX = Convert.ToDouble(resultFormat.EX_CA);
                                    }
                                    else if (viewModel.StudentResultType.Id == 1)
                                    {
                                        studentExamRawScoreSheet.T_CA = Convert.ToDouble(resultFormat.EX_CA);
                                    }

                                    studentExamRawScoreSheet.Remark = null;
                                    studentExamRawScoreSheet.Semester = viewModel.Semester;
                                    studentExamRawScoreSheet.Session = viewModel.Session;
                                    studentExamRawScoreSheet.Special_Case = null;
                                    //studentExamRawScoreSheet.Student = courseRegistration.Student;
                                    studentExamRawScoreSheet.Uploader = user;

                                    studentExamRawScoreSheet.T_CA = studentExamRawScoreSheet.T_CA ?? Convert.ToDouble(resultFormat.T_CA);
                                    studentExamRawScoreSheet.T_EX = studentExamRawScoreSheet.T_EX ?? Convert.ToDouble(resultFormat.T_EX);
                                    studentExamRawScoreSheet.QU1 = studentExamRawScoreSheet.QU1 ?? Convert.ToDouble(resultFormat.QU1);
                                    studentExamRawScoreSheet.QU2 = studentExamRawScoreSheet.QU2 ?? Convert.ToDouble(resultFormat.QU2);
                                    studentExamRawScoreSheet.QU3 = studentExamRawScoreSheet.QU3 ?? Convert.ToDouble(resultFormat.QU3);
                                    studentExamRawScoreSheet.QU4 = studentExamRawScoreSheet.QU4 ?? Convert.ToDouble(resultFormat.QU4);
                                    studentExamRawScoreSheet.QU5 = studentExamRawScoreSheet.QU5 ?? Convert.ToDouble(resultFormat.QU5);
                                    studentExamRawScoreSheet.QU6 = studentExamRawScoreSheet.QU6 ?? Convert.ToDouble(resultFormat.QU6);
                                    studentExamRawScoreSheet.QU7 = studentExamRawScoreSheet.QU7 ?? Convert.ToDouble(resultFormat.QU7);
                                    studentExamRawScoreSheet.QU8 = studentExamRawScoreSheet.QU8 ?? Convert.ToDouble(resultFormat.QU8);
                                    studentExamRawScoreSheet.QU9 = studentExamRawScoreSheet.QU9 ?? Convert.ToDouble(resultFormat.QU9);

                                    scoreSheetNotRegisteredLogic.Create(studentExamRawScoreSheet);

                                }
                                else
                                {
                                    if (viewModel.StudentResultType.Id == 2)
                                    {
                                        studentExamRawScoreSheet.QU1 = Convert.ToDouble(resultFormat.EX_CA);
                                        studentExamRawScoreSheet.T_EX = Convert.ToDouble(resultFormat.EX_CA);
                                    }
                                    else if (viewModel.StudentResultType.Id == 1)
                                    {
                                        studentExamRawScoreSheet.T_CA = Convert.ToDouble(resultFormat.EX_CA);
                                    }

                                    if (studentExamRawScoreSheet.T_CA == null)
                                    {
                                        studentExamRawScoreSheet.EX_CA = 0 + studentExamRawScoreSheet.T_EX;
                                    }
                                    else
                                    {
                                        studentExamRawScoreSheet.EX_CA = studentExamRawScoreSheet.T_CA + studentExamRawScoreSheet.T_EX;
                                    }

                                    if (studentExamRawScoreSheet.QU2 == null)
                                    {
                                        studentExamRawScoreSheet.QU2 = Convert.ToDouble(resultFormat.QU2);
                                    }

                                    studentExamRawScoreSheet.T_CA = studentExamRawScoreSheet.T_CA ?? Convert.ToDouble(resultFormat.T_CA);
                                    studentExamRawScoreSheet.T_EX = studentExamRawScoreSheet.T_EX ?? Convert.ToDouble(resultFormat.T_EX);
                                    studentExamRawScoreSheet.QU1 = studentExamRawScoreSheet.QU1 ?? Convert.ToDouble(resultFormat.QU1);
                                    studentExamRawScoreSheet.QU2 = studentExamRawScoreSheet.QU2 ?? Convert.ToDouble(resultFormat.QU2);
                                    studentExamRawScoreSheet.QU3 = studentExamRawScoreSheet.QU3 ?? Convert.ToDouble(resultFormat.QU3);
                                    studentExamRawScoreSheet.QU4 = studentExamRawScoreSheet.QU4 ?? Convert.ToDouble(resultFormat.QU4);
                                    studentExamRawScoreSheet.QU5 = studentExamRawScoreSheet.QU5 ?? Convert.ToDouble(resultFormat.QU5);
                                    studentExamRawScoreSheet.QU6 = studentExamRawScoreSheet.QU6 ?? Convert.ToDouble(resultFormat.QU6);
                                    studentExamRawScoreSheet.QU7 = studentExamRawScoreSheet.QU7 ?? Convert.ToDouble(resultFormat.QU7);
                                    studentExamRawScoreSheet.QU8 = studentExamRawScoreSheet.QU8 ?? Convert.ToDouble(resultFormat.QU8);
                                    studentExamRawScoreSheet.QU9 = studentExamRawScoreSheet.QU9 ?? Convert.ToDouble(resultFormat.QU9);

                                    scoreSheetNotRegisteredLogic.Modify(studentExamRawScoreSheet);
                                }

                                resultFormat.ResultSpecialCaseMessages = new ResultSpecialCaseMessages() { SpecialCaseMessage = "Student has not registered for this session." };

                                failedResults.Add(resultFormat);
                            }

                        }
                        scope.Complete();
                    }


                }

                TempData["FailedResult"] = failedResults;

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult UploadCA()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {

                ViewBag.Session = staffViewModel.SessionSelectList;
                ViewBag.CourseMode = staffViewModel.CourseModeSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult UploadCA(StaffViewModel staffViewModel)
        {

            try
            {

                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                staffViewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Level_Id == staffViewModel.Level.Id && p.COURSE.Semester_Id == staffViewModel.Semester.Id && p.Session_Id == staffViewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(staffViewModel);
                TempData["vModel"] = staffViewModel;

            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        public ActionResult ProcessUploadCA()
        {
            StaffViewModel viewModel = (StaffViewModel)TempData["staffViewModel"];
            try
            {
                if (viewModel != null)
                {
                    bool status = true;
                    //int status = validateFields(viewModel.resultFormatList);
                    for (int i = 0; i < viewModel.resultFormatList.Count; i++)
                    {
                        if (viewModel.resultFormatList[i].EX_CA < 0 || viewModel.resultFormatList[i].EX_CA > 30)
                        {
                            status = false;
                        }
                    }

                    if (!status)
                    {
                        SetMessage("Validation Error for one or more entries, cross check and re-upload!", Message.Category.Error);
                        KeepDropDownState(viewModel);
                        return RedirectToAction("UploadCA");
                    }


                    bool resultAdditionStatus = SaveNewStudentResult(viewModel);

                    SetMessage("Upload successful", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }

            KeepDropDownState(viewModel);
            return RedirectToAction("UploadCA");
        }
        [HttpPost]
        public ActionResult ProcessUploadCA(StaffViewModel staffViewModel)
        {

            try
            {
                List<ResultFormat> resultFormatList = new List<ResultFormat>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    FileUploadURL = savedFileName;
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if (studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < studentSet.Tables[0].Rows.Count; i++)
                        {
                            ResultFormat resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MATRICNO = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.EX_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][3].ToString().Trim());
                            resultFormat.fileUploadUrl = savedFileName;
                            if (resultFormat.MATRICNO != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }

                        }

                        CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                        staffViewModel.CourseAllocation =
                            courseAllocationLogic.GetModelsBy(
                                c =>
                                    c.Session_Id == staffViewModel.Session.Id &&
                                    c.Course_Id == staffViewModel.Course.Id &&
                                    c.Department_Id == staffViewModel.Department.Id &&
                                    c.Level_Id == staffViewModel.Level.Id &&
                                    c.Programme_Id == staffViewModel.Programme.Id &&
                                    c.Semester_Id == staffViewModel.Semester.Id).LastOrDefault();

                        staffViewModel.resultFormatList = resultFormatList;
                        staffViewModel.StudentResultType = new StudentResultType() { Id = 1 };

                        TempData["staffViewModel"] = staffViewModel;
                    }

                }

                //KeepDropDownState(staffViewModel);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    if (String.Equals(ex.Message, "Sequence contains more than one element"))
                    {
                        SetMessage(ContainsDuplicate, Message.Category.Error);
                    }
                }
                else if (ex.GetType() == typeof(NullReferenceException))
                {
                    if (String.Equals(ex.Message, "Object reference not set to an instance of an object."))
                    {
                        SetMessage(ArgumentNullException, Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                }
            }

            KeepDropDownState(staffViewModel);
            return View(staffViewModel);
        }
        public ActionResult StaffCAReportSheet()
        {
            StaffViewModel viewModel = null;
            try
            {
                viewModel = new StaffViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {

                throw;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StaffCAReportSheet(StaffViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult StaffDownloadCAReportSheet(string cid)
        {
            try
            {
                long Id = Convert.ToInt64(Utility.Decrypt(cid));
                viewModel = new StaffViewModel();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocation = courseAllocationLogic.GetModelsBy(p => p.Course_Allocation_Id == Id).LastOrDefault();
                if (viewModel.CourseAllocation != null)
                {
                    ReportViewModel reportViewModel = new ReportViewModel();
                    reportViewModel.Department = viewModel.CourseAllocation.Department;
                    reportViewModel.Course = viewModel.CourseAllocation.Course;
                    reportViewModel.Faculty = viewModel.CourseAllocation.Department.Faculty;
                    reportViewModel.Level = viewModel.CourseAllocation.Level;
                    reportViewModel.Programme = viewModel.CourseAllocation.Programme;
                    reportViewModel.Semester = viewModel.CourseAllocation.Semester;
                    reportViewModel.Session = viewModel.CourseAllocation.Session;
                    TempData["ReportViewModel"] = reportViewModel;
                    return RedirectToAction("CAResultSheet", "Report", new { area = "admin" });
                }

                return RedirectToAction("StaffCAReportSheet", "Staff", new { area = "admin" });
            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult CBEReportSheet()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {
                ViewBag.Session = staffViewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.CourseMode = staffViewModel.CourseModeSelectList;
                ViewBag.ResultType = staffViewModel.ResultTypeSelectList;
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);
                ViewBag.Programme = staffViewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
            }
            catch (Exception)
            {

                throw;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult CBEReportSheet(StaffViewModel viewModel)
        {
            try
            {
                DepartmentLogic departmentLogic = new DepartmentLogic();
                viewModel.Department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id);

                ReportViewModel reportViewModel = new ReportViewModel();
                reportViewModel.Department = viewModel.Department;
                reportViewModel.Course = viewModel.Course;
                reportViewModel.Faculty = viewModel.Department.Faculty;
                reportViewModel.Level = viewModel.Level;
                reportViewModel.Programme = viewModel.Programme;
                reportViewModel.Semester = viewModel.Semester;
                reportViewModel.Session = viewModel.Session;
                TempData["ReportViewModel"] = reportViewModel;

                return RedirectToAction("CBEResultSheet", "Report", new { area = "admin" });

            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult DownloadDepartmentResultSheet()
        {
            StaffViewModel staffViewModel = new StaffViewModel();
            try
            {
                ViewBag.Programme = staffViewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Session = staffViewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(staffViewModel.LevelList, ID, NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(staffViewModel);
        }
        [HttpPost]
        public ActionResult DownloadDepartmentResultSheet(StaffViewModel staffViewModel)
        {
            try
            {
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<GeneralResultFormat> generalResultFormat2 = new List<GeneralResultFormat>();
                List<GeneralResultFormat> generalResultFormat = new List<GeneralResultFormat>();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                Department department=departmentLogic.GetModelBy(x => x.Department_Id == staffViewModel.Department.Id);
                Programme programme = new Programme { Id = staffViewModel.Programme.Id };
                Session session = new Model.Model.Session { Id = staffViewModel.Session.Id };
                Semester semester = new Semester { Id = staffViewModel.Semester.Id };
                Level level = new Level { Id = staffViewModel.Level.Id };
                generalResultFormat2 = courseRegistrationLogic.GetDownloadResultFormatsForDeparment(department, programme, session, semester, level);

                if (generalResultFormat2 != null && generalResultFormat2.Count > 0)
                {
                    List<string> distinctMatricNumber = generalResultFormat2.Select(r => r.MATRICNO).Distinct().ToList();

                    for (int i = 0; i < distinctMatricNumber.Count; i++)
                    {
                        generalResultFormat.Add(generalResultFormat2.Where(r => r.MATRICNO == distinctMatricNumber[i]).LastOrDefault());
                    }
                }

                if (generalResultFormat.Count > 0)
                {
                    List<GeneralResultFormat> list = generalResultFormat.OrderBy(p => p.MATRICNO).ToList();
                    List<GeneralResultFormat> sort = new List<GeneralResultFormat>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort;// resultFormatList.OrderBy(p => p.MATRICNO);
                    CourseLogic courseLogic = new CourseLogic();

                    gv.Caption = " DEPARTMENT OF " + " " + department.Name.ToUpper();

                    gv.DataBind();

                    string filename = department.Code.Replace("/", "").Replace("\\", "") + department.Code + ".xls";
                    return new DownloadFileActionResult(gv, filename);
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
            return RedirectToAction("DownloadDepartmentResultSheet");
        }

    }
}