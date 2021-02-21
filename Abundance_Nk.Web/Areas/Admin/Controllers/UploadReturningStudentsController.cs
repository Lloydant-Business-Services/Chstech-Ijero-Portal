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
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Microsoft.Ajax.Utilities;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class UploadReturningStudentsController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";

        public ActionResult ReturningStudents()
        {
            var viewModel = new UploadReturningStudentViewModel();
            try
            {
                if (TempData["UploadedStudent"] != null)
                {
                    viewModel.UploadedStudents = (List<UploadedStudentModel>)TempData["UploadedStudent"];
                }

                populateDropdowns(viewModel);
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ReturningStudents(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var returningStudentList = new List<ReturningStudents>();

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);

                    string filePath = string.Empty;
                    string fileExt = string.Empty;
                    filePath = savedFileName; //get the path of the file  
                    fileExt = Path.GetExtension(filePath); //get the file extension  

                    //DataSet studentList = ReadExcel(savedFileName);
                    IExcelManager excelManager = new ExcelManager();
                    DataSet studentList = excelManager.ReadExcel(savedFileName);
                    //DataSet studentList = ReadExcel(savedFileName, fileExt);

                    if (studentList != null && studentList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < studentList.Tables[0].Rows.Count; i++)
                        {
                            var returningStudent = new ReturningStudents();
                            
                            returningStudent.Firstname = studentList.Tables[0].Rows[i][1].ToString();
                            returningStudent.Surname = studentList.Tables[0].Rows[i][0].ToString();
                            returningStudent.Othername = studentList.Tables[0].Rows[i][2].ToString();
                            returningStudent.DateOfBirth = studentList.Tables[0].Rows[i][3].ToString();
                            //returningStudent.Address = studentList.Tables[0].Rows[i][4].ToString();
                            returningStudent.State = studentList.Tables[0].Rows[i][4].ToString();
                            //returningStudent.LocalGovernmentArea = studentList.Tables[0].Rows[i][6].ToString();
                            //returningStudent.MobilePhone = studentList.Tables[0].Rows[i][7].ToString();
                            //returningStudent.Email = studentList.Tables[0].Rows[i][8].ToString();
                            returningStudent.Sex = studentList.Tables[0].Rows[i][5].ToString();
                            //returningStudent.Option = studentList.Tables[0].Rows[i][10].ToString();
                            //returningStudent.Programme = studentList.Tables[0].Rows[i][11].ToString();
                            //returningStudent.Department = studentList.Tables[0].Rows[i][12].ToString();
                            returningStudent.MatricNumber = studentList.Tables[0].Rows[i][6].ToString();
                            //returningStudent.Level = studentList.Tables[0].Rows[i][14].ToString();

                            returningStudentList.Add(returningStudent);
                        }
                    }
                }

                viewModel.ReturningStudentList = returningStudentList;
                //keepDropdownState(viewModel);
                ////ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.Session.Id);
                //TempData["UploadReturningStudentViewModel"] = viewModel;
                //return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
            
            keepDropdownState(viewModel);
            //ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.Session.Id);
            TempData["UploadReturningStudentViewModel"] = viewModel;
            return View(viewModel);
        }
        public DataSet ReadExcel(string fileName, string fileExt)
        {
            try
            {
                string conn = string.Empty;
                DataSet dtexcel = new DataSet();
                if (fileExt.CompareTo(".xls") == 0)
                    conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';"; //for below excel 2007  
                else
                    conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes';"; //for above excel 2007  
                using (OleDbConnection con = new OleDbConnection(conn))
                {
                    DataTable sheet = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    DataRow dataRow = sheet.Rows[0];

                    string sheetName = dataRow[2].ToString().Replace("'", "");
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from " + sheetName, con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable 
                }
                return dtexcel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult SaveUpload()
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();

                var viewModel = (UploadReturningStudentViewModel)TempData["UploadReturningStudentViewModel"];

                List<UploadedStudentModel> uploadedStudents = new List<UploadedStudentModel>();
                Department department = null;
                DepartmentOption departmentOption = null;
                Programme programme = null;
                Level level = null;

                Model.Model.Session session = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);
                department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id);
                programme = programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);
                level = levelLogic.GetModelBy(p => p.Level_Id == viewModel.Level.Id);

                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    departmentOption = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == viewModel.DepartmentOption.Id);
                }

                if (viewModel.ReturningStudentList != null && viewModel.ReturningStudentList.Count > 0)
                {
                    for (int i = 0; i < viewModel.ReturningStudentList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Surname))
                        {
                            continue;
                        }

                        viewModel.ReturningStudentList[i].Firstname = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Firstname) ? viewModel.ReturningStudentList[i].Firstname.Trim() : viewModel.ReturningStudentList[i].Surname;

                        if (!string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Surname.Trim()) || !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Firstname.Trim()))
                        {
                            using (var scope = new TransactionScope())
                            {
                                var person = new Person();
                                var student = new Model.Model.Student();
                                var studentLevel = new StudentLevel();

                                person.LastName = viewModel.ReturningStudentList[i].Surname.Trim();
                                person.FirstName = viewModel.ReturningStudentList[i].Firstname.Trim();
                                person.OtherName = viewModel.ReturningStudentList[i].Othername;

                                string dob = viewModel.ReturningStudentList[i].DateOfBirth;
                                int year, month, day;

                                if (string.IsNullOrEmpty(dob))
                                {
                                    person.DateOfBirth = DateTime.Now;
                                }
                                //else if (dob.Contains(" "))
                                //{
                                //    string dobfirstPart = dob.Split(' ').FirstOrDefault();
                                //    year = Convert.ToInt32(dobfirstPart.Split('/')[2]);
                                //    month = Convert.ToInt32(dobfirstPart.Split('/')[1]);
                                //    day = Convert.ToInt32(dobfirstPart.Split('/')[0]);

                                //    if (month > 12)
                                //    {
                                //        int oldMonth = month;
                                //        month = day;
                                //        day = oldMonth;
                                //    }

                                //    person.DateOfBirth = new DateTime(year, month, day);
                                //}
                                //else if(dob.Contains("/"))
                                //{
                                //    day = Convert.ToInt32(dob.Split('/')[0]);
                                //    month = Convert.ToInt32(dob.Split('/')[1]);
                                //    year = Convert.ToInt32(dob.Split('/')[2]);

                                //    day = day <= 31 ? day : 1;
                                //    month = month <= 12 ? month : 1;

                                //    person.DateOfBirth = new DateTime(year, month, day);
                                //}
                                //else if (dob.Contains("-"))
                                //{
                                //    day = Convert.ToInt32(dob.Split('-')[0]);
                                //    month = Convert.ToInt32(dob.Split('-')[1]);
                                //    year = Convert.ToInt32(dob.Split('-')[2]);

                                //    day = day <= 31 ? day : 1;
                                //    month = month <= 12 ? month : 1;

                                //    person.DateOfBirth = new DateTime(year, month, day);
                                //}
                                else
                                {
                                    DateTime myDate;
                                    if (DateTime.TryParse(dob, out myDate))
                                    {
                                        person.DateOfBirth = myDate;
                                    }
                                    else
                                    {
                                        person.DateOfBirth = DateTime.Now;
                                    }
                                }
                                //person.ContactAddress = viewModel.ReturningStudentList[i].Address;
                                //person.MobilePhone = viewModel.ReturningStudentList[i].MobilePhone;
                                //person.Email = viewModel.ReturningStudentList[i].Email;

                                string gender = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Sex) ? "MALE" : viewModel.ReturningStudentList[i].Sex;
                                string state = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].State) ? "EKITI" : viewModel.ReturningStudentList[i].State;
                                string localGovernmentArea = "Ado-Ekiti";

                                person = CreatePerson(person, gender, state, localGovernmentArea);
                                
                                if (person != null && person.Id > 0)
                                {
                                    viewModel.Programme = programme;
                                    ApplicationForm newForm = new ApplicationForm();

                                    if (viewModel.StudentType == "New")
                                    {

                                        int feeTypeId = 1;
                                        int listType = 1;

                                        switch (viewModel.Programme.Id)
                                        {
                                            case 1:
                                                feeTypeId = 1;
                                                listType = 1;
                                                break;
                                            case 2:
                                                feeTypeId = 5;
                                                listType = 3;
                                                break;
                                            case 3:
                                                feeTypeId = 4;
                                                listType = 2;
                                                break;
                                            case 4:
                                                feeTypeId = 6;
                                                listType = 4;
                                                break;
                                        }

                                        Payment payment = CreatePayment(viewModel, person, new PersonType() { Id = (int)PersonTypes.Applicant }, feeTypeId);

                                        if (payment != null && payment.Id > 0)
                                        {
                                            ApplicationFormSettingLogic formSettingLogic = new ApplicationFormSettingLogic();
                                            ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                                            ApplicationFormLogic formLogic = new ApplicationFormLogic();
                                            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                                            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                                            AdmissionListBatchLogic batchLogic = new AdmissionListBatchLogic();

                                            string formNumber = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].MatricNumber) ? null : viewModel.ReturningStudentList[i].MatricNumber;

                                            ApplicationForm existingApplicationForm = null;
                                            if (formNumber != null)
                                            {
                                                existingApplicationForm = formLogic.GetModelsBy(f => f.Application_Form_Number == formNumber).LastOrDefault();
                                            }
                                            
                                            if (existingApplicationForm == null)
                                            {
                                                long maxFormId = formLogic.GetEntitiesBy(f => f.Application_Form_Id > 0).Max(s => s.Application_Form_Id);

                                                //department = GetDepartment(viewModel.ReturningStudentList[i].Department);

                                                if (department == null || viewModel.Programme == null)
                                                {
                                                    continue;
                                                }

                                                if (!string.IsNullOrEmpty(formNumber) && formNumber.Split('/').Length > 0 && formNumber.Split('/').LastOrDefault().Contains("X"))
                                                {
                                                    TempData["Action"] = "The Application Number/Matric Number contains invalid input string, kindly check and try again.";
                                                    return RedirectToAction("ReturningStudents");
                                                }

                                                ApplicationFormSetting formSetting = formSettingLogic.GetModelBy(a => a.Session_Id == viewModel.Session.Id && a.Fee_Type_Id == feeTypeId);
                                                ApplicationProgrammeFee programmeFee = programmeFeeLogic.GetModelBy(a => a.Fee_Type_Id == feeTypeId && a.Programme_Id == viewModel.Programme.Id && a.Session_Id == viewModel.Session.Id);

                                                if (department != null && department.Id > 0 && formSetting != null && programmeFee != null)
                                                {
                                                    ApplicationForm form = new ApplicationForm();
                                                    form.DateSubmitted = DateTime.Now;

                                                    if (formNumber != null)
                                                    {
                                                        int lastFormNumberSplit;
                                                        if (int.TryParse(formNumber.Split('/').LastOrDefault(), out lastFormNumberSplit))
                                                        {
                                                            form.ExamNumber = department.Code + lastFormNumberSplit;
                                                            form.ExamSerialNumber = lastFormNumberSplit;
                                                            form.SerialNumber = lastFormNumberSplit;
                                                        }
                                                        
                                                        form.Number = formNumber;
                                                    }
                                                    else
                                                    {
                                                        form.ExamNumber = null;
                                                        form.ExamSerialNumber = null;
                                                        form.Number = null;
                                                        form.SerialNumber = null;
                                                    }

                                                    form.Payment = payment;
                                                    form.Person = person;
                                                    form.ProgrammeFee = programmeFee;
                                                    form.RejectReason = null;
                                                    form.Rejected = false;
                                                    form.Release = false;
                                                    form.Setting = formSetting;
                                                    form.Id = maxFormId + 1;

                                                    newForm = formLogic.Create(form);
                                                }

                                                if (newForm != null && newForm.Id > 0)
                                                {
                                                    AppliedCourse appliedCourse = new AppliedCourse();
                                                    appliedCourse.ApplicationForm = newForm;
                                                    appliedCourse.Department = department;
                                                    appliedCourse.Option = departmentOption;
                                                    appliedCourse.Person = person;
                                                    appliedCourse.Programme = viewModel.Programme;

                                                    appliedCourseLogic.Create(appliedCourse);

                                                    AdmissionListBatch batch = new AdmissionListBatch();
                                                    batch.DateUploaded = DateTime.Now;
                                                    batch.IUploadedFilePath = "STUDENT UPLOAD";
                                                    batch.Type = new AdmissionListType() {Id = listType};

                                                    AdmissionListBatch newBatch = batchLogic.Create(batch);

                                                    AdmissionList admissionList = new AdmissionList();
                                                    admissionList.Session = viewModel.Session;
                                                    admissionList.Activated = true;
                                                    admissionList.Batch = newBatch;
                                                    admissionList.Deprtment = department;
                                                    admissionList.Form = newForm;
                                                    admissionList.Programme = viewModel.Programme;

                                                    admissionListLogic.Create(admissionList);
                                                }
                                            }
                                        }
                                    }

                                    student.MatricNumber = viewModel.StudentType == "New" ? null : viewModel.ReturningStudentList[i].MatricNumber;
                                    student.ApplicationForm = newForm == null || newForm.Id <= 0 ? null : newForm;
                                    //student.ContactAddress = viewModel.ReturningStudentList[i].Address;

                                    if (student.MatricNumber != null && !MatricNumberIsCorrectFormat(student.MatricNumber))
                                        continue;

                                    student = CreateStudent(student, person, viewModel);
                                }

                                if (student != null && student.Id > 0)
                                {
                                    viewModel.Department = department;
                                    
                                    viewModel.DepartmentOption = departmentOption;
                                    
                                    viewModel.Level = level;
                                    studentLevel = CreateStudentLevel(student, viewModel);
                                }

                                if (person != null && person.Id > 0 && student != null && studentLevel != null && studentLevel.Id > 0)
                                {
                                    UploadedStudentModel uploadedStudent = new UploadedStudentModel();

                                    uploadedStudent.Name = person.LastName + " " + person.FirstName + " " + person.OtherName;
                                    uploadedStudent.MatricNumber = student.MatricNumber;
                                    uploadedStudent.Department = viewModel.Department.Name;
                                    uploadedStudent.Level = viewModel.Level.Name;
                                    uploadedStudent.Programme = viewModel.Programme.Name;
                                    uploadedStudent.Session = session.Name;

                                    uploadedStudents.Add(uploadedStudent);

                                    scope.Complete();
                                }
                            }
                        }
                    }

                    TempData["UploadedStudent"] = uploadedStudents;
                    if (viewModel.ReturningStudentList.Count != uploadedStudents.Count)
                    {
                        TempData["Action"] = "Upload Successful! However, some records were not uploaded, kindly check and retry.";
                    }
                    else
                    {
                        TempData["Action"] = "Upload Successful";
                    }
                }
                else
                {
                    TempData["Action"] = "Data Empty";
                }

                return RedirectToAction("ReturningStudents");
            }
            catch (Exception ex)
            {
                TempData["Action"] = ex.Message;
                return RedirectToAction("ReturningStudents");
            }
        }

        private bool MatricNumberIsCorrectFormat(string matricNumber)
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(matricNumber) && matricNumber.Split('-').LastOrDefault().Count() == 4)
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Incorrect matric number format." + ex.Message);
            }

            return response;
        }

        private Programme GetProgramme(string programmeName)
        {
            Programme programme = null;
            try
            {
                if (!string.IsNullOrEmpty(programmeName))
                {
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();
                    programme = programmeLogic.GetModelsBy(p => p.Programme_Name.Trim().Replace(" ", "") == programmeName.Trim().Replace(" ", "")).LastOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return programme;
        }
        private Department GetDepartment(string departmentName)
        {
            Department department = null;
            try
            {
                if (!string.IsNullOrEmpty(departmentName))
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    department = departmentLogic.GetModelsBy(p => p.Department_Name.Trim().Replace(" ", "") == departmentName.Trim().Replace(" ", "")).LastOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return department;
        }
        private DepartmentOption GetDepartmentOption(string departmentOptionName)
        {
            DepartmentOption departmentOption = null;
            try
            {
                if (!string.IsNullOrEmpty(departmentOptionName))
                {
                    DepartmentOptionLogic optionLogic = new DepartmentOptionLogic();
                    departmentOption = optionLogic.GetModelsBy(p => p.Department_Option_Name.Trim().Replace(" ", "") == departmentOptionName.Trim().Replace(" ", "")).LastOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return departmentOption;
        }
        private Level GetLevel(string levelId)
        {
            Level level = null;
            try
            {
                if (!string.IsNullOrEmpty(levelId))
                {
                    int id;
                    if (int.TryParse(levelId, out id) && id > 0)
                    {
                        LevelLogic levelLogic = new LevelLogic();
                        level = levelLogic.GetModelsBy(p => p.Level_Id == id).LastOrDefault();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return level;
        }
        private StudentLevel CreateStudentLevel(Model.Model.Student student, UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                studentLevel.Student = student;
                if (viewModel != null)
                {
                    studentLevel.Level = viewModel.Level;
                    studentLevel.Programme = viewModel.Programme;
                    studentLevel.Department = viewModel.Department;
                    studentLevel.DepartmentOption = viewModel.DepartmentOption ?? null;
                    studentLevel.Session = viewModel.Session;
                }

                if (student != null && student.Id > 0 && studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault() == null && studentLevel.Level != null 
                    && studentLevel.Programme != null && studentLevel.Department != null && studentLevel.Session != null)
                {
                    studentLevel = studentLevelLogic.Create(studentLevel);
                }

                return studentLevel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Model.Model.Student CreateStudent(Model.Model.Student student, Person person, UploadReturningStudentViewModel viewModel)
        {
            try
            {
                UserLogic userLogic = new UserLogic();
                StudentAuditLogic studentAuditLogic = new StudentAuditLogic();

                string operation = "INSERT";
                string action = "ADDED STUDENT RECORD";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                User user = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);

                StudentAudit studentAudit = new StudentAudit();
                studentAudit.Action = action;
                studentAudit.Client = client;
                studentAudit.Operation = operation;
                studentAudit.Time = DateTime.Now;
                studentAudit.User = user;
                studentAudit.Student = new Model.Model.Student() { Id = person.Id };
                studentAudit.InitialValues = "-";
                studentAudit.CurrentValues = student.MatricNumber ?? "-";

                StudentType studentType = null;
                var studentCategory = new StudentCategory { Id = 2 };
                var studentStatus = new StudentStatus { Id = 1 };
                var personLogic = new PersonLogic();
                Title title = null;
                var studentLogic = new StudentLogic();

                person = personLogic.GetModelBy(p => p.Person_Id == person.Id);
                if (viewModel.Programme != null)
                {
                    if (viewModel.Programme.Id == (int)Programmes.HNDFullTime || viewModel.Programme.Id == (int)Programmes.HNDPartTime)
                    {
                        studentType = new StudentType { Id = 2 };
                        student.Type = studentType;
                    }
                    else
                    {
                        studentType = new StudentType { Id = 1 };
                        student.Type = studentType;
                    }
                }

                //student.Number = Convert.ToInt32(student.MatricNumber.Split('-').LastOrDefault());
                if (person != null)
                {
                    student.Id = person.Id;
                    if (person.Sex != null)
                    {
                        if (person.Sex.Id == 1)
                        {
                            title = new Title { Id = 1 };
                            student.Title = title;
                        }
                        else
                        {
                            title = new Title { Id = 2 };
                            student.Title = title;
                        }
                    }
                }
                
                student.Category = studentCategory;
                student.Status = studentStatus;
                student.Activated = true;

                long? maxStudentNumber = 0;
                long? nextStudentNumber = 0;

                string matricNumber = student.MatricNumber;

                //if (string.IsNullOrEmpty(student.MatricNumber))
                //{
                //    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                //    SessionLogic sessionLogic = new SessionLogic();
                //    StudentAssignedMatricNumberLogic assignedMatricNumberLogic = new StudentAssignedMatricNumberLogic();

                //    Session session = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);

                //    maxStudentNumber = studentLogic.GetEntitiesBy(s => s.Matric_Number != null).Max(s => s.Student_Number);
                //    nextStudentNumber = maxStudentNumber == 0 ? 100758 : maxStudentNumber + 1;

                //    student.Number = nextStudentNumber;
                //    student.MatricNumber = session.Name.Split('/').FirstOrDefault() + "/" + nextStudentNumber + "/REGULAR";

                //    Model.Model.Student existingStudent = studentLogic.GetModelsBy(s => s.Matric_Number == student.MatricNumber).LastOrDefault();

                //    while (existingStudent != null)
                //    {
                //        student.MatricNumber = session.Name.Split('/').FirstOrDefault() + "/" + (nextStudentNumber + 1) + "/REGULAR";
                //        student.Number = nextStudentNumber + 1;

                //        existingStudent = studentLogic.GetModelsBy(s => s.Matric_Number == student.MatricNumber).LastOrDefault();
                //    }

                //    matricNumber = student.MatricNumber;

                //    StudentAssignedMatricNumber assignedMatricNumber = new StudentAssignedMatricNumber();
                //    assignedMatricNumber.Person = person;
                //    assignedMatricNumber.Programme = viewModel.Programme;
                //    assignedMatricNumber.Session = session;
                //    assignedMatricNumber.StudentMatricNumber = student.MatricNumber;
                //    assignedMatricNumber.StudentNumber = student.Number ?? 0;

                //    if (assignedMatricNumberLogic.GetModelBy(s => s.Person_Id == person.Id) == null)
                //    {
                //        assignedMatricNumberLogic.Create(assignedMatricNumber);
                //    }
                //}
                if(string.IsNullOrEmpty(matricNumber))
                {
                    student = studentLogic.Create(student);
                    studentAuditLogic.Create(studentAudit);
                }
                else if (studentType != null && studentLogic.GetModelsBy(s => s.Matric_Number == matricNumber).LastOrDefault() == null)
                {
                    student = studentLogic.Create(student);
                    studentAuditLogic.Create(studentAudit);
                }
                else
                {
                    student = null;
                }
                student = student ?? new Model.Model.Student() { MatricNumber = matricNumber };

                return student;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Person CreatePerson(Person person, string gender, string stateValue, string localGovernmentArea)
        {
            try
            {
                Sex sex = null;
                State state = null;
                LocalGovernment localGovernment = null;
                var nationality = new Nationality { Id = 1 };
                var personType = new PersonType { Id = 3 };
                var role = new Role { Id = 5 };
                var localGovernmentLogic = new LocalGovernmentLogic();
                var personLogic = new PersonLogic();
                var stateLogic = new StateLogic();
                if (gender.Trim().ToUpper() == "MALE")
                {
                    sex = new Sex { Id = 1 };
                    person.Sex = sex;
                }
                else if (gender.Trim().ToUpper() == "FEMALE")
                {
                    sex = new Sex { Id = 2 };
                    person.Sex = sex;
                }
                else if (gender.Trim().ToUpper() == "F")
                {
                    sex = new Sex { Id = 2 };
                    person.Sex = sex;
                }
                else if (gender.Trim().ToUpper() == "M")
                {
                    sex = new Sex { Id = 1 };
                    person.Sex = sex;
                }
                else
                {
                    sex = new Sex { Id = 1 };
                    person.Sex = sex;
                }
                state = stateLogic.GetModelBy(p => p.State_Name == stateValue.Trim());
                if (state != null)
                {
                    person.State = state;
                }
                else
                {
                    state = new State { Id = "ET" };
                    person.State = state;
                }
                localGovernment = localGovernmentLogic.GetModelsBy(p => p.Local_Government_Name == localGovernmentArea.Trim()).LastOrDefault();
                if (localGovernment != null)
                {
                    person.LocalGovernment = localGovernment;
                }
                else
                {
                    localGovernment = new LocalGovernment { Id = 265 };
                    person.LocalGovernment = localGovernment;
                }
                person.Nationality = nationality;
                person.DateEntered = DateTime.Now;
                person.Role = role;
                person.Type = personType;
                person = personLogic.Create(person);
                return person;
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

                var programme = new Programme { Id = Convert.ToInt32(id) };
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
                //Extended Properties = "Excel 12.0;HDR=YES";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataRow dataRow = sheet.Rows[0];
                string sheetName = dataRow[2].ToString().Replace("'", "");
                var command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                // Create DbDataReader to Data Worksheet

                var MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                var ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                Result = ds;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        private void populateDropdowns(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.LevelId = viewModel.LevelSelectListItem;
                ViewBag.SessionId = viewModel.SessionSelectListItem;
                ViewBag.SemesterId = viewModel.SemesterSelectListItem;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void keepDropdownState(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var programme = new Programme();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var department = new Department();
                var level = new Level();
                var levelLogic = new LevelLogic();
                var session = new Session();
                var sessionLogic = new SessionLogic();
                if (viewModel.Programme != null && viewModel.Programme.Id > 0)
                {
                    List<Department> departmentList = departmentLogic.GetBy(viewModel.Programme);
                    ViewBag.DepartmentId = new SelectList(departmentList, ID, NAME, viewModel.Department.Id);
                    ViewBag.ProgrammeId = new SelectList(programmeLogic.GetAll(), ID, NAME, viewModel.Programme.Id);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                    ViewBag.ProgrammeId = new SelectList(programmeLogic.GetAll(), ID, NAME);
                }
                if (viewModel.Level != null && viewModel.Level.Id > 0)
                {
                    ViewBag.LevelId = new SelectList(levelLogic.GetAll(), ID, NAME, viewModel.Level.Id);
                }
                else
                {
                    ViewBag.LevelId = new SelectList(levelLogic.GetAll(), ID, NAME);
                }
                if (viewModel.Session != null && viewModel.Session.Id > 0)
                {
                    ViewBag.SessionId = new SelectList(sessionLogic.GetAll(), ID, NAME, viewModel.Session.Id);
                }
                else
                {
                    ViewBag.SessionId = new SelectList(sessionLogic.GetAll(), ID, NAME);
                }
                if (viewModel.Semester != null && viewModel.Semester.Id > 0)
                    ViewBag.SemesterId = new SelectList(viewModel.SemesterSelectListItem, "Value", "Text", viewModel.Semester.Id);
                else
                    ViewBag.SemesterId = new SelectList(viewModel.SemesterSelectListItem, "Value", "Text");

                ViewBag.DepartmentOptionId = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult SampleSheet()
        {
            try
            {
                GridView gv = new GridView();
                List<SampleReturningStudent> sample = new List<SampleReturningStudent>();
                sample.Add(new SampleReturningStudent() { Surname = "Adekunle", Firstname = "Chukwuma", Othernames = "Ciroma", DateOfBirth = "16/05/1998", State = "Ekiti", 
                                                            Sex = "MALE", MatricNumberOrApplicationNumber = "FPA/GC/XX/X-XXXX"});
                string filename = "Sample Student Upload";
                IExcelServiceManager excelServiceManager = new ExcelServiceManager();
                MemoryStream ms = excelServiceManager.DownloadExcel(sample);
                ms.WriteTo(System.Web.HttpContext.Current.Response.OutputStream);
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xlsx");
                System.Web.HttpContext.Current.Response.StatusCode = 200;
                System.Web.HttpContext.Current.Response.End();

                //gv.DataSource = sample;
                //gv.DataBind();
                //string filename = "Sample Returning Student Upload";
                //return new DownloadFileAction(gv, filename + ".xls");
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                return RedirectToAction("ReturningStudents");
            }

            return RedirectToAction("ReturningStudents");
        }
        public Payment CreatePayment(UploadReturningStudentViewModel viewModel, Person person, PersonType personType, int feeTypeId)
        {
            try
            {
                PaymentMode paymentMode = new PaymentMode { Id = 1 };
                PaymentType paymentType = new PaymentType { Id = 2 };
                FeeType feeType = new FeeType { Id = feeTypeId };
                Session session = new Session { Id = viewModel.Session.Id };
                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                payment.Person = person;
                payment.PaymentMode = paymentMode;
                payment.PaymentType = paymentType;
                payment.PersonType = personType;
                payment.FeeType = feeType;
                payment.Session = session;
                payment.DatePaid = DateTime.Now;
                payment = paymentLogic.Create(payment);
                OnlinePayment newOnlinePayment = null;
                if (payment != null)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = payment;

                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }


                return payment;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public JsonResult GetDepartmentOption(int departmentId, int programmeId)
        {
            List<DepartmentOption> departmentOptions = new List<DepartmentOption>();

            try
            {
                DepartmentOptionLogic departmentLogic = new DepartmentOptionLogic();
                departmentOptions = departmentLogic.GetDeptOption(departmentId, programmeId);
                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid, int levelId)
        {
            try
            {
                List<DepartmentOption> departmentOptions = new List<DepartmentOption>();
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                int programmeId = Convert.ToInt32(programmeid);

                int[] optionLevels = { (int)Levels.HNDI, (int)Levels.HNDII, (int)Levels.HNDYRI, (int)Levels.HNDYRII, (int)Levels.HNDYRIII };

                if (!optionLevels.Contains(levelId))
                {
                    return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
                }

                int[] optionProgrammes = { (int)Programmes.HNDFullTime, (int)Programmes.HNDPartTime };

                if (!optionProgrammes.Contains(programmeId))
                {
                    return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
                }

                Department department = new Department() { Id = Convert.ToInt32(id) };
                Programme programme = new Programme() { Id = programmeId };
                DepartmentOptionLogic departmentLogic = new DepartmentOptionLogic();
                departmentOptions = departmentLogic.GetBy(department, programme);


                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);

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
        public ActionResult ReturningStudentsWithDetails()
        {
            var viewModel = new UploadReturningStudentViewModel();
            try
            {
                if (TempData["UploadedStudent"] != null)
                {
                    viewModel.UploadedStudents = (List<UploadedStudentModel>)TempData["UploadedStudent"];
                }

                if (TempData["FailedUploads"] != null)
                {
                    viewModel.FailedUploads = (List<UploadedStudentModel>)TempData["FailedUploads"];
                }

                populateDropdowns(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ReturningStudentsWithDetails(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var returningStudentList = new List<ReturningStudents>();

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);

                    string filePath = string.Empty;
                    string fileExt = string.Empty;
                    filePath = savedFileName; //get the path of the file  
                    fileExt = Path.GetExtension(filePath); //get the file extension  

                    IExcelManager excelManager = new ExcelManager();
                    DataSet studentList = excelManager.ReadExcel(savedFileName);

                    if (studentList != null && studentList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < studentList.Tables[0].Rows.Count; i++)
                        {
                            var returningStudent = new ReturningStudents();

                            returningStudent.Surname = studentList.Tables[0].Rows[i][1].ToString();
                            returningStudent.Firstname = studentList.Tables[0].Rows[i][2].ToString();
                            returningStudent.Othername = studentList.Tables[0].Rows[i][3].ToString();
                            returningStudent.DateOfBirth = studentList.Tables[0].Rows[i][4].ToString();
                            returningStudent.Address = studentList.Tables[0].Rows[i][5].ToString();
                            returningStudent.State = studentList.Tables[0].Rows[i][6].ToString();
                            returningStudent.LocalGovernmentArea = studentList.Tables[0].Rows[i][7].ToString();
                            returningStudent.MobilePhone = studentList.Tables[0].Rows[i][8].ToString();
                            returningStudent.Email = studentList.Tables[0].Rows[i][9].ToString();
                            returningStudent.NexOfKinName = studentList.Tables[0].Rows[i][10].ToString();
                            returningStudent.NexOfKinPhoneNumber = studentList.Tables[0].Rows[i][11].ToString();
                            returningStudent.Country = studentList.Tables[0].Rows[i][12].ToString();
                            returningStudent.MaritalStatus = studentList.Tables[0].Rows[i][13].ToString();
                            returningStudent.Sex = studentList.Tables[0].Rows[i][14].ToString();
                            returningStudent.MatricNumber = studentList.Tables[0].Rows[i][15].ToString();

                            returningStudentList.Add(returningStudent);
                        }
                    }
                }

                viewModel.ReturningStudentList = returningStudentList;
                //keepDropdownState(viewModel);
                ////ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.Session.Id);
                //TempData["UploadReturningStudentViewModel"] = viewModel;
                //return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            keepDropdownState(viewModel);
            //ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.Session.Id);
            TempData["UploadReturningStudentViewModel"] = viewModel;
            return View(viewModel);
        }
        public ActionResult SaveUploadStudentWithDetails()
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();

                var viewModel = (UploadReturningStudentViewModel)TempData["UploadReturningStudentViewModel"];

                List<UploadedStudentModel> uploadedStudents = new List<UploadedStudentModel>();
                List<UploadedStudentModel> failedUploads = new List<UploadedStudentModel>();
                Department department = null;
                DepartmentOption departmentOption = null;
                Programme programme = null;
                Level level = null;

                Model.Model.Session session = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);
                department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id);
                programme = programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);
                level = levelLogic.GetModelBy(p => p.Level_Id == viewModel.Level.Id);

                if (viewModel.DepartmentOption != null && viewModel.DepartmentOption.Id > 0)
                {
                    departmentOption = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == viewModel.DepartmentOption.Id);
                }

                if (viewModel.ReturningStudentList != null && viewModel.ReturningStudentList.Count > 0)
                {
                    for (int i = 0; i < viewModel.ReturningStudentList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Surname) || string.IsNullOrEmpty(viewModel.ReturningStudentList[i].MatricNumber))
                        {
                            UploadedStudentModel uploadedStudent = new UploadedStudentModel();

                            uploadedStudent.Name = viewModel.ReturningStudentList[i].Surname + " " + viewModel.ReturningStudentList[i].Firstname + " " + viewModel.ReturningStudentList[i].Othername;
                            uploadedStudent.MatricNumber = viewModel.ReturningStudentList[i].MatricNumber;
                            uploadedStudent.Department = viewModel.Department.Name;
                            uploadedStudent.Level = viewModel.Level.Name;
                            uploadedStudent.Programme = viewModel.Programme.Name;
                            uploadedStudent.Session = session.Name;
                            uploadedStudent.Reason = "No Surname and/or Matric Number.";

                            failedUploads.Add(uploadedStudent);

                            continue;
                        }

                        StudentLogic studentLogic = new StudentLogic();
                        string matricNumber = viewModel.ReturningStudentList[i].MatricNumber.Trim();
                        Model.Model.Student existingStudent = studentLogic.GetModelsBy(s => s.Matric_Number.Trim() == matricNumber).LastOrDefault();
                        if (existingStudent != null)
                        {
                            UploadedStudentModel uploadedStudent = new UploadedStudentModel();

                            uploadedStudent.Name = viewModel.ReturningStudentList[i].Surname + " " + viewModel.ReturningStudentList[i].Firstname + " " + viewModel.ReturningStudentList[i].Othername;
                            uploadedStudent.MatricNumber = viewModel.ReturningStudentList[i].MatricNumber;
                            uploadedStudent.Department = viewModel.Department.Name;
                            uploadedStudent.Level = viewModel.Level.Name;
                            uploadedStudent.Programme = viewModel.Programme.Name;
                            uploadedStudent.Session = session.Name;
                            uploadedStudent.Reason = "Record exist.";

                            failedUploads.Add(uploadedStudent);

                            continue;
                        }

                        viewModel.ReturningStudentList[i].Firstname = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Firstname) ? viewModel.ReturningStudentList[i].Firstname.Trim() : viewModel.ReturningStudentList[i].Surname;

                        if (!string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Surname.Trim()) || !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Firstname.Trim()))
                        {
                            using (var scope = new TransactionScope())
                            {
                                var person = new Person();
                                var student = new Model.Model.Student();
                                var studentLevel = new StudentLevel();

                                person.LastName = viewModel.ReturningStudentList[i].Surname.Trim();
                                person.FirstName = viewModel.ReturningStudentList[i].Firstname.Trim();
                                person.OtherName = viewModel.ReturningStudentList[i].Othername;

                                DateTime dob = new DateTime();
                                if (DateTime.TryParse(viewModel.ReturningStudentList[i].DateOfBirth, out dob))
                                {
                                    person.DateOfBirth = dob;
                                }
                                else
                                {
                                    person.DateOfBirth = DateTime.Now;
                                }

                                person.ContactAddress = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Address) ? viewModel.ReturningStudentList[i].Address.Trim() : null;
                                person.MobilePhone = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].MobilePhone) ? viewModel.ReturningStudentList[i].MobilePhone.Trim() : null;
                                person.Email = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Email) ? viewModel.ReturningStudentList[i].Email.Trim() : null;

                                string gender = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Sex) ? "MALE" : viewModel.ReturningStudentList[i].Sex.Trim();
                                string state = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].State) ? "EKITI" : viewModel.ReturningStudentList[i].State.Trim();
                                string localGovernmentArea = string.IsNullOrEmpty(viewModel.ReturningStudentList[i].LocalGovernmentArea) ? "Ado-Ekiti" : viewModel.ReturningStudentList[i].LocalGovernmentArea.Trim();
                                //string localGovernmentArea = "Ado-Ekiti";

                                person = CreatePerson(person, gender, state, localGovernmentArea);

                                if (person != null && person.Id > 0)
                                {
                                    if (!string.IsNullOrEmpty(viewModel.ReturningStudentList[i].NexOfKinName) && !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].NexOfKinPhoneNumber))
                                    {
                                        CreateNextOfKin(person.Id, viewModel.ReturningStudentList[i].NexOfKinName.Trim(), viewModel.ReturningStudentList[i].NexOfKinPhoneNumber.Trim());
                                    }


                                    viewModel.Programme = programme;
                                    
                                    student.MatricNumber = viewModel.ReturningStudentList[i].MatricNumber.Trim();
                                    student.ContactAddress = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Address) ? viewModel.ReturningStudentList[i].Address.Trim() : null;
                                    string maritalStatusVal = !string.IsNullOrEmpty(viewModel.ReturningStudentList[i].Address) ? viewModel.ReturningStudentList[i].Address.Trim() : null;
                                    MaritalStatus maritalStatus = null;
                                    if (maritalStatusVal != null)
                                    {
                                        MaritalStatusLogic maritalStatusLogic = new MaritalStatusLogic();
                                        MaritalStatus existingMaritalStatus = maritalStatusLogic.GetModelsBy(s => s.Marital_Status_Name.Trim() == maritalStatusVal).LastOrDefault();

                                        maritalStatus = existingMaritalStatus ?? new MaritalStatus(){ Id = 1};
                                    }
                                    else
                                    {
                                        maritalStatus = new MaritalStatus(){ Id = 1};
                                    }

                                    student.MaritalStatus = maritalStatus;
                                    student = CreateStudent(student, person, viewModel);
                                }

                                if (student != null && student.Id > 0)
                                {
                                    viewModel.Department = department;

                                    viewModel.DepartmentOption = departmentOption;

                                    viewModel.Level = level;
                                    studentLevel = CreateStudentLevel(student, viewModel);
                                }

                                if (person != null && person.Id > 0 && student != null && studentLevel != null && studentLevel.Id > 0)
                                {
                                    UploadedStudentModel uploadedStudent = new UploadedStudentModel();

                                    uploadedStudent.Name = person.LastName + " " + person.FirstName + " " + person.OtherName;
                                    uploadedStudent.MatricNumber = student.MatricNumber;
                                    uploadedStudent.Department = viewModel.Department.Name;
                                    uploadedStudent.Level = viewModel.Level.Name;
                                    uploadedStudent.Programme = viewModel.Programme.Name;
                                    uploadedStudent.Session = session.Name;

                                    uploadedStudents.Add(uploadedStudent);

                                    scope.Complete();
                                }
                            }
                        }
                    }

                    TempData["UploadedStudent"] = uploadedStudents;
                    TempData["FailedUploads"] = failedUploads;
                    if (viewModel.ReturningStudentList.Count != uploadedStudents.Count)
                    {
                        SetMessage("Upload Successful! However, some records were not uploaded, kindly check and retry.", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Upload Successful", Message.Category.Information);
                    }
                }
                else
                {
                    SetMessage("Data Empty", Message.Category.Error);
                }

                return RedirectToAction("ReturningStudentsWithDetails");
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
                return RedirectToAction("ReturningStudentsWithDetails");
            }
        }

        private void CreateNextOfKin(long personId, string nexOfKinName, string nexOfKinPhoneNumber)
        {
            try
            {
                NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                NextOfKin existingNextOfKin = nextOfKinLogic.GetModelsBy(n => n.Person_Id == personId).LastOrDefault();
                if (existingNextOfKin == null)
                {
                    existingNextOfKin = new NextOfKin();
                    existingNextOfKin.Person = new Person(){ Id = personId};
                    existingNextOfKin.ContactAddress = "Ado";
                    existingNextOfKin.MobilePhone = nexOfKinPhoneNumber;
                    existingNextOfKin.Name = nexOfKinName;
                    existingNextOfKin.PersonType = new PersonType(){ Id = 3};
                    existingNextOfKin.Relationship = new Relationship(){ Id = 1};

                    nextOfKinLogic.Create(existingNextOfKin);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SampleStudentUploadWithDetails()
        {
            try
            {
                GridView gv = new GridView();
                List<SampleReturningStudentWithDetails> sample = new List<SampleReturningStudentWithDetails>();
                sample.Add(new SampleReturningStudentWithDetails()
                {
                    SN = "1",
                    Lastname = "Adekunle",
                    Firstname = "Chukwuma",
                    Othernames = "Ciroma",
                    DateOfBirth = "16/05/1998",
                    Address = "Adopoly",
                    State = "Ekiti",
                    LocalGovernmentArea = "Ado-Ekiti",
                    PhoneNumber = "07055555555",
                    Email = "support@adopoly.com",
                    NextOfKin = "Adekunle",
                    NextOfKinPhoneNumber = "07055555555",
                    Country = "Nigeria",
                    MaritalStatus = "Single",
                    Sex = "MALE",
                    MatricNumber = "FPA/GC/XX/X-XXXX"
                });

                string filename = "Sample Student Upload";
                IExcelServiceManager excelServiceManager = new ExcelServiceManager();
                MemoryStream ms = excelServiceManager.DownloadExcel(sample);
                ms.WriteTo(System.Web.HttpContext.Current.Response.OutputStream);
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xlsx");
                System.Web.HttpContext.Current.Response.StatusCode = 200;
                System.Web.HttpContext.Current.Response.End();

                //gv.DataSource = sample;
                //gv.DataBind();
                //string filename = "Sample Returning Student Upload";
                //return new DownloadFileAction(gv, filename + ".xls");
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                return RedirectToAction("ReturningStudentsWithDetails");
            }

            return RedirectToAction("ReturningStudentsWithDetails");
        }
        public ActionResult SampleSpillOver()
        {
            try
            {
                GridView gv = new GridView();
                List<SampleSpillOver> sample = new List<SampleSpillOver>();
                sample.Add(new SampleSpillOver
                {
                    SN = "1",
                    Name = "Adekunle Adeleke",
                    MatricNumber = "FPA/GC/XX/X-XXXX"
                });

                string filename = "Sample Spillover Upload";
                IExcelServiceManager excelServiceManager = new ExcelServiceManager();
                MemoryStream ms = excelServiceManager.DownloadExcel(sample);
                ms.WriteTo(System.Web.HttpContext.Current.Response.OutputStream);
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xlsx");
                System.Web.HttpContext.Current.Response.StatusCode = 200;
                System.Web.HttpContext.Current.Response.End();

            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
                return RedirectToAction("SpillOverStudent");
            }

            return RedirectToAction("SpillOverStudent");
        }
        public ActionResult SpillOverStudent()
        {
            var viewModel = new UploadReturningStudentViewModel();
            try
            {
                if (TempData["UploadedStudent"] != null)
                {
                    viewModel.UploadedStudents = (List<UploadedStudentModel>)TempData["UploadedStudent"];
                }

                populateDropdowns(viewModel);
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult SpillOverStudent(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var returningStudentList = new List<ReturningStudents>();

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);

                    string filePath = string.Empty;
                    string fileExt = string.Empty;
                    filePath = savedFileName; //get the path of the file  
                    fileExt = Path.GetExtension(filePath); //get the file extension  

                    //DataSet studentList = ReadExcel(savedFileName);
                    IExcelManager excelManager = new ExcelManager();
                    DataSet studentList = excelManager.ReadExcel(savedFileName);
                    //DataSet studentList = ReadExcel(savedFileName, fileExt);

                    if (studentList != null && studentList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < studentList.Tables[0].Rows.Count; i++)
                        {
                            var returningStudent = new ReturningStudents();

                            returningStudent.Name = studentList.Tables[0].Rows[i][1].ToString();
                            returningStudent.MatricNumber = studentList.Tables[0].Rows[i][2].ToString();

                            returningStudentList.Add(returningStudent);
                        }
                    }
                }

                viewModel.ReturningStudentList = returningStudentList;
            }
            catch (Exception)
            {
                throw;
            }

            keepDropdownState(viewModel);
            TempData["UploadReturningStudentViewModel"] = viewModel;
            return View(viewModel);
        }
        public ActionResult SaveSpillOver()
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                SpilloverStudentLogic spilloverStudentLogic = new SpilloverStudentLogic();
                UserLogic userLogic = new UserLogic();

                var viewModel = (UploadReturningStudentViewModel)TempData["UploadReturningStudentViewModel"];

                List<UploadedStudentModel> unsuccessful = new List<UploadedStudentModel>();
                
                User user = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);

                if (viewModel.ReturningStudentList != null && viewModel.ReturningStudentList.Count > 0)
                {
                    for (int i = 0; i < viewModel.ReturningStudentList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(viewModel.ReturningStudentList[i].MatricNumber))
                        {
                            continue;
                        }

                        var matricNumber = viewModel.ReturningStudentList[i].MatricNumber.Trim();

                        Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == matricNumber).LastOrDefault();

                        if (student != null)
                        {
                            SpilloverStudent spilloverStudent = new SpilloverStudent();
                            spilloverStudent.Semester = new Semester{ Id = viewModel.Semester.Id };
                            spilloverStudent.Session = new Session{ Id = viewModel.Session.Id };
                            spilloverStudent.Student = student;
                            spilloverStudent.UploadedBy = user;

                            SpilloverStudent existingSpilloverStudent = spilloverStudentLogic.GetModelsBy(s => s.Session_Id == viewModel.Session.Id && s.Semester_Id == viewModel.Semester.Id &&
                                                                        s.Student_Id == student.Id).LastOrDefault();

                            SpilloverStudent createdSpilloverStudent = existingSpilloverStudent == null ? spilloverStudentLogic.Create(spilloverStudent) : null;
                        }
                        else
                            unsuccessful.Add(new UploadedStudentModel{ MatricNumber = matricNumber, Name = viewModel.ReturningStudentList[i].MatricNumber});
                    }

                    TempData["Unsuccessful"] = unsuccessful;
                    if (viewModel.ReturningStudentList.Count != unsuccessful.Count)
                    {
                        TempData["Action"] = "Upload Successful! However, some records were not uploaded, kindly check and retry.";
                    }
                    else
                    {
                        TempData["Action"] = "Upload Successful";
                    }
                }
                else
                {
                    TempData["Action"] = "Data Empty";
                }

                return RedirectToAction("SpillOverStudent");
            }
            catch (Exception ex)
            {
                TempData["Action"] = ex.Message;
                return RedirectToAction("SpillOverStudent");
            }
        }
    }
}