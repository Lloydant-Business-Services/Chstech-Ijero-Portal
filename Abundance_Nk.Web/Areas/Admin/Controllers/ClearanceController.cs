using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class ClearanceController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private ClearanceViewModel viewmodel;

        public ActionResult Index(string sortOrder)
        {
            viewmodel = new ClearanceViewModel();
            if (TempData["ClearanceViewModel"] != null)
            {
                viewmodel = (ClearanceViewModel)TempData["ClearanceViewModel"];
            }
            string sortDesc = sortOrder;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullName = String.IsNullOrEmpty(sortDesc) ? "name_desc" : "";
            ViewBag.Number = String.IsNullOrEmpty(sortDesc) ? "Number_desc" : "";
            ViewBag.Programme = String.IsNullOrEmpty(sortDesc) ? "Programme_desc" : "";
            ViewBag.Department = String.IsNullOrEmpty(sortDesc) ? "Department_desc" : "";

            try
            {
                switch (sortDesc)
                {
                    case "name_desc":
                        viewmodel.appliedCourseList = viewmodel.appliedCourseList.OrderByDescending(s => s.Person.FullName).ToList();
                        break;
                    case "Number_desc":
                        viewmodel.appliedCourseList = viewmodel.appliedCourseList.OrderByDescending(s => s.ApplicationForm.Number).ToList();
                        break;
                    case "Programme_desc":
                        viewmodel.appliedCourseList = viewmodel.appliedCourseList.OrderByDescending(s => s.Programme.Name).ToList();
                        break;
                    case "Department_desc":
                        viewmodel.appliedCourseList = viewmodel.appliedCourseList.OrderByDescending(s => s.Department.Name).ToList();
                        break;
                    default:
                        viewmodel.appliedCourseList = viewmodel.appliedCourseList.OrderByDescending(s => s.ApplicationForm.Id).ToList();
                        break;
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }



            TempData["ClearanceViewModel"] = viewmodel;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(ClearanceViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vModel.ApplicationNumber != null)
                    {
                        ClearanceViewModel viewModel = new ClearanceViewModel();

                        List<ApplicationForm> applicationForm = new List<ApplicationForm>();
                        ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                        List<Person> personList = new List<Person>();
                        PersonLogic personLogic = new PersonLogic();

                        personList = personLogic.GetModelsBy(n => n.Last_Name == vModel.ApplicationNumber || n.First_Name == vModel.ApplicationNumber || n.Other_Name == vModel.ApplicationNumber || n.Mobile_Phone == vModel.ApplicationNumber);
                        if (personList != null && personList.Count > 0)
                        {
                            foreach (Person applicant in personList)
                            {
                                ApplicationForm appForm = applicationFormLogic.GetModelBy(m => m.Person_Id == applicant.Id);
                                applicationForm.Add(appForm);
                            }

                        }
                        else
                        {

                            ApplicationForm appForm = applicationFormLogic.GetModelBy(m => m.Application_Form_Number == vModel.ApplicationNumber || m.Application_Exam_Number == vModel.ApplicationNumber);
                            applicationForm.Add(appForm);
                        }




                        if (applicationForm != null && applicationForm.Count > 0)
                        {
                            List<AppliedCourse> appCourse = new List<AppliedCourse>();
                            List<OLevelResult> result = new List<OLevelResult>();
                            List<OLevelResultDetail> resultdetail = new List<OLevelResultDetail>();
                            List<ApplicationForm> applicantForm = new List<ApplicationForm>();

                            AppliedCourse appliedcourse = new AppliedCourse();
                            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                            foreach (ApplicationForm applicant in applicationForm)
                            {
                                if (applicant != null && applicant.Person.Id > 0)
                                {
                                    appliedcourse = appliedCourseLogic.GetModelBy(n => n.Person_Id == applicant.Person.Id);
                                    if (appliedcourse != null && appliedcourse.Department.Id > 0)
                                    {
                                        applicantForm.Add(applicant);
                                        appCourse.Add(appliedcourse);

                                    }
                                }

                            }

                            viewModel.applicationFormList = applicantForm;
                            viewModel.appliedCourseList = appCourse;
                            TempData["ClearanceViewModel"] = viewModel;


                            return View(viewModel);


                        }
                        else
                        {
                            SetMessage("Record does not exist! ", Message.Category.Error);
                            return View(vModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(vModel);
        }

        public ActionResult View(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                AppliedCourse appliedCourse = new AppliedCourse();
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();

                long personid = Id;
                if (personid > 0)
                {
                    appliedCourse = appliedCourseLogic.GetModelBy(x => x.Person_Id == personid);
                    if (appliedCourse != null && appliedCourse.Person.Id > 0)
                    {
                        viewmodel.appliedCourse = appliedCourse;
                        viewmodel.person = appliedCourse.Person;
                        viewmodel.LoadApplicantResult(appliedCourse.Person);
                    }
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }



            TempData["ClearanceViewModel"] = viewmodel;
            return View(viewmodel);
        }

    
        public ActionResult AdmissionCriteria(string sortOrder)
        {
            viewmodel = new ClearanceViewModel();
            if ( TempData["ClearanceViewModel"] != null)
            {
                viewmodel = (ClearanceViewModel)TempData["ClearanceViewModel"];
            }
            try
            {
            string sortDesc = sortOrder;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Department = String.IsNullOrEmpty(sortDesc) ? "Department_desc" : "";
            ViewBag.Programme = String.IsNullOrEmpty(sortDesc) ? "Programme_desc" : "";
            ViewBag.Minimum = String.IsNullOrEmpty(sortDesc) ? "Minimum_desc" : "";
            ViewBag.Date = String.IsNullOrEmpty(sortDesc) ? "Date_desc" : "";

           
                switch (sortDesc)
                {
                    case "Department_desc":
                        viewmodel.admissionCriteriaList = viewmodel.admissionCriteriaList.OrderByDescending(s => s.Department.Name).ToList();
                        break;
                    case "Programme_desc":
                        viewmodel.admissionCriteriaList = viewmodel.admissionCriteriaList.OrderByDescending(s => s.Programme.Name).ToList();
                        break;
                    case "Minimum_desc":
                        viewmodel.admissionCriteriaList = viewmodel.admissionCriteriaList.OrderByDescending(s => s.MinimumRequiredNumberOfSubject).ToList();
                        break;
                    case "Date_desc":
                        viewmodel.admissionCriteriaList = viewmodel.admissionCriteriaList.OrderByDescending(s => s.DateEntered).ToList();
                        break;
                    default:
                        viewmodel.admissionCriteriaList = viewmodel.admissionCriteriaList.OrderByDescending(s => s.Department.Name).ToList();
                        break;

                        
                }
                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);
            }
          
               
               
          
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        public ActionResult ViewCriteria(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                AdmissionCriteriaForOLevelSubjectLogic OlevelLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                List<AdmissionCriteriaForOLevelSubjectAlternative> olevelAltList = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                AdmissionCriteriaForOLevelSubjectAlternative olevelAlt = new AdmissionCriteriaForOLevelSubjectAlternative();
                AdmissionCriteriaForOLevelSubjectAlternativeLogic olevelAltLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                AdmissionCriteriaForOLevelTypeLogic olevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();

                viewmodel.admissionCriteriaForOLevelSubject = OlevelLogic.GetModelsBy(a => a.Admission_Criteria_Id == Id);
                foreach (AdmissionCriteriaForOLevelSubject subject in viewmodel.admissionCriteriaForOLevelSubject)
                {
                    subject.Alternatives = olevelAltLogic.GetModelsBy(o => o.Admission_Criteria_For_O_Level_Subject_Id == subject.Id);
                    
                }
                viewmodel.admissionCriteriaForOLevelType = olevelTypeLogic.GetModelsBy(n => n.Admission_Criteria_Id == Id);
              
                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }


        public ActionResult AddAdmissionCriteria()
        {
            ClearanceViewModel Viewmodel = new ClearanceViewModel();

            try
            {
                ViewBag.ProgrammeId = Viewmodel.ProgrammeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);

                for (int i = 0; i < 15; i++)
                {
                    ViewData["FirstSittingOLevelSubjectId" + i] = Viewmodel.OLevelSubjectSelectList;
                    ViewData["SecondSittingOLevelSubjectId" + i] = Viewmodel.OLevelSubjectSelectList;
                }
                for (int i = 0; i < 8; i++)
                {
                    ViewData["OlevelTypeId" + i] = Viewmodel.OlevelTypeSelectListItem;
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["viewModel"] = Viewmodel;
            return View(Viewmodel);
        }

        [HttpPost]
        public ActionResult AddAdmissionCriteria(ClearanceViewModel criteriaModel)
        {

            try
            {
               
                if (ModelState.IsValid)
                {
                    //Add team and redirect
                }

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {
                    if (criteriaModel.Department != null && criteriaModel.Programme != null && criteriaModel.OLevelSubjects != null && criteriaModel.OLevelSubjects.Count > 0 && criteriaModel.OLevelTypes != null && criteriaModel.OLevelTypes.Count > 0)
                    {
                        AdmissionCriteria criteria = new AdmissionCriteria();
                        AdmissionCriteria criteria2 = new AdmissionCriteria();
                        AdmissionCriteriaLogic criteriaLogic = new AdmissionCriteriaLogic();
                        criteria = criteriaLogic.GetModelBy(c => c.Department_Id == criteriaModel.Department.Id && c.Programme_Id == criteriaModel.Programme.Id);
                        if (criteria == null)
                        {
                            criteria2.DateEntered = DateTime.Now;
                            criteria2.Department = criteriaModel.Department;
                            criteria2.Programme = criteriaModel.Programme;
                            criteria2.MinimumRequiredNumberOfSubject = 5;
                            criteriaLogic.Create(criteria2);
                            criteria = criteriaLogic.GetModelBy(c => c.Department_Id == criteriaModel.Department.Id && c.Programme_Id == criteriaModel.Programme.Id);

                            //Add subjects
                            AdmissionCriteriaForOLevelSubjectLogic criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                            AdmissionCriteriaForOLevelSubject criteriaSubject = new AdmissionCriteriaForOLevelSubject();
                            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                            AdmissionCriteriaForOLevelSubjectAlternative criteriaSubjectAlternative = new AdmissionCriteriaForOLevelSubjectAlternative();
                            OLevelGrade grade = new OLevelGrade();
                            OLevelGradeLogic gradeLogic = new OLevelGradeLogic();
                            grade = gradeLogic.GetModelBy(g => g.O_Level_Grade_Id == 6);

                            int count = 0;
                            foreach (OLevelSubject subject in criteriaModel.OLevelSubjects)
                            {
                                if (subject != null && subject.Id > 0)
                                {
                                    criteriaSubject.MainCriteria = criteria;
                                    criteriaSubject.Subject = (OLevelSubject)subject;
                                    criteriaSubject.IsCompulsory = subject.IsChecked;
                                    criteriaSubject.MinimumGrade = grade;
                                    criteriaSubject = criteriaSubjectLogic.Create(criteriaSubject);
                                    if (criteriaModel.OLevelSubjectsAlternatives[count].Id > 0)
                                    {
                                        criteriaSubjectAlternative.OLevelSubject = criteriaModel.OLevelSubjectsAlternatives[count];
                                        criteriaSubjectAlternative.Alternative = criteriaSubject;
                                        criteriaSubjectAlternativeLogic.Create(criteriaSubjectAlternative);
                                    }



                                }
                                count++;
                            }

                            AdmissionCriteriaForOLevelTypeLogic admissionCriteriaForOLevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();

                            for (int i = 0; i < criteriaModel.OLevelTypes.Count; i++)
                            {
                                OLevelType oLevelType = criteriaModel.OLevelTypes[i];
                                AdmissionCriteriaForOLevelType admissionCriteriaForOLevelType = new AdmissionCriteriaForOLevelType();

                                if (oLevelType != null && oLevelType.Id > 0)
                                {
                                    admissionCriteriaForOLevelType.MainCriteria = criteria;
                                    admissionCriteriaForOLevelType.OLevelType = oLevelType;

                                    admissionCriteriaForOLevelTypeLogic.Create(admissionCriteriaForOLevelType);
                                }
                            }
                        }
                    }

                    transaction.Complete();


                    ViewBag.ProgrammeId = criteriaModel.ProgrammeSelectListItem;
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);

                    for (int i = 0; i < 15; i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = criteriaModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelSubjectId" + i] = criteriaModel.OLevelSubjectSelectList;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        ViewData["OlevelTypeId" + i] = criteriaModel.OlevelTypeSelectListItem;
                    }
                }

                SetMessage("Successfully Added criteria", Message.Category.Information);

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            //TempData["Message"] = "Successfully Added criteria";
            return View(criteriaModel);
        }

        public ActionResult EditCriteria(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                AdmissionCriteriaForOLevelSubjectLogic OlevelLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                List<AdmissionCriteriaForOLevelSubjectAlternative> olevelAltList = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                AdmissionCriteriaForOLevelSubjectAlternative olevelAlt = new AdmissionCriteriaForOLevelSubjectAlternative();
                AdmissionCriteriaForOLevelSubjectAlternativeLogic olevelAltLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                AdmissionCriteriaForOLevelTypeLogic olevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();

                viewmodel.admissionCriteriaForOLevelSubject = OlevelLogic.GetModelsBy(a => a.Admission_Criteria_Id == Id);
                int count = viewmodel.admissionCriteriaForOLevelSubject.Count();
                for (int i = 0; i < count; i++)
                {
                    ViewData["FirstSittingOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;
                    ViewData["SecondSittingOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;
                    ViewData["OtherOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;
                    ViewData["FirstSittingOLevelGradeId" + i] = viewmodel.OLevelGradeSelectList;

                }

                foreach (AdmissionCriteriaForOLevelSubject subject in viewmodel.admissionCriteriaForOLevelSubject)
                {
                    subject.Alternatives = olevelAltLogic.GetModelsBy(o => o.Admission_Criteria_For_O_Level_Subject_Id == subject.Id);

                    if (subject.Alternatives.Count > 1)
                    {
                        List<AdmissionCriteriaForOLevelSubjectAlternative> alternativeList = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                        alternativeList.Add(subject.Alternatives[1]);
                        subject.OtherAlternatives = alternativeList;
                    }
                }
                
                //int count = viewmodel.admissionCriteriaForOLevelSubject.Count;

                for (int i = count; i < count + 6; i++)
                {
                    viewmodel.admissionCriteriaForOLevelSubject.Add(new AdmissionCriteriaForOLevelSubject()
                    {
                       Alternatives = new List<AdmissionCriteriaForOLevelSubjectAlternative>(),
                       IsCompulsory = false,
                       MainCriteria = viewmodel.admissionCriteriaForOLevelSubject[0].MainCriteria,
                       MinimumGrade = viewmodel.admissionCriteriaForOLevelSubject[0].MinimumGrade,
                       Subject = new OLevelSubject(),
                       OtherAlternatives = new List<AdmissionCriteriaForOLevelSubjectAlternative>()
                    });
                }

                viewmodel.admissionCriteriaForOLevelType = olevelTypeLogic.GetModelsBy(n => n.Admission_Criteria_Id == Id);
                SetSelectedSittingSubjectAndGrade(viewmodel);

               
                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        private void SetSelectedSittingSubjectAndGrade(ClearanceViewModel existingViewModel)
        {
            try
            {
                if (existingViewModel != null && existingViewModel.admissionCriteriaForOLevelSubject != null && existingViewModel.admissionCriteriaForOLevelSubject.Count > 0)
                {
                    int i = 0;
                    foreach (AdmissionCriteriaForOLevelSubject subject in existingViewModel.admissionCriteriaForOLevelSubject)
                    {
                        if (subject.Subject.Name != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, subject.Subject.Id);
                            if (subject.Alternatives.Count > 0)
                            {
                                ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, subject.Alternatives[0].OLevelSubject.Id);
                            }
                            if (subject.OtherAlternatives != null && subject.OtherAlternatives.Count > 0)
                            {
                                ViewData["OtherOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, subject.OtherAlternatives[0].OLevelSubject.Id);
                            }
                            ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT, subject.MinimumGrade.Id);
                           
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(viewmodel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(viewmodel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["OtherOLevelSubjectId" + i] = new SelectList(viewmodel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT,0);
                          
                        }

                        i++;
                    }
                    
                    AdmissionCriteriaForOLevelSubject sub = new AdmissionCriteriaForOLevelSubject();
                    sub.Id = -1;
                    sub.MainCriteria = existingViewModel.admissionCriteriaForOLevelSubject[0].MainCriteria;
                    if (sub.Alternatives != null)
                    {
                        sub.Alternatives[0].OLevelSubject.Id = -1;
                        //sub.Alternatives[1].OLevelSubject.Id = -1;
                        for (int u = 0; u < 5; u++)
                        {
                            existingViewModel.admissionCriteriaForOLevelSubject.Add(sub);
                        }
                    }
                }
                else if (existingViewModel != null && existingViewModel.admissionCriteriaForOLevelSubjectAlternative != null && existingViewModel.admissionCriteriaForOLevelSubjectAlternative.Count > 0)
                {
                    int i = 0;
                    foreach (AdmissionCriteriaForOLevelSubjectAlternative subject in existingViewModel.admissionCriteriaForOLevelSubjectAlternative)
                    {
                        if (subject.OLevelSubject.Name != null)
                        {
                            ViewData["AlternativeOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, subject.OLevelSubject.Id);

                        }
                        else
                        {
                            ViewData["AlternativeOLevelSubjectId" + i] = new SelectList(viewmodel.OLevelSubjectSelectList, VALUE, TEXT, 0);


                        }

                        i++;
                    }
                }
               
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
   
        [HttpPost]
        public ActionResult EditCriteria(ClearanceViewModel criteriaModel)
        {
           

            //Add subjects
            AdmissionCriteriaForOLevelSubjectLogic criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            criteriaSubjectLogic.Modify(criteriaModel.admissionCriteriaForOLevelSubject);

            SetMessage("Criteria was updated successfully", Message.Category.Information);
            return RedirectToAction("AdmissionCriteria");
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

        public ActionResult EditAlternative(long AdmissionCriteriaSubjectId)
        {

            AdmissionCriteriaForOLevelSubjectLogic OlevelLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            viewmodel = new ClearanceViewModel();
            try
            {
                viewmodel.admissionCriteriaForOLevelSubjectAlternative = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                viewmodel.admissionCriteriaForOLevelSubjectAlternative = criteriaSubjectAlternativeLogic.GetBy(AdmissionCriteriaSubjectId);
                int count = viewmodel.admissionCriteriaForOLevelSubjectAlternative.Count();
                for (int i = 0; i < count; i++)
                {
                    ViewData["AlternativeOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;

                }

                for (int i = count; i < count + 6; i++)
                {
                    viewmodel.admissionCriteriaForOLevelSubjectAlternative.Add(new AdmissionCriteriaForOLevelSubjectAlternative()
                    {
                        OLevelSubject = new OLevelSubject(),
                        Alternative = new AdmissionCriteriaForOLevelSubject()
                        {
                            Id = (int) AdmissionCriteriaSubjectId,
                            Subject = new OLevelSubject() { Name = "New Alternative"}
                        }
                    });
                }

                SetSelectedSittingSubjectAndGrade(viewmodel);
                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        public ActionResult DeleteCriteriaSubject(long AdmissionCriteriaSubjectId)
        {
            long CriteriaId = 0;
            AdmissionCriteriaForOLevelSubjectLogic criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            viewmodel = new ClearanceViewModel();
            try
            {
                AdmissionCriteriaForOLevelSubject subjectCriteria = criteriaSubjectLogic.GetBy(AdmissionCriteriaSubjectId);
                if (subjectCriteria != null && subjectCriteria.Id > 0)
                {
                   CriteriaId = subjectCriteria.MainCriteria.Id;
                   bool deleteAlternativeStatus =  criteriaSubjectAlternativeLogic.DeleteAllAlternatives(subjectCriteria);
                   bool deleteCriteriaStatus = criteriaSubjectLogic.DeleteSubject(subjectCriteria);

                    if (deleteCriteriaStatus)
                    {
                        SetMessage("Criteria Deleted! ", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Criteria couldn't be Deleted! ", Message.Category.Information);
                    }
                    
                    return RedirectToAction("EditCriteria","Clearance",new {Area ="Admin", Id = CriteriaId });
                }
               
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAlternativeCriteriaSubject(long AlternativeSubjectId)
        {
            long CriteriaId = 0;
            AdmissionCriteriaForOLevelSubjectLogic criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            viewmodel = new ClearanceViewModel();
            try
            {
                AdmissionCriteriaForOLevelSubjectAlternative subjectCriteria = criteriaSubjectAlternativeLogic.GetModelBy(a => a.Admission_Criteria_For_O_Level_Subject_Alternative_Id == AlternativeSubjectId);
                if (subjectCriteria != null && subjectCriteria.Id > 0)
                {
                    CriteriaId = subjectCriteria.Alternative.Id;
                    bool deleteAlternativeStatus = criteriaSubjectAlternativeLogic.DeleteAlternative(subjectCriteria);
                    //bool deleteCriteriaStatus = criteriaSubjectLogic.DeleteSubject(subjectCriteria);

                    if (deleteAlternativeStatus)
                    {
                        SetMessage("Criteria Deleted! ", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Criteria couldn't be Deleted! ", Message.Category.Information);
                    }

                    return RedirectToAction("EditAlternative", "Clearance", new { Area = "Admin", AdmissionCriteriaSubjectId = CriteriaId });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditAlternative(ClearanceViewModel criteriaModel)
        {
            //Add subjects
            AdmissionCriteriaForOLevelSubjectAlternativeLogic criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            criteriaSubjectAlternativeLogic.Modify(criteriaModel.admissionCriteriaForOLevelSubjectAlternative);

            SetMessage("Criteria was updated successfully", Message.Category.Information);
            return RedirectToAction("AdmissionCriteria");
        }



        private Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber)
        {
            Payment payment = new Payment();
            PaymentEtranzactLogic etranzactLogic = new PaymentEtranzactLogic();
            PaymentEtranzact etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
            if (etranzactDetails == null || etranzactDetails.ReceiptNo == null)
            {
                PaymentTerminal paymentTerminal = new PaymentTerminal();
                PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == 1 && p.Session_Id == 1);

                etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber, paymentTerminal);
                if (etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if (payment != null && payment.Id > 0)
                    {
                        FeeDetail feeDetail = new FeeDetail();
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                        //if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail.Fee.Amount))
                        //{
                        //    SetMessage("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.", Message.Category.Error);
                        //    payment = null;
                        //    return payment;
                        //}

                    }
                    else
                    {
                        SetMessage("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.", Message.Category.Error);

                    }
                }
                else
                {
                    SetMessage("Confirmation Order Number entered seems not to be valid! Please cross check and try again.", Message.Category.Error);

                }
            }
            else
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                if (payment != null && payment.Id > 0)
                {
                    FeeDetail feeDetail = new FeeDetail();
                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                    if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail.Fee.Amount))
                    {
                        SetMessage("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.", Message.Category.Error);
                        payment = null;
                        return payment;
                    }
                }
                else
                {
                    SetMessage("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.", Message.Category.Error);
                    //return View(viewModel);
                }
            }

            return payment;
        }

       public ActionResult CheckApplicants()
       {
           try
           {
               List<ApplicationForm> Applicants = new List<ApplicationForm>();
               ApplicationFormLogic CheckApplicants = new ApplicationFormLogic();
               Applicants = CheckApplicants.GetAllHndApplicants();
               foreach (ApplicationForm candidate in Applicants)
               {
                   ApplicationForm candidate2 = CheckApplicants.GetModelBy(m => m.Application_Form_Id == candidate.Id);
                   ApplicationForm audit = candidate2;

                   AppliedCourse CandidateAppliedCourse = new AppliedCourse();
                   AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                   CandidateAppliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == candidate2.Person.Id);
                   candidate2.Release = false;
                   
                   AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
                   string rejectReason = admissionCriteriaLogic.EvaluateApplication(CandidateAppliedCourse);
                   if (string.IsNullOrEmpty(rejectReason))
                   {
                       candidate2.Rejected = false;
                       candidate2.RejectReason = "";
                   }
                   else
                   {
                       candidate2.Rejected = true;
                       candidate2.RejectReason = rejectReason;

                      
                   }
                   if (!CheckApplicants.SetRejectReason(candidate2))
                   {
                       candidate2.Remarks = "Reject Reason not set"; //throw new Exception("Reject Reason not set! Please try again.");
                   }
                   using (StreamWriter writer = new StreamWriter("C:\\inetpub\\wwwroot\\log.txt", true))
                   {
                       writer.WriteLine(audit.Number + " " + audit.Rejected + " " + audit.RejectReason + " -  " + candidate2.Rejected + " " + candidate2.RejectReason + " " + candidate2.Remarks);
                   }
               }
              
           }
           catch (Exception ex)
           {
               
               throw ex;
           }
           return View();
       }

        public ActionResult VerifyApplicant()
       {
           ClearanceViewModel viewModel = new ClearanceViewModel();
           try
           {

           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           return View(viewModel);
       }
       [HttpPost]

        public ActionResult VerifyApplicant(ClearanceViewModel viewModel)
       {
           try
           {
               if (!string.IsNullOrEmpty(viewModel.ApplicationNumber))
               {
                   ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                   ApplicationForm applicationForm = applicationFormLogic.GetModelsBy(a => a.Application_Form_Number == viewModel.ApplicationNumber.Trim()).LastOrDefault();
                   if (applicationForm == null)
                   {
                       SetMessage("Application Form does not exist. Check if you typed in the correct number.", Message.Category.Error);
                       return View(viewModel);
                   }

                   return RedirectToAction("ApplicantVerificationPage", new {applicationFormId = applicationForm.Id});
               }
               else
               {
                   SetMessage("Input is null! Kindly check and try again.", Message.Category.Error);
                   return View(viewModel);
               }
           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           return View(viewModel);
       }

        public ActionResult ApplicantVerificationPage(long applicationFormId)
       {
           ClearanceViewModel viewModel = new ClearanceViewModel();
           try
           {
               if (applicationFormId <= 0)
               {
                   SetMessage("Invalid application ID.", Message.Category.Error);
                   return View(viewModel);
               }

               ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
               AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
               OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
               OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
               PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();

               viewModel.applicationForm = applicationFormLogic.GetModelBy(a => a.Application_Form_Id == applicationFormId);
               if (viewModel.applicationForm != null)
               {
                   if (viewModel.applicationForm.VerificationStatus == true)
                   {
                       viewModel.applicationForm.VerificationStatusStr = "Accept";
                   }
                   else if (viewModel.applicationForm.VerificationStatus == false)
                   {
                       viewModel.applicationForm.VerificationStatusStr = "Reject";
                   }

                   viewModel.appliedCourse = appliedCourseLogic.GetModelBy(a => a.Application_Form_Id == applicationFormId && a.Person_Id == viewModel.applicationForm.Person.Id);
                   viewModel.PreviousEducation = previousEducationLogic.GetBy(viewModel.applicationForm);
                   viewModel.FirstSittingOLevelResult = oLevelResultLogic.GetModelsBy(a => a.Person_Id == viewModel.applicationForm.Person.Id && a.Application_Form_Id == viewModel.applicationForm.Id && a.O_Level_Exam_Sitting_Id == 1).LastOrDefault();
                   if (viewModel.FirstSittingOLevelResult != null)
                   {
                       viewModel.FirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(a => a.Applicant_O_Level_Result_Id == viewModel.FirstSittingOLevelResult.Id);
                   }

                   viewModel.SecondSittingOLevelResult = oLevelResultLogic.GetModelsBy(a => a.Person_Id == viewModel.applicationForm.Person.Id && a.Application_Form_Id == viewModel.applicationForm.Id && a.O_Level_Exam_Sitting_Id == 2).LastOrDefault();
                   if (viewModel.SecondSittingOLevelResult != null)
                   {
                       viewModel.SecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(a => a.Applicant_O_Level_Result_Id == viewModel.SecondSittingOLevelResult.Id);
                   }
               }
           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           return View(viewModel);
       }
       [HttpPost]
       public ActionResult ApplicantVerificationPage(ClearanceViewModel viewModel)
       {
           try
           {
               if (viewModel.applicationForm != null)
               {
                   ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                   UserLogic userLogic = new UserLogic();

                   User user = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                   ApplicationForm applicationForm = applicationFormLogic.GetModelBy(a => a.Application_Form_Id == viewModel.applicationForm.Id);

                   if (viewModel.applicationForm.VerificationStatusStr == "Accept")
                   {
                       applicationForm.VerificationStatus = true;
                   }
                   else if (viewModel.applicationForm.VerificationStatusStr == "Reject")
                   {
                       applicationForm.VerificationStatus = false;
                   }

                   applicationForm.VerificationComment = viewModel.applicationForm.VerificationComment;
                   applicationForm.VerificationOfficer = user;

                   applicationFormLogic.Modify(applicationForm);

                   SetMessage("Applicant has been verified.", Message.Category.Information);
               }

           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           return RedirectToAction("VerifyApplicant");
       }
       public ActionResult VerifyApplicantByProgrammeDepartment()
       {
           ClearanceViewModel viewModel = new ClearanceViewModel();
           try
           {
               ViewBag.Departments = new SelectList(new List<Department>(), "Id", "Name");
               ViewBag.Sessions = viewModel.SessionSelectList;
               ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           return View(viewModel);
       }
       [HttpPost]
       public ActionResult VerifyApplicantByProgrammeDepartment(ClearanceViewModel viewModel)
       {
           try
           {
               if (viewModel.Department != null && viewModel.Programme != null && viewModel.Session != null)
               {
                   //AppliedCourseLogic  appliedCourseLogic = new AppliedCourseLogic();
                   AdmissionListLogic admissionListLogic = new AdmissionListLogic();

                   //viewModel.appliedCourseList = appliedCourseLogic.GetModelsBy(a => a.Department_Id == viewModel.Department.Id && a.Programme_Id == viewModel.Programme.Id && a.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == viewModel.Session.Id);
                   viewModel.AdmissionLists = admissionListLogic.GetModelsBy(a => a.Department_Id == viewModel.Department.Id && a.APPLICATION_FORM.APPLICATION_PROGRAMME_FEE.Programme_Id == viewModel.Programme.Id && a.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == viewModel.Session.Id);

               }
           }
           catch (Exception ex)
           {
               SetMessage("Error! " + ex.Message, Message.Category.Error);
           }

           RetainDropdownState(viewModel);

           return View(viewModel);
       }
       public void RetainDropdownState(ClearanceViewModel viewModel)
       {
           try
           {
               DepartmentLogic departmentLogic = new DepartmentLogic();
               SessionLogic sessionLogic = new SessionLogic();
               ProgrammeLogic programmeLogic = new ProgrammeLogic();

               if (viewModel != null)
               {
                   if (viewModel.Session != null)
                   {

                       ViewBag.Sessions = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true), ID, NAME, viewModel.Session.Id);
                   }
                   else
                   {
                       ViewBag.Sessions = viewModel.SessionSelectList;
                   }
                   
                   if (viewModel.Programme != null)
                   {
                       ViewBag.Programmes = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true), ID, NAME, viewModel.Programme.Id);
                   }
                   else
                   {
                       ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                   }

                   if (viewModel.Department != null && viewModel.Programme != null)
                   {
                       ViewBag.Departments = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME, viewModel.Department.Id);
                   }
                   else
                   {
                       ViewBag.Departments = new SelectList(new List<Department>(), ID, NAME);
                   }
               }
           }
           catch (Exception)
           {

               throw;
           }
       }
    }
}