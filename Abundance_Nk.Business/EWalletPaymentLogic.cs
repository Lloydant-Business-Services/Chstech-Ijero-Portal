using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class EWalletPaymentLogic : BusinessBaseLogic<EWalletPayment, E_WALLET_PAYMENT>
    {
        public EWalletPaymentLogic()
        {
            translator = new EWalletPaymentTranslator();
        }
        public bool Modify(EWalletPayment model)
        {
            try
            {
                Expression<Func<E_WALLET_PAYMENT, bool>> selector = a => a.Id == model.Id;
                E_WALLET_PAYMENT entity = GetEntityBy(selector);

                if (entity != null && entity.Id > 0)
                {
                    if (model.Payment != null)
                    {
                        entity.Payment_Id = model.Payment.Id;
                    }
                    if (model.Student != null)
                    {
                        entity.Student_Id = model.Student.Id;
                    }
                    if (model.FeeType != null)
                    {
                        entity.Fee_Type_Id = model.FeeType.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    if (model.Person != null)
                    {
                        entity.Person_Id = model.Person.Id;
                    }

                    entity.Amount = model.Amount;

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetPaymentStatus(EWalletPayment eWalletPayment)
        {
            try
            {
                if (eWalletPayment != null && eWalletPayment.Id > 0)
                {
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == eWalletPayment.Payment.Id);

                    if (remitaPayment != null && !remitaPayment.Status.Contains("01") && !remitaPayment.Description.ToLower().Contains("manual") && !remitaPayment.Status.Contains("00"))
                    {
                        remitaPayment = UpdateStudentRRRPayment(remitaPayment.payment.Person, remitaPayment);
                    }
                    
                    return remitaPayment.Status.Contains("01") || remitaPayment.Status.Contains("00") || remitaPayment.Description.ToLower().Contains("manual");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
        private RemitaPayment UpdateStudentRRRPayment(Person person, RemitaPayment remitaPayment)
        {
            try
            {
                if (person != null)
                {
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    RemitaResponse remitaResponse = new RemitaResponse();
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                    settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
                    string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                    
                    remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
                    if (remitaResponse != null && remitaResponse.Status != null)
                    {
                        remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                        remitaPaymentLogic.Modify(remitaPayment);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return remitaPayment;
        }
        public string GetPaymentPin(EWalletPayment eWalletPayment)
        {
            try
            {
                if (eWalletPayment != null && eWalletPayment.Id > 0)
                {
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == eWalletPayment.Payment.Id);

                    return remitaPayment != null ? remitaPayment.RRR : eWalletPayment.Payment.InvoiceNumber;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
    }
}
