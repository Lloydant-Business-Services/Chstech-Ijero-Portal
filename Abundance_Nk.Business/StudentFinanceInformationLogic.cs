using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class StudentFinanceInformationLogic : BusinessBaseLogic<StudentFinanceInformation, STUDENT_FINANCE_INFORMATION>
    {
        public StudentFinanceInformationLogic()
        {
            translator = new StudentFinanceInformationTranslator();
        }

        public bool Modify(StudentFinanceInformation studentFinanceInformation)
        {
            try
            {
                Expression<Func<STUDENT_FINANCE_INFORMATION, bool>> selector = p => p.Person_Id == studentFinanceInformation.Student.Id;
                STUDENT_FINANCE_INFORMATION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Person_Id = studentFinanceInformation.Student.Id;
                entity.Scholarship_Title = studentFinanceInformation.ScholarshipTitle;

                if (studentFinanceInformation.Mode != null && studentFinanceInformation.Mode.Id > 0)
                {
                    entity.Mode_Of_Finance_Id = studentFinanceInformation.Mode.Id;
                }              
                

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }




}
