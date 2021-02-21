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
    public class NationalityLogic : BusinessBaseLogic<Nationality, NATIONALITY>
    {
        public NationalityLogic()
        {
            translator = new NationalityTranslator();
        }

        public bool Modify(Nationality nationality)
        {
            try
            {
                Expression<Func<NATIONALITY, bool>> selector = n => n.Nationality_Id == nationality.Id;
                NATIONALITY entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Nationality_Name = nationality.Name;
               
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
