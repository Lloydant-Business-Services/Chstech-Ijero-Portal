using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ChangeOfCourseLogic : BusinessBaseLogic<ChangeOfCourse, CHANGE_OF_COURSE>
    {
        public ChangeOfCourseLogic()
        {
            translator = new ChangeOfCourseTranslator();
        }
        public bool Modify(ChangeOfCourse model)
        {
            try
            {
                Expression<Func<CHANGE_OF_COURSE, bool>> selector = p => p.Change_Of_Course_Id == model.Id;
                CHANGE_OF_COURSE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Jamb_Registration_Number = model.JambRegistrationNumber;

                if (model.ApplicationForm != null)
                {
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                }
                if (model.Session != null)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.OldPerson != null)
                {
                    entity.Old_Person_Id = model.OldPerson.Id;
                }
                if (model.NewPerson != null)
                {
                    entity.New_Person_Id = model.NewPerson.Id;
                }

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
    }
}
