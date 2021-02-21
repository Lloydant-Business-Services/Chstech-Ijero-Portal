using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{

    public class EAssignmentSubmissionLogic : BusinessBaseLogic<EAssignmentSubmission, E_ASSIGNMENT_SUBMISSION>
    {
        public EAssignmentSubmissionLogic()
        {
            translator = new EAssignmentSubmissionTranslator();
        }

        public List<EAssignmentSubmission> GetBy(long AssignmentId)
        {
            try
            {
                return GetModelsBy(a => a.Assignment_Id == AssignmentId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Modify(EAssignmentSubmission model)
        {
            try
            {
                E_ASSIGNMENT_SUBMISSION entity = GetEntityBy(a => a.Id == model.Id);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Assignment_Content = model.AssignmentContent;
                entity.Remarks = model.Remarks;
                entity.Date_Submitted = model.DateSubmitted;
                entity.Score = model.Score;
                entity.Submission_in_Text = model.TextSubmission;

                if (model.EAssignment != null && model.EAssignment.Id > 0)
                {
                    entity.Assignment_Id = model.EAssignment.Id;
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
