
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
    public class StudentTypeController : BasicSetupBaseController<StudentType, STUDENT_TYPE>
    {
        public StudentTypeController()
            : base(new StudentTypeLogic())
        {
            ModelName = "Student Type";
            Selector = r => r.Student_Type_Id == Id;
        }

        protected override bool ModifyModel(StudentType model)
        {
            try
            {
                StudentTypeLogic modelLogic = new StudentTypeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

	}



}