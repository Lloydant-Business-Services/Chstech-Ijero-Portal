using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentDefermentLogic : BusinessBaseLogic<StudentDeferementLog, STUDENT_DEFERMENT_LOG>
    {
        public StudentDefermentLogic()
        {
            translator = new StudentDefermentLogTranslator();
        }

        public override StudentDeferementLog Create(StudentDeferementLog model)
        {
            StudentDeferementLog studentDeferementLog = GetModelsBy(a => a.Person_Id == model.Student.Id && a.Semester_Id == model.Semester.Id && a.Session_Id == model.Session.Id).LastOrDefault();
            if (studentDeferementLog == null)
            {
                return base.Create(model);
            }

            return model;
        }

        public bool isStudentDefered(Result result)
        {
            try
            {
                STUDENT_DEFERMENT_LOG studentDeferementLog = GetEntityBy(a => a.Person_Id == result.StudentId && a.Session_Id == result.SessionId && a.Rusticated == false);
                if (studentDeferementLog != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }

        public bool isStudentRusticated(Result result, int SemesterId)
        {
            try
            {
                STUDENT_DEFERMENT_LOG studentDeferementLog = GetEntityBy(a => a.Person_Id == result.StudentId && a.Session_Id == result.SessionId && a.Semester_Id == SemesterId && a.Rusticated == true);
                if (studentDeferementLog != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }


    }
}