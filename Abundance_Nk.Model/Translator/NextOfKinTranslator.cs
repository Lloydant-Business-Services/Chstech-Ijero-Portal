using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
   public class NextOfKinTranslator : TranslatorBase<NextOfKin, NEXT_OF_KIN>
    {
       private AbilityTranslator abilityTranslator;
       private RelationshipTranslator relationshipTranslator;
       private ApplicationFormTranslator applicationFormTranslator;
       private PersonTypeTranslator personTypeTranslator;
       private PersonTranslator personTranslator;
       
       public NextOfKinTranslator()
       {
           personTranslator = new PersonTranslator();
           relationshipTranslator = new RelationshipTranslator();
           applicationFormTranslator = new ApplicationFormTranslator();
           personTypeTranslator = new PersonTypeTranslator();
           abilityTranslator = new AbilityTranslator();
       }

       public override NextOfKin TranslateToModel(NEXT_OF_KIN sponsorEntity)
        {
            try
            {
                NextOfKin sponsor = null;
                if (sponsorEntity != null)
                {
                    sponsor = new NextOfKin();
                    sponsor.Person = personTranslator.Translate(sponsorEntity.PERSON);
                    sponsor.Relationship = relationshipTranslator.TranslateToModel(sponsorEntity.RELATIONSHIP);
                    sponsor.Name = sponsorEntity.Sponsor_Name;
                    sponsor.PersonType = personTypeTranslator.Translate(sponsorEntity.PERSON_TYPE);
                    sponsor.ContactAddress = sponsorEntity.Sponsor_Contact_Address;
                    sponsor.MobilePhone = sponsorEntity.Sponsor_Mobile_Phone;
                    sponsor.ApplicationForm = applicationFormTranslator.Translate(sponsorEntity.APPLICATION_FORM);
                }

                return sponsor;
            }
            catch (Exception)
            {
                throw;
            }
        }

       public override NEXT_OF_KIN TranslateToEntity(NextOfKin sponsor)
       {
           try
           {
               NEXT_OF_KIN sponsorEntity = null;
               if (sponsor != null)
               {
                   sponsorEntity = new NEXT_OF_KIN();
                   sponsorEntity.Person_Id = sponsor.Person.Id;
                   sponsorEntity.Person_Type_Id = sponsor.PersonType.Id;
                   sponsorEntity.Relationship_Id = sponsor.Relationship.Id;
                   sponsorEntity.Sponsor_Name = sponsor.Name;
                   sponsorEntity.Sponsor_Contact_Address = sponsor.ContactAddress;
                   sponsorEntity.Sponsor_Mobile_Phone = sponsor.MobilePhone;

                   if (sponsor.ApplicationForm != null && sponsor.ApplicationForm.Id > 0)
                   {
                       sponsorEntity.Application_Form_Id = sponsor.ApplicationForm.Id;
                   }
               }

               return sponsorEntity;
           }
           catch (Exception)
           {
               throw;
           }
       }






    }
}
