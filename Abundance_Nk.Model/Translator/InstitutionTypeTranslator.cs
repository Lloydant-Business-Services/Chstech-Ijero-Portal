using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class InstitutionTypeTranslator : TranslatorBase<InstitutionType, INSTITUTION_TYPE>
    {
        public override InstitutionType TranslateToModel(INSTITUTION_TYPE entity)
        {
            try
            {
                InstitutionType InstitutionType = null;
                if (entity != null)
                {
                    InstitutionType = new InstitutionType();
                    InstitutionType.Id = entity.Institution_Type_Id;
                    InstitutionType.Name = entity.Institution_Type_Name;
                    InstitutionType.Description = entity.Institution_Type_Description;
                }

                return InstitutionType;
            }
            catch (Exception)
            {
                throw;
            };
        }

        public override INSTITUTION_TYPE TranslateToEntity(InstitutionType InstitutionType)
        {
            try
            {
                INSTITUTION_TYPE entity = null;
                if (InstitutionType != null)
                {
                    entity = new INSTITUTION_TYPE();
                    entity.Institution_Type_Id = InstitutionType.Id;
                    entity.Institution_Type_Name = InstitutionType.Name;
                    entity.Institution_Type_Description = InstitutionType.Description;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }


}
