using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Data.OleDb;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class MatricNumberController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private MatricNumberViewModel viewModel;
        private string FileUploadURL = null;
        protected const string ContainsDuplicate = "Error Occurred, the data contains duplicates, Please try again or contact ICT";
        protected const string ArgumentNullException = "Null object argument. Please contact your system administrator";
        public ActionResult MatricNumberFormat()
        {
            try
            {
                viewModel = new MatricNumberViewModel();
                //StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();

                //viewModel.MatricNumberAssignments = matricNumberAssignmentLogic.GetAll();
                ViewBag.Programmes = viewModel.ProgrammeSelectList;
                ViewBag.Sessions = viewModel.SessionSelectList;
                ViewBag.Levels = viewModel.LevelSelectList;
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.DepartmentOptions = viewModel.DepartmentOptionSelectList;

                //if (viewModel.MatricNumberAssignments != null && viewModel.MatricNumberAssignments.Count > 0)
                //{
                //    viewModel.MatricNumberAssignments =
                //        viewModel.MatricNumberAssignments.OrderBy(m => m.Session.Id)
                //            .ThenBy(m => m.Programme.Id)
                //            .ThenBy(m => m.Level.Id)
                //            .ThenBy(m => m.Department.Id)
                //            .ToList();
                //}
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult MatricNumberFormat(MatricNumberViewModel viewModel)
        {
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                DepartmentOptionLogic optionLogic = new DepartmentOptionLogic();

                viewModel.MatricNumberAssignments = matricNumberAssignmentLogic.GetModelsBy(m => m.Session_Id == viewModel.Session.Id);

                ViewBag.Programmes = viewModel.ProgrammeSelectList;
                ViewBag.Sessions = viewModel.SessionSelectList;
                ViewBag.Levels = viewModel.LevelSelectList;
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.DepartmentOptions = viewModel.DepartmentOptionSelectList;

                for (int i = 0; i < viewModel.MatricNumberAssignments.Count; i++)
                {
                    StudentMatricNumberAssignment assignment = viewModel.MatricNumberAssignments[i];
                    if (assignment.DepartmentOptionId > 0)
                    {
                        assignment.DepartmentOption = optionLogic.GetModelBy(o => o.Department_Option_Id == assignment.DepartmentOptionId);
                    }
                }

                if (viewModel.MatricNumberAssignments != null && viewModel.MatricNumberAssignments.Count > 0)
                {
                    viewModel.MatricNumberAssignments =
                        viewModel.MatricNumberAssignments.OrderBy(m => m.Session.Id)
                            .ThenBy(m => m.Programme.Id)
                            .ThenBy(m => m.Level.Id)
                            .ThenBy(m => m.Department.Id)
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Programmes = viewModel.ProgrammeSelectList;
            ViewBag.Sessions = viewModel.SessionSelectList;
            ViewBag.Levels = viewModel.LevelSelectList;
            ViewBag.Departments = viewModel.DepartmentSelectList;
            ViewBag.DepartmentOptions = viewModel.DepartmentOptionSelectList;

            return View(viewModel);
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
        public JsonResult GetDepartmentsAlt(int programmeId, int facultyId)
        {
            try
            {
                if (programmeId <= 0 && facultyId <= 0)
                {
                    return null;
                }

                Programme programme = new Programme() { Id = programmeId };
                Faculty faculty = new Faculty() { Id = facultyId };

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme, faculty);

                return Json(new SelectList(departments, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult RemoveFormat(string formatId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();

                long formatIdToDelete = Convert.ToInt64(formatId);

                matricNumberAssignmentLogic.Delete(x => x.Id == formatIdToDelete);

                result.IsError = false;
                result.Message = "Operation Successful!";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error! " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditFormat(int formatId, string myRecordArray, string action)
        {
            MatricNumberModel resultModel = new MatricNumberModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentMatricNumberAssignment matricNumberAssignment = null;

                if (action == "edit")
                {
                    if (formatId > 0)
                    {
                        matricNumberAssignment = matricNumberAssignmentLogic.GetModelBy(f => f.Id == formatId);
                        if (matricNumberAssignment != null)
                        {
                            resultModel.IsError = false;
                            resultModel.Id = matricNumberAssignment.Id.ToString();
                            resultModel.Session = matricNumberAssignment.Session.Id.ToString();
                            resultModel.Programme = matricNumberAssignment.Programme.Id.ToString();
                            resultModel.Level = matricNumberAssignment.Level.Id.ToString();
                            resultModel.Department = matricNumberAssignment.Department.Id.ToString();
                            resultModel.Format = matricNumberAssignment.MatricNoStartFrom;
                            resultModel.StartFrom = matricNumberAssignment.MatricSerialNoStartFrom.ToString();
                            resultModel.DepartmentCode = matricNumberAssignment.DepartmentCode;
                            resultModel.IsError = false;
                            if (matricNumberAssignment.DepartmentOptionId > 0)
                            {
                                resultModel.DepartmentOption = matricNumberAssignment.DepartmentOptionId.ToString();
                            }
                        }
                        else
                        {
                            resultModel.IsError = true;
                            resultModel.Message = "Record does not exist in the database.";
                        }
                    }
                    else
                    {
                        resultModel.IsError = true;
                        resultModel.Message = "Edit parameter was not set.";
                    }
                }

                if (action == "save")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    MatricNumberModel arrayJsonView = serializer.Deserialize<MatricNumberModel>(myRecordArray);

                    int sessionId = 0;
                    string format = null;
                    string departmentCode = null;
                    int programmeId = 0;
                    int departmentId = 0;
                    int departmentOptionId = 0;
                    int startFrom = 0;
                    int levelId = 0;
                    int id = 0;

                    if (!string.IsNullOrEmpty(arrayJsonView.Session))
                    {
                        sessionId = Convert.ToInt32(arrayJsonView.Session);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.StartFrom))
                    {
                        startFrom = Convert.ToInt32(arrayJsonView.StartFrom);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Programme))
                    {
                        programmeId = Convert.ToInt32(arrayJsonView.Programme);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Department))
                    {
                        departmentId = Convert.ToInt32(arrayJsonView.Department);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.DepartmentOption))
                    {
                        departmentOptionId = Convert.ToInt32(arrayJsonView.DepartmentOption);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Id))
                    {
                        id = Convert.ToInt32(arrayJsonView.Id);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Format))
                    {
                        format = arrayJsonView.Format;
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.DepartmentCode))
                    {
                        departmentCode = arrayJsonView.DepartmentCode;
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Level))
                    {
                        levelId = Convert.ToInt32(arrayJsonView.Level);
                    }

                    if (sessionId > 0 && programmeId > 0 && levelId > 0 && departmentId > 0 && startFrom >= 0 && !string.IsNullOrEmpty(format) && id > 0 )
                    {
                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        Department department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);

                        StudentMatricNumberAssignment existingMatricNumberAssignment = new StudentMatricNumberAssignment();
                        if (departmentOptionId > 0)
                        {
                            existingMatricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Department_Id == departmentId && m.Programme_Id == programmeId
                                                                && m.Level_Id == levelId && m.Session_Id == sessionId && m.Matric_Number_Start_From == format && 
                                                                m.Matric_Serial_Number_Start_From == startFrom && m.Department_Option_Id == departmentOptionId).LastOrDefault();
                        }
                        else
                        {
                            existingMatricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Department_Id == departmentId && m.Programme_Id == programmeId
                                                                && m.Level_Id == levelId && m.Session_Id == sessionId && m.Matric_Number_Start_From == format && m.Matric_Serial_Number_Start_From == startFrom
                                                                ).LastOrDefault();
                        }
                        
                        if (existingMatricNumberAssignment == null)
                        {
                            matricNumberAssignment = new StudentMatricNumberAssignment();
                            matricNumberAssignment.MatricSerialNoStartFrom = startFrom;
                            matricNumberAssignment.MatricNoStartFrom = format;
                            matricNumberAssignment.DepartmentCode = departmentCode;
                            matricNumberAssignment.Department = department;
                            matricNumberAssignment.Faculty = department.Faculty;
                            matricNumberAssignment.Id = id;
                            matricNumberAssignment.Used = true;
                            matricNumberAssignment.Level = new Level() { Id = levelId };
                            matricNumberAssignment.Programme = new Programme() { Id = programmeId };
                            matricNumberAssignment.Session = new Session() { Id = sessionId };
                            if (departmentOptionId > 0)
                            {
                                matricNumberAssignment.DepartmentOption = new DepartmentOption(){ Id = departmentOptionId };
                            }

                            matricNumberAssignmentLogic.Modify(matricNumberAssignment);
                            resultModel.IsError = false;
                            resultModel.Message = "Operation Successful!";
                        }
                        else
                        {
                            resultModel.IsError = true;
                            resultModel.Message = "Edited settings already exist, kindly check.";
                        }
                    }
                    else
                    {
                        resultModel.IsError = true;
                        resultModel.Message = "One of the required parameters was not set.";
                    }
                }
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = "Error! " + ex.Message;
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFormat(string myRecordArray)
        {
            MatricNumberModel resultModel = new MatricNumberModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentMatricNumberAssignment matricNumberAssignment = null;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    MatricNumberModel arrayJsonView = serializer.Deserialize<MatricNumberModel>(myRecordArray);

                    int sessionId = 0;
                    string format = null;
                    string departmentCode = null;
                    int programmeId = 0;
                    int departmentId = 0;
                    int departmentOptionId = 0;
                    int startFrom = 0;
                    int levelId = 0;
                    int id = 0;

                    if (!string.IsNullOrEmpty(arrayJsonView.Session))
                    {
                        sessionId = Convert.ToInt32(arrayJsonView.Session);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.StartFrom))
                    {
                        startFrom = Convert.ToInt32(arrayJsonView.StartFrom);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Programme))
                    {
                        programmeId = Convert.ToInt32(arrayJsonView.Programme);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Department))
                    {
                        departmentId = Convert.ToInt32(arrayJsonView.Department);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.DepartmentOption))
                    {
                        departmentOptionId = Convert.ToInt32(arrayJsonView.DepartmentOption);
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Format))
                    {
                        format = arrayJsonView.Format;
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.DepartmentCode))
                    {
                        departmentCode = arrayJsonView.DepartmentCode;
                    }
                    if (!string.IsNullOrEmpty(arrayJsonView.Level))
                    {
                        levelId = Convert.ToInt32(arrayJsonView.Level);
                    }

                    if (sessionId > 0 && programmeId > 0 && levelId > 0 && departmentId > 0 && startFrom >= 0 && !string.IsNullOrEmpty(format))
                    {
                        if (departmentOptionId > 0)
                        {
                            matricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Department_Id == departmentId && m.Level_Id == levelId && m.Session_Id == sessionId &&
                                                                                            m.Programme_Id == programmeId && m.Department_Option_Id == departmentOptionId).LastOrDefault();
                        }
                        else
                        {
                            matricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Department_Id == departmentId && m.Level_Id == levelId && m.Session_Id == sessionId &&
                                                                                            m.Programme_Id == programmeId).LastOrDefault();
                        }
                        
                        if (matricNumberAssignment == null)
                        {
                            DepartmentLogic departmentLogic = new DepartmentLogic();
                            Department department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);

                            matricNumberAssignment = new StudentMatricNumberAssignment();
                            List<StudentMatricNumberAssignment> matAssignments = matricNumberAssignmentLogic.GetAll();
                            int nextId = 1;
                            if (matAssignments != null && matAssignments.Count > 0)
                            {
                                nextId = matAssignments.Max(x => x.Id) + 1;
                            }
                            matricNumberAssignment.Id = nextId;
                            matricNumberAssignment.MatricSerialNoStartFrom = startFrom;
                            matricNumberAssignment.DepartmentCode = departmentCode;
                            matricNumberAssignment.MatricNoStartFrom = format;
                            matricNumberAssignment.Department = department;
                            matricNumberAssignment.Faculty = department.Faculty;
                            matricNumberAssignment.Used = true;
                            matricNumberAssignment.Level = new Level() { Id = levelId };
                            matricNumberAssignment.Programme = new Programme() { Id = programmeId };
                            matricNumberAssignment.Session = new Session() { Id = sessionId };
                            if (departmentOptionId > 0)
                            {
                                matricNumberAssignment.DepartmentOption = new DepartmentOption() { Id = departmentOptionId };
                            }

                            matricNumberAssignmentLogic.Create(matricNumberAssignment);
                            resultModel.IsError = false;
                            resultModel.Message = "Operation Successful!";
                        }
                        else
                        {
                            resultModel.IsError = true;
                            resultModel.Message = "Format for your selection exists.";
                        }
                    }
                    else
                    {
                        resultModel.IsError = true;
                        resultModel.Message = "One of the required parameters was not set.";
                    }
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = "Error! " + ex.Message;
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Department department = new Department() { Id = Convert.ToInt32(id) };
                Programme programme = new Programme() { Id = Convert.ToInt32(programmeid) };
                DepartmentOptionLogic departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

                //departmentOptions.Insert(0, new DepartmentOption(){ Id = 0 , Name = "-- Select Option --"});

                return Json(new SelectList(departmentOptions, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetLevels(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                LevelLogic levelLogic = new LevelLogic();
                List<Level> levels = levelLogic.GetBy(programme);

                return Json(new SelectList(levels, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult MatricNumberGenerationReport()
        {
            MatricNumberViewModel viewModel = new MatricNumberViewModel();
            try
            {
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");
                ViewBag.Session = viewModel.SessionSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult MatricNumberGenerationReport(MatricNumberViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                    viewModel.StudentModels = matricNumberAssignmentLogic.GetMatricNumberList(viewModel.Programme, viewModel.Department, viewModel.Session, viewModel.DepartmentOption);

                    if (viewModel.StudentModels != null && viewModel.StudentModels.Count > 0)
                    {
                        viewModel.StudentModels = viewModel.StudentModels.OrderBy(s => s.Name).ToList();
                    }

                    if (viewModel.StudentModels != null && viewModel.StudentModels.Count <= 0)
                    {
                        SetMessage("No records found.", Message.Category.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            DepartmentLogic departmentLogic = new DepartmentLogic();
            List<Department> departments = departmentLogic.GetBy(viewModel.Programme);
           
            ViewBag.Programme = viewModel.ProgrammeSelectList;
            ViewBag.Department = new SelectList(departments, "Id", "Name", viewModel.Department.Id);
            ViewBag.Session = viewModel.SessionSelectList;
            //ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");

            if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
            {
                ViewBag.DepartmentOption = new SelectList(Utility.PopulateDepartmentOptionSelectListItem(viewModel.Department, viewModel.Programme), Utility.VALUE, Utility.TEXT, viewModel.DepartmentOption.Id);

            }
            else
            {
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");
            }

            return View(viewModel);
        }
        public ActionResult StudentList()
        {
            MatricNumberViewModel viewModel = new MatricNumberViewModel();
            try
            {
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Level = viewModel.LevelSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult StudentList(MatricNumberViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                    viewModel.StudentModels = matricNumberAssignmentLogic.GetStudentList(viewModel.Programme, viewModel.Level, viewModel.Department, viewModel.DepartmentOption, viewModel.Session);

                    if (viewModel.StudentModels != null && viewModel.StudentModels.Count > 0)
                    {
                        viewModel.StudentModels = viewModel.StudentModels.OrderBy(s => s.Name).ToList();
                    }

                    if (viewModel.StudentModels != null && viewModel.StudentModels.Count <= 0)
                    {
                        SetMessage("No records found.", Message.Category.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            DepartmentLogic departmentLogic = new DepartmentLogic();
            DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
            List<Department> departments = departmentLogic.GetBy(viewModel.Programme);
            List<DepartmentOption> departmentOptions = departmentOptionLogic.GetBy(viewModel.Department, viewModel.Programme);
            if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
            {
                ViewBag.DepartmentOption = new SelectList(departmentOptions, "Id", "Name", viewModel.DepartmentOption.Id);
            }
            else
            {
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), "Id", "Name");
            }

            ViewBag.Programme = viewModel.ProgrammeSelectList;
            ViewBag.Department = new SelectList(departments, "Id", "Name", viewModel.Department.Id);
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Level = viewModel.LevelSelectList;

            return View(viewModel);
        }

        //Generate by Department
        public JsonResult GenerateMatricNumber(int formatId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLogic studentLogic = new StudentLogic();
                FeeDetailLogic _feeDetailLogic = new FeeDetailLogic();
                EWalletPaymentLogic eWalletPaymentLogic = new EWalletPaymentLogic();
                EWalletPayment eWalletPayment = new EWalletPayment();
                PaymentMode paymentMode = new PaymentMode() { Id = 1 };
                FeeType feeType = new FeeType() { Id = 3 };
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = new Payment();
                //Model.Model.Student 


       

                StudentMatricNumberAssignment matricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Id == formatId).LastOrDefault();

                List<StudentLevel> studentLevels = new List<StudentLevel>();

                int successfulRecords = 0;

                if (matricNumberAssignment != null)
                {
                    if (matricNumberAssignment.DepartmentOptionId > 0)
                    {
                        studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                        s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                        s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id && s.STUDENT.Matric_Number == null &&
                                        s.STUDENT.Activated != false && s.Department_Option_Id == matricNumberAssignment.DepartmentOptionId).OrderBy(s => s.Student.FullName).ToList();
                    }
                    else
                    {
                        studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                        s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                        s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id && s.STUDENT.Matric_Number == null &&
                                        s.STUDENT.Activated != false).OrderBy(s => s.Student.FullName).ToList();
                    }
                    
                    for (int i = 0; i < studentLevels.Count; i++)
                    {
                        if (String.IsNullOrEmpty(studentLevels[i].Student.MatricNumber))
                        {
                            Model.Model.Student student = studentLevels[i].Student;
                            Session student_session = studentLevels[i].Session;
                            decimal amountToPay = _feeDetailLogic.GetFeeByDepartmentLevel(studentLevels[i].Department,
                                                studentLevels[i].Level, studentLevels[i].Programme, feeType, studentLevels[i].Session, paymentMode);

                            //Checks and present only new students who has paid the required school fees amount
                            payment = paymentLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Fee_Type_Id == 3).LastOrDefault();
                            eWalletPayment = eWalletPaymentLogic.GetModelsBy(e => e.Person_Id == student.Id && (e.Fee_Type_Id == 3 || e.Fee_Type_Id == 12 || e.Fee_Type_Id == 22) && e.Session_Id == student_session.Id).LastOrDefault();
                            if(payment != null)
                            {
                                var hasPaidSchoolFees = HasPaidSchoolFees(payment, studentLevels[i].Session, amountToPay);

                                if (hasPaidSchoolFees)
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        matricNumberAssignment = matricNumberAssignmentLogic.GetModelBy(m => m.Id == formatId);

                                        int studentNumber = matricNumberAssignment.MatricSerialNoStartFrom + 1;
                                        student.Number = student.Id;
                                        string matricNumberPad = PaddNumber(studentNumber, 4);
                                        student.MatricNumber = matricNumberAssignment.MatricNoStartFrom + matricNumberPad;

                                        studentLogic.Modify(student);

                                        matricNumberAssignment.MatricSerialNoStartFrom = studentNumber;
                                        matricNumberAssignmentLogic.Modify(matricNumberAssignment);
                                        successfulRecords += 1;

                                        scope.Complete();
                                    }
                                }

                            }
                            else if(payment == null && eWalletPayment != null)
                            {
                                var hasPaidSchoolFees = HasPaidSchoolFees(eWalletPayment.Payment, studentLevels[i].Session, amountToPay);

                                if (hasPaidSchoolFees)
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        matricNumberAssignment = matricNumberAssignmentLogic.GetModelBy(m => m.Id == formatId);

                                        int studentNumber = matricNumberAssignment.MatricSerialNoStartFrom + 1;
                                        student.Number = student.Id;
                                        string matricNumberPad = PaddNumber(studentNumber, 4);
                                        student.MatricNumber = matricNumberAssignment.MatricNoStartFrom + matricNumberPad;

                                        studentLogic.Modify(student);

                                        matricNumberAssignment.MatricSerialNoStartFrom = studentNumber;
                                        matricNumberAssignmentLogic.Modify(matricNumberAssignment);

                                        successfulRecords += 1;

                                        scope.Complete();
                                    }
                                }
                            }
                        }
                        
                    }

                    if (successfulRecords > 0)
                    {
                        result.IsError = false;
                        result.Message = "Matric Number was generated for " + successfulRecords + " students.";
                    }
                    else
                    {
                        result.IsError = false;
                        result.Message = "No Matric Number was generated.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GenerateMatricNumberBulk(int sessionId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLogic studentLogic = new StudentLogic();

                if (sessionId <= 0)
                {
                    result.IsError = true;
                    result.Message = "Kindly select session to proceed.";

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                List<StudentMatricNumberAssignment> matricNumberAssignments = matricNumberAssignmentLogic.GetModelsBy(m => m.Session_Id == sessionId);

                for (int i = 0; i < matricNumberAssignments.Count; i++)
                {
                    StudentMatricNumberAssignment matricNumberAssignment = matricNumberAssignments[i];

                    List<StudentLevel> studentLevels = new List<StudentLevel>();

                    int successfulRecords = 0;

                    if (matricNumberAssignment != null)
                    {
                        if (matricNumberAssignment.DepartmentOptionId > 0)
                        {
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                            s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                            s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id && s.STUDENT.Matric_Number == null &&
                                            s.STUDENT.Activated != false && s.Department_Option_Id == matricNumberAssignment.DepartmentOptionId).OrderBy(s => s.Student.FullName).ToList();
                        }
                        else
                        {
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                            s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                            s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id && s.STUDENT.Matric_Number == null &&
                                            s.STUDENT.Activated != false).OrderBy(s => s.Student.FullName).ToList();
                        }
                        
                        for (int j = 0; j < studentLevels.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(studentLevels[j].Student.MatricNumber))
                            {
                                continue;
                            }
                            using (TransactionScope scope = new TransactionScope())
                            {
                                Model.Model.Student student = studentLevels[j].Student;
                                matricNumberAssignment = matricNumberAssignmentLogic.GetModelBy(m => m.Id == matricNumberAssignment.Id);

                                int studentNumber = matricNumberAssignment.MatricSerialNoStartFrom + 1;
                                student.Number = student.Id;
                                string matricNumberPad = PaddNumber(studentNumber, 4);
                                student.MatricNumber = matricNumberAssignment.MatricNoStartFrom + matricNumberPad;

                                studentLogic.Modify(student);

                                matricNumberAssignment.MatricSerialNoStartFrom = studentNumber;
                                matricNumberAssignmentLogic.Modify(matricNumberAssignment);
                                
                                scope.Complete();
                            }
                        }
                    }
                }

                result.IsError = false;
                result.Message = "Matriculation Numbers were generated for the selected session.";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ClearMatricNumber(int formatId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLogic studentLogic = new StudentLogic();

                StudentMatricNumberAssignment matricNumberAssignment = matricNumberAssignmentLogic.GetModelsBy(m => m.Id == formatId).LastOrDefault();

                List<StudentLevel> studentLevels = new List<StudentLevel>();

                int successfulRecords = 0;

                if (matricNumberAssignment != null)
                {
                    if (matricNumberAssignment.DepartmentOptionId > 0)
                    {
                        studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                        s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                        s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id 
                                        && s.Department_Option_Id == matricNumberAssignment.DepartmentOptionId).OrderBy(s => s.Student.FullName).ToList();
                    }
                    else
                    {
                        studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                        s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                        s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id).OrderBy(s => s.Student.FullName).ToList();
                    }
                    
                    for (int i = 0; i < studentLevels.Count; i++)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            Model.Model.Student student = studentLevels[i].Student;

                            student.MatricNumber = null;

                            studentLogic.ModifyMatricNumber(student);
                            
                            scope.Complete();
                        }
                    }

                    matricNumberAssignment.MatricSerialNoStartFrom = 0;
                    matricNumberAssignmentLogic.Modify(matricNumberAssignment);

                    result.IsError = false;
                    result.Message = "Matric Numbers were cleared.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ClearMatricNumberBulk(int sessionId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLogic studentLogic = new StudentLogic();

                if (sessionId <= 0)
                {
                    result.IsError = true;
                    result.Message = "Kindly select session to proceed.";

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                List<StudentMatricNumberAssignment> matricNumberAssignments = matricNumberAssignmentLogic.GetModelsBy(m => m.Session_Id == sessionId);

                for (int i = 0; i < matricNumberAssignments.Count; i++)
                {
                    StudentMatricNumberAssignment matricNumberAssignment = matricNumberAssignments[i];

                    List<StudentLevel> studentLevels = new List<StudentLevel>();
                    
                    if (matricNumberAssignment != null)
                    {
                        if (matricNumberAssignment.DepartmentOptionId > 0)
                        {
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                            s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                            s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id 
                                            && s.Department_Option_Id == matricNumberAssignment.DepartmentOptionId).OrderBy(s => s.Student.FullName).ToList();
                        }
                        else
                        {
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == matricNumberAssignment.Programme.Id && s.Department_Id == matricNumberAssignment.Department.Id &&
                                            s.Level_Id == matricNumberAssignment.Level.Id && s.Session_Id == matricNumberAssignment.Session.Id &&
                                            s.STUDENT.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == matricNumberAssignment.Session.Id).OrderBy(s => s.Student.FullName).ToList();
                        }
                        
                        for (int j = 0; j < studentLevels.Count; j++)
                        {
                            using (TransactionScope scope = new TransactionScope())
                            {
                                Model.Model.Student student = studentLevels[j].Student;

                                student.MatricNumber = null;

                                studentLogic.ModifyMatricNumber(student);

                                scope.Complete();
                            }
                        }

                        matricNumberAssignment.MatricSerialNoStartFrom = 0;
                        matricNumberAssignmentLogic.Modify(matricNumberAssignment);

                        result.IsError = false;
                        result.Message = "Matric Numbers were cleared.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    StringBuilder builder = new StringBuilder();
                    if (zeroCount > 0)
                    {
                        for (int counter = 0; counter < zeroCount; counter++)
                        {
                            builder.Append("0");
                        }
                    }
                    

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult MatricNumberGeneration()
        {
            try
            {
                viewModel = new MatricNumberViewModel();
                ViewBag.Programmes = viewModel.ProgrammeSelectList;
                ViewBag.Sessions = viewModel.SessionSelectList;
                ViewBag.Faculties = viewModel.FacultySelectList;
                ViewBag.Departments = viewModel.DepartmentSelectList;
                ViewBag.Status = GetStudentStatus();
                
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public List<SelectListItem> GetStudentStatus()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            try
            {
                status.Add(new SelectListItem() { Text = "All" , Value = "0"});
                status.Add(new SelectListItem() { Text = "Active", Value = "1"});
                status.Add(new SelectListItem() { Text = "Not Active", Value = "2"});
            }
            catch (Exception)
            {
                throw;
            }

            return status;
        }
        public JsonResult GetLargestMatricNumber(string wildcard)
        {
            List<StudentJsonResultModel> results = new List<StudentJsonResultModel>();
            StudentJsonResultModel result = new StudentJsonResultModel();
            try
            {
                if (!string.IsNullOrEmpty(wildcard))
                {
                    StudentLogic studentLogic = new StudentLogic();

                    List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number.Contains(wildcard));
                    
                    if (students.Count > 0)
                    {
                        Model.Model.Student maxMatricNumberStudent = new Model.Model.Student();
                        int maxNumber = 0;

                        for (int i = 0; i < students.Count; i++)
                        {
                            int lastNumber = 0;
                            if (int.TryParse(students[i].MatricNumber.Split('-').LastOrDefault(), out lastNumber))
                            {
                                maxNumber = lastNumber > maxNumber ? lastNumber : maxNumber;

                                maxMatricNumberStudent = maxNumber == lastNumber ? students[i] : maxMatricNumberStudent;
                            }
                        }
                        
                        result.IsError = false;
                        result.Name = maxMatricNumberStudent.FullName;
                        result.Sex = maxMatricNumberStudent.Sex != null ? maxMatricNumberStudent.Sex.Name : "NIL";
                        result.ApplicationNumber = maxMatricNumberStudent.ApplicationForm != null ? maxMatricNumberStudent.ApplicationForm.Number : "NIL";
                        result.MatricNumber = maxMatricNumberStudent.MatricNumber;
                        result.Status = maxMatricNumberStudent.Activated != false ? "Activated" : "Deactivated";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "No record found!";
                    } 
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            results.Add(result);

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudents(int programmeId, int sessionId, int departmentId, int statusId)
        {
            List<StudentJsonResultModel> students = new List<StudentJsonResultModel>();
            StudentJsonResultModel errorModel = new StudentJsonResultModel();
            FeeDetailLogic _feeDetailLogic = new FeeDetailLogic();
            PaymentMode paymentMode = new PaymentMode() { Id = 1};
            FeeType feeType = new FeeType() { Id = 3};
            EWalletPaymentLogic eWalletPaymentLogic = new EWalletPaymentLogic();
            EWalletPayment eWalletPayment = new EWalletPayment();
            PaymentLogic paymentLogic = new PaymentLogic();
            Payment payment = new Payment();
           
            try
            {
                if (programmeId > 0 && sessionId > 0 && departmentId > 0 && statusId >= 0)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    List<StudentLevel> studentLevels = new List<StudentLevel>();

                    switch (statusId)
                    {
                        case 0:
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == programmeId && s.Session_Id == sessionId && s.Department_Id == departmentId &&
                                            (s.Level_Id == (int)Levels.NDI || s.Level_Id == (int)Levels.HNDI || s.Level_Id == (int)Levels.HNDYRI || s.Level_Id == (int)Levels.NDYRI || s.Level_Id == (int)Levels.NDEI));
                            break;
                        case 1:
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == programmeId && s.Session_Id == sessionId && s.Department_Id == departmentId && s.STUDENT.Activated != false
                                            && (s.Level_Id == (int)Levels.NDI || s.Level_Id == (int)Levels.HNDI || s.Level_Id == (int)Levels.HNDYRI || s.Level_Id == (int)Levels.NDYRI || s.Level_Id == (int)Levels.NDEI));
                            break;
                        case 2:
                            studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == programmeId && s.Session_Id == sessionId && s.Department_Id == departmentId && s.STUDENT.Activated == false
                                            && (s.Level_Id == (int)Levels.NDI || s.Level_Id == (int)Levels.HNDI || s.Level_Id == (int)Levels.HNDYRI || s.Level_Id == (int)Levels.NDYRI || s.Level_Id == (int)Levels.NDEI));
                            break;
                    }

                    for (int i = 0; i < studentLevels.Count; i++)
                    {

                        Model.Model.Student student = studentLevels[i].Student;
                        var student_session = studentLevels[i].Session;

                        if (String.IsNullOrEmpty(student.MatricNumber))
                        {
                            decimal amountToPay = _feeDetailLogic.GetFeeByDepartmentLevel(studentLevels[i].Department,
                                            studentLevels[i].Level, studentLevels[i].Programme, feeType, studentLevels[i].Session, paymentMode);
                            //Checks and present only new students who has paid the required school fees amount
                            payment = paymentLogic.GetModelsBy(p => p.Person_Id == student.Id && p.Fee_Type_Id == 3).LastOrDefault();
                            eWalletPayment = eWalletPaymentLogic.GetModelsBy(e => e.Person_Id == student.Id && (e.Fee_Type_Id == 3 || e.Fee_Type_Id == 12 || e.Fee_Type_Id == 22) && e.Session_Id == student_session.Id).LastOrDefault();
                            if (payment != null)
                            {
                                var hasPaidSchoolFees = HasPaidSchoolFees(payment, studentLevels[i].Session, amountToPay);
                                if (hasPaidSchoolFees)
                                {
                                    StudentJsonResultModel studentJsonResult = new StudentJsonResultModel();
                                    studentJsonResult.IsError = false;
                                    studentJsonResult.Id = student.Id;
                                    studentJsonResult.Name = student.FullName;
                                    studentJsonResult.Sex = student.Sex != null ? student.Sex.Name : "NIL";
                                    studentJsonResult.ApplicationNumber = student.ApplicationForm != null ? student.ApplicationForm.Number : "NIL";
                                    studentJsonResult.MatricNumber = student.MatricNumber;
                                    studentJsonResult.Select = student.Activated != false ? true : false;
                                    studentJsonResult.Status = student.Activated != false ? "Activated" : "Deactivated";

                                    students.Add(studentJsonResult);
                                }
                            }

                            else if (payment == null && eWalletPayment != null)
                            {
                                var hasPaidSchoolFees = HasPaidSchoolFees(eWalletPayment.Payment, studentLevels[i].Session, amountToPay);

                                if (hasPaidSchoolFees)
                                {
                                    StudentJsonResultModel studentJsonResult = new StudentJsonResultModel();
                                    studentJsonResult.IsError = false;
                                    studentJsonResult.Id = student.Id;
                                    studentJsonResult.Name = student.FullName;
                                    studentJsonResult.Sex = student.Sex != null ? student.Sex.Name : "NIL";
                                    studentJsonResult.ApplicationNumber = student.ApplicationForm != null ? student.ApplicationForm.Number : "NIL";
                                    studentJsonResult.MatricNumber = student.MatricNumber;
                                    studentJsonResult.Select = student.Activated != false ? true : false;
                                    studentJsonResult.Status = student.Activated != false ? "Activated" : "Deactivated";

                                    students.Add(studentJsonResult);
                                }
                            }
                        }
                      
                    }
                    if (studentLevels.Count <= 0)
                    {
                        errorModel.IsError = true;
                        errorModel.Message = "No record found!";

                        students.Add(errorModel);
                    }
                    else if(students.Count <= 0){
                        errorModel.IsError = true;
                        errorModel.Message = "No record found as Students under the selected Department/Programme/Session have all been alloted matriculation Numbers!";

                        students.Add(errorModel);
                    }
                    else
                    {
                        students = students.OrderBy(s => s.Name).ToList();
                    }
                }
                else
                {
                    errorModel.IsError = true;
                    errorModel.Message = "Parameter not set!";

                    students.Add(errorModel);
                }
            }
            catch (Exception ex)
            {
                errorModel.IsError = true;
                errorModel.Message = ex.Message;
                students.Add(errorModel);
            }

            return Json(students, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GenerateMatricNumberAlt(string studentsArray, string prefix, string suffix, int startNumber, int numberLength, int programmeId, int sessionId, int departmentId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<StudentJsonResultModel> students = serializer.Deserialize<List<StudentJsonResultModel>>(studentsArray);

                if (students != null && students.Count > 0 && !string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(suffix) && startNumber >= 0 && numberLength > 0)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    int successfulRecords = 0;

                    students = students.OrderBy(s => s.Name).ToList();

                    string firstMatNumber = null;
                    string lastMatNumber = null;
                    int initialStartNumber = startNumber;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        for (int i = 0; i < students.Count; i++)
                        {
                            long personId = students[i].Id;

                            Model.Model.Student student = studentLogic.GetModelBy(s => s.Person_Id == personId);

                            //int studentNumber = startNumber + 1;
                            student.Number = student.Id;
                            string matricNumberPad = PaddNumber(startNumber, numberLength);
                            student.MatricNumber = prefix + suffix + matricNumberPad;

                            firstMatNumber = firstMatNumber ?? student.MatricNumber;
                            lastMatNumber = student.MatricNumber;

                            STUDENT existingStudent = studentLogic.GetEntitiesBy(s => s.Matric_Number == student.MatricNumber && s.Person_Id != personId).LastOrDefault();
                            if (existingStudent != null)
                            {
                                continue;
                            }

                            studentLogic.Modify(student);

                            startNumber += 1;

                            successfulRecords += 1;
                        }

                        scope.Complete();
                    }

                    if (successfulRecords > 0)
                    {
                        result.IsError = false;
                        result.Message = "Matric Number was generated for " + successfulRecords + " students.";

                        StudentMatricNumberGenerationAudit matricNumberGenerationAudit = new StudentMatricNumberGenerationAudit();
                        StudentMatricNumberGenerationAuditLogic auditLogic = new StudentMatricNumberGenerationAuditLogic();
                        UserLogic userLogic = new UserLogic();

                        string operation = "MODIFY";
                        string action = "GENERATED MATRIC NUMBER";
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                        User user = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);

                        matricNumberGenerationAudit.Action = action;
                        matricNumberGenerationAudit.Client = client;
                        matricNumberGenerationAudit.EndingMatricNumber = lastMatNumber;
                        matricNumberGenerationAudit.StartingMatricNumber = firstMatNumber;
                        matricNumberGenerationAudit.NumberLength = numberLength;
                        matricNumberGenerationAudit.Operation = operation;
                        matricNumberGenerationAudit.Time = DateTime.Now;
                        matricNumberGenerationAudit.User = user;
                        matricNumberGenerationAudit.StartNumber = initialStartNumber;
                        matricNumberGenerationAudit.Prefix = prefix;
                        matricNumberGenerationAudit.Suffix = suffix;
                        matricNumberGenerationAudit.Programme = programmeId > 0 ? new Programme(){ Id = programmeId} : null;
                        matricNumberGenerationAudit.Department = departmentId > 0 ? new Department() { Id = departmentId} : null;
                        matricNumberGenerationAudit.Session = sessionId > 0 ? new Session() { Id = sessionId} : null;

                        auditLogic.Create(matricNumberGenerationAudit);
                    }
                    else
                    {
                        result.IsError = false;
                        result.Message = "No Matric Number was generated.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error. " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadManuallyGeneratedMatricNo()
        {
            MatricNumberViewModel viewModel = new MatricNumberViewModel();
            try
            {
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                ViewBag.Session = viewModel.SessionSelectList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadManuallyGeneratedMatricNo(MatricNumberViewModel viewmodel)
        {
            
            try
            {
                //ViewBag.Programme = viewModel.ProgrammeSelectList;
                //ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                //ViewBag.Session = viewModel.SessionSelectList;
                GridView gv = new GridView();
                DataTable ds = new DataTable();
                StudentMatricNumberAssignmentLogic matricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                List<ExcelTemplateModel> sorted = new List<ExcelTemplateModel>();
                var matricNoList= matricNumberAssignmentLogic.GetStudentApplcationNoAndMatNo(viewmodel.Programme, viewmodel.Level, viewmodel.Department, viewmodel.Session);
                if (matricNoList.Count > 0)
                {
                    for(int y = 0; y < matricNoList.Count; y++)
                    {
                        matricNoList[y].SN = (y + 1);
                        sorted.Add(matricNoList[y]);
                    }
                    gv.DataSource = sorted;
                    ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                    ProgrammeDepartment programmeDepartment = programmeDepartmentLogic.GetModelsBy(p => p.Programme_Id == viewmodel.Programme.Id && p.Department_Id==viewmodel.Department.Id).FirstOrDefault();
                    gv.Caption = programmeDepartment.Programme.Name.ToUpper() + " " + " DEPARTMENT OF " + " " +
                                 programmeDepartment.Department.Name.ToUpper();

                    gv.DataBind();

                    string filename = programmeDepartment.Programme.Name.Replace("/", "").Replace("\\", "") + programmeDepartment.Department.Code + ".xls";
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
                throw ex;
            }

        }
        [HttpPost]
        //public ActionResult UploadMatricNumberExcel(MatricNumberViewModel matricNumberViewModel)
        public ActionResult ProcessUploadMatricNumberExcel()
        {
            MatricNumberViewModel matricNumberViewModel = new MatricNumberViewModel();
            try
            {
                List<ExcelTemplateModel> excelTemplateModelList = new List<ExcelTemplateModel>();
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
                            ExcelTemplateModel excelTemplateModel = new ExcelTemplateModel();
                            excelTemplateModel.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            excelTemplateModel.FullName = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            excelTemplateModel.ApplicationNo = studentSet.Tables[0].Rows[i][2].ToString().Trim();
                            excelTemplateModel.MatricNo = studentSet.Tables[0].Rows[i][3].ToString().Trim();
                            if (excelTemplateModel.ApplicationNo != "")
                            {
                                excelTemplateModelList.Add(excelTemplateModel);
                            }

                        }

                        matricNumberViewModel.ExcelTemplateModels = excelTemplateModelList;

                        TempData["matricNumberViewModel"] = matricNumberViewModel;
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

            KeepDropDownState(matricNumberViewModel);
            return View(matricNumberViewModel);
        }
        public ActionResult SaveUploadedMatricNoExcel()
        {
            MatricNumberViewModel matricNumberViewModel=(MatricNumberViewModel)TempData["matricNumberViewModel"];
            try
            {
                if (matricNumberViewModel != null)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    for (int i = 0; i < matricNumberViewModel.ExcelTemplateModels.Count; i++)
                    {
                        var applicationNumber = matricNumberViewModel.ExcelTemplateModels[i].ApplicationNo;
                        var matricNumber = matricNumberViewModel.ExcelTemplateModels[i].MatricNo;
                        var application= admissionListLogic.GetModelsBy(f => f.APPLICATION_FORM.Application_Form_Number == applicationNumber).FirstOrDefault();
                        if (application != null)
                        {
                            var student=studentLogic.GetModelsBy(d => d.Person_Id == application.Form.Person.Id).FirstOrDefault();
                            if (student != null)
                            {
                                student.MatricNumber = matricNumber;
                                if (student.MatricNumber == "")
                                {
                                    student.MatricNumber = null;
                                }
                                studentLogic.ModifyMatricNumber(student);
                            }
                            else
                            {
                                SetMessage("Validation Error! This Application Number" + applicationNumber + ", " + " Does Not Have Student Record. Cross check and re-upload!", Message.Category.Error);
                                KeepDropDownState(matricNumberViewModel);
                                return RedirectToAction("ProcessUploadMatricNumberExcel");
                            }
                        }
                        else
                        {
                            SetMessage("Validation Error! This Application Number" + applicationNumber+ ", " + " Does Not Exist in Admission List. Cross check and re-upload!", Message.Category.Error);
                            KeepDropDownState(matricNumberViewModel);
                            return RedirectToAction("ProcessUploadMatricNumberExcel");
                        }

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

            KeepDropDownState(viewModel);
            return RedirectToAction("UploadManuallyGeneratedMatricNo");
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
        public void KeepDropDownState(MatricNumberViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                if (viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, ID, NAME, viewModel.Department.Id);
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool HasPaidSchoolFees(Payment payment, Session session, decimal AmountToPay)
        {
            long progId = 0;
          
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            EWalletPaymentLogic eWalletPaymentLogic = new EWalletPaymentLogic();
            RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == payment.Id && r.Status.Contains("021:") && r.Description.Contains("SCHOOL FEES"));
            StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault();
            //EWalletPayment eWalletPayment = eWalletPaymentLogic.GetModelsBy(e => e.Fee_Type_Id == 3 && e.Person_Id == payment.Person.Id && e.Session_Id == session.Id).LastOrDefault();


            if (studentLevel != null)
            {
                progId = studentLevel.Programme.Id;
                
            }
            if (remitaPayment != null)
            {
                if (progId == (int)Programmes.HNDPartTime || progId == (int)Programmes.NDPartTime)
                {
                    decimal eightyPercentOfFirstInstallment = AmountToPay * (80M / 100M);
                    if (remitaPayment != null && remitaPayment.TransactionAmount >= eightyPercentOfFirstInstallment)
                        return true;
                    return false;
                }
                else if (progId == (int)Programmes.HNDEvening || progId == (int)Programmes.NDEveningFullTime)
                {
                    decimal fiftyPercentOfFullPayment = AmountToPay * (45M / 100M);
                    if (remitaPayment != null && remitaPayment.TransactionAmount >= fiftyPercentOfFullPayment)
                        return true;
                    return false;
                }
                else
                {
                    if (remitaPayment.TransactionAmount >= AmountToPay)
                        return true;
                    return false;
                }
            }

            //else if(eWalletPayment != null && remitaPayment == null)
            //{
            //    if (progId == (int)Programmes.HNDPartTime || progId == (int)Programmes.NDPartTime)
            //    {
            //        decimal eightyPercentOfFirstInstallment = AmountToPay * (80M / 100M);
            //        if (eWalletPayment != null && eWalletPayment.Amount >= eightyPercentOfFirstInstallment)
            //            return true;
            //        return false;
            //    }
            //    else if (progId == (int)Programmes.HNDEvening || progId == (int)Programmes.NDEveningFullTime)
            //    {
            //        decimal fiftyPercentOfFullPayment = AmountToPay * (45M / 100M);
            //        if (eWalletPayment != null && eWalletPayment.Amount >= fiftyPercentOfFullPayment)
            //            return true;
            //        return false;
            //    }
            //    else
            //    {
            //        if (eWalletPayment.Amount >= AmountToPay)
            //            return true;
            //        return false;
            //    }
            //}
            else
            {
                return false;
            }
        }
    }
}