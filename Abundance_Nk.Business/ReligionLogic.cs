using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class ReligionLogic : BusinessBaseLogic<Religion, RELIGION>
    {
        public ReligionLogic()
        {
            base.translator = new ReligionTranslator();
        }

        public bool Modify(Religion religion)
        {
            try
            {
                Expression<Func<RELIGION, bool>> selector = p => p.Religion_Id == religion.Id;
                RELIGION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Religion_Name = religion.Name;

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
