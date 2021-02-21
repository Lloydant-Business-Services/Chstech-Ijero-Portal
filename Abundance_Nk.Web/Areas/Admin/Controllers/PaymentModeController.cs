
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
    public class PaymentModeController : BasicSetupBaseController<PaymentMode, PAYMENT_MODE>
    {
        public PaymentModeController()
            : base(new PaymentModeLogic())
        {
            ModelName = "Payment Mode";
            Selector = p => p.Payment_Mode_Id == Id;
        }

        protected override bool ModifyModel(PaymentMode model)
        {
            try
            {
                PaymentModeLogic modelLogic = new PaymentModeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }



	}

}