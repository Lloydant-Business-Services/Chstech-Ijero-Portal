using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class EducationalQualificationTranslator : TranslatorBase<EducationalQualification, EDUCATIONAL_QUALIFICATION>
    {
        public override EducationalQualification TranslateToModel(EDUCATIONAL_QUALIFICATION entity)
        {
            try
            {
                EducationalQualification model = null;
                if (entity != null)
                {
                    model = new EducationalQualification();
                    model.Id = entity.Educational_Qualification_Id;
                    model.ShortName = entity.Educational_Qualification_Abbreviation;
                    model.Name = entity.Educational_Qualification_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override EDUCATIONAL_QUALIFICATION TranslateToEntity(EducationalQualification model)
        {
            try
            {
                EDUCATIONAL_QUALIFICATION entity = null;
                if (model != null)
                {
                    entity = new EDUCATIONAL_QUALIFICATION();
                    entity.Educational_Qualification_Id = model.Id;
                    entity.Educational_Qualification_Abbreviation = model.ShortName;
                    entity.Educational_Qualification_Name = model.Name;
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
