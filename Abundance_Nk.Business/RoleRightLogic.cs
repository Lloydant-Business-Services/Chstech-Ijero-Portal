using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class RoleRightLogic : BusinessBaseLogic<RoleRight, ROLE_RIGHT>
    {
        public RoleRightLogic()
        {
            base.translator = new RoleRightTranslator();
        }

        public List<RoleRight> Create(Role role, List<Right> rights)
        {
            try
            {
                List<RoleRight> roleRights = null;
                if (role != null && rights != null)
                {
                    roleRights = new List<RoleRight>();
                    foreach (Right right in rights)
                    {
                        if (right.IsInRole)
                        {
                            RoleRight roleRight = new RoleRight();
                            roleRight.Role = role;
                            roleRight.Right = right;

                            roleRights.Add(roleRight);
                        }
                    }
                }

                return roleRights;
            }
            catch (Exception)
            {
                throw;
            }

        }



    }
}
