using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class MenuTranslator : TranslatorBase<Menu, MENU>
    {
        private MenuGroupTranslator menuGroupTranslator;

        public MenuTranslator()
        {
            menuGroupTranslator = new MenuGroupTranslator();
        }
        public override Menu TranslateToModel(MENU entity)
        {
            try
            {
                Menu model = null;
                if (entity != null)
                {
                    model = new Menu();
                    model.Id = entity.Menu_Id;
                    model.Action = entity.Action;
                    model.Activated = entity.Activated;
                    model.Area = entity.Area;
                    model.Controller = entity.Controller;
                    model.DisplayName = entity.Display_Name;
                    model.MenuGroup = menuGroupTranslator.Translate(entity.MENU_GROUP);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MENU TranslateToEntity(Menu model)
        {
            try
            {
                MENU entity = null;
                if (model != null)
                {
                    entity = new MENU();
                    entity.Action = model.Action;
                    entity.Activated = model.Activated;
                    entity.Area = model.Area;
                    entity.Controller = model.Controller;
                    entity.Display_Name = model.DisplayName;
                    entity.Menu_Id = model.Id;
                    entity.Menu_Group_Id = model.MenuGroup.Id;
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
