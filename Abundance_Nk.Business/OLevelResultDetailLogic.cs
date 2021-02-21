using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class OLevelResultDetailLogic : BusinessBaseLogic<OLevelResultDetail, APPLICANT_O_LEVEL_RESULT_DETAIL>
    {
        public OLevelResultDetailLogic()
        {
            translator = new OLevelResultDetailTranslator();
        }

      
        public bool DeleteBy(OLevelResult oLevelResult,OlevelResultdDetailsAudit olevelResultdDetailsAudit)
        {
            try
            {
                Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> selector = o => o.Applicant_O_Level_Result_Id == oLevelResult.Id;
                OLevelResultDetailAuditLogic oLevelResultDetailAuditLogic = new OLevelResultDetailAuditLogic();
                List<OLevelResultDetail> oLevelResultDetails = GetModelsBy(selector);
                foreach (OLevelResultDetail oLevelResultDetail in oLevelResultDetails)
                {
                    OlevelResultdDetailsAudit audit = new OlevelResultdDetailsAudit();
                    audit = olevelResultdDetailsAudit;
                    audit.Time = DateTime.Now;
                    audit.Grade = oLevelResultDetail.Grade;
                    audit.Header = oLevelResultDetail.Header;
                    audit.Subject = oLevelResultDetail.Subject;
                    audit.OLevelResultDetail = oLevelResultDetail;
                    oLevelResultDetailAuditLogic.Create(audit);
                }

               return Delete(selector);
            }            
            catch (Exception)
            {
                throw;
            }
        }

        public OLevelResultDetail Create(OLevelResultDetail oLevelResultDetail,OlevelResultdDetailsAudit olevelResultdDetailsAudit)
        {
            OLevelResultDetail resultDetail = new OLevelResultDetail();
            try
            {
                OLevelResultDetailAuditLogic oLevelResultDetailAuditLogic = new OLevelResultDetailAuditLogic();
                resultDetail =  base.Create(oLevelResultDetail);
                olevelResultdDetailsAudit.Time = DateTime.Now;
                olevelResultdDetailsAudit.OLevelResultDetail = resultDetail;
                olevelResultdDetailsAudit.Grade = oLevelResultDetail.Grade;
                olevelResultdDetailsAudit.Header = oLevelResultDetail.Header;
                olevelResultdDetailsAudit.Subject = oLevelResultDetail.Subject;
                olevelResultdDetailsAudit.OLevelResultDetail = oLevelResultDetail;
                oLevelResultDetailAuditLogic.Create(olevelResultdDetailsAudit);
                return resultDetail;

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public int Create(List<OLevelResultDetail> oLevelResultDetaillList,OlevelResultdDetailsAudit olevelResultdDetailsAudit)
        {
            OLevelResultDetail resultDetail = new OLevelResultDetail();
            int rowsAdded = 0;
            try
            {
                OLevelResultDetailAuditLogic oLevelResultDetailAuditLogic = new OLevelResultDetailAuditLogic();
                foreach (OLevelResultDetail oLevelResultDetail in oLevelResultDetaillList)
                {
                    Create(oLevelResultDetail, olevelResultdDetailsAudit);
                    rowsAdded++;
                }

                return rowsAdded;
            }
            catch (Exception)
            {
                
                throw;
            }
        }


    }




}
