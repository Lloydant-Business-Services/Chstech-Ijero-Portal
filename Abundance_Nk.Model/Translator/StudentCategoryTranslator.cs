
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentCategoryTranslator : TranslatorBase<StudentCategory, STUDENT_CATEGORY>
    {
        public override StudentCategory TranslateToModel(STUDENT_CATEGORY studentCategoryEntity)
        {
            try
            {
                StudentCategory studentCategory = null;
                if (studentCategoryEntity != null)
                {
                    studentCategory = new StudentCategory();
                    studentCategory.Id = studentCategoryEntity.Student_Category_Id;
                    studentCategory.Name = studentCategoryEntity.Student_Category_Name;
                    studentCategory.Description = studentCategoryEntity.Student_Category_Description;
                }

                return studentCategory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_CATEGORY TranslateToEntity(StudentCategory studentCategory)
        {
            try
            {
                STUDENT_CATEGORY studentCategoryEntity = null;
                if (studentCategory != null)
                {
                    studentCategoryEntity = new STUDENT_CATEGORY();
                    studentCategoryEntity.Student_Category_Id = studentCategory.Id;
                    studentCategoryEntity.Student_Category_Name = studentCategory.Name;
                    studentCategoryEntity.Student_Category_Description = studentCategory.Description;
                }

                return studentCategoryEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }


}
