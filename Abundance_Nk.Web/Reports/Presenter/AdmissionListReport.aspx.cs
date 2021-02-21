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
    public partial class AdmissionListReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
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

        private void DisplayReportBy(Session session, Programme programme, Department department)
        {
            try
            {
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                List<AdmissionListReportFormat> list = admissionListLogic.GetListBy(session, programme, department);

                string bind_dsAdmissionList = "dsAdmissionList";
                string reportPath = @"Reports\AdmissionList.rdlc";

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
                Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
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
                
                DisplayReportBy(SelectedSession, Programme, Department);
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

        protected void btnBulkReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
                    return;
                }

                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();

                List<Programme> programmes = programmeLogic.GetAll();
                
                if (Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"), true);
                }

                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                string zipName = "Admission List Report";

                for (int i = 0; i < programmes.Count; i++)
                {
                    Programme currentProgramme = programmes[i];
                    List<Department> departments = departmentLogic.GetBy(currentProgramme);

                    for (int j = 0; j < departments.Count; j++)
                    {
                        Department currentDepartment = departments[j];
                        AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                        List<AdmissionListReportFormat> list = admissionListLogic.GetListBy(SelectedSession, currentProgramme, currentDepartment);
                        
                        if (list.Count > 0)
                        {
                            Warning[] warnings;
                            string[] streamIds;
                            string mimeType = string.Empty;
                            string encoding = string.Empty;
                            string extension = string.Empty;

                            string bind_dsAdmissionList = "dsAdmissionList";
                            string reportPath = @"Reports\AdmissionList.rdlc";

                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.Visible = false;
                            rptViewer.Reset();
                            rptViewer.LocalReport.DisplayName = "Admission List";
                            rptViewer.ProcessingMode = ProcessingMode.Local;
                            rptViewer.LocalReport.ReportPath = reportPath;
                            rptViewer.LocalReport.EnableExternalImages = true;
                            rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAdmissionList.Trim(), list));

                            byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                            string path = Server.MapPath("~/Content/temp");
                            string savelocation = Path.Combine(path, currentProgramme.Name.Replace("/", "_") + " " + currentDepartment.Name.Replace("/", "_") + ".pdf");
                            File.WriteAllBytes(savelocation, bytes); 
                        }
                    }
                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file, "");
                    string zipFileName = zipName;
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    //return System.IO.File(Server.MapPath(export), "application/zip", zipFileName + ".zip");
                    //UrlHelper urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //Response.Redirect(urlHelp.Action("DownloadStatementOfResultZip", new { controller = "Result", area = "Admin", downloadName = department.Name }), false);
                    Response.Redirect(export, false);
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}