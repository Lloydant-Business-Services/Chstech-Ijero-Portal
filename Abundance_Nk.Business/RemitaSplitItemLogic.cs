using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class RemitaSplitItemLogic: BusinessBaseLogic<RemitaSplitItems, REMITA_SPLIT_DETAILS>
    {
        public RemitaSplitItemLogic()
        {
            translator = new RemitaSplitItemsTranslator();
        }


        public RemitaSplitItems GetBy(long Id)
        {
            try
            {
                Expression<Func<REMITA_SPLIT_DETAILS, bool>> selector = a => a.Id == Id;
                RemitaSplitItems remitaSettings = GetModelBy(selector);
                return remitaSettings;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
