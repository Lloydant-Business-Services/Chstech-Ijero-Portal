using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class EAssignmentSubmissionTranslator : TranslatorBase<EAssignmentSubmission, E_ASSIGNMENT_SUBMISSION>
    {
        private EAssignmentTranslator eAssignmentTranslator;
        private StudentTranslator studentTranslator;
        public EAssignmentSubmissionTranslator()
        {
            eAssignmentTranslator = new EAssignmentTranslator();
            studentTranslator = new StudentTranslator();
        }
        public override E_ASSIGNMENT_SUBMISSION TranslateToEntity(EAssignmentSubmission model)
        {
            try
            {
                E_ASSIGNMENT_SUBMISSION entity = null;
                if (model != null)
                {
                    entity = new E_ASSIGNMENT_SUBMISSION();
                    entity.Assignment_Content = model.AssignmentContent;
                    entity.Assignment_Id = model.EAssignment.Id;
                    entity.Date_Submitted = model.DateSubmitted;
                    entity.Id = model.Id;
                    entity.Remarks = model.Remarks;
                    entity.Score = model.Score;
                    entity.Student_Id = model.Student.Id;
                    entity.Submission_in_Text = model.TextSubmission;

                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override EAssignmentSubmission TranslateToModel(E_ASSIGNMENT_SUBMISSION entity)
        {
            try
            {
                EAssignmentSubmission model = null;
                if (entity != null)
                {
                    model = new EAssignmentSubmission();
                    model.Id = entity.Id;
                    model.AssignmentContent = entity.Assignment_Content;
                    model.DateSubmitted = entity.Date_Submitted;
                    model.EAssignment = eAssignmentTranslator.Translate(entity.E_ASSIGNMENT);
                    model.Remarks = entity.Remarks;
                    model.Score = entity.Score;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.TextSubmission = entity.Submission_in_Text;
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
