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
    public class StudentSponsorLogic : BusinessBaseLogic<StudentSponsor, STUDENT_SPONSOR>
    {
        public StudentSponsorLogic()
        {
            translator = new StudentSponsorTranslator();
        }

        public bool Modify(StudentSponsor sponsor)
        {
            try
            {
                 STUDENT_SPONSOR entity = GetEntityBy(s => s.Person_Id == sponsor.Student.Id);
                 if (entity != null)
                 {
                     entity.Sponsor_Name = sponsor.Name;
                     entity.Sponsor_Mobile_Phone = sponsor.MobilePhone;
                     entity.Email = sponsor.Email;
                     entity.Relationship_Id = sponsor.Relationship.Id;
                     entity.Sponsor_Contact_Address = sponsor.ContactAddress;
                     int modified = Save();
                     return true;
                 }
                 return false;  
            }
            catch (Exception ex)
            {
                
                throw;
            }
          
        }
    
    
    }




}
