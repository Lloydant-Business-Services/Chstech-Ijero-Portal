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
    public class SessionLogic : BusinessBaseLogic<Session, SESSION>
    {
        public SessionLogic()
        {
            translator = new SessionTranslator();
        }

        public bool Modify(Session session)
        {
            try
            {
                Expression<Func<SESSION, bool>> selector = s => s.Session_Id == session.Id;
                SESSION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Session_Name = session.Name;
                entity.Start_Date = session.StartDate;
                entity.End_date = session.EndDate;
                entity.Activated = session.Activated;
                entity.Active_For_Result = session.ActiveForResult;
                entity.Active_For_Allocation = session.ActiveForAllocation;
                entity.Active_For_Application = session.ActiveForApplication;
                entity.Active_For_Fees = session.ActiveForFees;
                entity.Is_Late_Registration = session.IsLateRegistration;

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
        
        public List<Session> GetActiveSessions()
        {
            List<Session> sessions = new List<Session>();
            try
            {
              return  GetModelsBy(a => a.Activated == true).OrderByDescending(k => k.Name).ToList();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public List<Session> GetApplicationSession()
        {
            try
            {
                return GetModelsBy(a => a.Active_For_Application == true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

