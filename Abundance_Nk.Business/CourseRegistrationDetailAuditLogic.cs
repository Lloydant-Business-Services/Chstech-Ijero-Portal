using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseRegistrationDetailAuditLogic:BusinessBaseLogic<CourseRegistrationDetailAudit,STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT>
    {
        public CourseRegistrationDetailAuditLogic()
        {
            translator = new CourseRegistrationDetailAuditTranslator();
        }
    }
}
