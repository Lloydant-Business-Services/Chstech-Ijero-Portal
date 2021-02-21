using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StudentController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];

        private StudentViewModel _viewModel;
        public ActionResult SearchCriteria()
        {
            try
            {
                _viewModel = new StudentViewModel();

                _viewModel.ProgrammeSelectList.Where(p => string.IsNullOrEmpty(p.Value)).LastOrDefault().Text = "All";
                _viewModel.SessionSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.DepartmentSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.ModeOfEntrySelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.StudentStatusSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.AdmittedSessionSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.CountrySelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.LocalGovernmentSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";
                _viewModel.StateSelectList.Where(s => string.IsNullOrEmpty(s.Value)).LastOrDefault().Text = "All";

                ViewBag.Programme = _viewModel.ProgrammeSelectList;
                ViewBag.Session = _viewModel.SessionSelectList;
                ViewBag.Department = _viewModel.DepartmentSelectList;
                ViewBag.Confirmed = GetStudentStatus();
                ViewBag.ModeOfEntry = _viewModel.ModeOfEntrySelectList;
                ViewBag.Status = _viewModel.StudentStatusSelectList;
                ViewBag.AdmissionSet = _viewModel.AdmittedSessionSelectList;
                ViewBag.Country = _viewModel.CountrySelectList;
                ViewBag.LocalGovernment = _viewModel.LocalGovernmentSelectList;
                ViewBag.State = _viewModel.StateSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(_viewModel);
        }
        public ContentResult GetStudents(string searchParameter)
        {
            StudentJsonResult result = new StudentJsonResult();
            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            try
            {
                SearchModel searchCriteria = serializer.Deserialize<SearchModel>(searchParameter);

                if (searchCriteria != null)
                {
                    List<StudentInformation> studentInfo = GetStudentInformation(searchCriteria);
                    result.StudentInformation = studentInfo;
                    result.IsError = false;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Parameter not set.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            var newResult = serializer.Serialize(result);

            var returnData = new ContentResult
            {
                Content = newResult,
                ContentType = "application/json"
            };

            return returnData;
        }

        private List<StudentInformation> GetStudentInformation(SearchModel searchCrieria)
        {
            List<StudentInformation> masterInfo = new List<StudentInformation>();

            try
            {
                StudentLogic studentLogic = new StudentLogic();
                List<StudentInformation> studentInfo = studentLogic.GetStudentInfo();

                if (studentInfo == null)
                    return null;

                Type modelType = searchCrieria.GetType();
                PropertyInfo[] arrayPropertyInfos = modelType.GetProperties();

                foreach (PropertyInfo property in arrayPropertyInfos)
                {
                    switch (property.Name)
                    {
                        case "SessionId":
                            int sessionId = !string.IsNullOrEmpty(searchCrieria.SessionId.Trim()) ? Convert.ToInt32(searchCrieria.SessionId.Trim()) : 0;
                            if (sessionId > 0)
                                studentInfo = studentInfo.Where(s => s.SessionId == sessionId).ToList();
                            break;
                        case "ProgrammeId":
                            int programmeId = !string.IsNullOrEmpty(searchCrieria.ProgrammeId.Trim()) ? Convert.ToInt32(searchCrieria.ProgrammeId.Trim()) : 0;
                            if (programmeId > 0)
                                studentInfo = studentInfo.Where(s => s.ProgrammeId == programmeId).ToList();
                            break;
                        case "ApplicationNumber":
                            string applicationNumber = searchCrieria.ApplicationNumber.Trim();
                            if (!string.IsNullOrEmpty(applicationNumber))
                                studentInfo = studentInfo.Where(s => s.ApplicationFormNumber == applicationNumber).ToList();
                            break;
                        case "FirstName":
                            string firstName = searchCrieria.FirstName.Trim();
                            if (!string.IsNullOrEmpty(firstName))
                                studentInfo = studentInfo.Where(s => s.Name.Contains(firstName)).ToList();
                            break;
                        case "StateId":
                            string stateId = searchCrieria.StateId.Trim();
                            if (!string.IsNullOrEmpty(stateId))
                                studentInfo = studentInfo.Where(s => s.StateId == stateId).ToList();
                            break;
                        case "ModeOfEntryId":
                            int modeOfEntryId = !string.IsNullOrEmpty(searchCrieria.ModeOfEntryId.Trim()) ? Convert.ToInt32(searchCrieria.ModeOfEntryId.Trim()) : 0;
                            if (modeOfEntryId > 0)
                                studentInfo = studentInfo.Where(s => s.ModeOfEntryId == modeOfEntryId).ToList();
                            break;
                        case "DepartmentId":
                            int departmentId = !string.IsNullOrEmpty(searchCrieria.DepartmentId.Trim()) ? Convert.ToInt32(searchCrieria.DepartmentId) : 0;
                            if (departmentId > 0)
                                studentInfo = studentInfo.Where(s => s.DepartmentId == departmentId).ToList();
                            break;
                        case "Confirmed":
                            int confirmed = !string.IsNullOrEmpty(searchCrieria.Confirmed.Trim()) ? Convert.ToInt32(searchCrieria.Confirmed.Trim()) : 0;
                            if (confirmed == 1)
                                studentInfo = studentInfo.Where(s => s.Activated == true || s.Activated == null).ToList();
                            else if (confirmed == 2)
                                studentInfo = studentInfo.Where(s => s.Activated == false).ToList();
                            break;
                        case "LastName":
                            string lastName = searchCrieria.LastName.Trim();
                            if (!string.IsNullOrEmpty(lastName))
                                studentInfo = studentInfo.Where(s => s.Name.Contains(lastName)).ToList();
                            break;
                        case "StatusId":
                            int statusId = !string.IsNullOrEmpty(searchCrieria.StatusId.Trim()) ? Convert.ToInt32(searchCrieria.StatusId.Trim()) : 0;
                            if (statusId > 0)
                                studentInfo = studentInfo.Where(s => s.StudentStatusId == statusId).ToList();
                            break;
                        case "AdmissionSetId":
                            int admissionSetId = !string.IsNullOrEmpty(searchCrieria.AdmissionSetId.Trim()) ? Convert.ToInt32(searchCrieria.AdmissionSetId.Trim()) : 0;
                            if (admissionSetId > 0)
                                studentInfo = studentInfo.Where(s => s.AdmittedSessionId == admissionSetId).ToList();
                            break;
                        case "MatricNumber":
                            string matricNumber = searchCrieria.MatricNumber.Trim();
                            if (!string.IsNullOrEmpty(matricNumber))
                                studentInfo = studentInfo.Where(s => s.MatricNumber == matricNumber).ToList();
                            break;
                        //case "CountryId":
                        //    string countryId = searchCrieria.CountryId;
                        //    if (!string.IsNullOrEmpty(countryId))
                        //        studentInfo = studentInfo.Where(s => s.NationalityId == countryId).ToList();
                        //    break;
                        case "LocalGovernmentId":
                            int localGovernmentId = !string.IsNullOrEmpty(searchCrieria.LocalGovernmentId.Trim()) ? Convert.ToInt32(searchCrieria.LocalGovernmentId.Trim()) : 0;
                            if (localGovernmentId > 0)
                                studentInfo = studentInfo.Where(s => s.LocalGovernmentId == localGovernmentId).ToList();
                            break;
                    }
                }

                masterInfo = studentInfo != null && studentInfo.Count > 0 ? studentInfo.OrderBy(s => s.ProgrammeId).ThenBy(s => s.DepartmentId).ThenBy(s => s.LevelId).ThenBy(s => s.MatricNumber).ToList() : null;
            }
            catch (Exception)
            {
                throw;
            }

            return masterInfo;
        }

        public List<SelectListItem> GetStudentStatus()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            try
            {
                status.Add(new SelectListItem() { Text = "All", Value = "" });
                status.Add(new SelectListItem() { Text = "Yes", Value = "1" });
                status.Add(new SelectListItem() { Text = "No", Value = "2" });
            }
            catch (Exception)
            {
                throw;
            }

            return status;
        }
        public JsonResult GetDepartments(string programmeId)
        {
            try
            {
                if (string.IsNullOrEmpty(programmeId))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(programmeId) };

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetStates(string countryId)
        {
            try
            {
                if (string.IsNullOrEmpty(countryId))
                {
                    return null;
                }

                List<State> states = new List<State>();

                StateLogic stateLogic = new StateLogic();

                if (countryId == "NIG")
                    states = stateLogic.GetModelsBy(s => s.State_Id != "OT");
                else if (countryId == "OTH")
                {
                    State state = new State { Id = "0", Name = "OTHERS" };
                    states.Add(state);
                }
                else
                    return null;

                return Json(new SelectList(states, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetLocalGovernment(string stateId)
        {
            try
            {
                if (string.IsNullOrEmpty(stateId))
                {
                    return null;
                }

                List<LocalGovernment> lgas = new List<LocalGovernment>();
                LocalGovernmentLogic localGovernmentLogic = new LocalGovernmentLogic();

                lgas = localGovernmentLogic.GetModelsBy(l => l.State_Id == stateId);

                return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult UpdateStudentLevel()
        {
            try
            {
                _viewModel = new StudentViewModel();

                ViewBag.Programme = _viewModel.ProgrammeSelectList;
                ViewBag.Session = _viewModel.SessionSelectList;
                ViewBag.Level = _viewModel.LevelSelectList;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(_viewModel);
        }

        [HttpPost]
        public ActionResult UpdateStudentLevel(StudentViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.Programme != null && viewModel.Session != null && viewModel.Level != null && viewModel.NewLevel != null && viewModel.NewSession != null)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                    if (!string.IsNullOrEmpty(viewModel.MatricNumberWildCard))
                    {
                        viewModel.StudentLevelList = studentLevelLogic.GetModelsBy(s => s.Programme_Id == viewModel.Programme.Id && s.Session_Id == viewModel.Session.Id &&
                                                                        s.Level_Id == viewModel.Level.Id && s.STUDENT.Matric_Number.Contains(viewModel.MatricNumberWildCard));
                    }
                    else
                    {
                        viewModel.StudentLevelList = studentLevelLogic.GetModelsBy(s => s.Programme_Id == viewModel.Programme.Id && s.Session_Id == viewModel.Session.Id && s.Level_Id == viewModel.Level.Id);
                    }

                    GeneralAudit generalAudit = new GeneralAudit();
                    GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();

                    for (int i = 0; i < viewModel.StudentLevelList.Count; i++)
                    {
                        int oldLevelId = viewModel.StudentLevelList[i].Level.Id;
                        int oldSessionId = viewModel.StudentLevelList[i].Session.Id;

                        viewModel.StudentLevelList[i].Level = viewModel.NewLevel;
                        viewModel.StudentLevelList[i].Session = viewModel.NewSession;

                        studentLevelLogic.ModifyById(viewModel.StudentLevelList[i]);
                    }

                    ProgrammeLogic programmeLogic = new ProgrammeLogic();

                    //Create Audit For Operation
                    generalAudit.Action = "Update Student Level";
                    generalAudit.InitialValues = "Level_Id: " + viewModel.Level.Id + ", Session_Id: " + viewModel.Session.Id;
                    generalAudit.CurrentValues = "Level_Id: " + viewModel.NewLevel.Id + ", Session_Id: " + viewModel.NewSession.Id;
                    generalAudit.Operation = "Updated student level records for programme: " + programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id).Name;
                    generalAudit.TableNames = "StudentLevel Table";

                    generalAuditLogic.CreateGeneralAudit(generalAudit);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
                else
                {
                    SetMessage("Error! Invalid parameter.", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Programme = viewModel.ProgrammeSelectList;
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Level = viewModel.LevelSelectList;

            return View(viewModel);
        }
        //[HttpPost]
        //public ActionResult SaveStudentLevel(StudentViewModel viewModel)
        //{
        //    try
        //    {
        //        if (viewModel != null && viewModel.StudentLevelList != null)
        //        {
        //            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

        //            viewModel.StudentLevelList = viewModel.StudentLevelList.Where(s => s.Student.Activated == true).ToList();

        //            for (int i = 0; i < viewModel.StudentLevelList.Count; i++)
        //            {
        //                long studentLevelId = viewModel.StudentLevelList[i].Id;

        //                StudentLevel existingStudentLevel = studentLevelLogic.GetModelBy(s => s.Student_Level_Id == studentLevelId);

        //                existingStudentLevel.Level = viewModel.NewLevel;

        //                studentLevelLogic.Modify(existingStudentLevel);
        //            }

        //            SetMessage("Operation Successful! ", Message.Category.Information);
        //        }
        //        else
        //        {
        //            SetMessage("Error! No record in the list.", Message.Category.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error! " + ex.Message, Message.Category.Error);
        //    }

        //    return RedirectToAction("UpdateStudentLevel");
        //}

        public ActionResult Information()
        {
            try
            {
                UploadAdmissionViewModel viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
                ViewBag.LevelId = viewmodel.LevelSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }

            return View();
        }
        [HttpPost]
        public ActionResult Information(UploadAdmissionViewModel viewModel)
        {
            try
            {
                List<StudentDetailsModel> details = new List<StudentDetailsModel>();
                StudentLogic studentLogic = new StudentLogic();

                if (viewModel.IsBulk && viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Level != null && viewModel.Level.Id > 0 && viewModel.CurrentSession != null && viewModel.CurrentSession.Id > 0)
                {
                    details = studentLogic.GetStudentDetails(viewModel.Programme, viewModel.Level, viewModel.CurrentSession);
                }
                else if (viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Department != null && viewModel.Department.Id > 0 && viewModel.CurrentSession != null && viewModel.CurrentSession.Id > 0)
                {
                    details = studentLogic.GetStudentDetails(viewModel.Programme, viewModel.Department, viewModel.DepartmentOption, viewModel.Level, viewModel.CurrentSession);
                }

                GridView gv = new GridView();
                DataTable ds = new DataTable();
                if (details.Count > 0)
                {
                    List<StudentDetailsModel> list = details.OrderBy(p => p.ApplicationNumber).ToList();
                    List<StudentDetailsDisplay> sort = new List<StudentDetailsDisplay>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        StudentDetailsDisplay student = new StudentDetailsDisplay();
                        student.SN = (i + 1);
                        student.Name = list[i].Name;

                        student.MatricNumber = list[i].MatricNumber != null ? list[i].MatricNumber.ToUpper() : "";
                        student.ProgrammeName = list[i].ProgrammeName != null ? list[i].ProgrammeName.ToUpper() : "";
                        student.DepartmentName = list[i].DepartmentName != null ? list[i].DepartmentName.ToUpper() : "";
                        student.NationalityName = list[i].NationalityName != null ? list[i].NationalityName.ToUpper() : "";
                        student.StateName = list[i].StateName != null ? list[i].StateName.ToUpper() : "";
                        student.LocalGovernmentName = list[i].LocalGovernmentName != null ? list[i].LocalGovernmentName.ToUpper() : "";
                        student.HomeTown = list[i].HomeTown != null ? list[i].HomeTown.ToUpper() : "";

                        student.Genotype = list[i].Genotype;
                        student.SexName = list[i].SexName != null ? list[i].SexName.ToUpper() : "";
                        student.Email = list[i].Email != null ? list[i].Email.ToUpper() : "";
                        student.MobilePhone = list[i].MobilePhone;
                        student.DateOfBirth = list[i].DateOfBirth;

                        sort.Add(student);
                    }

                    gv.DataSource = sort;
                    string caption = "Report for " + list.FirstOrDefault().SessionName + " Session in " + list.FirstOrDefault().ProgrammeName + " " + " DEPARTMENT OF " + " " +
                                   list.FirstOrDefault().DepartmentName;

                    gv.Caption = caption.ToUpper();
                    gv.DataBind();

                    string filename = caption.Replace("\\", "") + ".xls";
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
            return RedirectToAction("Information");
        }

        [AllowAnonymous]
        public ActionResult UpdateStudentData()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateStudentData(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.MatricNumber != null)
                {
                    var studentLogic = new StudentLogic();
                    var student = studentLogic.GetModelsBy(st => st.Matric_Number == viewModel.MatricNumber).FirstOrDefault();

                    if (student != null)
                    {
                        //Fill the Details with Data from Student And StudentLevel
                        viewModel = SetStudentDataFromStudentTable(viewModel, student);
                    }
                    else
                    {
                        var applicationFormLogic = new ApplicationFormLogic();
                        var applicationForm = applicationFormLogic.GetModelsBy(ap => ap.Application_Form_Number == viewModel.MatricNumber).FirstOrDefault();

                        if (applicationForm != null)
                        {
                            //Pull Details from ApplicationForm/AdmissionList Table
                            viewModel = SetStudentDataFromApplicationTable(viewModel, applicationForm);
                        }
                        else
                        {
                            SetMessage("No student matches this Matric Number/Application Number", Message.Category.Error);
                        }
                    }
                }
                else
                {
                    SetMessage("No student matches this Matric Number", Message.Category.Error);
                }

                ViewBag.Sexes = Utility.PopulateSexSelectListItem();
                ViewBag.States = Utility.PopulateStateSelectListItem();
                ViewBag.Departments = Utility.PopulateDepartmentSelectListItem();
                ViewBag.Levels = Utility.PopulateLevelSelectListItem();
                ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
                ViewBag.Relationships = Utility.PopulateRelationshipSelectListItem();
                ViewBag.Nationlities = Utility.PopulateNationalitySelectListItem();
                ViewBag.HOD = Utility.PopulateDepartmentHeadSelectListItem();

                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        public StudentCourseRegistrationViewModel SetStudentDataFromStudentTable(StudentCourseRegistrationViewModel viewModel, Model.Model.Student student)
        {
            if (student != null)
            {
                viewModel.UpdateRegistration = new UpdateRegistrationViewModel();
                var personLogic = new PersonLogic();
                var person = personLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();

                if (person != null)
                {
                    viewModel.UpdateRegistration.PersonId = person.Id;
                    viewModel.UpdateRegistration.FirstName = person.FirstName;
                    viewModel.UpdateRegistration.OtherName = person.OtherName;
                    viewModel.UpdateRegistration.LastName = person.LastName;
                    if (person.Sex != null)
                    {
                        viewModel.UpdateRegistration.Sex = person.Sex.Id;
                    }
                    viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
                    viewModel.UpdateRegistration.EmailAddress = person.Email;
                    viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
                    viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

                    viewModel.UpdateRegistration.StateId = person.State.Id;
                    if (person.DateOfBirth != null)
                    {
                        viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
                    }

                    var applicantSponsorLogic = new ApplicantSponsorLogic();
                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
                                                                .Select(ap => new
                                                                {
                                                                    ap.Relationship_Id,
                                                                    ap.Sponsor_Name
                                                                })
                                                                .FirstOrDefault();
                    if (applicantSponsor != null)
                    {
                        viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
                        viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
                    }

                    var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                    var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
                                                                                    .Select(ap => new { ap.Previous_School_Name })
                                                                                    .FirstOrDefault();

                    if (applicantPreviousEducation != null)
                    {
                        viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
                    }

                    var studentLevelLogic = new StudentLevelLogic();
                    var studentLevel = studentLevelLogic.GetModelsBy(st => st.Person_Id == student.Id).FirstOrDefault();

                    if (studentLevel != null)
                    {
                        if (studentLevel.Department == null || studentLevel.Session == null || studentLevel.Level != null)
                        {
                            var applicationFormLogic = new ApplicationFormLogic();
                            var applicationForm = applicationFormLogic.GetModelsBy(ap => ap.Person_Id == person.Id)
                                                                    .Select(ap => new { ap.Id })
                                                                    .FirstOrDefault();
                            if (applicationForm != null)
                            {
                                var admissionListLogic = new AdmissionListLogic();
                                var admissionList = admissionListLogic.GetModelsBy(ad => ad.Application_Form_Id == applicationForm.Id).FirstOrDefault();
                                if (admissionList != null)
                                {
                                    viewModel.UpdateRegistration.DepartmentId = admissionList.Deprtment.Id;
                                    //viewModel.UpdateRegistration.LevelId = admissionList.Level.Id;
                                    viewModel.UpdateRegistration.SessionId = admissionList.Session.Id;
                                    int programmeId = admissionList.Programme.Id;

                                    if (programmeId > 0)
                                    {
                                        if (programmeId == 1 || programmeId == 2)
                                        {
                                            viewModel.UpdateRegistration.LevelId = 1;
                                        }
                                        if (programmeId == 3 || programmeId == 4)
                                        {
                                            viewModel.UpdateRegistration.LevelId = 2;
                                        }
                                    }
                                }
                            }
                        }
                        if (studentLevel.Department != null && studentLevel.Session != null && studentLevel.Level != null)
                        {
                            viewModel.UpdateRegistration.DepartmentId = studentLevel.Department.Id;
                            viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
                            viewModel.UpdateRegistration.SessionId = studentLevel.Session.Id;
                        }

                        var departmentHeadLogic = new DepartmentHeadLogic();
                        var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == studentLevel.Department.Id)
                                                                .Select(d => new { Department_Head_Id = d.DepartmentHeadId })
                                                                .FirstOrDefault();

                        if (departmentHead != null)
                        {
                            viewModel.UpdateRegistration.HOD = departmentHead.Department_Head_Id;
                        }
                    }
                }
            }

            return viewModel;
        }

        public StudentCourseRegistrationViewModel SetStudentDataFromApplicationTable(StudentCourseRegistrationViewModel viewModel, ApplicationForm applicationForm)
        {
            try
            {
                if (applicationForm != null)
                {
                    viewModel.UpdateRegistration = new UpdateRegistrationViewModel();

                    var personLogic = new PersonLogic();
                    var person = personLogic.GetModelsBy(p => p.Person_Id == applicationForm.Person.Id).FirstOrDefault();

                    if (person != null)
                    {
                        viewModel.UpdateRegistration.PersonId = person.Id;
                        viewModel.UpdateRegistration.FirstName = person.FirstName;
                        viewModel.UpdateRegistration.OtherName = person.OtherName;
                        viewModel.UpdateRegistration.LastName = person.LastName;
                        if (person.Sex != null)
                        {
                            viewModel.UpdateRegistration.Sex = person.Sex.Id;
                        }
                        viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
                        viewModel.UpdateRegistration.EmailAddress = person.Email;
                        viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
                        viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
                        viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

                        viewModel.UpdateRegistration.StateId = person.State.Id;
                        if (person.DateOfBirth != null)
                        {
                            viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
                        }
                        var applicantSponsorLogic = new ApplicantSponsorLogic();
                        var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == applicationForm.Person.Id)
                                                                    .Select(ap => new
                                                                    {
                                                                        ap.Relationship_Id,
                                                                        ap.Sponsor_Name
                                                                    })
                                                                    .FirstOrDefault();
                        if (applicantSponsor != null)
                        {
                            viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
                            viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
                        }

                        var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                        var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == applicationForm.Person.Id)
                                                                                        .Select(ap => new { ap.Previous_School_Name })
                                                                                        .FirstOrDefault();

                        if (applicantPreviousEducation != null)
                        {
                            viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
                        }

                        var admissionListLogic = new AdmissionListLogic();
                        var admissionList = admissionListLogic.GetModelsBy(ad => ad.Application_Form_Id == applicationForm.Id).FirstOrDefault();


                        if (admissionList != null)
                        {
                            if (admissionList.Session == null || admissionList.Programme == null)
                            {
                                //Try Check in studentLevel table
                                var studentLevelLogic = new StudentLevelLogic();
                                var studentLevel = studentLevelLogic.GetModelsBy(st => st.Person_Id == person.Id).FirstOrDefault();
                                if (studentLevel != null)
                                {
                                    viewModel.UpdateRegistration.DepartmentId = studentLevel.Department.Id;
                                    viewModel.UpdateRegistration.SessionId = studentLevel.Session.Id;
                                    viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
                                }
                            }
                            if (admissionList.Session != null && admissionList.Programme != null)
                            {
                                viewModel.UpdateRegistration.DepartmentId = (int)admissionList.Deprtment.Id;
                                viewModel.UpdateRegistration.SessionId = (int)admissionList.Session.Id;

                                if (admissionList.Programme.Id > 0)
                                {
                                    if (admissionList.Programme.Id == 1 || admissionList.Programme.Id == 2)
                                    {
                                        viewModel.UpdateRegistration.LevelId = 1;
                                    }
                                    if (admissionList.Programme.Id == 3 || admissionList.Programme.Id == 4)
                                    {
                                        viewModel.UpdateRegistration.LevelId = 2;
                                    }
                                }
                            }


                            var departmentHeadLogic = new DepartmentHeadLogic();
                            var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == admissionList.Deprtment.Id)
                                                                    .Select(d => new { Department_Head_Id = d.DepartmentHeadId })
                                                                    .FirstOrDefault();

                            if (departmentHead != null)
                            {
                                viewModel.UpdateRegistration.HOD = departmentHead.Department_Head_Id;
                            }
                        }
                    }
                }
                return viewModel;
            }
            catch (Exception ex) { throw ex; }
        }

        public StudentCourseRegistrationViewModel ModifyStudentLevel(StudentCourseRegistrationViewModel viewModel, Person person, StudentLevel studentLevel)
        {
            try
            {
                //var studentLevelLogic = new StudentLevelLogic();

                //if (viewModel.UpdateRegistration.DepartmentId > 0)
                //{
                //    studentLevel.Department.Id = viewModel.UpdateRegistration.DepartmentId;
                //}
                //if (viewModel.UpdateRegistration.SessionId > 0)
                //{
                //    studentLevel.Session.Id = viewModel.UpdateRegistration.SessionId;
                //}
                //if (viewModel.UpdateRegistration.LevelId > 0)
                //{
                //    studentLevel.Level.Id = viewModel.UpdateRegistration.LevelId;
                //}

                //studentLevelLogic.Modify(studentLevel);

                var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(a => a.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();

                if (applicantPreviousEducation != null)
                {
                    applicantPreviousEducation.Previous_School_Name = viewModel.UpdateRegistration.SchoolName;
                    applicantPreviousEducationLogic.Modify(applicantPreviousEducation);
                }

                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.ParentGuardian))
                {
                    var applicantSponsorLogic = new ApplicantSponsorLogic();
                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(a => a.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                    if (applicantSponsor != null)
                    {
                        //Update the existing entry
                        applicantSponsor.Sponsor_Name = viewModel.UpdateRegistration.ParentGuardian;
                        applicantSponsor.Relationship_Id = viewModel.UpdateRegistration.RelationshipId;
                        applicantSponsorLogic.Modify(applicantSponsor);
                    }
                    else
                    {
                        //create a new Entry
                        var applicationFormLogic = new ApplicationFormLogic();
                        var applicationForm = applicationFormLogic.GetModelsBy(ap => ap.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                        if (applicationForm != null && viewModel.UpdateRegistration.RelationshipId > 0)
                        {
                            var newApplicantSponsor = new ApplicantSponsor()
                            {
                                Person = new Person() { Id = viewModel.UpdateRegistration.PersonId },
                                ApplicationForm = new ApplicationForm() { Id = applicationForm.Id },
                                Relationship = new Relationship() { Id = viewModel.UpdateRegistration.RelationshipId },
                                Sponsor_Name = viewModel.UpdateRegistration.ParentGuardian,
                                Sponsor_Contact_Address = "",
                                Sponsor_Mobile_Phone = ""
                            };

                            applicantSponsorLogic.Create(newApplicantSponsor);
                        }
                    }
                }
                return viewModel;
            }
            catch (Exception ex) { throw ex; }
        }

        public StudentCourseRegistrationViewModel ModifyApplicantLevel(StudentCourseRegistrationViewModel viewModel, Person person, AdmissionList admissionList)
        {
            try
            {
                //var admissionListLogic = new AdmissionListLogic();
                //if (viewModel.UpdateRegistration.DepartmentId > 0)
                //{
                //    admissionList.Deprtment.Id = viewModel.UpdateRegistration.DepartmentId;
                //}
                //if (viewModel.UpdateRegistration.SessionId > 0)
                //{
                //    admissionList.Session.Id = viewModel.UpdateRegistration.SessionId;
                //}
                ////if (viewModel.UpdateRegistration.LevelId > 0)
                ////{
                ////    studentLevel.Level.Id = viewModel.UpdateRegistration.LevelId;
                ////}

                //admissionListLogic.Modify(admissionList);

                var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(a => a.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();

                if (applicantPreviousEducation != null)
                {
                    applicantPreviousEducation.Previous_School_Name = viewModel.UpdateRegistration.SchoolName;
                    applicantPreviousEducationLogic.Modify(applicantPreviousEducation);
                }

                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.ParentGuardian))
                {
                    var applicantSponsorLogic = new ApplicantSponsorLogic();
                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(a => a.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                    if (applicantSponsor != null)
                    {
                        //Update the existing entry
                        applicantSponsor.Sponsor_Name = viewModel.UpdateRegistration.ParentGuardian;
                        applicantSponsor.Relationship_Id = viewModel.UpdateRegistration.RelationshipId;
                        applicantSponsorLogic.Modify(applicantSponsor);
                    }
                    else
                    {
                        //create a new Entry
                        var applicationFormLogic = new ApplicationFormLogic();
                        var newApplicantSponsor = new ApplicantSponsor()
                        {
                            Person = new Person() { Id = viewModel.UpdateRegistration.PersonId },
                            ApplicationForm = new ApplicationForm() { Id = admissionList.Form.Id },
                            Relationship = new Relationship() { Id = viewModel.UpdateRegistration.RelationshipId },
                            Sponsor_Name = viewModel.UpdateRegistration.ParentGuardian,
                            Sponsor_Contact_Address = "",
                            Sponsor_Mobile_Phone = ""
                        };
                        applicantSponsorLogic.Create(newApplicantSponsor);

                        //var applicationForm = applicationFormLogic.GetModelsBy(ap => ap.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                        //if (applicationForm != null && viewModel.UpdateRegistration.RelationshipId > 0)
                        //{
                        //    var newApplicantSponsor = new ApplicantSponsor()
                        //    {
                        //        Person = new Person() { Id = viewModel.UpdateRegistration.PersonId },
                        //        ApplicationForm = new ApplicationForm() { Id = applicationForm.Id },
                        //        Relationship = new Relationship() { Id = viewModel.UpdateRegistration.RelationshipId },
                        //        Sponsor_Name = viewModel.UpdateRegistration.ParentGuardian,
                        //        Sponsor_Contact_Address = "",
                        //        Sponsor_Mobile_Phone = ""
                        //    };

                        //    applicantSponsorLogic.Create(newApplicantSponsor);
                        //}
                    }
                }

                return viewModel;
            }
            catch (Exception ex) { throw ex; }
        }

        public StudentCourseRegistrationViewModel PreviewStudentDetailsFromStudentTable(StudentCourseRegistrationViewModel viewModel, Model.Model.Student student)
        {
            try
            {
                viewModel.UpdateRegistration = new UpdateRegistrationViewModel();
                viewModel.UpdateRegistration.MatricNumber = student.MatricNumber;
                var personLogic = new PersonLogic();
                var person = personLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();

                if (person != null)
                {
                    viewModel.UpdateRegistration.PersonId = person.Id;
                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;
                    viewModel.UpdateRegistration.FirstName = person.FirstName;
                    viewModel.UpdateRegistration.OtherName = person.OtherName;
                    viewModel.UpdateRegistration.LastName = person.LastName;
                    viewModel.UpdateRegistration.Sex = person.Sex.Id;
                    viewModel.UpdateRegistration.SexName = person.Sex.Name;
                    viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
                    viewModel.UpdateRegistration.EmailAddress = person.Email;
                    viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
                    viewModel.UpdateRegistration.NationalityName = person.Nationality.Name;
                    viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

                    viewModel.UpdateRegistration.StateId = person.State.Id;
                    viewModel.UpdateRegistration.StateName = person.State.Name;
                    if (person.DateOfBirth != null)
                    {
                        viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
                    }

                    var applicantSponsorLogic = new ApplicantSponsorLogic();
                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
                                                                .Select(ap => new {
                                                                    ap.Relationship_Id,
                                                                    ap.Sponsor_Name
                                                                })
                                                                .FirstOrDefault();
                    if (applicantSponsor != null)
                    {
                        viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
                        viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
                    }

                    var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                    var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
                                                                                    .Select(ap => new { ap.Previous_School_Name })
                                                                                    .FirstOrDefault();

                    if (applicantPreviousEducation != null)
                    {
                        viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
                    }

                    var studentLevelLogic = new StudentLevelLogic();
                    var studentLevel = studentLevelLogic.GetModelsBy(st => st.Person_Id == student.Id).FirstOrDefault();

                    if (studentLevel != null)
                    {
                        viewModel.UpdateRegistration.DepartmentId = studentLevel.Department.Id;
                        viewModel.UpdateRegistration.DepartmentName = studentLevel.Department.Name;
                        viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
                        viewModel.UpdateRegistration.LevelName = studentLevel.Level.Name;
                        viewModel.UpdateRegistration.SessionId = studentLevel.Session.Id;
                        viewModel.UpdateRegistration.SessionName = studentLevel.Session.Name;

                        var departmentHeadLogic = new DepartmentHeadLogic();
                        var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == studentLevel.Department.Id)
                                                                //.Select(d => new { Department_Head_Id = d.DepartmentHeadId })
                                                                .FirstOrDefault();

                        if (departmentHead != null)
                        {
                            viewModel.UpdateRegistration.HOD = departmentHead.DepartmentHeadId;
                            viewModel.UpdateRegistration.HODName = departmentHead.Person.FullName;
                        }
                    }
                }

                return viewModel;
            }
            catch (Exception ex) { throw ex; }
        }

        public StudentCourseRegistrationViewModel PreviewStudentDetailsFromApplicantTable(StudentCourseRegistrationViewModel viewModel, ApplicationForm applicationForm)
        {
            try
            {
                viewModel.UpdateRegistration = new UpdateRegistrationViewModel();
                viewModel.UpdateRegistration.MatricNumber = applicationForm.Number;
                var personLogic = new PersonLogic();
                var person = personLogic.GetModelsBy(p => p.Person_Id == applicationForm.Person.Id).FirstOrDefault();

                if (person != null)
                {
                    viewModel.UpdateRegistration.PersonId = person.Id;
                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;
                    viewModel.UpdateRegistration.FirstName = person.FirstName;
                    viewModel.UpdateRegistration.OtherName = person.OtherName;
                    viewModel.UpdateRegistration.LastName = person.LastName;
                    viewModel.UpdateRegistration.Sex = person.Sex.Id;
                    viewModel.UpdateRegistration.SexName = person.Sex.Name;
                    viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
                    viewModel.UpdateRegistration.EmailAddress = person.Email;
                    viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
                    viewModel.UpdateRegistration.NationalityName = person.Nationality.Name;
                    viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

                    viewModel.UpdateRegistration.StateId = person.State.Id;
                    viewModel.UpdateRegistration.StateName = person.State.Name;
                    if (person.DateOfBirth != null)
                    {
                        viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
                    }

                    var applicantSponsorLogic = new ApplicantSponsorLogic();
                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == applicationForm.Person.Id)
                                                                .Select(ap => new {
                                                                    ap.Relationship_Id,
                                                                    ap.Sponsor_Name
                                                                })
                                                                .FirstOrDefault();
                    if (applicantSponsor != null)
                    {
                        viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
                        viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
                    }

                    var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
                    var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == applicationForm.Person.Id)
                                                                                    .Select(ap => new { ap.Previous_School_Name })
                                                                                    .FirstOrDefault();

                    if (applicantPreviousEducation != null)
                    {
                        viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
                    }

                    var admissionListLogic = new AdmissionListLogic();
                    var studentAdmission = admissionListLogic.GetModelsBy(ad => ad.APPLICATION_FORM.Person_Id == applicationForm.Person.Id).FirstOrDefault();

                    if (studentAdmission != null)
                    {
                        viewModel.UpdateRegistration.DepartmentId = studentAdmission.Deprtment.Id;
                        viewModel.UpdateRegistration.DepartmentName = studentAdmission.Deprtment.Name;
                        //viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
                        //viewModel.UpdateRegistration.LevelName = studentLevel.Level.Name;
                        viewModel.UpdateRegistration.SessionId = studentAdmission.Session.Id;
                        viewModel.UpdateRegistration.SessionName = studentAdmission.Session.Name;

                        var departmentHeadLogic = new DepartmentHeadLogic();
                        var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == studentAdmission.Deprtment.Id)
                                                                //.Select(d => new { Department_Head_Id = d.DepartmentHeadId })
                                                                .FirstOrDefault();

                        if (departmentHead != null)
                        {
                            viewModel.UpdateRegistration.HOD = departmentHead.DepartmentHeadId;
                            viewModel.UpdateRegistration.HODName = departmentHead.Person.FullName;
                        }
                    }
                }

                return viewModel;
            }
            catch (Exception ex) { throw ex; }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult UpdateStudentData(StudentCourseRegistrationViewModel viewModel)
        //{
        //    try
        //    {
        //        if (viewModel.MatricNumber != null)
        //        {
        //            var studentLogic = new StudentLogic();
        //            var student = studentLogic.GetModelsBy(st => st.Matric_Number == viewModel.MatricNumber).FirstOrDefault();

        //            if (student != null)
        //            {
        //                viewModel.UpdateRegistration = new UpdateRegistrationViewModel();
        //                var personLogic = new PersonLogic();
        //                var person = personLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();

        //                if (person != null)
        //                {
        //                    viewModel.UpdateRegistration.PersonId = person.Id;
        //                    viewModel.UpdateRegistration.FirstName = person.FirstName;
        //                    viewModel.UpdateRegistration.OtherName = person.OtherName;
        //                    viewModel.UpdateRegistration.LastName = person.LastName;
        //                    viewModel.UpdateRegistration.Sex = person.Sex.Id;
        //                    viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
        //                    viewModel.UpdateRegistration.EmailAddress = person.Email;
        //                    viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
        //                    viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
        //                    viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

        //                    viewModel.UpdateRegistration.StateId = person.State.Id;
        //                    if (person.DateOfBirth != null)
        //                    {
        //                        viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
        //                    }

        //                    var applicantSponsorLogic = new ApplicantSponsorLogic();
        //                    var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
        //                                                                .Select(ap => new {
        //                                                                    ap.Relationship_Id,
        //                                                                    ap.Sponsor_Name
        //                                                                })
        //                                                                .FirstOrDefault();
        //                    if (applicantSponsor != null)
        //                    {
        //                        viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
        //                        viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
        //                    }

        //                    var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
        //                    var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
        //                                                                                    .Select(ap => new { ap.Previous_School_Name })
        //                                                                                    .FirstOrDefault();

        //                    if (applicantPreviousEducation != null)
        //                    {
        //                        viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
        //                    }

        //                    var studentLevelLogic = new StudentLevelLogic();
        //                    var studentLevel = studentLevelLogic.GetModelsBy(st => st.Person_Id == student.Id).FirstOrDefault();

        //                    if (studentLevel != null)
        //                    {
        //                        viewModel.UpdateRegistration.DepartmentId = studentLevel.Department.Id;
        //                        viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
        //                        viewModel.UpdateRegistration.SessionId = studentLevel.Session.Id;

        //                        var departmentHeadLogic = new DepartmentHeadLogic();
        //                        var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == studentLevel.Department.Id)
        //                                                                .Select(d => new { Department_Head_Id = d.DepartmentHeadId })
        //                                                                .FirstOrDefault();

        //                        if (departmentHead != null)
        //                        {
        //                            viewModel.UpdateRegistration.HOD = departmentHead.Department_Head_Id;
        //                        }
        //                    }
        //                }
        //                ViewBag.Sexes = Utility.PopulateSexSelectListItem();
        //                ViewBag.States = Utility.PopulateStateSelectListItem();
        //                ViewBag.Departments = Utility.PopulateDepartmentSelectListItem();
        //                ViewBag.Levels = Utility.PopulateLevelSelectListItem();
        //                ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
        //                ViewBag.Relationships = Utility.PopulateRelationshipSelectListItem();
        //                ViewBag.Nationlities = Utility.PopulateNationalitySelectListItem();
        //                ViewBag.HOD = Utility.PopulateDepartmentHeadSelectListItem();

        //                //PRE-SELECT DEPARTMENT, THEN SELECT FILL-IN COURSES
        //            }
        //        }
        //        else
        //        {
        //            SetMessage("No student matches this Matric Number", Message.Category.Error);
        //        }

        //        return View(viewModel);
        //    }
        //    catch(Exception ex) { throw ex; }
        //}

        [HttpPost]
        //[AllowAnonymous]
        public ActionResult UpdateStudentDataForm(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.UpdateRegistration.HOD > 0)
                {
                    var departmentHeadMatchesDepartment = DepartmentMatchesDepartmentHead(viewModel.UpdateRegistration.HOD);
                    if (departmentHeadMatchesDepartment == null)
                    {
                        SetMessage("HOD Selection does not match Department Selection", Message.Category.Error);
                        return View(viewModel);
                    }
                }

                var personLogic = new PersonLogic();
                var person = personLogic.GetModelsBy(p => p.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.FirstName))
                {
                    person.FirstName = viewModel.UpdateRegistration.FirstName;
                }
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.LastName))
                {
                    person.LastName = viewModel.UpdateRegistration.LastName;
                }
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.OtherName))
                {
                    person.OtherName = viewModel.UpdateRegistration.OtherName;
                }
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.EmailAddress))
                {
                    person.Email = viewModel.UpdateRegistration.EmailAddress;
                }
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.PhoneNumber))
                {
                    person.MobilePhone = viewModel.UpdateRegistration.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(viewModel.UpdateRegistration.HomeAddress))
                {
                    person.ContactAddress = viewModel.UpdateRegistration.HomeAddress;
                }
                if (viewModel.UpdateRegistration.Sex > 0)
                {
                    person.Sex = new Sex() { Id = viewModel.UpdateRegistration.Sex };
                }
                if (viewModel.UpdateRegistration.Nationality > 0)
                {
                    person.Nationality = new Nationality() { Id = viewModel.UpdateRegistration.Nationality };
                }
                if (TempData["imageUrl"] != null)
                {
                    person.ImageFileUrl = (string)TempData["imageUrl"];
                    string junkFilePath, destinationFilePath;

                    SetPersonPassportDestination(person, out junkFilePath, out destinationFilePath);
                    SavePersonPassport(junkFilePath, destinationFilePath, person);
                }
                personLogic.Modify(person);

                var studentLevelLogic = new StudentLevelLogic();
                var studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.UpdateRegistration.PersonId).FirstOrDefault();
                if (studentLevel != null)
                {
                    viewModel = ModifyStudentLevel(viewModel, person, studentLevel);
                }
                else
                {
                    var admissionListLogic = new AdmissionListLogic();
                    var admissionList = admissionListLogic.GetModelsBy(ad => ad.APPLICATION_FORM.Person_Id == person.Id).FirstOrDefault();
                    if (admissionList != null)
                    {
                        //Call Applicant Modification Function here
                        viewModel = ModifyApplicantLevel(viewModel, person, admissionList);
                    }
                }

                var libraryRegistrationLogic = new LibraryRegistrationLogic();
                var libraryRegistration = libraryRegistrationLogic.GetModelsBy(lib => lib.Person_Id == person.Id).FirstOrDefault();
                if (libraryRegistration == null)
                {
                    var username = User.Identity.Name;
                    var userLogic = new UserLogic();
                    var user = userLogic.GetModelsBy(u => u.User_Name == username).FirstOrDefault();

                    if (user != null)
                    {
                        string parentName = !string.IsNullOrEmpty(viewModel.UpdateRegistration.ParentGuardian) ? viewModel.UpdateRegistration.ParentGuardian : null;
                        string schoolName = !string.IsNullOrEmpty(viewModel.UpdateRegistration.SchoolName) ? viewModel.UpdateRegistration.SchoolName : null;

                        var newLibraryRegistration = new LibraryRegistration();
                        newLibraryRegistration.Person = new Person() { Id = person.Id };
                        newLibraryRegistration.Guardian_Name = parentName;
                        newLibraryRegistration.User = new User() { Id = user.Id };
                        newLibraryRegistration.Previous_School_Name = schoolName;
                        newLibraryRegistration.Date = DateTime.Now;
                        if (viewModel.UpdateRegistration.SessionId > 0)
                        {
                            newLibraryRegistration.Session = new Session() { Id = viewModel.UpdateRegistration.SessionId };
                        }

                        libraryRegistrationLogic.Create(newLibraryRegistration);
                    }
                }

                SetMessage("Student updated successfully", Message.Category.Information);

                return RedirectToAction("UpdateStudentData");
            }
            catch (Exception ex) { throw ex; }
        }

        //[AllowAnonymous]
        public ActionResult ViewStudentsByDepartmentAndProgramme()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Departments = Utility.PopulateAllDepartmentSelectListItem();
                ViewBag.Programmes = Utility.PopulateProgrammeSelectListItem();
                ViewBag.Sessions = Utility.PopulateAllSessionSelectListItem();

                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        [HttpPost]
        //[AllowAnonymous]
        public ActionResult ViewStudentsByDepartmentAndProgramme(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                var libraryRegistrationLogic = new LibraryRegistrationLogic();
                var studentList = libraryRegistrationLogic.GetAll();
                var studentLevelLogic = new StudentLevelLogic();
                var admissionLogic = new AdmissionListLogic();

                for (int i = 0; i < studentList.Count(); i++)
                {
                    StudentDetailViewModel student = new StudentDetailViewModel();
                    var currentIteration = studentList[i];
                    var findStudent = studentLevelLogic.GetModelsBy(s => s.STUDENT.Person_Id == currentIteration.Person.Id && s.Department_Id == viewModel.DepartmentId && s.Programme_Id == viewModel.ProgrammeId).FirstOrDefault();
                    if (findStudent != null)
                    {
                        //Student
                        student.ProgrammeName = findStudent.Programme.Name;
                        student.DepartmentName = findStudent.Department.Name;
                        student.SessionName = findStudent.Session.Name;
                        student.MatricNumber = findStudent.Student.MatricNumber;
                        student.StudentName = findStudent.Student.FullName;
                        student.PersonId = findStudent.Student.Id;

                        viewModel.StudentDetails.Add(student);
                    }
                    else
                    {
                        //Applicant_List
                        var findApplicant = admissionLogic.GetModelsBy(ad => ad.APPLICATION_FORM.Person_Id == currentIteration.Person.Id && ad.Department_Id == viewModel.DepartmentId && ad.Programme_Id == viewModel.ProgrammeId).FirstOrDefault();
                        if (findApplicant != null)
                        {
                            student.ProgrammeName = findApplicant.Programme.Name;
                            student.DepartmentName = findApplicant.Deprtment.Name;
                            student.SessionName = findApplicant.Session.Name;
                            student.MatricNumber = findApplicant.Form.Number;
                            student.StudentName = findApplicant.Form.Person.FullName;
                            student.PersonId = findApplicant.Form.Person.Id;

                            viewModel.StudentDetails.Add(student);
                        }
                    }
                }

                ViewBag.Departments = Utility.PopulateAllDepartmentSelectListItem();
                ViewBag.Programmes = Utility.PopulateProgrammeSelectListItem();

                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        public ActionResult ViewStudentDetail(int Id)
        {
            try
            {
                if (Id < 0)
                {
                    return RedirectToAction("ViewStudentsByDepartmentAndProgramme");
                }
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                var studentLogic = new StudentLogic();
                var student = studentLogic.GetModelsBy(st => st.Person_Id == Id).FirstOrDefault();

                if (student != null)
                {
                    viewModel = PreviewStudentDetailsFromStudentTable(viewModel, student);
                }
                else
                {
                    var applicationFormLogic = new ApplicationFormLogic();
                    var applicationForm = applicationFormLogic.GetModelsBy(ap => ap.Person_Id == Id).FirstOrDefault();
                    if (applicationForm != null)
                    {
                        viewModel = PreviewStudentDetailsFromApplicantTable(viewModel, applicationForm);
                    }
                    else
                    {
                        viewModel.UpdateRegistration = null;
                        SetMessage("No record was found for this student", Message.Category.Error);
                    }
                }

                //ViewBag.Sexes = Utility.PopulateSexSelectListItem();
                //ViewBag.States = Utility.PopulateStateSelectListItem();
                //ViewBag.Departments = Utility.PopulateDepartmentSelectListItem();
                //ViewBag.Levels = Utility.PopulateLevelSelectListItem();
                //ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
                //ViewBag.Relationships = Utility.PopulateRelationshipSelectListItem();
                //ViewBag.Nationlities = Utility.PopulateNationalitySelectListItem();
                //ViewBag.HOD = Utility.PopulateDepartmentHeadSelectListItem();
                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        public ActionResult ViewMultipleStudentDetails(int departmentId, int programmeId)
        {
            try
            {
                if (departmentId < 0 || programmeId < 0)
                {
                    return RedirectToAction("ViewStudentsByDepartmentAndProgramme");
                }

                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                var studentLevelLogic = new StudentLevelLogic();
                var studentList = studentLevelLogic.GetModelsBy(st => st.Department_Id == departmentId && st.Programme_Id == programmeId).Select(st => st.Student).ToList();
                if (studentList?.Count() > 0)
                {
                    foreach (var item in studentList)
                    {
                        var returnedViewModel = PreviewStudentDetailsFromStudentTable(viewModel, item);

                        viewModel.UpdateRegistrationList.Add(returnedViewModel.UpdateRegistration);
                    }
                }
                else
                {
                    var admissionListLogic = new AdmissionListLogic();
                    var admissions = admissionListLogic.GetModelsBy(ad => ad.Department_Id == departmentId && ad.Programme_Id == programmeId).Select(ad => ad.Form).ToList();
                    if (admissions?.Count() > 0)
                    {
                        foreach (var item in admissions)
                        {
                            var returnedViewModel = PreviewStudentDetailsFromApplicantTable(viewModel, item);

                            viewModel.UpdateRegistrationList.Add(returnedViewModel.UpdateRegistration);
                        }
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex) { throw ex; }
        }

        //[HttpPost]
        ////[AllowAnonymous]
        //public ActionResult ViewStudentsByDepartmentAndProgramme(StudentCourseRegistrationViewModel viewModel)
        //{
        //    try
        //    {
        //        var studentLevelLogic = new StudentLevelLogic();
        //        //Remove Take Ten
        //        var studentList = studentLevelLogic.GetModelsBy(st => st.Programme_Id == viewModel.ProgrammeId && st.Department_Id == viewModel.DepartmentId).Take(10).ToList();
        //        for (int i = 0; i < studentList.Count(); i++)
        //        {
        //            StudentDetailViewModel student = new StudentDetailViewModel();
        //            var currentIteration = studentList[i];
        //            student.ProgrammeName = currentIteration.Programme.Name;
        //            student.SessionName = currentIteration.Session.Name;
        //            student.DepartmentName = currentIteration.Department.Name;
        //            student.MatricNumber = currentIteration.Student.MatricNumber;

        //            var studentLogic = new StudentLogic();
        //            var studentDetail = studentLogic.GetModelsBy(st => st.Matric_Number == currentIteration.Student.MatricNumber).FirstOrDefault();
        //            if (studentDetail != null)
        //            {
        //                student.StudentName = studentDetail.FullName;
        //                student.PersonId = studentDetail.Id;
        //            }

        //            viewModel.StudentDetails.Add(student);
        //        }

        //        ViewBag.Departments = Utility.PopulateAllDepartmentSelectListItem();
        //        ViewBag.Programmes = Utility.PopulateProgrammeSelectListItem();

        //        return View(viewModel);
        //    }
        //    catch(Exception ex) { throw ex; }
        //}

        //[AllowAnonymous]
        //public ActionResult ViewStudentDetail(int Id)
        //{
        //    try
        //    {
        //        if (Id < 0)
        //        {
        //            return RedirectToAction("ViewStudentsByDepartmentAndProgramme");
        //        }
        //        StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
        //        var studentLogic = new StudentLogic();
        //        var student = studentLogic.GetModelsBy(st => st.Person_Id == Id).FirstOrDefault();

        //        if (student != null)
        //        {  
        //            viewModel.UpdateRegistration = new UpdateRegistrationViewModel();
        //            viewModel.UpdateRegistration.MatricNumber = student.MatricNumber;
        //            var personLogic = new PersonLogic();
        //            var person = personLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();

        //            if (person != null)
        //            {
        //                viewModel.UpdateRegistration.PersonId = person.Id;
        //                viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;
        //                viewModel.UpdateRegistration.FirstName = person.FirstName;
        //                viewModel.UpdateRegistration.OtherName = person.OtherName;
        //                viewModel.UpdateRegistration.LastName = person.LastName;
        //                viewModel.UpdateRegistration.Sex = person.Sex.Id;
        //                viewModel.UpdateRegistration.SexName = person.Sex.Name;
        //                viewModel.UpdateRegistration.PhoneNumber = person.MobilePhone;
        //                viewModel.UpdateRegistration.EmailAddress = person.Email;
        //                viewModel.UpdateRegistration.Nationality = person.Nationality.Id;
        //                viewModel.UpdateRegistration.NationalityName = person.Nationality.Name;
        //                viewModel.UpdateRegistration.HomeAddress = person.ContactAddress;
        //                viewModel.UpdateRegistration.ImageFile = person.ImageFileUrl;

        //                viewModel.UpdateRegistration.StateId = person.State.Id;
        //                viewModel.UpdateRegistration.StateName = person.State.Name;
        //                if (person.DateOfBirth != null)
        //                {
        //                    viewModel.UpdateRegistration.DateOfBirth = (DateTime)person.DateOfBirth;
        //                }

        //                var applicantSponsorLogic = new ApplicantSponsorLogic();
        //                var applicantSponsor = applicantSponsorLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
        //                                                            .Select(ap => new {
        //                                                                ap.Relationship_Id,
        //                                                                ap.Sponsor_Name
        //                                                            })
        //                                                            .FirstOrDefault();
        //                if (applicantSponsor != null)
        //                {
        //                    viewModel.UpdateRegistration.RelationshipId = applicantSponsor.Relationship_Id;
        //                    viewModel.UpdateRegistration.ParentGuardian = applicantSponsor?.Sponsor_Name;
        //                }

        //                var applicantPreviousEducationLogic = new ApplicantPreviousEducationLogic();
        //                var applicantPreviousEducation = applicantPreviousEducationLogic.GetModelsBy(ap => ap.Person_Id == student.Id)
        //                                                                                .Select(ap => new { ap.Previous_School_Name })
        //                                                                                .FirstOrDefault();

        //                if (applicantPreviousEducation != null)
        //                {
        //                    viewModel.UpdateRegistration.SchoolName = applicantPreviousEducation.Previous_School_Name;
        //                }

        //                var studentLevelLogic = new StudentLevelLogic();
        //                var studentLevel = studentLevelLogic.GetModelsBy(st => st.Person_Id == student.Id).FirstOrDefault();

        //                if (studentLevel != null)
        //                {
        //                    viewModel.UpdateRegistration.DepartmentId = studentLevel.Department.Id;
        //                    viewModel.UpdateRegistration.DepartmentName = studentLevel.Department.Name;
        //                    viewModel.UpdateRegistration.LevelId = studentLevel.Level.Id;
        //                    viewModel.UpdateRegistration.LevelName = studentLevel.Level.Name;
        //                    viewModel.UpdateRegistration.SessionId = studentLevel.Session.Id;
        //                    viewModel.UpdateRegistration.SessionName = studentLevel.Session.Name;

        //                    var departmentHeadLogic = new DepartmentHeadLogic();
        //                    var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Id == studentLevel.Department.Id)
        //                                                            //.Select(d => new { Department_Head_Id = d.DepartmentHeadId })
        //                                                            .FirstOrDefault();

        //                    if (departmentHead != null)
        //                    {
        //                        viewModel.UpdateRegistration.HOD = departmentHead.DepartmentHeadId;
        //                        viewModel.UpdateRegistration.HODName = departmentHead.Person.FullName;
        //                    }
        //                }
        //            }

        //            ViewBag.Sexes = Utility.PopulateSexSelectListItem();
        //            ViewBag.States = Utility.PopulateStateSelectListItem();
        //            ViewBag.Departments = Utility.PopulateDepartmentSelectListItem();
        //            ViewBag.Levels = Utility.PopulateLevelSelectListItem();
        //            ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
        //            ViewBag.Relationships = Utility.PopulateRelationshipSelectListItem();
        //            ViewBag.Nationlities = Utility.PopulateNationalitySelectListItem();
        //            ViewBag.HOD = Utility.PopulateDepartmentHeadSelectListItem();
        //        }
        //        return View(viewModel);
        //    }
        //    catch(Exception ex) { throw ex; }
        //}

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["MyFile"];

            bool isUploaded = false;
            string personId = form["UpdateRegistration.PersonId"].ToString();
            string passportUrl = form["UpdateRegistration.ImageFile"].ToString();
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded = isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded = isUploaded, message = message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }

        public Department DepartmentMatchesDepartmentHead(int departmentHeadId)
        {
            try
            {
                var departmentHeadLogic = new DepartmentHeadLogic();
                var departmentHead = departmentHeadLogic.GetModelsBy(d => d.Department_Head_Id == departmentHeadId).FirstOrDefault();
                return departmentHead.Department;
            }
            catch (Exception ex) { throw ex; }
        }

        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 50 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension + "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") + " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        /*TODO: You must process this exception.*/
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SavePersonPassport(string junkFilePath, string pathForSaving, Person person)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(pathForSaving);
                string mainFileName = person.Id.ToString() + "__";

                DeleteFileIfExist(folderPath, mainFileName);

                System.IO.File.Move(junkFilePath, pathForSaving);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPersonPassportDestination(Person person, out string junkFilePath, out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                string passportUrl = person.ImageFileUrl;
                junkFilePath = Server.MapPath(TILDA + person.ImageFileUrl);
                destinationFilePath = junkFilePath.Replace("Junk", "Student");
                person.ImageFileUrl = passportUrl.Replace("Junk", "Student");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}