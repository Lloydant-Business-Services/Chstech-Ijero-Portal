
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class NationalityController : BasicSetupBaseController<Nationality, NATIONALITY>
    {
        public NationalityController()
            : base(new NationalityLogic())
        {
            ModelName = "Nationality";
            Selector = n => n.Nationality_Id == Id;
        }

        protected override bool ModifyModel(Nationality model)
        {
            try
            {
                NationalityLogic modelLogic = new NationalityLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
	}
}