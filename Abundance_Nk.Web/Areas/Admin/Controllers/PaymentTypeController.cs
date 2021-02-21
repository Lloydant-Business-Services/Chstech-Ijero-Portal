
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
    public class PaymentTypeController : BasicSetupBaseController<PaymentType, PAYMENT_TYPE>
    {
        public PaymentTypeController()
            : base(new PaymentTypeLogic())
        {
            ModelName = "Person Type";
            Selector = p => p.Payment_Type_Id == Id;
        }

        protected override bool ModifyModel(PaymentType model)
        {
            try
            {
                PaymentTypeLogic modelLogic = new PaymentTypeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }



	}
}