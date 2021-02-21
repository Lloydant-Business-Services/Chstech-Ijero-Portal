using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentAuditTranslator : TranslatorBase<StudentAudit, STUDENT_AUDIT>
    {
        private StudentTranslator studentTranslator;
        private UserTranslator userTranslator;

        public StudentAuditTranslator()
        {
            studentTranslator = new StudentTranslator();
            userTranslator = new UserTranslator();
        }

        public override StudentAudit TranslateToModel(STUDENT_AUDIT entity)
        {
            try
            {
                StudentAudit model = null;
                if (entity != null)
                {
                    model = new StudentAudit();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.CurrentValues = entity.Current_Values;
                    model.Id = entity.Student_Audit_Id;
                    model.InitialValues = entity.Initial_Values;
                    model.Operation = entity.Operation;
                    model.Time = entity.Time;
                    model.User = userTranslator.Translate(entity.USER);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_AUDIT TranslateToEntity(StudentAudit model)
        {
            try
            {
                STUDENT_AUDIT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_AUDIT();
                    entity.Time = model.Time;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Current_Values = model.CurrentValues;
                    entity.Initial_Values = model.InitialValues;
                    entity.Operation = model.Operation;
                    entity.Student_Audit_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.User_Id = model.User.Id;
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
