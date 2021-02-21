using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UploadReturningStudentViewModel
    {
        public UploadReturningStudentViewModel()
        {
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            SessionSelectListItem = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            SemesterSelectListItem = Utility.PopulateSemesterSelectListItem();
        }

        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<ReturningStudents> ReturningStudentList { get; set; }
        public List<UploadedStudentModel> UploadedStudents { get; set; }
        public List<UploadedStudentModel> FailedUploads { get; set; }
        public string StudentType { get; set; }
        public Semester Semester { get; set; }
        public List<SelectListItem> SemesterSelectListItem { get; set; }
    }

    public class UploadedStudentModel
    {
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string Session { get; set; }
        public string Reason { get; set; }
    }

    public class SampleReturningStudent
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string DateOfBirth { get; set; }
        //public string Address { get; set; }
        public string State { get; set; }
        //public string LocalGovernmentArea { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public string Sex { get; set; }
        //public string CourseOption { get; set; }
        //public string Programme { get; set; }
        //public string Department { get; set; }
        public string MatricNumberOrApplicationNumber { get; set; }
        //public string Level { get; set; }
    }
    public class SampleReturningStudentWithDetails
    {
        public string SN { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string LocalGovernmentArea { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NextOfKin { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string Country { get; set; }
        public string MaritalStatus { get; set; }
        public string Sex { get; set; }
        public string MatricNumber { get; set; }
    }
    public class SampleSpillOver
    {
        public string SN { get; set; }
        public string Name { get; set; }
        public string MatricNumber { get; set; }
    }
}