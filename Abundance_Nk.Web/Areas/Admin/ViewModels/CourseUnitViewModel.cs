using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{

    public class CourseUnitViewModel
    {
        public CourseUnitViewModel()
        {

            SessionSelectList = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
        }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Level Level { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public byte Minimum_Unit { get; set; }
        public byte Maximum_Unit { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
    }
}