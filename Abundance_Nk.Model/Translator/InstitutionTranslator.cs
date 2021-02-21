using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class InstitutionTranslator : TranslatorBase<Institution, INSTITUTION>
    {
        private InstitutionTypeTranslator InstitutionTypeTranslator;

        public InstitutionTranslator()
        {
            InstitutionTypeTranslator = new InstitutionTypeTranslator();
        }

        public override Institution TranslateToModel(INSTITUTION entity)
        {
            try
            {
                Institution Institution = null;
                if (entity != null)
                {
                    Institution = new Institution();
                    Institution.Id = entity.Institution_Id;
                    Institution.Name = entity.Institution_Name;
                    Institution.Type = InstitutionTypeTranslator.Translate(entity.INSTITUTION_TYPE);
                }

                return Institution;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override INSTITUTION TranslateToEntity(Institution Institution)
        {
            try
            {
                INSTITUTION entity = null;
                if (Institution != null)
                {
                    entity = new INSTITUTION();
                    entity.Institution_Id = Institution.Id;
                    entity.Institution_Name = Institution.Name;
                    entity.Institution_Type_Id = Institution.Type.Id;
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
