using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Web.Mvc;
using System.Data;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ResultUploadViewModel
    {
        private const int ONE = 1;
        private const int ONE_HUNDRED = 100;

        public ResultUploadViewModel()
        {
            Level = new Level();
            Programme = new Programme();
            StudentResultType = new StudentResultType();
            SessionSemester = new SessionSemester();
            Department = new Department();
            MaximumObtainableScore = new Value();

            StudentResult = new StudentResult();
            StudentResult.Programme = new Programme();
            StudentResult.Department = new Department();
            StudentResult.Level = new Level();
            StudentResult.SessionSemester = new SessionSemester();
            StudentResult.Type = new StudentResultType();
            StudentResult.Uploader = new User();

            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            StudentResultTypeSelectList = Utility.PopulateStudentResultTypeSelectListItem();
            SessionSemesterSelectList = Utility.PopulateSessionSemesterSelectListItem();
            MaximumObtainableScores =GetMaximumObtainableScores();
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            StudentTypeSelectList = Utility.PopulateStudentTypeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            AllSessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelList = Utility.GetAllLevels();
        }

        private List<Value> GetMaximumObtainableScores()
        {
            try
            {
                List<Value> obtainableScores = Utility.CreateNumberListFrom(ONE, ONE_HUNDRED);
                if (obtainableScores != null && obtainableScores.Count > 0)
                {
                    obtainableScores.Insert(0, new Value() { Name = "-- Select Max Score Obtainable --" });
                }

                return obtainableScores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public HttpPostedFileBase ExcelFile { get; set; }

        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public StudentResultType StudentResultType { get; set; }
        public StudentResult StudentResult { get; set; }
        public SessionSemester SessionSemester { get; set; }
        public Department Department { get; set; }
        public List<StudentResultDetail> StudentResultDetails { get; set; }
        public List<CourseRegistrationDetail> StudentCourseRegistrationDetails { get; set; }
        public List<Value> MaximumObtainableScores { get; set; }
        public Value MaximumObtainableScore { get; set; }
        public DataTable ExcelData { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> StudentResultTypeSelectList { get; set; }
        public List<SelectListItem> SessionSemesterSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<SelectListItem> MaximumObtainableScoreSelectList { get; set; }
        public List<SelectListItem> StudentTypeSelectList { get; set; }
    
        public Model.Model.Student student { get; set; }
        public string MatricNumber { get; set; }
        public string CourseCode { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> AllSessionSelectList { get; set; }
        public List<Level> LevelList { get; set; }
        public List<ResultFormat> resultFormatList { get; set; }
        public HttpPostedFileBase File { get; set; }
        public StudentType StudentType { get; set; }
        public List<Result> ResultList { get; set; }
        public Result Result { get; set; }
        public string Enable { get; set; }
        public string RejectCategory { get; set; }
        public string Reason { get; set; }

        public string Client { get; set; }
        public List<StudentLevel> StudentLevels { get; set; }

        public DateTime Date { get; set; }
        public bool IsGraduationDate { get; set; } 
        public bool IsTranscriptDate { get; set; } 
    }


}