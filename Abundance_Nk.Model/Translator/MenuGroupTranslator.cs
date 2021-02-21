using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class MenuGroupTranslator : TranslatorBase<MenuGroup, MENU_GROUP>
    {
        public override MenuGroup TranslateToModel(MENU_GROUP entity)
        {
            try
            {
                MenuGroup model = null;
                if (entity != null)
                {
                    model = new MenuGroup();
                    model.Id = entity.Menu_Group_Id;
                    model.Name = entity.Name;
                    model.Activated = entity.Activated;
                }

                return model;
            }
            catch (Exception)
            { 
                throw;
            }
            
        }

        public override MENU_GROUP TranslateToEntity(MenuGroup model)
        {
            try
            {
                MENU_GROUP entity = null;
                if (model != null)
                {
                    entity = new MENU_GROUP();
                    entity.Menu_Group_Id = model.Id;
                    entity.Name = model.Name;
                    entity.Activated = model.Activated;
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
