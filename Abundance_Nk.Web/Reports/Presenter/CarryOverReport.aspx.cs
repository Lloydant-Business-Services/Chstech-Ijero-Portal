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
    public partial class CarryOverReport : System.Web.UI.Page
    {
        string deptId;
        string progId;
        string sessionId;
        string semesterId;
        string levelId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (Request.QueryString["deptId"] != null && Request.QueryString["progId"] != null && Request.QueryString["sessionId"] != null && Request.QueryString["semesterId"] != null && Request.QueryString["levelId"] != null)
                {
                    deptId = Request.QueryString["deptId"];
                    progId = Request.QueryString["progId"];
                    sessionId = Request.QueryString["sessionId"];
                    semesterId = Request.QueryString["semesterId"];
                    levelId = Request.QueryString["levelId"];
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            int deptId1 = Convert.ToInt32(deptId);
            int progId1 = Convert.ToInt32(progId);
            int sessionId1 = Convert.ToInt32(sessionId);
            int semesterId1 = Convert.ToInt32(semesterId);
            int levelId1 = Convert.ToInt32(levelId);
            BuildCarryOverCourseList(deptId1, progId1, levelId1, sessionId1, semesterId1 );
        }
        private void BuildCarryOverCourseList(int deptId, int progId, int levelId, int sessionId, int semesterId)
        {
            try
            {
                Department department = new Department() { Id = deptId };
                Programme programme = new Programme() { Id = progId };
                Level level = new Level() { Id = levelId };
                Session session = new Session() { Id = sessionId };
                Semester semester = new Semester() { Id = semesterId }; 
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                List<CarryOverReportModel> carryOverList = courseRegistrationLogic.GetCarryOverList(department, programme, session, level, semester);
                
                string bind_dsCarryOverStudentList = "dsCarryOverCourses";
                
                string reportPath = @"Reports\CarryOverReport.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Carry Over Courses ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (carryOverList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsCarryOverStudentList.Trim(), carryOverList));
                    
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