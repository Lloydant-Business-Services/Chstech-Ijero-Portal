using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentPaymentLogic : BusinessBaseLogic<StudentPayment, STUDENT_PAYMENT>
    {
        private PaymentLogic paymentLogic;

        public StudentPaymentLogic()
        {
            base.translator = new StudentPaymentTranslator();
            paymentLogic = new PaymentLogic();
        }



        //public StudentPayment Add(StudentPayment studentPayment, ScratchCard scratchCard)
        //{
        //    try
        //    {

        //        Payment payment = paymentLogic.PayFeeByScratchCard(studentPayment, scratchCard);
        //        if (payment == null)
        //        {
        //            throw new Exception("Base Payment Object was not successfully added! Please try again.");
        //        }

        //        studentPayment.Id = payment.Id;
        //        StudentPayment newStudentPayment = this.Add(studentPayment);

        //        return newStudentPayment;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<StudentPaymentDetail> GetBy(ScratchCard scratchCard, SessionTerm sessionTerm)
        //{
        //    try
        //    {
        //        var paymentDetails = (from pd in repository.Fetch<VW_STDENT_PAYMENT>()
        //                              where pd.Serial_Number == scratchCard.SerialNumber && pd.Pin == scratchCard.Pin && pd.Session_Term_Id == sessionTerm.Id && pd.Usage_Count > 0
        //                              select new StudentPaymentDetail
        //                               {
        //                                   CardID = pd.Scratch_Card_Id,
        //                                   StudentID = pd.Person_Id,
        //                                   SessionTermID = pd.Session_Term_Id,
        //                                   PaymentID = pd.Payment_Id,
        //                                   CardSerialNumber = pd.Serial_Number,
        //                                   CardPin = pd.Pin,
        //                                   UsageCount = pd.Usage_Count,
        //                                   FirstUsedDate = pd.First_Used_Date,
        //                                   ExpiryDate = pd.Expiry_Date,
        //                                   UsageCountLimit = pd.Usage_Count_Limit,
        //                                   Price = pd.Price,
        //                                   CardTypeID = pd.Scratch_Card_Type_Id,
        //                                   CardBatchID = pd.Scratch_Card_Batch_Id,
        //                                   //StudentPaymentId = pd.Student_Payment_Id,
        //                               });

        //        return paymentDetails.ToList();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<StudentPaymentDetail> GetBy(ScratchCard scratchCard, SessionTerm sessionTerm, Student student)
        //{
        //    try
        //    {
        //        var paymentDetails = (from pd in repository.Fetch<VW_STDENT_PAYMENT>()
        //                              where pd.Serial_Number == scratchCard.SerialNumber && pd.Pin == scratchCard.Pin && pd.Session_Term_Id == sessionTerm.Id && pd.Usage_Count > 0 && pd.Person_Id == student.Id
        //                              select new StudentPaymentDetail
        //                              {
        //                                  CardID = pd.Scratch_Card_Id,
        //                                  StudentID = pd.Person_Id,
        //                                  SessionTermID = pd.Session_Term_Id,
        //                                  PaymentID = pd.Payment_Id,
        //                                  CardSerialNumber = pd.Serial_Number,
        //                                  CardPin = pd.Pin,
        //                                  UsageCount = pd.Usage_Count,
        //                                  FirstUsedDate = pd.First_Used_Date,
        //                                  ExpiryDate = pd.Expiry_Date,
        //                                  UsageCountLimit = pd.Usage_Count_Limit,
        //                                  Price = pd.Price,
        //                                  CardTypeID = pd.Scratch_Card_Type_Id,
        //                                  CardBatchID = pd.Scratch_Card_Batch_Id,
        //                                  //StudentPaymentId = pd.Student_Payment_Id,
        //                              });

        //        return paymentDetails.ToList();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public StudentPayment GetBy(Student student, SessionTerm sessionTerm)
        //{
        //    try
        //    {
        //        Func<STUDENT_PAYMENT, bool> selector = sp => sp.STUDENT.Person_Id == student.Id && sp.Session_Term_Id == sessionTerm.Id;
        //        return base.GetModelsBy(selector).LastOrDefault();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public StudentPayment GetBy(Student student, ScratchCard card)
        //{
        //    try
        //    {
        //        Func<STUDENT_PAYMENT, bool> selector = sp => sp.STUDENT.Person_Id == student.Id && sp.PAYMENT.CARD_PAYMENT.Scratch_Card_Id == card.Id;
        //        return base.GetModelBy(selector);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        

        //public override void ModifyHelper(StudentPayment studentPayment)
        //{
        //    try
        //    {
        //        STUDENT_PAYMENT studentPaymentEntity = GetEntityBy(sp => sp.Payment_Id == studentPayment.Id);
        //        if (studentPaymentEntity != null)
        //        {
        //            studentPaymentEntity.Payment_Id = studentPayment.Id;
        //            studentPaymentEntity.Person_Id = studentPayment.Student.Id;
        //            studentPaymentEntity.Session_Term_Id = studentPayment.SessionTerm.Id;
        //            studentPaymentEntity.Level_Id = studentPayment.Level.Id;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public ScratchCard GetUsedScratchCardBy(Student student)
        //{
        //    try
        //    {
        //        var card = (from sc in repository.Fetch<VW_STDENT_PAYMENT>()
        //                    where sc.Person_Id == student.Id
        //                    select new ScratchCard
        //                    {
        //                        Id = sc.Scratch_Card_Id,
        //                        SerialNumber = sc.Serial_Number,
        //                        Pin = sc.Pin,
        //                    }).FirstOrDefault();

        //        return card;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        ////public override void ModifyHelper(StudentPayment studentPayment)
        ////{
        ////    try
        ////    {
        ////        STUDENT_PAYMENT studentPaymentEntity = GetEntityBy(sp => sp.Student_Payment_Id == studentPayment.StudentPaymentId);
        ////        if (studentPaymentEntity != null)
        ////        {
        ////            studentPaymentEntity.Person_Id = studentPayment.Student.Id;
        ////            studentPaymentEntity.Payment_Id = studentPayment.Id;
        ////            studentPaymentEntity.Session_Term_Id = studentPayment.SessionTerm.Id;
        ////            studentPaymentEntity.Level_Id = studentPayment.Level.Id;
        ////        }
        ////    }
        ////    catch (Exception)
        ////    {
        ////        throw;
        ////    }
        ////}




    }
}
