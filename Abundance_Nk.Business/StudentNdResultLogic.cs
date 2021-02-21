using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentNdResultLogic : BusinessBaseLogic<StudentNdResult, STUDENT_ND_RESULT>
    {
        public StudentNdResultLogic()
        {
            translator = new StudentNdResultTranslator();
        }

        public bool Modify(StudentNdResult studentNDResult)
        {
            try
            {
                STUDENT_ND_RESULT entity = GetEntityBy(s => s.Person_Id == studentNDResult.Student.Id);

                entity.Date_Awarded = studentNDResult.DateAwarded;

                if (studentNDResult.Student != null)
                {
                    entity.Person_Id = studentNDResult.Student.Id;
                }

                int modified = Save();
                return true;
            }
            catch (Exception ex)
            {
                throw;
                return false;
            }

        }
        public bool Modify(StudentNdResult studentNDResult, long legitimatePersonId)
        {
            try
            {
                STUDENT_ND_RESULT entity = GetEntityBy(s => s.Person_Id == studentNDResult.Student.Id);
           
                entity.STUDENT.Person_Id = legitimatePersonId;
                entity.Date_Awarded = studentNDResult.DateAwarded;

                int modified = Save();
                return true;
            }
            catch (Exception ex)
            {
                throw;
                return false;
            }

        }
    
    }
}
