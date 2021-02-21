using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class StudentWithoutResult : System.Web.UI.Page
    {
        List<Department> departments;
        List<Semester> semesters = new List<Semester>();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                
                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);

                    ddlSemester.Visible = false;
                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);

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
        public Level SelectedLevel
        {
            get { return new Level() { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
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

                DisplayReportBy(SelectedSession, SelectedSemester, SelectedDepartment, SelectedLevel, SelectedProgramme);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; ;
            }
        }
        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedDepartment == null || SelectedDepartment.Id <= 0 || SelectedProgramme == null || SelectedProgramme.Id <= 0)
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
        private void DisplayReportBy(Session session, Semester semester, Department department, Level level, Programme programme)
        {
            try
            {
                StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();
                List<ExamRawScoreSheetReport> studentList = studentExamRawScoreSheetResultLogic.GetStudentsWithoutResult(session, semester, department, level, programme);
                string bind_dsStudentPaymentSummary = "dsExamRawScoreSheet";
                string reportPath = @"Reports\Result\StudentWithoutResult.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Report of students without result";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (studentList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), studentList));
                    ReportViewer1.LocalReport.Refresh();
                    ReportViewer1.Visible = true;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }



        protected void ddlProgramme_SelectedIndexChanged1(object sender, EventArgs e)
        {
            Programme programme = new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
            DepartmentLogic departmentLogic = new DepartmentLogic();
            departments = departmentLogic.GetBy(programme);
            departments.Insert(0, new Department() { Name = "-- Select Department--" });
            Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);

            ddlDepartment.Visible = true;
        }


        protected void ddlSession_SelectedIndexChanged1(object sender, EventArgs e)
        {
            Session session = new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue) };
            SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
            List<SessionSemester> semesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);
            foreach (SessionSemester sessionSemester in semesterList)
            {

                semesters.Add(sessionSemester.Semester);
            }
            semesters.Insert(0, new Semester() { Name = "-- Select Semester--" });
            Utility.BindDropdownItem(ddlSemester, semesters, Utility.ID, Utility.NAME);
            ddlSemester.Visible = true;
        }

    }
}