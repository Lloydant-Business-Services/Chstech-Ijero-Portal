using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;
using System.Net.Mime;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AttendanceReportBulk : System.Web.UI.Page
    {

        List<Department> departments;
        List<Semester> semesters;
        List<SessionSemester> sessionSemesterList;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);

                    ddlDepartmentOption.Visible = false;
                    ddlDepartment.Visible = false;
                    ddlSemester.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        public Session SelectedSession
        {
            get { return new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }
        public Semester SelectedSemester
        {
            get { return new Semester() { Id = Convert.ToInt32(ddlSemester.SelectedValue), Name = ddlSemester.SelectedItem.Text }; }
            set { ddlSemester.SelectedValue = value.Id.ToString(); }
        }
        public Programme SelectedProgramme
        {
            get { return new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue), Name = ddlProgramme.SelectedItem.Text }; }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }
        public Department SelectedDepartment
        {
            get { return new Department() { Id = Convert.ToInt32(ddlDepartment.SelectedValue), Name = ddlDepartment.SelectedItem.Text }; }
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        }
        public DepartmentOption SelectedDepartmentOption
        {
            get { return new DepartmentOption() { Id = Convert.ToInt32(ddlDepartmentOption.SelectedValue), Name = ddlDepartmentOption.SelectedItem.Text }; }
            set { ddlDepartmentOption.SelectedValue = value.Id.ToString(); }
        }
        public Level SelectedLevel
        {
            get { return new Level() { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }


        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedSemester == null || SelectedSemester.Id <= 0 || SelectedDepartment == null || SelectedDepartment.Id <= 0 || SelectedProgramme == null || SelectedProgramme.Id <= 0 || SelectedLevel == null || SelectedLevel.Id <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void ddlProgramme_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                Programme programme = new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                departments = departmentLogic.GetBy(programme);
                Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                ddlDepartment.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            try
            {
                Session session = SelectedSession;
                Semester semester = SelectedSemester;
                Programme programme = SelectedProgramme;
                Department department = SelectedDepartment;
                DepartmentOption departmentOption = SelectedDepartmentOption;
                Level level = SelectedLevel;
                
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                
                if (Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"), true);
                }
                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                List<Course> courseList = new List<Course>();

                if (departmentOption != null && departmentOption.Id > 0)
                {
                    courseList = Utility.GetCoursesByOptionLevelDepartmentAndSemester(departmentOption, level, department, semester);
                }
                else
                {
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level, department, semester);
                }
                
                foreach (Course course in courseList)
                {
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    var report = courseRegistrationDetailLogic.GetCourseAttendanceSheet(session, semester, programme, department, level, course);

                    if (report.Count > 0)
                    {
                        string bind_dsAttendanceList = "dsAttendanceList";
                        string reportPath = @"Reports\AttendanceReport.rdlc";

                        ReportViewer rptViewer = new ReportViewer();
                        rptViewer.Visible = false;
                        rptViewer.Reset();
                        rptViewer.LocalReport.DisplayName = "Attendance Sheet";
                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(), report));

                        byte[] bytes = rptViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string path = Server.MapPath("~/Content/temp");
                        string savelocation = "";
                        if (departmentOption != null && departmentOption.Id > 0)
                        {
                            savelocation = Path.Combine(path, course.Code + "_" + departmentOption.Name.Replace("/", "_") + ".xls");
                        }
                        else
                        {
                            savelocation = Path.Combine(path, course.Code + ".xls");
                        }
                         
                        File.WriteAllBytes(savelocation, bytes);
                    }

                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file, "");
                    string zipFileName = SelectedDepartment.Name.Replace("/", "_");
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(urlHelp.Action("DownloadZip", new { controller = "StaffCourseAllocation", area = "Admin", downloadName = SelectedDepartment.Name.Replace("/", "_") }), false);

                    return;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; 
            }
        }
        protected void ddlSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Session session = new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue) };
                SemesterLogic semesterLogic = new SemesterLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                semesters = new List<Semester>();
                foreach (SessionSemester item in sessionSemesterList)
                {
                    semesters.Add(item.Semester);
                }
                Utility.BindDropdownItem(ddlSemester, semesters, Utility.ID, Utility.NAME);
                ddlSemester.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (SelectedProgramme != null && SelectedProgramme.Id > 0 && SelectedDepartment != null && SelectedDepartment.Id > 0)
                {
                    PopulateDepartmentOptionDropdownByDepartment(SelectedDepartment, SelectedProgramme);
                }
                else
                {
                    ddlDepartmentOption.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private void PopulateDepartmentOptionDropdownByDepartment(Department department, Programme programme)
        {
            try
            {
                List<DepartmentOption> departments = Utility.GetDepartmentByOptionByDepartmentProgramme(department, programme);
                if (departments != null && departments.Count > 0)
                {
                    Utility.BindDropdownItem(ddlDepartmentOption, Utility.GetDepartmentByOptionByDepartmentProgramme(department, programme), Utility.ID, Utility.NAME);
                    ddlDepartmentOption.Visible = true;
                }
                else
                {
                    DepartmentOption departmentOption = new DepartmentOption() { Id = -1, Name = "No item to be displayed" };
                    List<DepartmentOption> emptyList = new List<DepartmentOption>();
                    emptyList.Add(departmentOption);
                    Utility.BindDropdownItem(ddlDepartmentOption, emptyList, Utility.ID, Utility.NAME);

                    ddlDepartmentOption.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}