using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class NotificationOfResultBulk : System.Web.UI.Page
    {
        List<Department> departments;
        List<Semester> semesters;
        List<SessionSemester> sessionSemesterList;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!Page.IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);

                    ddlDepartment.Visible = false;
                    ddlSemester.Visible = false;
                   
                }
            }
            catch (Exception)
            {
                throw;
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
                Session session = SelectedSession;
                Semester semester = SelectedSemester;
                Programme programme = SelectedProgramme;
                Department department = SelectedDepartment;
                Level level = SelectedLevel;

                if (InvalidUserInput(session, semester, department, level, programme))
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                string downloadPath = "~/Content/temp" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                if (Directory.Exists(Server.MapPath(downloadPath)))
                {
                    Directory.Delete(Server.MapPath(downloadPath), true);
                }
                else
                {
                    DirectoryInfo folder = Directory.CreateDirectory(Server.MapPath(downloadPath));
                    int filesInFolder = folder.GetFiles().Count();
                    if (filesInFolder > 0)
                    {
                        //complete the code
                    }
                }


                List<Abundance_Nk.Model.Model.Result> StudentList = GetResultList(session, semester, department, level, programme);
                foreach (Abundance_Nk.Model.Model.Result item in StudentList)
                {
                    Student student = new Student() { Id = item.StudentId };

                    List<Abundance_Nk.Model.Model.Result> resultList = GetReportList(semester, session, programme, department, level, student);

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    string bind_dsStudentPaymentSummary = "dsNotificationOfResult";
                    string reportPath = @"Reports\Result\NotificationOfResult.rdlc";

                    ReportViewer rptViewer = new ReportViewer();
                    rptViewer.Visible = false;
                    rptViewer.Reset();
                    rptViewer.LocalReport.DisplayName = "Notification Of Result";

                    if (resultList != null && resultList.Count > 0)
                    {
                        string appRoot = ConfigurationManager.AppSettings["AppRoot"];

                        for (int i = 0; i < resultList.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(resultList[i].PassportUrl))
                            {
                                resultList[i].PassportUrl = appRoot + resultList[i].PassportUrl;
                            }
                            else
                            {
                                resultList[i].PassportUrl = appRoot + Utility.DEFAULT_AVATAR;
                            }
                        }

                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.EnableExternalImages = true;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), resultList));

                        byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string path = Server.MapPath(downloadPath);
                        string savelocation = Path.Combine(path, item.Name + ".pdf");
                        File.WriteAllBytes(savelocation, bytes);
                    }
                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath(downloadPath);
                    zip.AddDirectory(file, "");
                    string zipFileName = department.Name.Replace('/', '_');
                    zip.Save(file + zipFileName + ".zip");
                    string export = downloadPath + zipFileName + ".zip";

                    Response.Redirect(export, false);
                    //UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //Response.Redirect(urlHelp.Action("DownloadZip", new { controller = "Result", area = "Admin", downloadName = department.Name }), false);

                    return;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        protected void ddlProgramme_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                Programme programme = new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                departments = departmentLogic.GetBy(programme);
                Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                ddlDepartment.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        protected void ddlSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Session session = new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue) };
                SemesterLogic semesterLogic = new SemesterLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                semesters = new List<Semester>();
                foreach (SessionSemester item in sessionSemesterList)
                {
                    semesters.Add(item.Semester);
                }
                Utility.BindDropdownItem(ddlSemester, semesters, Utility.ID, Utility.NAME);
                ddlSemester.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<Model.Model.Result> GetReportList(Semester semester, Session session, Programme programme, Department department, Level level, Student student)
        {

            List<Abundance_Nk.Model.Model.Result> resultList = null;
            try
            {
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();

                string graduationDate = "";

                StudentAcademicInformation studentAcademicInformation = studentAcademicInformationLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault();
                if (studentAcademicInformation != null && studentAcademicInformation.GraduationDate != null)
                {
                    int month = studentAcademicInformation.GraduationDate.Value.Month;
                    string monthStr = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                    graduationDate = monthStr + ", " + studentAcademicInformation.GraduationDate.Value.Year;
                }

                if (semester.Id == 1)
                {
                    decimal firstYearFirstSemesterGPCUSum = 0;
                    int firstYearFirstSemesterTotalCourseUnit = 0;
                    decimal firstYearSecondSemesterGPCUSum = 0;
                    int firstYearSecondSemesterTotalCourseUnit = 0;

                    Model.Model.Result firstYearFirstSemester = new Model.Model.Result();
                    Model.Model.Result firstYearSecondSemester = new Model.Model.Result();

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                    STUDENT_LEVEL studentLevelEntity = new STUDENT_LEVEL();
                    if (programme.Id > 2)
                    {
                        studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == 3 && s.Person_Id == student.Id && s.Department_Id == department.Id && s.Programme_Id == programme.Id).LastOrDefault();
                    }
                    else
                    {
                        studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == 1 && s.Person_Id == student.Id && s.Department_Id == department.Id && s.Programme_Id == programme.Id).LastOrDefault();
                    }

                    if (studentLevelEntity != null && (level.Id != 1 || level.Id != 3))
                    {
                        firstYearFirstSemester = studentResultLogic.GetFirstYearFirstSemesterResultInfo(level.Id, department, programme, student);
                        if (firstYearFirstSemester != null)
                        {
                            firstYearFirstSemesterGPCUSum = Convert.ToDecimal(firstYearFirstSemester.GPCU);
                            firstYearFirstSemesterTotalCourseUnit = Convert.ToInt32(firstYearFirstSemester.TotalSemesterCourseUnit);
                        }

                        firstYearSecondSemester = studentResultLogic.GetFirstYearSecondSemesterResultInfo(level.Id, department, programme, student);
                        if (firstYearSecondSemester != null)
                        {
                            firstYearSecondSemesterGPCUSum = Convert.ToDecimal(firstYearSecondSemester.GPCU);
                            firstYearSecondSemesterTotalCourseUnit = Convert.ToInt32(firstYearSecondSemester.TotalSemesterCourseUnit);
                        }
                    }

                    List<Abundance_Nk.Model.Model.Result> result = null;
                    result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, student, semester, programme);
                    decimal? firstSemesterGPCUSum = result.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    Abundance_Nk.Model.Model.Result studentResultFirstSemester = new Model.Model.Result();
                    studentResultFirstSemester = result.FirstOrDefault();
                    firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                    decimal? firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                    studentResultFirstSemester.GPA = firstSemesterGPA;
                    studentResultFirstSemester.CGPA = Decimal.Round(Convert.ToDecimal(firstSemesterGPCUSum + firstYearFirstSemesterGPCUSum + firstYearSecondSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit + firstYearFirstSemesterTotalCourseUnit + firstYearSecondSemesterTotalCourseUnit), 2); ;
                    studentResultFirstSemester.StudentTypeName = GetGraduatingDegree(studentResultFirstSemester.ProgrammeId);
                    studentResultFirstSemester.GraduationStatus = GetGraduationStatus(studentResultFirstSemester.CGPA);
                    resultList = new List<Model.Model.Result>();

                    studentResultFirstSemester.GraduationDate = graduationDate;

                    resultList.Add(studentResultFirstSemester);
                }
                else
                {
                    decimal firstYearFirstSemesterGPCUSum = 0;
                    int firstYearFirstSemesterTotalCourseUnit = 0;
                    decimal firstYearSecondSemesterGPCUSum = 0;
                    int firstYearSecondSemesterTotalCourseUnit = 0;

                    Model.Model.Result firstYearFirstSemester = new Model.Model.Result();
                    Model.Model.Result firstYearSecondSemester = new Model.Model.Result();

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                    STUDENT_LEVEL studentLevelEntity = new STUDENT_LEVEL();
                    if (programme.Id > 2)
                    {
                        studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == 3 && s.Person_Id == student.Id && s.Department_Id == department.Id && s.Programme_Id == programme.Id).LastOrDefault();
                    }
                    else
                    {
                        studentLevelEntity = studentLevelLogic.GetEntitiesBy(s => s.Level_Id == 1 && s.Person_Id == student.Id && s.Department_Id == department.Id && s.Programme_Id == programme.Id).LastOrDefault();
                    }

                    if (studentLevelEntity != null && (level.Id != 1 || level.Id != 3))
                    {
                        firstYearFirstSemester = studentResultLogic.GetFirstYearFirstSemesterResultInfo(level.Id, department, programme, student);
                        if (firstYearFirstSemester != null)
                        {
                            firstYearFirstSemesterGPCUSum = Convert.ToDecimal(firstYearFirstSemester.GPCU);
                            firstYearFirstSemesterTotalCourseUnit = Convert.ToInt32(firstYearFirstSemester.TotalSemesterCourseUnit);
                        }

                        firstYearSecondSemester = studentResultLogic.GetFirstYearSecondSemesterResultInfo(level.Id, department, programme, student);
                        if (firstYearSecondSemester != null)
                        {
                            firstYearSecondSemesterGPCUSum = Convert.ToDecimal(firstYearSecondSemester.GPCU);
                            firstYearSecondSemesterTotalCourseUnit = Convert.ToInt32(firstYearSecondSemester.TotalSemesterCourseUnit);
                        }
                    }

                    List<Abundance_Nk.Model.Model.Result> result = null;
                    Semester firstSemester = new Semester() { Id = 1 };
                    result = studentResultLogic.GetStudentProcessedResultBy(session, level, department, student, firstSemester, programme);
                    decimal? firstSemesterGPCUSum = result.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    Abundance_Nk.Model.Model.Result studentResultFirstSemester = new Model.Model.Result();
                    studentResultFirstSemester = result.FirstOrDefault();
                    decimal? firstSemesterGPA = 0M;

                    if (studentResultFirstSemester == null)
                    {
                        firstSemesterTotalSemesterCourseUnit = 0;
                        firstSemesterGPCUSum = 0M;
                    }
                    else
                    {
                        firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                        firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                        studentResultFirstSemester.GPA = firstSemesterGPA;
                    }



                    Semester secondSemester = new Semester() { Id = 2 };
                    Abundance_Nk.Model.Model.Result studentResultSecondSemester = new Model.Model.Result();
                    List<Abundance_Nk.Model.Model.Result> secondSemesterResultList = studentResultLogic.GetStudentProcessedResultBy(session, level, department, student, secondSemester, programme);
                    decimal? secondSemesterGPCUSum = secondSemesterResultList.Sum(p => p.GPCU);
                    studentResultSecondSemester = secondSemesterResultList.FirstOrDefault();

                    studentResultSecondSemester.GPA = Decimal.Round((decimal)(secondSemesterGPCUSum / studentResultSecondSemester.TotalSemesterCourseUnit), 2);
                    studentResultSecondSemester.CGPA = Decimal.Round((decimal)((firstSemesterGPCUSum + secondSemesterGPCUSum + firstYearFirstSemesterGPCUSum + firstYearSecondSemesterGPCUSum) / (studentResultSecondSemester.TotalSemesterCourseUnit + firstSemesterTotalSemesterCourseUnit + firstYearFirstSemesterTotalCourseUnit + firstYearSecondSemesterTotalCourseUnit)), 2);
                    studentResultSecondSemester.StudentTypeName = GetGraduatingDegree(studentResultSecondSemester.ProgrammeId);
                    studentResultSecondSemester.GraduationStatus = GetGraduationStatus(studentResultSecondSemester.CGPA);
                    resultList = new List<Model.Model.Result>();

                    studentResultSecondSemester.GraduationDate = graduationDate;

                    resultList.Add(studentResultSecondSemester);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resultList;
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

        private string GetGraduatingDegree(int? progId)
        {
            try
            {
                if (progId == 1 || progId == 2)
                {
                    return "NATIONAL DIPLOMA";
                }
                else
                {
                    return "HIGHER NATIONAL DIPLOMA";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private List<Abundance_Nk.Model.Model.Result> GetResultList(Session session, Semester semester, Department department, Level level, Programme programme)
        {
            try
            {
                List<Abundance_Nk.Model.Model.Result> filteredResult = new List<Model.Model.Result>();
                StudentResultLogic studentResultLogic = new StudentResultLogic();
                List<string> resultList = studentResultLogic.GetProcessedResutBy(session, semester, level, department, programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Abundance_Nk.Model.Model.Result> result = studentResultLogic.GetProcessedResutBy(session, semester, level, department, programme);
                foreach (string item in resultList)
                {
                    Abundance_Nk.Model.Model.Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }

                return filteredResult.OrderBy(p => p.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }
        private bool InvalidUserInput(Session session, Semester semester, Department department, Level level, Programme programme)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0 || department == null || department.Id <= 0 || programme == null || programme.Id <= 0 || level == null || level.Id <= 0)
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
    }
}