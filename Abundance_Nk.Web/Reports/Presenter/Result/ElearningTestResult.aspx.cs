using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class ElearningTestResult : System.Web.UI.Page
    {
        long courseAllocationId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Request.QueryString["courseAllocationId"] != null)
                    {
                        courseAllocationId = Convert.ToInt64(Request.QueryString["courseAllocationId"]);
                        BuildReport(courseAllocationId);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        private void BuildReport(long courseAllocationId)
        {
            CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
            EAssignmentLogic eAssignmentLogic = new EAssignmentLogic();

            try
            {
                List<ElearningResult> results = null;
                var courseAllocation = courseAllocationLogic.GetModelBy(g => g.Course_Allocation_Id == courseAllocationId);
                if (courseAllocation?.Id > 0)
                {
                    results = eAssignmentLogic.GetEAssignmentResult(courseAllocation);

                    string bind_dsElearningResult = "dsElearningResult";
                    string reportPath = "";
                    reportPath = @"Reports\Result\ElearningTestAverage.rdlc";

                    rptViewer.Visible = true;
                    rptViewer.Reset();
                    rptViewer.LocalReport.DisplayName = "ELearning Result";
                    rptViewer.ProcessingMode = ProcessingMode.Local;
                    rptViewer.LocalReport.ReportPath = reportPath;
                    rptViewer.LocalReport.EnableExternalImages = true;

                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsElearningResult.Trim(), results));
                    rptViewer.LocalReport.Refresh();


                }
            }

            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
    }
}