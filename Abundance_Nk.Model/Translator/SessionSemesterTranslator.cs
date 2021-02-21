using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class SessionSemesterTranslator : TranslatorBase<SessionSemester, SESSION_SEMESTER>
    {
        private SessionTranslator sessionTranslator;
        private SemesterTranslator semesterTranslator;

        public SessionSemesterTranslator()
        {
            sessionTranslator = new SessionTranslator();
            semesterTranslator = new SemesterTranslator();
        }

        public override SessionSemester TranslateToModel(SESSION_SEMESTER entity)
        {
            try
            {
                SessionSemester sessionSemester = null;
                if (entity != null)
                {
                    sessionSemester = new SessionSemester();
                    sessionSemester.Id = entity.Session_Semester_Id;
                    sessionSemester.Session = sessionTranslator.TranslateToModel(entity.SESSION);
                    sessionSemester.Semester = semesterTranslator.TranslateToModel(entity.SEMESTER);
                    sessionSemester.StartDate = entity.Start_Date;
                    sessionSemester.EndDate = entity.End_Date;
                    sessionSemester.SequenceNumber = entity.Sequence_Number;
                    sessionSemester.RegistrationEnded = entity.Registration_Ended;
                    sessionSemester.Active = entity.Active;

                    if (sessionSemester.Semester != null && sessionSemester.Session != null)
                    {
                        sessionSemester.Name = sessionSemester.Semester.Name + " of " + sessionSemester.Session.Name + " academic session";
                    }
                    
                }

                return sessionSemester;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SESSION_SEMESTER TranslateToEntity(SessionSemester sessionSemester)
        {
            try
            {
                SESSION_SEMESTER entity = null;
                if (sessionSemester != null)
                {
                    entity = new SESSION_SEMESTER();
                    entity.Session_Semester_Id = sessionSemester.Id;
                    entity.Session_Id = sessionSemester.Session.Id;
                    entity.Semester_Id = sessionSemester.Semester.Id;
                    entity.Start_Date = sessionSemester.StartDate;
                    entity.End_Date = sessionSemester.EndDate;
                    entity.Sequence_Number = sessionSemester.SequenceNumber;
                    entity.Registration_Ended = sessionSemester.RegistrationEnded;
                    entity.Active = sessionSemester.Active;
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
