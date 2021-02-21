using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

using Abundance_Nk.Business.eTranzactWebService;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using System.Transactions;
using System.Net;
using System.IO;

namespace Abundance_Nk.Business
{
    public class PaymentEtranzactLogic : BusinessBaseLogic<PaymentEtranzact, PAYMENT_ETRANZACT>
    {
        string baseUrl = "http://www.etranzact.net/WebConnectPlus/query.jsp";
        public PaymentEtranzactLogic()
        {
            translator = new PaymentEtranzactTranslator();
        }

        public PaymentEtranzact GetBy(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == payment.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        } 
     
        //Function to retrieve payment details from eTranzact
        public bool ValidatePin(PaymentEtranzact etranzactPayment, Payment payment, decimal Amount)
        {
            try
            {
                PaymentScholarship scholarship = new PaymentScholarship();
                PaymentScholarshipLogic scholarshipLogic = new PaymentScholarshipLogic();
                if (scholarshipLogic.IsStudentOnScholarship(payment.Person, payment.Session))
                {
                    scholarship = scholarshipLogic.GetBy(payment.Person);
                    Amount = Amount - scholarship.Amount;
                }
                if (Amount == 2500 || Amount == 2900)
                {
                    return true;
                }

                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Confirmation_No == etranzactPayment.ConfirmationNo && p.Customer_Id == payment.InvoiceNumber && p.Transaction_Amount == Amount;
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinUsed(string confirmationOrderNumber,int personId)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Confirmation_No == confirmationOrderNumber && p.Used == true && p.Used_By_Person_Id != personId;
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzact RetrievePin(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";

                Hashtable hsParams = new Hashtable();
                hsParams.Clear();
                queryPayoutletTransaction payoutletTransaction = new queryPayoutletTransaction();
                queryPayoutletTransactionResponse gateWayResponse = new queryPayoutletTransactionResponse();

                payoutletTransaction.confirmationNo = confirmationNo.Trim();

                payoutletTransaction.terminalId = TerminalID.TerminalId;
                eTranzactWebService.QueryPayoutletTransactionClient ws = new QueryPayoutletTransactionClient();

                gateWayResponse = ws.queryPayoutletTransaction(payoutletTransaction);
                string Result = gateWayResponse.@return;

                PaymentEtranzact paymentEtz = new PaymentEtranzact();
                if (Result != "-1")
                {
                    String[] RSplit = Result.Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }

                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    //Session = "1";
                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false; 
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;

                    PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == TerminalID.FeeType.Id).LastOrDefault();                
                    paymentEtz.EtranzactType = paymentEtranzactType;

                    Payment payment = new Payment();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == CustomerId);
                   
                    if (payment != null)
                    {
                        OnlinePayment onlinePayment = new OnlinePayment();
                        OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                        onlinePayment = onlinePaymentLogic.GetModelBy(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);

                        paymentEtz.Payment = onlinePayment;
                    }

                    base.Create(paymentEtz);
                    return paymentEtz;
                }

               
                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePin(Payment payment, Person person)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT_ETRANZACT paymentEtranzactEntity = GetEntityBy(selector);

