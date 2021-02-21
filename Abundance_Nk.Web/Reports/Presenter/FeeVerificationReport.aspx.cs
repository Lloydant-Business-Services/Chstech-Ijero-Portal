using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class FeeVerificationReport : System.Web.UI.Page
    {
        private List<Department> departments;

        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        public Programme SelectedProgramme
        {
            get
            {
                return new Programme
                {
                    Id = Convert.ToInt32(ddlProgramme.SelectedValue),
                    Name = ddlProgramme.SelectedItem.Text
                };
            }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }

        public Department SelectedDepartment
        {
            get
            {
                return new Department
                {
                    Id = Convert.ToInt32(ddlDepartment.SelectedValue),
                    Name = ddlDepartment.SelectedItem.Text
                };
            }
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        }

        public Level SelectedLevel
        {
            get
            {
                return new Level { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text };
            }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

        public FeeType SelectedFeeType
        {
            get
            {
                return new FeeType { Id = Convert.ToInt32(ddlFeeType.SelectedValue), Name = ddlFeeType.SelectedItem.Text };
            }
            set { ddlFeeType.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlFeeType, Utility.GetAllFeeTypes(), Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedDepartment == null || SelectedDepartment.Id <= 0 || SelectedProgramme == null ||
                    SelectedProgramme.Id <= 0 || SelectedSession == null || SelectedSession.Id <= 0 || SelectedLevel == null || SelectedLevel.Id <= 0 || SelectedFeeType == null || SelectedFeeType.Id <= 0)
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

        private void DisplayReportBy(Session session, Department department, Programme programme, Level level, FeeType feeType)
        {
            try
            {
                var paymentVerificationLogic = new PaymentVerificationLogic();
                List<PaymentVerificationReport> report = paymentVerificationLogic.GetVerificationReport(department, session, programme, level, feeType);
                string bind_dsStudentPaymentSummary = "dsScreeningReport";
                string reportPath = @"Reports\PaymentReports\PaymentVerificationReport.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Payment Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(),
                        report));
                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var programme = new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
            var departmentLogic = new DepartmentLogic();
            departments = departmentLogic.GetBy(programme);
            Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
            ddlDepartment.Visible = true;
        }

        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                DisplayReportBy(SelectedSession, SelectedDepartment, SelectedProgramme, SelectedLevel, SelectedFeeType);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }
    }
}