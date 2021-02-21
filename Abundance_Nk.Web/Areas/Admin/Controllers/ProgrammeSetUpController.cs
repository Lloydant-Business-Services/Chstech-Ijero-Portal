using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class ProgrammeSetUpController : BaseController
    {
        ProgrammeSetUpViewModel viewModel = new ProgrammeSetUpViewModel();

        public bool IsError { get; set; }

        public ActionResult Setup()
        {
            try
            {

                ViewBag.DepartmentId = viewModel.DepartmentSelectListItem;
                //ViewBag.DepartmentOptionId = viewModel.DepartmentOpionSelectListItem;
                ViewBag.levelId = viewModel.levelSelectListItem;
                ViewBag.SessionId = viewModel.SessionSelectList;
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.FacultyId = viewModel.FacultySelectListItems;
                ViewBag.SemesterId = viewModel.SemesterSelectListItems;

            }
            catch (Exception)
            {

                SetMessage("Error Occured!", Message.Category.Error);

            }
            return View(viewModel);
        }

        public JsonResult PopulateSetupTables(int setUpTableType)
        {
            ProgrammeSetUpViewModel result = new ProgrammeSetUpViewModel();

            List<ProgrammeSetUpViewModel> setupModel = new List<ProgrammeSetUpViewModel>();
            FacultyLogic facultyLogic = new FacultyLogic();

            try
            {

                switch (setUpTableType)
                {
                    case 1:
                        ProgrammeLogic programmeLogic = new ProgrammeLogic();
                        result.Programmes = programmeLogic.GetAll();
                        for (int i = 0; i < result.Programmes.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {

                                IsError = false,
                                Id = result.Programmes[i].Id.ToString(),
                                ProgrammeName = result.Programmes[i].Name,
                                ProgrammeShortName = result.Programmes[i].ShortName,
                                ProgrammeActivated = Convert.ToString(result.Programmes[i].Activated),
                                ProgrammeDescription = result.Programmes[i].Description,
                            });
                        }

                        break;
                    case 2:
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        List<DEPARTMENT> department = departmentLogic.GetEntitiesBy();

                        for (int i = 0; i < department.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = department[i].Department_Id.ToString(),
                                DepartmentName = department[i].Department_Name,
                                DepartmentCode = department[i].Department_Code,
                                FacultyName = department[i].FACULTY.Faculty_Name,
                                DepartmentActivated = Convert.ToString(department[i].Active)

                            });

                        }

                        break;
                    case 3:
                        LevelLogic levelLogic = new LevelLogic();
                        result.Levels = levelLogic.GetAll();
                        for (int i = 0; i < result.Levels.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = result.Levels[i].Id.ToString(),
                                LevelName = result.Levels[i].Name,
                                LevelDescription = result.Levels[i].Description,

                            });

                        }

                        break;

                    case 4:
                        SessionLogic sessionLogic = new SessionLogic();

                        result.Sessions = sessionLogic.GetAll();
                        for (int i = 0; i < result.Sessions.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = result.Sessions[i].Id.ToString(),
                                SessionName = result.Sessions[i].Name,
                                SessionActivated = Convert.ToString(result.Sessions[i].Activated)
                            });
                        }

                        break;

                    case 5:

                        result.Faculties = facultyLogic.GetAll();
                        for (int i = 0; i < result.Faculties.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = result.Faculties[i].Id.ToString(),
                                FacultyName = result.Faculties[i].Name,
                                FacultyDescription = result.Faculties[i].Description

                            });
                        }

                        break;

                    case 6:
                        DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                        result.DepartmentOptions = departmentOptionLogic.GetAll();
                        for (int i = 0; i < result.DepartmentOptions.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = result.DepartmentOptions[i].Id.ToString(),
                                DepartmentOptionName = result.DepartmentOptions[i].Name,
                                DepartmentName = result.DepartmentOptions[i].Department.Name,
                                DepartmentOptionActivated = Convert.ToString(result.DepartmentOptions[i].Activated)
                            });
                        }
                        break;
                    case 7:
                        ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                        List<PROGRAMME_DEPARTMENT> programmeDepartments = programmeDepartmentLogic.GetEntitiesBy();
                        for (int i = 0; i < programmeDepartments.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = programmeDepartments[i].Programme_Department_Id.ToString(),
                                ProgrammeName = programmeDepartments[i].PROGRAMME.Programme_Name,
                                DepartmentName = programmeDepartments[i].DEPARTMENT.Department_Name,

                            });
                        }

                        break;
                    case 8:
                        SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                        List<SESSION_SEMESTER> sessionSemesters = sessionSemesterLogic.GetEntitiesBy();
                        for (int i = 0; i < sessionSemesters.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = sessionSemesters[i].Session_Semester_Id.ToString(),
                                SessionName = sessionSemesters[i].SESSION.Session_Name,
                                SesmesterName = sessionSemesters[i].SEMESTER.Semester_Name,
                                SequenceNumber = sessionSemesters[i].Sequence_Number.ToString(),
                                RegistrationEnded = Convert.ToString(sessionSemesters[i].Registration_Ended)
                            });
                        }

                        break;
                    case 9:
                        RoleLogic roleLogic = new RoleLogic();
                        List<ROLE> role = roleLogic.GetEntitiesBy();
                        for (int i = 0; i < role.Count; i++)
                        {
                            setupModel.Add(new ProgrammeSetUpViewModel()
                            {
                                IsError = false,
                                Id = role[i].Role_Id.ToString(),
                                RoleName = role[i].Role_Name,
                                RoleDescription = role[i].Role_Description

                            });
                        }

                        break;
                }


                switch (setUpTableType)
                {
                    case 1:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 2:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 3:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 4:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 5:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 6:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 7:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 8:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);
                    case 9:
                        return Json(setupModel, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateRecord(int setUpTableType, string myRecordArray)
        {
            ProgrammeSetUpViewModel result = new ProgrammeSetUpViewModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ProgrammeSetUpViewModel arrayJsonView = serializer.Deserialize<ProgrammeSetUpViewModel>(myRecordArray);




                int programmeId = 0;
                string programmeName = null;
                string programmeDescription = null;
                string programmeShortName = null;
                bool programmeActivated = false;
                int departmentId = 0;
                string departmentName = null;
                string departmentCode = null;
                string departmentOptionName = null;
                bool departmentOptionActivated = false;
                bool departmentActivated = false;
                bool registrationEnded = false;
                string levelName = null;
                string levelDescription = null;
                int facultyId = 0;
                int sessionId = 0;
                string sessionName = null;
                string facultyName = null;
                string facultyDescription = null;
                bool sessionActivated = false;
                bool programmedepartmentActivated = false;
                int sequenceNumber = 0;
                int semesterId = 0;
                string roleName = null;
                string roleDescription = null;

                AssignJsonArrayValues(arrayJsonView, ref programmeId, ref programmeName, ref departmentId, ref programmeDescription, ref programmeShortName, ref programmeActivated, ref departmentName, ref departmentCode,
                    ref departmentOptionName, ref departmentOptionActivated, ref levelName, ref levelDescription, ref facultyId, ref sessionName, ref facultyName, ref facultyDescription, 
                    ref sessionActivated, ref sequenceNumber, ref semesterId, ref sessionId, ref roleName, ref roleDescription, ref departmentActivated, ref registrationEnded);



                switch (setUpTableType)
                {
                    case 1:
                        result.Message = CreateProgramme(programmeName, programmeDescription, programmeShortName, programmeActivated);
                        result.IsError = IsError;
                        break;
                    case 2:
                        result.Message = CreateDepartment(departmentName, departmentCode, facultyId, departmentActivated);
                        result.IsError = IsError;
                        break;
                    case 3:
                        result.Message = CreateLevel(levelName, levelDescription);
                        result.IsError = IsError;
                        break;
                    case 4:
                        result.Message = CreateSession(sessionName, sessionActivated);
                        result.IsError = IsError;
                        break;
                    case 5:
                        result.Message = CreateFaculty(facultyName, facultyDescription);
                        result.IsError = IsError;
                        break;
                    case 6:
                        result.Message = CreateDepartmentOption(departmentOptionName, departmentId, departmentOptionActivated);
                        result.IsError = IsError;
                        break;
                    case 7:
                        result.Message = CreateProgrammeDepartment(programmeId, departmentId, programmedepartmentActivated);
                        result.IsError = IsError;
                        break;
                    case 8:
                        result.Message = CreateSessionSemester(sessionId, semesterId, sequenceNumber, registrationEnded);
                        result.IsError = IsError;
                        break;
                    case 9:
                        result.Message = CreateRole(roleName, roleDescription);
                        result.IsError = IsError;
                        break;

                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditRecord(int setUpTableType, string recordId, string myRecordArray, string action)
        {
            ProgrammeSetUpViewModel result = new ProgrammeSetUpViewModel();
            try
            {
                if (action == "edit" && !string.IsNullOrEmpty(recordId))
                {
                    switch (setUpTableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel resultModel = new ProgrammeSetUpViewModel();
                            ProgrammeLogic programmeLogic = new ProgrammeLogic();
                            Programme programme = new Programme();
                            if (id > 0)
                            {
                                programme = programmeLogic.GetModelsBy(f => f.Programme_Id == id).LastOrDefault();
                                if (programme != null)
                                {
                                    resultModel.IsError = false;
                                    resultModel.Id = programme.Id.ToString();
                                    resultModel.ProgrammeName = programme.Name;
                                    resultModel.ProgrammeShortName = programme.ShortName;
                                    resultModel.ProgrammeDescription = programme.Description;
                                    resultModel.ProgrammeActivated = Convert.ToString(programme.Activated);
                                    resultModel.Activated = (bool)programme.Activated;

                                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    resultModel.IsError = true;
                                    resultModel.Message = "Record does not exist in the database.";
                                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                resultModel.IsError = true;
                                resultModel.Message = "Edit parameter was not set.";
                                return Json(resultModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 2:
                            int departmentId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel departmentModel = new ProgrammeSetUpViewModel();
                            DepartmentLogic departmentLogic = new DepartmentLogic();
                            Department department = new Department();
                            if (departmentId > 0)
                            {
                                department = departmentLogic.GetModelsBy(f => f.Department_Id == departmentId).LastOrDefault();
                                if (department != null)
                                {
                                    departmentModel.IsError = false;
                                    departmentModel.Id = department.Id.ToString();
                                    departmentModel.DepartmentName = department.Name;
                                    departmentModel.DepartmentCode = department.Code;
                                    departmentModel.DepartmentActivated = Convert.ToString(department.Active);
                                    departmentModel.FacultyId = Convert.ToString(department.Faculty.Id);

                                    return Json(departmentModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    departmentModel.IsError = true;
                                    departmentModel.Message = "Record does not exist in the database.";
                                    return Json(departmentModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                departmentModel.IsError = true;
                                departmentModel.Message = "Edit parameter was not set.";
                                return Json(departmentModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 3:
                            int levelId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel levelModel = new ProgrammeSetUpViewModel();
                            LevelLogic levelLogic = new LevelLogic();
                            Level level;
                            if (levelId > 0)
                            {
                                level = levelLogic.GetModelsBy(f => f.Level_Id == levelId).LastOrDefault();
                                if (level != null)
                                {
                                    levelModel.IsError = false;
                                    levelModel.Id = level.Id.ToString();
                                    levelModel.LevelName = level.Name;
                                    levelModel.LevelDescription = level.Description;


                                    return Json(levelModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    levelModel.IsError = true;
                                    levelModel.Message = "Record does not exist in the database.";
                                    return Json(levelModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                levelModel.IsError = true;
                                levelModel.Message = "Edit parameter was not set.";
                                return Json(levelModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 4:
                            int sessionId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel sessionModel = new ProgrammeSetUpViewModel();
                            SessionLogic sessionLogic = new SessionLogic();
                            Session session;
                            if (sessionId > 0)
                            {
                                session = sessionLogic.GetModelsBy(f => f.Session_Id == sessionId).LastOrDefault();
                                if (session != null)
                                {
                                    sessionModel.IsError = false;
                                    sessionModel.Id = session.Id.ToString();

                                    sessionModel.SessionName = session.Name;
                                    sessionModel.SessionActivated = Convert.ToString(session.Activated);
                                    sessionModel.Activated = Convert.ToBoolean(session.Activated);


                                    return Json(sessionModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    sessionModel.IsError = true;
                                    sessionModel.Message = "Record does not exist in the database.";
                                    return Json(sessionModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                sessionModel.IsError = true;
                                sessionModel.Message = "Edit parameter was not set.";
                                return Json(sessionModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 5:
                            int facultyId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel facultyModel = new ProgrammeSetUpViewModel();
                            FacultyLogic facultyLogic = new FacultyLogic();
                            Faculty faculty;
                            if (facultyId > 0)
                            {
                                faculty = facultyLogic.GetModelsBy(f => f.Faculty_Id == facultyId).LastOrDefault();
                                if (faculty != null)
                                {
                                    facultyModel.IsError = false;
                                    facultyModel.Id = faculty.Id.ToString();
                                    facultyModel.FacultyName = faculty.Name;
                                    facultyModel.FacultyDescription = faculty.Description;


                                    return Json(facultyModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    facultyModel.IsError = true;
                                    facultyModel.Message = "Record does not exist in the database.";
                                    return Json(facultyModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                facultyModel.IsError = true;
                                facultyModel.Message = "Edit parameter was not set.";
                                return Json(facultyModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 6:
                            int departmentOptionId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel departmentOptionModel = new ProgrammeSetUpViewModel();
                            DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                            DepartmentOption departmentOption = new DepartmentOption();
                            if (departmentOptionId > 0)
                            {
                                departmentOption = departmentOptionLogic.GetModelsBy(f => f.Department_Option_Id == departmentOptionId).LastOrDefault();
                                if (departmentOption != null)
                                {
                                    departmentOptionModel.IsError = false;
                                    departmentOptionModel.Id = departmentOption.Id.ToString();
                                    departmentOptionModel.DepartmentOptionName = departmentOption.Name;
                                    departmentOptionModel.DepartmentName = departmentOption.Department.Name;
                                    departmentOptionModel.DepartmentOptionActivated = Convert.ToString(departmentOption.Activated);
                                    departmentOptionModel.Activated = departmentOption.Activated;
                                    departmentOptionModel.DepartmentId = departmentOption.Department.Id.ToString();

                                    return Json(departmentOptionModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    departmentOptionModel.IsError = true;
                                    departmentOptionModel.Message = "Record does not exist in the database.";
                                    return Json(departmentOptionModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                departmentOptionModel.IsError = true;
                                departmentOptionModel.Message = "Edit parameter was not set.";
                                return Json(departmentOptionModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 7:
                            int programmeDepartmentId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel programmeDepartmentModel = new ProgrammeSetUpViewModel();
                            ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                            ProgrammeDepartment programmeDepartment = new ProgrammeDepartment();
                            if (programmeDepartmentId > 0)
                            {
                                programmeDepartment = programmeDepartmentLogic.GetModelsBy(f => f.Programme_Department_Id == programmeDepartmentId).LastOrDefault();
                                if (programmeDepartment != null)
                                {
                                    programmeDepartmentModel.IsError = false;
                                    programmeDepartmentModel.Id = programmeDepartment.Id.ToString();
                                    programmeDepartmentModel.ProgrammeName = programmeDepartment.Programme.Name;
                                    programmeDepartmentModel.ProId = programmeDepartment.Programme.Id.ToString();
                                    programmeDepartmentModel.DepartmentName = programmeDepartment.Department.Name;
                                    programmeDepartmentModel.DepartmentId = programmeDepartment.Department.Id.ToString();

                                    return Json(programmeDepartmentModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    programmeDepartmentModel.IsError = true;
                                    programmeDepartmentModel.Message = "Record does not exist in the database.";
                                    return Json(programmeDepartmentModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                programmeDepartmentModel.IsError = true;
                                programmeDepartmentModel.Message = "Edit parameter was not set.";
                                return Json(programmeDepartmentModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 8:
                            int sessionSemesterId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel sessionSemesterModel = new ProgrammeSetUpViewModel();
                            SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                            SessionSemester sessionSemester = new SessionSemester();
                            if (sessionSemesterId > 0)
                            {
                                sessionSemester = sessionSemesterLogic.GetModelsBy(f => f.Session_Semester_Id == sessionSemesterId).LastOrDefault();
                                if (sessionSemester != null)
                                {
                                    sessionSemesterModel.IsError = false;
                                    sessionSemesterModel.Id = sessionSemester.Id.ToString();
                                    sessionSemesterModel.SessionName = sessionSemester.Session.Name;
                                    sessionSemesterModel.SesmesterName = sessionSemester.Semester.Name;
                                    sessionSemesterModel.SequenceNumber = sessionSemester.SequenceNumber.ToString();
                                    sessionSemesterModel.SessionId = sessionSemester.Session.Id.ToString();
                                    sessionSemesterModel.SemesterId = sessionSemester.Semester.Id.ToString();
                                    sessionSemesterModel.RegistrationEnded = Convert.ToString(sessionSemester.RegistrationEnded);


                                    return Json(sessionSemesterModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    sessionSemesterModel.IsError = true;
                                    sessionSemesterModel.Message = "Record does not exist in the database.";
                                    return Json(sessionSemesterModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                sessionSemesterModel.IsError = true;
                                sessionSemesterModel.Message = "Edit parameter was not set.";
                                return Json(sessionSemesterModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 9:
                            int roleId = Convert.ToInt32(recordId);
                            ProgrammeSetUpViewModel roleModel = new ProgrammeSetUpViewModel();
                            RoleLogic roleLogic = new RoleLogic();
                            Role role = new Role();
                            if (roleId > 0)
                            {
                                role = roleLogic.GetModelsBy(f => f.Role_Id == roleId).LastOrDefault();
                                if (role != null)
                                {
                                    roleModel.IsError = false;
                                    roleModel.Id = role.Id.ToString();
                                    roleModel.RoleName = role.Name;
                                    roleModel.RoleDescription = role.Description;

                                    return Json(roleModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    roleModel.IsError = true;
                                    roleModel.Message = "Record does not exist in the database.";
                                    return Json(roleModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                roleModel.IsError = true;
                                roleModel.Message = "Edit parameter was not set.";
                                return Json(roleModel, JsonRequestBehavior.AllowGet);
                            }
                            break;

                    }
                }
                else if (action == "save")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ProgrammeSetUpViewModel arrayJsonView = serializer.Deserialize<ProgrammeSetUpViewModel>(myRecordArray);

                    int programmeId = 0;
                    string programmeName = null;
                    string programmeDescription = null;
                    string programmeShortName = null;
                    bool programmeActivated = false;
                    int departmentId = 0;
                    string departmentName = null;
                    string departmentCode = null;
                    string departmentOptionName = null;
                    bool departmentOptionActivated = false;
                    bool departmentActivated = false;
                    bool registrationEnded = false;
                    string levelName = null;
                    string levelDescription = null;
                    int facultyId = 0;
                    string sessionName = null;
                    string facultyName = null;
                    string facultyDescription = null;
                    bool sessionActivated = false;
                    bool programmedepartmentActivated = false;
                    int sequenceNumber = 0;
                    int semesterId = 0;
                    int sessionId = 0;
                    string roleName = null;
                    string roleDescription = null;

                    AssignJsonArrayValues(arrayJsonView, ref programmeId, ref programmeName, ref departmentId, ref programmeDescription, ref programmeShortName, ref programmeActivated, ref departmentName, ref departmentCode,
                    ref departmentOptionName, ref departmentOptionActivated, ref levelName, ref levelDescription, ref facultyId, ref sessionName, ref facultyName, ref facultyDescription,
                    ref sessionActivated, ref sequenceNumber, ref semesterId, ref sessionId, ref roleName, ref roleDescription, ref departmentActivated, ref registrationEnded);

                    switch (setUpTableType)
                    {
                        case 1:
                            int saveProgrammeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveProgramme(saveProgrammeId, programmeName, programmeDescription, programmeShortName, programmeActivated);
                            result.IsError = IsError;
                            break;
                        case 2:
                            int saveDepartmentId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveDepartment(saveDepartmentId, departmentName, departmentCode, facultyId, departmentActivated);
                            result.IsError = IsError;
                            break;
                        case 3:
                            int saveLevelId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveLevel(saveLevelId, levelName, levelDescription);
                            result.IsError = IsError;
                            break;
                        case 4:
                            int saveSessionId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveSession(saveSessionId, sessionName, sessionActivated);
                            result.IsError = IsError;
                            break;
                        case 5:
                            int saveFacultyId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveFaculty(saveFacultyId, facultyName, facultyDescription);
                            result.IsError = IsError;
                            break;
                        case 6:
                            int saveDepartmentOptionId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveDepartmentOption(saveDepartmentOptionId, departmentOptionName, departmentOptionActivated, departmentId);
                            result.IsError = IsError;
                            break;
                        case 7:
                            int saveProgrammeDepartmentId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveProgrammeDepartment(saveProgrammeDepartmentId, programmeId, departmentId, programmedepartmentActivated);
                            result.IsError = IsError;
                            break;
                        case 8:
                            int saveSessionSemesterId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveSessionSemester(saveSessionSemesterId, semesterId, sessionId, sequenceNumber, registrationEnded);
                            result.IsError = IsError;
                            break;
                        case 9:
                            int saveRoleId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveRole(saveRoleId, roleName, roleDescription);
                            result.IsError = IsError;
                            break;

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
        public JsonResult DeleteRecord(int setUpTableType, string recordId)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            ProgrammeSetUpViewModel result = new ProgrammeSetUpViewModel();
            try
            {
                if (setUpTableType > 0 && !string.IsNullOrEmpty(recordId))
                {
                    switch (setUpTableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(recordId);
                            ProgrammeLogic programmeLogic = new ProgrammeLogic();
                            var deletedProgrammeAudit = programmeLogic.GetModelBy(x => x.Programme_Id == id);
                            bool deleted = programmeLogic.Delete(f => f.Programme_Id == id);
                            
                            if (deleted)
                            {
                                result.IsError = false;
                                
                                if (deletedProgrammeAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "ProgrammeId"+ deletedProgrammeAudit.Id+"ProgrammeName=" + deletedProgrammeAudit.Name + "ProgrammeShortName=" + deletedProgrammeAudit.ShortName + "ProgrammeActivated" + deletedProgrammeAudit.Activated;
                                    generalAudit.Operation = "Deleted  Programme";
                                    generalAudit.TableNames = "Programme Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }

                            break;
                        case 2:
                            int deleteDepartmentId = Convert.ToInt32(recordId);
                            DepartmentLogic departmentLogic = new DepartmentLogic();
                           var deletedDepartmentAudit = departmentLogic.GetModelBy(x => x.Department_Id == deleteDepartmentId);
                            bool deletedDdepartment = departmentLogic.Delete(f => f.Department_Id == deleteDepartmentId);
                            if (deletedDdepartment)
                            {
                                result.IsError = false;
                                if (deletedDepartmentAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.CurrentValues = "DepartmentName=" + deletedDepartmentAudit.Name + "DepartmentCode=" + deletedDepartmentAudit.Code + "Faculty=" + deletedDepartmentAudit.Faculty.Name + "DepartmentActivated=" + deletedDepartmentAudit.Active;
                                    generalAudit.Operation = "Delete Department";
                                    generalAudit.TableNames = "Deparment Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 3:
                            int deleteLevelId = Convert.ToInt32(recordId);
                            LevelLogic levelLogic = new LevelLogic();
                            var deletedLevelAudit = levelLogic.GetModelBy(x => x.Level_Id == deleteLevelId);
                            bool deletedLevel = levelLogic.Delete(f => f.Level_Id == deleteLevelId);
                            if (deletedLevel)
                            {
                                result.IsError = false;
                                if (deletedLevelAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues ="LeveId"+ deletedLevelAudit .Id+ "LevelName=" + deletedLevelAudit.Name + "LevelDescription=" + deletedLevelAudit.Description;
                                    generalAudit.Operation = "Delete Level";
                                    generalAudit.TableNames = "Level Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 4:
                            int deleteSessionId = Convert.ToInt32(recordId);
                            SessionLogic sessionLogic = new SessionLogic();
                            var deletedSessionAudit = sessionLogic.GetModelBy(x => x.Session_Id == deleteSessionId);
                            bool deletedSession = sessionLogic.Delete(f => f.Session_Id == deleteSessionId);
                            if (deletedSession)
                            {
                                result.IsError = false;
                                if (deletedSessionAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "SessionId"+ deletedSessionAudit.Id+ "SessionName=" + deletedSessionAudit.Name + "SessionActivated=" + deletedSessionAudit.Activated;
                                    generalAudit.Operation = "Deleted Session";
                                    generalAudit.TableNames = "Session Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 5:
                            int deletedFacultyId = Convert.ToInt32(recordId);
                            FacultyLogic facultyLogic = new FacultyLogic();
                            var deletedFacultyAudit = facultyLogic.GetModelBy(x => x.Faculty_Id == deletedFacultyId);
                            bool deletedfaculty = facultyLogic.Delete(f => f.Faculty_Id == deletedFacultyId);
                            if (deletedfaculty)
                            {
                                if (deletedFacultyAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "FacultyId"+ deletedFacultyAudit.Id+ "FacultyName=" + deletedFacultyAudit.Name + "FacultyDescription=" + deletedFacultyAudit.Description;
                                    generalAudit.Operation = "Deleted Faculty";
                                    generalAudit.TableNames = "Faculty Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                                result.IsError = false;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 6:
                            int deletedDepartmentOptionId = Convert.ToInt32(recordId);
                            DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                            var deletedDeptOptionAudit = departmentOptionLogic.GetModelBy(x => x.Department_Option_Id == deletedDepartmentOptionId);
                            bool deletedDeparmentOption = departmentOptionLogic.Delete(f => f.Department_Option_Id == deletedDepartmentOptionId);
                            if (deletedDeparmentOption)
                            {
                                result.IsError = false;
                                if (deletedDeptOptionAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "DepartmentOptionId="+ deletedDeptOptionAudit.Id + "DeptOptionName=" + deletedDeptOptionAudit.Name + "Department=" + deletedDeptOptionAudit.Department.Name;
                                    generalAudit.Operation = "Delete DepartmentOption";
                                    generalAudit.TableNames = "DepartmentOption Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 7:
                            int deletedProgrammeDepartmentId = Convert.ToInt32(recordId);
                            ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                            var deletedProgrammeDeptAudit = programmeDepartmentLogic.GetModelBy(x => x.Programme_Department_Id == deletedProgrammeDepartmentId);
                            bool deletedProgrammeDepartment = programmeDepartmentLogic.Delete(f => f.Programme_Department_Id == deletedProgrammeDepartmentId);
                            if (deletedProgrammeDepartment)
                            {
                                result.IsError = false;
                                if (deletedProgrammeDeptAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "ProgrammeDepartmentId="+ deletedProgrammeDeptAudit.Id+ "ProgrammeId=" + deletedProgrammeDeptAudit.Programme.Id + "DepartmentId=" + deletedProgrammeDeptAudit.Department.Id;
                                    generalAudit.Operation = "Delete ProgrammeDepartment";
                                    generalAudit.TableNames = "ProgrammeDepartment Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 8:
                            int deletedSessionSemesterId = Convert.ToInt32(recordId);
                            SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                            var deletedSessionSemesterAudit = sessionSemesterLogic.GetModelBy(x => x.Session_Semester_Id == deletedSessionSemesterId);
                            bool deletedSessionSemester = sessionSemesterLogic.Delete(f => f.Session_Semester_Id == deletedSessionSemesterId);
                            if (deletedSessionSemester)
                            {
                                result.IsError = false;
                                if (deletedSessionSemesterAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "SessionSemesterId="+ deletedSessionSemesterAudit .Id+ "SessionSemesterName="+ deletedSessionSemesterAudit.Name+ "SequenceNo.=" + deletedSessionSemesterAudit.SequenceNumber + "RegistrationEnded=" + deletedSessionSemesterAudit.RegistrationEnded;
                                    generalAudit.Operation = "Delete SessionSemester";
                                    generalAudit.TableNames = "SessionSemester Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 9:
                            int deletedRoleId = Convert.ToInt32(recordId);
                            RoleLogic roleLogic = new RoleLogic();
                            var deletedRoleAudit = roleLogic.GetModelBy(x => x.Role_Id == deletedRoleId);
                            bool deletedRole = roleLogic.Delete(f => f.Role_Id == deletedRoleId);
                            if (deletedRole)
                            {
                                result.IsError = false;
                                if (deletedRoleAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.CurrentValues = "RoleId="+ deletedRoleAudit.Id+ "RoleName.=" + deletedRoleAudit.Name + "Description=" + deletedRoleAudit.Description;
                                    generalAudit.Operation = "Delete Role";
                                    generalAudit.TableNames = "Role Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Parameter was not set.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void AssignJsonArrayValues(ProgrammeSetUpViewModel arrayJsonView, ref int programmeId, ref string programmeName, ref int departmentId, ref string programmeDescription, ref string programmeShortName, ref bool programmeActivated,
             ref string departmentName, ref string departmentCode, ref string departmentOptionName, ref bool departmentOptionActivated, ref string levelName, ref string levelDescription, 
                ref int facultyId, ref string sessionName, ref string facultyName, ref string facultyDescription, ref bool sessionActivated, ref int sequenceNumber, ref int semesterId,
            ref int sessionId, ref string roleName, ref string roleDescription, ref bool departmentActivated, ref bool registrationEnded)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrayJsonView.ProId))
                {
                    programmeId = Convert.ToInt32(arrayJsonView.ProId);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.ProgrammeName))
                {
                    programmeName = arrayJsonView.ProgrammeName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentId))
                {
                    departmentId = Convert.ToInt32(arrayJsonView.DepartmentId);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.ProgrammeDescription))
                {
                    programmeDescription = arrayJsonView.ProgrammeDescription;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.ProgrammeShortName))
                {
                    programmeShortName = arrayJsonView.ProgrammeShortName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.ProgrammeActivated))
                {
                    programmeActivated = Convert.ToBoolean(arrayJsonView.ProgrammeActivated);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentName))
                {
                    departmentName = arrayJsonView.DepartmentName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentCode))
                {
                    departmentCode = arrayJsonView.DepartmentCode;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentOptionName))
                {
                    departmentOptionName = arrayJsonView.DepartmentOptionName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentOptionActivated))
                {
                    departmentOptionActivated = Convert.ToBoolean(arrayJsonView.DepartmentOptionActivated);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.DepartmentActivated))
                {
                    departmentActivated = Convert.ToBoolean(arrayJsonView.DepartmentActivated);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.RegistrationEnded))
                {
                    registrationEnded = Convert.ToBoolean(arrayJsonView.RegistrationEnded);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.LevelName))
                {
                    levelName = arrayJsonView.LevelName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.LevelDescription))
                {
                    levelDescription = arrayJsonView.LevelDescription;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FacultyId))
                {
                    facultyId = Convert.ToInt32(arrayJsonView.FacultyId);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.SessionName))
                {
                    sessionName = arrayJsonView.SessionName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FacultyName))
                {
                    facultyName = arrayJsonView.FacultyName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FacultyDescription))
                {
                    facultyDescription = arrayJsonView.FacultyDescription;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.SessionActivated))
                {
                    sessionActivated = Convert.ToBoolean(arrayJsonView.SessionActivated);
                }

                if (!string.IsNullOrEmpty(arrayJsonView.SequenceNumber))
                {
                    sequenceNumber = Convert.ToInt32(arrayJsonView.SequenceNumber);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.SemesterId))
                {
                    semesterId = Convert.ToInt32(arrayJsonView.SemesterId);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.SessionId))
                {
                    sessionId = Convert.ToInt32(arrayJsonView.SessionId);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.RoleName))
                {
                    roleName = arrayJsonView.RoleName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.RoleDescription))
                {
                    roleDescription = arrayJsonView.RoleDescription;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string CreateProgramme(string programmeName, string programmeDescription, string programmeShortName, bool programmeActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (programmeName != null && programmeShortName != null)
                {
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();
                    var programmeNameCheck = programmeName.Trim().Replace(" ", "");


                    Programme existingProgramme =
                        programmeLogic.GetModelsBy(
                            a =>
                                a.Programme_Name.Trim().Replace(" ", "").Contains(programmeNameCheck) &&
                                a.Activated == programmeActivated).LastOrDefault();

                    if (existingProgramme == null)
                    {
                        existingProgramme = new Programme();

                        existingProgramme.Name = programmeName;
                        existingProgramme.ShortName = programmeShortName;
                        existingProgramme.Description = programmeDescription;
                        existingProgramme.Activated = programmeActivated;

                        programmeLogic.Create(existingProgramme);

                        message = "Programme created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "ProgrammeName=" + programmeName + "ProgrammeShortName=" + programmeShortName + "ProgrammeActivated" + programmeActivated;
                        generalAudit.Operation = "Created New Programme";
                        generalAudit.TableNames = "Programme Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Programme already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateDepartment(string departmentName, string departmentCode, int facultyId, bool departmentActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (departmentName != null && departmentCode != null && facultyId > 0)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    var departmentNameCheck = departmentName.Trim().Replace(" ", "");

                    Department existingDepartment =
                        departmentLogic.GetModelsBy(
                            a =>
                                a.Department_Name.Trim().Replace(" ", "") == departmentNameCheck &&
                                a.Faculty_Id == facultyId).LastOrDefault();

                    if (existingDepartment == null)
                    {
                        existingDepartment = new Department();
                        FacultyLogic facultyLogic = new FacultyLogic();
                        var faculty = facultyLogic.GetModelBy(f => f.Faculty_Id == facultyId);
                        if (faculty == null) throw new ArgumentNullException("faculty");

                        existingDepartment.Name = departmentName;
                        existingDepartment.Code = departmentCode;
                        existingDepartment.Faculty = faculty;
                        existingDepartment.Active = departmentActivated;


                        departmentLogic.Create(existingDepartment);

                        message = "Department created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "DepartmentName=" + departmentName + "DepartmentCode=" + departmentCode + "Faculty=" + faculty+ "DepartmentActivated="+ departmentActivated;
                        generalAudit.Operation = "Created New Department";
                        generalAudit.TableNames = "Deparment Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Department already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateLevel(string levelName, string levelDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (levelName != null && levelDescription != null)
                {
                    LevelLogic levelLogic = new LevelLogic();
                    var levelNameCheck = levelName.Trim().Replace(" ", "");
                    var levelDescriptionCheck = levelDescription.Trim().Replace(" ", "");

                    Level existingLevel =
                        levelLogic.GetModelsBy(
                            a =>
                                a.Level_Name.Trim().Replace(" ", "").Contains(levelNameCheck) &&
                                a.Level_Description.Trim().Replace(" ", "").Contains(levelDescriptionCheck)).LastOrDefault();

                    if (existingLevel == null)
                    {
                        existingLevel = new Level();

                        existingLevel.Name = levelName;
                        existingLevel.Description = levelDescription;


                        levelLogic.Create(existingLevel);

                        message = "Level created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "LevelName=" + levelName + "LevelDescription=" + levelDescription;
                        generalAudit.Operation = "Created New Level";
                        generalAudit.TableNames = "Level Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Level already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateSession(string sessionName, bool sessionActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionName != null)
                {
                    SessionLogic sessionLogic = new SessionLogic();
                    var sessionNameCheck = sessionName.Trim().Replace(" ", "");

                    Session existingSession =
                        sessionLogic.GetModelsBy(
                            a =>
                                a.Session_Name.Trim().Replace(" ", "").Contains(sessionNameCheck) && a.Activated == sessionActivated).LastOrDefault();

                    if (existingSession == null)
                    {
                        existingSession = new Session();

                        existingSession.Name = sessionName;
                        existingSession.Activated = sessionActivated;
                        existingSession.StartDate = DateTime.UtcNow;
                        existingSession.EndDate = DateTime.UtcNow;


                        sessionLogic.Create(existingSession);

                        message = "Session created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "SessionName=" + sessionName + "SessionActivated=" + sessionActivated;
                        generalAudit.Operation = "Created New Session";
                        generalAudit.TableNames = "Session Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Session already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateFaculty(string facultyName, string facultyDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (facultyName != null)
                {
                    FacultyLogic facultyLogic = new FacultyLogic();
                    var facultyNameCheck = facultyName.Trim().Replace(" ", "");


                    Faculty existingFaculty =
                        facultyLogic.GetModelsBy(
                            a =>
                                a.Faculty_Name.Trim().Replace(" ", "").Contains(facultyNameCheck)).LastOrDefault();

                    if (existingFaculty == null)
                    {
                        existingFaculty = new Faculty();

                        existingFaculty.Name = facultyName;
                        existingFaculty.Description = facultyDescription;



                        facultyLogic.Create(existingFaculty);

                        message = "Faculty created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FacultyName=" + facultyName + "FacultyDescription=" + facultyDescription;
                        generalAudit.Operation = "Created New Faculty";
                        generalAudit.TableNames = "Faculty Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Faculty already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateDepartmentOption(string departmentOptionName, int departmentId, bool departmentOptionActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (departmentOptionName != null && departmentId > 0)
                {
                    DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                    var departmentOptionNameCheck = departmentOptionName.Trim().Replace(" ", "");


                    DepartmentOption existingDepartmentOption =
                        departmentOptionLogic.GetModelsBy(
                            a =>
                                a.Department_Option_Name.Trim().Replace(" ", "") == departmentOptionNameCheck && a.Department_Id == departmentId ).LastOrDefault();

                    if (existingDepartmentOption == null)
                    {
                        existingDepartmentOption = new DepartmentOption();
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        var department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                        if (department == null) throw new ArgumentNullException("departmentOptionName");

                        existingDepartmentOption.Name = departmentOptionName;
                        existingDepartmentOption.Activated = departmentOptionActivated;
                        existingDepartmentOption.Department = department;

                        List<DepartmentOption> options = departmentOptionLogic.GetAll();

                        int optionId = options != null && options.Count > 0 ? options.Max(o => o.Id) + 1 : 1;

                        existingDepartmentOption.Id = optionId;

                        departmentOptionLogic.Create(existingDepartmentOption);

                        message = "Department Option created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "DeptOptionName=" + departmentOptionName + "Department=" + department;
                        generalAudit.Operation = "Created New DepartmentOption";
                        generalAudit.TableNames = "DepartmentOption Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Department Option already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateProgrammeDepartment(int programmeId, int departmentId, bool programmedepartmentActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (programmeId > 0 && departmentId > 0)
                {
                    ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();



                    ProgrammeDepartment existingProgrammeDepartment =
                        programmeDepartmentLogic.GetModelsBy(
                            a =>
                                a.Department_Id == departmentId && a.Programme_Id == programmeId).LastOrDefault();

                    if (existingProgrammeDepartment == null)
                    {
                        existingProgrammeDepartment = new ProgrammeDepartment();
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        ProgrammeLogic programmeLogic = new ProgrammeLogic();

                        var department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                        var programme = programmeLogic.GetModelBy(p => p.Programme_Id == programmeId);

                        if (department == null) throw new ArgumentNullException("departmentId");
                        if (programme == null) throw new ArgumentNullException("programmeId");

                        existingProgrammeDepartment.Programme = programme;
                        existingProgrammeDepartment.Department = department;

                        programmeDepartmentLogic.Create(existingProgrammeDepartment);

                        message = "Programme Department created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "ProgrammeId=" + programmeId + "DepartmentId=" + departmentId;
                        generalAudit.Operation = "Created New ProgrammeDepartment";
                        generalAudit.TableNames = "ProgrammeDepartment Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Programme Department already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateSessionSemester(int sessionId, int semesterId, int sequenceNumber, bool registrationEnded)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && semesterId > 0)
                {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();



                    SessionSemester existingSessionSemester =
                        sessionSemesterLogic.GetModelsBy(
                            a =>
                                a.Semester_Id == semesterId && a.Session_Id == sessionId &&
                                a.Sequence_Number == sequenceNumber).LastOrDefault();

                    if (existingSessionSemester == null)
                    {
                        existingSessionSemester = new SessionSemester();
                        SemesterLogic semesterLogic = new SemesterLogic();
                        SessionLogic sessionLogic = new SessionLogic();

                        var semester = semesterLogic.GetModelBy(d => d.Semester_Id == semesterId);
                        var session = sessionLogic.GetModelBy(p => p.Session_Id == sessionId);

                        if (semester == null) throw new ArgumentNullException("semesterId");
                        if (session == null) throw new ArgumentNullException("sessionId");

                        existingSessionSemester.Semester = semester;
                        existingSessionSemester.Session = session;
                        existingSessionSemester.SequenceNumber = sequenceNumber;
                        existingSessionSemester.StartDate = DateTime.UtcNow;
                        existingSessionSemester.EndDate = DateTime.UtcNow;
                        existingSessionSemester.RegistrationEnded = registrationEnded;

                        sessionSemesterLogic.Create(existingSessionSemester);

                        message = "Session Semester created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "SequenceNo.=" + sequenceNumber + "RegistrationEnded=" + registrationEnded;
                        generalAudit.Operation = "Created New SessionSemester";
                        generalAudit.TableNames = "SessionSemester Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Session Semester already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string CreateRole(string roleName, string roleDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(roleName))
                {
                    RoleLogic roleLogic = new RoleLogic();
                    var roleNameCheck = roleName.Trim().Replace(" ", "");


                    Role existingRole =
                          roleLogic.GetModelsBy(
                              a =>
                                  a.Role_Name.Trim().Replace(" ", "") == roleNameCheck).LastOrDefault();


                    if (existingRole == null)
                    {
                        existingRole = new Role();

                        existingRole.Name = roleName;
                        existingRole.Description = roleDescription;



                        roleLogic.Create(existingRole);

                        message = "Role created Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "RoleName.=" + roleName + "Description=" + roleDescription;
                        generalAudit.Operation = "Created New Role";
                        generalAudit.TableNames = "Role Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Role already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveProgramme(int saveProgrammeId, string programmeName, string programmeDescription, string programmeShortName, bool programmeActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (saveProgrammeId > 0)
                {
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();
                    var programmeNameCheck = programmeName.Trim().Replace(" ", "");

                    Programme existingProgramme =
                        programmeLogic.GetModelsBy(
                            a =>
                                a.Programme_Name.Trim().Replace(" ", "") == programmeNameCheck &&
                                a.Activated == programmeActivated).LastOrDefault();
                    var initialProgramme=programmeLogic.GetModelBy(x => x.Programme_Id == saveProgrammeId);
                    if (existingProgramme == null)
                    {
                        existingProgramme = new Programme();

                        existingProgramme.Id = saveProgrammeId;
                        existingProgramme.Name = programmeName;
                        existingProgramme.Description = programmeDescription;
                        existingProgramme.Activated = programmeActivated;

                        programmeLogic.Modify(existingProgramme);

                        message = "Programme was modified";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Modify";
                        if (initialProgramme != null)
                        {
                            generalAudit.InitialValues = "ProgrammeName=" + initialProgramme.Name + "ProgrammeShortName=" + initialProgramme.ShortName + "ProgrammeActivated" + initialProgramme.Activated;
                        }
                        generalAudit.CurrentValues = "ProgrammeName=" + programmeName + "ProgrammeShortName=" + programmeShortName + "ProgrammeActivated" + programmeActivated;
                        generalAudit.Operation = "Modify New Programme";
                        generalAudit.TableNames = "Programme Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Programme values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveDepartment(int saveDepartmentId, string departmentName, string departmentCode, int facultyId, bool departmentActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (saveDepartmentId > 0 && facultyId > 0)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();

                    var departmentNameCheck = departmentName.Trim().Replace(" ", "");
                    var departmentCodeCheck = departmentCode.Trim().Replace(" ", "");

                    Department existingDepartment =
                        departmentLogic.GetModelsBy(
                            a =>
                                a.Department_Name.Trim().Replace(" ", "") == departmentNameCheck &&
                                a.Department_Code.Trim().Replace(" ", "") == departmentCodeCheck &&
                                a.Faculty_Id == facultyId && a.Active == departmentActivated).LastOrDefault();

                    FacultyLogic facultyLogic = new FacultyLogic();
                    var faculty = facultyLogic.GetModelBy(f => f.Faculty_Id == facultyId);
                    if (faculty == null) throw new ArgumentNullException("departmentName");
                    var initialDepartment=departmentLogic.GetModelBy(x => x.Department_Id == saveDepartmentId);

                    if (existingDepartment == null)
                    {
                        existingDepartment = new Department();

                        existingDepartment.Id = saveDepartmentId;
                        existingDepartment.Name = departmentName;
                        existingDepartment.Code = departmentCode;
                        existingDepartment.Faculty = faculty;
                        existingDepartment.Active = departmentActivated;

                        departmentLogic.Modify(existingDepartment);

                        message = "Department was modified";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Modify";
                        if (initialDepartment != null)
                        {
                            generalAudit.InitialValues= "DepartmentName=" + initialDepartment.Name + "DepartmentCode=" + initialDepartment.Code + "Faculty=" + initialDepartment.Faculty.Name + "DepartmentActivated=" + initialDepartment.Active;
                        }
                        generalAudit.CurrentValues = "DepartmentName=" + departmentName + "DepartmentCode=" + departmentCode + "Faculty=" + faculty.Name + "DepartmentActivated=" + departmentActivated;
                        generalAudit.Operation = "Modify New Department";
                        generalAudit.TableNames = "Deparment Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Department values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveLevel(int saveLevelId, string levelName, string levelDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(levelDescription))
                {
                    LevelLogic levelLogic = new LevelLogic();

                    var levelNameCheck = levelName.Trim().Replace(" ", "");
                    var levelDescriptionCheck = levelDescription.Trim().Replace(" ", "");

                    Level existingLevel =
                        levelLogic.GetModelsBy(
                            a =>
                                a.Level_Name.Trim().Replace(" ", "") == levelNameCheck &&
                                a.Level_Description.Trim().Replace(" ", "") == levelDescriptionCheck).LastOrDefault();
                    var initialLevel=levelLogic.GetModelBy(x => x.Level_Id == saveLevelId);
                    if (existingLevel == null)
                    {
                        existingLevel = new Level();

                        existingLevel.Id = saveLevelId;
                        existingLevel.Name = levelName;
                        existingLevel.Description = levelDescription;

                        levelLogic.Modify(existingLevel);

                        message = "Level was modified";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Modify";
                        if (initialLevel != null)
                        {
                            generalAudit.InitialValues = "LevelName=" + initialLevel.Name + "LevelDescription=" + initialLevel.Description;
                        }
                        generalAudit.CurrentValues = "LevelName=" + levelName + "LevelDescription=" + levelDescription;
                        generalAudit.Operation = "Modify New Level";
                        generalAudit.TableNames = "Level Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Level values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveSession(int saveSessionId, string sessionName, bool sessionActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(sessionName) && saveSessionId > 0)
                {
                    SessionLogic sessionLogic = new SessionLogic();

                    var sessionNameCheck = sessionName.Trim().Replace(" ", "");

                    Session existingSession =
                        sessionLogic.GetModelsBy(
                            a =>
                                a.Session_Name.Trim().Replace(" ", "") == sessionNameCheck && a.Activated == sessionActivated).LastOrDefault();

                    var initialSession=sessionLogic.GetModelBy(x => x.Session_Id == saveSessionId);
                    if (existingSession == null)
                    {
                        existingSession = new Session();

                        existingSession.Id = saveSessionId;
                        existingSession.Name = sessionName;
                        existingSession.Activated = sessionActivated;
                        existingSession.StartDate = DateTime.UtcNow;
                        existingSession.EndDate = DateTime.UtcNow;


                        sessionLogic.Modify(existingSession);

                        message = "Session was modified Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Modify";
                        if (initialSession != null)
                        {
                            generalAudit.InitialValues = "SessionName=" + initialSession.Name + "SessionActivated=" + initialSession.Activated;
                        }
                        generalAudit.CurrentValues = "SessionName=" + sessionName + "SessionActivated=" + sessionActivated;
                        generalAudit.Operation = "Modify Session";
                        generalAudit.TableNames = "Session Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Session values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveFaculty(int saveFacultyId, string facultyName, string facultyDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(facultyName))
                {
                    FacultyLogic facultyLogic = new FacultyLogic();
                    var facultyNameCheck = facultyName.Trim().Replace(" ", "");
                    var facultyDescriptionCheck = facultyDescription.Trim().Replace(" ", "");

                    Faculty existingFaculty =
                        facultyLogic.GetModelsBy(
                            a =>
                                a.Faculty_Name.Trim().Replace(" ", "") == facultyNameCheck && a.Faculty_Description.Trim().Replace(" ", "") == facultyDescriptionCheck).LastOrDefault();
                    var initialFaculty=facultyLogic.GetModelBy(x => x.Faculty_Id == saveFacultyId);
                    if (existingFaculty == null)
                    {
                        existingFaculty = new Faculty();

                        existingFaculty.Id = saveFacultyId;
                        existingFaculty.Name = facultyName;
                        existingFaculty.Description = facultyDescription;

                        facultyLogic.Modify(existingFaculty);

                        message = "Faculty was modified Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Modify";
                        if (initialFaculty != null)
                        {
                            generalAudit.InitialValues = "FacultyName=" + initialFaculty.Name + "FacultyDescription=" + initialFaculty.Description;
                        }
                        generalAudit.CurrentValues = "FacultyName=" + facultyName + "FacultyDescription=" + facultyDescription;
                        generalAudit.Operation = "Modify Faculty";
                        generalAudit.TableNames = "Faculty Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Faculty values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveDepartmentOption(int saveDepartmentOptionId, string departmentOptionName, bool departmentOptionActivated, int departmentId)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(departmentOptionName) && departmentId > 0)
                {
                    DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                    var departmentOptionNameCheck = departmentOptionName.Trim().Replace(" ", "");


                    DepartmentOption existingDepartmentOption =
                        departmentOptionLogic.GetModelsBy(
                            a =>
                                a.Department_Option_Name.Trim().Replace(" ", "") == departmentOptionNameCheck && a.Department_Id == departmentId && a.Activated == departmentOptionActivated).LastOrDefault();

                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    var department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                    if (department == null) throw new ArgumentNullException("departmentOptionName");
                    var initialDeptOption=departmentOptionLogic.GetModelBy(x => x.Department_Option_Id == saveDepartmentOptionId);
                    if (existingDepartmentOption == null)
                    {
                        existingDepartmentOption = new DepartmentOption();

                        existingDepartmentOption.Id = saveDepartmentOptionId;
                        existingDepartmentOption.Name = departmentOptionName;
                        existingDepartmentOption.Activated = departmentOptionActivated;
                        existingDepartmentOption.Department = department;
                        departmentOptionLogic.Modify(existingDepartmentOption);

                        message = "Fee was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialDeptOption != null)
                        {
                            generalAudit.InitialValues = "DeptOptionName=" + initialDeptOption.Name + "Department=" + initialDeptOption.Department.Name;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "DeptOptionName=" + departmentOptionName + "Department=" + department.Name;
                        generalAudit.Operation = "Created New DepartmentOption";
                        generalAudit.TableNames = "DepartmentOption Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Fee values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveProgrammeDepartment(int saveDepartmentOptionId, int programmeId, int departmentId, bool programmedepartmentActivated)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (programmeId > 0 && departmentId > 0)
                {
                    ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();



                    ProgrammeDepartment existingProgrammeDepartment =
                         programmeDepartmentLogic.GetModelsBy(
                             a =>
                                 a.Department_Id == departmentId && a.Programme_Id == programmeId).LastOrDefault();

                    var initialProgrammeDept=programmeDepartmentLogic.GetModelsBy(x => x.Programme_Id == programmeId && x.Department_Id==departmentId).FirstOrDefault();
                    if (existingProgrammeDepartment == null)
                    {
                        existingProgrammeDepartment = new ProgrammeDepartment();
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        ProgrammeLogic programmeLogic = new ProgrammeLogic();

                        var department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                        var programme = programmeLogic.GetModelBy(p => p.Programme_Id == programmeId);

                        if (department == null) throw new ArgumentNullException("departmentId");
                        if (programme == null) throw new ArgumentNullException("programmeId");

                        existingProgrammeDepartment.Programme = programme;
                        existingProgrammeDepartment.Department = department;



                        programmeDepartmentLogic.Modify(existingProgrammeDepartment);

                        message = "Programme Department Edited Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialProgrammeDept != null)
                        {
                            generalAudit.InitialValues = "ProgrammeId=" + initialProgrammeDept.Programme.Id + "DepartmentId=" + initialProgrammeDept.Department.Id;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "ProgrammeId=" + programmeId + "DepartmentId=" + departmentId;
                        generalAudit.Operation = "Modify ProgrammeDepartment";
                        generalAudit.TableNames = "ProgrammeDepartment Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Programme Department values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveSessionSemester(int saveSessionSemesterId, int semesterId, int sessionId, int sequenceNumber, bool registrationEnded)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (semesterId > 0 && sessionId > 0)
                {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                    SessionSemester existingSessionSemester =
                         sessionSemesterLogic.GetModelsBy(
                             a =>
                                 a.Semester_Id == semesterId && a.Session_Id == sessionId &&
                                 a.Sequence_Number == sequenceNumber && a.Registration_Ended == registrationEnded).LastOrDefault();

                    var initialSessionSmester=sessionSemesterLogic.GetModelBy(x => x.Session_Semester_Id == saveSessionSemesterId);
                    if (existingSessionSemester == null)
                    {
                        existingSessionSemester = new SessionSemester();
                        SessionLogic sessionLogic = new SessionLogic();
                        SemesterLogic semesterLogic = new SemesterLogic();

                        var session = sessionLogic.GetModelBy(d => d.Session_Id == sessionId);
                        var semester = semesterLogic.GetModelBy(p => p.Semester_Id == semesterId);

                        if (session == null) throw new ArgumentNullException("semesterId");
                        if (semester == null) throw new ArgumentNullException("semesterId");

                        existingSessionSemester.Id = saveSessionSemesterId;
                        existingSessionSemester.Session = session;
                        existingSessionSemester.Semester = semester;
                        existingSessionSemester.SequenceNumber = sequenceNumber;
                        existingSessionSemester.RegistrationEnded = registrationEnded;


                        sessionSemesterLogic.Modify(existingSessionSemester);

                        message = "Session Semester Edited Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialSessionSmester != null)
                        {
                            generalAudit.InitialValues = "SequenceNo.=" + initialSessionSmester.SequenceNumber + "RegistrationEnded=" + initialSessionSmester.RegistrationEnded;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "SequenceNo.=" + sequenceNumber + "RegistrationEnded=" + registrationEnded;
                        generalAudit.Operation = "Modify SessionSemester";
                        generalAudit.TableNames = "SessionSemester Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Session Semester values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveRole(int saveRoleId, string roleName, string roleDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(roleName))
                {

                    RoleLogic roleLogic = new RoleLogic();
                    var roleNameCheck = roleName.Trim().Replace(" ", "");
                    var roleDescriptionCheck = roleDescription.Trim().Replace(" ", "");


                    Role existingRole =
                        roleLogic.GetModelsBy(
                            a =>
                                a.Role_Name.Trim().Replace(" ", "") == roleNameCheck).LastOrDefault();

                    var initialRole=roleLogic.GetModelBy(x => x.Role_Id == saveRoleId);
                    if (existingRole == null)
                    {
                        existingRole = new Role();
                        existingRole.Id = saveRoleId;
                        existingRole.Name = roleName;
                        existingRole.Description = roleDescription;


                        roleLogic.Modify(existingRole);

                        message = "Role Edited Successfully";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialRole != null)
                        {
                            generalAudit.InitialValues = "RoleName.=" + initialRole.Name + "Description=" + initialRole.Description;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "RoleName.=" + roleName + "Description=" + roleDescription;
                        generalAudit.Operation = "Modify Role";
                        generalAudit.TableNames = "Role Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Role values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
    }
}