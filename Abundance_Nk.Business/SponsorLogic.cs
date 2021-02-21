using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SponsorLogic : BusinessBaseLogic<Sponsor, APPLICANT_SPONSOR>
    {
        public SponsorLogic()
        {
            translator = new SponsorTranslator();
        }

         public bool Modify(Sponsor sponsor)
        {
            try
            {
                APPLICANT_SPONSOR entity = GetEntityBy(sponsor.Person);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Relationship_Id = sponsor.Relationship.Id;
                entity.Sponsor_Contact_Address = sponsor.ContactAddress;
                entity.Sponsor_Mobile_Phone = sponsor.MobilePhone;
                entity.Sponsor_Name = sponsor.Name;

                if (sponsor.ApplicationForm != null)
                {
                    entity.Application_Form_Id = sponsor.ApplicationForm.Id;
                }
                if (sponsor.Person != null)
                {
                    entity.Person_Id = sponsor.Person.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    
         private APPLICANT_SPONSOR GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_SPONSOR, bool>> selector = s => s.Person_Id == person.Id;
                APPLICANT_SPONSOR entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

      
    }



}
