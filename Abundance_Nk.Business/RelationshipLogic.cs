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
    public class RelationshipLogic : BusinessBaseLogic<Relationship, RELATIONSHIP>
    {
        public RelationshipLogic()
        {
            base.translator = new RelationshipTranslator();
        }

        public bool Modify(Relationship relationship)
        {
            try
            {
                Expression<Func<RELATIONSHIP, bool>> selector = p => p.Relationship_Id == relationship.Id;
                RELATIONSHIP entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Relationship_Name = relationship.Name;

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


