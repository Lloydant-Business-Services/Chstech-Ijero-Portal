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
    public class StudentEmploymentInformationLogic : BusinessBaseLogic<StudentEmploymentInformation, STUDENT_EMPLOYMENT_INFORMATION>
    {
        public StudentEmploymentInformationLogic()
        {
            translator = new StudentEmploymentInformationTranslator();
        }

        public bool Modify(StudentEmploymentInformation studentEmploymentInformation)
        {
            try
            {
                STUDENT_EMPLOYMENT_INFORMATION entity = GetEntityBy(s => s.Student_Employment_Information_Id == studentEmploymentInformation.Id);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Place_Of_Last_Employment = studentEmploymentInformation.PlaceOfLastEmployment;
                if (studentEmploymentInformation.EndDate != null)
                {
                    entity.End_Date = studentEmploymentInformation.EndDate;
                }
                if (studentEmploymentInformation.StartDate != null)
                {
                    entity.Start_Date = studentEmploymentInformation.StartDate;
                }

                if (studentEmploymentInformation.Student != null)
                {
                    entity.Person_Id = studentEmploymentInformation.Student.Id;
                }
                int modified = Save();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    
    
    }
}
