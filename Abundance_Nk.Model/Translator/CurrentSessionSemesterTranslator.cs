using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CurrentSessionSemesterTranslator : TranslatorBase<CurrentSessionSemester, CURRENT_SESSION_SEMESTER>
    {
        private SessionSemesterTranslator sessionTermTranslator = new SessionSemesterTranslator();

        public override CurrentSessionSemester TranslateToModel(CURRENT_SESSION_SEMESTER entity)
        {
            try
            {
                CurrentSessionSemester currentSessionTerm = null;
                if (entity != null)
                {
                    currentSessionTerm = new CurrentSessionSemester();
                    currentSessionTerm.SessionSemester = sessionTermTranslator.TranslateToModel(entity.SESSION_SEMESTER);
                }

                return currentSessionTerm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override CURRENT_SESSION_SEMESTER TranslateToEntity(CurrentSessionSemester currentSessionTerm)
        {
            try
            {
                CURRENT_SESSION_SEMESTER entity = null;
                if (currentSessionTerm != null)
                {
                    entity = new CURRENT_SESSION_SEMESTER();
                    entity.Session_Semester_Id = currentSessionTerm.SessionSemester.Id;
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
