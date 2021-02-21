
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.Reporting.WebForms;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ApplicationFormSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
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

        private void DisplayReportBy(Session session)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                List<Model.Model.ApplicationFormSummary> applicationFormSummary = applicationFormLogic.GetSummary(session);

                string bind_dsApplicationFormSummary = "dsApplicationFormSummary";
                string reportPath = @"Reports\ApplicationFormSummary.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "Applicantion Form Summary for" + session.Name;
                rv.LocalReport.ReportPath = reportPath;

                if (applicationFormSummary != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsApplicationFormSummary.Trim(), applicationFormSummary));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message; ;
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

                DisplayReportBy(SelectedSession);
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
                if (SelectedSession == null || SelectedSession.Id <= 0)
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