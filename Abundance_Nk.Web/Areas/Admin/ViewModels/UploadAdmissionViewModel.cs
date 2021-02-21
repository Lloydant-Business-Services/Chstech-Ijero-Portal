using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UploadAdmissionViewModel
    {
        public AppliedCourseLogic appliedCourseLogic;
        public PersonLogic personLogic;
        private DepartmentLogic departmentLogic;
        public UploadAdmissionViewModel()
        {
            appliedCourseLogic = new AppliedCourseLogic();
            personLogic = new PersonLogic();
            departmentLogic = new DepartmentLogic();
            SessionSelectListItem = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
            if (Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
                
            }
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
           
            
        }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session CurrentSession { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Level Level { get; set; }
        [Display(Name="Admission List Type")]
        public AdmissionListType AdmissionListType { get; set; }
        public AdmissionListBatch AdmissionListBatch { get; set; }
        public List<SelectListItem> AdmissionListTypeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public Person Person { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<AdmissionList> AdmissionList { get; set; }
        public AdmissionList AdmissionListDetail { get; set; }
        public List<AppliedCourse> AppliedCourseList { get; set; }
        public List<AdmissionList> AdmiissionLists { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        
        [Display(Name="Exam No / Application No")]
        public string SearchString { get; set; }
        public bool IsBulk { get; set; }

    }
    public class SampleAdmissionUpload
    {
        public string SN { get; set; }
        public string ApplicationNumber { get; set; }
    }
}