using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models;
using System.Threading.Tasks;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class StudentPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    rblSortOption.SelectedIndex = 100;
                    ddlDepartment.Visible = false;
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
        public Level SelectedLevel
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

        private void DisplayReportBy(Session session, Level level,Programme programme, Department department, string sortOption)
        {
            try
            {
                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();
                PaymentMode paymentMode = new PaymentMode(){Id = Convert.ToInt32(sortOption)};
                List<PaymentView> payments = new List<PaymentView>();

                payments = paymentModeLogic.GetComprehensivePayment(session, level, programme, department, paymentMode, txtBoxDateFrom.Text, txtBoxDateTo.Text);

                string bind_dsPhotoCard = "dsStudentPayment";
                string reportPath = "";
                switch (Convert.ToInt32(sortOption))
                {
                    case 100:
                        reportPath = @"Reports\ComprehensivePayment.rdlc";
                        break;
                    case 1:
                        reportPath = @"Reports\FullPayment.rdlc";
                        break;
                    case 2:
                        reportPath = @"Reports\FirstInstallment.rdlc";
                        break;
                    case 3:
                        reportPath = @"Reports\FirstSecondInstallment.rdlc";
                        break;
                    case 101:
                        reportPath = @"Reports\NoPayment.rdlc";
                        break;
                }

                rv.Reset();
                rv.LocalReport.DisplayName = "Student School Fees Payment";
                rv.LocalReport.ReportPath = reportPath;

                if (payments != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsPhotoCard.Trim(), payments));
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
                List<Programme> programmes = Utility.GetAllProgrammes();
                List<Level> levels = Utility.GetAllLevels();

                //programmes.Add(new Programme(){ Id = 1001, Name = "All"});
                //levels.Add(new Level(){ Id = 1001, Name = "All"});

                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme, programmes, Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlLevel, levels, Utility.ID, Utility.NAME);
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
                if (Programme == null || Programme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return true;
                }
                if (Department == null || Department.Id <= 0)
                {
                    lblMessage.Text = "Please select Department";
                    return true;
                }
                if (SelectedLevel == null || SelectedLevel.Id <= 0)
                {
                    lblMessage.Text = "Please select Level";
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
                
                DisplayReportBy(SelectedSession, SelectedLevel, Programme, Department, rblSortOption.SelectedValue);
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
                List<Department> departments = new List<Department>();
                //DepartmentLogic departmentLogic = new DepartmentLogic();

                //if (programme.Id == 1001)
                //{
                //    departments = departmentLogic.GetAll();
                //    departments.Insert(0, new Department() { Id = 0, Name = "-- Select Department --" });
                //}
                //else
                //{
                    departments = Utility.GetDepartmentByProgramme(programme);
                //}
                
                if (departments != null && departments.Count > 0)
                {
                    departments = departments.OrderBy(d => d.Name).ToList();
                    //departments.Add(new Department(){Id = 1001, Name = "All"});
                    Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = true;

                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void rblSortOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (rblSortOption.SelectedValue == "101")
            //{
            //    ddlLevel.Visible = false;
            //}
            //else
            //{
            //    ddlLevel.Visible = true;
            //}
        }

        protected void btnFullReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
                    return;
                }

                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();
                PaymentMode paymentMode = new PaymentMode() { Id = Convert.ToInt32(rblSortOption.SelectedValue) };
                List<PaymentView> payments = paymentModeLogic.GetComprehensivePaymentFull(SelectedSession, paymentMode);

                string bind_dsPhotoCard = "dsStudentPayment";
                string reportPath = "";
                switch (Convert.ToInt32(rblSortOption.SelectedValue))
                {
                    case 100:
                        reportPath = @"Reports\ComprehensivePaymentAll.rdlc";
                        break;
                    case 1:
                        reportPath = @"Reports\FullPaymentAll.rdlc";
                        break;
                    case 2:
                        reportPath = @"Reports\FirstInstallmentAll.rdlc";
                        break;
                    case 3:
                        reportPath = @"Reports\FirstSecondInstallmentAll.rdlc";
                        break;
                    case 101:
                        reportPath = @"Reports\NoPaymentAll.rdlc";
                        break;
                }

                rv.Reset();
                rv.LocalReport.DisplayName = "Student School Fees Payment";
                rv.LocalReport.ReportPath = reportPath;

                if (payments != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsPhotoCard.Trim(), payments));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }      

    }
}