using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class NextOfKinLogic : BusinessBaseLogic<NextOfKin, NEXT_OF_KIN>
    {
        public NextOfKinLogic()
        {
            translator = new NextOfKinTranslator();
        }


        public bool Modify(NextOfKin NextOfKin)
        {
            try
            {
                NEXT_OF_KIN entity = GetEntityBy(s => s.Person_Id == NextOfKin.Person.Id);
                if (entity != null)
                {
                    entity.Sponsor_Name = NextOfKin.Name;
                    entity.Sponsor_Mobile_Phone = NextOfKin.MobilePhone;
                    entity.Relationship_Id = NextOfKin.Relationship.Id;
                    entity.Sponsor_Contact_Address = NextOfKin.ContactAddress;
                    if (NextOfKin.ApplicationForm != null)
                    {
                        entity.Application_Form_Id = NextOfKin.ApplicationForm.Id;
                    }
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
