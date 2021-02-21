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
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class FacultyController : BasicSetupBaseController<Faculty, FACULTY>
    {
        public FacultyController()
            : base(new FacultyLogic())
        {
            ModelName = "Faculty";
            Selector = f => f.Faculty_Id == Id;
        }

        protected override bool ModifyModel(Faculty model)
        {
            try
            {
                FacultyLogic facultyLogic = new FacultyLogic();
                return facultyLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}
