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
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{

    public class RemitaReportController :  BaseController
    {
        private RemitaPaymentViewModel viewModel;
        public ActionResult Index()
        {
            RemitaPaymentViewModel viewModel = new RemitaPaymentViewModel();
            //viewModel = new RemitaPaymentViewModel();
            //RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
            //viewModel.remitaPaymentList = remitaLogic.GetAll();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Index(RemitaPaymentViewModel viewModel)
        {

            RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
            //viewModel.remitaPaymentList = remitaLogic.GetAll();
            if (viewModel.DateFrom > viewModel.DateTo)
            {
                SetMessage("The date range selected is not valid", Message.Category.Information);
                return View(viewModel);
            }
            viewModel.DateTo = viewModel.DateTo.AddDays(1).AddMinutes(-1);
            viewModel.remitaPaymentList = remitaLogic.GetModelsBy(f=>f.Transaction_Date>=viewModel.DateFrom && f.Transaction_Date<=viewModel.DateTo);
            return View(viewModel);
        }
        public ActionResult GetStatus(string order_Id)
        {
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
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPaymentLogic.Modify(remitaPayment);
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult PaymentReceipt(string orderID)
        {
            RemitaResponse remitaResponse = new RemitaResponse();
            viewModel = new RemitaPaymentViewModel();

            try
            {

                if (Request.QueryString["orderID"] != null || !string.IsNullOrEmpty(orderID))
                {
                    orderID = orderID ?? Request.QueryString["orderID"].ToString().Trim();
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment = remitaPaymentLogic.GetBy(orderID);
                    string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                    //string remitaVerifyUrl = "http://www.remitademo.net/remita/ecomm/split/init.reg";
                    //string remitaVerifyUrl = "http://www.remitademo.net/remita/ecomm";
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                    remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
                    if (remitaResponse != null && remitaResponse.Status != null)
                    {
                        viewModel.message = remitaResponse.Message;
                        viewModel.rrr = remitaResponse.RRR;
                        viewModel.statuscode = remitaResponse.Status;
                        remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                        remitaPaymentLogic.Modify(remitaPayment);
                        viewModel.remitaResponse = remitaResponse;
                        return Json("Ok", JsonRequestBehavior.AllowGet);
                    }

                    remitaResponse.Message = "Order ID was not generated from this system";
                    viewModel.remitaResponse = remitaResponse;
                }
                else
                {
                    remitaResponse.Message = "No data was received!";
                    viewModel.remitaResponse = remitaResponse;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(remitaResponse.Message, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PaymentReceipt()
        {
            List<RemitaResponse> remitaResponse = new List<RemitaResponse>();
            try
            {
                var resolveRequest = HttpContext.Request;
                resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
                string jsondata = new StreamReader(resolveRequest.InputStream).ReadToEnd();
                remitaResponse = new JavaScriptSerializer().Deserialize<List<RemitaResponse>>(jsondata);
                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                //using (StreamWriter w = new StreamWriter(@"C:\Users\abundance\Documents\test.txt"))
                //{
                //    w.WriteLine("RRR: " + remitaResponse[0].RRR + " Order_Id:" + remitaResponse[0].orderRef);
                //}
                if (remitaResponse != null && remitaResponse.Count > 0)
                {
                   foreach (RemitaResponse response in remitaResponse)
                   {
                        return PaymentReceipt(response.orderRef);
                        //viewModel = new RemitaPaymentViewModel();
                        //RemitaSettings settings = new RemitaSettings();
                        //RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                        //settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);

                        //remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == response.orderRef);

                        //if (remitaResponse != null && response.responseCode != null && remitaPayment != null)
                        //{
                        //    remitaPayment.Description = response.Message;
                        //    remitaPayment.RRR = response.RRR;
                        //    remitaPayment.Status = response.responseCode + ":" + response.StatusCode;
                        //    if (response.bank != null)
                        //    {
                        //        remitaPayment.BankCode = response.bank;
                        //        remitaPayment.Status ="01:approved";
                        //    }
                        //    if (response.RemitaDetails != null)
                        //    {
                        //        remitaPayment.CustomerName = remitaResponse[0].RemitaDetails.payerName;
                        //    }
                        //    if (response.amount > 0)
                        //    {
                        //        remitaPayment.TransactionAmount = response.amount;
                        //    }

                        //    remitaPaymentLogic.Modify(remitaPayment);
                       
                        //}
                        //return Json("Ok", JsonRequestBehavior.AllowGet);
                   
                   }
                   remitaResponse[0].Message = "Order ID was not generated from this system";

                }
                else
                {
                    remitaResponse = null;
                    remitaResponse[0].Message = "No data received";
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return Json(remitaResponse, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult PaymentReceiptCard(string orderID)
        {
            RemitaResponse remitaResponse = new RemitaResponse();
            viewModel = new RemitaPaymentViewModel();

            try
            {

                if (Request.QueryString["orderID"] != null)
                {
                    orderID = Request.QueryString["orderID"].ToString().Trim();
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
                    RemitaPayment remitaPayment = new RemitaPayment();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    remitaPayment = remitaPaymentLogic.GetBy(orderID);
                    string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                    //string remitaVerifyUrl = "http://www.remitademo.net/remita/ecomm/split/init.reg";
                    //string remitaVerifyUrl = "http://www.remitademo.net/remita/ecomm";
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                    remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
                    if (remitaResponse != null && remitaResponse.Status != null)
                    {
                        viewModel.message = remitaResponse.Message;
                        viewModel.rrr = remitaResponse.RRR;
                        viewModel.statuscode = remitaResponse.Status;
                        remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                        remitaPaymentLogic.Modify(remitaPayment);
                        viewModel.remitaResponse = remitaResponse;
                        return View(viewModel);
                    }

                    remitaResponse.Message = "Order ID was not generated from this system";
                    viewModel.remitaResponse = remitaResponse;
                }
                else
                {
                    remitaResponse.Message = "No data was received!";
                    viewModel.remitaResponse = remitaResponse;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PaymentReceiptCard()
        {
            List<RemitaResponse> remitaResponse = new List<RemitaResponse>();
            try
            {
                var resolveRequest = HttpContext.Request;
                resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
                string jsondata = new StreamReader(resolveRequest.InputStream).ReadToEnd();
                remitaResponse = new JavaScriptSerializer().Deserialize<List<RemitaResponse>>(jsondata);
                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                //using (StreamWriter w = new StreamWriter(@"C:\Users\abundance\Documents\test.txt"))
                //{
                //    w.WriteLine("RRR: " + remitaResponse[0].RRR + " Order_Id:" + remitaResponse[0].orderRef);
                //}
                if (remitaResponse != null && remitaResponse.Count > 0)
                {
                    foreach (RemitaResponse response in remitaResponse)
                    {

                        viewModel = new RemitaPaymentViewModel();
                        RemitaSettings settings = new RemitaSettings();
                        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                        settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);

                        remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == response.orderRef);

                        if (remitaResponse != null && response.responseCode != null && remitaPayment != null)
                        {
                            remitaPayment.Description = response.Message;
                            remitaPayment.RRR = response.RRR;
                            remitaPayment.Status = response.responseCode + ":" + response.StatusCode;
                            if (response.bank != null)
                            {
                                remitaPayment.BankCode = response.bank;
                                remitaPayment.Status = "01:approved";
                            }
                            if (response.RemitaDetails != null)
                            {
                                remitaPayment.CustomerName = remitaResponse[0].RemitaDetails.payerName;
                            }
                            if (response.amount > 0)
                            {
                                remitaPayment.TransactionAmount = response.amount;
                            }

                            remitaPaymentLogic.Modify(remitaPayment);

                        }
                        return Json(remitaResponse);

                    }
                    remitaResponse[0].Message = "Order ID was not generated from this system";

                }
                else
                {
                    remitaResponse = null;
                    remitaResponse[0].Message = "No data received";
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(remitaResponse);
        }


        private string SHA512(string hash_string)
        {
            System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
            return hashed;
        }
   
        private ActionResult AcceptanceReport()
        {
            try
            {

            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        private ActionResult SchoolFeesReport()
        {
            try
            {

            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        public ActionResult UpdateRRR()
        {
            RemitaPayment remitaPayment = new RemitaPayment();
            try
            {

            }
            catch (Exception ex)
            {
                
                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateRRR(RemitaPayment remitaPayment)
        {
            try
            {
                if (remitaPayment != null && remitaPayment.payment.InvoiceNumber != null)
                {
                    RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                    remitaPayment = remitaLogic.GetModelBy(r => r.PAYMENT.Invoice_Number == remitaPayment.payment.InvoiceNumber || r.RRR == remitaPayment.payment.InvoiceNumber);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }
            return View(remitaPayment);
        }

        [HttpPost]
        public ActionResult SaveRRR(RemitaPayment remitaPayment)
        {

            try
            {
                if (remitaPayment != null)
                {
                    RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                    remitaPayment.Status = "01:";
                    remitaPayment.Description = "MANUAL PAYMENT ACCEPTANCE";
                    remitaLogic.Modify(remitaPayment);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("UpdateRRR");
        }
   
    }
}