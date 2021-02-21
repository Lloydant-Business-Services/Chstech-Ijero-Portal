using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Abundance_Nk.Data;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class LevelLogic : BusinessBaseLogic<Level, LEVEL>
    {
        public LevelLogic()
        {
            translator = new LevelTranslator();
        }
        public List<Level> GetONDs()
        {
            try
            {
                System.Linq.Expressions.Expression<Func<LEVEL, bool>> selector = l => l.Level_Id <= 2;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Level> GetHNDs()
        {
            try
            {
                System.Linq.Expressions.Expression<Func<LEVEL, bool>> selector = l => l.Level_Id > 2;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Level> GetLevelsByProgramme(Programme programme)
        {
            try
            {
                List<Level> levels = new List<Level>();
                ProgrammeLevelLogic programmeLevelLogic = new ProgrammeLevelLogic();
                List<ProgrammeLevel> programmeLevels = programmeLevelLogic.GetModelsBy(p => p.Programme_Id == programme.Id);

                for (int i = 0; i < programmeLevels.Count; i++)
                {
                    levels.Add(programmeLevels[i].Level);
                }

                return levels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Level> GetBy(Programme programme)
        {
            try
            {
                repository = new Repository();
                List<Level> levels = (from d in repository.GetBy<VW_PROGRAMME_LEVEL>()
                                   where d.Programme_Id == programme.Id
                                   select new Level
                                   {
                                       Id = d.Level_Id,
                                       Name = d.Level_Name
                                   }
                                       ).ToList();

                return levels.OrderBy(l => l.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(Level level)
        {
            try
            {
                Expression<Func<LEVEL, bool>> selector = f => f.Level_Id == level.Id;
                LEVEL entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Level_Name = level.Name;
                entity.Level_Description = level.Description;

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



