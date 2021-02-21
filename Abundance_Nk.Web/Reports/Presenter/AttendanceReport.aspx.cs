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
    public partial class AttendanceReport : System.Web.UI.Page
    {
        string departmentId;
        string programmeId;
        string sessionId;
        string semesterId;
        string levelId;
        string courseId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (Request.QueryString["departmentId"] != null && Request.QueryString["programmeId"] != null && Request.QueryString["sessionId"] != null && Request.QueryString["levelId"] != null && Request.QueryString["semesterId"] != null && Request.QueryString["courseId"] != null)
                {
                    departmentId = Request.QueryString["departmentId"];
                    programmeId = Request.QueryString["programmeId"];
                    sessionId = Request.QueryString["sessionId"];
                    levelId = Request.QueryString["levelId"];
                    semesterId = Request.QueryString["semesterId"];
                    courseId = Request.QueryString["courseId"];
                    if (!IsPostBack)
                    {
                        int departmentIdNew = Convert.ToInt32(departmentId);
                        int programmeIdNew = Convert.ToInt32(programmeId);
                        int sessionIdNew = Convert.ToInt32(sessionId);
                        int levelIdNew = Convert.ToInt32(levelId);
                        int semesterIdNew = Convert.ToInt32(semesterId);
                        int courseIdNew = Convert.ToInt32(courseId);
                        BuildCarryOverCourseList(departmentIdNew, programmeIdNew, levelIdNew, sessionIdNew, courseIdNew, semesterIdNew);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void BuildCarryOverCourseList(int departmentId, int programmeId, int levelId, int sessionId, int courseId, int semesterId)
        {
            try
            {
                Department department = new Department() { Id = departmentId };
                Programme programme = new Programme() { Id = programmeId };
                Level level = new Level() { Id = levelId };
                Session session = new Session() { Id = sessionId };
                Course course = new Course() { Id = courseId };
                Semester semester = new Semester() { Id = semesterId };
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                List<AttendanceFormat> attendanceList = courseRegistrationDetailLogic.GetCourseAttendanceSheet(session, semester, programme, department, level, course);

                string bind_dsAttendanceList = "dsAttendanceList";

                string reportPath = @"Reports\AttendanceReport.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Attendance Report";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (attendanceList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(), attendanceList));

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