                if (paymentEtranzactEntity == null || paymentEtranzactEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                paymentEtranzactEntity.Used = true;
                paymentEtranzactEntity.Used_By_Person_Id = person.Id;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzact RetrievePinsWithoutInvoice(string confirmationNo, string ivn, int feeType_Id, PaymentTerminal TerminalID)
        {
            try
            {
                PaymentEtranzact paymentEtz = new PaymentEtranzact();
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    string ReceiptNo = "";
                    string PaymentCode = "";
                    string MerchantCode = "";
                    string TransactionAmount = "";
                    string TransactionDescription = "";
                    string BankCode = "";
                    string BankBranchCode = "";
                    string CustomerName = "";
                    string CustomerAddress = "";
                    string CustomerId = "";
                    //string Session = "";

                    Hashtable hsParams = new Hashtable();
                    hsParams.Clear();
                    queryPayoutletTransaction payoutletTransaction = new queryPayoutletTransaction();
                    queryPayoutletTransactionResponse gateWayResponse = new queryPayoutletTransactionResponse();

                    payoutletTransaction.confirmationNo = confirmationNo.Trim();

                    payoutletTransaction.terminalId = TerminalID.TerminalId;
                    eTranzactWebService.QueryPayoutletTransactionClient ws = new QueryPayoutletTransactionClient();

                    gateWayResponse = ws.queryPayoutletTransaction(payoutletTransaction);
                    string Result = gateWayResponse.@return;


                    if (Result != "-1")
                    {
                        String[] RSplit = Result.Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                        String[] Rsplitx;
                        foreach (string s in RSplit)
                        {
                            Rsplitx = s.Split('=');
                            hsParams.Add(Rsplitx[0], Rsplitx[1]);
                        }

                        ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                        PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                        MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                        TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                        TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                        BankCode = hsParams["BANK_CODE"].ToString().Trim();
                        BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                        CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                        CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                        CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                        //Session = "1";
                        hsParams.Clear();

                        paymentEtz.BankCode = BankCode;
                        paymentEtz.BranchCode = BankBranchCode;
                        paymentEtz.ConfirmationNo = PaymentCode;
                        paymentEtz.CustomerAddress = CustomerAddress;
                        paymentEtz.CustomerName = CustomerName;
                        paymentEtz.MerchantCode = MerchantCode;
                        paymentEtz.PaymentCode = PaymentCode;
                        paymentEtz.ReceiptNo = ReceiptNo;
                        paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                        paymentEtz.TransactionDate = DateTime.Now;
                        paymentEtz.TransactionDescription = TransactionDescription;
                        paymentEtz.Used = false;
                        paymentEtz.Terminal = TerminalID;
                        paymentEtz.UsedBy = 0;

                       
                      
                        Session CurrentSession = GetCurrentSession();
                        FeeType feeType = new FeeType();
                        FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                        feeType = feeTypeLogic.GetModelBy(f => f.Fee_Type_Id == feeType_Id);
                        PaymentEtranzactType pet = new PaymentEtranzactType();
                        pet = GetPaymentTypeBy(feeType);
                       
                        PaymentLogic paymentLogic = new PaymentLogic();
                        Payment payment = paymentLogic.GetBy(ivn);

                       
                        PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                        PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == feeType.Id);

                        paymentEtz.EtranzactType = paymentEtranzactType;


                        if (payment != null)
                        {
                            OnlinePayment onlinePayment = new OnlinePayment();
                            OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                            onlinePayment = onlinePaymentLogic.GetModelBy(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);
                            paymentEtz.Payment = onlinePayment;
                        }



                        paymentEtz.CustomerID = payment.InvoiceNumber;

                        base.Create(paymentEtz);
                        transaction.Complete();
                        return paymentEtz;
                    }
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
        //public FeeType GetFeeTypeBy(Session session, Programme programme)
        //{
        //    try
        //    {
        //        ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
        //        ApplicationProgrammeFee A = new ApplicationProgrammeFee();
        //        A = programmeFeeLogic.GetBy(programme, session);

        //        if (A != null)
        //        {
        //            return A.FeeType;
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public PaymentEtranzactType GetPaymentTypeBy(FeeType feeType)
        {
            PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
            PaymentEtranzactType p = new PaymentEtranzactType();
            p = paymentEtranzactTypeLogic.GetBy(feeType);

            if (p != null)
            {
                return p;
            }

            return null;
        }

        public ApplicationFormSetting GetApplicationFormSettingBy(Session session, FeeType feeType)
        {
            try
            {
                ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();
                return applicationFormSettingLogic.GetBy(session, feeType);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Session GetCurrentSession()
        {
            try
            {
                CurrentSessionSemesterLogic currentSessionLogic = new CurrentSessionSemesterLogic();
                CurrentSessionSemester currentSessionSemester = currentSessionLogic.GetCurrentSessionTerm();

                if (currentSessionSemester != null && currentSessionSemester.SessionSemester != null)
                {
                    return currentSessionSemester.SessionSemester.Session;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FeeType GetFeeTypeBy(Session session, Programme programme)
        {
            try
            {
                ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                List<ApplicationProgrammeFee> applicationProgrammeFess = programmeFeeLogic.GetListBy(programme, session);
                foreach (ApplicationProgrammeFee item in applicationProgrammeFess)
                {
                    if (item.FeeType.Id <= 6)
                    {
                        return item.FeeType;
                    }
                }
              
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzact RetrieveEtranzactWebServicePinDetails(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";

                Hashtable hsParams = new Hashtable();
                hsParams.Clear();
                string Result = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = "TERMINAL_ID=" + TerminalID.TerminalId + "&CONFIRMATION_NO=" + confirmationNo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                Result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();


                PaymentEtranzact paymentEtz = new PaymentEtranzact();
                if (Result != "-1" && Result != "\r\n\r\nSUCCESS=-1\r\n\r\n\r\n</html>" && Result.Length > 10)
                {
                    String[] RSplit = Result.Replace("\r\n", "").Replace("</html>", "").Replace("=%20&", "=FPIDEFAULT&").Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }


                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();

                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false;
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;                   
                    return paymentEtz;
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PaymentEtranzact RetrievePinAlternative(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                //if (!string.IsNullOrEmpty(confirmationNo) && confirmationNo.StartsWith("768"))
                //{
                //    throw new Exception("Pin not recognised, contact your system administrator.");
                //}

                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";


                Hashtable hsParams = new Hashtable();
                hsParams.Clear();
                string Result = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = "TERMINAL_ID=" + TerminalID.TerminalId + "&CONFIRMATION_NO=" + confirmationNo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                Result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();


                PaymentEtranzact paymentEtz = new PaymentEtranzact();
                if (Result != "-1" && Result.Length > 10)
                {
                    String[] RSplit = Result.Replace("\r\n", "").Replace("</html>", "").Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }

                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    //Session = "1";
                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false;
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;

                    PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    if (TerminalID.Session == null || TerminalID.Session.Id <= 0)
                    {
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == TerminalID.FeeType.Id && p.Session_Id == 7).FirstOrDefault();
                    }
                    else
                    {
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == TerminalID.FeeType.Id && p.Session_Id == TerminalID.Session.Id).FirstOrDefault();
                    }

                    paymentEtz.EtranzactType = paymentEtranzactType;

                    Payment payment = new Payment();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    //CustomerId
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == CustomerId);

                    if (payment != null)
                    {
                        OnlinePayment onlinePayment = new OnlinePayment();
                        OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                        onlinePayment = onlinePaymentLogic.GetModelBy(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);

                        paymentEtz.Payment = onlinePayment;
                    }

                    base.Create(paymentEtz);
                    return paymentEtz;
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public bool Modify(PaymentEtranzact paymentEtranzact)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == paymentEtranzact.Payment.Payment.Id;
                PAYMENT_ETRANZACT entity = GetEntityBy(selector);
                entity.Transaction_Amount = paymentEtranzact.TransactionAmount;
                
                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinOnTable(string confirmationOrderNumber)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Confirmation_No == confirmationOrderNumber;
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
