using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Models
{
    public static class Menu
    {
        public static string GetUserRole(string userName)
        {
            string roleName = "";
            try
            {
                UserLogic userLogic = new UserLogic();
                User user = userLogic.GetModelsBy(u => u.User_Name == userName).FirstOrDefault();
                roleName = user.Role.Name;
            }
            catch (Exception)
            {   
                throw;
            }

            return roleName;
        }

        public static List<Model.Model.Menu> GetMenuList(string role)
        {
            List<Model.Model.Menu> menuList = new List<Model.Model.Menu>();
            try
            {
                MenuLogic menuLogic = new MenuLogic();
                MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();

                List<MenuInRole> menuInRoleList = menuInRoleLogic.GetModelsBy(m => m.ROLE.Role_Name == role && m.Activated && m.MENU.MENU_GROUP.Activated && m.MENU.Activated == true);
                for (int i = 0; i < menuInRoleList.Count; i++)
                {
                    MenuInRole thisMenuInRole = menuInRoleList[i];
                    menuList.Add(thisMenuInRole.Menu);
                }
            }
            catch (Exception)
            {   
                throw;
            }

            return menuList;
        } 
    }
}