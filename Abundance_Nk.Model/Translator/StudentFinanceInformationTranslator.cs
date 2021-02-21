using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentFinanceInformationTranslator : TranslatorBase<StudentFinanceInformation, STUDENT_FINANCE_INFORMATION>
    {
        private StudentTranslator studentTranslator;
        private ModeOfFinanceTranslator modeOfFinanceTranslator;

        public StudentFinanceInformationTranslator()
        {
            studentTranslator = new StudentTranslator();
            modeOfFinanceTranslator = new ModeOfFinanceTranslator();
        }

        public override StudentFinanceInformation TranslateToModel(STUDENT_FINANCE_INFORMATION entity)
        {
            try
            {
                StudentFinanceInformation model = null;
                if (entity != null)
                {
                    model = new StudentFinanceInformation();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Mode = modeOfFinanceTranslator.Translate(entity.MODE_OF_FINANCE);
                    model.ScholarshipTitle = entity.Scholarship_Title;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_FINANCE_INFORMATION TranslateToEntity(StudentFinanceInformation model)
        {
            try
            {
                STUDENT_FINANCE_INFORMATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_FINANCE_INFORMATION();
                    entity.Person_Id = model.Student.Id;
                    entity.Mode_Of_Finance_Id = model.Mode.Id;
                    entity.Scholarship_Title= model.ScholarshipTitle;
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
