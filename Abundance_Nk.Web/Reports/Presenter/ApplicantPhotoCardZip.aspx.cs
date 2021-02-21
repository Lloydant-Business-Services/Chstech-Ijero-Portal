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
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Abundance_Nk.Web.Models;
using Ionic.Zip;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ApplicantPhotoCardZip : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    rblSortOption.SelectedIndex = 0;
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
        
        public int Option
        {
            get { return Convert.ToInt32(rblSortOption.SelectedValue); }
            set { rblSortOption.SelectedValue = value.ToString(); }
        }
        
        private void DisplayReportBy(Session session, Programme programme, SortOption sortOption)
        {
            try
            {
                List<PhotoCard> photoCards = null;
                ApplicationFormLogic applicationformLogic = new ApplicationFormLogic();

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                if (Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"), true);
                }

                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                List<Department> departments = Utility.GetDepartmentByProgramme(programme);

                for (int i = 0; i < departments.Count; i++)
                {
                    if (departments[i].Id <= 0)
                    {
                        continue;
                    }

                    Department department = departments[i];

                    photoCards = applicationformLogic.GetPostJAMBApplicationsBy(session, programme, department, sortOption);

                    string bind_dsPhotoCard = "dsPhotoCard";
                    string reportPath = @"Reports\PhotoCard.rdlc";

                    if (photoCards != null && photoCards.Count > 0)
                    {
                        string appRoot = ConfigurationManager.AppSettings["AppRoot"];

                        foreach (PhotoCard photocard in photoCards)
                        {
                            if (!string.IsNullOrWhiteSpace(photocard.PassportUrl))
                            {
                                photocard.PassportUrl = appRoot + photocard.PassportUrl;
                            }
                            else
                            {
                                photocard.PassportUrl = appRoot + Utility.DEFAULT_AVATAR;
                            }
                        }

                        ReportViewer rptViewer = new ReportViewer();
                        rptViewer.Visible = false;
                        rptViewer.Reset();

                        rptViewer.LocalReport.DisplayName = "Applicant Photo Card";
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.EnableExternalImages = true;

                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsPhotoCard.Trim(), photoCards));
                        
                        byte[] bytes = rptViewer.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string path = Server.MapPath("~/Content/temp");
                        string savelocation = "";
                        
                        savelocation = Path.Combine(path, department.Name.Replace("/", "_") + ".pdf");
                        
                        File.WriteAllBytes(savelocation, bytes);
                    }
                }
                using (ZipFile zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file, "");
                    string zipFileName = programme.Name.Replace("/", "_");
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    Response.Redirect(export, false);

                    return;
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
                else if (Programme == null || Programme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
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

                SortOption sortOption = Option == 2 ? SortOption.ExamNo : Option == 3 ? SortOption.ApplicationNo : SortOption.Name;
                DisplayReportBy(SelectedSession, Programme, sortOption);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }


    }
}