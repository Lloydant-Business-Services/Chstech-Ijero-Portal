using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StudentCourseRegistrationViewModel
    {
        public StudentCourseRegistrationViewModel()
        {
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
            LevelList = Utility.GetAllLevels();
        }
        public List<StudentDeferementLog> StudentDeferementLogs { get; set; }
        public StudentDeferementLog StudentDeferementLog { get; set; }
        public Course Course { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public int OptionId { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Semester Semester { get; set; }

        [Display(Name = "Matric Number/Application Number")]
        public string MatricNumber { get; set; }

        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
        public CourseRegistrationDetail CourseRegistrationDetail { get; set; }
        public List<Level> LevelList { get; set; }
        public List<Course> Courses { get; set; }
        public List<Payment> Payments { get; set; }
        public List<StudentLevel> StudentLevelList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<CourseRegistration> CourseRegistrations { get; set; }
        public List<PaymentEtranzact> PaymentEtranzacts { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public Decimal Amount { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Programme")]
        public int ProgrammeId { get; set; }

        [Display(Name = "Session")]
        public int SessionId { get; set; }

        public List<StudentDetailViewModel> StudentDetails { get; set; } = new List<StudentDetailViewModel>();

        public UpdateRegistrationViewModel UpdateRegistration { get; set; }

        public List<UpdateRegistrationViewModel> UpdateRegistrationList { get; set; } = new List<UpdateRegistrationViewModel>();
    }

    public class JsonPostResult
    {
        public bool IsError { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }


    public class UpdateRegistrationViewModel
    {
        public long PersonId { get; set; }

        //BIO DATA
        public string ImageFile { get; set; }

        public string MatricNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Other Name")]
        public string OtherName { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        public byte Sex { get; set; }

        public string SexName { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        //PERSONAL DATA
        public int Nationality { get; set; }

        public string NationalityName { get; set; }

        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; }

        [Display(Name = "Select State")]
        public string StateId { get; set; }

        public string StateName { get; set; }

        [Display(Name = "Parent/Guardian")]
        public string ParentGuardian { get; set; }

        [Display(Name = "Select Relationship")]
        public int RelationshipId { get; set; }

        public string RelationshipName { get; set; }

        //DEPARTMENTAL DATA
        [Display(Name = "School Name")]
        public string SchoolName { get; set; }

        [Display(Name = "Head Of Department")]
        public int HOD { get; set; }

        public string HODName { get; set; }

        [Display(Name = "Select Department")]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        [Display(Name = "Select Level")]
        public int LevelId { get; set; }

        public string LevelName { get; set; }

        [Display(Name = "Select Session")]
        public int SessionId { get; set; }

        public string SessionName { get; set; }

        [Display(Name = "Course Of Study")]
        public int Course { get; set; }

        [Display(Name = "Year Of Admission")]
        public int YearOfAdmission { get; set; }

        //[Display(Name = "Matric Number")]
        //public string MatricNumber { get; set; }
    }

    public class StudentDetailViewModel
    {
        public long PersonId { get; set; }

        public string StudentName { get; set; }

        public string MatricNumber { get; set; }

        public string ProgrammeName { get; set; }

        public string DepartmentName { get; set; }

        public string SessionName { get; set; }
    }
}