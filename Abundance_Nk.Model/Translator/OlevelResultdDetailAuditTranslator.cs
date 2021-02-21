using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class OlevelResultdDetailAuditTranslator:TranslatorBase<OlevelResultdDetailsAudit,APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT>
    {
        private OLevelResultTranslator oLevelResultTranslator;
        private OLevelSubjectTranslator oLevelSubjectTranslator;
        private OLevelGradeTranslator oLevelGradeTranslator;
        private OLevelResultDetailTranslator oLevelResultDetailTranslator;
        private UserTranslator userTranslator;
        public OlevelResultdDetailAuditTranslator()
        {
            oLevelResultTranslator = new OLevelResultTranslator();
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
            oLevelGradeTranslator = new OLevelGradeTranslator();
            oLevelResultDetailTranslator = new OLevelResultDetailTranslator();
            userTranslator = new UserTranslator();
        }

        public override OlevelResultdDetailsAudit TranslateToModel(APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT entity)
        {
            try
            {
                OlevelResultdDetailsAudit oLevelResultDetail = null;
                if (entity != null)
                {
                    oLevelResultDetail = new OlevelResultdDetailsAudit();
                    oLevelResultDetail.Id = entity.Applicant_O_Level_Result_Detail_Audit_Id;
                    oLevelResultDetail.Header = oLevelResultTranslator.Translate(entity.APPLICANT_O_LEVEL_RESULT);
                    oLevelResultDetail.Subject = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT);
                    oLevelResultDetail.Grade = oLevelGradeTranslator.Translate(entity.O_LEVEL_GRADE);
                    oLevelResultDetail.Operation = entity.Operation;
                    oLevelResultDetail.Action = entity.Action;
                    oLevelResultDetail.Client = entity.Client;
                    oLevelResultDetail.Time = entity.Time;
                    oLevelResultDetail.User = userTranslator.Translate(entity.USER);
                }

                return oLevelResultDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT TranslateToEntity(OlevelResultdDetailsAudit oLevelResultDetail)
        {
            try
            {
                APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT entity = null;
                if (oLevelResultDetail != null)
                {
                    entity = new APPLICANT_O_LEVEL_RESULT_DETAIL_AUDIT();
                    entity.Applicant_O_Level_Result_Detail_Audit_Id = oLevelResultDetail.Id;
                    entity.Applicant_O_Level_Result_Detail_Id = oLevelResultDetail.Id;
                    entity.Applicant_O_Level_Result_Id = oLevelResultDetail.Header.Id;
                    entity.O_Level_Subject_Id= oLevelResultDetail.Subject.Id;
                    entity.O_Level_Grade_Id = oLevelResultDetail.Grade.Id;
                    entity.Operation = oLevelResultDetail.Operation;
                    entity.Action = oLevelResultDetail.Action;
                    entity.Client = oLevelResultDetail.Client;
                    entity.Time = oLevelResultDetail.Time;
                    entity.User_Id = oLevelResultDetail.User.Id;
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
