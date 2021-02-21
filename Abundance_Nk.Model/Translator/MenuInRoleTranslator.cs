using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class MenuInRoleTranslator : TranslatorBase<MenuInRole, MENU_IN_ROLE>
    {
        private MenuTranslator menuTranslator;
        private RoleTranslator roleTranslator;

        public MenuInRoleTranslator()
        {
            menuTranslator = new MenuTranslator(); 
            roleTranslator = new RoleTranslator();
        }
        public override MenuInRole TranslateToModel(MENU_IN_ROLE entity)
        {
            try
            {
                MenuInRole model = null;
                if (entity != null)
                {
                    model = new MenuInRole();
                    model.Id = entity.Menu_In_Role_Id;
                    model.Activated = entity.Activated;
                    model.Menu = menuTranslator.Translate(entity.MENU);
                    model.Role = roleTranslator.Translate(entity.ROLE);
                }

                return model;
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public override MENU_IN_ROLE TranslateToEntity(MenuInRole model)
        {
            try
            {
                MENU_IN_ROLE entity = null;
                if (model != null)
                {
                    entity = new MENU_IN_ROLE();
                    entity.Menu_In_Role_Id = model.Id;
                    entity.Activated = model.Activated;
                    entity.Menu_Id = model.Menu.Id;
                    entity.Role_Id = model.Role.Id;
                }

                return entity;
            }
            catch (Exception)
            {   
                throw;
            }
        }
    }
}
