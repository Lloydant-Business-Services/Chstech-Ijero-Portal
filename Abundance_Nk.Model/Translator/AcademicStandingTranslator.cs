using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AcademicStandingTranslator : TranslatorBase<AcademicStanding, ACADEMIC_STANDING>
    {
        public override AcademicStanding TranslateToModel(ACADEMIC_STANDING entity)
        {
            try
            {
                AcademicStanding model = null;
                if (entity != null)
                {
                    model = new AcademicStanding();
                    model.Id = entity.Academic_Standing_Id;
                    model.Name = entity.Academic_Standing_Name;
                    model.Abbreviation = entity.Academic_Standing_Abbreviation;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ACADEMIC_STANDING TranslateToEntity(AcademicStanding model)
        {
            try
            {
                ACADEMIC_STANDING entity = null;
                if (model != null)
                {
                    entity = new ACADEMIC_STANDING();
                    entity.Academic_Standing_Id = model.Id;
                    entity.Academic_Standing_Name = model.Name;
                    entity.Academic_Standing_Abbreviation = model.Abbreviation;
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
