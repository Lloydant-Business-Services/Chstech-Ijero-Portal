
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
    public class InstitutionLogic : BusinessBaseLogic<Institution, INSTITUTION>
    {
        public InstitutionLogic()
        {
            translator = new InstitutionTranslator();
        }

        public bool Modify(Institution institution)
        {
            try
            {
                Expression<Func<INSTITUTION, bool>> selector = i => i.Institution_Id == institution.Id;
                INSTITUTION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Institution_Name = institution.Name;
                entity.Institution_Type_Id = institution.Type.Id;

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
