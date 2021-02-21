using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PutmeResultTranslator : TranslatorBase<PutmeResult, PUTME_RESULT>
    {
       private PersonTranslator personTranslator;
        private SessionTranslator sessionTranslator;
        
       public PutmeResultTranslator()
       {
           personTranslator = new PersonTranslator();
           sessionTranslator = new SessionTranslator();
       }

       public override PutmeResult TranslateToModel(PUTME_RESULT entity)
       {
           try
           {
               PutmeResult model = null;
               if (entity != null)
               {
                   model = new PutmeResult();
                   model.Id = entity.ID;
                   model.Jambscore = entity.JAMBSCORE;
                   model.Course = entity.COURSE;
                   model.Programme = entity.PROGRAMME;
                   model.ExamNo = entity.EXAMNO;
                   model.RegNo = entity.REGNO;
                   model.RawScore =  entity.RAW_SCORE;
                   model.Total = entity.TOTAL;
                   model.FullName = entity.FULLNAME;
                   model.Session = sessionTranslator.Translate(entity.SESSION);
               }

               return model;
           }
           catch (Exception)
           {
               throw;
           }
       }

       public override PUTME_RESULT TranslateToEntity(PutmeResult model)
       {
           try
           {
               PUTME_RESULT entity = null;
               if (model != null)
               {
                   entity = new PUTME_RESULT();
                   entity.ID = model.Id;
                   entity.JAMBSCORE = model.Jambscore;
                   entity.RAW_SCORE = model.RawScore;
                   entity.REGNO = model.RegNo;
                   entity.TOTAL = model.Total;
                   entity.COURSE = model.Course;
                   entity.PROGRAMME = model.Programme;
                   entity.EXAMNO = model.ExamNo;
                   entity.FULLNAME = model.FullName;
                   entity.Session_Id = model.Session.Id;
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
