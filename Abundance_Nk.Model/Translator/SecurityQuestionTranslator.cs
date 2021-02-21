using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class SecurityQuestionTranslator : TranslatorBase<SecurityQuestion, SECURITY_QUESTION>
    {
        public override SecurityQuestion TranslateToModel(SECURITY_QUESTION entity)
        {
            try
            {
                SecurityQuestion model = null;
                if (entity != null)
                {
                    model = new SecurityQuestion();
                    model.Id = entity.Security_Question_Id;
                    model.Name = entity.Question;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SECURITY_QUESTION TranslateToEntity(SecurityQuestion model)
        {
            try
            {
                SECURITY_QUESTION entity = null;
                if (model != null)
                {
                    entity = new SECURITY_QUESTION();
                    entity.Security_Question_Id = model.Id;
                    entity.Question = model.Name;
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
