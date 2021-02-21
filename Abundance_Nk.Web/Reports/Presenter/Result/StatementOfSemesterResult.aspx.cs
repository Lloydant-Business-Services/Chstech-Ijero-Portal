using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Configuration;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models.Intefaces;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class StatementOfSemesterResult : System.Web.UI.Page, IReport
    {
        public Student Student { get; set; }
        public Level SelectedLevel
        {
            get
            {
                return new Level
                {
                    Id = Convert.ToInt32(ddlLevel.SelectedValue),
                    Name = ddlLevel.SelectedItem.Text
                };
            }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }
        public Session SelectedSession
        {
            get
            {
                return new Session
                {
                    Id = Convert.ToInt32(ddlSession.SelectedValue),
                    Name = ddlSession.SelectedItem.Text
                };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }
        public Semester SelectedSemester
        {
            get
            {
                return new Semester
                {
                    Id = Convert.ToInt32(ddlSemester.SelectedValue),
                    Name = ddlSemester.SelectedItem.Text
                };
            }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

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

        public int ReportType
        {
            get { throw new NotImplementedException(); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    PopulateAllDropDown();
                    ddlSession.Visible = true;
                    ddlLevel.Visible = true;
                    ddlSemester.Visible = true;
                    btnDisplayReport.Visible = true;

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private void DisplayReportBy(SessionSemester session, Level level, Programme programme, Department department, Student student)
        {
            try
            {
                StudentResultLogic resultLogic = new StudentResultLogic();
                List<Model.Model.Result> results = resultLogic.GetStudentResultBy(session, level, programme, department, student);

                List<StatementOfResultSummary> resultSummaries = resultLogic.GetStatementOfResultSummaryBy(session, level, programme, department, student);

                string bind_ds = "dsMasterSheet";
                string bind_resultSummary = "dsResultSummary";
                string reportPath = @"Reports\Result\StatementOfResult.rdlc";

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

                rv.Reset();
                rv.LocalReport.DisplayName = "Statement of Result";
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;

                string programmeName = programme.Id > 2 ? "Higher National Diploma" : "National Diploma";

                if (results != null && results.Count > 0)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), results));
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_resultSummary.Trim(), resultSummaries));

                    rv.LocalReport.Refresh();
                    rv.Visible = true;
                }
                else
                {
                    lblMessage.Text = "No result to display";
                    rv.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void PopulateAllDropDown()
        {
            try
            {
                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions().Where(s => s.Activated == true || s.Id == 0), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlSemester, Utility.GetAllSemesters(), Utility.ID, Utility.NAME);
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
                if (SelectedLevel == null || SelectedLevel.Id <= 0)
                {
                    lblMessage.Text = "Please select Level";
                    return true;
                }

                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = " Session not set! Please contact your system administrator.";
                    return true;
                }

                if (SelectedSemester == null || SelectedSemester.Id <= 0)
                {
                    lblMessage.Text = " Semester not set! Please contact your system administrator.";
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

                long sid = Convert.ToInt64(Request["sid"]);
                StudentLogic studentLogic = new StudentLogic();
                Student = studentLogic.GetBy(sid);
                if (Student == null || Student.Id <= 0)
                {
                    lblMessage.Text = "No student record found";
                    return;
                }

                var studentLevelLogic = new StudentLevelLogic();
                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester sessionSemester = sessionSemesterLogic.GetBySessionSemester(SelectedSemester.Id, SelectedSession.Id);
                StudentLevel studentLevel = studentLevelLogic.GetBy(Student.Id);
                if (studentLevel != null)
                {
                    CourseEvaluationAnswerLogic courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                    List<CourseEvaluationAnswer> courseEvaluationAnswers = courseEvaluationAnswerLogic.GetModelsBy(a => a.Person_Id == studentLevel.Student.Id && a.Session_Id == SelectedSession.Id && a.Semester_Id == SelectedSemester.Id);
                    if (courseEvaluationAnswers != null && courseEvaluationAnswers.Count > 0)
                    {
                        StudentResultStatus studentResultStatus = new StudentResultStatus();
                        StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();
                        studentResultStatus = studentResultStatusLogic.GetModelBy(s => s.Department_Id == studentLevel.Department.Id && s.Level_Id == studentLevel.Level.Id && s.Programme_Id == studentLevel.Programme.Id && s.Activated);
                        if (studentResultStatus != null && studentResultStatus.Activated)
                        {
                            DisplayReportBy(sessionSemester, SelectedLevel, studentLevel.Programme, studentLevel.Department, Student);
                        }
                        else
                        {
                            lblMessage.Text = "Result for your department hasn't been released!";
                        }
                    }
                    else
                    {
                        lblMessage.Text = "You are yet to fill the student evaluation form!";
                    }
                }
                else
                {
                    lblMessage.Text = "No result to display";
                    rv.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}