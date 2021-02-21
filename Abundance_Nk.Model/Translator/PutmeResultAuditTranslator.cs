using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{

    public class PutmeResultAuditTranslator : TranslatorBase<PutmeResultAudit, PUTME_RESULT_AUDIT>
    {
        private UserTranslator userTranslator;
        private PutmeResultTranslator putmeResultTranslator;
        public PutmeResultAuditTranslator()
        {
            userTranslator = new UserTranslator();
            putmeResultTranslator = new PutmeResultTranslator();
        }

        public override PutmeResultAudit TranslateToModel(PUTME_RESULT_AUDIT entity)
        {
            try
            {
                PutmeResultAudit NdPUTMEResultAudit = null;
                if (entity != null)
                {
                    NdPUTMEResultAudit = new PutmeResultAudit();
                    NdPUTMEResultAudit.Id = entity.Id;
                    NdPUTMEResultAudit.New_RegNo = entity.New_RegNo;
                    NdPUTMEResultAudit.Old_RegNo = entity.Old_RegNo;
                    NdPUTMEResultAudit.Result_Id = entity.Result_Id;
                    NdPUTMEResultAudit.User = userTranslator.Translate(entity.USER);
                    NdPUTMEResultAudit.Action = entity.Action;
                    NdPUTMEResultAudit.Time = entity.Time;
                    NdPUTMEResultAudit.Client = entity.Client;
                }

                return NdPUTMEResultAudit;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PUTME_RESULT_AUDIT TranslateToEntity(PutmeResultAudit model)
        {
            try
            {
                PUTME_RESULT_AUDIT entity = null;
                if (model != null)
                {
                    entity = new PUTME_RESULT_AUDIT();
                    entity.Id = model.Id;
                    entity.New_RegNo = model.New_RegNo;
                    entity.Old_RegNo = model.Old_RegNo;
                    entity.Result_Id = model.Result_Id;
                    entity.User_Id = model.User.Id;
                    entity.Action = model.Action;
                    entity.Time = model.Time;
                    entity.Client = model.Client;
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
