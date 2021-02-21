using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Entity;
using System.Net;
using Abundance_Nk.Model.Entity;
using System.Xml;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Linq.Expressions;
using System.Web.Routing;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Transactions;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;

namespace Abundance_Nk.Web
{
    public partial class WebApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["payee_id"] != null && Request.QueryString["payment_type"] != null)
            {
                string payeeid = Request.QueryString["payee_id"];
                string payment_type = Request.QueryString["payment_type"];
                BuidXml(payeeid, payment_type);
            }
        }
        private void BuidXml(string InvoiceNumber, string paymentpurpose)
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
                payment = paymentLogic.GetBy(InvoiceNumber);
                if (payment == null)
                {
                    CreateErrorTree(url);
                    return;
                }

                PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id).FirstOrDefault();
                if (paymentEtranzactType == null)
                {
                    CreateErrorTree(url);
                    return;
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
                    string shortFallAmount = "";
                    string hostelAmount = "";

                    AppliedCourse appliedCourse = new AppliedCourse();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == payment.Person.Id);

                    //the below code is used to genrate correct department at bank for those whose applied course is not their admitted course

                    AdmissionList list = new AdmissionList();
                    AdmissionListLogic listLogic = new AdmissionListLogic();
                    if (appliedCourse != null && appliedCourse.ApplicationForm != null)
                    {
                        list = listLogic.GetBy(appliedCourse.ApplicationForm.Id);
                        if (list != null && list.Deprtment != null)
                        {
                            appliedCourse.Department = list.Deprtment;
                        }
                    }

                    Model.Model.Student student = new Model.Model.Student();
                    StudentLogic studentLogic = new StudentLogic();
                    student = studentLogic.GetBy(payment.Person.Id);
                    StudentLevel studentLevel = new StudentLevel();
                    StudentLevelLogic levelLogic = new StudentLevelLogic();
                    if (student != null)
                    {
                        studentLevel = levelLogic.GetModelsBy(sl => sl.Person_Id == student.Id && sl.Session_Id == payment.Session.Id).LastOrDefault();
                        if (studentLevel == null || studentLevel.Id <= 0)
                        {
                            studentLevel = levelLogic.GetBy(student.Id);
                        }
                    }

                    Decimal PaymentAmount = 0;

                    if (student != null && studentLevel != null && (payment.FeeType.Id == 3 || payment.FeeType.Id == 10))
                    {
                        PaymentAmount = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Level.Id, payment.PaymentMode.Id, studentLevel.Department.Id, payment.Session.Id).Sum(a => a.Fee.Amount);

                        StudentExtraYearSession extraYear = new StudentExtraYearSession();
                        StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
                        extraYear = extraYearLogic.GetBy(payment.Person.Id, payment.Session.Id);

                        if (extraYear != null)
                        {
                            int lastSession = Convert.ToInt32(extraYear.LastSessionRegistered.Name.Substring(0, 4));
                            int currentSession = Convert.ToInt32(payment.Session.Name.Substring(0, 4));
                            int NoOfOutstandingSession = currentSession - lastSession;
                            if (NoOfOutstandingSession == 0)
                            {
                                NoOfOutstandingSession = 1;
                            }

                            PaymentAmount = payment.FeeDetails.Sum(p => p.Fee.Amount) * NoOfOutstandingSession;
                        }

                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id && m.Programme_Id == studentLevel.Programme.Id && m.Level_Id == studentLevel.Level.Id).FirstOrDefault();
                        if (paymentEtranzactType == null)
                        {
                            CreateErrorTree(url);
                            return;
                        }

                        if (PaymentAmount > 0)
                        {
                            faculty = studentLevel.Department.Faculty.Name;
                            dept = studentLevel.Department.Name;
                            level = studentLevel.Level.Name;
                            studenttypeid = studentLevel.Programme.Name;
                            modeofentry = "N/A";
                            sessionid = payment.Session.Name;
                            paymentstatus = "FEE HAS NOT BEEN PAID";
                            PaymentType = paymentEtranzactType.Name;
                            phoneNo = person.MobilePhone;
                            email = person.Email;
                            MatricNo = studentLevel.Student.MatricNumber;
                            levelid = studentLevel.Level.Id.ToString();
                            PaymentCategory = paymentEtranzactType.Name;
                            semester = "N/A";
                            Amount = PaymentAmount.ToString();
                        }
                        else
                        {
                            CreateErrorTree(url);
                            return;
                        }

                    }

                    else if ((appliedCourse != null && payment.FeeType.Id == 3) || (appliedCourse != null && payment.FeeType.Id == 2) || (appliedCourse != null && payment.FeeType.Id == 9))
                    {

                        Int32 levelId = 1;
                        if (appliedCourse.Programme.Id == 3)
                        {
                            levelId = 3;
                        }
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id && m.Programme_Id == appliedCourse.Programme.Id && m.Level_Id == levelId).FirstOrDefault();

                        if (paymentEtranzactType == null)
                        {
                            CreateErrorTree(url);
                            return;
                        }

                        PaymentAmount = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id, levelId, payment.PaymentMode.Id, appliedCourse.Department.Id, payment.Session.Id).Sum(a => a.Fee.Amount);

                        faculty = appliedCourse.Department.Faculty.Name;
                        dept = list.Deprtment.Name;
                        studenttypeid = appliedCourse.Programme.Name;
                        level = "N/A";
                        modeofentry = "N/A";
                        sessionid = payment.Session.Name;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment.FeeType);
                        Amount = PaymentAmount.ToString();
                        paymentstatus = "FEE HAS NOT BEEN PAID";
                        PaymentType = paymentEtranzactType.Name;
                        phoneNo = person.MobilePhone;
                        email = person.Email;
                        MatricNo = InvoiceNumber;
                        levelid = "N/A";
                        PaymentCategory = paymentEtranzactType.Name;
                        semester = "N/A";
                        Amount = PaymentAmount.ToString();
                    }
                    else if (appliedCourse != null && payment.FeeType.Id == 1 || payment.FeeType.Id == 4 || payment.FeeType.Id == 5 || payment.FeeType.Id == 6)
                    {
                        Int32 levelId = 1;
                        if (appliedCourse.Programme.Id == 3)
                        {
                            levelId = 3;
                        }
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id && m.Programme_Id == appliedCourse.Programme.Id && m.Level_Id == levelId).FirstOrDefault();

                        if (paymentEtranzactType == null)
                        {
                            CreateErrorTree(url);
                            return;
                        }

                        faculty = appliedCourse.Department.Faculty.Name;
                        if (list != null && list.Deprtment != null && list.Deprtment.Id > 0)
                        {
                            dept = list.Deprtment.Name;
                        }
                        else
                        {
                            dept = appliedCourse.Department.Name;
                        }
                        
                        studenttypeid = appliedCourse.Programme.Name;
                        level = "N/A";
                        modeofentry = "N/A";
                        sessionid = payment.Session.Name;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment.FeeType);
                        Amount = payment.FeeDetails.Sum(p => p.Fee.Amount).ToString();
                        paymentstatus = "FEE HAS NOT BEEN PAID";
                        PaymentType = paymentEtranzactType.Name;
                        phoneNo = person.MobilePhone;
                        email = person.Email;
                        MatricNo = InvoiceNumber;
                        levelid = "N/A";
                        PaymentCategory = paymentEtranzactType.Name;
                        semester = "N/A";
                    }
                    else if (appliedCourse != null && payment.FeeType.Id == (int)FeeTypes.ChangeOfCourseFees)
                    {

                        Int32 levelId = 1;
                        if (appliedCourse.Programme.Id == 3)
                        {
                            levelId = 3;
                        }
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id && m.Programme_Id == appliedCourse.Programme.Id && m.Level_Id == levelId).FirstOrDefault();

                        if (paymentEtranzactType == null)
                        {
                            CreateErrorTree(url);
                            return;
                        }

                        PaymentAmount = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id, levelId, payment.PaymentMode.Id, appliedCourse.Department.Id, payment.Session.Id).Sum(a => a.Fee.Amount);

                        faculty = appliedCourse.Department.Faculty.Name;
                        dept = list.Deprtment.Name;
                        studenttypeid = appliedCourse.Programme.Name;
                        level = "N/A";
                        modeofentry = "N/A";
                        sessionid = payment.Session.Name;
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment.FeeType);
                        Amount = PaymentAmount.ToString();
                        paymentstatus = "FEE HAS NOT BEEN PAID";
                        PaymentType = paymentEtranzactType.Name;
                        phoneNo = person.MobilePhone;
                        email = person.Email;
                        MatricNo = InvoiceNumber;
                        levelid = "N/A";
                        PaymentCategory = paymentEtranzactType.Name;
                        semester = "N/A";
                        Amount = PaymentAmount.ToString();
                    }
                    else
                    {
                        ShortFallLogic shortFallLogic = new ShortFallLogic();
                        ShortFall shortFall = new ShortFall();
                        if (payment.FeeType.Id == (int)FeeTypes.ShortFall)
                        {
                            paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Session_Id == payment.Session.Id && m.Fee_Type_Id == payment.FeeType.Id).FirstOrDefault();
                            shortFall = shortFallLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                            if (shortFall != null)
                            {
                                shortFallAmount = shortFall.Amount.ToString();
                                faculty = studentLevel.Department.Faculty.Name;
                                dept = studentLevel.Department.Name;
                                level = studentLevel.Level.Name;
                                studenttypeid = studentLevel.Programme.Name;
                                modeofentry = "N/A";
                                sessionid = payment.Session.Name;
                                Amount = shortFallAmount;
                                paymentstatus = "FEE HAS NOT BEEN PAID";
                                PaymentType = paymentEtranzactType.Name;
                                phoneNo = person.MobilePhone;
                                email = person.Email;
                                MatricNo = studentLevel.Student.MatricNumber;
                                levelid = studentLevel.Level.Id.ToString();
                                PaymentCategory = paymentEtranzactType.Name;
                                semester = "N/A";
                            }
                            else
                            {
                                CreateErrorTree(url);
                            }

                        }
                        else if (payment.FeeType.Id == (int)FeeTypes.HostelFee)
                        {
                            HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                            HostelFee hostelFee = hostelFeeLogic.GetModelBy(h => h.Payment_Id == payment.Id);
                            paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Session_Id == payment.Session.Id && m.Fee_Type_Id == payment.FeeType.Id).FirstOrDefault();
                            if (hostelFee != null)
                            {
                                if (studentLevel != null && studentLevel.Id > 0)
                                {
                                    faculty = studentLevel.Department.Faculty.Name;
                                    dept = studentLevel.Department.Name;
                                    level = studentLevel.Level.Name;
                                    studenttypeid = studentLevel.Programme.Name;
                                    MatricNo = studentLevel.Student.MatricNumber;
                                    levelid = studentLevel.Level.Id.ToString();
                                }
                                else if (appliedCourse != null)
                                {
                                    faculty = appliedCourse.Department.Faculty.Name;
                                    dept = list.Deprtment.Name;
                                    studenttypeid = appliedCourse.Programme.Name;
                                    MatricNo = null;
                                    Level appliedLevel = null;
                                    appliedLevel = new Level() { Id = 1, Name = "ND I" };
                                    if (appliedCourse.Programme.Id == 3)
                                    {
                                        appliedLevel = new Level() { Id = 3, Name = "HND I" };
                                    }

                                    level = appliedLevel.Name;

                                    levelid = appliedLevel.Id.ToString();
                                }

                                hostelAmount = hostelFee.Amount.ToString();
                                modeofentry = "N/A";
                                sessionid = payment.Session.Name;
                                Amount = hostelAmount;
                                paymentstatus = "FEE HAS NOT BEEN PAID";
                                PaymentType = paymentEtranzactType.Name;
                                phoneNo = person.MobilePhone;
                                email = person.Email;
                                PaymentCategory = paymentEtranzactType.Name;
                                semester = "N/A";
                            }
                            else
                            {
                                CreateErrorTree(url);
                            }
                        }
                        else
                        {

                            paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(m => m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id && m.Programme_Id == studentLevel.Programme.Id && m.Level_Id == studentLevel.Level.Id).FirstOrDefault();
                            if (paymentEtranzactType == null)
                            {
                                CreateErrorTree(url);
                                return;
                            }
                            faculty = studentLevel.Department.Faculty.Name;
                            dept = studentLevel.Department.Name;
                            level = studentLevel.Level.Name;
                            studenttypeid = studentLevel.Programme.Name;
                            modeofentry = "N/A";
                            sessionid = payment.Session.Name;
                            Amount = payment.FeeDetails.Sum(a => a.Fee.Amount).ToString();
                            paymentstatus = "FEE HAS NOT BEEN PAID";
                            PaymentType = paymentEtranzactType.Name;
                            phoneNo = person.MobilePhone;
                            email = person.Email;
                            MatricNo = studentLevel.Student.MatricNumber;
                            levelid = studentLevel.Level.Id.ToString();
                            PaymentCategory = paymentEtranzactType.Name;
                            semester = "N/A";
                        }
                    }

                    CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory, url);

                    return;
                }
            }
            catch (Exception ex)
            {
                //CreateErrorTree(url);
                //return;
                throw ex;
            }
            return;
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

                WriteXmlToPage(Server.MapPath(url));


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

                writer.WriteStartElement("PaymentType");
                writer.WriteString(PaymentType);
                writer.WriteEndElement();



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
            System.Windows.Forms.RichTextBox t = new System.Windows.Forms.RichTextBox();
            t.Text = sr.ReadToEnd().Trim();
            sr.Close();

            for (int i = 0; i <= t.Lines.GetUpperBound(0); i++)
            {
                Response.Write(t.Lines[i].ToString());
            }

            File.Delete(url);
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
        private Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch (programme.Id)
                {
                    case 1:
                        {
                            return level = new Level() { Id = 1 };

                        }
                    case 2:
                        {
                            return level = new Level() { Id = 1 };

                        }
                    case 3:
                        {
                            return level = new Level() { Id = 3 };

                        }
                    case 4:
                        {
                            return level = new Level() { Id = 3 };

                        }
                    case 5:
                        {
                            return level = new Level() { Id = 1 };

                        }
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}