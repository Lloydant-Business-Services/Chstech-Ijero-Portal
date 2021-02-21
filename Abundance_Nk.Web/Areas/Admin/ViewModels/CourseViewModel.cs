using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;


namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class CourseViewModel
    {
        public CourseViewModel()
        {
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            levelSelectListItem = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
            firstSemesterCourses = new List<Course>();
            secondSemesterCourses = new List<Course>();
        }

        public Programme programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Level level { get; set; }
        public Course course { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Model.Model.Student Student { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> levelSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<Course> firstSemesterCourses  { get; set; }
        public List<Course> secondSemesterCourses { get; set; }
        public List<Course> Courses { get; set; }

        public int FirstSemesterTotalCourseUnit { get; set; }
        public int SecondSemesterTotalCourseUnit { get; set; }
    }

    public class SampleSheetUpload
    {
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public int CourseUnit { get; set; }
        public int SemesterId { get; set; }
    }
}