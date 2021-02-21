using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class SpilloverStudentTranslator : TranslatorBase<SpilloverStudent, SPILL_OVER_STUDENT>
    {
        private SemesterTranslator semesterTranslator;
        private SessionTranslator sessionTranslator;
        private StudentTranslator studentTranslator;
        private UserTranslator userTranslator;

        public SpilloverStudentTranslator()
        {
            sessionTranslator = new SessionTranslator();
            semesterTranslator = new SemesterTranslator();
            studentTranslator = new StudentTranslator();
            userTranslator = new UserTranslator();
        }
        public override SpilloverStudent TranslateToModel(SPILL_OVER_STUDENT entity)
        {
            try
            {
                SpilloverStudent model = null;
                if (entity != null)
                {
                    model = new SpilloverStudent();
                    model.Id = entity.Spillover_Student_Id;
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.UploadedBy = userTranslator.Translate(entity.USER);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SPILL_OVER_STUDENT TranslateToEntity(SpilloverStudent model)
        {
            try
            {
                SPILL_OVER_STUDENT entity = null;
                if (model != null)
                {
                    entity = new SPILL_OVER_STUDENT();
                    entity.Spillover_Student_Id = model.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Student_Id = model.Student.Id;
                    entity.Uploaded_By = model.UploadedBy.Id;
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
