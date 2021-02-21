using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentDefermentLogTranslator: TranslatorBase<StudentDeferementLog,STUDENT_DEFERMENT_LOG>
    {
        private SessionTranslator sessionTranslator;
        private StudentTranslator studentTranslator;
        private SemesterTranslator semesterTranslator;

        public StudentDefermentLogTranslator()
        {
            sessionTranslator = new SessionTranslator();
            studentTranslator = new StudentTranslator();
            semesterTranslator = new SemesterTranslator();
        }
       
        public override STUDENT_DEFERMENT_LOG TranslateToEntity(StudentDeferementLog model)
        {
            try
            {
                STUDENT_DEFERMENT_LOG entity = null;
                if (model != null)
                {
                    entity = new STUDENT_DEFERMENT_LOG();
                    entity.Student_Deferment_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Rusticated = model.Rusticated;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override StudentDeferementLog TranslateToModel(STUDENT_DEFERMENT_LOG entity)
        {
           try
            {
                StudentDeferementLog model = null;
                if (entity != null)
                {
                    model = new StudentDeferementLog();
                    model.Id = entity.Student_Deferment_Id;
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Rusticated = entity.Rusticated;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
    
    }
}
