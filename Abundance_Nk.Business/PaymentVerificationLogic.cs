using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentVerificationLogic:BusinessBaseLogic<PaymentVerification,PAYMENT_VERIFICATION>
    {
       public PaymentVerificationLogic()
        {
            translator = new PaymentVerificationTranslator();
        }
       public PaymentVerification GetBy(Int64 PaymentId)
        {
            try
            {
                return GetModelBy(a => a.Payment_Id == PaymentId);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
       public override PaymentVerification Create(PaymentVerification model)
        {
            PaymentVerification paymentVerification = GetBy(model.Payment.Id);
            if (paymentVerification == null)
            {
                paymentVerification = base.Create(model);
            }
            return paymentVerification;

        }
       public List<PaymentVerificationReport> GetVerificationReport(Department department,Session session, Programme programme, Level level,FeeType feeType)
       {
           List<PaymentVerificationReport> paymentVerifications = null;
           if (level.Id == 1)
           {
               paymentVerifications =(from a in
            repository.GetBy<VW_PAYMENT_VERIFICATION_NEW_STUDENT>( a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Fee_Type_Id == feeType.Id && a.Confirmation_No != null)
            select new PaymentVerificationReport
            {
                Department = a.Department_Name,
                PaymentMode = a.Payment_Mode_Name,
                PaymentType = a.Fee_Type_Name,
                Level = "100 Level",
                StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                PaymentReference = a.Confirmation_No,
                Session = a.Session_Name,
                PaymentAmount = a.Transaction_Amount.ToString(),
                Programme = a.Programme_Name,
                FeeType = a.Fee_Type_Name,
                VerificationOfficer = a.VerificationOfficer
            }).ToList();
           }
           else
           {
                paymentVerifications =(from a in
                repository.GetBy<VW_PAYMENT_VERIFICATION_OLD_STUDENT>( a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Level_Id == level.Id && a.Fee_Type_Id == feeType.Id && a.Confirmation_No != null)
                select new PaymentVerificationReport
                {
                    Department = a.Department_Name,
                    PaymentMode = a.Payment_Mode_Name,
                    PaymentType = a.Fee_Type_Name,
                    Level = a.Level_Name,
                    StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                    PaymentReference = a.Confirmation_No,
                    Session = a.Session_Name,
                    PaymentAmount = a.Transaction_Amount.ToString(),
                    VerificationOfficer = a.VerificationOfficer
                }).ToList();
           }
           
            return paymentVerifications;
        }


    }
}
