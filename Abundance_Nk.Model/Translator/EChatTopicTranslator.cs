using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{

    public class EChatTopicTranslator : TranslatorBase<EChatTopic, E_CHAT_TOPIC>
    {
        private CourseAllocationTranslator courseAllocationTranslator;
        public EChatTopicTranslator()
        {
            courseAllocationTranslator = new CourseAllocationTranslator();
        }
        public override EChatTopic TranslateToModel(E_CHAT_TOPIC entity)
        {
            try
            {
                EChatTopic model = null;
                if (entity != null)
                {
                    model = new EChatTopic();
                    model.EChatTopicId = entity.E_Chat_Topic_Id;
                    model.CourseAllocation = courseAllocationTranslator.Translate(entity.COURSE_ALLOCATION);
                    model.Active = entity.Active;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override E_CHAT_TOPIC TranslateToEntity(EChatTopic model)
        {
            try
            {
                E_CHAT_TOPIC entity = null;
                if (model != null)
                {
                    entity = new E_CHAT_TOPIC();
                    entity.E_Chat_Topic_Id = model.EChatTopicId;
                    entity.Active = model.Active;
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

    }
}
