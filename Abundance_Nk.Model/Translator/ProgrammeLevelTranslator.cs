using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ProgrammeLevelTranslator : TranslatorBase<ProgrammeLevel, PROGRAMME_LEVEL>
    {
        private LevelTranslator levelTranslator;
        private ProgrammeTranslator programmeTranslator;

        public ProgrammeLevelTranslator()
        {
            levelTranslator = new LevelTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }

        public override ProgrammeLevel TranslateToModel(PROGRAMME_LEVEL entity)
        {
            try
            {
                ProgrammeLevel model = null;
                if (entity != null)
                {
                    model = new ProgrammeLevel();
                    model.Id = entity.Id;
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Active = entity.Active;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PROGRAMME_LEVEL TranslateToEntity(ProgrammeLevel model)
        {
            try
            {
                PROGRAMME_LEVEL entity = null;
                if (model != null)
                {
                    entity = new PROGRAMME_LEVEL();
                    entity.Id = model.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Active = model.Active;
                    entity.Programme_Id = model.Programme.Id;
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
