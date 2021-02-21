using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class EAssignmentTranslator : TranslatorBase<EAssignment, E_ASSIGNMENT>
    {
        private CourseTranslator courseTranslator;
        private CourseAllocationTranslator courseAllocationTranslator;
        public EAssignmentTranslator()
        {
            courseTranslator = new CourseTranslator();
            courseAllocationTranslator = new CourseAllocationTranslator();
        }
        public override E_ASSIGNMENT TranslateToEntity(EAssignment model)
        {
            try
            {
                E_ASSIGNMENT entity = null;
                if (model != null)
                {
                    entity = new E_ASSIGNMENT();
                    entity.Id = model.Id;
                    entity.Instructions = model.Instructions;
                    entity.Url = model.URL;
                    entity.Assignment = model.Assignment;
                    entity.Course_Id = model.Course.Id;
                    entity.Date_Set = model.DateSet;
                    entity.Due_Date = model.DueDate;
                    entity.Assignment_Text = model.AssignmentinText;
                    entity.Max_Score = model.MaxScore;
                    entity.IsDelete = model.IsDelete;
                    entity.Publish = model.Publish;
                    if (model.CourseAllocation?.Id > 0)
                    {
                        entity.Course_Allocation_Id = model.CourseAllocation.Id;
                    }

                }
                return entity;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override EAssignment TranslateToModel(E_ASSIGNMENT entity)
        {
            try
            {
                EAssignment model = null;
                if (entity != null)
                {
                    model = new EAssignment();
                    model.Assignment = entity.Assignment;
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.DateSet = entity.Date_Set;
                    model.DueDate = entity.Due_Date;
                    model.Id = entity.Id;
                    model.Instructions = entity.Instructions;
                    model.URL = entity.Url;
                    model.MaxScore = entity.Max_Score;
                    model.AssignmentinText = entity.Assignment_Text;
                    model.CourseAllocation = courseAllocationTranslator.Translate(entity.COURSE_ALLOCATION);
                    model.IsDelete = entity.IsDelete;
                    model.Publish = entity.Publish;
                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
