using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;


namespace Abundance_Nk.Model.Translator
{
    public class StudentEmploymentInformationTranslator : TranslatorBase<StudentEmploymentInformation, STUDENT_EMPLOYMENT_INFORMATION>
    {
        private StudentTranslator studentTranslator;

        public StudentEmploymentInformationTranslator()
        {
            studentTranslator = new StudentTranslator();
        }

        public override StudentEmploymentInformation TranslateToModel(STUDENT_EMPLOYMENT_INFORMATION entity)
        {
            try
            {
                StudentEmploymentInformation model = null;
                if (entity != null)
                {
                    model = new StudentEmploymentInformation();
                    model.Id = entity.Student_Employment_Information_Id;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.PlaceOfLastEmployment = entity.Place_Of_Last_Employment;
                    model.StartDate = entity.Start_Date;
                    model.EndDate = entity.End_Date;

                    model.StartDay = new Value() { Id = entity.Start_Date.Day };
                    model.StartMonth = new Value() { Id = entity.Start_Date.Month };
                    model.StartYear = new Value() { Id = entity.Start_Date.Year };

                    model.EndDay = new Value() { Id = entity.End_Date.Day };
                    model.EndMonth = new Value() { Id = entity.End_Date.Month };
                    model.EndYear = new Value() { Id = entity.End_Date.Year };
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_EMPLOYMENT_INFORMATION TranslateToEntity(StudentEmploymentInformation model)
        {
            try
            {
                STUDENT_EMPLOYMENT_INFORMATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_EMPLOYMENT_INFORMATION();
                    entity.Student_Employment_Information_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Place_Of_Last_Employment = model.PlaceOfLastEmployment;
                    entity.Start_Date = model.StartDate;
                    entity.End_Date = model.EndDate;
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
