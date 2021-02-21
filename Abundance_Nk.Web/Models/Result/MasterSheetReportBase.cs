using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Models.Result
{
    public class MasterSheetReportBase //: ReportBase
    {
        //protected string reportPath;
        //protected List<Result> results;

        //protected Level level;
        //protected ClassRoom classRoom;
        //protected Session session;

        //protected ReportParameter termParam;
        //protected ReportParameter classParam;
        //protected ReportParameter sessionParam;
        //protected ReportParameter studentCountParam;

        //protected StudentScoreLogic studentScoreLogic;

        //protected IReport report;

        //public MasterSheetReportBase(Level level, ClassRoom classRoom, Session session, IReport report)
        //{
        //    this.level = level;
        //    this.classRoom = classRoom;
        //    this.session = session;
        //    this.report = report;

        //    reportName = "AnnualMasterSheetResult" + level.Name + classRoom.Name; ;

        //    studentScoreLogic = new StudentScoreLogic();
        //}

        //public override void SetPath()
        //{
        //    try
        //    {
        //        reportPath = @"reports\AnnualStudentResultMasterList.rdlc";
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public override void GetData()
        //{
        //    try
        //    {
        //        results = studentScoreLogic.GetReportCardMasterListBy(level, classRoom, session);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
       
        //public override void SetProperties()
        //{
        //    try
        //    {
        //        report.Viewer.Reset();
        //        report.Viewer.LocalReport.DisplayName = reportName;
        //        report.Viewer.LocalReport.ReportPath = reportPath;
        //    }
        //    catch (Exception ex)
        //    {
        //        report.Message = ex.Message;
        //    }
        //}

        //public override void DisplayHelper()
        //{
        //    try
        //    {
        //        string bind_dsResultMasterList = "dsResultMasterList";

        //        if (results != null)
        //        {
        //            report.Viewer.ProcessingMode = ProcessingMode.Local;
        //            report.Viewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsResultMasterList.Trim(), results));
        //            report.Viewer.LocalReport.Refresh();
        //            report.Viewer.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        report.Message = ex.Message;
        //    }
        //}

        //public override void SetParameter()
        //{
        //    try
        //    {
        //        int studentCount = results.GroupBy(r => r.Name).Distinct().Count();

        //        classParam = new ReportParameter("Class", level.Name + " " + classRoom.Name);
        //        sessionParam = new ReportParameter("Session", session.Name);
        //        termParam = new ReportParameter("Term", "ANNUAL");
        //        studentCountParam = new ReportParameter("StudentCount", studentCount.ToString());

        //        ReportParameter[] reportParams = new ReportParameter[] { classParam, sessionParam, termParam, studentCountParam };

        //        report.Viewer.LocalReport.SetParameters(reportParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        report.Message = ex.Message;
        //    }
        //}

        //public override bool NoResultFound()
        //{
        //    try
        //    {
        //        if (results == null || results.Count <= 0)
        //        {
        //            report.Message = "No record found for this selection!";

        //            //clear report
        //            report.Viewer.ProcessingMode = ProcessingMode.Local;
        //            report.Viewer.LocalReport.DataSources.Clear();
        //            report.Viewer.LocalReport.Refresh();

        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


    }
}