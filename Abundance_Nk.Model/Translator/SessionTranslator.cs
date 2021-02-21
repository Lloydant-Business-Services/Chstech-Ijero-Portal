using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class SessionTranslator : TranslatorBase<Session, SESSION>
    {
        public override Session TranslateToModel(SESSION sessionEntity)
        {
            try
            {
                Session session = null;
                if (sessionEntity != null)
                {
                    session = new Session();
                    session.Id = sessionEntity.Session_Id;
                    session.Name = sessionEntity.Session_Name;
                    session.StartDate = sessionEntity.Start_Date;
                    session.EndDate = sessionEntity.End_date;
                    session.Activated = sessionEntity.Activated;
                    session.ActiveForResult = sessionEntity.Active_For_Result;
                    session.ActiveForAllocation = sessionEntity.Active_For_Allocation;
                    session.ActiveForApplication = sessionEntity.Active_For_Application;
                    session.ActiveForFees = sessionEntity.Active_For_Fees;
                    session.IsLateRegistration = sessionEntity.Is_Late_Registration;
                }

                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SESSION TranslateToEntity(Session session)
        {
            try
            {
                SESSION sessionEntity = null;
                if (session != null)
                {
                    sessionEntity = new SESSION();
                    sessionEntity.Session_Name = session.Name;
                    sessionEntity.Start_Date = session.StartDate;
                    sessionEntity.End_date = session.EndDate;
                    sessionEntity.Activated = session.Activated;
                    sessionEntity.Active_For_Result = session.ActiveForResult;
                    sessionEntity.Active_For_Allocation = session.ActiveForAllocation;
                    sessionEntity.Active_For_Application = session.ActiveForApplication;
                    sessionEntity.Active_For_Fees = session.ActiveForFees;
                    sessionEntity.Is_Late_Registration = session.IsLateRegistration;
                }

                return sessionEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}
