using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentTypeTranslator : TranslatorBase<StudentType, STUDENT_TYPE>
    {
        public override StudentType TranslateToModel(STUDENT_TYPE studentTypeEntity)
        {
            try
            {
                StudentType studentType = null;
                if (studentTypeEntity != null)
                {
                    studentType = new StudentType();
                    studentType.Id = studentTypeEntity.Student_Type_Id;
                    studentType.Name = studentTypeEntity.Student_Type_Name;
                    studentType.Description = studentTypeEntity.Student_Type_Description;
                }

                return studentType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_TYPE TranslateToEntity(StudentType studentType)
        {
            try
            {
                STUDENT_TYPE studentTypeEntity = null;
                if (studentType != null)
                {
                    studentTypeEntity = new STUDENT_TYPE();
                    studentTypeEntity.Student_Type_Id = studentType.Id;
                    studentTypeEntity.Student_Type_Name = studentType.Name;
                    studentTypeEntity.Student_Type_Description = studentType.Description;
                }
                return studentTypeEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
