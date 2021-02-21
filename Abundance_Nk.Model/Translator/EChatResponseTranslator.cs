using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class EChatResponseTranslator : TranslatorBase<EChatResponse, E_CHAT_RESPONPSE>
    {
        private EChatTopicTranslator eChatTopicTranslator;
        private StudentTranslator studentTranslator;
        private UserTranslator userTranslator;
        public EChatResponseTranslator()
        {
            eChatTopicTranslator = new EChatTopicTranslator();
            studentTranslator = new StudentTranslator();
            userTranslator = new UserTranslator();
        }
        public override EChatResponse TranslateToModel(E_CHAT_RESPONPSE entity)
        {
            try
            {
                EChatResponse model = null;
                if (entity != null)
                {
                    model = new EChatResponse();
                    model.EChatResponseId = entity.E_Chat_Response_Id;
                    model.EChatTopic = eChatTopicTranslator.Translate(entity.E_CHAT_TOPIC);
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Active = entity.Active;
                    model.Upload = entity.Upload;
                    model.Response_Time = entity.Response_Time;
                    model.Response = entity.Response;


                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override E_CHAT_RESPONPSE TranslateToEntity(EChatResponse model)
        {
            try
            {
                E_CHAT_RESPONPSE entity = null;
                if (model != null)
                {
                    entity = new E_CHAT_RESPONPSE();
                    entity.E_Chat_Response_Id = model.EChatResponseId;
                    entity.Active = model.Active;
                    entity.Response = model.Response;
                    entity.Response_Time = model.Response_Time;
                    entity.Upload = model.Upload;

                    if (model.Student?.Id > 0)
                    {
                        entity.Student_Id = model.Student.Id;
                    }
                    if (model.User?.Id > 0)
                    {
                        entity.User_Id = model.User.Id;
                    }
                    if (model.EChatTopic?.EChatTopicId > 0)
                    {
                        entity.E_Chat_Topic_Id = model.EChatTopic.EChatTopicId;
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
