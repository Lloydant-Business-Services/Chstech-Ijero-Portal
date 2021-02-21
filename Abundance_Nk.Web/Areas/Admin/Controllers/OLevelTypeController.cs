
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class OLevelTypeController : BasicSetupBaseController<OLevelType, O_LEVEL_TYPE>
    {
        public OLevelTypeController()
            : base(new OLevelTypeLogic())
        {
            ModelName = "O-Level Type";
            Selector = o => o.O_Level_Type_Id == Id;
        }

        protected override bool ModifyModel(OLevelType model)
        {
            try
            {
                OLevelTypeLogic modelLogic = new OLevelTypeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }




	}
}