using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class StudentStatusTranslator : TranslatorBase<StudentStatus, STUDENT_STATUS>
    {
        public override StudentStatus TranslateToModel(STUDENT_STATUS entity)
        {
            try
            {
                StudentStatus model = null;
                if (entity != null)
                {
                    model = new StudentStatus();
                    model.Id = entity.Student_Status_Id;
                    model.Name = entity.Student_Status_Name;
                    model.Description = entity.Student_Status_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_STATUS TranslateToEntity(StudentStatus model)
        {
            try
            {
                STUDENT_STATUS entity = null;
                if (model != null)
                {
                    entity = new STUDENT_STATUS();
                    entity.Student_Status_Id = model.Id;
                    entity.Student_Status_Name = model.Name;
                    entity.Student_Status_Description = model.Description;
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
