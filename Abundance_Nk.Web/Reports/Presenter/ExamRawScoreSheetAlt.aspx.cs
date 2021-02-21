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
using Microsoft.AspNet.Identity;
namespace Abundance_Nk.Web.Reports.Presenter
{
   
    public partial class ExamRawScoreSheetAlt : System.Web.UI.Page
    {
        string deptId;
        string progId;
        string sessionId;
        string levelId;
        string semesterId;
        string courseId;
        string courseModeId;

        Session session = null;
        Semester semester = null;
        Course course = null;
        CourseMode courseMode = null;
        Department department = null;
        Level level = null;
        Programme programme = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lblMessage.Text = "";

                    if (Request.QueryString["levelId"] != null && Request.QueryString["semesterId"] != null && Request.QueryString["progId"] != null && Request.QueryString["deptId"] != null && Request.QueryString["sessionId"] != null && Request.QueryString["courseId"] != null && Request.QueryString["courseModeId"] != null)
                    {
                        levelId = Request.QueryString["levelId"];
                        semesterId = Request.QueryString["semesterId"];
                        progId = Request.QueryString["progId"];
                        deptId = Request.QueryString["deptId"];
                        sessionId = Request.QueryString["sessionId"];
                        courseId = Request.QueryString["courseId"];
                        courseModeId = Request.QueryString["courseModeId"];

                        int deptId1 = Convert.ToInt32(deptId);
                        int progId1 = Convert.ToInt32(progId);
                        int sessionId1 = Convert.ToInt32(sessionId);
                        int levelId1 = Convert.ToInt32(levelId);
                        int semesterId1 = Convert.ToInt32(semesterId);
                        int courseId1 = Convert.ToInt32(courseId);
                        int courseModeId1 = Convert.ToInt32(courseModeId);

                        session = new Model.Model.Session() { Id = sessionId1 };
                        semester = new Semester() { Id = semesterId1 };
                        course = new Course() { Id = courseId1 };
                        courseMode = new CourseMode() { Id = courseModeId1 };
                        department = new Department() { Id = deptId1 };
                        level = new Level() { Id = levelId1 };
                        programme = new Programme() { Id = progId1 };

                        DisplayReportBy(session, semester, course, department, level, programme, courseMode);
                    }
                
                }
                
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void DisplayReportBy(Session session, Semester semester,Course course,Department department, Level level,Programme programme, CourseMode  courseMode)
        {
            try
            {
                StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();
                List<ExamRawScoreSheetReport> examRawScoreSheetReportList = studentExamRawScoreSheetResultLogic.GetScoreSheetAltBy(session, semester, course, department, level, programme, courseMode);
                string bind_dsStudentPaymentSummary = "dsExamRawScoreSheet";
                string reportPath = @"Reports\Result\ExamRawScoreSheetAlt.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Examination Raw Score Sheet ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (examRawScoreSheetReportList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), examRawScoreSheetReportList));
                    ReportViewer1.LocalReport.Refresh();
                    ReportViewer1.Visible = true;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; 
            }
        }
        
    }
}