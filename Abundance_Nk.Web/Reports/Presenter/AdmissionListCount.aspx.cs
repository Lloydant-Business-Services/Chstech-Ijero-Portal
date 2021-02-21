using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Configuration;
using System.IO;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ionic.Zip;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AdmissionListCount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    PopulateAllDropDown();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
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
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                List<AdmissionListReportFormat> list = admissionListLogic.GetAdmissionCount(session);

                string bind_dsAdmissionList = "dsAdmissionList";
                string reportPath = @"Reports\AdmissionListCount.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "Admission List";
                rv.LocalReport.ReportPath = reportPath;

                if (list != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAdmissionList.Trim(), list));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private void PopulateAllDropDown()
        {
            try
            {
                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
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
                if (SelectedSession == null || SelectedSession.Id <= 0)
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
                
                DisplayReportBy(SelectedSession);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }   
    }
}