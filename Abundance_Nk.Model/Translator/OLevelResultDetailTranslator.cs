using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelResultDetailTranslator : TranslatorBase<OLevelResultDetail, APPLICANT_O_LEVEL_RESULT_DETAIL>
    {
        private OLevelResultTranslator oLevelResultTranslator;
        private OLevelSubjectTranslator oLevelSubjectTranslator;
        private OLevelGradeTranslator oLevelGradeTranslator;

        public OLevelResultDetailTranslator()
        {
            oLevelResultTranslator = new OLevelResultTranslator();
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
            oLevelGradeTranslator = new OLevelGradeTranslator();
        }

        public override OLevelResultDetail TranslateToModel(APPLICANT_O_LEVEL_RESULT_DETAIL entity)
        {
            try
            {
                OLevelResultDetail oLevelResultDetail = null;
                if (entity != null)
                {
                    oLevelResultDetail = new OLevelResultDetail();
                    oLevelResultDetail.Id = entity.Applicant_O_Level_Result_Detail_Id;
                    oLevelResultDetail.Header = oLevelResultTranslator.Translate(entity.APPLICANT_O_LEVEL_RESULT);
                    oLevelResultDetail.Subject = oLevelSubjectTranslator.Translate(entity.O_LEVEL_SUBJECT);
                    oLevelResultDetail.Grade = oLevelGradeTranslator.Translate(entity.O_LEVEL_GRADE);
                }

                return oLevelResultDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_O_LEVEL_RESULT_DETAIL TranslateToEntity(OLevelResultDetail oLevelResultDetail)
        {
            try
            {
                APPLICANT_O_LEVEL_RESULT_DETAIL entity = null;
                if (oLevelResultDetail != null)
                {
                    entity = new APPLICANT_O_LEVEL_RESULT_DETAIL();
                    entity.Applicant_O_Level_Result_Detail_Id = oLevelResultDetail.Id;
                    entity.Applicant_O_Level_Result_Id = oLevelResultDetail.Header.Id;
                    entity.O_Level_Subject_Id= oLevelResultDetail.Subject.Id;
                    entity.O_Level_Grade_Id = oLevelResultDetail.Grade.Id;
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
