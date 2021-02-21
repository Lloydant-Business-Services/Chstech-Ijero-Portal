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
using Microsoft.Ajax.Utilities;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class StudentCount : System.Web.UI.Page
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
        private void DisplayReportBy(Session session, string sortOption)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                List<StudentDetailFormat> list = new List<StudentDetailFormat>();
                string reportPath = "";

                switch (sortOption)
                {
                    case "1":
                        list = studentLogic.GetStudentCount(session);
                        reportPath = @"Reports\StudentCount.rdlc";
                        break;
                    case "2":
                        list = studentLogic.GetStudentCountByAcceptance(session);
                        reportPath = @"Reports\StudentCountAcceptance.rdlc";
                        break;
                    case "3":
                        list = studentLogic.GetStudentCountByAdmission(session);
                        reportPath = @"Reports\StudentCountAdmission.rdlc";
                        break;
                    case "4":
                        list = studentLogic.GetStudentCountAll(session);
                        reportPath = @"Reports\StudentCountAll.rdlc";
                        break;
                }
                
                string bind_dsAdmissionList = "dsStudent";

                rv.Reset();
                rv.LocalReport.DisplayName = "Student Count";
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
                if (string.IsNullOrEmpty(rblSortOption.SelectedValue))
                {
                    lblMessage.Text = "Please select sort option";
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

                DisplayReportBy(SelectedSession, rblSortOption.SelectedValue);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }        
    }
}