using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class RoleLogic : BusinessBaseLogic<Role, ROLE>
    {
        private RightLogic rightLogic;
        private RoleRightLogic roleRightLogic;

        public RoleLogic()
        {
            base.translator = new RoleTranslator();
            rightLogic = new RightLogic();
            roleRightLogic = new RoleRightLogic();
        }

        public override List<Role> GetAll()
        {
            try
            {
                List<Role> roles = base.GetAll();
                return SetPersonRightView(roles);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Role> GetAll(Person user)
        {
            try
            {
                List<Role> roles = new List<Role>();
                if (user != null)
                {
                    if (user.Role.Id != 2)
                    {
                        Expression<Func<ROLE, bool>> selector = r => r.Role_Id != 2 && r.Role_Id != user.Role.Id;
                        roles = base.GetModelsBy(selector);
                    }
                    else
                    {
                        roles = base.GetAll();
                    }
                }

                return SetPersonRightView(roles);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Role Get(Person user)
        {
            try
            {
                Role role = null;
                if (user != null)
                {
                    Expression<Func<ROLE, bool>> selector = r => r.Role_Id == user.Role.Id;
                    role = base.GetModelBy(selector);
                }

                return role;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(Role role)
        {
            try
            {
                Expression<Func<ROLE, bool>> selector = r => r.Role_Id == role.Id;
                ROLE roleEntity = GetEntityBy(selector);
                roleEntity.Role_Name = role.Name;
                roleEntity.Role_Description = role.Description;

                int rowsAffected = repository.Save();
                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception(NoItemModified);
                }
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(ArgumentNullException);
            }
            //catch (UpdateException)
            //{
            //    throw new UpdateException(UpdateException);
            //}
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(Role role)
        {
            try
            {
                Func<ROLE, bool> selector = r => r.Role_Id == role.Id;
                bool suceeded = base.Delete(selector);

                base.repository.Save();
                return suceeded;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Role SetPersonRightView(Role role)
        {
            try
            {
                List<Right> rightsInRole = role.Rights;
                List<Right> rights = rightLogic.GetAll(); //get all rights

                foreach (Right right in rights)
                {
                    foreach (Right rightInRole in rightsInRole)
                    {
                        if (rightInRole.Id == right.Id)
                        {
                            right.IsInRole = true;
                        }
                    }
                }

                role.UserRight = new PersonRight();
                role.UserRight.View = rights;

                return role;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<Role> SetPersonRightView(List<Role> roles)
        {
            try
            {
                List<Role> newRoles = new List<Role>();
                if (roles != null)
                {
                    foreach (Role role in roles)
                    {
                        Role newRole = SetPersonRightView(role);
                        newRoles.Add(newRole);
                    }
                }

                return newRoles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AssignRightToRole(Role role)
        {
            try
            {
                bool isSuccessful = false;
                if (role != null)
                {
                    Func<ROLE_RIGHT, bool> selector = rr => rr.Role_Id == role.Id;

                    if (roleRightLogic.Delete(selector))
                    {
                        List<RoleRight> roleRights = new List<RoleRight>();
                        roleRights = roleRightLogic.Create(role, role.UserRight.View);
                        if (roleRights != null)
                        {
                            isSuccessful = roleRightLogic.Add(roleRights) > 0 ? true : false;
                        }
                    }
                }

                return isSuccessful;
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}
