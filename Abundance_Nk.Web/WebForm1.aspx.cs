using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using System.Transactions;
using System.Data.Entity.Validation;

namespace Abundance_Nk.Web
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        Abundance_NkEntities db;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GenerateInvoice();
            }
        }

        private void GenerateInvoice()
        {

            try
            {
                using (db = new Abundance_NkEntities())
                {
                    ApplicationFormLogic appFormLogic = new ApplicationFormLogic();
                    ApplicationForm appForm = new ApplicationForm();

                    Payment payment = new Payment();
                    PaymentLogic paymentLogic = new PaymentLogic();

                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                   
                        foreach (var itemPaid in db.PAID)
                    {
                            //using (TransactionScope transaction = new TransactionScope())
                            //{

                                appForm = appFormLogic.GetModelBy(a => a.Application_Form_Number == itemPaid.Applicantion_Form);
                                if (appForm != null)
                                {
                                    payment.DatePaid = DateTime.Now;
                                    payment.FeeType = new FeeType { Id = 2 };
                                    payment.PaymentMode = new PaymentMode { Id = 1 };
                                    payment.PaymentType = new PaymentType { Id = 2 };
                                    payment.Person = appForm.Person;
                                    payment.PersonType = appForm.Person.Type;
                                    payment.Session = new Model.Model.Session { Id = 1 };
                                    payment = paymentLogic.Create(payment);

                                    if (payment.Id > 0 && payment.InvoiceNumber != null)
                                    {

                                        remitaPayment.RRR = itemPaid.RRR;
                                        remitaPayment.Status = "01:";
                                        remitaPayment.payment = payment;
                                        remitaPayment.OrderId = payment.InvoiceNumber;
                                        remitaPayment.Receipt_No = payment.InvoiceNumber;
                                        remitaPayment.TransactionAmount = 33700;
                                        remitaPayment.TransactionDate = DateTime.Now;
                                        remitaPayment.Description = "MANUAL PAYMENT ACCEPTANCE";
                                        remitaPayment = remitaPaymentLogic.Create(remitaPayment);

                                    }
                                }
                           
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}",
                        //                        validationError.PropertyName,
                        //                        validationError.ErrorMessage);
                    }
                }
            }
            
        }


    }
}