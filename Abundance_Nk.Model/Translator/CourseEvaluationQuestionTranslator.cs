using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseEvaluationQuestionTranslator: TranslatorBase<CourseEvaluationQuestion, COURSE_EVALUATION_QUESTION>
    {
        
        public override CourseEvaluationQuestion TranslateToModel(COURSE_EVALUATION_QUESTION entity)
        {
            try
            {
                CourseEvaluationQuestion model = null;
                if (entity != null)
                {
                    model = new CourseEvaluationQuestion();
                    model.Id = entity.Id;
                    model.Question = entity.Question;
                    model.Score = entity.Score;
                    model.Section = entity.Section;
                    model.Activated = entity.Activated;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_EVALUATION_QUESTION TranslateToEntity(CourseEvaluationQuestion model)
        {
            try
            {
                COURSE_EVALUATION_QUESTION entity = null;
                if (model != null)
                {
                    entity = new COURSE_EVALUATION_QUESTION();
                    entity.Id = model.Id;
                    entity.Question = model.Question;
                    entity.Score = model.Score;
                    entity.Section  = model.Section;
                    entity.Activated = model.Activated;
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
