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
    public class InstitutionTypeController : BasicSetupBaseController<InstitutionType, INSTITUTION_TYPE>
    {
        public InstitutionTypeController()
            : base(new InstitutionTypeLogic())
        {
            ModelName = "School Type";
            Selector = i => i.Institution_Type_Id == Id;
        }

        protected override bool ModifyModel(InstitutionType model)
        {
            try
            {
                InstitutionTypeLogic modelLogic = new InstitutionTypeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }



        
    }
}
