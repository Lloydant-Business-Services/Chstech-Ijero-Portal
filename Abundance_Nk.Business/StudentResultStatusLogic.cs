using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentResultStatusLogic:BusinessBaseLogic<StudentResultStatus,STUDENT_RESULT_STATUS>
    {
        public StudentResultStatusLogic()
        {
            translator = new StudentResultStatusTranslator();
        }

        public bool Modify(StudentResultStatus studentResultStatus)
        {
            try
            {
                Expression<Func<STUDENT_RESULT_STATUS, bool>> selector = a => a.Id == studentResultStatus.Id;
                STUDENT_RESULT_STATUS entity = GetEntityBy(selector);
                if (entity != null && entity.Id > 0)
                {
                    entity.Activated = studentResultStatus.Activated;
                    if (studentResultStatus.Department != null)
                    {
                        entity.Department_Id = studentResultStatus.Department.Id;
                    }
                    if (studentResultStatus.Level != null)
                    {
                        entity.Level_Id = studentResultStatus.Level.Id;
                    }
                    if (studentResultStatus.Programme != null)
                    {
                        entity.Programme_Id = studentResultStatus.Programme.Id;
                    }

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
