using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class ScoreGradeLogic : BusinessBaseLogic<ScoreGrade, SCORE_GRADE>
    {
        public ScoreGradeLogic()
        {
            base.translator = new ScoreGradeTranslator();
        }

        public string GetScoreGradingKey()
        {
            try
            {
                string concatenatedGrades = null;
                List<ScoreGrade> grades = GetAll().OrderBy(g => g.Id).ToList();
                if (grades != null && grades.Count > 0)
                {
                    concatenatedGrades += "SCORE GRADING KEY:- ";
                    //for (int i = 0; i < grades.Count; i++)
                    //{
                    //    concatenatedGrades += "[" + grades[i].From + "-" + grades[i].To + ":" + grades[i].Grade + "/" + grades[i].GradePoint + "]";
                    //}

                    foreach (ScoreGrade scoreGrade in grades)
                    {
                        concatenatedGrades += "[" + scoreGrade.From + "-" + scoreGrade.To + ":" + scoreGrade.Grade.Trim() + "/" + scoreGrade.GradePoint + "]";
                    }

                    concatenatedGrades += "[X:F/0.0-Absent][*-Failed Course][@-Audited Course].";
                }

                return concatenatedGrades;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Get()
        {
            try
            {
                string concatenatedGrades = null;
                List<ScoreGrade> grades = GetAll().OrderBy(g => g.Id).ToList();
                if (grades != null && grades.Count > 0)
                {
                    concatenatedGrades += "KEY TO GRADE: ";
                    for (int i = 0; i < grades.Count; i++)
                    {
                        concatenatedGrades += grades[i].Grade + " = " + grades[i].Performance + " (" + grades[i].Description + ")";
                        if (i != grades.Count - 1)
                        {
                            concatenatedGrades += ", ";
                        }
                    }
                }

                return concatenatedGrades;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(ScoreGrade scoreGrade)
        {
            try
            {
                Expression<Func<SCORE_GRADE, bool>> selector = p => p.Grade_Id == scoreGrade.Id;
                SCORE_GRADE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Performance = scoreGrade.Performance;
                entity.Grade_Point = scoreGrade.GradePoint;
                entity.Grade = scoreGrade.Grade;
                entity.Grade_Description = scoreGrade.Description;
                entity.Score_From = scoreGrade.From;
                entity.Score_To = scoreGrade.To;

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
