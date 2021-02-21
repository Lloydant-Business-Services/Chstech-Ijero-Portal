using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Model.Model
{
    public class RemitaPayementProcessor
    {
        private string apiKey;
        private RemitaResponse remitaResponse;
        public RemitaPayementProcessor(string _apiKey)
        {
            apiKey = _apiKey;
        }
        public string HashPaymentDetailToSHA512(string hash_string)
        {
            System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
            return hashed;

        }
        public RemitaResponse PostJsonDataToUrl(string baseAddress,Remita remitaObj, Payment payment)
          {
              remitaResponse = new RemitaResponse();
              try
              {
                  string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + apiKey;
          
                  string json = "";
                  string jsondata = "";
                  if (remitaObj != null)
                  {
                      remitaObj.hash = HashPaymentDetailToSHA512(toHash);
                      json = new JavaScriptSerializer().Serialize(remitaObj);
                      using (var request = new WebClient())
                      {
                          request.Headers[HttpRequestHeader.Accept] = "application/json";
                          request.Headers[HttpRequestHeader.ContentType] = "application/json";
                          jsondata = request.UploadString(baseAddress, "POST", json);
                      
                      }
                      jsondata = jsondata.Replace("jsonp(", "");
                      jsondata = jsondata.Replace(")", "");
                     
                      remitaResponse = new JavaScriptSerializer().Deserialize<RemitaResponse>(jsondata);

                  }
              }
              catch (Exception ex)
              {
                  remitaResponse.Message = ex.Message;
                  throw ex;
              }
              return remitaResponse;
          }
        public RemitaResponse PostHtmlDataToUrl(string baseAddress, Remita remitaObj, Payment payment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + apiKey;

                string param = "";
                string postdata = "";
                if (remitaObj != null)
                {
                    remitaObj.payerEmail = string.IsNullOrEmpty(remitaObj.payerEmail) ? "support@lloydant.com" : remitaObj.payerEmail;

                    remitaObj.hash = HashPaymentDetailToSHA512(toHash);
                    param = "payerName="+remitaObj.payerName+"&merchantId="+remitaObj.merchantId+"&serviceTypeId="+remitaObj.serviceTypeId+"&orderId="+remitaObj.orderId+"&hash="+remitaObj.hash+"&payerEmail="+remitaObj.payerEmail+"&payerPhone="+remitaObj.payerPhone+"&amt="+remitaObj.totalAmount+"&responseurl="+remitaObj.responseurl ;

                    var request = (HttpWebRequest)WebRequest.Create("https://login.remita.net/remita/ecomm/init.reg");

                    var postData = "payerName="+remitaObj.payerName;
                    postData += "&merchantId="+remitaObj.merchantId;
                    postData += "&serviceTypeId="+remitaObj.serviceTypeId;
                    postData += "&orderId="+remitaObj.orderId;
                    postData += "&hash="+remitaObj.hash;
                    postData += "&payerEmail="+remitaObj.payerEmail;
                    postData += "&payerPhone="+remitaObj.payerPhone;
                    postData += "&amt="+remitaObj.totalAmount;
                    postData += "&responseurl=" + remitaObj.responseurl;
                    postData += "&paymenttype=RRRGEN";

                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using(var stream = request.GetRequestStream())
                    {
                        stream.Write(data,0,data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    var urlResponse = response.ResponseUri;
                    string queryString = urlResponse.Query;
                    remitaResponse.RRR = HttpUtility.ParseQueryString(queryString).Get("RRR");
                    remitaResponse.orderId = HttpUtility.ParseQueryString(queryString).Get("orderID");
                    remitaResponse.StatusCode = HttpUtility.ParseQueryString(queryString).Get("statuscode");
                    remitaResponse.Status = HttpUtility.ParseQueryString(queryString).Get("status");
                   
                }
            }
            catch (Exception ex)
            {
                remitaResponse.Message = ex.Message;
                throw ex;
            }
            return remitaResponse;
        }
        public RemitaResponse TransactionStatus(string baseAddress, RemitaPayment remitaPayment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string json = "";
                string jsondata = "";
                if (remitaPayment != null)
                {
                    string toHash = remitaPayment.RRR.Trim() + apiKey.Trim() + remitaPayment.MerchantCode.Trim();
                    string hash = HashPaymentDetailToSHA512(toHash);
                    string URI = baseAddress;
                    //string myParameters = "/" + remitaPayment.MerchantCode + "/" + remitaPayment.RRR + "/" + hash + "/json/status.reg";
                    string myParameters = "/" + remitaPayment.MerchantCode + "/" + remitaPayment.RRR + "/" + hash + "/status.reg";
                    json = URI + myParameters;
                    jsondata = new WebClient().DownloadString(json);
                    remitaResponse = new JavaScriptSerializer().Deserialize<RemitaResponse>(jsondata);

                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return remitaResponse;
        }
        public RemitaPayment GenerateRRR(string ivn,string remitaBaseUrl,string description, List<RemitaSplitItems> splitItems, RemitaSettings settings, decimal? Amount)
        {
            try
            {
                RemitaPayment remitaPyament = new RemitaPayment();
                RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                Payment payment = new Payment();
                PaymentLogic pL = new PaymentLogic();
                payment = pL.GetModelBy(p => p.Invoice_Number == ivn);

                if (String.IsNullOrEmpty(payment.Person.Email))
                {
                    payment.Person.Email = "support@fedpolyado.edu.ng";
                }

                if (Amount == 0)
                {
                    Amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                }
                long milliseconds = DateTime.Now.Ticks;
               string testid =  milliseconds.ToString();
                RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                Remita remita = new Remita()
                {
                    merchantId = settings.MarchantId,
                    serviceTypeId = settings.serviceTypeId,
                    orderId = testid,
                    totalAmount = (decimal)Amount,
                    payerName = payment.Person.FullName,
                    payerEmail =  payment.Person.Email,
                    payerPhone = payment.Person.MobilePhone,
                    responseurl = settings.Response_Url,
                    lineItems = splitItems,
                };

                RemitaResponse remitaResponse = remitaPayementProcessor.PostJsonDataToUrl(remitaBaseUrl, remita, payment);
                if (remitaResponse.Status != null && remitaResponse.StatusCode.Equals("025"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament.payment = payment;
                    remitaPyament.RRR = remitaResponse.RRR;
                    remitaPyament.OrderId = remitaResponse.orderId;
                    remitaPyament.Status = remitaResponse.StatusCode + ":" + remitaResponse.Status;
                    remitaPyament.TransactionAmount = remita.totalAmount;
                    remitaPyament.TransactionDate = DateTime.Now;
                    remitaPyament.MerchantCode = remita.merchantId;
                    remitaPyament.Description = description;
                    if (remitaLogic.GetBy(payment.Id) == null)
                    {
                        remitaLogic.Create(remitaPyament);
                    }
                    else
                    {
                        remitaLogic.Modify(remitaPyament);
                    }

                    return remitaPyament;

                }
                else if (remitaResponse.StatusCode.Trim().Equals("028"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament = remitaLogic.GetModelBy(r => r.OrderId == payment.InvoiceNumber);
                    if (remitaPyament != null)
                    {
                        return remitaPyament;
                    }
                }
                remitaPyament = null;
                return remitaPyament;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Use only for non split transactions
        public RemitaPayment GenerateRRRCard(string ivn, string remitaBaseUrl, string description,  RemitaSettings settings, decimal? Amount)
        {
            try
            {
                RemitaPayment remitaPyament = new RemitaPayment();
                RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                Payment payment = new Payment();
                PaymentLogic pL = new PaymentLogic();
                payment = pL.GetModelBy(p => p.Invoice_Number == ivn);

                if (payment.Person.Email == null)
                {
                    payment.Person.Email = "support@fedpolyado.edu.ng";
                }

                if (Amount == 0)
                {
                    Amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                }
                long milliseconds = DateTime.Now.Ticks;
                string testid = milliseconds.ToString();
                RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                Remita remita = new Remita()
                {
                    merchantId = settings.MarchantId,
                    serviceTypeId = settings.serviceTypeId,
                    orderId = testid,
                    totalAmount = (decimal)Amount,
                    payerName = payment.Person.FullName,
                    payerEmail = payment.Person.Email,
                    payerPhone = payment.Person.MobilePhone,
                    responseurl = settings.Response_Url,
                    paymenttype = "MasterCard",
                };

                RemitaResponse remitaResponse = remitaPayementProcessor.PostHtmlDataToUrl(remitaBaseUrl, remita, payment);
                if (remitaResponse.Status != null && remitaResponse.StatusCode.Equals("025"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament.payment = payment;
                    remitaPyament.RRR = remitaResponse.RRR;
                    remitaPyament.OrderId = remitaResponse.orderId;
                    remitaPyament.Status = remitaResponse.StatusCode + ":" + remitaResponse.Status;
                    remitaPyament.TransactionAmount = remita.totalAmount;
                    remitaPyament.TransactionDate = DateTime.Now;
                    remitaPyament.MerchantCode = remita.merchantId;
                    remitaPyament.Description = description;
                    if (remitaLogic.GetBy(payment.Id) == null)
                    {
                        remitaLogic.Create(remitaPyament);
                    }

                    return remitaPyament;

                }
                else if (remitaResponse.StatusCode.Trim().Equals("028"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament = remitaLogic.GetModelBy(r => r.OrderId == payment.InvoiceNumber);
                    if (remitaPyament != null)
                    {
                        return remitaPyament;
                    }
                }
                remitaPyament = null;
                return remitaPyament;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void GetTransactionStatus(string rrr, string remitaVerifyUrl, int RemitaSettingId)
        {
            RemitaSettings settings = new RemitaSettings();
            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == RemitaSettingId);
            RemitaResponse remitaResponse = new RemitaResponse();
            RemitaPayment remitaPayment = new RemitaPayment();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelsBy(m => m.RRR == rrr).FirstOrDefault(); 
            remitaResponse = TransactionStatus(remitaVerifyUrl, remitaPayment);
            if (remitaResponse != null && remitaResponse.Status != null)
            {
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPaymentLogic.Modify(remitaPayment);
            }
        }

        public RemitaPayment GetStatus(string order_Id)
        {
            RemitaSettings settings = new RemitaSettings();
            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
            RemitaResponse remitaResponse = new RemitaResponse();
            RemitaPayment remitaPayment = new RemitaPayment();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == order_Id);
            //string remitaVerifyUrl = "https://login.remita.net/remita/ecomm";
            string remitaVerifyUrl = " https://login.remita.net/remita/exapp/api/v1/send/api/echannelsvc/";


            RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
            remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
            if (remitaResponse != null && remitaResponse.Status != null)
            {
                if (remitaResponse.Status.Contains("00"))
                {
                    remitaResponse.Status = "01";
                }
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPayment.TransactionDate = remitaResponse.paymentDate != null ? DateTime.Parse(remitaResponse.paymentDate) : remitaPayment.TransactionDate;
                remitaPaymentLogic.Modify(remitaPayment);
                return remitaPayment;
            }
            return remitaPayment;
        }

     
    
    }
   
} 
