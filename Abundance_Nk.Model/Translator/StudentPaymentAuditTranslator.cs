using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class StudentPaymentAuditTranslator :  TranslatorBase<StudentPaymentAudit, STUDENT_PAYMENT_AUDIT>
    {
             private readonly LevelTranslator levelTranslator;
        private readonly SessionTranslator sessionTranslator;
        private PersonTranslator personTranslator;
        private UserTranslator UserTranslator;

        public StudentPaymentAuditTranslator()
        {
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
            levelTranslator = new LevelTranslator();
            UserTranslator = new UserTranslator();
        }

        public override StudentPaymentAudit TranslateToModel(STUDENT_PAYMENT_AUDIT entity)
        {
            try
            {
                StudentPaymentAudit model = null;
                if (entity != null)
                {
                    model = new StudentPaymentAudit();
                    model.Id = entity.Student_Payment_Audit_Id;
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Amount = entity.Amount;
                    model.Status = entity.Status;
                    model.OldPerson = personTranslator.Translate(entity.PERSON1);
                    model.OldSession = sessionTranslator.Translate(entity.SESSION1);
                    model.OldLevel = levelTranslator.Translate(entity.LEVEL1);
                    model.OldAmount = entity.Old_Amount;
                    model.User = UserTranslator.Translate(entity.USER);
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_PAYMENT_AUDIT TranslateToEntity(StudentPaymentAudit model)
        {
            try
            {
                STUDENT_PAYMENT_AUDIT entity = new STUDENT_PAYMENT_AUDIT();
                if (model != null)
                {
                    entity = new STUDENT_PAYMENT_AUDIT();
                    entity.Student_Payment_Audit_Id = model.Id;
                   entity.Payment_Id = model.StudentPayment.Id;
                   entity.Person_Id = model.Person.Id;
                   entity.Session_Id = model.Session.Id;
                   entity.Level_Id = model.Level.Id;
                   entity.Amount = model.Amount;
                   entity.Status = model.Status;
                   entity.Old_Person_Id = model.OldPerson.Id;
                   entity.Old_Session_Id = model.OldSession.Id;
                   entity.Old_Level_Id = model.OldLevel.Id;
                   entity.Old_Amount = model.OldAmount;
                   entity.Old_Status = model.OldStatus;
                   entity.User_Id = model.User.Id;
                   entity.Operation = model.Operation;
                   entity.Action = model.Action;
                   entity.Client = model.Client;
                   entity.Time = model.Time;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
