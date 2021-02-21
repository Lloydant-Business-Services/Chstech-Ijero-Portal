using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
   
    public class ComplainController : BaseController
    {
        private MailMessage mail;
        // GET: Applicant/Complain
        [AllowAnonymous]
        public ActionResult Index()
        {
            ComplaintLog log = new ComplaintLog();
            try
            {
                
            }
            catch (Exception ex)
            {
                
               SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(log);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ComplaintLog log)
        {
            try
            {
                if (log != null)
                {
                    ComplainLogLogic logLogic = new ComplainLogLogic();
                    log = logLogic.Create(log);
                    SetMessage("Your complaint has been received with ticket ID " + log.TicketID + ". Your issue would be resolved shortly.", Message.Category.Information);
                    //SendMail(log);
                    log = new ComplaintLog();
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(log);
        }

        private void SendMail(ComplaintLog log)
        {
            try
            {
                mail = new MailMessage();
                mail.To.Add("lawsgacc@gmail.com");
                mail.From = new MailAddress("lawrence.eze0@gmail.com", "FEDPOLY PORTAL");
                mail.Subject = "COMPLAINT MAIL";
                mail.Body = "Name: " + log.Name + ", Application Number: " + log.ApplicationNumber + ", Phone Number: " + log.MobileNumber + ". " + "\n" + log.Complain;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 465;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("lawrence.eze0@gmail.com", "gaccount");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {   
                throw;
            }
        }
        
        [AllowAnonymous]
        public ActionResult Status()
        {
            ComplaintLog log = new ComplaintLog();
            try
            {

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(log);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Status(ComplaintLog log)
        {
            try
            {
                if (log != null)
                {
                    ComplainLogLogic logLogic = new ComplainLogLogic();
                    log = logLogic.GetBy(log.TicketID);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(log);
        }

        public ActionResult View()
        {
            List<ComplaintLog> logList = new List<ComplaintLog>();
            ComplainLogLogic logLogic = new ComplainLogLogic();
            try
            {

                logList = logLogic.GetAllUnResolved();
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(logList);
        }

        public ActionResult Resolve(long id)
        {
            ComplaintLog log = new ComplaintLog();
            ComplainLogLogic logLogic = new ComplainLogLogic();
            try
            {

                log = logLogic.GetBy(id);
                if (log != null)
                {
                    return View(log);
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resolve(ComplaintLog log)
        {
            ComplainLogLogic logLogic = new ComplainLogLogic();
            try
            {              
                if (log != null)
                {
                   if (logLogic.Modify(log))
                   {
                       SetMessage("Issue has been updated! ", Message.Category.Information);
                   }
                    
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("View");
        }
        
        [AllowAnonymous]
        public ActionResult Fix()
        {
           
            return View();

        }
    
    }
}