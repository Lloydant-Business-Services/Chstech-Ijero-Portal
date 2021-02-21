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
    public class StaffLogic : BusinessBaseLogic<Staff, STAFF>
    {
        public StaffLogic()
        {
            translator = new StaffTranslator();
        }

        public bool IsUserProfileCaptured(Person person)
        {
            try
            {
                Staff staff = GetModelBy(a => a.PERSON.Person_Id == person.Id);
                if (staff != null && staff.Id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public bool IsUserProfileCaptured(User user)
        {
            try
            {
                Staff staff = GetModelBy(a => a.User_Id == user.Id);
                if (staff != null && staff.Id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
         public bool IsUserProfileCaptured(string username)
        {
            try
            {
                Staff staff = GetModelBy(a => a.USER.User_Name == username);
                if (staff != null && staff.Id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public Staff GetBy(User user)
        {
            try
            {
                return GetModelBy(a => a.User_Id == user.Id);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public Staff GetBy(string username)
        {
            try
            {
                return GetModelBy(a => a.USER.User_Name == username);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public bool Modify(Staff staff)
        {
            try
            {
                Expression<Func<STAFF, bool>> selector = p => p.Staff_Id == staff.Id;
                STAFF staffEntity = GetEntityBy(selector);

                if (staffEntity == null || staffEntity.Staff_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

               staffEntity.Marital_Status_Id = staff.MaritalStatus.Id;
                staffEntity.Profile_Description = staff.ProfileDescription;
               int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                    
                throw ex;
            }
        }
    
    }
}
