using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ProgrammeSetUpViewModel
    {
        public ProgrammeSetUpViewModel()
        {
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            levelSelectListItem = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            FacultySelectListItems = Utility.PopulateFacultySelectListItem();
            //DepartmentOpionSelectListItem = Utility.PopulateAllDepartmentOptionSelectListItem();
            SemesterSelectListItems = Utility.PopulateSemesterSelectListItem();
        }
        public Programme programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public ProgrammeDepartment ProgrmProgrammeDepartment { get; set; }
        public SessionSemester SessionSemester { get; set; }
        public Faculty Faculty { get; set; }
        public Level level { get; set; }
        public Course course { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Role Role { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> levelSelectListItem { get; set; }
        //public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> FacultySelectListItems { get; set; }
        public List<SelectListItem> SemesterSelectListItems { get; set; }
        public List<Programme> Programmes { get; set; }
        public List<Department> Departments { get; set; }
        public List<Level> Levels { get; set; }
        public List<Session> Sessions { get; set; }
        public List<Faculty> Faculties { get; set; }
        public List<DepartmentOption> DepartmentOptions { get; set; } 
        public int tableType { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }
        public string EnteredBy { get; set; }
        public string ProgrammeName { get; set; }
        public string ProId { get; set; }
        public string ProgrammeDescription { get; set; }
        public string DepartmentOptionName { get; set; }
        public string DepartmentOptionId { get; set; }
        public string FacultyName { get; set; }
        public string FacultyId { get; set; }
        public string FacultyDescription { get; set; }
        public string SessionName { get; set; }
        public string SessionId { get; set; }
        public string SessionStart { get; set; }
        public string SessionEnd { get; set; }
        public string ProgrammeShortName { get; set; }
        public string DepartmentCode { get; set; }
        public string Description { get; set; }
        public bool Activated { get; set; }
        public string ProgrammeActivated { get; set; }
        public string DepartmentActivated { get; set; }
        public string DepartmentOptionActivated { get; set; }
        public string SessionActivated { get; set; }
        public string SemesterId { get; set; }
        public string SesmesterName { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string SequenceNumber { get; set; }
        public string RegistrationEnded { get; set; }
        public List<Programme> ApplicationProgrammeList { get; set; }
    }
}