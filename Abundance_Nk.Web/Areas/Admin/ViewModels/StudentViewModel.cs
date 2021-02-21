using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StudentViewModel
    {
        public StudentViewModel()
        {
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            FacultySelectList = Utility.PopulateFacultySelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            StateSelectList = Utility.PopulateStateSelectListItem();
            ModeOfEntrySelectList = Utility.PopulateModeOfEntrySelectListItem();
            StudentStatusSelectList = Utility.PopulateStudentStatusSelectListItem();
            AdmittedSessionSelectList = Utility.PopulateAllSessionSelectListItem();
            CountrySelectList = Utility.PopulateCountrySelectListItem();
            LocalGovernmentSelectList = Utility.PopulateLocalGovernmentSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
        }
        public Programme Programme { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; internal set; }
        public List<SelectListItem> SessionSelectList { get; internal set; }
        public List<SelectListItem> FacultySelectList { get; internal set; }
        public List<SelectListItem> DepartmentSelectList { get; internal set; }
        public ApplicationForm ApplicationForm { get; set; }
        public Person Person { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public StudentAcademicInformation StudentAcademicInformation { get; set; }
        public ModeOfEntry ModeOfEntry { get; set; }
        public ModeOfStudy ModeOfStudy { get; set; }
        public List<SelectListItem> ModeOfEntrySelectList { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Session NewSession { get; set; }
        public Faculty Faculty { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public StudentStatus StudentStatus { get; set; }
        public List<SelectListItem> StudentStatusSelectList { get; set; }
        public Model.Model.Student Student { get; set; }
        public List<SelectListItem> AdmittedSessionSelectList { get; set; }
        public Session AdmittedSession { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public List<SelectListItem> LocalGovernmentSelectList { get; set; }
        public Level Level { get; set; }
        public Level NewLevel { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
        public LocalGovernment LocalGovernment { get; set; }
        public Value Confirmed { get; set; }
        public string MatricNumberWildCard{ get; set; }
        public List<StudentLevel> StudentLevelList { get; set; }
    }
    public class SearchModel
    {
        public string SessionId { get; set; }
        public string ProgrammeId { get; set; }
        public string ApplicationNumber { get; set; }
        public string FirstName { get; set; }
        public string StateId { get; set; }
        public string ModeOfEntryId { get; set; }
        public string DepartmentId { get; set; }
        public string Confirmed { get; set; }
        public string LastName { get; set; }
        public string StatusId { get; set; }
        public string AdmissionSetId { get; set; }
        public string MatricNumber { get; set; }
        public string CountryId { get; set; }
        public string LocalGovernmentId { get; set; }
    }
    public class StudentJsonResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public List<StudentInformation> StudentInformation { get; set; }
    }
}