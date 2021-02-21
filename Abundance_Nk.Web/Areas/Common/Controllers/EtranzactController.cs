using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;
using System.Xml;
using System.Text;
using System.IO;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Linq.Expressions;
using System.Web.Routing;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Transactions;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    public class EtranzactController : Controller
    {
        //
        // GET: /Common/Etranzact/
        public ActionResult Index(string payeeid, string payment_type)
        {
            if (payeeid != "" && payment_type != null)
            {
                ViewBag.xmlUrl = BuidXml(payeeid, payment_type);
            }
          
            return View();
        }

        private string BuidXml(string InvoiceNumber, string paymentpurpose)
        {
            string url = "";

            try
            {
                string filename = InvoiceNumber.Replace("-", "").Replace("/", "").Replace(":", "").Replace(" ", "") + DateTime.Now.ToString().Replace("-", "").Replace("/", "").Replace(":", "").Replace(" ", "");
                url = "~/PayeeId/" + filename + ".xml";
                if (!Directory.Exists(Server.MapPath("~/PayeeId")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/PayeeId"));
                }

                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();

                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == InvoiceNumber);
                if (payment == null)
                {
                    CreateErrorTree(url);
                    return url;
                }

                Person person = new Person();
                PersonLogic personLogic = new PersonLogic();

                person = personLogic.GetModelBy(p => p.Person_Id == payment.Person.Id);
                if (person != null)
                {
                    string fullname = person.FullName;
                    string faculty = "";
                    string dept = "";
                    string level = "";
                    string studenttypeid = "";
                    string modeofentry = "";
                    string sessionid = "";
                    string Amount = "";
                    string paymentstatus = "";
                    string PaymentType = "";
                    string phoneNo = person.MobilePhone;
                    string email = person.Email;
                    string MatricNo = ""; ;
                    string levelid = ""; ;
                    string PaymentCategory = ""; ;
                    string semester = "";
                    string semesterid = "";

                    if (semesterid == "1")
                    {
                        semester = "BOTH";
                    }
                    else if (semesterid == "2")
                    {
                        semester = "FIRST SEMESTER";
                    }
                    else if (semesterid == "3")
                    {
                        semester = "SECOND SEMESTER";
                    }
                    else
                    {
                        semester = "N/A";
                    }

                    CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory, url);

                    return url;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return url;
        }

        private void CreateTree(string payeename, string Faculty, string Department, string Level, string ProgrammeType, string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester, string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category, string url)
        {
            try
            {
                FileStream fs = new FileStream(Server.MapPath(url), FileMode.Create);
                fs.Close();

                XmlTextWriter writer = new XmlTextWriter(Server.MapPath(url), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;

                writer.WriteStartElement("FeeRequest");
                createNode(payeename, Faculty, Department, Level, ProgrammeType, StudyType, Session, PayeeID, Amount, FeeStatus, Semester, PaymentType, MatricNumber, Email, PhoneNumber, category, writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                //WriteXmlToPage(Server.MapPath(url));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void createNode(string payeeName, string Faculty, string Department, string Level, string ProgrammeType, string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester, string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("PayeeName");
                writer.WriteString(payeeName);
                writer.WriteEndElement();

                writer.WriteStartElement("Faculty");
                writer.WriteString(Faculty);
                writer.WriteEndElement();

                writer.WriteStartElement("Department");
                writer.WriteString(Department);
                writer.WriteEndElement();

                writer.WriteStartElement("Level");
                writer.WriteString(Level);
                writer.WriteEndElement();

                writer.WriteStartElement("ProgrammeType");
                writer.WriteString(ProgrammeType);
                writer.WriteEndElement();

                writer.WriteStartElement("StudyType");
                writer.WriteString(StudyType);
                writer.WriteEndElement();

                writer.WriteStartElement("Session");
                writer.WriteString(Session);
                writer.WriteEndElement();

                writer.WriteStartElement("PayeeID");
                writer.WriteString(PayeeID);
                writer.WriteEndElement();

                writer.WriteStartElement("Amount");
                writer.WriteString(Amount);
                writer.WriteEndElement();

                writer.WriteStartElement("FeeStatus");
                writer.WriteString(FeeStatus);
                writer.WriteEndElement();

                writer.WriteStartElement("Semester");
                writer.WriteString(Semester);
                writer.WriteEndElement();

                if (category.Trim() == "")
                {
                    writer.WriteStartElement("PaymentType");
                    writer.WriteString(PaymentType);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("PaymentType");
                    writer.WriteString(PaymentType + " - " + category);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("PaymentCategory");
                writer.WriteString(category);
                writer.WriteEndElement();

                writer.WriteStartElement("MatricNumber");
                writer.WriteString(MatricNumber);
                writer.WriteEndElement();

                writer.WriteStartElement("Email");
                writer.WriteString(Email);
                writer.WriteEndElement();

                writer.WriteStartElement("PhoneNumber");
                writer.WriteString(PhoneNumber);
                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WriteXmlToPage(string url)
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string t = sr.ReadToEnd().Trim();
            sr.Close();

            //for (int i = 0; i <= t.; i++)
            //{
            //    Response.Write(t.Lines[i].ToString());
            //}
            //File.Delete(url);

            Response.End();
        }

        private void CreateErrorTree(string url)
        {
            try
            {
                FileStream fs = new FileStream(Server.MapPath(url), FileMode.Create);
                fs.Close();

                XmlTextWriter writer = new XmlTextWriter(Server.MapPath(url), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;

                writer.WriteStartElement("FeeRequest");
                writer.WriteStartElement("Error");
                writer.WriteString("-1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                WriteXmlToPage(Server.MapPath(url));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult RetrievePin()
        {
            if (Request.QueryString["RECEIPT_NO"] != null)
            {
                //var d = System.Web.HttpContext.Current.Request.UrlReferrer;
                //String clientIP = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]==null)?System.Web.HttpContext.Current.Request.UserHostAddress:System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (clientIP != "197.255.244.10")
                //{
                //     return Json( "Access denied" , JsonRequestBehavior.AllowGet);

                //}
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = new PaymentEtranzact();
                paymentEtranzact.ReceiptNo = Request.QueryString["RECEIPT_NO"];
                paymentEtranzact.PaymentCode = Request.QueryString["PAYMENT_CODE"];
                paymentEtranzact.ConfirmationNo = Request.QueryString["PAYMENT_CODE"];
                paymentEtranzact.MerchantCode = Request.QueryString["MERCHANT_CODE"];
                paymentEtranzact.TransactionAmount = Convert.ToDecimal(Request.QueryString["TRANS_AMOUNT"]);
                paymentEtranzact.TransactionDescription = Request.QueryString["TRANS_DESCR"];
                paymentEtranzact.BankCode = Request.QueryString["BANK_CODE"];
                paymentEtranzact.BranchCode = Request.QueryString["BRANCH_CODE"];
                paymentEtranzact.CustomerName = Request.QueryString["CUSTOMER_NAME"];
                paymentEtranzact.CustomerAddress = "FETC" + Request.QueryString["CUSTOMER_ADDRESS"];
                paymentEtranzact.CustomerID = Request.QueryString["CUSTOMER_ID"];
                paymentEtranzact.TransactionDate = Convert.ToDateTime(Request.QueryString["TRANS_DATE"]);
                paymentEtranzact.Used = false;
                paymentEtranzact.UsedBy = 0;

                if (paymentEtranzactLogic.IsPinOnTable(paymentEtranzact.ConfirmationNo))
                {

                    return Json("Duplicate Record", "text/html", JsonRequestBehavior.AllowGet);

                }

                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetBy(paymentEtranzact.CustomerID);
                if (payment != null && payment.Id > 0)
                {
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    OnlinePayment onlinePayment = onlinePaymentLogic.GetBy(payment.Id);

                    paymentEtranzact.UsedBy = payment.Person.Id;
                    paymentEtranzact.Payment = onlinePayment;
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    PaymentTerminal paymentTerminal = paymentTerminalLogic.GetBy(payment);
                    if (paymentTerminal != null)
                    {
                        paymentEtranzact.Terminal = paymentTerminal;
                    }
                    else
                    {
                        paymentEtranzact.Terminal = new PaymentTerminal() { Id = 1, FeeType = new FeeType() { Id = 1 } };
                    }

                    var paymentEtranzactType = new PaymentEtranzactType();
                    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == paymentEtranzact.Terminal.FeeType.Id).LastOrDefault();
                    paymentEtranzact.EtranzactType = paymentEtranzactType;
                    paymentEtranzactLogic.Create(paymentEtranzact);
                    string json = "";
                    return Json(new { paymentEtranzact }, "text/html", JsonRequestBehavior.AllowGet);

                }

            }

            return View();

        }
    
    }
}