using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Configuration;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models.Intefaces;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class CourseResult : System.Web.UI.Page, IReport
    {
        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        public ReportViewer Viewer
        {
            get { return rv; }
            set { rv = value; }
        }

        public int ReportType
        {
            get { throw new NotImplementedException(); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    ddlCourse.Visible = false;
                    ddlDepartment.Visible = false;

                    PopulateAllDropDown();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        public SessionSemester SelectedSession
        {
            get { return new SessionSemester() { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        public Level Level
        {
            get { return new Level() { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

        public Programme Programme
        {
            get { return new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue), Name = ddlProgramme.SelectedItem.Text }; }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }

        public Department Department
        {
            get { return new Department() { Id = Convert.ToInt32(ddlDepartment.SelectedValue), Name = ddlDepartment.SelectedItem.Text }; }
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        }
        
        public Course Course
        {
            get { return new Course() { Id = Convert.ToInt32(ddlCourse.SelectedValue), Name = ddlCourse.SelectedItem.Text }; }
            set { ddlCourse.SelectedValue = value.Id.ToString(); }
        }

        //public int Option
        //{
        //    get { return Convert.ToInt32(rblSortOption.SelectedValue); }
        //    set { rblSortOption.SelectedValue = value.ToString(); }
        //}

        //private void DisplayReportBy(SessionSemester session, Level level, Programme programme, Department department, SortOption sortOption)

        private void DisplayReportBy(SessionSemester session, Level level, Programme programme, Department department, Course course)
        {
            try
            {
                StudentResultLogic resultLogic = new StudentResultLogic();
                List<Model.Model.Result> results = resultLogic.GetStudentResultByCourse(session, level, programme, department, course);
                
                string bind_ds = "dsMasterSheet";
                string reportPath = @"Reports\Result\CourseResult.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "Student Course Result";
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;

                string programmeName = programme.Id > 2 ? "Higher National Diploma" : "National Diploma";
                ReportParameter programmeParam = new ReportParameter("Programme", programmeName);
                ReportParameter departmentParam = new ReportParameter("Department", department.Name);
                ReportParameter sessionSemesterParam = new ReportParameter("SessionSemester", session.Name);
                ReportParameter[] reportParams = new ReportParameter[] { departmentParam, programmeParam, sessionSemesterParam };

                rv.LocalReport.SetParameters(reportParams);

                if (results != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), results));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateAllDropDown()
        {
            try
            {
                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessionSemesters(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
                    return true;
                }
                else if (Programme == null || Programme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return true;
                }
                else if (Department == null || Department.Id <= 0)
                {
                    lblMessage.Text = "Please select Department";
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void btnDisplayReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    return;
                }

                //SortOption sortOption = Option == 2 ? SortOption.ExamNo : Option == 3 ? SortOption.ApplicationNo : SortOption.Name;
                //DisplayReportBy(SelectedSession, Level, Programme, Department, sortOption);

                DisplayReportBy(SelectedSession, Level, Programme, Department, Course);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Programme != null && Programme.Id > 0)
                {
                    PopulateDepartmentDropdownByProgramme(Programme);
                }
                else
                {
                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateDepartmentDropdownByProgramme(Programme programme)
        {
            try
            {
                List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                if (departments != null && departments.Count > 0)
                {
                    Utility.BindDropdownItem(ddlDepartment, Utility.GetDepartmentByProgramme(programme), Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateCourseDropdownBy(Department department, Level level, Semester semester)
        {
            try
            {
                List<Course> courses = Utility.GetCoursesByLevelDepartmentAndSemester(level, department, semester);
                if (courses != null && courses.Count > 0)
                {
                    Utility.BindDropdownItem(ddlCourse, courses, Utility.ID, Utility.NAME);
                    ddlCourse.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Department != null && Department.Id > 0 && Level != null && Level.Id > 0 && SelectedSession != null && SelectedSession.Id > 0)
                {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    SessionSemester sessionSemester = sessionSemesterLogic.GetBy(SelectedSession.Id);

                    PopulateCourseDropdownBy(Department, Level, sessionSemester.Semester);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        





        
    }
}