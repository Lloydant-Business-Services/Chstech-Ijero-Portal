using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentScholarshipTranslator: TranslatorBase<PaymentScholarship,PAYMENT_SCHOLARSHIP>
    {
        private PersonTranslator personTranslator;
        private SessionTranslator sessionTranslator;
        public PaymentScholarshipTranslator()
        {
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override PaymentScholarship TranslateToModel(PAYMENT_SCHOLARSHIP scholarshipEntity)
        {
            try
            {
                PaymentScholarship paymentScholarship = null;
                if (scholarshipEntity != null)
                {
                    paymentScholarship = new PaymentScholarship();
                    paymentScholarship.Id = scholarshipEntity.Id;
                    paymentScholarship.Amount = scholarshipEntity.Amount;
                    paymentScholarship.ScholarshipName = scholarshipEntity.Scholarship_Name;
                    paymentScholarship.person = personTranslator.Translate(scholarshipEntity.PERSON);
                    paymentScholarship.session = sessionTranslator.Translate(scholarshipEntity.SESSION);
                }

                return paymentScholarship;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_SCHOLARSHIP TranslateToEntity(PaymentScholarship paymentScholarship)
        {
            try
            {
                PAYMENT_SCHOLARSHIP scholarshipEntity = null;
                if (paymentScholarship != null)
                {
                    scholarshipEntity = new PAYMENT_SCHOLARSHIP();
                    scholarshipEntity.Id = paymentScholarship.Id;
                    scholarshipEntity.Person_Id = paymentScholarship.person.Id;
                    scholarshipEntity.Amount = paymentScholarship.Amount;
                    scholarshipEntity.Scholarship_Name = paymentScholarship.ScholarshipName;
                    scholarshipEntity.Session_id = paymentScholarship.session.Id;

                }

                return scholarshipEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
