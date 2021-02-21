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
    public partial class StatementOfSemesterResultAdmin : System.Web.UI.Page, IReport
    {
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
        public Student Student
        {
            get { return new Student() { Id = Convert.ToInt32(ddlStudent.SelectedValue), Name = ddlStudent.SelectedItem.Text }; }
            set { ddlStudent.SelectedValue = value.Id.ToString(); }
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

                    ddlStudent.Visible = false;
                    ddlDepartment.Visible = false;
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
                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlSemester, Utility.GetAllSemesters(), Utility.ID, Utility.NAME);
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

                StudentLogic studentLogic = new StudentLogic();
                Student currentStudent = studentLogic.GetBy(Student.Id);
                if (currentStudent == null || currentStudent.Id <= 0)
                {
                    lblMessage.Text = "No student record found";
                    return;
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester sessionSemester = sessionSemesterLogic.GetBySessionSemester(SelectedSemester.Id, SelectedSession.Id);
                
                DisplayReportBy(sessionSemester, SelectedLevel, Programme, Department, currentStudent);
                       
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
        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    return;
                }

                rv.LocalReport.DataSources.Clear();

                List<Student> students = Utility.GetStudentsBy(SelectedLevel, Programme, Department, SelectedSession);

                if (students != null && students.Count > 0)
                {
                    students.Insert(0, new Student() { Id = 0, FirstName = "-- Select Student --" });
                }

                if (students != null && students.Count > 0)
                {
                    Utility.BindDropdownItem(ddlStudent, students, Utility.ID, "FirstName");
                    ddlStudent.Visible = true;
                    rv.Visible = true;
                }
                else
                {
                    ddlStudent.Visible = false;
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