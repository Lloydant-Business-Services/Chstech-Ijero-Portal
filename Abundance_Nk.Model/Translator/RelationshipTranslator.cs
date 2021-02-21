using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class RelationshipTranslator : TranslatorBase<Relationship, RELATIONSHIP>
    {
        public override Relationship TranslateToModel(RELATIONSHIP relationshipEntity)
        {
            try
            {
                Relationship relationship = null;
                if (relationshipEntity != null)
                {
                    relationship = new Relationship();
                    relationship.Id = relationshipEntity.Relationship_Id;
                    relationship.Name = relationshipEntity.Relationship_Name;
                }

                return relationship;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override RELATIONSHIP TranslateToEntity(Relationship relationship)
        {
            try
            {
                RELATIONSHIP relationshipEntity = null;
                if (relationship != null)
                {
                    relationshipEntity = new RELATIONSHIP();
                    relationshipEntity.Relationship_Id = relationship.Id;
                    relationshipEntity.Relationship_Name = relationship.Name;
                }

                return relationshipEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
