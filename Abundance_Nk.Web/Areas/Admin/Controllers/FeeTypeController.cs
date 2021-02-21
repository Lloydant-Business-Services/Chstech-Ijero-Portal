
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class FeeTypeController : BasicSetupBaseController<FeeType, FEE_TYPE>
    {
        public FeeTypeController()
            : base(new FeeTypeLogic())
        {
            ModelName = "Fee Type";
            Selector = f => f.Fee_Type_Id == Id;
        }

        protected override bool ModifyModel(FeeType model)
        {
            try
            {
                FeeTypeLogic oLevelGradeLogic = new FeeTypeLogic();
                return oLevelGradeLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
