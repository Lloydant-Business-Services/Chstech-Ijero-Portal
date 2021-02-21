
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
    public class LocalGovernmentLogic : BusinessBaseLogic<LocalGovernment, LOCAL_GOVERNMENT>
    {
        public LocalGovernmentLogic()
        {
            translator = new LocalGovernmentTranslator();
        }

        public bool Modify(LocalGovernment lga)
        {
            try
            {
                Expression<Func<LOCAL_GOVERNMENT, bool>> selector = s => s.Local_Government_Id == lga.Id;
                LOCAL_GOVERNMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Local_Government_Name = lga.Name;
                entity.State_Id = lga.State.Id;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }




}
