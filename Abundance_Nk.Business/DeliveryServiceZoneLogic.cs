using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class DeliveryServiceZoneLogic : BusinessBaseLogic<DeliveryServiceZone, DELIVERY_SERVICE_ZONE>
    {
        public DeliveryServiceZoneLogic()
        {
            translator = new DeliveryServiceZoneTranslator();
        }
    }
}
