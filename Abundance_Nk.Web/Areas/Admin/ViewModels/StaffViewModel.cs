using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StaffViewModel
    {
        
        public StaffViewModel()
        {
            //SessionSelectList = Utility.PopulateSessionSelectListItemById(7);
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            AllSessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            LevelList = Utility.GetAllLevels();
            UserSelectList = Utility.PopulateStaffSelectListItem();
            CourseModeSelectList = Utility.PopulateCourseModeSelectListItem();
            ResultTypeSelectList = Utility.PopulateResultTypeSelectListItem();
        }
        public List<SelectListItem> CourseModeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> AllSessionSelectList { get; set; }
        public List<SelectListItem> UserSelectList { get; set; }
        public List<ResultFormat> resultFormatList { get; set; }
        public List<ResultFormat> FailedResultFormatList { get; set; }
        public List<Level> LevelList { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public CourseMode CourseMode { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public User User { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<CourseAllocation> CourseAllocationList { get; set; }
        public long Cid { get; set; }
        public int courseModeId { get; set; }
        public List<CourseAllocation> CourseAllocations { get; set; }
        public List<UploadedCourseFormat> UploadedCourses { get; set; }
        public bool IsAlternate { get; set; }
        public StudentResultType StudentResultType { get; set; }
        public List<SelectListItem> ResultTypeSelectList { get; set; }
    }
}