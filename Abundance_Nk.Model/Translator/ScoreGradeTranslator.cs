using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class ScoreGradeTranslator : TranslatorBase<ScoreGrade, SCORE_GRADE>
    {
        public override ScoreGrade TranslateToModel(SCORE_GRADE scoreGradeEntity)
        {
            try
            {
                ScoreGrade scoreGrade = null;
                if (scoreGradeEntity != null)
                {
                    scoreGrade = new ScoreGrade();
                    scoreGrade.Id = scoreGradeEntity.Grade_Id;
                    scoreGrade.From = (int)scoreGradeEntity.Score_From;
                    scoreGrade.To = (int)scoreGradeEntity.Score_To;
                    scoreGrade.Grade = scoreGradeEntity.Grade;
                    scoreGrade.GradePoint = scoreGradeEntity.Grade_Point;
                    scoreGrade.Performance = scoreGradeEntity.Performance;
                    scoreGrade.Description = scoreGradeEntity.Grade_Description;
                }

                return scoreGrade;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SCORE_GRADE TranslateToEntity(ScoreGrade scoreGrade)
        {
            try
            {
                SCORE_GRADE scoreGradeEntity = null;
                if (scoreGrade != null)
                {
                    scoreGradeEntity = new SCORE_GRADE();
                    scoreGradeEntity.Grade_Id = scoreGrade.Id;
                    scoreGradeEntity.Score_From = scoreGrade.From;
                    scoreGradeEntity.Score_To = scoreGrade.To;
                    scoreGradeEntity.Grade = scoreGrade.Grade;
                    scoreGradeEntity.Grade_Point = scoreGrade.GradePoint;
                    scoreGradeEntity.Performance = scoreGrade.Performance;
                    scoreGradeEntity.Grade_Description = scoreGrade.Description;
                }

                return scoreGradeEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
