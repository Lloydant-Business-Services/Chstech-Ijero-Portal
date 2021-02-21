using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Configuration;
using System.Globalization;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models.Intefaces;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class Transcript : System.Web.UI.Page, IReport
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

        public int ReportType
        {
            get { throw new NotImplementedException(); }
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
        public Student Student
        {
            get { return new Student() { Id = Convert.ToInt32(ddlStudent.SelectedValue), Name = ddlStudent.SelectedItem.Text }; }
            set { ddlStudent.SelectedValue = value.Id.ToString(); }
        }

        public Session CurrentSession
        {
            get { return new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue) }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }
        public SessionSemester SelectedSession
        {
            get { return new SessionSemester() { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        public Semester TranscriptSemester { get; set; }
        public Session TranscriptSession { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    ddlStudent.Visible = false;
                    ddlDepartment.Visible = false;
                    PopulateAllDropDown();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void SetCurrentSession()
        {
            try
            {
                CurrentSessionSemesterLogic currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                CurrentSessionSemester currentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();
                CurrentSession = currentSessionSemester.SessionSemester.Session;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void DisplayReportBy(Student student, Programme programme, Department department, Faculty faculty)
        {
            try
            {
                ScoreGradeLogic scoreGradeLogic = new ScoreGradeLogic();
                StudentResultLogic resultLogic = new StudentResultLogic();
                AcademicStandingLogic academicStandingLogic = new AcademicStandingLogic();
                List<Model.Model.Result> results = resultLogic.GetTranscriptBy(student);

                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == SelectedSession.Id);

                TranscriptSemester = sessionSemester.Semester;
                TranscriptSession = sessionSemester.Session;

                StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();

                string graduationDate = "";

                StudentAcademicInformation studentAcademicInformation = studentAcademicInformationLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault();
                if (studentAcademicInformation != null && studentAcademicInformation.TranscriptDate != null)
                {
                    int month = studentAcademicInformation.TranscriptDate.Value.Month;
                    string monthStr = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                    graduationDate = monthStr + ", " + studentAcademicInformation.TranscriptDate.Value.Year;
                }
                //string abbreviations = academicStandingLogic.GetAbbreviations();
                //string scoreGradingKeys = scoreGradeLogic.GetScoreGradingKey();

                decimal CGPA = 0M;
                string graduationStatus = "";
                string remark = "";
                if (results != null && results.Count > 0)
                {
                    CGPA = Convert.ToDecimal(results.Sum(s => s.GPCU)/results.Sum(s => s.CourseUnit));
                    graduationStatus = GetGraduationStatus(CGPA);
                    remark = programme.Id > 2 ? "QUALIFIED FOR THE AWARD OF HIGHER NATIONAL DIPLOMA" : "QUALIFIED FOR THE AWARD OF NATIONAL DIPLOMA";
                }

                string reportPath = @"Reports\Result\ResultTranscript.rdlc";
                //string reportPath = @"Reports\Transcript1.rdlc";
                string bind_ds = "dsMasterSheet";

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

                        result.TranscriptLevel = Level.Name;
                        result.TranscriptSemester = TranscriptSemester.Name;
                        result.TranscriptSession = TranscriptSession.Name;

                        result.GraduationStatus = graduationStatus;
                        result.GraduationDate = graduationDate;
                        result.Remark = remark;
                    }
                }

                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.DisplayName = "Transcript";

                string programmeName = programme.Id > 2 ? "Higher National Diploma" : "National Diploma";
                //ReportParameter programmeParam = new ReportParameter("Programme", programmeName);
                //ReportParameter departmentParam = new ReportParameter("Department", department.Name);
                //ReportParameter facultyParam = new ReportParameter("Faculty", faculty.Name);
                //ReportParameter abbreviationsParam = new ReportParameter("Abbreviations", abbreviations);
                //ReportParameter scoreGradingKeysParam = new ReportParameter("ScoreGradingKeys", scoreGradingKeys);
                //ReportParameter[] reportParams = new ReportParameter[] { departmentParam, facultyParam, programmeParam, abbreviationsParam, scoreGradingKeysParam };

                //rv.LocalReport.SetParameters(reportParams);
                if (results != null && results.Count > 0)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), results));
                    rv.LocalReport.Refresh();
                    //rv.Visible = true;
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

        private string GetGraduationStatus(decimal? CGPA)
        {
            string title = null;
            try
            {
                if (CGPA >= 3.5M && CGPA <= 4.0M)
                {
                    title = "DISTINCTION";
                }
                else if (CGPA >= 3.0M && CGPA <= 3.49M)
                {
                    title = "UPPER CREDIT";
                }
                else if (CGPA >= 2.5M && CGPA <= 2.99M)
                {
                    title = "LOWER CREDIT";
                }
                else if (CGPA >= 2.0M && CGPA <= 2.49M)
                {
                    title = "PASS";
                }
                else if (CGPA < 2.0M)
                {
                    title = "POOR";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return title;
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

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    return;
                }

                rv.LocalReport.DataSources.Clear();

                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == SelectedSession.Id);

                TranscriptSemester = sessionSemester.Semester;
                TranscriptSession = sessionSemester.Session;

                List<Student> students = Utility.GetStudentsBy(Level, Programme, Department, sessionSemester.Session);

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
            catch(Exception ex)
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
                    lblMessage.Text = "Please select Session";
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
                else if (Student == null || Student.Id <= 0)
                {
                    lblMessage.Text = "Please select Student";
                    return;
                }

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetBy(Student, CurrentSession);

                DisplayReportBy(Student, Programme, Department, studentLevel.Department.Faculty);
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

        
    }
}