using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class ModeOfStudyTranslator : TranslatorBase<ModeOfStudy, MODE_OF_STUDY>
    {
        public override ModeOfStudy TranslateToModel(MODE_OF_STUDY entity)
        {
            try
            {
                ModeOfStudy model = null;
                if (entity != null)
                {
                    model = new ModeOfStudy();
                    model.Id = entity.Mode_Of_Study_Id;
                    model.Name = entity.Mode_Of_Study_Name;
                    model.Description = entity.Mode_Of_Study_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override MODE_OF_STUDY TranslateToEntity(ModeOfStudy model)
        {
            try
            {
                MODE_OF_STUDY entity = null;
                if (model != null)
                {
                    entity = new MODE_OF_STUDY();
                    entity.Mode_Of_Study_Id = model.Id;
                    entity.Mode_Of_Study_Name = model.Name;
                    entity.Mode_Of_Study_Description = model.Description;
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
