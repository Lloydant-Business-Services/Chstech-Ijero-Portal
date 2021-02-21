using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class PutmeResultLogic : BusinessBaseLogic<PutmeResult, PUTME_RESULT>
    {
        public PutmeResultLogic()
        {
              translator = new PutmeResultTranslator();
        }

      
        public bool Modify(PutmeResult result)
        {
            try
            {
                Expression<Func<PUTME_RESULT, bool>> selector = s => s.ID == result.Id;
                PUTME_RESULT entity = GetEntityBy(selector);

            if (entity == null)
            {
                throw new Exception(NoItemFound);
            }
            entity.REGNO = result.RegNo;
            entity.EXAMNO = result.ExamNo;

            int modifiedRecordCount = Save();
            if (modifiedRecordCount <= 0)
            {
                return false;
            }

            return true;
            }
            catch (Exception ex)
            {
                
                throw;
            }

        }

        public bool Modify(PutmeResult result, PutmeResultAudit resultAudit)
        {
            try
            {
                Expression<Func<PUTME_RESULT, bool>> selector = p => p.ID == result.Id;
                PUTME_RESULT resultEntity = GetEntityBy(selector);

                bool audited = CreateAudit(result, resultAudit, resultEntity);
                if (audited)
                {
                    if (resultEntity == null || resultEntity.ID <= 0)
                    {
                        throw new Exception(NoItemFound);
                    }

                    resultEntity.REGNO = result.RegNo;
                    resultEntity.EXAMNO = result.ExamNo;
                    int modifiedRecordCount = Save();
                    if (modifiedRecordCount <= 0)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                
                throw;
            }
          
        }

        private bool CreateAudit(PutmeResult result, PutmeResultAudit audit, PUTME_RESULT resultEntity)
        {
            try
            {
                if (result.Id == resultEntity.ID)
                {
                    audit.Result_Id =  result.Id ;
                    audit.New_RegNo = result.ExamNo;
                    PutmeResult oldResult = translator.Translate(resultEntity);
                    audit.Old_RegNo = oldResult.ExamNo;

                    PostUtmeResultAuditLogic resultAuditLogic = new PostUtmeResultAuditLogic();
                    PutmeResultAudit personAudit = resultAuditLogic.Create(audit);
                    if (personAudit == null || personAudit.Id <= 0)
                    {
                        return false;
                    }

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

       

    
    
    }
}
