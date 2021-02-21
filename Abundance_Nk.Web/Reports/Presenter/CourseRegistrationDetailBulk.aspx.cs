

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CourseRegistrationDetailBulk : System.Web.UI.Page
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblMessage.Text = "";

        //        if (!IsPostBack)
        //        {
        //            LoadReports();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = ex.Message + ex.InnerException.Message;
        //    }
        //}

        //private void LoadReports()
        //{
        //    try
        //    {
        //        ProgrammeLogic programmeLogic = new ProgrammeLogic();
        //        List<Programme> programmes = programmeLogic.GetAll();

        //        if (Directory.Exists(Server.MapPath("~/Content/temp")))
        //        {
        //            Directory.Delete(Server.MapPath("~/Content/temp"), true);
        //        }
        //        Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

        //        string zipName = "CourseRegistrationDetail";

        //        foreach (Programme programme in programmes)
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/Content/temp/" + programme.Name.Replace("/", "") + "/"));
        //            LevelLogic levelLogic = new LevelLogic();
        //            List<Level> levels = levelLogic.GetAll();
        //            foreach (Level level in levels)
        //            {

        //                DepartmentLogic departmentLogic = new DepartmentLogic();
        //                List<Department> departments = departmentLogic.GetBy(programme);


        //                foreach (Department department in departments)
        //                {
        //                    Session session = new Session() { Id = 1 };
        //                    Semester semester = new Semester() { Id = 1 };

        //                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
        //                    List<RegisteredCoursesReport> coureRegistrationList = courseRegistrationLogic.GetCourseRegistrationSummary(department, programme, level, session, semester);

        //                    Warning[] warnings;
        //                    string[] streamIds;
        //                    string mimeType = string.Empty;
        //                    string encoding = string.Empty;
        //                    string extension = string.Empty;

        //                    if (coureRegistrationList.Count > 0)
        //                    {
        //                        string bind_dsAttendanceList = "dsCourseRegistrationReport";
        //                        string reportPath = @"Reports\CourseRegistrationDetails.rdlc";

        //                        ReportViewer rptViewer = new ReportViewer();
        //                        rptViewer.Visible = false;
        //                        rptViewer.Reset();
        //                        rptViewer.LocalReport.DisplayName = "Course Registration Report";
        //                        rptViewer.ProcessingMode = ProcessingMode.Local;
        //                        rptViewer.LocalReport.ReportPath = reportPath;
        //                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(), coureRegistrationList));

        //                        byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        //                        string path = Server.MapPath("~/Content/temp/" + programme.Name.Replace("/", "") + "/");
        //                        string savelocation = Path.Combine(path, programme.Name.Replace("/", "").Replace("(", "").Replace(")", "") + " " + department.Name.Replace("/", "_") + level.Name + ".pdf");
        //                        File.WriteAllBytes(savelocation, bytes);
        //                    }
        //                }

        //            }
        //        }
        //        using (ZipFile zip = new ZipFile())
        //        {
        //            string file = Server.MapPath("~/Content/temp/");
        //            zip.AddDirectory(file, "");
        //            string zipFileName = zipName;
        //            zip.Save(file + zipFileName + ".zip");
        //            string export = "~/Content/temp/" + zipFileName + ".zip";

        //            //UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
        //            //Response.Redirect(urlHelp.Action("DownloadStatementOfResultZip", new { controller = "Result", area = "Admin", downloadName = department.Name }), false);
        //            Response.Redirect(export, false);

        //            return;
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        List<Department> departments;
        readonly SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddllevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlSemester, Utility.GetAllSessionSemesters(), Utility.ID, Utility.NAME);

                    ddlDepartment.Visible = false;
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

        public Level SelectedLevel
        {
            get { return new Level() { Id = Convert.ToInt32(ddllevel.SelectedValue), Name = ddlDepartment.SelectedItem.Text }; }
            set { ddllevel.SelectedValue = value.Id.ToString(); }
        }

        public Semester SelectedSemester
        {
            get { return new Semester() { Id = Convert.ToInt32(ddlSemester.SelectedValue), Name = ddlSemester.SelectedItem.Text }; }
            set { ddllevel.SelectedValue = value.Id.ToString(); }
        }
        private void DisplayReport(Session session, Department department, Programme programme, Level level, Semester semester)
        {
            try
            {
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                var report = courseRegistrationLogic.GetCourseRegistrationSummary(department, programme, level, session, semester);

                string bind_dsAttendanceList = "dsCourseRegistrationReport";
                string reportPath = @"Reports\CourseRegistrationDetails.rdlc";


                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Course Registration  Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(), report));
                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedDepartment == null || SelectedDepartment.Id <= 0
                    || SelectedProgramme == null || SelectedProgramme.Id <= 0 || SelectedLevel == null || SelectedLevel.Id <= 0 || SelectedSemester == null || SelectedSemester.Id <= 0)
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
            Programme programme = new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
            DepartmentLogic departmentLogic = new DepartmentLogic();
            departments = departmentLogic.GetBy(programme);
            if (departments != null && departments.Count > 0)
            {
                departments.Insert(0, new Department() { Id = 0, Name = "-- Select Department --" });
            }
            Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
            ddlDepartment.Visible = true;
        }
        protected void Display_Button_Click(object sender, EventArgs e)
        {
            if (InvalidUserInput())
            {
                lblMessage.Text = "All fields must be selected";
                return;
            }

            SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(ss => ss.Session_Semester_Id == SelectedSemester.Id);
            DisplayReport(SelectedSession, SelectedDepartment, SelectedProgramme, SelectedLevel, sessionSemester.Semester);

        }

        protected void btBulk_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
                    return;
                }
                if (SelectedProgramme == null || SelectedProgramme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return;
                }


                DepartmentLogic departmentLogic = new DepartmentLogic();
                LevelLogic levelLogic = new LevelLogic();


                if (Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"), true);
                }

                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                string zipName = "Course Registration Report ";



                List<Department> programmedepartments = departmentLogic.GetBy(SelectedProgramme);
                List<Level> levels = levelLogic.GetAll();
                List<SessionSemester> sessionSemesters = sessionSemesterLogic.GetModelsBy(ss => ss.Session_Id == SelectedSession.Id);




                foreach (Department department in programmedepartments)
                {
                    foreach (Level level in levels)
                    {

                        foreach (SessionSemester semester in sessionSemesters)
                        {



                            CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                            var report = courseRegistrationLogic.GetCourseRegistrationSummary(department, SelectedProgramme, level, SelectedSession, semester.Semester);

                            if (report.Count > 0)
                            {
                                Warning[] warnings;
                                string[] streamIds;
                                string mimeType = string.Empty;
                                string encoding = string.Empty;
                                string extension = string.Empty;

                                string bind_dsAttendanceList = "dsCourseRegistrationReport";
                                string reportPath = @"Reports\CourseRegistrationDetails.rdlc";

                                ReportViewer rptViewer = new ReportViewer();
                                rptViewer.Visible = false;
                                rptViewer.Reset();
                                rptViewer.LocalReport.DisplayName = "Course Registration Report";
                                rptViewer.ProcessingMode = ProcessingMode.Local;
                                rptViewer.LocalReport.ReportPath = reportPath;
                                rptViewer.LocalReport.EnableExternalImages = true;
                                rptViewer.LocalReport.DataSources.Add(
                                    new ReportDataSource(bind_dsAttendanceList.Trim(),
                                        report));

                                byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding,
                                    out extension, out streamIds, out warnings);

                                string path = Server.MapPath("~/Content/temp");
                                string savelocation = Path.Combine(path,
                                    SelectedProgramme.Name.Replace("/", "_") + " " +
                                    department.Name.Replace("/", "_") + " " +
                                    level.Name.Replace("/", "_") +
                                    ".pdf");
                                File.WriteAllBytes(savelocation, bytes);
                            }
                        }
                    }
                }


                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file, "");
                    string zipFileName = zipName;
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    Response.Redirect(export, false);
                    return;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

    }
}