using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CourseRegistrationDetails : System.Web.UI.Page
    {
        string departmentId;
        string programmeId;
        string sessionId;
        string semesterId;
        string levelId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (Request.QueryString["departmentId"] != null && Request.QueryString["programmeId"] != null && Request.QueryString["sessionId"] != null && Request.QueryString["levelId"] != null && Request.QueryString["semesterId"] != null )
                {
                    departmentId = Request.QueryString["departmentId"];
                    programmeId = Request.QueryString["programmeId"];
                    sessionId = Request.QueryString["sessionId"];
                    levelId = Request.QueryString["levelId"];
                    semesterId = Request.QueryString["semesterId"];

                }

                if (!Page.IsPostBack)
                {
                    LoadReport();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void LoadReport()
        {
            int departmentIdNew = Convert.ToInt32(departmentId);
            int programmeIdNew = Convert.ToInt32(programmeId);
            int sessionIdNew = Convert.ToInt32(sessionId);
            int levelIdNew = Convert.ToInt32(levelId);
            int semesterIdNew = Convert.ToInt32(semesterId);
            BuildCourseRegistrationDetailsList(departmentIdNew, programmeIdNew, levelIdNew, sessionIdNew, semesterIdNew);
        }

        private void BuildCourseRegistrationDetailsList(int departmentId, int programmeId, int levelId, int sessionId, int semesterId)
        {
            try
            {
                Department department = new Department() { Id = departmentId };
                Programme programme = new Programme() { Id = programmeId };
                Level level = new Level() { Id = levelId };
                Session session = new Session() { Id = sessionId };
                Semester semester = new Semester() { Id = semesterId };
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                List<RegisteredCoursesReport> coureRegistrationList = courseRegistrationLogic.GetCourseRegistrationSummary( department, programme, level,session, semester);

                string bind_dsAttendanceList = "dsCourseRegistrationReport";

                string reportPath = @"Reports\CourseRegistrationDetails.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Course Registration Report";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (coureRegistrationList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(), coureRegistrationList));

                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;

            }
        }
  
    }
}