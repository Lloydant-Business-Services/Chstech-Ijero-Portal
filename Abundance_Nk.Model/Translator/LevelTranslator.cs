using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class LevelTranslator : TranslatorBase<Level, LEVEL>
    {
        public override Level TranslateToModel(LEVEL levelEntity)
        {
            try
            {
                Level level = null;
                if (levelEntity != null)
                {
                    level = new Level();
                    level.Id = levelEntity.Level_Id;
                    level.Name = levelEntity.Level_Name;
                    level.Description = levelEntity.Level_Description;
                }

                return level;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override LEVEL TranslateToEntity(Level level)
        {
            try
            {
                LEVEL levelEntity = null;
                if (level != null)
                {
                    levelEntity = new LEVEL();
                    levelEntity.Level_Id = level.Id;
                    levelEntity.Level_Name = level.Name;
                    levelEntity.Level_Description = level.Description;
                }

                return levelEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
