using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentExtraYearSessionTranslator : TranslatorBase<StudentExtraYearSession, STUDENT_EXTRA_YEAR_SESSION>
    {
        PersonTranslator personTranslator;
        SessionTranslator sessionTranslator;
        public StudentExtraYearSessionTranslator()
        {
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override StudentExtraYearSession TranslateToModel(STUDENT_EXTRA_YEAR_SESSION entity)
        {
            try
            {
                StudentExtraYearSession model = null;
                if (entity != null)
                {
                    model = new StudentExtraYearSession();
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Id = entity.Student_Extra_Year_Session_Id;
                    model.Session = sessionTranslator.Translate(entity.SESSION1);
                    model.Sessions_Registered = entity.Sessions_Registered;
                    model.LastSessionRegistered = sessionTranslator.Translate(entity.SESSION2);
                    model.DeferementCommencedSession = sessionTranslator.Translate(entity.SESSION);
                }
                return model;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public override STUDENT_EXTRA_YEAR_SESSION TranslateToEntity(StudentExtraYearSession model)
        {
            STUDENT_EXTRA_YEAR_SESSION entity = null;
            try
            {
                if (model != null)
                {
                    entity = new STUDENT_EXTRA_YEAR_SESSION();
                    entity.Student_Extra_Year_Session_Id = model.Id;
                    if (model.DeferementCommencedSession != null)
                    {
                        entity.Differement_Commenced_Session = model.DeferementCommencedSession.Id;
                    }
                    entity.Last_Session_Registered = model.LastSessionRegistered.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Sessions_Registered = model.Sessions_Registered;
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
