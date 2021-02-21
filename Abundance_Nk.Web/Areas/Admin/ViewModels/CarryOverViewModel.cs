using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class CarryOverViewModel
    {
        public CarryOverViewModel()
        {
            ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            CourseRegistration = new List<CourseRegistration>();
            LevelList = Utility.GetAllLevels();
            Students = new List<Model.Model.Student>();
        }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<Level> LevelList { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Level Level { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public CourseRegistrationDetail CourseRegistrationDetail { get; set; }
        public List<Model.Model.Student> Students { get; set; }
        public List<CourseRegistration> CourseRegistration { get; set; }
    }
}