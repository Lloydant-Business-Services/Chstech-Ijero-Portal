using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.IO;
using System.Data.OleDb;
using Abundance_Nk.Model.Entity.Model;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Views
{
    public class UploadAdmissionController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private UploadAdmissionViewModel viewmodel;
        //
        // GET: /Admin/UploadAdmission/
        public ActionResult UploadAdmission()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.SessionSelectListItem = Utility.PopulateAdmissionSessionSelectListItem();
                viewmodel.AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();

                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
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

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadAdmission(UploadAdmissionViewModel vmodel)
        {
            try
            {
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();

                if (vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme != null && (vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id == (int)Programmes.HNDFullTime ||
                     vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id == (int)Programmes.HNDEvening || vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id == (int)Programmes.HNDPartTime))
                {
                    DepartmentOption departmentOption = departmentOptionLogic.GetModelsBy(d => d.Department_Id == vmodel.AdmissionListDetail.Deprtment.Id && d.Activated).LastOrDefault();
                    if (departmentOption != null && (vmodel.AdmissionListDetail.DepartmentOption == null || vmodel.AdmissionListDetail.DepartmentOption.Id <= 0))
                    {
                        SetMessage("Please select department option!", Message.Category.Error);
                        KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
                        return View(vmodel);
                    }
                }
                
                KeepApplicationFormInvoiceGenerationDropDownState(vmodel);

                List<AppliedCourse> applicants = new List<AppliedCourse>();
                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = "";
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        FileInfo fileInfo = new FileInfo(hpf.FileName);
                        string fileExtension = fileInfo.Extension;
                        string newFile = "Admission" + "__";
                        string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                        savedFileName = Path.Combine(pathForSaving,newFileName);
                        hpf.SaveAs(savedFileName);
                    }

                    IExcelManager excelManager = new ExcelManager();
                    DataSet dsAdmissionList = excelManager.ReadExcel(savedFileName);

                    if (dsAdmissionList != null && dsAdmissionList.Tables[0].Rows.Count > 0)
                    {
                        string Application_Number = "";

                        AppliedCourse appliedCourse = new AppliedCourse();
                        ApplicantJambDetail applicantJambDetail = new ApplicantJambDetail();
                        ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
                        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();

                        for (int i = 0; i < dsAdmissionList.Tables[0].Rows.Count; i++)
                        {
                            Application_Number = dsAdmissionList.Tables[0].Rows[i][1].ToString();

                            appliedCourse = appliedCourseLogic.GetModelsBy(m => m.APPLICATION_FORM.Application_Form_Number == Application_Number).LastOrDefault();
                            if (appliedCourse == null)
                            {
                                applicantJambDetail = applicantJambDetailLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number == Application_Number && a.Application_Form_Id != null).LastOrDefault();
                                if (applicantJambDetail != null)
                                {
                                    if (applicantJambDetail.ApplicationForm != null)
                                    {
                                        appliedCourse = appliedCourseLogic.GetModelsBy(a => a.Application_Form_Id == applicantJambDetail.ApplicationForm.Id).LastOrDefault();   
                                    }
                                }
                            }

                            if (appliedCourse != null)
                            {
                                applicants.Add(appliedCourse);
                            }
                        }

                        vmodel.AppliedCourseList = applicants;
                        TempData["UploadAdmissionViewModel"] = vmodel;

                        return View(vmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
            return View(vmodel);
        }

        [HttpPost]
        public ActionResult SaveAdmissionList(UploadAdmissionViewModel vmodel)
        {
            try
            {
                string operation = "INSERT";
                string action = "UPLOADING OF ADMISSION LIST";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                vmodel = (UploadAdmissionViewModel)TempData["UploadAdmissionViewModel"];
                if (vmodel.AppliedCourseList != null && vmodel.AppliedCourseList.Count > 0)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        AdmissionListBatch batch = new AdmissionListBatch();
                        AdmissionListBatchLogic batchLogic = new AdmissionListBatchLogic();
                        AdmissionListType AdmissionType = new AdmissionListType();
                        AdmissionListAudit AdmissionListAudit = new Model.Model.AdmissionListAudit();
                        UserLogic loggeduser = new UserLogic();
                        AdmissionType = vmodel.AdmissionListType;
                        batch.DateUploaded = DateTime.Now;
                        batch.IUploadedFilePath = "NAN";
                        batch.Type = AdmissionType;
                        batch = batchLogic.Create(batch);

                        AdmissionListAudit.Action = action;
                        AdmissionListAudit.Client = client;
                        AdmissionListAudit.Operation = operation;
                        AdmissionListAudit.Time = DateTime.Now;
                        AdmissionListAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                        for (int i = 0; i < vmodel.AppliedCourseList.Count; i++)
                        {
                            AdmissionList admissionlist = new AdmissionList();
                            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                            admissionlist.Form = vmodel.AppliedCourseList[i].ApplicationForm;
                            admissionlist.Deprtment = vmodel.AdmissionListDetail.Deprtment;
                            admissionlist.Programme = vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme;
                            admissionlist.Session = vmodel.CurrentSession;
                            if (vmodel.AdmissionListDetail.DepartmentOption != null && vmodel.AdmissionListDetail.DepartmentOption.Id > 0)
                            {
                                admissionlist.DepartmentOption = vmodel.AdmissionListDetail.DepartmentOption;
                            }

                            admissionlist.Activated = true;
                            if (!admissionListLogic.IsAdmitted(admissionlist.Form))
                            {
                                admissionListLogic.Create(admissionlist, batch, AdmissionListAudit);
                            }
                        }
                        transaction.Complete();
                    }

                    SetMessage("List was uploaded successfully", Message.Category.Information);
                    return RedirectToAction("UploadAdmission");
                }
            }
            catch (Exception ex)
            {  
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            
            KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
            return View("UploadAdmission");
        }

        public ActionResult ViewAdmission()
        {

            try
            {

                viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.SessionSelectListItem = Utility.PopulateAdmissionSessionSelectListItem();
                viewmodel.AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }
       
        [HttpPost]
        public ActionResult ViewAdmission(UploadAdmissionViewModel vmodel)
        {

            try
            {
                KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
                if (vmodel.AdmissionListDetail.Deprtment != null && vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme != null && vmodel.CurrentSession != null && vmodel.AdmissionListType != null)
                {
                    List<AdmissionList> list = new List<AdmissionList>();
                    AdmissionListLogic ListLogic = new AdmissionListLogic();
                    list = ListLogic.GetModelsBy(a => a.Department_Id == vmodel.AdmissionListDetail.Deprtment.Id && a.APPLICATION_FORM.APPLICATION_PROGRAMME_FEE.PROGRAMME.Programme_Id == vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id && a.APPLICATION_FORM.APPLICATION_FORM_SETTING.SESSION.Session_Id == vmodel.CurrentSession.Id && a.ADMISSION_LIST_BATCH.ADMISSION_LIST_TYPE.Admission_List_Type_Id == vmodel.AdmissionListType.Id);
                    if (list != null)
                    {
                        vmodel.AdmissionList = list;
                        return View(vmodel);
                    }
                }
               
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
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
                case ".xls":
                    return false;
                case ".xlsx":
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

        public static DataSet ReadExcelFile(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection connection = new OleDbConnection(xConnStr);
                OleDbCommand command = new OleDbCommand("Select * FROM [Sheet1$]", connection);
                connection.Open();
                // Create DbDataReader to Data Worksheet

                OleDbDataAdapter MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                DataSet ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                Result = ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        private void KeepApplicationFormInvoiceGenerationDropDownState(UploadAdmissionViewModel viewModel)
        {
            try
            {
                if (viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id != null && viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme);
                    viewModel.DepartmentOpionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.AdmissionListDetail.Deprtment,viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme);
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem, VALUE, TEXT, viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id);
                    ViewBag.SessionId = Utility.PopulateAdmissionSessionSelectListItem();
                    ViewBag.AdmissionListTypeId = Utility.PopulateAdmissionListTypeSelectListItem();

                    if (viewModel.AdmissionListDetail.Deprtment != null && viewModel.AdmissionListDetail.Deprtment.Id > 0)
                    {

                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem, VALUE, TEXT, viewModel.AdmissionListDetail.Deprtment.Id);
                        ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOpionSelectListItem, VALUE, TEXT);


                    }
                    else
                    {
                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem, VALUE, TEXT);
                        ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOpionSelectListItem, VALUE, TEXT);
                    }
                }
                else
                {
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem, VALUE, TEXT);
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                    ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
                    ViewBag.SessionId = new SelectList(Utility.PopulateAdmissionSessionSelectListItem(), VALUE, TEXT);
                    ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AllowAnonymous]
        public ActionResult SearchAdmittedStudents()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SearchAdmittedStudents(UploadAdmissionViewModel vModel)
        {
            try
            {
               
                List<AdmissionList> admissionList = new List<AdmissionList>();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                if (vModel.SearchString != null)
                {
                    string search = vModel.SearchString.ToString();
                    //admissionList = admissionListLogic.GetModelsBy(p => p.APPLICATION_FORM.Application_Exam_Number.Contains(search) || p.APPLICATION_FORM.Application_Form_Number.Contains(search));
                    admissionList = admissionListLogic.GetModelsBy(p => p.APPLICATION_FORM.Application_Exam_Number.Equals(search, StringComparison.OrdinalIgnoreCase) || p.APPLICATION_FORM.Application_Form_Number.Equals(search, StringComparison.OrdinalIgnoreCase));
                    if (admissionList.Count > 0)
                    {
                        vModel.AdmissionList = admissionList;
                    }
                    else
                    {
                        TempData["Action"] = "Student does not have admission";
                        return RedirectToAction("SearchAdmittedStudents");
                    }
                                      
                }

                return View(vModel);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult EditAdmittedStudentDepartment(long id)
        {
            try
            {
                AdmissionList admissionList = new AdmissionList();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                admissionList = admissionListLogic.GetModelBy(p => p.Admission_List_Id == id);
                if (admissionList == null)
                {
                    TempData["Action"] = "Student is does not have admission";
                    return RedirectToAction("SearchAdmittedStudents");
                }
                UploadAdmissionViewModel vModel = new UploadAdmissionViewModel();
                vModel.AdmissionListDetail = admissionList;
                KeepApplicationFormInvoiceGenerationDropDownState(vModel);
                return View(vModel);
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public ActionResult EditAdmittedStudentDepartment(UploadAdmissionViewModel vModel)
        {
            try
            {
               if (vModel.AdmissionListDetail.Id > 0)
               {
                   AdmissionList admissionList = new AdmissionList();
                   AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                   admissionList = admissionListLogic.GetBy(vModel.AdmissionListDetail.Form.Id);
                   if (admissionList != null)
                   {
                       admissionList.Deprtment.Id = vModel.AdmissionListDetail.Deprtment.Id;
                       admissionList.Programme.Id = vModel.AdmissionListDetail.Programme.Id;
                        if (vModel.AdmissionListDetail.DepartmentOption != null && vModel.AdmissionListDetail.DepartmentOption.Id > 0)
                        {
                            admissionList.DepartmentOption = vModel.AdmissionListDetail.DepartmentOption;
                        }
                        else
                        {
                            admissionList.DepartmentOption = null;
                        }
                       
                       User user = new User();
                       UserLogic userLogic = new UserLogic();
                       user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                       string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                       
                       AdmissionListAudit Audit = new AdmissionListAudit();
                       Audit.Client = client;
                       Audit.Action = "UPDATE";
                       Audit.Operation = "UPDATING ADMISSION LIST";
                       Audit.User = user;
                       
                       bool isUpdate = admissionListLogic.Update(admissionList, Audit);
                       if (isUpdate)
                       {
                           TempData["UpdateSuccess"] = "Student Admission Details Updated Successfully";
                           return RedirectToAction("SearchAdmittedStudents");
                       }
                       TempData["UpdateFailure"] = "Student Admission Details Update Failed";
                       return RedirectToAction("SearchAdmittedStudents");
                   }
                   
               }
               return RedirectToAction("SearchAdmittedStudents");
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult EditAdmission()
        {
            UploadAdmissionViewModel viewModel = new UploadAdmissionViewModel();
            try
            { 
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.SessionId = Utility.PopulateAdmissionSessionSelectListItem();
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
            }
            catch (Exception ex)
            {   
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditAdmission(UploadAdmissionViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    List<AdmissionList> admissionList = new List<AdmissionList>();

                    admissionList = admissionListLogic.GetModelsBy(a => a.Session_Id == viewModel.CurrentSession.Id && a.APPLICATION_FORM.APPLICATION_PROGRAMME_FEE.Programme_Id == viewModel.Programme.Id && a.Department_Id == viewModel.Department.Id);
                    viewModel.AdmiissionLists = admissionList.OrderBy(a => a.Form.Person.FullName).ToList();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
            ViewBag.SessionId = Utility.PopulateAdmissionSessionSelectListItem();
            if (viewModel.Department != null && viewModel.Programme != null)
            {
                ViewBag.DepartmentId = new SelectList(Utility.PopulateDepartmentSelectListItem(viewModel.Programme), "Value", "Text", viewModel.Department.Id); 
            }
            else
            {
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME); 
            } 

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult SaveEditedAdmission(UploadAdmissionViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    AdmissionListAudit audit = new AdmissionListAudit();
                    User user = new User();
                    UserLogic userLogic = new UserLogic();

                    for (int i = 0; i < viewModel.AdmiissionLists.Count; i++)
                    {
                        if (viewModel.AdmiissionLists[i].Deactivated)
                        {
                            long currentAdmissionListId = viewModel.AdmiissionLists[i].Id;
                            AdmissionList admissionList = admissionListLogic.GetModelBy(a => a.Admission_List_Id == currentAdmissionListId);
                            admissionList.Activated = false;   
                            
                            user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                            string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                            audit.Client = client;
                            audit.Action = "MODIFY";
                            audit.Operation = "MODIFY ADMISSION";
                            audit.User = user;

                            admissionListLogic.Modify(admissionList, audit);
                        }
                        if (viewModel.AdmiissionLists[i].ActivateAlt)
                        {
                            long currentAdmissionListId = viewModel.AdmiissionLists[i].Id;
                            AdmissionList admissionList = admissionListLogic.GetModelBy(a => a.Admission_List_Id == currentAdmissionListId);
                            admissionList.Activated = true;

                            user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                            string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                            audit.Client = client;
                            audit.Action = "MODIFY";
                            audit.Operation = "MODIFY ADMISSION";
                            audit.User = user;

                            admissionListLogic.Modify(admissionList, audit);
                        }
                    }

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditAdmission");
        }
        public ActionResult SampleAdmissionUpload()
        {
            try
            {
                GridView gv = new GridView();
                List<SampleAdmissionUpload> sample = new List<SampleAdmissionUpload>();
                sample.Add(new SampleAdmissionUpload()
                {
                    SN = "1",
                    ApplicationNumber = "FPA/XXX/XXX/XXXXXXXXX"
                });

                string filename = "Sample Admission Upload";
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
                return RedirectToAction("UploadAdmission");
            }

            return RedirectToAction("UploadAdmission");
        }
    }
}