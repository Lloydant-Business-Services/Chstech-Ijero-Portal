using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms.VisualStyles;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            SexSelectList = Utility.PopulateSexSelectListItem();
            RoleSelectList = Utility.PopulateRoleSelectListItem();
            SecurityQuestionSelectList = Utility.PopulateSecurityQuestionSelectListItem();
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            DepartmentSelectList = Utility.PopulateAllDepartmentSelectListItem();
            CurrentSessionSelectList = Utility.PopulateSessionSelectListItemById(7);
            ActiveSessionSelectList = Utility.PopulateActiveSessionSelectListItem();
            ExamOfficersAndHODRoleSelectList = Utility.PopulateExamOfficerAndHODSelectListItem();
           
        }
        public User User { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<User> Users { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> RoleSelectList { get; set; }
        public List<SelectListItem> SecurityQuestionSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public Staff Staff { get; set; }
        public StaffDepartment StaffDepartment { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public Department Department { get; set; }
        public List<SelectListItem> CurrentSessionSelectList { get; set; }
        public List<SelectListItem> ActiveSessionSelectList { get; set; }
        public List<SelectListItem> ExamOfficersAndHODRoleSelectList { get; set; }
        public Session Session { get; set; }
        public bool RemoveHOD { get; set; }
        public List<StaffDepartment> StaffDepartments { get; set; }
        public List<UserStaffRecord> UserStaffRecords { get; set; }
        public List<UserUploadFormat> UserUploadFormats { get; set; }
        public bool ShowTable { get; set; }
        public Role Role { get; set; }
    }
    public class UserStaffRecord
    {
        public User User { get; set; }
        public StaffDepartment StaffDepartment { get; set; }
    }
    public class UserUploadFormat
    {
        public int SN { get; set; }
        public string Username { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        
    }
}