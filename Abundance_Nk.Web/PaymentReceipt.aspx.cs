using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Configuration;

namespace Abundance_Nk.Web
{
    public partial class PaymentReceipt : System.Web.UI.Page
    {
        public string order_Id;
        public string url;
        public string message;
        public string rrr;
        public string statuscode;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form["orderID"].ToString() != null)
            {
                order_Id = Request.Form["orderID"].ToString();
            }
            else if (Request.Form["orderID"].ToString() != null)
            {
                order_Id = Request.QueryString["orderID"].ToString();
            }
           
            RemitaSettings settings = new RemitaSettings();
            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
            RemitaResponse remitaResponse = new RemitaResponse();
            RemitaPayment remitaPayment = new RemitaPayment();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == order_Id);
            string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
            RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
            remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
            if (remitaResponse != null && remitaResponse.Status != null)
            {
                message = remitaResponse.Message;
                rrr = remitaResponse.RRR;
                statuscode = remitaResponse.Status;
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPayment.BankCode = remitaResponse.bank;
                remitaPayment.CustomerName = remitaResponse.RemitaDetails.payerName;
                remitaPayment.TransactionAmount = remitaResponse.amount;
                remitaPaymentLogic.Modify(remitaPayment);
            }
            //return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(remitaResponse);

        }
        private string SHA512(string hash_string)
        {
            System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
            return hashed;
        }
   
    
    }
}