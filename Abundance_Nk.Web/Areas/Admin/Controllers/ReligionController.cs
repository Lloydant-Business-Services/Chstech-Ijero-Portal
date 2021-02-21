
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
    public class ReligionController : BasicSetupBaseController<Religion, RELIGION>
    {
        public ReligionController()
            : base(new ReligionLogic())
        {
            ModelName = "Religion";
            Selector = r => r.Religion_Id == Id;
        }
        
        protected override bool ModifyModel(Religion model)
        {
            try
            {
                ReligionLogic ReligionLogic = new ReligionLogic();
                return ReligionLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }



	}




}