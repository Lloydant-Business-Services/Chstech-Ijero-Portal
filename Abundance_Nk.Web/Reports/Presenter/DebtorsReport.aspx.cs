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
using Ionic.Zip;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class DebtorsReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    //rblSortOption.SelectedIndex = 100;
                    //rblSortOption.SelectedValue ="10l";
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

        private void DisplayReportBy(Session session, Level level,Programme programme, Department department)
        {
            try
            {
                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();
                //PaymentMode paymentMode = new PaymentMode(){Id = Convert.ToInt32(sortOption)};
                List<PaymentView> payments = paymentModeLogic.GetDebtorsList(session, level, programme, department);

                string bind_dsPhotoCard = "dsStudentPayment";
                string reportPath = "";

                
                reportPath = @"Reports\DebtorsReport.rdlc";
                

                rv.Reset();
                rv.LocalReport.DisplayName = "Debtors Report";
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
                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
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

                //rblSortOption.SelectedValue = "101";
                
                DisplayReportBy(SelectedSession, SelectedLevel, Programme, Department);
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
                    departments = departments.OrderBy(d => d.Name).ToList();
                    Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = true;

                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        //protected void rblSortOption_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (rblSortOption.SelectedValue == "101")
        //    {
        //        ddlLevel.Visible = false;
        //    }
        //    else
        //    {
        //        ddlLevel.Visible = true;
        //    }
        //}

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
                //PaymentMode paymentMode = new PaymentMode() { Id = Convert.ToInt32(rblSortOption.SelectedValue) };
                List<PaymentView> payments = paymentModeLogic.GetDebtorsListFull(SelectedSession, SelectedLevel);

                string bind_dsPhotoCard = "dsStudentPayment";
                string reportPath = "";

                reportPath = @"Reports\DebtorsReportAllWithLevel.rdlc";
               
                rv.Reset();
                rv.LocalReport.DisplayName = "School Fees Debtors Report";
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

        //protected void btnBulk_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (SelectedSession == null || SelectedSession.Id <= 0 || Programme == null || Programme.Id <= 0)
        //        {
        //            lblMessage.Text = "For bulk report, you need to select the session and programme!";
        //            return;
        //        }

        //        ProgrammeLogic programmeLogic = new ProgrammeLogic();
        //        LevelLogic levelLogic = new LevelLogic();

        //        Programme = programmeLogic.GetModelBy(p => p.Programme_Id == Programme.Id);

        //        if (Directory.Exists(Server.MapPath("~/Content/temp")))
        //        {
        //            Directory.Delete(Server.MapPath("~/Content/temp"), true);
        //        }

        //        Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

        //        string zipName = "Debtors Report " + Programme.Name + " " + SelectedSession.Name;

        //        List<Department> departments = Utility.GetDepartmentByProgramme(Programme);

        //        departments.RemoveAt(0);

        //        for (int i = 0; i < departments.Count; i++)
        //        {
        //            Model.Model.Department currentDepartment = departments[i];

        //            List<Level> levels = new List<Level>();
        //            if (Programme.Id == 1 || Programme.Id == 2 || Programme.Id == 5)
        //            {
        //                levels.Add(levelLogic.GetModelBy(l => l.Level_Id == 1));
        //                levels.Add(levelLogic.GetModelBy(l => l.Level_Id == 2));
        //            }
        //            if (Programme.Id == 3 || Programme.Id == 4)
        //            {
        //                levels.Add(levelLogic.GetModelBy(l => l.Level_Id == 3));
        //                levels.Add(levelLogic.GetModelBy(l => l.Level_Id == 4));
        //            }

        //            for (int j = 0; j < levels.Count; j++)
        //            {
        //                Level currentLevel = levels[j];

        //                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();
        //                PaymentMode paymentMode = new PaymentMode() { Id = Convert.ToInt32(rblSortOption.SelectedValue) };
        //                List<PaymentView> payments = paymentModeLogic.GetDebtorsList(SelectedSession, currentLevel, Programme, currentDepartment, paymentMode, txtBoxDateFrom.Text, txtBoxDateTo.Text);

        //                if (payments.Count > 0)
        //                {
        //                    Warning[] warnings;
        //                    string[] streamIds;
        //                    string mimeType = string.Empty;
        //                    string encoding = string.Empty;
        //                    string extension = string.Empty;

        //                    string bind_dsPhotoCard = "dsStudentPayment";
        //                    string reportPath = "";

        //                    if (paymentMode.Id == 100)
        //                    {
        //                        reportPath = @"Reports\SecondInstallmentDebtorsReportBulk.rdlc";
        //                    }
        //                    else if (paymentMode.Id == 101)
        //                    {
        //                        reportPath = @"Reports\DebtorsReportBulk.rdlc";
        //                    }

        //                    ReportViewer rptViewer = new ReportViewer();
        //                    rptViewer.Visible = false;
        //                    rptViewer.Reset();
        //                    rptViewer.LocalReport.DisplayName = "Debtors Report";
        //                    rptViewer.ProcessingMode = ProcessingMode.Local;
        //                    rptViewer.LocalReport.ReportPath = reportPath;
        //                    rptViewer.LocalReport.EnableExternalImages = true;
        //                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsPhotoCard.Trim(), payments));

        //                    byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        //                    string path = Server.MapPath("~/Content/temp");
        //                    string savelocation = Path.Combine(path, currentLevel.Name.Replace(" ", "").Trim() + " " + currentDepartment.Name.Replace("/", "_") + ".pdf");
        //                    File.WriteAllBytes(savelocation, bytes);
        //                }
        //            }
        //        }
        //        using (ZipFile zip = new ZipFile())
        //        {
        //            string file = Server.MapPath("~/Content/temp/");
        //            zip.AddDirectory(file, "");
        //            string zipFileName = zipName;
        //            zip.Save(file + zipFileName + ".zip");
        //            string export = "~/Content/temp/" + zipFileName + ".zip";

        //            Response.Redirect(export, false);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = ex.Message;
        //    }
        //}
    }
}