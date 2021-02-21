using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{                                                                                
    public class InstitutionChoiceTranslator : TranslatorBase<InstitutionChoice, INSTITUTION_CHOICE>
    {
        public override InstitutionChoice TranslateToModel(INSTITUTION_CHOICE entity)
        {
            try
            {
                InstitutionChoice model = null;
                if (entity != null)
                {
                    model = new InstitutionChoice();
                    model.Id = entity.Institution_Choice_Id;
                    model.Name = entity.Institution_Choice_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override INSTITUTION_CHOICE TranslateToEntity(InstitutionChoice model)
        {
            try
            {
                INSTITUTION_CHOICE entity = null;
                if (model != null)
                {
                    entity = new INSTITUTION_CHOICE();
                    entity.Institution_Choice_Id = model.Id;
                    entity.Institution_Choice_Name = model.Name;
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
