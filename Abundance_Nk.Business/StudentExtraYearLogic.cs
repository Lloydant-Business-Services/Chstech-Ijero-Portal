using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class StudentExtraYearLogic : BusinessBaseLogic<StudentExtraYearSession,STUDENT_EXTRA_YEAR_SESSION>
    {
        public StudentExtraYearLogic()
        {
            translator = new StudentExtraYearSessionTranslator();
        }

        public StudentExtraYearSession GetBy(long sid)
        {
            try
            {
                return GetModelsBy(s => s.Person_Id == sid).LastOrDefault();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public StudentExtraYearSession GetBy(long personId , int sessionId)
        {
            try
            {
                return GetModelsBy(s => s.Person_Id == personId && s.Session_Id == sessionId).LastOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    
        public bool Modify(StudentExtraYearSession extraYear)
        {
            try 
	        {
                Expression<Func<STUDENT_EXTRA_YEAR_SESSION, bool>> selector = p => p.Student_Extra_Year_Session_Id == extraYear.Id;
                STUDENT_EXTRA_YEAR_SESSION extraYearEntity = GetEntityBy(selector);

                if (extraYearEntity == null || extraYearEntity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (extraYear.DeferementCommencedSession != null && extraYear.DeferementCommencedSession.Id >0 )
                {
                    extraYearEntity.Differement_Commenced_Session = extraYear.DeferementCommencedSession.Id;
                }

                if (extraYear.LastSessionRegistered != null && extraYear.LastSessionRegistered.Id > 0)
                {
                    extraYearEntity.Last_Session_Registered = extraYear.LastSessionRegistered.Id;
                }

                if (extraYear.Session != null && extraYear.Session.Id > 0)
                {
                    extraYearEntity.Session_Id = extraYear.Session.Id;
                }

                if (extraYear.Sessions_Registered > 0)
                {
                    extraYearEntity.Sessions_Registered = extraYear.Sessions_Registered;
                }

                int modifiedCount = Save();
                if (modifiedCount > 0)
                {
                    return true;
                }
	        }
	        catch (Exception ex)
	        {
		
		        throw;
	        }
            return false;
        }
    
    }
}
