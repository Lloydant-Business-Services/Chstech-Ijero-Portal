using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StudentResultViewModel
    {
        public StudentResultViewModel()
        {
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
        }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public StudentResultStatus StudentResultStatus { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<Department> Departments { get; set; }
        public List<StudentResultStatusFormat> StudentResultStatusFormats { get; set; }
        public List<StudentResultStatus> StudentResultStatusList { get; set; }
    }

    public class StudentResultStatusFormat
    {
        public int Id { get; set; } 
        public Department Department { get; set; }
        public bool Approved { get; set; }
    }
}