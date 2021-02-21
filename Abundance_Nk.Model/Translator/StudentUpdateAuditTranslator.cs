using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class StudentUpdateAuditTranslator:TranslatorBase<StudentUpdateAudit, STUDENT_UPDATE_AUDIT>
    {
         private UserTranslator userTranslator;
        private StudentTranslator studentTranslator;

        public StudentUpdateAuditTranslator()
        {
            userTranslator = new UserTranslator();
            studentTranslator = new StudentTranslator();
        }

        public override StudentUpdateAudit TranslateToModel(STUDENT_UPDATE_AUDIT entity)
        {
            try
            {
                StudentUpdateAudit model = null;
                if (entity != null)
                {
                    model = new StudentUpdateAudit();
                    model.Id = entity.Student_Update_Audit_Id;
                    model.User = userTranslator.TranslateToModel(entity.USER);
                    model.Student = studentTranslator.TranslateToModel(entity.STUDENT);
                    model.Date = entity.Date;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_UPDATE_AUDIT TranslateToEntity(StudentUpdateAudit model)
        {
            try
            {
                STUDENT_UPDATE_AUDIT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_UPDATE_AUDIT();
                    entity.Student_Update_Audit_Id = model.Id;
                    entity.Student_Id = model.Student.Id;
                    entity.User_Id = model.User.Id;
                    entity.Date = model.Date;
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
