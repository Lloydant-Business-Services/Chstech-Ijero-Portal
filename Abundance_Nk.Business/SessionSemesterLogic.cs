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
    public class SessionSemesterLogic : BusinessBaseLogic<SessionSemester, SESSION_SEMESTER>
    {
        private CurrentSessionSemesterLogic currentSessionSemesterLogic;

        public SessionSemesterLogic()
        {
            translator = new SessionSemesterTranslator();
            currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
        }

        public SessionSemester GetBy(int id)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Semester_Id == id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Semester_Id == sessionSemester.Id;
                SESSION_SEMESTER entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Session_Id = sessionSemester.Session.Id;
                entity.Semester_Id = sessionSemester.Semester.Id;
                //entity.Start_Date = sessionSemester.StartDate;
                //entity.End_Date = sessionSemester.EndDate;
                entity.Registration_Ended = sessionSemester.RegistrationEnded;
                entity.Active = sessionSemester.Active;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SessionSemester GetBySessionSemester(int semesterId, int sessionId)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Id == sessionId && s.Semester_Id == semesterId;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
