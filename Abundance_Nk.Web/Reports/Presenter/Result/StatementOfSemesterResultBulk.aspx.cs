using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class StatementOfSemesterResultBulk : System.Web.UI.Page
    {
        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    PopulateAllDropDown();
                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
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
        public SessionSemester SelectedSession
        {
            get { return new SessionSemester() { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
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
                if (Level == null || Level.Id <= 0)
                {
                    lblMessage.Text = "Please select Level";
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
                else if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = " Session not set! Please contact your system administrator.";
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
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                Programme programme = Programme;
                Department department = Department;
                Level level = Level;

                SessionSemester sessionSemester = sessionSemesterLogic.GetBy(SelectedSession.Id);

                if (InvalidUserInput())
                {
                    return;
                }
                string downloadPath = "~/Content/temp" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                if (Directory.Exists(Server.MapPath(downloadPath)))
                {
                    Directory.Delete(Server.MapPath(downloadPath), true);
                }
                Directory.CreateDirectory(Server.MapPath(downloadPath));

                List<Abundance_Nk.Model.Model.Result> StudentList = GetResultList(sessionSemester.Session, sessionSemester.Semester, department, level, programme);
                foreach (Abundance_Nk.Model.Model.Result item in StudentList)
                {
                    Student student = new Student() { Id = item.StudentId };

                    StudentLevel studentLevel = studentLevelLogic.GetBy(student, sessionSemester.Session);
                    if (studentLevel != null)
                    {
                        ScoreGradeLogic scoreGradeLogic = new ScoreGradeLogic();
                        StudentResultLogic resultLogic = new StudentResultLogic();
                        AcademicStandingLogic academicStandingLogic = new AcademicStandingLogic();
                        List<Model.Model.Result> results = resultLogic.GetStudentResultBy(SelectedSession, studentLevel.Level, studentLevel.Programme, studentLevel.Department, student);

                        List<StatementOfResultSummary> resultSummaries = resultLogic.GetStatementOfResultSummaryBy(SelectedSession, studentLevel.Level, studentLevel.Programme, studentLevel.Department, student);

                        Warning[] warnings;
                        string[] streamIds;
                        string mimeType = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;

                        string bind_ds = "dsMasterSheet";
                        string bind_resultSummary = "dsResultSummary";
                        string reportPath = @"Reports\Result\StatementOfResultAdmin.rdlc";

                        if (results != null && results.Count > 0)
                        {
                            string appRoot = ConfigurationManager.AppSettings["AppRoot"];
                            foreach (Model.Model.Result result in results)
                            {
                                if (!string.IsNullOrWhiteSpace(result.PassportUrl))
                                {
                                    result.PassportUrl = appRoot + result.PassportUrl;
                                }
                                else
                                {
                                    result.PassportUrl = appRoot + Utility.DEFAULT_AVATAR;
                                }
                            }
                        }

                        ReportViewer rptViewer = new ReportViewer();
                        rptViewer.Visible = false;
                        rptViewer.Reset();
                        rptViewer.LocalReport.DisplayName = "Statement Of Result";
                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.EnableExternalImages = true;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), results));
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_resultSummary.Trim(), resultSummaries));

                        byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string path = Server.MapPath(downloadPath);
                        string savelocation = Path.Combine(path, item.Name + ".pdf");
                        File.WriteAllBytes(savelocation, bytes);
                    }
                    else
                    {
                        lblMessage.Text = "No result to display";
                    }
                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath(downloadPath);
                    zip.AddDirectory(file, "");
                    string zipFileName = department.Name.Replace("/", "_");
                    zip.Save(file + zipFileName + ".zip");
                    string export = downloadPath + zipFileName + ".zip";

                    Response.Redirect(export, false);
                    //UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //Response.Redirect(urlHelp.Action("DownloadStatementOfResultZip", new { controller = "Result", area = "Admin", downloadName = department.Name }), false);

                    return;
                }
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
        private List<Abundance_Nk.Model.Model.Result> GetResultList(Session session, Semester semester, Department department, Level level, Programme programme)
        {
            try
            {
                List<Abundance_Nk.Model.Model.Result> filteredResult = new List<Model.Model.Result>();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<string> resultList = studentResultLogic.GetProcessedResutBy(session, semester, level, department, programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Abundance_Nk.Model.Model.Result> result = studentResultLogic.GetProcessedResutBy(session, semester, level, department, programme);
                foreach (string item in resultList)
                {
                    Abundance_Nk.Model.Model.Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }

                return filteredResult.OrderBy(p => p.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }
        
    }
}