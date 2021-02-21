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
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Model.Entity.Model;
using System.Transactions;
using System.IO;
using System.Web.UI.WebControls;
using Ionic.Zip;
using System.Web.UI;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class AdmissionProcessingController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        AdmissionProposalViewModel vModel = new AdmissionProposalViewModel();
        AdmissionListBatchLogic batchLogic = new AdmissionListBatchLogic();
        ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
        ProposeAdmissionLogic proposeAdmissionLogic = new ProposeAdmissionLogic();
        ProposeAdmission proposeAdmission = new ProposeAdmission();
        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
        DepartmentLogic departmentLogic = new DepartmentLogic();
        ProgrammeLogic programmeLogic = new ProgrammeLogic();
        SessionLogic sessionLogic = new SessionLogic();
        UserLogic userLogic = new UserLogic();
        AdmissionListLogic admissionListLogic = new AdmissionListLogic();
        AdmissionList admission = new AdmissionList();
        ApplicationForm applicationForm = new ApplicationForm();
        AdmissionQuota admissionQuota = new AdmissionQuota();
        AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
        DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();

        private AdmissionProcessingViewModel viewModel;


        public AdmissionProcessingController()
        {
            viewModel = new AdmissionProcessingViewModel();
        }

        public ActionResult Index()
        {
            ViewBag.SessionId = viewModel.SessionSelectList;

            return View(viewModel);
        }

        public ActionResult ViewDetails(int? mid)
        {

            return View();
        }

        public ActionResult ClearApplicant()
        {
            viewModel.GetApplicantByStatus(ApplicantStatus.Status.CompletedStudentInformationForm);
            return View(viewModel);
        }

        public ActionResult Index2()
        {
            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
            AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == 32);

            //AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
            //ViewBag.RejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AcceptOrReject(List<int> ids, int sessionId, bool isRejected)
        {
            try
            {
                if (ids != null && ids.Count > 0)
                {
                    List<ApplicationForm> applications = new List<ApplicationForm>();
                    foreach (int id in ids)
                    {
                        ApplicationForm application = new ApplicationForm() { Id = id };
                        applications.Add(application);
                    }

                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    bool accepted = applicationFormLogic.AcceptOrReject(applications, isRejected);
                    if (accepted)
                    {
                        Session session = new Session() { Id = sessionId };
                        viewModel.GetApplicationsBy(!isRejected, session);
                        SetMessage("Select Applications has be successfully Accepted.", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Opeartion failed during selected Application Acceptance! Please try again.", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        }

        [HttpPost]
        public ActionResult FindBy(int sessionId, bool isRejected)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Session session = new Session() { Id = sessionId };
                    viewModel.GetApplicationsBy(isRejected, session);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Operation failed! " + ex.Message;
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        }

        public ActionResult ApplicationForm(long fid)
        {
            try
            {
                ApplicationFormViewModel applicationFormViewModel = new ApplicationFormViewModel();
                ApplicationForm form = new ApplicationForm() { Id = fid };

                applicationFormViewModel.GetApplicationFormBy(form);
                if (applicationFormViewModel.Person != null && applicationFormViewModel.Person.Id > 0)
                {
                    applicationFormViewModel.SetApplicantAppliedCourse(applicationFormViewModel.Person);
                }

                return View(applicationFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult StudentForm(long fid)
        {
            try
            {
                StudentFormViewModel studentFormViewModel = new StudentFormViewModel();
                ApplicationForm form = new ApplicationForm() { Id = fid };

                studentFormViewModel.LoadApplicantionFormBy(fid);

                if (studentFormViewModel.ApplicationForm.Person != null && studentFormViewModel.ApplicationForm.Person.Id > 0)
                {
                    studentFormViewModel.LoadStudentInformationFormBy(studentFormViewModel.ApplicationForm.Person.Id);
                }

                return View(studentFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ClearPartTimeApplicants()
        {
            viewModel = new AdmissionProcessingViewModel();
            try
            {
                string[] partTimeProgrammes = { Convert.ToString((int)Programmes.HNDEvening), Convert.ToString((int)Programmes.HNDPartTime), Convert.ToString((int)Programmes.NDPartTime), Convert.ToString((int)Programmes.NDEveningFullTime) };
                viewModel.ProgrammeSelectList = viewModel.ProgrammeSelectList.Where(p => string.IsNullOrEmpty(p.Value) || partTimeProgrammes.Contains(p.Value)).ToList();

                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ClearPartTimeApplicants(AdmissionProcessingViewModel viewModel)
        {
            try
            {
                AdmissionListLogic listLogic = new AdmissionListLogic();
                viewModel.ListOfAdmission = listLogic.GetModelsBy(l => l.Programme_Id == viewModel.Programme.Id && l.Department_Id == viewModel.Department.Id && l.Session_Id == viewModel.Session.Id);

                string[] partTimeProgrammes = { Convert.ToString((int)Programmes.HNDEvening), Convert.ToString((int)Programmes.HNDPartTime), Convert.ToString((int)Programmes.NDPartTime), Convert.ToString((int)Programmes.NDEveningFullTime) };
                viewModel.ProgrammeSelectList = viewModel.ProgrammeSelectList.Where(p => string.IsNullOrEmpty(p.Value) || partTimeProgrammes.Contains(p.Value)).ToList();

                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Session = viewModel.SessionSelectList;
                if (viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Department != null)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, "Id", "Name", viewModel.Department.Id);
                }
                else
                    ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");

                if (viewModel.ListOfAdmission.Any())
                    viewModel.ListOfAdmission = viewModel.ListOfAdmission.OrderBy(l => l.Form.Person.FullName).ToList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult ProposeApplicants()
        {
            AdmissionProposalViewModel vModel = new AdmissionProposalViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
            ViewBag.DepartmentOptionSelectList = new SelectList(new List<Department>(), ID, NAME);


            return View(vModel);
        }

        [HttpPost]
        public ActionResult ProposeApplicants(AdmissionProposalViewModel vModel)
        {
            try
            {
                ApplicantLogic applicantLogic = new ApplicantLogic();
                AppliedCourseLogic applicantAppliedCourse = new AppliedCourseLogic();
                AppliedCourse appliedCourse = new AppliedCourse();
                ApplicantJambDetail applicantJambDetail = new ApplicantJambDetail();
                ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
                List<ProposeAdmission> ProposeList = new List<ProposeAdmission>();
                List<AppliedCourse> AppliedCourseList = new List<AppliedCourse>();
                List<AppliedCourse> GetAppliedCourseList = new List<AppliedCourse>();
                List<ApplicantJambDetail> JambDetail = new List<ApplicantJambDetail>();
                List<OLevelResultDetail> oLevelDetailList = new List<OLevelResultDetail>();
                List<ApplicantResult> applicantResult = new List<ApplicantResult>();
                List<ApplicantResult> applicantResultList = new List<ApplicantResult>();
                //var prop;

                if(vModel.DepartmentOption != null)
                {
                    GetAppliedCourseList = applicantAppliedCourse.GetModelsBy(a => a.Programme_Id == vModel.Programme.Id && a.APPLICATION_FORM.APPLICATION_PROGRAMME_FEE.Session_Id == vModel.Session.Id && a.Department_Id == vModel.Department.Id && a.Department_Option_Id == vModel.DepartmentOption.Id);
                }
                else
                {
                    GetAppliedCourseList = applicantAppliedCourse.GetModelsBy(a => a.Programme_Id == vModel.Programme.Id && a.APPLICATION_FORM.APPLICATION_PROGRAMME_FEE.Session_Id == vModel.Session.Id && a.Department_Id == vModel.Department.Id);
                }
                

                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
                ViewBag.DepartmentOptionSelectList = new SelectList(new List<Department>(), ID, NAME);


                if (GetAppliedCourseList != null)
                {
                    foreach (var item in GetAppliedCourseList)
                    {
                        var appliedSession = item.ApplicationForm.ProgrammeFee.Session;
                        applicantJambDetail = applicantJambDetailLogic.GetModelBy(j => j.Application_Form_Id == item.ApplicationForm.Id);
                      

                        applicantResult = applicantJambDetailLogic.GetApplicantAggregateScore(item.Person, item.Programme, item.Department, appliedSession);
                        var getScore = applicantResult.LastOrDefault();

                        if (getScore != null)

                        {
                            applicantResultList.Add(getScore);
                        }
                        else
                        {
                            applicantResultList.Add(new ApplicantResult());

                        }



                        AppliedCourseList.Add(item);

                        if (applicantJambDetail != null)
                            JambDetail.Add(applicantJambDetail);


                    }
                    //vModel.ProposeAdmission = ProposeList;
                    vModel.ApplicantJambDetail = JambDetail;
                    vModel.ApplicantOLevel = oLevelDetailList;
                    vModel.ApplicantAppliedCourse = AppliedCourseList;
                    vModel.ApplicantResult = applicantResultList;
                }
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View(vModel);
        }
        public JsonResult AddProposeApplicants(int programmeId, int departmentId, int sessionId, int departmentOptionId, List<long> applicantIds)
        {
            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
            ProposeAdmissionLogic proposeAdmissionLogic = new ProposeAdmissionLogic();
            ProposeAdmission proposeAdmission = new ProposeAdmission();
            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
            DepartmentLogic departmentLogic = new DepartmentLogic();
            ProgrammeLogic programmeLogic = new ProgrammeLogic();
            SessionLogic sessionLogic = new SessionLogic();
            UserLogic userLogic = new UserLogic();
            //DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
            var username = User.Identity.Name;
            AdmissionProposalViewModel vModel = new AdmissionProposalViewModel();
            if (applicantIds != null)
            {
                foreach (var item in applicantIds)
                {

                    var check = proposeAdmissionLogic.GetModelBy(p => p.Application_Form_Id == item);
                    if (check == null)
                    {
                        var applicantInfo = appliedCourseLogic.GetModelBy(a => a.Application_Form_Id == item);
                        proposeAdmission.Person = applicantInfo.Person;
                        proposeAdmission.Department = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                        proposeAdmission.Programme = programmeLogic.GetModelBy(p => p.Programme_Id == programmeId);
                        proposeAdmission.Session = sessionLogic.GetModelBy(s => s.Session_Id == sessionId);
                        proposeAdmission.ApplicationForm = applicationFormLogic.GetModelBy(a => a.Application_Form_Id == item);
                        proposeAdmission.User = userLogic.GetModelBy(u => u.User_Name == username);
                        proposeAdmission.DepartmentOption = departmentOptionLogic.GetModelBy(d => d.Department_Option_Id == departmentOptionId);
                        proposeAdmission.Active = true;
                        proposeAdmissionLogic.Create(proposeAdmission);
                    }


                }
            }
            return Json("Operation Successful!", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DisproveProposedApplicants(List<long> applicantIds)
        {
            ProposeAdmissionLogic proposeAdmissionLogic = new ProposeAdmissionLogic();
            ProposeAdmission proposeAdmission = new ProposeAdmission();
            if (applicantIds != null)
            {
                foreach (var item in applicantIds)
                {

                    var check = proposeAdmissionLogic.GetModelBy(p => p.Application_Form_Id == item);
                    if (check != null)
                    {
                        proposeAdmissionLogic.Delete(x => x.Application_Form_Id == item);

                    }


                }
            }
            return Json("Operation Successful!", JsonRequestBehavior.AllowGet);

        }
       


        public ActionResult StudentPassportByDepartment(string fileName = null)
        {
            AdmissionProcessingViewModel vModel = new AdmissionProcessingViewModel();

            try
            {
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
                ViewBag.LevelSelectList = vModel.LevelSelectList;


                if (fileName != null)
                {
                    //new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav").Play();

                    return File(Server.MapPath("~/Content/tempFolder/" + fileName), "application/zip", fileName);
                }
            }
            catch (Exception ex) { throw ex; }

            return View(vModel);
        }

        [HttpPost]
        public ActionResult StudentPassportByDepartment(AdmissionProcessingViewModel vmodel)
        {
            try
            {
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
                ViewBag.LevelSelectList = vModel.LevelSelectList;
                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                if (vmodel.Session != null && vmodel.Session.Id > 0 && vmodel.Programme != null && vmodel.Programme.Id > 0)
                {
                    vmodel.StudentLevel = studentLevelLogic.GetModelsBy(s => s.Session_Id == vmodel.Session.Id && s.Programme_Id == vmodel.Programme.Id && s.Department_Id == vmodel.Department.Id && s.Level_Id == vmodel.Level.Id);
                    TempData["StudentDetails"] = vmodel.StudentLevel;
                    TempData["SessionId"] = vmodel.Session.Id;
                }
            }
            catch (Exception ex) { SetMessage(string.Format("A Error occured! {0}", ex.Message), Message.Category.Error); }

            return View(vmodel);
        }
        public ActionResult DownloadIdCardPassport()
        {
            string zipFileName = null;
            List<StudentLevel> studentDetails = (List<StudentLevel>)TempData["StudentDetails"];
            try
            {
                if (studentDetails != null && studentDetails.Count > 0)
                {
                    if (Directory.Exists(Server.MapPath("~/Content/tempFolder")))
                    {
                        Directory.Delete(Server.MapPath("~/Content/tempFolder"), true);
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }

                    List<StudentLevel> sort = new List<StudentLevel>();

                    System.Web.UI.WebControls.GridView gv = new GridView();

                    foreach (var student in studentDetails)
                    {
                        string imagePath = student.Student.ImageFileUrl;

                        if (string.IsNullOrEmpty(imagePath)) { continue; }

                        string[] splitUrl = imagePath.Split('/');
                        string imageUrl = splitUrl[3];
                        FileInfo fileInfo = new FileInfo(imageUrl);
                        string fileExtension = fileInfo.Extension;
                        string newFileName = string.Format("{0}{1}", imageUrl.Split('.')[0], fileExtension);

                        if (!System.IO.File.Exists(Server.MapPath("~" + imagePath))) { continue; }

                        System.IO.File.Copy(Server.MapPath(imagePath), Server.MapPath(Path.Combine("~/Content/tempFolder/", newFileName)), true);

                        student.Student.ImageFileUrl = newFileName;

                        sort.Add(student);
                    }

                    int SessionId = (int)TempData["SessionId"];
                    SessionLogic sessionLogic = new SessionLogic();

                    var SessionName = "";
                    Model.Model.Session session = new Model.Model.Session();
                    if (SessionId > 0)
                    {
                        session = sessionLogic.GetModelBy(f => f.Session_Id == SessionId);
                    }

                    SessionName = session != null ? session.Name.Replace("/", "_") : "Selected Session";
                    gv.DataSource = sort;
                    gv.Caption = string.Format("{0} SESSION", SessionName);
                    gv.DataBind();
                    SaveStudentDetailsToExcel(gv, "Students_Data.xls");

                    zipFileName = string.Format("ADO_STUDENT_Passports_For_{0}.zip", SessionName);
                    string downloadPath = "~/Content/tempFolder/";

                    using (ZipFile zip = new ZipFile())
                    {
                        string file = Server.MapPath(downloadPath);
                        zip.AddDirectory(file, "");

                        zip.Save(file + zipFileName);
                        string export = downloadPath + zipFileName;

                        Response.ClearContent();
                        Response.ClearHeaders();

                        //Set zip file name
                        Response.AppendHeader("content-disposition", "attachment; filename=" + zipFileName);

                        Response.Redirect(export, false);
                    }
                }
            }
            catch (Exception ex) { SetMessage(string.Format("Error Occured! {0}", ex.Message), Message.Category.Error); }

            new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav").Play();
            //return File();
            return RedirectToAction("StudentPassportByDepartment", "AdmissionProcessing", new { fileName = zipFileName });
        }

        public ActionResult DisproveAdmission()
        {
            DisproveAdmissionViewModel vModel = new DisproveAdmissionViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            return View(vModel);
        }

        [HttpPost]
        public ActionResult DisproveAdmission(DisproveAdmissionViewModel vModel)
        {
            AdmissionList admissionList = new AdmissionList();
            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
            ApplicationForm applicationForm = new ApplicationForm();
            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
            TempData["Programme_Id"] = vModel.Programme.Id;
            TempData["Session_Id"] = vModel.Session.Id;
            TempData["Department_Id"] = vModel.Department.Id;
            TempData.Keep("Programme_Id");
            TempData.Keep("Session_Id");
            TempData.Keep("Department_Id");
            try
            {
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

                vModel.AdmissionList = admissionListLogic.GetModelsBy(a => a.Programme_Id == vModel.Programme.Id && a.Department_Id == vModel.Department.Id && a.Session_Id == vModel.Session.Id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return View(vModel);
        }

        public JsonResult DisproveAdmissionAction(List<long> admissionListIds, int progId, int sessId, int deptId)
        {
            JsonResultModel result = new JsonResultModel();
            AdmissionList admissionList = new AdmissionList();
            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
            ApplicationForm applicationForm = new ApplicationForm();
            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
            AdmissionQuota admissionQuota = new AdmissionQuota();
            AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
            //int progId = (int)TempData["Programme_Id"];
            //int sessId = (int)TempData["Session_Id"];
            //int deptId = (int)TempData["Department_Id"];

            try
            {
                long retractedQuota = 0;

                if (admissionListIds != null && admissionListIds.Count > 0)
                {
                    using (var t_scope = new TransactionScope())
                    {

                        foreach (var item in admissionListIds)
                        {
                            var studentExist = admissionListLogic.GetModelsBy(e => e.Admission_List_Id == item).LastOrDefault();
                            if (studentExist != null)
                            {
                                admissionListLogic.Delete(x => x.Admission_List_Id == item);
                                retractedQuota++;
                            }

                        }
                        if (retractedQuota > 0)
                        {
                            admissionQuota = admissionQuotaLogic.GetModelsBy(q => q.Programme_Id == progId && q.Session_Id == sessId && q.Department_Id == deptId).FirstOrDefault();
                            if (admissionQuota != null)
                            {
                                long initialQuota = admissionQuota.UnusedQuota;
                                long newQuota = initialQuota + retractedQuota;
                                admissionQuota.UnusedQuota = newQuota;
                                admissionQuota.Active = true;
                                admissionQuotaLogic.Modify(admissionQuota);
                            }
                        }
                        t_scope.Complete();

                    }
                    result.IsError = false;
                    result.Message = "Operation Successful!";
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    result.IsError = false;
                    result.Message = "No Student was selected!";
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);

            }


        }

        public ActionResult GetProposeApplicants()
        {
            AdmissionProposalViewModel vModel = new AdmissionProposalViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
            ViewBag.DepartmentOptionSelectList = new SelectList(new List<Department>(), ID, NAME);
            ViewBag.Quota = 0;

            return View(vModel);
        }
        [HttpPost]
        public ActionResult GetProposeApplicants(AdmissionProposalViewModel vModel)
        {
            ApplicantLogic applicantLogic = new ApplicantLogic();
            AppliedCourseLogic applicantAppliedCourse = new AppliedCourseLogic();
            ProposeAdmissionLogic proposeAdmissionLogic = new ProposeAdmissionLogic();
            ApplicantJambDetail applicantJambDetail = new ApplicantJambDetail();
            ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
            ProposeAdmission proposeAdmission = new ProposeAdmission();
            OLevelResult oLevelResult = new OLevelResult();
            OLevelResultDetail oLevelResultDetail = new OLevelResultDetail();
            OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
            OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();

            List<ProposeAdmission> ProposeList = new List<ProposeAdmission>();
            List<ProposeAdmission> GetProposeList = new List<ProposeAdmission>();
            List<ApplicantJambDetail> JambDetail = new List<ApplicantJambDetail>();
            List<OLevelResultDetail> oLevelDetailList = new List<OLevelResultDetail>();
            List<ApplicantResult> applicantResult = new List<ApplicantResult>();
            List<ApplicantResult> applicantResultList = new List<ApplicantResult>();



            try
            {
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
                DepartmentOptionLogic departmentLogic = new DepartmentOptionLogic();
               
                    ViewBag.DepartmentOptionSelectList = new SelectList(new List<Department>(), ID, NAME);
                

                if (vModel.DepartmentOption != null)
                {
                    GetProposeList = proposeAdmissionLogic.GetModelsBy(p => p.Programme_Id == vModel.Programme.Id && p.Department_Id == vModel.Department.Id && p.Department_Option_Id == vModel.DepartmentOption.Id && p.Active);
                }
                else
                {
                    GetProposeList = proposeAdmissionLogic.GetModelsBy(p => p.Programme_Id == vModel.Programme.Id && p.Department_Id == vModel.Department.Id && p.Active);
                }
                

                if (GetProposeList != null)
                {
                    foreach (var item in GetProposeList)
                    {
                        var getResult = oLevelResultLogic.GetModelsBy(l => l.Application_Form_Id == item.ApplicationForm.Id).LastOrDefault();
                        var appliedSession = item.ApplicationForm.ProgrammeFee.Session;
                        applicantJambDetail = applicantJambDetailLogic.GetModelBy(j => j.Application_Form_Id == item.ApplicationForm.Id);
                        proposeAdmission = proposeAdmissionLogic.GetModelBy(p => p.Application_Form_Id == item.ApplicationForm.Id);
                        applicantResult = applicantJambDetailLogic.GetApplicantAggregateScore(item.ApplicationForm.Person, item.Programme, item.Department, appliedSession);
                        var getScore = applicantResult.LastOrDefault();

                        if (getScore != null)

                        {
                            applicantResultList.Add(getScore);
                        }
                        //else
                        //{
                        //    applicantResultList.Add(new ApplicantResult());

                        //}
                        if (proposeAdmission != null)
                            ProposeList.Add(proposeAdmission);
                        if (applicantJambDetail != null)
                            JambDetail.Add(applicantJambDetail);

                    }
                    vModel.ProposeAdmission = ProposeList;
                    vModel.ApplicantJambDetail = JambDetail;
                    vModel.ApplicantOLevel = oLevelDetailList;
                    vModel.ApplicantResult = applicantResultList;

                }



            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }


            return View(vModel);
        }

        public JsonResult GetAdmissionQuota(int programmeId, int departmentId, int sessionId)
        {
            AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
            AdmissionQuota admissionQuota = new AdmissionQuota();
            JsonResultModel jsonResult = new JsonResultModel();
            JsonResultObject jsonResultObject = new JsonResultObject();

            try
            {
                admissionQuota = admissionQuotaLogic.GetModelBy(a => a.Department_Id == departmentId && a.Session_Id == sessionId && a.Programme_Id == programmeId);
                if (admissionQuota == null)
                {
                    jsonResultObject.Message = "Admission Quota not set for the selected Session/Department";
                    return Json(jsonResultObject, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    jsonResultObject.Message = Convert.ToString(admissionQuota.Quota);
                    jsonResultObject.UnusedQuota = Convert.ToString(admissionQuota.UnusedQuota);

                }


            }
            catch (Exception ex)
            {
                SetMessage("Error!" + ex.Message, Message.Category.Information);
            }
            return Json(jsonResultObject, JsonRequestBehavior.AllowGet);


        }

        public JsonResult AdmitProposedApplicants(int programmeId, int departmentId, int sessionId, int departmentOptionId, List<long> applicantIds)
        {
            long unUsedQuota = 0;


            long alreadyExistCount = 0;
            admissionQuota = admissionQuotaLogic.GetModelBy(q => q.Programme_Id == programmeId && q.Session_Id == sessionId && q.Department_Id == departmentId);
            if (admissionQuota != null && admissionQuota.UnusedQuota > 0)
            {
                unUsedQuota = admissionQuota.UnusedQuota;

            }
            else
            {
                return Json("No set/Unused Quota found for the selected data!", JsonRequestBehavior.AllowGet);

            }


            if (applicantIds != null && applicantIds.Count > 0)
            {
                using (var t_scope = new TransactionScope())
                {
                    var listType = GetAdmissionListType(programmeId);

                    AdmissionListBatch batch = new AdmissionListBatch();
                    batch.DateUploaded = DateTime.Now;
                    batch.IUploadedFilePath = "Applicant Admission";
                    batch.Type = new AdmissionListType() { Id = listType };

                    AdmissionListBatch newBatch = batchLogic.Create(batch);
                    List<ApplicationForm> applicantEmail = new List<ApplicationForm>();

                    foreach (var item in applicantIds)
                    {
                        var doesApplicantExist = admissionListLogic.GetModelBy(e => e.Application_Form_Id == item);
                        if (doesApplicantExist != null)
                        {
                            alreadyExistCount++;
                        }
                        else
                        {
                            admission.Form = applicationFormLogic.GetModelBy(n => n.Application_Form_Id == item);
                            admission.Batch = newBatch;
                            admission.Deprtment = departmentLogic.GetModelBy(d => d.Department_Id == departmentId);
                            admission.Programme = programmeLogic.GetModelBy(p => p.Programme_Id == programmeId);
                            admission.Session = sessionLogic.GetModelBy(s => s.Session_Id == sessionId);
                            admission.DepartmentOption = departmentOptionLogic.GetModelBy(x => x.Department_Option_Id == departmentOptionId);
                            admission.Activated = true;
                            admissionListLogic.Create(admission);

                            unUsedQuota--;
                            admissionQuota.UnusedQuota = unUsedQuota;
                            admissionQuotaLogic.Modify(admissionQuota);

                            proposeAdmission = proposeAdmissionLogic.GetModelBy(p => p.Application_Form_Id == item);
                            proposeAdmission.Active = false;
                            proposeAdmissionLogic.Modify(proposeAdmission);
                            if (admission.Form != null)
                            {
                                applicantEmail.Add(admission.Form);

                            }

                        }

                    }
                    t_scope.Complete();

                    SendMail(applicantEmail);
                }

            }

            return Json("Operation Successful!", JsonRequestBehavior.AllowGet);
        }


        public void SendMail(List<Model.Model.ApplicationForm> applicantList)
        {
            try
            {
                if (applicantList?.Count > 0)
                {
                    foreach (var item in applicantList)
                    {
                        AdmissionEmail admissionEmail = new AdmissionEmail();
                        admissionEmail.Name = item.Person.FirstName;
                        EmailMessage message = new EmailMessage();

                        message.Email = item.Person.Email ?? "support@lloydant.com";
                        message.Subject = "FEDERAL POLYTECHNIC ADO-EKITI";
                        admissionEmail.header = "Admission Notification";
                        admissionEmail.footer = "https://applications.federalpolyilaro.edu.ng/Security/Account/Login";
                        message.Body = "You have been offered Admission at our institution";
                        admissionEmail.message = message.Body;

                        var template = Server.MapPath("/Areas/Admin/Views/AdmissionProcessing/EmailTemplate.cshtml");
                        EmailSenderLogic<AdmissionEmail> receiptEmailSenderLogic = new EmailSenderLogic<AdmissionEmail>(template, admissionEmail);

                        receiptEmailSenderLogic.Send(message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SetAdmissionQuota()
        {
            AdmissionQuotaViewModel vModel = new AdmissionQuotaViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            return View(vModel);
        }

        [HttpPost]
        public ActionResult SetAdmissionQuota(AdmissionQuotaViewModel vModel)
        {
            DepartmentLogic departmentLogic = new DepartmentLogic();
            ProgrammeLogic programmeLogic = new ProgrammeLogic();
            SessionLogic sessionLogic = new SessionLogic();
            UserLogic userLogic = new UserLogic();
            AdmissionQuota admissionQuota = new AdmissionQuota();
            AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
            var username = User.Identity.Name;
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            if (vModel != null)
            {
                var existQuota = admissionQuotaLogic.GetModelsBy(f => f.Department_Id == vModel.Department.Id && f.Programme_Id == vModel.Programme.Id
                  && f.Session_Id == vModel.Session.Id);
                if (existQuota?.Count > 0)
                {
                    SetMessage("Quota already exist", Message.Category.Information);
                    return View(vModel);
                }
                admissionQuota.Programme = vModel.Programme;
                admissionQuota.Department = vModel.Department;
                admissionQuota.Session = vModel.Session;
                admissionQuota.Quota = vModel.Quota;
                admissionQuota.UnusedQuota = vModel.Quota;
                admissionQuota.User = userLogic.GetModelBy(u => u.User_Name == username);
                admissionQuotaLogic.Create(admissionQuota);
                ViewBag.Success = true;
                SetMessage("Quota set successfully", Message.Category.Information);
                return View(vModel);
            }
            else
            {
                ViewBag.Error = true;

            }


            return View(vModel);
        }

        //public long GetAdmissionQuota(int programmeId, int departmentId, int sessionId)
        //{
        //    AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
        //    AdmissionQuota admissionQuota = new AdmissionQuota();
        //    JsonResultModel jsonResult = new JsonResultModel();

        //    try
        //    {
        //        admissionQuota = admissionQuotaLogic.GetModelBy(a => a.Department_Id == departmentId && a.Session_Id == sessionId && a.Programme_Id == programmeId);
        //        if (admissionQuota != null)
        //        {
        //            return admissionQuota.Quota;


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error!" + ex.Message, Message.Category.Information);
        //    }
        //    return 0;


        //}
        public ActionResult ClearFullTimeApplicants()
        {
            viewModel = new AdmissionProcessingViewModel();
            try
            {
                string[] fullTimeProgrammes = { Convert.ToString((int)Programmes.NDFullTime), Convert.ToString((int)Programmes.HNDFullTime) };
                viewModel.ProgrammeSelectList = viewModel.ProgrammeSelectList.Where(p => string.IsNullOrEmpty(p.Value) || fullTimeProgrammes.Contains(p.Value)).ToList();

                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ClearFullTimeApplicants(AdmissionProcessingViewModel viewModel)
        {
            try
            {
                AdmissionListLogic listLogic = new AdmissionListLogic();
                viewModel.ListOfAdmission = listLogic.GetModelsBy(l => l.Programme_Id == viewModel.Programme.Id && l.Department_Id == viewModel.Department.Id && l.Session_Id == viewModel.Session.Id);

                string[] fullTimeProgrammes = { Convert.ToString((int)Programmes.NDFullTime), Convert.ToString((int)Programmes.HNDFullTime) };
                viewModel.ProgrammeSelectList = viewModel.ProgrammeSelectList.Where(p => string.IsNullOrEmpty(p.Value) || fullTimeProgrammes.Contains(p.Value)).ToList();

                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Session = viewModel.SessionSelectList;
                if (viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Department != null)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, "Id", "Name", viewModel.Department.Id);
                }
                else
                    ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");

                if (viewModel.ListOfAdmission.Any())
                    viewModel.ListOfAdmission = viewModel.ListOfAdmission.OrderBy(l => l.Form.Person.FullName).ToList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public JsonResult ClearApplicantForAcceptance(long personId, bool status, string remark)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
                GeneralAudit generalAudit = new GeneralAudit();
                UserLogic userLogic = new UserLogic();
                ApplicantClearanceLogic clearanceLogic = new ApplicantClearanceLogic();
                ApplicantLogic applicantLogic = new ApplicantLogic();

                bool? initialStatus = false;

                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                ApplicationForm form = applicationFormLogic.GetModelsBy(a => a.Person_Id == personId).LastOrDefault();
                if (form != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        initialStatus = form.CleardForAcceptance;

                        form.CleardForAcceptance = status;
                        form.Remarks = remark;
                        applicationFormLogic.Modify(form);

                        ApplicantClearance applicantClearance = clearanceLogic.GetModelsBy(c => c.Application_Form_Id == form.Id).LastOrDefault();
                        if (applicantClearance == null)
                        {
                            applicantClearance = new ApplicantClearance();
                            applicantClearance.ApplicationForm = form;
                            applicantClearance.Cleared = status;
                            applicantClearance.DateCleared = DateTime.Now;
                            applicantClearance.User = user;

                            clearanceLogic.Create(applicantClearance);

                            generalAudit.Action = "CLEARANCE";
                            generalAudit.Client = client;
                            generalAudit.CurrentValues = Convert.ToString(status);
                            generalAudit.InitialValues = Convert.ToString(initialStatus);
                            generalAudit.Operation = "Cleared applicant for acceptance, " + form.Number;
                        }
                        else
                        {
                            applicantClearance.Cleared = status;
                            applicantClearance.DateCleared = DateTime.Now;
                            applicantClearance.User = user;

                            clearanceLogic.Modify(applicantClearance);

                            generalAudit.Action = "MODIFY";
                            generalAudit.Client = client;
                            generalAudit.CurrentValues = Convert.ToString(status);
                            generalAudit.InitialValues = Convert.ToString(initialStatus);
                            generalAudit.Operation = "Updated applicant clearance, " + form.Number;
                        }

                        Model.Model.Applicant applicant = applicantLogic.GetModelsBy(a => a.Application_Form_Id == form.Id).LastOrDefault();
                        if (applicant != null && status)
                        {
                            applicant.Status = new ApplicantStatus { Id = (int)ApplicantStatus.Status.ClearedAndAccepted };
                            applicantLogic.Modify(applicant);
                        }
                        else if (applicant != null && !status)
                        {
                            applicant.Status = new ApplicantStatus { Id = (int)ApplicantStatus.Status.ClearedAndRejected };
                            applicantLogic.Modify(applicant);
                        }

                        generalAudit.TableNames = "APPLICATION_FORM, APPLICANT_CLEARANCE";
                        generalAudit.Time = DateTime.Now;
                        generalAudit.User = user;

                        generalAuditLogic.Create(generalAudit);

                        scope.Complete();
                    }

                    return Json("Operation Successful!", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("Form not found!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error! " + ex.Message, JsonRequestBehavior.AllowGet);
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

        public int GetAdmissionListType(int programmeId)
        {
            int listType = 1;


            switch (programmeId)
            {
                case 1:
                    listType = 1;
                    break;
                case 2:
                    listType = 3;
                    break;
                case 3:
                    listType = 2;
                    break;
                case 4:
                    listType = 4;
                    break;
                case 5:
                    listType = 2;
                    break;
                case 6:
                    listType = 1;
                    break;
            }
            return listType;
        }
        public ActionResult ViewQuota()
        {
            AdmissionQuotaViewModel vModel = new AdmissionQuotaViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            return View(vModel);
        }
        [HttpPost]
        public ActionResult ViewQuota(AdmissionQuotaViewModel vModel)
        {
            AdmissionQuotaLogic admissionQuotaLogic = new AdmissionQuotaLogic();
            Department department = new Department();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
            if (vModel?.Programme?.Id > 0 && vModel?.Session?.Id > 0)
            {
                vModel.AdmissionQuota = admissionQuotaLogic.GetModelsBy(f => f.Programme_Id == vModel.Programme.Id && f.Session_Id == vModel.Session.Id);
            }
            return View(vModel);
        }
        public ActionResult EditQuota(int qid)
        {
            AdmissionQuotaViewModel vModel = new AdmissionQuotaViewModel();

            try
            {
                admissionQuota = admissionQuotaLogic.GetModelBy(q => q.Quota_Id == qid);
                vModel.AdmissionQuotaEdit = admissionQuota;

                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(vModel);
        }

        [HttpPost]
        public ActionResult EditQuota(AdmissionQuotaViewModel vModel)
        {
            long diff = 0;
            long final = 0;

            try
            {
                admissionQuota = admissionQuotaLogic.GetModelBy(q => q.Quota_Id == vModel.AdmissionQuotaEdit.Id);
                var username = User.Identity.Name;
                if (admissionQuota != null)
                {
                    long former = admissionQuota.Quota;
                    long current = vModel.AdmissionQuotaEdit.Quota;
                    if (current > former)
                    {
                        diff = current - former;
                        final = admissionQuota.UnusedQuota + diff;
                        vModel.AdmissionQuotaEdit.UnusedQuota = final;
                    }
                    else if (former > current)
                    {
                        diff = former - current;
                        final = admissionQuota.UnusedQuota - diff;
                        vModel.AdmissionQuotaEdit.UnusedQuota = final;
                        if (vModel.AdmissionQuotaEdit.UnusedQuota < 0)
                        {
                            vModel.AdmissionQuotaEdit.UnusedQuota = 0;
                        }
                    }

                    admissionQuota = vModel.AdmissionQuotaEdit;
                    admissionQuota.User = userLogic.GetModelBy(u => u.User_Name == username);

                    admissionQuotaLogic.Modify(admissionQuota);
                    SetMessage("Admission Quota was successfully updated!", Message.Category.Information);
                }
                vModel.AdmissionQuotaEdit = admissionQuota;
                ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                ViewBag.SessionSelectList = vModel.SessionSelectList;
                ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(vModel);
        }

        public void SaveStudentDetailsToExcel(GridView ExcelGridView, string fileName)
        {
            try
            {
                Response.Clear();

                Response.Charset = "";

                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.ContentType = "application/vnd.ms-excel";

                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                ExcelGridView.RenderControl(htw);

                Response.Write(sw.ToString());
                string renderedGridView = sw.ToString();
                System.IO.File.WriteAllText(Server.MapPath(Path.Combine("~/Content/temp/", fileName)), renderedGridView);
            }
            catch (Exception ex) { throw ex; }
        }

        public ActionResult OpenCloseApplication()
        {
            Programme programme = new Programme();
            ProgrammeLogic programmeLogic = new ProgrammeLogic();
            ProgrammeSetUpViewModel vModel = new ProgrammeSetUpViewModel();
            List<Programme> programmeList = programmeLogic.GetModelsBy(p => p.Programme_Id > 0).ToList();
            if(programmeList.Count <= 0)
            {
                return View();
            }
            vModel.ApplicationProgrammeList = programmeList;
            return View(vModel);
        }

        public ActionResult ToggleApplicationOpenClose(long aid)
        {
            Programme programme = new Programme();
            ProgrammeLogic programmeLogic = new ProgrammeLogic();
            try
            {
                programme = programmeLogic.GetModelBy(p => p.Programme_Id == aid);
                if(programme.IsActiveForApplication == false || programme.IsActiveForApplication == null)
                {
                    programme.IsActiveForApplication = true;
                    programmeLogic.Modify(programme);
                }
                else
                {
                    programme.IsActiveForApplication = false;
                    programmeLogic.Modify(programme);
                }
                SetMessage("Operation was successful", Message.Category.Information);
                return RedirectToAction("OpenCloseApplication", "AdmissionProcessing");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //private AdmissionProcessingViewModel viewModel;
        //private Abundance_NkEntities db = new Abundance_NkEntities();

        //public AdmissionProcessingController()
        //{
        //    viewModel = new AdmissionProcessingViewModel();
        //}

        //public ActionResult Index()
        //{
        //    ViewBag.SessionId = viewModel.SessionSelectList;

        //    return View(viewModel);
        //}

        //[AllowAnonymous]
        //public ActionResult Index2()
        //{
        //    try
        //    {
        //        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
        //        PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
        //        AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == 32);
        //        PreviousEducation previouseducation = previousEducationLogic.GetModelBy(p => p.Person_Id == 32);

        //        AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
        //        string rejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse, previouseducation);
        //        ViewBag.RejectReason = rejectReason;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.RejectReason = ex.Message;
        //        SetMessage(ex.Message, Message.Category.Error);
        //    }

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public ActionResult AcceptOrReject(List<int> ids, int sessionId, bool isRejected)
        //{
        //    try
        //    {
        //        if (ids != null && ids.Count > 0)
        //        {
        //            List<ApplicationForm> applications = new List<ApplicationForm>();

        //            foreach (int id in ids)
        //            {
        //                ApplicationForm application = new ApplicationForm() { Id = id };
        //                applications.Add(application);
        //            }

        //            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
        //            bool accepted = applicationFormLogic.AcceptOrReject(applications, isRejected);
        //            if (accepted)
        //            {
        //                Session session = new Session() { Id = sessionId };
        //                viewModel.GetApplicationsBy(!isRejected, session);
        //                SetMessage("Select Applications has be successfully Accepted.", Message.Category.Information);
        //            }
        //            else
        //            {
        //                SetMessage("Opeartion failed during selected Application Acceptance! Please try again.", Message.Category.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
        //    }

        //    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        //}

        //[HttpPost]
        //public ActionResult FindBy(int sessionId, bool isRejected)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Session session = new Session() { Id = sessionId };
        //            viewModel.GetApplicationsBy(isRejected, session);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["msg"] = "Operation failed! " + ex.Message;
        //    }

        //    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        //}

        ////public ActionResult FindAllAcceptedBy(int sessionId)
        ////{
        ////    try
        ////    {
        ////        if (ModelState.IsValid)
        ////        {
        ////            Session session = new Session() { Id = sessionId };
        ////            viewModel.GetApplicationsBy(false, session);
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {

        ////    }

        ////    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        ////}

        ////[HttpPost]
        ////public void ApproveOrReject(List<int> ids, string status)
        ////{
        ////    try
        ////    {
        ////        TempData["msg"] = "Operation was successful.";
        ////    }
        ////    catch(Exception ex)
        ////    {
        ////        TempData["msg"] = "Operation failed! " + ex.Message;
        ////    }
        ////}



        ////[HttpPost]
        ////public ActionResult Index(AdmissionProcessingViewModel admissionProcessingViewModel)
        ////{
        ////    //bool rejected = admissionProcessingViewModel.Rejected;
        ////    //admissionProcessingViewModel.GetApplicationsBy(admissionProcessingViewModel.Rejected);

        ////    return View(admissionProcessingViewModel.ApplicationForms);
        ////}

        //// GET: /Admin/AdmissionProcessing/Details/5
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue");
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id");
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number");
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name");
        //    return View();
        //}

        //// POST: /Admin/AdmissionProcessing/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="Application_Form_Id,Serial_Number,Application_Form_Number,Application_Form_Setting_Id,Application_Programme_Fee_Id,Payment_Id,Person_Id,Date_Submitted,Release,Rejected,Reject_Reason,Remarks")] APPLICATION_FORM application_form)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.APPLICATION_FORM.Add(application_form);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// POST: /Admin/AdmissionProcessing/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="Application_Form_Id,Serial_Number,Application_Form_Number,Application_Form_Setting_Id,Application_Programme_Fee_Id,Payment_Id,Person_Id,Date_Submitted,Release,Rejected,Reject_Reason,Remarks")] APPLICATION_FORM application_form)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(application_form).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(application_form);
        //}

        //// POST: /Admin/AdmissionProcessing/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    db.APPLICATION_FORM.Remove(application_form);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
