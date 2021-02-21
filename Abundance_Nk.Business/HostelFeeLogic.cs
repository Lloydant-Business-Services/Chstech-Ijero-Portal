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
    public class HostelFeeLogic : BusinessBaseLogic<HostelFee, HOSTEL_FEE>
    {
        public HostelFeeLogic()
        {
            translator = new HostelFeeTranslator();
        }
    }
}
