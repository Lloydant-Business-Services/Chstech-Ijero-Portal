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
    public class HostelAmountLogic: BusinessBaseLogic<HostelAmount, HOSTEL_AMOUNT>
    {
        public HostelAmountLogic()
        {
            translator=new HostelAmountTranslator();
        }

    }
}
