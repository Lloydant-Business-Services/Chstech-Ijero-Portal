using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentPaymentTranslator : TranslatorBase<StudentPayment, STUDENT_PAYMENT>
    {
        private readonly LevelTranslator levelTranslator;
        private readonly SessionTranslator sessionTranslator;
        private PersonTranslator personTranslator;

        public StudentPaymentTranslator()
        {
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
            levelTranslator = new LevelTranslator();
        }

        public override StudentPayment TranslateToModel(STUDENT_PAYMENT studentPaymentEntity)
        {
            try
            {
                StudentPayment studentPayment = null;
                if (studentPaymentEntity != null)
                {
                    studentPayment = new StudentPayment();
                    studentPayment.Id = studentPaymentEntity.Payment_Id;
                    studentPayment.Session = sessionTranslator.TranslateToModel(studentPaymentEntity.SESSION);
                    studentPayment.Level = levelTranslator.TranslateToModel(studentPaymentEntity.LEVEL);
                    studentPayment.Amount = studentPaymentEntity.Amount;
                    studentPayment.Status = studentPaymentEntity.Status;
                    studentPayment.Person = personTranslator.Translate(studentPaymentEntity.PERSON);
                }

                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_PAYMENT TranslateToEntity(StudentPayment studentPayment)
        {
            try
            {
                STUDENT_PAYMENT studentPaymentEntity = null;
                if (studentPayment != null)
                {
                    studentPaymentEntity = new STUDENT_PAYMENT();
                    studentPaymentEntity.Payment_Id = studentPayment.Id;
                    if (studentPayment.Student != null)
                    {
                        studentPaymentEntity.Person_Id = studentPayment.Student.Id;
                    }
                    if (studentPayment.Person != null)
                    {
                        studentPaymentEntity.Person_Id = studentPayment.Person.Id;
                    }
                    studentPaymentEntity.Session_Id = studentPayment.Session.Id;
                    studentPaymentEntity.Level_Id = studentPayment.Level.Id;
                    studentPaymentEntity.Status = studentPayment.Status;
                    studentPaymentEntity.Amount = studentPayment.Amount;
                }

                return studentPaymentEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}