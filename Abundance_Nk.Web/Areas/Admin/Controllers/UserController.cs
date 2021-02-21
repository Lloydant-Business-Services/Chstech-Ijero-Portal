using System.EnterpriseServices.Internal;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, PTO, CPO")]
    public class UserController : BaseController
    {
        private UserViewModel viewModel;
        private User user;
        private Person person;
        private PersonType personType;
        private UserLogic userLogic;
        private PersonLogic personLogic;
        private PersonTypeLogic personTypeLogic;
        private StaffLogic staffLogic;
        private StaffDepartmentLogic staffDepartmentLogic;
        private RoleLogic roleLogic;
        private DepartmentLogic departmentLogic;
        private string FileUploadURL = null;
        public UserController()
        {
            viewModel = new UserViewModel();
            user = new User();
            person = new Person();
            personType = new PersonType();
            personTypeLogic = new PersonTypeLogic();
            userLogic = new UserLogic();
            personLogic = new PersonLogic();
            staffLogic = new StaffLogic();
            staffDepartmentLogic = new StaffDepartmentLogic();
            roleLogic = new RoleLogic();
            departmentLogic = new DepartmentLogic();
        }

        public ActionResult Index()
        {
            try
            {
                List<User> userList = new List<User>();
                userList = userLogic.GetModelsBy(p => p.Role_Id != 1);
                viewModel.Users = userList;
            }
            catch (Exception e)
            {
                SetMessage("Error Occured " + e.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        public ActionResult CreateUser()
        {
            
                ViewBag.SexList = viewModel.SexSelectList;
                ViewBag.RoleList = viewModel.RoleSelectList;
                ViewBag.SecurityQuestionList = viewModel.SecurityQuestionSelectList;
            
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(UserViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    List<User> users = new List<User>();
                    UserLogic userLogic = new UserLogic();
                    users = userLogic.GetModelsBy(p => p.User_Name == viewModel.User.Username);
                    if (users.Count > 0)
                    {
                        SetMessage("Error: Staff with this Name already exist", Message.Category.Error);
                        return RedirectToAction("CreateUser");
                    }

                    //Role role = new Role() { Id = 8 };
                    //viewModel.User.Role = role;
                    viewModel.User.LastLoginDate = DateTime.Now;
                    userLogic.Create(viewModel.User);
                    SetMessage("User Created Succesfully", Message.Category.Information);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    SetMessage("Input is null", Message.Category.Error);
                    return RedirectToAction("CreateUser");
                }
                
            }
            catch (Exception e)
            {
                SetMessage("Error Occured " + e.Message, Message.Category.Error);
            }

            ViewBag.SexList = viewModel.SexSelectList;
            ViewBag.RoleList = viewModel.RoleSelectList;
            ViewBag.SecurityQuestionList = viewModel.SecurityQuestionSelectList;
            return View("CreateUser",viewModel);
        }

        public ActionResult GetUserDetails()
        {     
            return View();
        }
        [HttpPost]
        public ActionResult GetUserDetails(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User.Username != null)
                {
                    userLogic = new UserLogic();
                    List<User> users = userLogic.GetModelsBy(u => u.User_Name == viewModel.User.Username);
                    if (users.Count > 1)
                    {
                        SetMessage("There are more than one user with this username!", Message.Category.Error);
                        return View(viewModel);
                    }
                    if (users.Count == 0)
                    {
                        SetMessage("There is no user with this username!", Message.Category.Error);
                        return View(viewModel);
                    }

                    Model.Model.User user = users.FirstOrDefault();
                    return RedirectToAction("ViewUserDetails", new { @id = user.Id });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }

            return View();
        }
        public ActionResult ViewUserDetails(int? id)
        {
            try
            {
                viewModel = null;
                if (id != null)
                {
                    user = userLogic.GetModelBy(p => p.User_Id == id);
                    viewModel = new UserViewModel();
                    viewModel.User = user;
                    return View(viewModel);
                }
                else
                {
                    SetMessage("Error Occured: Select a User", Message.Category.Error);
                    return RedirectToAction("Index");
               }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }
            return View();
        }
        public ActionResult EditUser(int? id)
        {
            viewModel = null;
            try
            {
                if (id != null)
                {
                    TempData["userId"] = id;
                    user = userLogic.GetModelBy(p => p.User_Id == id);
                    viewModel = new UserViewModel();
                    viewModel.User = user;
                }
                else
                {
                    SetMessage("Select a User", Message.Category.Error);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }

            ViewBag.RoleList = viewModel.RoleSelectList;
            ViewBag.SecurityQuestionList = viewModel.SecurityQuestionSelectList;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditUser(UserViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    //Role role = new Role() { Id = 8 };
                    //viewModel.User.Role = role;
                    viewModel.User.Id = (int)TempData["userId"];
                    userLogic.Update(viewModel.User);
                    SetMessage("User Edited Successfully", Message.Category.Information);
                }
                else
                {
                    SetMessage("Input is null", Message.Category.Warning);
                    return RedirectToAction("EditUser");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        public ActionResult EditUserRole()
        {
            try
            {                
                viewModel = new UserViewModel();
                ViewBag.Role = viewModel.RoleSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditUserRole(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User.Role != null)
                {
                    UserLogic userLogic = new UserLogic();
                    viewModel.Users = userLogic.GetModelsBy(u => u.Role_Id == viewModel.User.Role.Id);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Role = viewModel.RoleSelectList;
            return View(viewModel);
        }
        public ActionResult EditRole(string id)
        {
            try
            {
                int userId = Convert.ToInt32(id);
                UserLogic userLogic = new UserLogic();

                viewModel = new UserViewModel();
                viewModel.User = userLogic.GetModelBy(u => u.User_Id == userId);

                ViewBag.Role = new SelectList(viewModel.RoleSelectList, "Value", "Text", viewModel.User.Role.Id);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditRole(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User != null)
                {
                    UserLogic userLogic = new UserLogic();
                    bool isUserModified = userLogic.Modify(viewModel.User);

                    if (isUserModified)
                    {
                        SetMessage("Operation Successful!", Message.Category.Information);
                        return RedirectToAction("EditRoleByUserName"); 
                    }
                    else
                    {
                        SetMessage("No item was Modified", Message.Category.Information);
                        return RedirectToAction("EditRoleByUserName");
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditRoleByUserName");
        }

        public ActionResult EditRoleByUserName()
        {
            try
            {
                viewModel = new UserViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditRoleByUserName(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User.Username != null)
                {
                    UserLogic userLogic = new UserLogic();
                    User user = userLogic.GetModelBy(u => u.User_Name == viewModel.User.Username);
                    viewModel.User = user;
                    if (user == null)
                    {
                        SetMessage("User does not exist!", Message.Category.Error);
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Role = viewModel.RoleSelectList;
            return View(viewModel);
        }

        public ActionResult ChangeCourseStaffRole()
        {
            SessionLogic sessionLogic = new SessionLogic();
            try
            {
                viewModel = new UserViewModel();
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.CurrentSession = viewModel.ActiveSessionSelectList;
                ViewBag.User = viewModel.ExamOfficersAndHODRoleSelectList;

            }
            catch (Exception ex)
            {
                 SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
       
        [HttpPost]
        public ActionResult ChangeCourseStaffRole(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User.Username != null)
                {
                    UserLogic userLogic = new UserLogic();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                    StaffLogic staffLogic = new StaffLogic();
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    CourseAllocation courseAllocation = new CourseAllocation();
                    SessionLogic sessionLogic = new SessionLogic();

                    User user = userLogic.GetModelBy(u => u.User_Name == viewModel.User.Username);
                    if (user == null)
                    {
                        SetMessage("User Does Not Exist", Message.Category.Error);
                        ViewBag.Departments = viewModel.DepartmentSelectList;
                        ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                        return View(viewModel);
                    }

                    courseAllocation = courseAllocationLogic.GetModelsBy(ca => ca.User_Id == user.Id && ca.Session_Id == viewModel.Session.Id).LastOrDefault();
                    if (courseAllocation == null)
                    {
                        Staff staff = staffLogic.GetModelsBy(s => s.User_Id == user.Id).LastOrDefault();
                        if (staff != null)
                        {
                            StaffDepartment staffDepartment = staffDepartmentLogic.GetModelsBy(s => s.Staff_Id == staff.Id).LastOrDefault();
                            if (staffDepartment != null)
                            {
                                StaffDepartment existingStaffDepartmentHOD = staffDepartmentLogic.GetModelsBy(s => s.IsHead && s.Department_Id == viewModel.Department.Id && s.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && s.Staff_Id != staff.Id).LastOrDefault();
                                if (existingStaffDepartmentHOD != null)
                                {
                                    SetMessage("Another person has been set as the HOD for this department this session!", Message.Category.Error);
                                    ViewBag.Departments = viewModel.DepartmentSelectList;
                                    ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                                    return RedirectToAction("ChangeCourseStaffRole");
                                }

                                if (!viewModel.RemoveHOD)
                                {
                                    staffDepartment.IsHead = true;
                                    staffDepartment.Department = viewModel.Department;
                                    staffDepartmentLogic.Modify(staffDepartment);
                                }
                                else
                                {
                                    staffDepartment.IsHead = false;
                                    staffDepartmentLogic.Modify(staffDepartment);
                                }

                                SetMessage("Operation Successful!", Message.Category.Information);
                                ViewBag.Departments = viewModel.DepartmentSelectList;
                                ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                                return RedirectToAction("ChangeCourseStaffRole");
                            }
                            else if (viewModel.Department != null)
                            {
                                StaffDepartment existingStaffDepartmentHOD = staffDepartmentLogic.GetModelsBy(s => s.IsHead && s.Department_Id == viewModel.Department.Id && s.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && s.Staff_Id != staff.Id).LastOrDefault();
                                if (existingStaffDepartmentHOD != null)
                                {
                                    SetMessage("Another person has been set as the HOD for this department this session!", Message.Category.Error);
                                    ViewBag.Departments = viewModel.DepartmentSelectList;
                                    ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                                    return RedirectToAction("ChangeCourseStaffRole");
                                }


                                staffDepartment = new StaffDepartment();
                                staffDepartment.DateEntered = DateTime.Now;
                                staffDepartment.Department = viewModel.Department;
                                staffDepartment.IsHead = true;
                                staffDepartment.SessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == viewModel.Session.Id).LastOrDefault();
                                staffDepartment.Staff = staff;
                                var activeSession = sessionLogic.GetActiveSessions();


                                staffDepartmentLogic.Create(staffDepartment);

                                SetMessage("Operation Successful!", Message.Category.Information);
                                ViewBag.Departments = viewModel.DepartmentSelectList;
                                ViewBag.CurrentSession = activeSession;
                                return RedirectToAction("ChangeCourseStaffRole");
                            }
                            else
                            {
                                SetMessage("Staff has not been allocated any course", Message.Category.Error);
                                ViewBag.Departments = viewModel.DepartmentSelectList;
                                ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                                return View(viewModel);
                            }
                        }
                        else
                        {
                            SetMessage("User has not filled staff profile!", Message.Category.Error);
                            ViewBag.Departments = viewModel.DepartmentSelectList;
                            ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
                            return View(viewModel);
                        }
                    }

                    viewModel.CourseAllocation = courseAllocation;
                    viewModel.User = user;
                    RetainDropDownState(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Departments = viewModel.DepartmentSelectList;
            ViewBag.CurrentSession = viewModel.CurrentSessionSelectList;
            return View(viewModel); 
        }

        public ActionResult SaveCourseStaffRole(UserViewModel viewModel)
        {
            try
            {
                UserLogic userLogic = new UserLogic();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                User user = new User();
                List<CourseAllocation> courseAllocations = new List<CourseAllocation>();

                user = userLogic.GetModelBy(u => u.User_Id == viewModel.User.Id);
                courseAllocations = courseAllocationLogic.GetModelsBy(ca => ca.User_Id == user.Id);
                //if (courseAllocations.FirstOrDefault().HodDepartment.Id != viewModel.CourseAllocation.HodDepartment.Id || courseAllocations.FirstOrDefault().Programme.Id != viewModel.CourseAllocation.Programme.Id || courseAllocations.FirstOrDefault().Level.Id != viewModel.CourseAllocation.Level.Id || courseAllocations.FirstOrDefault().Session.Id != viewModel.CourseAllocation.Session.Id || courseAllocations.FirstOrDefault().Semester.Id != viewModel.CourseAllocation.Semester.Id)
                //{
                //    SetMessage("The User has not been allocated any course in this Programme, Department, Level, Session and Semester", Message.Category.Error); 
                //    RetainDropDownState(viewModel);
                //    return View("ChangeCourseStaffRole");
                //}
                using (TransactionScope scope = new TransactionScope())
                {
                    user.Role = viewModel.User.Role;
                    userLogic.Modify(user);

                    for (int i = 0; i < courseAllocations.Count; i++)
                    {
                        if (user.Role.Id != 12)
                        {
                            courseAllocations[i].HodDepartment = null;
                            courseAllocations[i].IsHOD = null;
                            courseAllocationLogic.Modify(courseAllocations[i]);
                        }
                        else
                        {
                            courseAllocations[i].HodDepartment = viewModel.CourseAllocation.HodDepartment;
                            courseAllocations[i].IsHOD = true;
                            courseAllocationLogic.Modify(courseAllocations[i]);
                        }
                    }

                    scope.Complete();
                    SetMessage("Operation Successful!", Message.Category.Information);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("ChangeCourseStaffRole");
        }
        public void RetainDropDownState(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.CourseAllocation != null)
                {
                    if (viewModel.CourseAllocation.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList, "Value", "Text", viewModel.CourseAllocation.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }

                    if (viewModel.CourseAllocation.Department != null && viewModel.CourseAllocation.Programme != null)
                    {
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.CourseAllocation.Programme), "Id", "Name", viewModel.CourseAllocation.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                    }

                    if (viewModel.CourseAllocation.Semester != null && viewModel.CourseAllocation.Session != null)
                    {
                        SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                        List<SessionSemester> sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.CourseAllocation.Session.Id);

                        List<Semester> semesters = new List<Semester>();
                        foreach (SessionSemester sessionSemester in sessionSemesterList)
                        {
                            semesters.Add(sessionSemester.Semester);
                        }

                        ViewBag.Semester = new SelectList(semesters, "Id", "Name", viewModel.CourseAllocation.Session.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
                    }

                    if (viewModel.CourseAllocation.Session != null)
                    {
                        ViewBag.Session = new SelectList(viewModel.SessionSelectList, "Value", "Text", viewModel.CourseAllocation.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                    }

                    if (viewModel.CourseAllocation.Level != null)
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelSelectList, "Value", "Text", viewModel.CourseAllocation.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = viewModel.LevelSelectList;
                    }
                }
                if (viewModel.User != null)
                {
                    if (viewModel.User.Role != null)
                    {
                        ViewBag.Role = new SelectList(viewModel.RoleSelectList, "Value", "Text", viewModel.User.Role.Id);
                    }
                    else
                    {
                        ViewBag.Role = viewModel.RoleSelectList;
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

                return Json(new SelectList(departments, "Id", "Name"), JsonRequestBehavior.AllowGet);

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

                return Json(new SelectList(semesters, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DeleteStaff()
        {
            try
            {
                viewModel = new UserViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult DeleteStaff(UserViewModel viewModel)
        {
            try
            {
                UserLogic userLogic = new UserLogic();
                List<User> users = new List<User>();
                if (viewModel.User != null)
                {

                    users = userLogic.GetModelsBy(x => x.User_Name.Contains(viewModel.User.Username) && x.Role_Id != 1).OrderBy(x => x.Username).ToList();
                    viewModel.Users = users;
                }
                
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult DeleteStaffAction(int? id)
        {
            try
            {
                UserLogic userLogic = new UserLogic();
                Model.Model.User user = new User();
                List<CourseAllocation> courseAllocations = new List<CourseAllocation>();
                CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                bool isDeleted = false;
                if (id != null)
                {
                    user = userLogic.GetModelBy(x => x.User_Id == id);
                    courseAllocations = courseAllocationLogic.GetModelsBy(x => x.User_Id == user.Id);
                    if (courseAllocations.Count > 0)
                    {
                        SetMessage("Cannot Delete User. Staff has course allocated to him/her", Message.Category.Error);
                        return RedirectToAction("DeleteStaff");
                    }

                    isDeleted = userLogic.Delete(u => u.User_Id == user.Id);
                }
                if (isDeleted == true)
                {
                    SetMessage("Deleted Successfully", Message.Category.Information); 
                }
                else
                {
                    SetMessage("Error", Message.Category.Error);  
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("DeleteStaff");
        }
        public ActionResult AssignDepartmentToUser()
        {
            try
            {
                viewModel = new UserViewModel();
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.CurrentSession = viewModel.ActiveSessionSelectList;
                ViewBag.User = viewModel.ExamOfficersAndHODRoleSelectList;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AssignDepartmentToUser(UserViewModel viewModel)
        {
            try
            {
                
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.CurrentSession = viewModel.ActiveSessionSelectList;
                ViewBag.User = viewModel.ExamOfficersAndHODRoleSelectList;
                UserLogic userLogic = new UserLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                var department=departmentLogic.GetModelBy(f => f.Department_Id == viewModel.Department.Id);
                var user=userLogic.GetModelBy(f => f.User_Id == viewModel.User.Id);
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                StaffLogic staffLogic = new StaffLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                if (viewModel.User.Id > 0)
                {
                    var staff=staffLogic.GetModelBy(f => f.User_Id == viewModel.User.Id);
                    if (staff == null)
                    {
                        staff = CreateStaff(user);
                    }
                    if (!viewModel.RemoveHOD)
                    {
                        if (staff != null)
                        {
                            var staffDepartmentExist = staffDepartmentLogic.GetModelsBy(f => f.Staff_Id != staff.Id && f.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && f.Department_Id == viewModel.Department.Id).FirstOrDefault();
                            if (staffDepartmentExist == null)
                            {
                                var exist = staffDepartmentLogic.GetModelsBy(f => f.Staff_Id == staff.Id && f.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && f.STAFF.USER.Role_Id == user.Role.Id).FirstOrDefault();
                                if (exist == null)
                                {
                                    StaffDepartment staffDepartment = new StaffDepartment();
                                    staffDepartment.DateEntered = DateTime.Now;
                                    staffDepartment.Department = department;
                                    staffDepartment.Staff = staff;
                                    staffDepartment.SessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == viewModel.Session.Id).LastOrDefault();

                                    if (user.Role.Id == (int)UserRoles.HOD)
                                    {
                                        staffDepartment.IsHead = true;
                                    }
                                    else if (user.Role.Id == (int)UserRoles.ExamOfficer)
                                    {
                                        staffDepartment.IsExamOfficer = true;
                                    }
                                    var created=staffDepartmentLogic.Create(staffDepartment);
                                    if (created?.Id>0)
                                    {
                                        SetMessage(user.Username + "has been Added, as the " + user.Role.Name + " of  " + department.Name, Message.Category.Information);
                                        return View(viewModel);
                                    }

                                }
                                else
                                {
                                    SetMessage("User already exists as the " + user.Role.Name + " " + "of " + " " + department.Name, Message.Category.Information);
                                    return View(viewModel);
                                }


                            }
                            else
                            {
                                SetMessage("A user already exist as the " + user.Role.Name + " " + "of " + " " + department.Name, Message.Category.Information);
                                return View(viewModel);
                            }
                        }
                    }
                    else
                    {
                        var staffDepartmentExist = staffDepartmentLogic.GetModelsBy(f => f.Staff_Id == staff.Id && f.SESSION_SEMESTER.Session_Id == viewModel.Session.Id && f.Department_Id == viewModel.Department.Id).FirstOrDefault();
                        if (staffDepartmentExist != null)
                        {
                            if (user.Role.Id == (int)UserRoles.HOD)
                            {
                                staffDepartmentExist.IsHead = false;
                            }
                            else if (user.Role.Id == (int)UserRoles.ExamOfficer)
                            {
                                staffDepartmentExist.IsExamOfficer = false;
                            }
                            var isModified=staffDepartmentLogic.Modify(staffDepartmentExist);
                            if (isModified)
                            {
                                SetMessage( user.Username + "has been removed, as the " + user.Role.Name + " of  " + department.Name, Message.Category.Information);
                                return View(viewModel);
                            }

                        }
                    }
                    
                   
                }
               

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public Staff CreateStaff(User user)
        {
            Staff staff = new Staff();
            StaffLogic staffLogic = new StaffLogic();
            Role role = new Role() { Id = 6 };
            PersonType personType = new PersonType() { Id = 1 };
            Nationality nationality = new Nationality() { Id = 1 };
            staff.FirstName = user.Username;
            staff.LastName = user.Username;
            staff.Role = user.Role;
            staff.Nationality = nationality;
            staff.DateEntered = DateTime.Now;
            staff.Type = personType;
            staff.State = new State() { Id = "ET" };
            Person person = personLogic.Create(staff);
            staff.Id = person.Id;
            staff.User = user;
            staff.StaffType = new StaffType() { Id = 1 };
            staff.MaritalStatus = new MaritalStatus() { Id = 1 };
            
            return staffLogic.Create(staff);


        }
        public ActionResult ViewDeparmentalPrincipalOfficers()
        {
            ViewBag.Session = viewModel.SessionSelectList;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewDeparmentalPrincipalOfficers(UserViewModel userView)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                viewModel.StaffDepartments= staffDepartmentLogic.GetModelsBy(f => f.SESSION_SEMESTER.Session_Id == userView.Session.Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public JsonResult ActivateDeactivatePortfolio(string id, bool status)
        {
            JsonResultModel result = new JsonResultModel();

            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                    var staffDepartmentId = Convert.ToInt32(id);
                    var staffDepartment=staffDepartmentLogic.GetModelBy(f => f.Staff_Department_Id == staffDepartmentId);

                    if (staffDepartment?.Id > 0)
                    {
                        if (status == true)
                        {
                            var otherExistinggOfficer=staffDepartmentLogic.GetModelsBy(f => f.SESSION_SEMESTER.Session_Id == staffDepartment.SessionSemester.Session.Id && f.STAFF.USER.Role_Id == staffDepartment.Staff.User.Role.Id && f.Staff_Department_Id != staffDepartment.Id).ToList();
                            if (otherExistinggOfficer?.Count > 0)
                            {
                                result.IsError = false;
                                result.Message = "This Account can not be activated. Another Account is already Active";
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                        if (staffDepartment.Staff.User.Role.Id == (int)UserRoles.HOD)
                        {
                            staffDepartment.IsHead = status;
                        }
                        else if (staffDepartment.Staff.User.Role.Id == (int)UserRoles.ExamOfficer)
                        {
                            staffDepartment.IsExamOfficer = status;
                        }
                        staffDepartment.DateEntered = DateTime.Now;
                        staffDepartmentLogic.Modify(staffDepartment);
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
        public ActionResult CreateStaffDepartment()
        {
            List<UserStaffRecord> userStaffRecordList = new List<UserStaffRecord>();
            viewModel = new UserViewModel();
            ViewBag.Departments = viewModel.DepartmentSelectList;
            viewModel.Users=userLogic.GetModelsBy(f=>f.Role_Id==(int)UserRoles.CourseStaff);
            if (viewModel.Users?.Count > 0)
            {
                foreach(var item in viewModel.Users)
                {
                    UserStaffRecord userStaffRecord = new UserStaffRecord();
                    userStaffRecord.User = item;
                    var staff=staffLogic.GetModelsBy(f => f.User_Id == item.Id).FirstOrDefault();
                    if(staff?.Id > 0)
                    {
                        userStaffRecord.StaffDepartment=staffDepartmentLogic.GetModelsBy(f => f.Staff_Id == staff.Id).FirstOrDefault();
                    }
                    userStaffRecordList.Add(userStaffRecord);

                }
            }
            viewModel.UserStaffRecords = userStaffRecordList;
            return View(viewModel);
        }
        public JsonResult CreateStaffDepartmentRecord(List<long> UserIdArray, int departmentId)
        {
            JsonResultModel result = new JsonResultModel();

            try
            {
                if (departmentId>0 && UserIdArray.Count > 0)
                {
                    StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                    foreach(var item in UserIdArray)
                    {
                        var user=userLogic.GetModelBy(f => f.User_Id == item);
                        if (user?.Id > 0)
                        {
                            var staff=staffLogic.GetModelsBy(f => f.User_Id == user.Id).FirstOrDefault();
                            if (staff ==null)
                            {
                                staff=CreateStaff(user);
                            }
                            var staffDepartment=staffDepartmentLogic.GetModelsBy(f => f.Staff_Id == staff.Id).FirstOrDefault();
                            if (staffDepartment?.Id > 0)
                            {
                                staffDepartment.Department = new Department() { Id = departmentId };
                                staffDepartmentLogic.Modify(staffDepartment);
                            }
                            else
                            {
                                StaffDepartment newStaffDepartment = new StaffDepartment()
                                {
                                    DateEntered = DateTime.Now,
                                    Department = new Department() { Id = departmentId },
                                    IsExamOfficer = false,
                                    IsHead = false,
                                    Staff = staff,

                                };
                                staffDepartmentLogic.Create(newStaffDepartment);
                            }
                        }
                        
                    }

                    result.IsError = false;
                    result.Message = "Operation Successful";
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    result.IsError = true;
                    result.Message = "Operation Failed. Please provide the requied fields";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddBulkStaff()
        {
            viewModel = new UserViewModel();
            ViewBag.Departments = viewModel.DepartmentSelectList;
            ViewBag.Roles=viewModel.RoleSelectList;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddBulkStaff(UserViewModel userViewModel)
        {
            viewModel = new UserViewModel();
            ViewBag.Departments = viewModel.DepartmentSelectList;
            ViewBag.Roles = viewModel.RoleSelectList;

            viewModel.ShowTable = true;
            List<UserUploadFormat> userUploadFormatList = new List<UserUploadFormat>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                FileUploadURL = savedFileName;
                hpf.SaveAs(savedFileName);
                DataSet userSet = ReadExcel(savedFileName);

                if (userSet != null && userSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < userSet.Tables[0].Rows.Count; i++)
                    {
                        UserUploadFormat userUploadFormat = new UserUploadFormat();
                        userUploadFormat.SN = Convert.ToInt32(userSet.Tables[0].Rows[i][0].ToString().Trim());
                        userUploadFormat.Surname = userSet.Tables[0].Rows[i][1].ToString().Trim();
                        userUploadFormat.FirstName = userSet.Tables[0].Rows[i][2].ToString().Trim();
                        userUploadFormat.OtherName = userSet.Tables[0].Rows[i][3].ToString().Trim();
                        userUploadFormat.Username = userUploadFormat.FirstName + "." + userUploadFormat.Surname;


                        if (!String.IsNullOrEmpty(userUploadFormat.Surname) && !String.IsNullOrEmpty(userUploadFormat.FirstName))
                        {
                            userUploadFormatList.Add(userUploadFormat);
                        }

                    }
                    viewModel.Role= roleLogic.GetModelBy(f => f.Role_Id == userViewModel.Role.Id);
                    viewModel.Department = departmentLogic.GetModelBy(f => f.Department_Id == userViewModel.Department.Id);
                    viewModel.UserUploadFormats = userUploadFormatList;
                    TempData["userViewmodel"] = viewModel;

                }

            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult SaveBulkStaff()
        {
            UserViewModel viewModel =
                (UserViewModel)TempData["userViewmodel"];
            int count = 0;
            try
            {
                if (viewModel?.UserUploadFormats?.Count > 0)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var item in viewModel.UserUploadFormats)
                        {
                            if (!String.IsNullOrEmpty(item.Username))
                            {
                                var existUser = userLogic.GetModelsBy(f => f.User_Name == item.Username).FirstOrDefault();
                                if (existUser == null)
                                {
                                    User user = new User()
                                    {
                                        LastLoginDate = DateTime.Now,
                                        Password = "1@password",
                                        Role = new Role() { Id = viewModel.Role.Id },
                                        SecurityAnswer = "Am a lecturer",
                                        SecurityQuestion = new SecurityQuestion() { Id = 1 },
                                        Username = item.Username,
                                    };
                                    var userCreated = userLogic.Create(user);
                                    if (userCreated?.Id > 0)
                                    {
                                        userCreated.Role = viewModel.Role;
                                        var staff = CreateStaffBulk(userCreated, item);
                                        if (staff?.Id > 0)
                                        {
                                            StaffDepartment staffDepartment = new StaffDepartment()
                                            {
                                                DateEntered = DateTime.Now,
                                                Department = new Department() { Id = viewModel.Department.Id },
                                                Staff = new Staff() { Id = staff.Id }
                                            };
                                            staffDepartmentLogic.Create(staffDepartment);
                                        }
                                        count += 1;
                                    }
                                }
                            }
                        }
                        scope.Complete();
                    }
                    SetMessage(count+ " record(s) was(were) Uploaded successfully.", Message.Category.Information);
                    return RedirectToAction("AddBulkStaff");
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            SetMessage("Something went wrong", Message.Category.Information);
            return RedirectToAction("AddBulkStaff");
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
        public Staff CreateStaffBulk(User user,UserUploadFormat userUploadFormat)
        {
            Staff staff = new Staff();
            StaffLogic staffLogic = new StaffLogic();
            Role role = new Role() { Id = 6 };
            PersonType personType = new PersonType() { Id = 1 };
            Nationality nationality = new Nationality() { Id = 1 };
            staff.FirstName = userUploadFormat.FirstName;
            staff.LastName = userUploadFormat.Surname;
            staff.OtherName = userUploadFormat.OtherName;
            staff.Role = user.Role;
            staff.Nationality = nationality;
            staff.DateEntered = DateTime.Now;
            staff.Type = personType;
            staff.State = new State() { Id = "ET" };
            Person person = personLogic.Create(staff);
            staff.Id = person.Id;
            staff.User = user;
            staff.StaffType = new StaffType() { Id = 1 };
            staff.MaritalStatus = new MaritalStatus() { Id = 1 };

            return staffLogic.Create(staff);


        }

    }
}