using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class FacultyOfficerLogic : BusinessBaseLogic<FacultyOfficer, FACULTY_OFFICER>
    {
        public FacultyOfficerLogic()
        {
            translator = new FacultyOfficerTranslator();
        }

        public FacultyOfficer GetBy(User user)
        {
            try
            {
                Expression<Func<FACULTY_OFFICER, bool>> selector = fo => fo.User_Id == user.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }



}
