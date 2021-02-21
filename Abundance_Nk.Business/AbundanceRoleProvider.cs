using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Abundance_Nk.Business
{
    public class AbundanceRoleProvider : RoleProvider
    {
        private RoleLogic roleLogic;
        private UserLogic userLogic;
        public AbundanceRoleProvider()
        {
           
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                
                userLogic = new UserLogic();
                string[] userRoles = new string[]{ userLogic.GetModelBy(u => u.User_Name == username).Role.Name.ToString()};
                userLogic.Dispose();
                return userRoles;
                
              
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                User user = new User();
                if (username != null && roleName != null)
                {
                    userLogic = new UserLogic();
                    user = userLogic.GetModelBy(u => u.User_Name == username);
                    if (user != null)
                    {
                        if (user.Role.Name == roleName)
                        {
                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

    }
}
