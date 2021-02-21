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
    public class EChatResponseLogic : BusinessBaseLogic<EChatResponse, E_CHAT_RESPONPSE>
    {
        public EChatResponseLogic()
        {
            translator = new EChatResponseTranslator();
        }
        public List<EChatBoard> LoadChatBoard(Student person, User user, long courseAllocationId)
        {
            List<EChatBoard> eChatBoardList = new List<EChatBoard>();
            try
            {
                var responses = GetModelsBy(f => f.E_CHAT_TOPIC.Course_Allocation_Id == courseAllocationId).OrderBy(g => g.Response_Time).ToList();
                if (responses?.Count > 0)
                {
                    foreach (var item in responses)
                    {
                        EChatBoard eChatBoard = new EChatBoard();

                        eChatBoard.Sender = item.Student != null ? item.Student.FullName : "Lecturer";
                        eChatBoard.Response = item.Response;
                        eChatBoard.DateSent = item.Response_Time.ToString("U");
                        if (person?.Id > 0)
                        {
                            eChatBoard.ActiveSender = item.Student?.Id == person.Id ? true : false;
                        }
                        else if (user?.Id > 0)
                        {
                            eChatBoard.ActiveSender = item.User?.Id == user.Id ? true : false;
                        }


                        // eChatBoard.Topic = item.EChatTopic.EContentType.Name;
                        eChatBoard.FilePath = item.Upload;
                        eChatBoardList.Add(eChatBoard);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return eChatBoardList;
        }
        public void SaveChatResponse(long courseAllocationId, Student person, string response, User user)
        {
            try
            {
                if (courseAllocationId > 0 && (person?.Id > 0 || user?.Id > 0) && response != null)
                {
                    EChatTopicLogic eChatTopicLogic = new EChatTopicLogic();
                    var eChatTopic = eChatTopicLogic.GetModelsBy(g => g.Course_Allocation_Id == courseAllocationId).FirstOrDefault();
                    if (eChatTopic?.EChatTopicId > 0)
                    {
                        EChatResponse eChatResponse = new EChatResponse();
                        eChatResponse.EChatTopic = eChatTopic;
                        eChatResponse.Response_Time = DateTime.Now;
                        eChatResponse.Response = response;
                        eChatResponse.Active = true;

                        if (person?.Id > 0)
                        {
                            eChatResponse.Student = person;
                        }
                        if (user?.Id > 0)
                        {
                            eChatResponse.User = user;
                        }
                        Create(eChatResponse);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
