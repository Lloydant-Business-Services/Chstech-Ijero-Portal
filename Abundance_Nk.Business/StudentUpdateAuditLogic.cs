using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class StudentUpdateAuditLogic : BusinessBaseLogic<StudentUpdateAudit, STUDENT_UPDATE_AUDIT>
    {
        public StudentUpdateAuditLogic()
        {
            translator = new StudentUpdateAuditTranslator();
        }
        public bool Modify(StudentUpdateAudit studentUpdateAudit)
        {
            try
            {
                Expression<Func<STUDENT_UPDATE_AUDIT, bool>> selector = a => a.Student_Update_Audit_Id == studentUpdateAudit.Id;
                STUDENT_UPDATE_AUDIT entity = GetEntityBy(selector);
                if (entity != null)
                {
                    if (studentUpdateAudit.Student != null)
                    {
                        entity.Student_Id = studentUpdateAudit.Student.Id;
                    }

                    int modifiedRecordCount = Save();

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {

                throw;
            }

            return false;

        }
    }
}
