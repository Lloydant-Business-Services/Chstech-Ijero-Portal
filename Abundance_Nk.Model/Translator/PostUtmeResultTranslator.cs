using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PostUtmeResultTranslator : TranslatorBase<PostUtmeResult, POST_UTME_RESULT>
    {
        public override PostUtmeResult TranslateToModel(POST_UTME_RESULT entity)
        {
            try
            {
                PostUtmeResult postumeresult = null;
                if (entity != null)
                {
                    postumeresult = new PostUtmeResult();
                    postumeresult.Id = entity.POST_UME_RESULT_ID;
                    postumeresult.Regno = entity.REGNO;
                    postumeresult.Examno = entity.EXAMNO;
                    postumeresult.Fullname = entity.FULLNAME;
                    postumeresult.Sex = entity.SEX;
                    postumeresult.JambScore = entity.JAMBSCORE;
                    postumeresult.State = entity.STATE;
                    postumeresult.LGA = entity.LGA;
                    postumeresult.Course = entity.COURSE;

                    if (entity.ENG.HasValue)
                    {
                        postumeresult.Eng = entity.ENG.Value;
                    }
                   
                    postumeresult.Sub2 = entity.SUB2;
                    if (entity.SCR2.HasValue)
                    {
                        postumeresult.Scr2 = entity.SCR2.Value;
                    }
                   
                    postumeresult.Sub3 = entity.SUB3;
                    if (entity.SCR3.HasValue)
                    {
                        postumeresult.Scr3 = entity.SCR3.Value;
                    }

                    postumeresult.Sub4 = entity.SUB4;
                    if (entity.SCR4.HasValue)
                    {
                        postumeresult.Scr4 = entity.SCR4.Value;
                    }

                    if (entity.TOTAL.HasValue)
                    {
                        postumeresult.Total = entity.TOTAL.Value;
                    }

                    if (entity.AVERAGE.HasValue)
                    {
                        postumeresult.Average = entity.AVERAGE.Value;
                    }
                }

                return postumeresult;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public override POST_UTME_RESULT TranslateToEntity(PostUtmeResult postumeresult)
        {
            try
            {
                POST_UTME_RESULT entity = null;
                if (postumeresult != null)
                {
                    entity = new POST_UTME_RESULT();
                    entity.POST_UME_RESULT_ID = postumeresult.Id;
                    entity.REGNO = postumeresult.Regno;
                    entity.EXAMNO = postumeresult.Examno;
                    entity.FULLNAME = postumeresult.Fullname;
                    entity.SEX = postumeresult.Sex;
                    entity.JAMBSCORE = postumeresult.JambScore;
                    entity.STATE = postumeresult.State;
                    entity.LGA = postumeresult.LGA;
                    entity.COURSE = postumeresult.Course;
                    entity.ENG = postumeresult.Eng ;
                    entity.SUB2 = postumeresult.Sub2 ;
                    entity.SCR2 = postumeresult.Scr2;
                    entity.SUB3 = postumeresult.Sub3;
                    entity.SCR3 = postumeresult.Scr3 ;
                    entity.SUB4 = postumeresult.Sub4 ;
                    entity.SCR4 = postumeresult.Scr4 ;
                    entity.TOTAL = postumeresult.Total;
                    entity.AVERAGE = postumeresult.Average;
                   

                }

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
