using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StaffCourseAllocationController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private StaffCourseAllocationViewModel viewModel;
        private string FileUploadURL = null;
        private UserLogic userLogic;
        public StaffCourseAllocationController()
        {
            userLogic = new UserLogic();
        }

        public ActionResult AllocatedCourses()
        {
            try
            {
               
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.CourseProgrammeSelectList;
                ViewBag.Level = viewModel.CourseAllocationLevel;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AllocatedCourses(StaffCourseAllocationViewModel viewModel)
        {
            CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
            ViewBag.Programme = viewModel.CourseProgrammeSelectList;
            ViewBag.Level = viewModel.CourseAllocationLevel;
            try
            {
                if (viewModel != null)
                {
                    UserLogic userLogic = new UserLogic();
                    StaffLogic staffLogic = new StaffLogic();
                    StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                    var activeUser = User.Identity.Name;
                    var getUserId = userLogic.GetModelBy(u => u.User_Name == activeUser);
                    var getStaffId = staffLogic.GetModelBy(s => s.User_Id == getUserId.Id);
                    var getStaffDept = staffDepartmentLogic.GetModelBy(d => d.Staff_Id == getStaffId.Id);
                    if (getStaffDept != null && (getStaffDept.IsHead || getStaffDept.IsExamOfficer))
                    {
                        viewModel.CourseAllocationList =
                            courseAllocationLogic.GetModelsBy(
                                p =>
                                    p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                    p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                    p.Session_Id == viewModel.CourseAllocation.Session.Id && p.Department_Id == getStaffDept.Department.Id).OrderBy(c => c.User.Username).ToList();
                    }
                    else
                    {
                        viewModel.CourseAllocationList =
                        courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id).OrderBy(c => c.User.Username).ToList();
                    }
                    
                    

                    //viewModel.CourseAllocationList.OrderBy(c => c.Department.Name).ThenBy(c => c.User.Username);
                }

                KeepCourseDropDownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }

            return View("AllocatedCourses", viewModel);
        }
        //[Authorize(Roles = "Admin,Exam Officer")]
        public ActionResult AllocateCourse()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                SessionLogic sessionLogic = new SessionLogic();
                var currectActiveSession = sessionLogic.GetModelsBy(f => f.Activated == true).LastOrDefault();
                // ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Session = Utility.PopulateSessionSelectListItemById(currectActiveSession.Id);
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.CourseProgrammeSelectList;

                ViewBag.Level = viewModel.CourseAllocationLevel;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
                var username = User.Identity.Name;
                var loggedInUser = userLogic.GetModelsBy(g => g.User_Name == username).FirstOrDefault();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                
                if (loggedInUser?.Id > 0 && loggedInUser.Role.Id == (int)UserRoles.Admin && currectActiveSession?.Id>0)
                {
                    ViewBag.User = Utility.PopulateStaffDepartmentSelectListItem(0);
                }
                else if(loggedInUser?.Id > 0 && (loggedInUser.Role.Id == (int)UserRoles.ExamOfficer || loggedInUser.Role.Id == (int)UserRoles.HOD) && currectActiveSession?.Id > 0)
                {
                    var assignedDeparment = staffDepartmentLogic.GetModelsBy(g => g.STAFF.USER.User_Id == loggedInUser.Id && (g.IsHead || g.IsExamOfficer)).FirstOrDefault();
                    if (assignedDeparment?.Id > 0 && assignedDeparment.Department?.Id > 0)
                    {
                        ViewBag.User = Utility.PopulateStaffDepartmentSelectListItem(assignedDeparment.Department.Id);
                        
                    }
                    else
                    {
                        SetMessage("You are not authorized to perform this task", Message.Category.Information);
                        return RedirectToAction("AllocatedCourses", "StaffCourseAllocation");
                    }
                }
                else
                {
                    SetMessage("You are not authorized to perform this task", Message.Category.Information);
                    return RedirectToAction("index", "Profile", new { Areas = "Admin" });
                }
                
               
            }
            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AllocateCourse(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
                GeneralAudit generalAudit = new GeneralAudit();
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var username = User.Identity.Name;
                var loggedInUser = userLogic.GetModelsBy(g => g.User_Name == username).FirstOrDefault();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                if (loggedInUser?.Id > 0 && loggedInUser.Role.Id == (int)UserRoles.Admin)
                {
                    ViewBag.User = Utility.PopulateStaffDepartmentSelectListItem(0);
                }
                else if (loggedInUser?.Id > 0 && loggedInUser.Role.Id == (int)UserRoles.ExamOfficer)
                {
                    var assignedDeparment = staffDepartmentLogic.GetModelsBy(g => g.STAFF.USER.User_Id == loggedInUser.Id && g.IsExamOfficer).LastOrDefault();
                    if (assignedDeparment?.Id > 0 && assignedDeparment.Department?.Id > 0)
                    {
                        ViewBag.User = Utility.PopulateStaffDepartmentSelectListItem(assignedDeparment.Department.Id);
                    }
                }
                if (viewModel != null)
                {

                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    CourseAllocation courseAllocationList =
                        courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                p.Course_Id == viewModel.CourseAllocation.Course.Id &&
                                p.Department_Id == viewModel.CourseAllocation.Department.Id).FirstOrDefault();
                    if (courseAllocationList == null)
                    {
                        
                        viewModel.CourseAllocation.CanUpload = true;
                        var created=courseAllocationLogic.Create(viewModel.CourseAllocation);
                        if (created?.Id > 0)
                        {
                            
                            generalAudit.Action = "Create Course Allocation";
                            generalAudit.Client = client;
                            generalAudit.CurrentValues = Convert.ToString(created.Id);
                            //generalAudit.InitialValues = Convert.ToString(initialStatus);
                            var user = userLogic.GetModelBy(f => f.User_Id == viewModel.CourseAllocation.User.Id);
                            generalAudit.Operation = "Assigned course - Id" + " " + viewModel.CourseAllocation.Course.Id + " "+ "to"+" "+ user.Username ;
                            generalAudit.User = loggedInUser;
                            generalAudit.TableNames = "Course-Allocation";
                            generalAuditLogic.CreateGeneralAudit(generalAudit);
                        }
                        SetMessage("Course has been allocated", Message.Category.Information);
                        //Audit
                        
                        //
                        KeepCourseDropDownState(viewModel);
                        return View(viewModel);
                    }
                    else
                    {
                        //Check for existing Allocation

                        //courseAllocationList.CanUpload = true;
                        //courseAllocationList.User.Id = viewModel.CourseAllocation.User.Id;
                        //courseAllocationLogic.Modify(courseAllocationList);
                        if (courseAllocationList.User.Id == viewModel.CourseAllocation.User.Id)
                        {
                            SetMessage("The selected Course has already been assigned to this Lecturer.", Message.Category.Information);
                        }
                        else
                        {
                            viewModel.ShowButton = true;
                            SetMessage("The selected Course has already been Allocated to: " + courseAllocationList.User.Username +" click continue to change", Message.Category.Error);
                        }
                        
                        KeepCourseDropDownState(viewModel);
                        
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        public ActionResult DownloadResultSheet()
        {
            try
            {
                StaffCourseAllocationViewModel viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectListAllocation;
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
        public ActionResult DownloadResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseAllocation courseAllocation = new CourseAllocation();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    courseAllocation =
                        courseAllocationLogic.GetModelBy(
                            p =>
                                p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id &&
                                p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id &&
                                p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                                p.USER.User_Name == User.Identity.Name);
                    if (courseAllocation == null)
                    {
                        if (!User.IsInRole("Admin"))
                        {
                            SetMessage(
                                "You are not allocated to this course, with this Programme-Department combination",
                                Message.Category.Error);
                            KeepDropDownState(viewModel);
                            return RedirectToAction("DownloadResultSheet");
                        }
                    }
                    GridView gv = new GridView();

                    DataTable ds = new DataTable();
                    List<ResultFormat> resultFormatList = new List<ResultFormat>();

                    SessionLogic sessionLogic = new SessionLogic();
                    Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    List<CourseRegistrationDetail> courseRegistrationDetailList = new List<CourseRegistrationDetail>();
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Session_Id == session.Id && p.Level_Id == viewModel.Level.Id &&
                                p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id);
                    if (viewModel.Course != null && viewModel.Semester != null && courseRegistrationList.Count > 0)
                    {
                        int count = 1;
                        foreach (CourseRegistration courseRegistration in courseRegistrationList)
                        {
                            courseRegistrationDetailList =
                                courseRegistrationDetailLogic.GetModelsBy(
                                    p =>
                                        p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id &&
                                        p.Student_Course_Registration_Id == courseRegistration.Id);
                            if (courseRegistrationDetailList.Count > 0)
                            {

                                foreach (
                                    CourseRegistrationDetail courseRegistrationDetailItem in
                                        courseRegistrationDetailList)
                                {
                                    ResultFormat resultFormat = new ResultFormat();
                                    // resultFormat.SN = count;
                                    resultFormat.MATRICNO =
                                        courseRegistrationDetailItem.CourseRegistration.Student.MatricNumber;
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

                        gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                        CourseLogic courseLogic = new CourseLogic();
                        Course course = courseLogic.GetModelBy(p => p.Course_Id == viewModel.Course.Id);
                        gv.Caption = course.Name.ToUpper() + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                                     course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                        gv.DataBind();

                        string filename = course.Code.Replace("/", "").Replace("\\", "") + course.Department.Code +
                                          ".xls";
                        return new DownloadFileActionResult(gv, filename);
                    }
                    else
                    {
                        Response.Write("No data available for download");
                        Response.End();
                        return new JavaScriptResult();
                    }
                }
                else
                {
                    SetMessage("Input is null", Message.Category.Error);

                    KeepDropDownState(viewModel);

                    return RedirectToAction("DownloadResultSheet");
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }


            return RedirectToAction("DownloadResultSheet");
        }

        public ActionResult UploadResultSheet()
        {
            try
            {
                StaffCourseAllocationViewModel viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectListAllocation;
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
        public ActionResult UploadResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation =
                    courseAllocationLogic.GetModelBy(
                        p =>
                            p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id &&
                            p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                            p.USER.User_Name == User.Identity.Name);
                //if (courseAllocation == null)
                //{
                //    if (!User.IsInRole("Admin"))
                //    {
                //        SetMessage("You are not allocated to this course, with this Programme-Department combination", Message.Category.Error);
                //        RetainDropdownState(viewModel);
                //        return View(viewModel);
                //    }
                //}
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
                        TempData["staffCourseAllocationViewModel"] = viewModel;

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
            StaffCourseAllocationViewModel viewModel =
                (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
            try
            {
                if (viewModel != null)
                {

                    int status = NewvalidateFields(viewModel.NewResultFormat);

                    if (status > 0)
                    {
                        NewResultFormat format = viewModel.NewResultFormat.ElementAt((status - 1));
                        SetMessage("Validation Error for" + " " + format.MATRICNO, Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return RedirectToAction("StaffResultSheet");
                    }


                    bool resultAdditionStatus = NewaddStudentResult(viewModel);

                    SetMessage("Upload successful", Message.Category.Information);
                }


            }
            catch (Exception ex)
            {
                SetMessage("Error occured " + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return RedirectToAction("StaffResultSheet");
        }

        public ActionResult DownloadAttendanceSheet()
        {
            try
            {
                StaffCourseAllocationViewModel viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectListAllocation;
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
        public ActionResult AttendanceReport(StaffCourseAllocationViewModel viewModel)
        {
            CourseAllocation courseAllocation = new CourseAllocation();
            CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
            // courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id && p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
            //if (courseAllocation == null)
            //{
            //if (!User.IsInRole("Admin"))
            //{
            //    SetMessage("You are not allocated to this course, with this Programme-Department combination", Message.Category.Error);
            //    KeepDropDownState(viewModel);
            //    return RedirectToAction("DownloadResultSheet");                    
            //}
            //}

            ViewBag.SessionId = viewModel.Session.Id.ToString();
            ViewBag.SemesterId = viewModel.Semester.Id.ToString();
            ViewBag.ProgrammeId = viewModel.Programme.Id.ToString();
            ViewBag.DepartmentId = viewModel.Department.Id.ToString();
            ViewBag.LevelId = viewModel.Level.Id.ToString();
            ViewBag.CourseId = viewModel.Course.Id.ToString();

            return View();
        }

        public ActionResult StaffResultSheet()
        {
            StaffCourseAllocationViewModel viewModel = new StaffCourseAllocationViewModel();
            //ViewBag.Session = viewModel.SessionSelectListAllocation;
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
            ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StaffResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception)
            {

                throw;
            }
            return View(viewModel);
        }

        public ActionResult ResultUploadSheet(long cid)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == cid);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<NewResultFormat> resultFormatList = new List<NewResultFormat>();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                resultFormatList = courseRegistrationLogic.GetDownloadResultNewFormats(courseAllocation);

                if (resultFormatList.Count > 0)
                {
                    List<NewResultFormat> list = resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                    List<NewResultFormat> sort = new List<NewResultFormat>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                    CourseLogic courseLogic = new CourseLogic();
                    Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                    gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                                 course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";

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
            return RedirectToAction("StaffResultSheet");
        }

        [HttpPost]
        public ActionResult ResultUploadSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.cid);
                CourseLogic courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                if (courseAllocation.CanUpload != true)
                {
                    SetMessage("You are not activated to upload courses in this semester.", Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                List<NewResultFormat> resultFormatList = new List<NewResultFormat>();
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
                            NewResultFormat resultFormat = new NewResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MATRICNO = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            //resultFormat.QU1 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][2].ToString().Trim());
                            //resultFormat.QU2 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][3].ToString().Trim());
                            //resultFormat.QU3 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            //resultFormat.QU4 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][5].ToString().Trim());
                            //resultFormat.QU5 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][6].ToString().Trim());
                            //resultFormat.QU6 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][7].ToString().Trim());
                            //resultFormat.QU7 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][8].ToString().Trim());
                            //resultFormat.QU8 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][9].ToString().Trim());
                            //resultFormat.QU9 = Convert.ToDecimal(studentSet.Tables[0].Rows[i][10].ToString().Trim());
                            resultFormat.T_EX = Convert.ToDecimal(studentSet.Tables[0].Rows[i][2].ToString().Trim());
                            resultFormat.T_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][3].ToString().Trim());
                            //resultFormat.EX_CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            //resultFormat.fileUploadUrl = savedFileName;
                            if (resultFormat.MATRICNO != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }

                        }
                        resultFormatList.OrderBy(p => p.MATRICNO);
                        viewModel.NewResultFormat = resultFormatList;
                        viewModel.Course = course;
                        StaffCourseAllocationViewModel vModel = (StaffCourseAllocationViewModel)TempData["vModel"];
                        vModel.Course = course;
                        vModel.NewResultFormat = resultFormatList;
                        vModel.CourseAllocation = courseAllocation;
                        TempData["staffCourseAllocationViewModel"] = vModel;

                    }

                }
            }
            catch (Exception ex)
            {

                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult DownloadZip(string downloadName)
        {
            TempData["downloadName"] = downloadName + ".zip";
            return View();
        }

        public ActionResult GetZip()
        {
            try
            {
                string downloadName = (string)TempData["downloadName"];
                TempData.Keep("downloadName");
                string path = "~/Content/temp/" + downloadName;
                Response.Redirect(path, false);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }

            return View("DownloadZip");
        }

        private bool addStudentResult(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester =
                    sessionSemesterLogicc.GetModelBy(
                        p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    CourseRegistration courseRegistration = new CourseRegistration();
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    StudentResultDetail studentResultDetailTest;
                    StudentResultDetail studentResultDetailExam;
                    List<StudentResultDetail> studentResultDetailTestList;
                    List<StudentResultDetail> studentResultDetailExamList;
                    StudentResultLogic studentResultLogic = new StudentResultLogic();
                    StudentResult studentResultTest;
                    StudentResult studentResultExam;
                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic =
                        new StudentExamRawScoreSheetResultLogic();

                    string operation = "MODIFY";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    courseRegistrationDetailAudit.User = user;
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    if (User.IsInRole("Admin") || User.IsInRole("School Admin") || User.IsInRole("support") || User.IsInRole("R&DC") || User.IsInRole("ExamChairman") || User.IsInRole("lloydant") || User.IsInRole("SchoolOfficer"))
                    {
                        viewModel.CourseAllocation = courseAllocationLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id && c.Department_Id == viewModel.Department.Id && c.Level_Id == viewModel.Level.Id && c.Programme_Id == viewModel.Programme.Id && c.Semester_Id == viewModel.Semester.Id && c.Session_Id == viewModel.Session.Id);
                    }
                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (
                        TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            InitializeStudentResult(viewModel, "ADMIN", sessionSemester, testResultType, examResultType,
                                user, out studentResultDetailTest, out studentResultDetailExam,
                                out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic,
                                out studentResultTest, out studentResultExam);
                            studentResultDetailTest.Course = viewModel.Course;

                            courseRegistration =
                                courseRegistrationLogic.GetModelsBy(
                                    c =>
                                        c.STUDENT.Matric_Number == resultFormat.MATRICNO.Trim() &&
                                        c.Session_Id == viewModel.Session.Id).LastOrDefault();
                            if (courseRegistration != null)
                            {
                                studentResultDetailTest.Student =
                                    studentLogic.GetModelBy(
                                        p =>
                                            p.Matric_Number == resultFormat.MATRICNO.Trim() &&
                                            p.Person_Id == courseRegistration.Student.Id);
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



                                    studentExamRawScoreSheet =
                                        StudentExamRawScoreSheetLogic.GetModelBy(
                                            p =>
                                                p.Student_Id == studentResultDetailExam.Student.Id &&
                                                p.Semester_Id == viewModel.Semester.Id &&
                                                p.Session_Id == viewModel.Session.Id &&
                                                p.Course_Id == viewModel.Course.Id);
                                    List<StudentExamRawScoreSheet> a =
                                        StudentExamRawScoreSheetLogic.GetModelsBy(
                                            p =>
                                                p.Student_Id == studentResultDetailExam.Student.Id &&
                                                p.Semester_Id == viewModel.Semester.Id &&
                                                p.Session_Id == viewModel.Session.Id &&
                                                p.Course_Id == viewModel.Course.Id);
                                    if (a.Count > 1)
                                    {
                                        Response.Write("o");
                                    }


                                    if (studentExamRawScoreSheet == null)
                                    {
                                        studentExamRawScoreSheet = CreateExamRawScoreSheet(viewModel, user,
                                            studentResultDetailTest, studentExamRawScoreSheet,
                                            StudentExamRawScoreSheetLogic, resultFormat);
                                    }
                                    else
                                    {
                                        ModifyExamRawScoreSheet(viewModel, user, studentResultDetailTest,
                                            studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
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
        private bool NewaddStudentResult(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester =
                    sessionSemesterLogicc.GetModelBy(
                        p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if (viewModel != null && viewModel.NewResultFormat.Count > 0)
                {
                    CourseRegistration courseRegistration = new CourseRegistration();
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    StudentResultDetail studentResultDetailTest;
                    StudentResultDetail studentResultDetailExam;
                    List<StudentResultDetail> studentResultDetailTestList;
                    List<StudentResultDetail> studentResultDetailExamList;
                    StudentResultLogic studentResultLogic = new StudentResultLogic();
                    StudentResult studentResultTest;
                    StudentResult studentResultExam;
                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic =
                        new StudentExamRawScoreSheetResultLogic();

                    string operation = "MODIFY";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    courseRegistrationDetailAudit.User = user;
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    if (User.IsInRole("Admin") || User.IsInRole("School Admin") || User.IsInRole("support") || User.IsInRole("R&DC") || User.IsInRole("ExamChairman") || User.IsInRole("lloydant") || User.IsInRole("SchoolOfficer"))
                    {
                        viewModel.CourseAllocation = courseAllocationLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id && c.Department_Id == viewModel.Department.Id && c.Level_Id == viewModel.Level.Id && c.Programme_Id == viewModel.Programme.Id && c.Semester_Id == viewModel.Semester.Id && c.Session_Id == viewModel.Session.Id);
                    }
                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (
                        TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (NewResultFormat resultFormat in viewModel.NewResultFormat)
                        {
                            InitializeStudentResult(viewModel, "ADMIN", sessionSemester, testResultType, examResultType,
                                user, out studentResultDetailTest, out studentResultDetailExam,
                                out studentResultDetailTestList, out studentResultDetailExamList, out studentResultLogic,
                                out studentResultTest, out studentResultExam);
                            studentResultDetailTest.Course = viewModel.Course;

                            courseRegistration =
                                courseRegistrationLogic.GetModelsBy(
                                    c =>
                                        c.STUDENT.Matric_Number == resultFormat.MATRICNO.Trim() &&
                                        c.Session_Id == viewModel.Session.Id).LastOrDefault();
                            if (courseRegistration != null)
                            {
                                studentResultDetailTest.Student =
                                    studentLogic.GetModelBy(
                                        p =>
                                            p.Matric_Number == resultFormat.MATRICNO.Trim() &&
                                            p.Person_Id == courseRegistration.Student.Id);
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



                                    studentExamRawScoreSheet =
                                        StudentExamRawScoreSheetLogic.GetModelBy(
                                            p =>
                                                p.Student_Id == studentResultDetailExam.Student.Id &&
                                                p.Semester_Id == viewModel.Semester.Id &&
                                                p.Session_Id == viewModel.Session.Id &&
                                                p.Course_Id == viewModel.Course.Id);
                                    List<StudentExamRawScoreSheet> a =
                                        StudentExamRawScoreSheetLogic.GetModelsBy(
                                            p =>
                                                p.Student_Id == studentResultDetailExam.Student.Id &&
                                                p.Semester_Id == viewModel.Semester.Id &&
                                                p.Session_Id == viewModel.Session.Id &&
                                                p.Course_Id == viewModel.Course.Id);
                                    if (a.Count > 1)
                                    {
                                        Response.Write("o");
                                    }


                                    if (studentExamRawScoreSheet == null)
                                    {
                                        studentExamRawScoreSheet = CreateNewExamRawScoreSheet(viewModel, user,
                                            studentResultDetailTest, studentExamRawScoreSheet,
                                            StudentExamRawScoreSheetLogic, resultFormat);
                                    }
                                    else
                                    {
                                        ModifyNewExamRawScoreSheet(viewModel, user, studentResultDetailTest,
                                            studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat);
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

        private static void ModifyExamRawScoreSheet(StaffCourseAllocationViewModel viewModel, User user,
            StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
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
        private static void ModifyNewExamRawScoreSheet(StaffCourseAllocationViewModel viewModel, User user,
            StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, NewResultFormat resultFormat)
        {
            studentExamRawScoreSheet.Course = viewModel.Course;
            //studentExamRawScoreSheet.EX_CA = (double)resultFormat.EX_CA;
            studentExamRawScoreSheet.EX_CA = (double)resultFormat.T_EX + (double)resultFormat.T_CA;
            studentExamRawScoreSheet.Level = viewModel.Level;
            studentExamRawScoreSheet.T_CA = (double)resultFormat.T_CA;

            studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.T_EX = (double)resultFormat.T_EX;

            studentExamRawScoreSheet.Remark = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.Semester = viewModel.Semester;
            studentExamRawScoreSheet.Session = viewModel.Session;
            studentExamRawScoreSheet.Student = studentResultDetailTest.Student;
            studentExamRawScoreSheet.Uploader = user;
            bool isScoreSheetModified = StudentExamRawScoreSheetLogic.Modify(studentExamRawScoreSheet);

        }

        private static StudentExamRawScoreSheet CreateExamRawScoreSheet(StaffCourseAllocationViewModel viewModel,
            User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, ResultFormat resultFormat)
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
        private static StudentExamRawScoreSheet CreateNewExamRawScoreSheet(StaffCourseAllocationViewModel viewModel,
            User user, StudentResultDetail studentResultDetailTest, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetResultLogic StudentExamRawScoreSheetLogic, NewResultFormat resultFormat)
        {
            studentExamRawScoreSheet = new StudentExamRawScoreSheet();
            studentExamRawScoreSheet.Course = viewModel.Course;
            //studentExamRawScoreSheet.EX_CA = (double)resultFormat.EX_CA;
            studentExamRawScoreSheet.EX_CA = (double)resultFormat.T_CA + (double)resultFormat.T_EX;
            studentExamRawScoreSheet.Level = viewModel.Level;
            studentExamRawScoreSheet.T_CA = (double)resultFormat.T_CA;

            studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.Remark = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.T_EX = (double)resultFormat.T_EX;
            studentExamRawScoreSheet.Semester = viewModel.Semester;
            studentExamRawScoreSheet.Session = viewModel.Session;
            studentExamRawScoreSheet.Student = studentResultDetailTest.Student;
            studentExamRawScoreSheet.Uploader = user;
            StudentExamRawScoreSheetLogic.Create(studentExamRawScoreSheet);
            return studentExamRawScoreSheet;
        }

        private static void InitializeStudentResult(StaffCourseAllocationViewModel viewModel, string FileUploadURL,
            SessionSemester sessionSemester, StudentResultType testResultType, StudentResultType examResultType,
            User user, out StudentResultDetail studentResultDetailTest, out StudentResultDetail studentResultDetailExam,
            out List<StudentResultDetail> studentResultDetailTestList,
            out List<StudentResultDetail> studentResultDetailExamList, out StudentResultLogic studentResultLogic,
            out StudentResult studentResultTest, out StudentResult studentResultExam)
        {
            studentResultDetailTest = new StudentResultDetail();
            studentResultDetailExam = new StudentResultDetail();
            studentResultDetailTestList = new List<StudentResultDetail>();
            studentResultDetailExamList = new List<StudentResultDetail>();
            studentResultLogic = new StudentResultLogic();
            studentResultTest = new StudentResult();
            studentResultExam = new StudentResult();

            studentResultTest.MaximumObtainableScore = 40;
            studentResultTest.DateUploaded = DateTime.Now;
            if (viewModel.Course.Department == null)
            {
                studentResultTest.Department = viewModel.CourseAllocation.Department;
            }
            else
            {
                studentResultTest.Department = viewModel.Course.Department;
            }

            studentResultTest.Level = viewModel.CourseAllocation.Level;
            studentResultTest.Programme = viewModel.CourseAllocation.Programme;
            studentResultTest.SessionSemester = sessionSemester;
            studentResultTest.UploadedFileUrl = FileUploadURL;
            studentResultTest.Uploader = user;
            studentResultTest.Type = testResultType;

            studentResultExam.MaximumObtainableScore = 70;
            studentResultExam.DateUploaded = DateTime.Now;
            if (viewModel.Course.Department == null)
            {
                studentResultExam.Department = viewModel.CourseAllocation.Department;
            }
            else
            {
                studentResultExam.Department = viewModel.Course.Department;
            }
            studentResultExam.Level = viewModel.CourseAllocation.Level;
            studentResultExam.Programme = viewModel.CourseAllocation.Programme;
            studentResultExam.SessionSemester = sessionSemester;
            studentResultExam.UploadedFileUrl = FileUploadURL;
            studentResultExam.Uploader = user;
            studentResultExam.Type = examResultType;
        }

        private List<ResultHolder> RetrieveCourseRegistrationInformation(StaffCourseAllocationViewModel viewModel)
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
                            courseRegistration =
                                courseRegistrationLogic.GetModelBy(
                                    p =>
                                        p.Person_Id == studentId && p.Level_Id == viewModel.Level.Id &&
                                        p.Session_Id == viewModel.Session.Id &&
                                        p.Department_Id == viewModel.Department.Id &&
                                        p.Programme_Id == viewModel.Programme.Id);

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
                            decimal calculatedExamScore = list[i].QU1 + list[i].QU2 + list[i].QU3 + list[i].QU4 +
                                                          list[i].QU5 + list[i].QU6 + list[i].QU7 + list[i].QU8 +
                                                          list[i].QU9;
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

                            if ((inputExamScore >= 0 && inputExamScore <= 70) &&
                                (calculatedExamScore >= 0 && calculatedExamScore <= 70))
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
        private int NewvalidateFields(List<NewResultFormat> list)
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
                            NewAssignSpecialCaseRemarks(list, i, testScore, inputExamScore);
                        }
                        else
                        {
                            //decimal inputTotalScore = list[i].EX_CA;
                            decimal calculatedTotalScore = list[i].T_CA + list[i].T_EX;
                            if (testScore >= 0 && testScore <= 40)
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

                            if ((inputExamScore >= 0 && inputExamScore <= 70))
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

                            //if ((calculatedTotalScore == inputTotalScore) && calculatedTotalScore <= 100)
                            if (calculatedTotalScore <= 100)
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

        private static void AssignSpecialCaseRemarks(List<ResultFormat> list, int i, decimal testScore,
            decimal inputExamScore)
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
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Deferred)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "DEFERRED";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "DEFERRED";
                    list[i].T_EX = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Incomplete)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "INCOMPLETE";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "INCOMPLETE";
                    list[i].T_EX = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Withheld)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "WITHHELD";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "WITHHELD";
                    list[i].T_EX = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private static void NewAssignSpecialCaseRemarks(List<NewResultFormat> list, int i, decimal testScore,
     decimal inputExamScore)
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
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Deferred)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "DEFERRED";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "DEFERRED";
                    list[i].T_EX = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Incomplete)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "INCOMPLETE";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "INCOMPLETE";
                    list[i].T_EX = 0;
                }
                else if (inputExamScore == (decimal)SpeicalCaseCodes.Withheld)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "WITHHELD";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "WITHHELD";
                    list[i].T_EX = 0;
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
        public JsonResult GetAssignedDepartments(string id, string sessionId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(sessionId))
                {
                    return null;
                }
                var username = User.Identity.Name;
                var loggedInUser = userLogic.GetModelsBy(g => g.User_Name == username).FirstOrDefault();

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                Session session = new Session() { Id = Convert.ToInt32(sessionId) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                if(loggedInUser?.Id>0 && loggedInUser.Role.Id == (int)UserRoles.Admin)
                {
                    result.IsError = false;
                    result.Departments = departmentLogic.GetBy(programme);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var assignedDeparment = staffDepartmentLogic.GetModelsBy(g => g.STAFF.USER.User_Id == loggedInUser.Id && g.IsExamOfficer && g.SESSION_SEMESTER.Session_Id == session.Id).LastOrDefault();
                if (assignedDeparment != null)
                {
                    result.IsError = false;
                    result.Departments = departmentLogic.GetBy(programme, assignedDeparment.Department);
                }
                else
                {
                    result.IsError = true;
                    result.Message = "No Assigned Department";
                }
                

                return Json(result, JsonRequestBehavior.AllowGet);

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
                //var levelId=level.Id==8 || level.Id==11?
                Programme programme = new Programme() { Id = Convert.ToInt32(ids[4]) };
                var programmeId = programme.Id;
                programme.Id = programmeId == 2 || programmeId == 6 ? 1 : programmeId == 4 || programmeId == 5 ? 3 : programmeId;
                Department department = new Department() { Id = Convert.ToInt32(ids[0]) };
                Semester semester = new Semester() { Id = Convert.ToInt32(ids[2]) };
                

                DepartmentOption departmentOption = null;

                if (ids.Count() >= 5)
                {
                    departmentOption = new DepartmentOption() { Id = Convert.ToInt32(ids[3]) };
                }

                List<Course> courseList = Utility.GetCoursesByOptionProgrammeDepartmentAndSemester(programme,departmentOption, department, semester);

                for (int i = 0; i < courseList.Count; i++)
                {
                    courseList[i].Name += ", " + courseList[i].Code;
                    if (courseList[i].DepartmentOption != null)
                    {
                        courseList[i].Name += " IN " + courseList[i].DepartmentOption.Name + " Option";
                    }
                }

                courseList.OrderBy(c => c.Code);

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void KeepDropDownState(StaffCourseAllocationViewModel viewModel)
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
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level, viewModel.Department,
                        viewModel.Semester);

                    ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Course.Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void KeepCourseDropDownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.CourseProgrammeSelectList;
                ViewBag.Level = viewModel.CourseAllocationLevel;
                ViewBag.User = viewModel.UserSelectList;
                if (viewModel.CourseAllocation.Semester != null)
                {
                    List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList =
                        sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.CourseAllocation.Session.Id);

                    List<Semester> semesters = new List<Semester>();
                    foreach (SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters, ID, NAME, viewModel.CourseAllocation.Semester.Id);
                }
                else
                {
                    ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                }
                if (viewModel.CourseAllocation.Department != null)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.CourseAllocation.Programme);

                    ViewBag.Department = new SelectList(departments, ID, NAME, viewModel.CourseAllocation.Department.Id);
                }
                else
                {
                    ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                }
                if (viewModel.CourseAllocation.Course != null)
                {
                    List<Course> courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.CourseAllocation.Level,
                        viewModel.CourseAllocation.Department, viewModel.CourseAllocation.Semester);

                    ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.CourseAllocation.Course.Id);
                }
                else
                {
                    ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                }
                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0 && viewModel.CourseAllocation.Programme != null && viewModel.CourseAllocation.Department != null)
                {
                    DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                    ViewBag.DepartmentOptions = new SelectList(departmentOptionLogic.GetBy(viewModel.CourseAllocation.Department, viewModel.CourseAllocation.Programme), ID, NAME, viewModel.DepartmentOption.Id);
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
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
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
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

        public void RetainDropdownState(StaffCourseAllocationViewModel viewModel)
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
                        ViewBag.Session = viewModel.SessionSelectListAllocation;
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

        public enum SpeicalCaseCodes
        {
            Sick = 101,
            Absent = 201,
            Deferred = 301,
            Withheld = 401,
            Incomplete = 501,
            Other = 601
        }

        public ActionResult StaffReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
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
        public ActionResult StaffReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id &&
                            p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {

                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult StaffDownloadReportSheet(string cid, int courseModeId)
        {
            try
            {
                long Id = Convert.ToInt64(Utility.Decrypt(cid));
                viewModel = new StaffCourseAllocationViewModel();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocation = courseAllocationLogic.GetModelsBy(p => p.Course_Allocation_Id == Id).LastOrDefault();
                if (viewModel.CourseAllocation != null)
                {
                    //ReportViewModel reportViewModel = new ReportViewModel();
                    //reportViewModel.Department = viewModel.CourseAllocation.Department;
                    //reportViewModel.Course = viewModel.CourseAllocation.Course;
                    //reportViewModel.Faculty = viewModel.CourseAllocation.Department.Faculty;
                    //reportViewModel.Level = viewModel.CourseAllocation.Level;
                    //reportViewModel.Programme = viewModel.CourseAllocation.Programme;
                    //reportViewModel.Semester = viewModel.CourseAllocation.Semester;
                    //reportViewModel.Session = viewModel.CourseAllocation.Session;
                    //TempData["ReportViewModel"] = reportViewModel;

                    StaffViewModel staffViewModel = new StaffViewModel();
                    staffViewModel.CourseAllocation = viewModel.CourseAllocation;
                    staffViewModel.courseModeId = courseModeId;

                    TempData["viewModel"] = staffViewModel;

                    return RedirectToAction("ResultSheetAlt", "Report", new { area = "admin" });
                }

                return RedirectToAction("StaffReportSheet", "StaffCourseAllocation", new { area = "admin" });
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult DownloadBlankResultSheet()
        {
            try
            {
                StaffCourseAllocationViewModel viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectListAllocation;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                return View(viewModel);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult DownloadBlankResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception)
            {

                throw;
            }
            return View(viewModel);
        }

        public ActionResult BlankResultUploadSheet(long cid)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == cid);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<ResultFormat> resultFormatList = new List<ResultFormat>();
                ResultFormat sampleFormat = new ResultFormat();
                sampleFormat.MATRICNO = "N/XX/15/12345";
                resultFormatList.Add(sampleFormat);
                gv.DataSource = resultFormatList; // resultFormatList.OrderBy(p => p.MATRICNO);
                CourseLogic courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                             course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                gv.DataBind();

                string filename = course.Code.Replace("/", "").Replace("\\", "") + course.Department.Code + ".xls";
                return new DownloadFileActionResult(gv, filename);


            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("StaffResultSheet");
        }

        [HttpPost]
        public ActionResult BlankResultUploadSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocation courseAllocation = new CourseAllocation();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.cid);
                CourseLogic courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                if (courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination", Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                if (courseAllocation.CanUpload != true)
                {
                    SetMessage("You are not activated to upload courses in this semester.", Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
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
                        viewModel.resultFormatList = resultFormatList;
                        viewModel.Course = course;
                        StaffCourseAllocationViewModel vModel = (StaffCourseAllocationViewModel)TempData["vModel"];
                        vModel.Course = course;
                        vModel.resultFormatList = resultFormatList;
                        vModel.CourseAllocation = courseAllocation;
                        TempData["staffCourseAllocationViewModel"] = vModel;

                    }

                }
            }
            catch (Exception ex)
            {

                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveUploadedBlankResultSheet()
        {
            StaffCourseAllocationViewModel viewModel =
                (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
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
                        return RedirectToAction("DownloadBlankResultSheet");
                    }

                    bool resultAdditionStatus = AddUnregisteredStudentResult(viewModel);

                    SetMessage("Upload successful", Message.Category.Information);
                }


            }
            catch (Exception ex)
            {
                SetMessage("Error occured " + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return RedirectToAction("DownloadBlankResultSheet");
        }

        private bool AddUnregisteredStudentResult(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
                SessionSemesterLogic sessionSemesterLogicc = new SessionSemesterLogic();
                SessionSemester sessionSemester = new SessionSemester();
                UserLogic userLogic = new UserLogic();
                StudentResultType testResultType = new StudentResultType() { Id = 1 };
                StudentResultType examResultType = new StudentResultType() { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester =
                    sessionSemesterLogicc.GetModelBy(
                        p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if (viewModel != null && viewModel.resultFormatList.Count > 0)
                {

                    StudentExamRawScoreSheet studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    StudentExamRawScoreSheetNotRegisteredLogic StudentExamRawScoreSheetLogic =
                        new StudentExamRawScoreSheetNotRegisteredLogic();

                    StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                    using (
                        TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new System.TimeSpan(0, 15, 0)))
                    {
                        foreach (ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            studentExamRawScoreSheet =
                                StudentExamRawScoreSheetLogic.GetModelBy(
                                    p =>
                                        p.Student_Matric_Number == resultFormat.MATRICNO &&
                                        p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                                        p.Course_Id == viewModel.Course.Id);
                            if (studentExamRawScoreSheet == null)
                            {
                                studentExamRawScoreSheet = CreateUnregisteredStudentExamRawScoreSheet(viewModel, user,
                                    studentExamRawScoreSheet, StudentExamRawScoreSheetLogic, resultFormat,
                                    resultFormat.fileUploadUrl);
                            }
                            else
                            {
                                ModifyUnregisteredStudentExamRawScoreSheet(viewModel, user, studentExamRawScoreSheet,
                                    StudentExamRawScoreSheetLogic, resultFormat, resultFormat.fileUploadUrl);
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

        private static StudentExamRawScoreSheet CreateUnregisteredStudentExamRawScoreSheet(
            StaffCourseAllocationViewModel viewModel, User user, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic,
            ResultFormat resultFormat, string fileURL)
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
            studentExamRawScoreSheet.Programme = viewModel.CourseAllocation.Programme;
            studentExamRawScoreSheetNotRegisteredLogic.Create(studentExamRawScoreSheet);
            return studentExamRawScoreSheet;
        }

        private static void ModifyUnregisteredStudentExamRawScoreSheet(StaffCourseAllocationViewModel viewModel,
            User user, StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic,
            ResultFormat resultFormat, string fileURL)
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
            studentExamRawScoreSheet.Programme = viewModel.CourseAllocation.Programme;
            bool isScoreSheetModified = studentExamRawScoreSheetNotRegisteredLogic.Modify(studentExamRawScoreSheet);
        }

        public ActionResult StaffBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
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
        public ActionResult StaffBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);

                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {

                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult AdminBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Department_Id == viewModel.Department.Id);

                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult StaffDownloadBlankReportSheet(int Session_Id, int Semester_Id, int Programme_Id,
            int Department_Id, int Level_Id, int Course_Id)
        {
            try
            {

                return RedirectToAction("UnregisteredStudentResultSheet",
                    new
                    {
                        area = "admin",
                        controller = "Report",
                        levelId = Level_Id,
                        semesterId = Semester_Id,
                        progId = Programme_Id,
                        deptId = Department_Id,
                        sessionId = Session_Id,
                        courseId = Course_Id
                    });

            }
            catch (Exception)
            {
                throw;
            }

        }

        public ActionResult ViewUploadedCourses()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.PastSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewUploadedCourses(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                CourseLogic courseLogic = new CourseLogic();
                viewModel.UploadedCourses = studentResultLogic.GetUploadedCourses(viewModel.Session, viewModel.Semester, viewModel.Programme, viewModel.Department, viewModel.Level);
                for (int i = 0; i < viewModel.UploadedCourses.Count; i++)
                {
                    UploadedCourseFormat currentUploadedCourse = viewModel.UploadedCourses[i];
                    if (currentUploadedCourse.CourseId > 0)
                    {
                        Course course = courseLogic.GetModelBy(c => c.Course_Id == currentUploadedCourse.CourseId);
                        if (course != null && course.DepartmentOption != null)
                        {
                            viewModel.UploadedCourses[i].DepartmentOption = course.DepartmentOption.Name;
                        }
                    }
                }

                viewModel.UploadedCourses.OrderBy(r => r.Department);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            TempData["UploadedCourses"] = viewModel.UploadedCourses;
            TempData.Keep("UploadedCourses");

            RetainDropdownState(viewModel);
            //RetainSessionSemesterDropDown(viewModel);

            return View(viewModel);
        }

        private void RetainSessionSemesterDropDown(StaffCourseAllocationViewModel viewModel)
        {
            List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
            SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
            sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

            List<Semester> semesters = new List<Semester>();
            foreach (SessionSemester sessionSemester in sessionSemesterList)
            {
                semesters.Add(sessionSemester.Semester);
            }
            ViewBag.Session = viewModel.SessionSelectListAllocation;
            if (viewModel.Semester != null)
            {
                ViewBag.Semester = new SelectList(semesters, "Id", "Name", viewModel.Semester.Id);
            }
            else
            {
                ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
            }
        }

        public ActionResult ViewScoreSheet(string index, int courseModeId)
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                viewModel.UploadedCourses = (List<UploadedCourseFormat>)TempData["UploadedCourses"];
                UploadedCourseFormat uploadedCourseFormat = viewModel.UploadedCourses[Convert.ToInt32(index)];
                //ReportViewModel reportViewModel = new ReportViewModel();
                //reportViewModel.Department.Name = uploadedCourseFormat.Department;
                //reportViewModel.Department.Id = uploadedCourseFormat.DepartmentId;
                //reportViewModel.Course.Id = uploadedCourseFormat.CourseId;
                //reportViewModel.Course.Name = uploadedCourseFormat.CourseTitle;
                //reportViewModel.Level.Id = uploadedCourseFormat.LevelId;
                //reportViewModel.Level.Name = uploadedCourseFormat.Level;
                //reportViewModel.Programme.Id = uploadedCourseFormat.ProgrammeId;
                //reportViewModel.Semester.Id = uploadedCourseFormat.SemesterId;
                //reportViewModel.Session.Id = uploadedCourseFormat.SessionId;

                //TempData.Keep("UploadedCourses");
                //TempData["ReportViewModel"] = reportViewModel;

                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                CourseAllocation courseAllocation = courseAllocationLogic.GetModelBy(c => c.Course_Allocation_Id == uploadedCourseFormat.CourseAllocationId);

                if (courseAllocation == null)
                {
                    SetMessage("Course has not been allocated", Message.Category.Error);
                    return RedirectToAction("ViewUploadedCourses");
                }

                StaffViewModel staffViewModel = new StaffViewModel();
                staffViewModel.CourseAllocation = courseAllocation;
                staffViewModel.courseModeId = courseModeId;

                TempData["viewModel"] = staffViewModel;

                return RedirectToAction("ResultSheetAlt", new { area = "admin", controller = "Report" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult DepartmentReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
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
        public ActionResult DepartmentReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                StudentResultLogic studentResultLogic = new StudentResultLogic();

                viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name && p.Is_HOD == true);

                if (viewModel.CourseAllocations != null && viewModel.CourseAllocations.Count > 0)
                {
                    int HodDepartmentId = viewModel.CourseAllocations[0].HodDepartment.Id;
                    viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Department_Id == HodDepartmentId && p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id && p.Semester_Id == viewModel.Semester.Id);

                    if (viewModel.CourseAllocations != null)
                    {
                        viewModel.UploadedCourses = studentResultLogic.GetUploadedCourses(viewModel.Session, viewModel.Semester, viewModel.CourseAllocations.LastOrDefault().Programme, viewModel.CourseAllocations.LastOrDefault().Department, viewModel.Level);
                        List<long> uploadedCourses = viewModel.UploadedCourses.Select(s => s.CourseId).Distinct().ToList();

                        for (int i = 0; i < viewModel.CourseAllocations.Count; i++)
                        {
                            if (uploadedCourses.Contains(viewModel.CourseAllocations[i].Course.Id))
                            {
                                viewModel.CourseAllocations[i].Uploaded = true;
                            }
                            else
                            {
                                viewModel.CourseAllocations[i].Uploaded = false;
                            }
                        }
                    }

                }
                else
                {
                    StaffDepartment staffDepartment = staffDepartmentLogic.GetModelsBy(s => s.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && s.STAFF.USER.User_Name == User.Identity.Name && s.IsHead).LastOrDefault();

                    if (staffDepartment != null)
                    {
                        viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(p => p.Department_Id == staffDepartment.Department.Id && p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id && p.Semester_Id == viewModel.Semester.Id);

                        if (viewModel.CourseAllocations != null)
                        {
                            viewModel.UploadedCourses = studentResultLogic.GetUploadedCourses(viewModel.Session, viewModel.Semester, viewModel.CourseAllocations.LastOrDefault().Programme, viewModel.CourseAllocations.LastOrDefault().Department, viewModel.Level);
                            List<long> uploadedCourses = viewModel.UploadedCourses.Select(s => s.CourseId).Distinct().ToList();

                            for (int i = 0; i < viewModel.CourseAllocations.Count; i++)
                            {
                                if (uploadedCourses.Contains(viewModel.CourseAllocations[i].Course.Id))
                                {
                                    viewModel.CourseAllocations[i].Uploaded = true;
                                }
                                else
                                {
                                    viewModel.CourseAllocations[i].Uploaded = false;
                                }
                            }
                        }

                    }
                    else
                    {
                        SetMessage("You have not been set as the department head for this session, please contact system administrator.", Message.Category.Error);
                    }
                }

                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {

                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult ViewAlternateScoreSheet()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewAlternateScoreSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                viewModel.UploadedCourses = studentResultLogic.GetUploadedAlternateCourses(viewModel.Session,
                    viewModel.Semester);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            TempData["UploadedCourses"] = viewModel.UploadedCourses;
            TempData.Keep("UploadedCourses");
            RetainSessionSemesterDropDown(viewModel);

            return View(viewModel);
        }

        public ActionResult DownloadViewAlternateScoreSheet(int Session_Id, int Semester_Id, int Programme_Id,
            int Department_Id, int Level_Id, int Course_Id)
        {
            try
            {

                return RedirectToAction("UnregisteredStudentResultSheet",
                    new
                    {
                        area = "admin",
                        controller = "Report",
                        levelId = Level_Id,
                        semesterId = Semester_Id,
                        progId = Programme_Id,
                        deptId = Department_Id,
                        sessionId = Session_Id,
                        courseId = Course_Id
                    });

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult AdminDownloadBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminDownloadBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Department_Id == viewModel.Department.Id);

                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult AdminBlankResultUploadSheet(long cid, int deptId, int progId, int levelId, int sessionId)
        {
            try
            {
                StudentExamRawScoreSheetNotRegisteredLogic scoreSheetNotRegisteredLogic =
                    new StudentExamRawScoreSheetNotRegisteredLogic();

                GridView gv = new GridView();
                DataTable ds = new DataTable();
                List<ResultFormat> resultFormatList = scoreSheetNotRegisteredLogic.GetDownloadResultFormats(cid, deptId,
                    progId, levelId, sessionId);

                List<ResultFormat> list = resultFormatList.OrderBy(p => p.MATRICNO).ToList();
                List<ResultFormat> sort = new List<ResultFormat>();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SN = (i + 1);
                    sort.Add(list[i]);
                }

                gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                CourseLogic courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == cid);
                gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                             course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                gv.DataBind();

                string filename = course.Code.Replace("/", "").Replace("\\", "") + course.Department.Code + ".xls";
                return new DownloadFileActionResult(gv, filename);


            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("AdminDownloadBlankReportSheet");
        }

        public ActionResult ProcessAlternateSheet()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ProcessAlternateSheet(StaffCourseAllocationViewModel viewModel)
        {
            List<ExamRawScoreSheetReport> unregisteredRawScores = new List<ExamRawScoreSheetReport>();
            List<ExamRawScoreSheetReport> masterUnregisteredRawScores = new List<ExamRawScoreSheetReport>();
            List<string> includedMatricNumbers = new List<string>();

            DepartmentLogic departmentLogic = new DepartmentLogic();
            DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

            try
            {
                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    includedMatricNumbers = studentLevelLogic.GetModelsBy(
                                            s =>
                                                s.Department_Id == viewModel.Department.Id &&
                                                s.Department_Option_Id == viewModel.DepartmentOption.Id &&
                                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id &&
                                                s.Session_Id == viewModel.Session.Id).Select(s => s.Student.MatricNumber).ToList();
                }
                else
                {
                    includedMatricNumbers = studentLevelLogic.GetModelsBy(
                                            s =>
                                                s.Department_Id == viewModel.Department.Id &&
                                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id &&
                                                s.Session_Id == viewModel.Session.Id).Select(s => s.Student.MatricNumber).ToList();
                }

                StudentExamRawScoreSheetNotRegisteredLogic rawScoreSheetNotRegisteredLogic = new StudentExamRawScoreSheetNotRegisteredLogic();
                Department department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id);


                unregisteredRawScores = rawScoreSheetNotRegisteredLogic.GetExamRawScoreSheetsToProcess(viewModel.Session, viewModel.Semester, viewModel.Level, viewModel.Programme, department);

                List<string> distinctMatricNumbers = unregisteredRawScores.Select(u => u.MatricNumber).Distinct().ToList();

                for (int i = 0; i < distinctMatricNumbers.Count; i++)
                {
                    string currentMatricNumber = distinctMatricNumbers[i];

                    if (!includedMatricNumbers.Contains(currentMatricNumber))
                    {
                        continue;
                    }

                    List<ExamRawScoreSheetReport> currentScores = unregisteredRawScores.Where(r => r.MatricNumber == currentMatricNumber).ToList();

                    string courses = "";
                    for (int j = 0; j < currentScores.Count; j++)
                    {
                        courses += "| " + currentScores[j].CourseCode;
                    }

                    currentScores[0].CourseCode = courses;
                    masterUnregisteredRawScores.Add(currentScores[0]);
                }

                masterUnregisteredRawScores.OrderBy(m => m.MatricNumber);
                viewModel.ExamRawScoreSheetReports = masterUnregisteredRawScores;
                viewModel.MatricNumbers = includedMatricNumbers;
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }

            SemesterLogic semesterLogic = new SemesterLogic();
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(semesterLogic.GetAll(), ID, NAME, viewModel.Semester.Id);
            ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            ViewBag.Programme = viewModel.ProgrammeSelectList;
            ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME, viewModel.Department.Id);
            if (viewModel.DepartmentOption != null)
            {
                ViewBag.DepartmentOptions = new SelectList(departmentOptionLogic.GetBy(viewModel.Department, viewModel.Programme), ID, NAME, viewModel.DepartmentOption.Id);
            }
            else
            {
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), ID, NAME);
            }

            TempData["Scores"] = unregisteredRawScores;
            TempData["viewModel"] = viewModel;

            return View(viewModel);
        }

        public ActionResult SaveProcessedAlternateSheets()
        {
            try
            {
                List<ExamRawScoreSheetReport> Scores = (List<ExamRawScoreSheetReport>)TempData["Scores"];
                StaffCourseAllocationViewModel viewModel = (StaffCourseAllocationViewModel)TempData["viewModel"];

                if (Scores != null && Scores.Count > 0)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    UserLogic userLogic = new UserLogic();
                    StudentExamRawScoreSheetNotRegisteredLogic rawScoreSheetNotRegisteredLogic = new StudentExamRawScoreSheetNotRegisteredLogic();

                    List<string> distinctMatricNumbers = Scores.Select(u => u.MatricNumber).Distinct().ToList();
                    for (int i = 0; i < distinctMatricNumbers.Count; i++)
                    {
                        string currentMatricNumber = distinctMatricNumbers[i];

                        if (!viewModel.MatricNumbers.Contains(currentMatricNumber))
                        {
                            continue;
                        }

                        List<CourseRegistration> courseRegistrations = courseRegistrationLogic.GetModelsBy(c => c.STUDENT.Matric_Number == currentMatricNumber && c.Session_Id == viewModel.Session.Id && c.Programme_Id == viewModel.Programme.Id);
                        if (courseRegistrations.Count != 1)
                        {
                            continue;
                        }
                        List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == currentMatricNumber);
                        if (students.Count != 1)
                        {
                            continue;
                        }

                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                        List<ExamRawScoreSheetReport> currentScores = Scores.Where(r => r.MatricNumber == currentMatricNumber).ToList();

                        for (int j = 0; j < currentScores.Count; j++)
                        {
                            ExamRawScoreSheetReport thisScore = currentScores[j];

                            ResultUploadViewModel resultUploadViewModel = new ResultUploadViewModel();
                            resultUploadViewModel.Session = new Session() { Id = thisScore.SessionId };
                            resultUploadViewModel.Semester = new Semester() { Id = thisScore.SemesterId };

                            List<ResultFormat> resultFormats = new List<ResultFormat>()
                            {
                                new ResultFormat()
                                {
                                    MATRICNO = thisScore.MatricNumber,
                                    T_CA = Convert.ToDecimal(thisScore.T_CA),
                                    ResultSpecialCaseMessages = new ResultSpecialCaseMessages() {SpecialCaseMessage = thisScore.SpecialCase},
                                    T_EX = Convert.ToDecimal(thisScore.T_EX),
                                    EX_CA = Convert.ToDecimal(thisScore.EX_CA),
                                    QU1 = Convert.ToDecimal(thisScore.QU1),
                                    QU2 = Convert.ToDecimal(thisScore.QU2),
                                    QU3 = Convert.ToDecimal(thisScore.QU3),
                                    QU4 = Convert.ToDecimal(thisScore.QU4),
                                    QU5 = Convert.ToDecimal(thisScore.QU5),
                                    QU6 = Convert.ToDecimal(thisScore.QU6),
                                    QU7 = Convert.ToDecimal(thisScore.QU7),
                                    QU8 = Convert.ToDecimal(thisScore.QU8),
                                    QU9 = Convert.ToDecimal(thisScore.QU9)
                                }
                            };

                            resultUploadViewModel.Client = client;
                            resultUploadViewModel.resultFormatList = resultFormats;
                            resultUploadViewModel.Department = courseRegistrations.FirstOrDefault().Department;
                            resultUploadViewModel.Level = courseRegistrations.FirstOrDefault().Level;
                            resultUploadViewModel.Programme = courseRegistrations.FirstOrDefault().Programme;
                            resultUploadViewModel.Course = GetCorrectCourse(thisScore.CourseId, students.FirstOrDefault(), viewModel.DepartmentOption, viewModel.Session, viewModel.Level);

                            User User = userLogic.GetModelBy(u => u.User_Id == thisScore.UserId);

                            ResultController resultController = new ResultController();
                            bool isResultProcessed = resultController.addStudentResult(resultUploadViewModel, User.Username);

                            if (isResultProcessed)
                            {
                                rawScoreSheetNotRegisteredLogic.Delete(r => r.Student_Result_Id == thisScore.StudentResultId);
                            }
                        }

                    }

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ProcessAlternateSheet");
        }

        private Course GetCorrectCourse(long courseId, Model.Model.Student student, DepartmentOption departmentOption, Session session, Level level)
        {
            Course correctCourse = new Course();
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                Course course = courseLogic.GetModelBy(c => c.Course_Id == courseId);

                if (departmentOption != null && departmentOption.Id > 0)
                {
                    CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetailLogic.GetModelsBy(
                                                                            c =>
                                                                                c.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                                                                                c.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id &&
                                                                                c.STUDENT_COURSE_REGISTRATION.Level_Id == level.Id &&
                                                                                c.COURSE.Course_Code == course.Code).LastOrDefault();
                    if (courseRegistrationDetail != null)
                    {
                        Course courseForOption = courseLogic.GetModelsBy(c => c.Course_Code == course.Code && c.Level_Id == level.Id && c.Department_Option_Id == departmentOption.Id).LastOrDefault();
                        if (courseForOption != null && courseRegistrationDetail.Course.Id != courseForOption.Id)
                        {
                            courseRegistrationDetail.Course = courseForOption;

                            string operation = "MODIFY";
                            string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffCourseAllocationController)";
                            string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                            var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                            courseRegistrationDetailAudit.Action = action;
                            courseRegistrationDetailAudit.Operation = operation;
                            courseRegistrationDetailAudit.Client = client;
                            UserLogic loggeduser = new UserLogic();
                            courseRegistrationDetailAudit.User = loggeduser.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();

                            courseRegistrationDetailLogic.Modify(courseRegistrationDetail, courseRegistrationDetailAudit);
                        }

                        correctCourse = courseForOption;
                    }
                    else
                    {
                        correctCourse = course;
                        return correctCourse;
                    }
                }
                else
                {
                    correctCourse = course;
                    return correctCourse;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return correctCourse;
        }


        public ActionResult ScoreSheetDisparity(int id)
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                viewModel.UploadedCourses = (List<UploadedCourseFormat>)TempData["UploadedCourses"];
                UploadedCourseFormat uploadedCourseFormat = viewModel.UploadedCourses[Convert.ToInt32(id)];

                StudentExamRawScoreSheetResultLogic rawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                Session session = new Session() { Id = uploadedCourseFormat.SessionId };
                Semester semester = new Semester() { Id = uploadedCourseFormat.SemesterId };
                Course course = new Course() { Id = uploadedCourseFormat.CourseId };
                Department department = new Department() { Id = uploadedCourseFormat.DepartmentId };
                Level level = new Level() { Id = uploadedCourseFormat.LevelId };
                Programme programme = new Programme() { Id = uploadedCourseFormat.ProgrammeId };

                List<ExamRawScoreSheetReport> rawScoreSheets = rawScoreSheetResultLogic.GetScoreSheetBy(session,
                    semester, course, department, level, programme);
                List<CourseRegistrationDetail> courseRegistrationDetails =
                    courseRegistrationDetailLogic.GetModelsBy(
                        c =>
                            c.Course_Id == course.Id && c.Semester_Id == semester.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id);
                List<string> rawScoreRegNumbers = rawScoreSheets.Select(r => r.MatricNumber).Distinct().ToList();

                List<CourseRegistrationDetail> omittedStudentScores = new List<CourseRegistrationDetail>();

                for (int i = 0; i < courseRegistrationDetails.Count; i++)
                {
                    CourseRegistrationDetail thisRegDetail = courseRegistrationDetails[i];
                    if (!rawScoreRegNumbers.Contains(thisRegDetail.CourseRegistration.Student.MatricNumber))
                    {
                        omittedStudentScores.Add(thisRegDetail);
                    }
                }

                viewModel.CourseRegistrationDetails = omittedStudentScores.OrderBy(o => o.CourseRegistration.Student.MatricNumber).ToList();

                TempData.Keep("UploadedCourses");
                TempData["viewModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult GetStudentScores(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }
                StaffCourseAllocationViewModel viewModel = (StaffCourseAllocationViewModel)TempData["viewModel"];
                long personId = Convert.ToInt64(id);

                StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();

                List<StudentExamRawScoreSheet> scores = new List<StudentExamRawScoreSheet>();

                scores = studentExamRawScoreSheetResultLogic.GetModelsBy(s => s.STUDENT.Person_Id == personId);

                viewModel.RawScoreSheets = scores;
                TempData.Keep("viewModel");
                return PartialView("_StudentRawScores", viewModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetCourseRegistration(int id)
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                viewModel.UploadedCourses = (List<UploadedCourseFormat>)TempData["UploadedCourses"];
                UploadedCourseFormat uploadedCourseFormat = viewModel.UploadedCourses[Convert.ToInt32(id)];

                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                Session session = new Session() { Id = uploadedCourseFormat.SessionId };
                Semester semester = new Semester() { Id = uploadedCourseFormat.SemesterId };
                Course course = new Course() { Id = uploadedCourseFormat.CourseId };
                Department department = new Department() { Id = uploadedCourseFormat.DepartmentId };
                Level level = new Level() { Id = uploadedCourseFormat.LevelId };
                Programme programme = new Programme() { Id = uploadedCourseFormat.ProgrammeId };

                List<CourseRegistrationDetail> courseRegistrationDetails =
                    courseRegistrationDetailLogic.GetModelsBy(
                        c =>
                            c.Course_Id == course.Id && c.Semester_Id == semester.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                            c.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id);

                viewModel.CourseRegistrationDetails = courseRegistrationDetails.OrderBy(o => o.CourseRegistration.Student.MatricNumber).ToList();

                TempData.Keep("UploadedCourses");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult EnableStaffCourseUpload()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EnableStaffCourseUpload(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(c => c.Session_Id == viewModel.Session.Id && c.Department_Id == viewModel.Department.Id && c.Level_Id == viewModel.Level.Id && c.Programme_Id == viewModel.Programme.Id && c.Semester_Id == viewModel.Semester.Id);
                    for (int i = 0; i < viewModel.CourseAllocations.Count; i++)
                    {
                        viewModel.CourseAllocations[i].StaffCanUpload = viewModel.CourseAllocations[i].CanUpload.Value;
                    }

                    viewModel.CourseAllocations.OrderBy(c => c.Department.Name);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);

            return View(viewModel);
        }

        public ActionResult SaveEditedCourseUploadPrivilege(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                for (int i = 0; i < viewModel.CourseAllocations.Count; i++)
                {
                    CourseAllocation currentAllocation = viewModel.CourseAllocations[i];
                    CourseAllocation courseAllocation = courseAllocationLogic.GetModelBy(c => c.Course_Allocation_Id == currentAllocation.Id);

                    courseAllocation.CanUpload = viewModel.CourseAllocations[i].StaffCanUpload;

                    courseAllocationLogic.Modify(courseAllocation);
                }

                SetMessage("Operation Successful! ", Message.Category.Information);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EnableStaffCourseUpload");
        }
        public ActionResult ResultModificationDetail()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ResultModificationDetail(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    viewModel.CourseAllocations = courseAllocationLogic.GetModelsBy(c => c.Session_Id == viewModel.Session.Id && c.Department_Id == viewModel.Department.Id && c.Level_Id == viewModel.Level.Id && c.Programme_Id == viewModel.Programme.Id && c.Semester_Id == viewModel.Semester.Id);

                    viewModel.CourseAllocations.OrderBy(c => c.Department.Name);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            TempData["ViewModel"] = viewModel;
            RetainDropdownState(viewModel);

            return View(viewModel);
        }
        public ActionResult ViewCourseResultModification(string courseId)
        {
            try
            {
                long currentCourseId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(courseId));
                if (currentCourseId > 0)
                {
                    viewModel = (StaffCourseAllocationViewModel)TempData["ViewModel"];
                    TempData.Keep("ViewModel");

                    StudentResultLogic studentResultLogic = new StudentResultLogic();
                    Course course = new Course() { Id = currentCourseId };

                    viewModel.ResultUpdates = new List<ResultUpdateModel>();
                    viewModel.ResultUpdates = studentResultLogic.GetResultUpdates(viewModel.Session, viewModel.Semester, viewModel.Programme, viewModel.Department, viewModel.Level, course);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public JsonResult RemoveAllocation(string id)
        {
            try
            {
                long allocationId = Convert.ToInt64(id);
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();

                courseAllocationLogic.Delete(s => s.Course_Allocation_Id == allocationId);
                //return Json(new { result = "Success" });
                return Json(new { success = true, responseText = "Record was removed!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ModifyCourseAllocation(string pId, string dId, string sId, string seId, string uId, string lId, string cId )
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if(!String.IsNullOrEmpty(dId) && !String.IsNullOrEmpty(sId) && !String.IsNullOrEmpty(seId) && !String.IsNullOrEmpty(cId) && !String.IsNullOrEmpty(pId) && !String.IsNullOrEmpty(uId) && !String.IsNullOrEmpty(lId))
                {
                    long programmeId = Convert.ToInt64(pId);
                    long departmentId = Convert.ToInt64(dId);
                    long sessionId = Convert.ToInt64(sId);
                    long semesterId = Convert.ToInt64(seId);
                    long userId = Convert.ToInt64(uId);
                    long levelId = Convert.ToInt64(lId);
                    long courseId = Convert.ToInt64(cId);
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();

                    CourseAllocation courseAllocation = courseAllocationLogic.GetModelsBy(f => f.Department_Id == departmentId && f.Programme_Id == programmeId && f.Session_Id == sessionId && f.Semester_Id == semesterId && f.Course_Id == courseId && f.Level_Id == levelId).FirstOrDefault();
                    if (courseAllocation?.Id > 0)
                    {
                        GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
                        GeneralAudit generalAudit = new GeneralAudit();
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                        var username = User.Identity.Name;
                        var loggedInUser = userLogic.GetModelsBy(g => g.User_Name == username).FirstOrDefault();

                        courseAllocation.User = new User() { Id = userId };
                        courseAllocationLogic.Modify(courseAllocation);

                        var allocatedCourse = courseAllocationLogic.GetModelBy(f => f.Course_Allocation_Id == courseAllocation.Id);
                        generalAudit.Action = "Modify Course Allocation";
                        generalAudit.Client = client;
                        generalAudit.CurrentValues = Convert.ToString(allocatedCourse.User.Id);
                        generalAudit.InitialValues = Convert.ToString(userId);
                        generalAudit.Operation = "Re-Assigned" + " " + allocatedCourse.Course.Code + " " +" from  "+ courseAllocation.User.Username  + "to" + " " + allocatedCourse.User.Username;
                        generalAudit.User = loggedInUser;
                        generalAudit.TableNames = "Course-Allocation";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                        result.IsError = false;
                        result.Message = "Operation was successful";
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
        public ActionResult ViewCourseAllocation()
        {
            viewModel = new StaffCourseAllocationViewModel();
            ViewBag.SessionId = viewModel.SessionSelectList;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewCourseAllocation(StaffCourseAllocationViewModel viewModel)
        {
            ViewBag.SessionId = viewModel.SessionSelectList;
            if (viewModel?.Session?.Id > 0)
            {
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocationList= courseAllocationLogic.GetModelsBy(f => f.Session_Id == viewModel.Session.Id);
                viewModel.ShowButton = true;
            }
            return View(viewModel);
        }


    }
    public class ResultErrorDisplay
    {
        public int ErrorCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}