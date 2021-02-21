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
    public class PersonTypeLogic : BusinessBaseLogic<PersonType, PERSON_TYPE>
    {
        public PersonTypeLogic()
        {
            translator = new PersonTypeTranslator();
        }

        public bool Modify(PersonType personType)
        {
            try
            {
                Expression<Func<PERSON_TYPE, bool>> selector = p => p.Person_Type_Id == personType.Id;
                PERSON_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Person_Type_Name = personType.Name;
                entity.Person_Type_Description = personType.Description;

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
