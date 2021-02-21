using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class SupportViewModel 
    {
        private PaymentLogic paymentLogic;
        private DepartmentLogic departmentLogic;
        private PersonLogic personLogic;
        private OLevelTypeLogic oLevelTypeLogic;
        private OLevelGradeLogic oLevelGradeLogic;
        private OLevelSubjectLogic oLevelSubjectLogic;
        private ResultGradeLogic resultGradeLogic;
        public SupportViewModel()
        {

            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();
            resultGradeLogic = new ResultGradeLogic();
            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();
            paymentLogic = new PaymentLogic();
            personLogic = new PersonLogic();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            ExamYearSelectList = Utility.PopulateExamYearSelectListItem(1990);
            OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
            OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
            OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
            ResultGradeSelectList = Utility.PopulateResultGradeSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            FeeTypeSelectList = Utility.PopulateFeeTypeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            AllSessionSelectList = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectList=Utility.PopulateSemesterSelectListItem();
            LevelList = Utility.GetAllLevels();
            if (Programme!= null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
            PopulateDropdowns();
            StatusSelectListItem = Utility.PopulateApplicantStatusSelectListItem();
            InitialiseOLevelResult();
        }

        [Display(Name="Invoice Number")]
        public string InvoiceNumber { get; set; }
        [RegularExpression("^0[0-9]{10}$", ErrorMessage = "Phone number is not valid")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Pin { get; set; }
        [Display(Name = "Message Content")]
        public string Body { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }  
        public Programme Programme { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> StatusSelectListItem { get; set; }
        public CurrentSessionSemester CurrentSessionSemester { get; set; }
     
        public Abundance_Nk.Model.Model.Applicant Applicant { get; set; }
        public Abundance_Nk.Model.Model.Payment Payment { get; set; }
        public Abundance_Nk.Model.Model.AppliedCourse AppliedCourse { get; set; }
        public Abundance_Nk.Model.Model.Person Person { get; set; }
        public Abundance_Nk.Model.Model.ApplicantJambDetail ApplicantJambDetail { get; set; }
        public List<PersonAudit> personAudit { get; set; }
        public PersonAudit personAuditDetails { get; set; }
        public AppliedCourseAudit appliedCourseAuditDetails { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public List<ApplicantJambDetail> ApplicantJambDetailList { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public List<ResultGrade> ResultGrades { get; set; }
        public List<Model.Model.Student> StudentList { get; set; }
        public StudentPayment StudentPayment { get; set; }
        public RemitaPayment RemitaPayment { get; set; }
        public List<ManualPayment> ManualPayments { get; set; }


        public List<Payment> Payments { get; set; }
        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> ResultGradeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }
        public List<ApplicantResult> ApplicantResults { get; set; }
        public List<AdmissionList> AdmissionLists { get; set; }
        public List<StudentLevel> StudentLevelList { get; set; }

        //public List<StudentLevel> StudentLevels { get; set; }
        public Model.Model.Student Student { get; set; }
        public PaymentHistory PaymentHistory { get; set; }
        public string ApplicationFormNumber { get; set; }
        public List<SessionSemester> SessionSemesters { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        
        public void InitialiseOLevelResult()
        {
            try
            {
                List<OLevelResultDetail> oLevelResultDetails = new List<OLevelResultDetail>();
                OLevelResultDetail oLevelResultDetail1 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail2 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail3 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail4 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail5 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail6 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail7 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail8 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail9 = new OLevelResultDetail();

                OLevelResultDetail oLevelResultDetail11 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail22 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail33 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail44 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail55 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail66 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail77 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail88 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail99 = new OLevelResultDetail();

                FirstSittingOLevelResultDetails = new List<OLevelResultDetail>();
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail1);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail2);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail3);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail4);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail5);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail6);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail7);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail8);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail9);

                SecondSittingOLevelResultDetails = new List<OLevelResultDetail>();
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail11);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail22);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail33);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail44);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail55);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail66);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail77);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail88);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail99);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ApplicationForm GetApplicationFormBy(Person person, Payment payment)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                return applicationFormLogic.GetModelBy(a => a.Person_Id == person.Id && a.Payment_Id == payment.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void PopulateDropdowns()
        {
            try
            {
                
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                ResultGrades = resultGradeLogic.GetAll();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        public StudentLevel studentLevel { get; set; }
        public Abundance_Nk.Model.Model.Applicant Applicants { get; set; }            
        public Abundance_Nk.Model.Model.Student studentModel { get; set; }
        public Level Level { get; set; }
        public string MatricNumber { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public FeeType FeeType { get; set; }
        public List<SelectListItem> FeeTypeSelectList { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<Programme> ProgrammeList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<SelectListItem> SessionSelectList { get;set;}
        public Session Session { get; set; }
        public List<SelectListItem> AllSessionSelectList { get; set; }
        public List<Level> LevelList { get; set; }
        public List<Course> Courses { get; set; }
        public Semester Semester { get; set; }
        public string MatricNumberAlt { get; set; }
        public AdmissionList AdmissionList { get; set; }
        public ChangeOfCourse ChangeOfCourse { get; set; }
        public List<ChangeOfCourse> ChangeOfCourseList { get; set; }

        public List<CourseAllocation> CourseAllocationList { get; set; }
        public List<User> Users { get; set; }
        public Role Role { get; set; }
    }
    public class StaffUser
    {
        public int SN { get; set; }
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }

    }



}