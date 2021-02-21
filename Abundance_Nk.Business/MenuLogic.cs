using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class MenuLogic : BusinessBaseLogic<Menu, MENU>
    {
        public MenuLogic()
        {
            translator = new MenuTranslator();
        }

        public bool Modify(Menu menu)
        { 
            try
            {
                Expression<Func<MENU, bool>> selector = a => a.Menu_Id == menu.Id;
                MENU entity = GetEntityBy(selector);
                if (entity != null && entity.Menu_Id > 0)
                {
                    entity.Controller = menu.Controller;
                    entity.Activated = menu.Activated;
                    entity.Action = menu.Action;
                    entity.Area = menu.Area;
                    entity.Display_Name = menu.DisplayName;
                    if (menu.MenuGroup != null)
                    {
                        entity.Menu_Group_Id = menu.MenuGroup.Id; 
                    }
                    
                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
