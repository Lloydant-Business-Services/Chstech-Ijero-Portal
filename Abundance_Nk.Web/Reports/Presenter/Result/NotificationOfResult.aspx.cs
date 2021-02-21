using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class NotificationOfResult : System.Web.UI.Page
    {
        string strPersonId = "";
        string strSemesterid = "";
        string strSessionId = "";
        string strProgrammeId = "";
        string strDepartmentId = "";
        string strLevelId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!Page.IsPostBack)
                {
                    strPersonId = Request.QueryString["personId"];
                    strSemesterid = Request.QueryString["semesterId"];
                    strSessionId = Request.QueryString["sessionId"];
                    strProgrammeId = Request.QueryString["programmeId"];
                    strDepartmentId = Request.QueryString["departmentId"];
                    strLevelId = Request.QueryString["levelId"];
                    if (!string.IsNullOrEmpty(strPersonId))
                    {
                        long personId = Convert.ToInt64(strPersonId);
                        int semesterId = Convert.ToInt32(strSemesterid);
                        int sessionId = Convert.ToInt32(strSessionId);
                        int programmeId = Convert.ToInt32(strProgrammeId);
                        int departmentId = Convert.ToInt32(strDepartmentId);
                        int levelId = Convert.ToInt32(strLevelId);

                        Student student = new Student() { Id = personId };
                        Semester semester = new Semester() { Id = semesterId };
                        Session session = new Session() { Id = sessionId };
                        Programme programme = new Programme() { Id = programmeId };
                        Department department = new Department() { Id = departmentId };
                        Level level = new Level() { Id = levelId };
                        DisplayReportBy(session, semester, student, department, level, programme);
                    }

                }
            }
            catch (Exception ex)
            {

                lblMessage.Text = ex.Message + ex.InnerException.Message; 
            }
  
        }

        private void DisplayReportBy(Session session, Semester semester, Student student, Department department, Level level, Programme programme)
        {
            try
            {
                List<Abundance_Nk.Model.Model.Result> resultList = null;
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

                    firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                    decimal? firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                    studentResultFirstSemester.GPA = firstSemesterGPA;


                    Semester secondSemester = new Semester() { Id = 2};
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
               




                string bind_dsStudentPaymentSummary = "dsNotificationOfResult";
                string reportPath = @"Reports\Result\NotificationOfResult.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Notification of Result ";
                ReportViewer1.LocalReport.ReportPath = reportPath;
                ReportViewer1.LocalReport.EnableExternalImages = true;

                string appRoot = ConfigurationManager.AppSettings["AppRoot"];

                if (resultList.Count > 0)
                {
                    for(int i = 0; i < resultList.Count; i++)
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
                    
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), resultList));
                    ReportViewer1.LocalReport.Refresh();
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
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
        private List<Abundance_Nk.Model.Model.Result> GetResultList(Session session, Semester semester, Student student, Department department, Level level, Programme programme)
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
    }
}