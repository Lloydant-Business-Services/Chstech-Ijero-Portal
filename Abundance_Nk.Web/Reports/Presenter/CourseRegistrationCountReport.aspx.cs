using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CourseRegistrationCountReport : System.Web.UI.Page
    {
        List<Semester> semesters = new List<Semester>();

        SemesterLogic semesterLogic = new SemesterLogic();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    semesters.Add(new Semester() { Id = 0, Name = "-- Select--" });
                    semesters = semesterLogic.GetAll();
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlSemester, semesters, Utility.ID, Utility.NAME);


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

        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedSemester == null || SelectedSemester.Id <= 0)
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
        private void DisplayReportBy(Session session, Semester semester)
        {
            try
            {

                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                var report = courseRegistrationLogic.GetSummary(session, semester);
                string bind_dsStudentCourseRegistrationReport = "ds_RegistrationSummary";
                string reportPath = @"Reports\CourseRegistrationSummary.rdlc";


                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Course Registration Count Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentCourseRegistrationReport.Trim(), report));
                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; ;
            }
        }


        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                DisplayReportBy(SelectedSession, SelectedSemester);

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; ;
            }
        }
    }
}