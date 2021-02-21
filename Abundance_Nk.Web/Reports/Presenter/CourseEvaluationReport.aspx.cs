using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CourseEvaluationReport : System.Web.UI.Page
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
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
        private void DisplayReportBy(SessionSemester session, Level level, Programme programme)
        {
            try
            {
                List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                departments.RemoveAt(0);

                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                Programme thisProgramme = programmeLogic.GetModelBy(p => p.Programme_Id == programme.Id);
                Level thisLevel = levelLogic.GetModelBy(l => l.Level_Id == level.Id);

                if (Directory.Exists(Server.MapPath("~/Content/EvaluationTempFolder")))
                {
                    Directory.Delete(Server.MapPath("~/Content/EvaluationTempFolder"), true);
                    Directory.CreateDirectory(Server.MapPath("~/Content/EvaluationTempFolder"));
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/EvaluationTempFolder"));
                }

                for (int i = 0; i < departments.Count; i++)
                {
                    Department currentDepartment = departments[i];
                    List<Model.Entity.Model.CourseEvaluationReport> reportList = new List<Model.Entity.Model.CourseEvaluationReport>();
                    CourseEvaluationAnswerLogic courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();

                    reportList = courseEvaluationAnswerLogic.GetCourseEvaluationReport(programme, currentDepartment, level, session);

                    if (reportList.Count > 0)
                    {
                        Warning[] warnings;
                        string[] streamIds;
                        string mimeType = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;

                        string bind_dsCourseEvaluationReport = "dsCourseEvaluationReport";
                        string reportPath = @"Reports\CourseEvaluationReport.rdlc";

                        rv.Visible = false;
                        rv.Reset();
                        rv.LocalReport.DisplayName = "Course Evaluation Report";
                        rv.ProcessingMode = ProcessingMode.Local;
                        rv.LocalReport.ReportPath = reportPath;
                        rv.LocalReport.EnableExternalImages = true;
                        rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsCourseEvaluationReport.Trim(), reportList));

                        byte[] bytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string path = Server.MapPath("~/Content/EvaluationTempFolder");
                        string savelocation = Path.Combine(path, currentDepartment.Name.Replace("/", "_") + ".pdf");
                        File.WriteAllBytes(savelocation, bytes); 
                    }

                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/EvaluationTempFolder/");
                    zip.AddDirectory(file, "");
                    string zipFileName = thisProgramme.Name + "_" + thisLevel.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/EvaluationTempFolder/" + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(urlHelp.Action("CourseEvaluationReport", new { controller = "Report", area = "Admin", downloadPath = export, downloadName = zipFileName }), false);

                    //return;
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
                else if (Level == null || Level.Id <= 0)
                {
                    lblMessage.Text = "Please select Level";
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
                
                DisplayReportBy(SelectedSession, Level, Programme);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        
    }
}