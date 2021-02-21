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
   public class PersonAuditLogic : BusinessBaseLogic<PersonAudit, PERSON_AUDIT>
    {
       public PersonAuditLogic()
        {
            translator = new PersonAuditTranslator();
        }


       public bool Add(Person OldDetails, Person newDetails, User userLoggedIn)
       {
           try
           {
              
               PERSON_AUDIT personAuditEntity = new PERSON_AUDIT();

               if (OldDetails.FirstName != null)
               {
                   personAuditEntity.First_Name = OldDetails.FirstName;
               }
               if (OldDetails.LastName != null)
               {
                   personAuditEntity.Last_Name = OldDetails.LastName;
               }
               if (OldDetails.OtherName != null)
               {
                   personAuditEntity.Other_Name = OldDetails.OtherName;
               }
               if (OldDetails.ContactAddress != null)
               {
                   personAuditEntity.Contact_Address = OldDetails.ContactAddress;
               }
               if (OldDetails.Email != null)
               {
                   personAuditEntity.Email = OldDetails.Email;
               }
               if (OldDetails.MobilePhone != null)
               {
                   personAuditEntity.Mobile_Phone = OldDetails.MobilePhone;
               }
               if (OldDetails.SignatureFileUrl != null)
               {
                   personAuditEntity.Signature_File_Url = OldDetails.SignatureFileUrl;
               }
               if (OldDetails.ImageFileUrl != null)
               {
                   personAuditEntity.Image_File_Url = OldDetails.ImageFileUrl;
               }
               if (OldDetails.DateOfBirth != null)
               {
                   personAuditEntity.Date_Of_Birth = OldDetails.DateOfBirth;
               }
               if (OldDetails.HomeTown != null)
               {
                   personAuditEntity.Home_Town = OldDetails.HomeTown;
               }
               if (OldDetails.HomeAddress != null)
               {
                   personAuditEntity.Home_Address = OldDetails.HomeAddress;
               }
               if (OldDetails.DateEntered != null)
               {
                   personAuditEntity.Date_Entered = OldDetails.DateEntered;
               }
               if (OldDetails.Initial != null)
               {
                   personAuditEntity.Initial = OldDetails.Initial;
               }
               if (OldDetails.Title != null)
               {
                   personAuditEntity.Title = OldDetails.Title;
               }

               if (OldDetails.Role != null && OldDetails.Role.Id > 0)
               {
                   personAuditEntity.Role_Id = OldDetails.Role.Id;
               }
               if (OldDetails.Nationality != null && OldDetails.Nationality.Id > 0)
               {
                   personAuditEntity.Nationality_Id = OldDetails.Nationality.Id;
               }
               if (OldDetails.State != null && !string.IsNullOrEmpty(OldDetails.State.Id))
               {
                   personAuditEntity.State_Id = OldDetails.State.Id;
               }
               if (OldDetails.Type != null && OldDetails.Type.Id > 0)
               {
                   personAuditEntity.Person_Type_Id = OldDetails.Type.Id;
               }
               if (OldDetails.Religion != null)
               {
                   personAuditEntity.Religion_Id = OldDetails.Religion.Id;
               }
               if (OldDetails.LocalGovernment != null)
               {
                   personAuditEntity.Local_Government_Id = OldDetails.LocalGovernment.Id;
               }
               if (OldDetails.Sex != null)
               {
                   personAuditEntity.Sex_Id = OldDetails.Sex.Id;
               }


              
               return true;
           }
           catch (Exception)
           {
               throw;
           }
       }



    }
}
