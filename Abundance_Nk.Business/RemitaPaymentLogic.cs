using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
  public  class RemitaPaymentLogic : BusinessBaseLogic<RemitaPayment, REMITA_PAYMENT>
    {
      public RemitaPaymentLogic()
        {
            translator = new RemitaPaymentTranslator();
        }

      public RemitaPayment GetBy(Payment payment)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

      public RemitaPayment GetBy(long PaymentID)
      {
          try
          {
              Expression<Func<REMITA_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
              return GetModelBy(selector);

          }
          catch (Exception)
          {
              throw;
          }
      }

      private REMITA_PAYMENT GetEntityBy(RemitaPayment remitaPayment)
      {
          try
          {
              Expression<Func<REMITA_PAYMENT, bool>> selector = s => s.PAYMENT.Payment_Id == remitaPayment.payment.Id;
              REMITA_PAYMENT entity = GetEntityBy(selector);

              return entity;
          }
          catch (Exception)
          {
              throw;
          }
      }


      public RemitaPayment GetBy(string OrderId)
      {
          try
          {
              Expression<Func<REMITA_PAYMENT, bool>> selector = s => s.OrderId == OrderId;
              return GetModelBy(selector);
          }
          catch (Exception)
          {
              throw;
          }
      }


      public bool Modify (RemitaPayment remitaPayment)
      {
          try
          {
                PaymentLogic paymentLogic = new PaymentLogic();
                REMITA_PAYMENT entity = GetEntityBy(remitaPayment);

              if (entity == null)
              {
                  throw new Exception(NoItemFound);
              }

              entity.RRR = remitaPayment.RRR;
              entity.Status = remitaPayment.Status;
              entity.Transaction_Date = remitaPayment.TransactionDate;

              if (remitaPayment.BankCode != null)
              {
                  entity.Bank_Code = remitaPayment.BankCode;
              }
              if (remitaPayment.MerchantCode != null)
              {
                  entity.Merchant_Code = remitaPayment.MerchantCode;
              }
             
              if (remitaPayment.TransactionAmount > 0)
              {
                  entity.Transaction_Amount = remitaPayment.TransactionAmount;
              }
                if (remitaPayment.Description != null)
                {
                    entity.Description = remitaPayment.Description;
                }
            

              int modifiedRecordCount = Save();
              if (modifiedRecordCount <= 0)
              {
                    paymentLogic.UpdatePaymentRecord(remitaPayment.payment.Id, remitaPayment.TransactionDate);
                    return false;
                   
              }
                paymentLogic.UpdatePaymentRecord(remitaPayment.payment.Id, remitaPayment.TransactionDate);
                return true;
          }
          catch (Exception)
          {
              throw;
          }
      }

      public void DeleteBy(long PaymentID)
      {
          try
          {
              Expression<Func<REMITA_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
              Delete(selector);
          }
          catch (Exception)
          {
              throw;
          }
      }

      public bool HasStudentPaidFirstInstallmentOrCompletedFeesForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {

                Expression<Func<REMITA_PAYMENT, bool>> selector = p => (p.PAYMENT.Person_Id == StudentId && p.PAYMENT.Session_Id == session.Id && p.PAYMENT.Fee_Type_Id == feeType.Id && p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Full) || (p.PAYMENT.Person_Id == StudentId && p.PAYMENT.Session_Id == session.Id && p.PAYMENT.Fee_Type_Id == feeType.Id && p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.FirstInstallment);
                RemitaPayment payment = GetModelBy(selector);
                if (payment != null && payment.RRR != null && payment.Status.Contains("01"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

      public bool HasStudentCompletedFeesForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {
                
                Expression<Func<REMITA_PAYMENT, bool>> selector =
                    p =>
                        (p.PAYMENT.Person_Id == StudentId &&
                         p.PAYMENT.Session_Id == session.Id &&
                         p.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Full) ||
                        (p.PAYMENT.Person_Id == StudentId &&
                         p.PAYMENT.Session_Id == session.Id &&
                         p.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.SecondInstallment);
                RemitaPayment payment = GetModelBy(selector);
                if (payment != null && payment.ConfirmationNo != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
        public int NumberofStudentThatPaidSchoolFees(List<long> studentIds,Session session, FeeType feeType, Programme programme)
        {
            try
            {
                int count = 0;
                foreach(var studentId in studentIds)
                {
                    if (studentId == 12255)
                    {
                        var alertme = 0;
                    }
                    Expression<Func<REMITA_PAYMENT, bool>> selector =
                    p =>
                        (p.PAYMENT.Person_Id==studentId &&
                         p.PAYMENT.Session_Id == session.Id &&
                         p.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Full) ||
                        (p.PAYMENT.Person_Id == studentId &&
                         p.PAYMENT.Session_Id == session.Id &&
                         p.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.PAYMENT.Payment_Mode_Id == (int)PaymentModes.SecondInstallment);
                    RemitaPayment payment = GetModelBy(selector);
                    if (payment != null&& payment.RRR != null && payment.Status.Contains("01"))
                    {
                        count += 1;
                    }
                }
                return count;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

  
  }


}
