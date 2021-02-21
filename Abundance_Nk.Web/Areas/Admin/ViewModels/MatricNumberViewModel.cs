using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class MatricNumberViewModel
    {
        public MatricNumberViewModel()
        {
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            DepartmentOptionSelectList = Utility.PopulateDepartmentOptionSelectListItem();
            FacultySelectList = Utility.PopulateFacultySelectListItem();
        }
        public Faculty Faculty { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public StudentMatricNumberAssignment StudentMatricNumberAssignment { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<StudentMatricNumberAssignment> MatricNumberAssignments { get; set; }
        public int Id { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public List<StudentDetailsModel> StudentModels { get; set; }
        public List<SelectListItem> FacultySelectList { get; set; }
        public Value StudentStatus { get; set; }
        public List<Model.Model.Student> Students { get; set; }
        public List<SelectListItem> DepartmentOptionSelectList { get; set; }
        public List<ExcelTemplateModel> ExcelTemplateModels { get; set; }
    }
    public class MatricNumberModel
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Programme { get; set; }
        public string Format { get; set; }
        public string Session { get; set; }
        public string StartFrom { get; set; }
        public string Level { get; set; }
        public string Department { get; set; }
        public string DepartmentOption { get; set; }
        public string DepartmentCode { get; set; }
    }
    
}