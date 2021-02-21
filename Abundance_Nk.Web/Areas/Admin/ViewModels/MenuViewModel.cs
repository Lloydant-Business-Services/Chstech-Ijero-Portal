using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            RoleSelectList = Utility.PopulateRoleSelectListItem();
            MenuGroupSelectList = Utility.PopulateMenuGroupSelectListItem();
            MenuSelectList = Utility.PopulateMenuSelectListItem();
        }
        public Role Role { get; set; }
        public MenuGroup MenuGroup { get; set; }
        public Abundance_Nk.Model.Model.Menu Menu { get; set; }
        public MenuInRole MenuInRole { get; set; }
        public List<Abundance_Nk.Model.Model.Menu> MenuList { get; set; }
        public List<MenuInRole> MenuInRoleList { get; set; }
        public List<SelectListItem> RoleSelectList { get; set; }
        public List<SelectListItem> MenuGroupSelectList { get; set; }
        public List<SelectListItem> MenuSelectList { get; set; }
    }
}