using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;

namespace MailerApp.Business
{
    public class InfoBipSMS
    {
        //private string baseUrl = "https://api.infobip.com/sms/1/text/multi";
        private string baseUrl = "https://api.infobip.com/sms/1/text/single";
        private string username = "LLoydantng";
        private string password = "2@lloydant";


        public InfoBipResponse SendSMS(InfoSMS msg)
        {
            try
            {

                string authorizationHeader = "Basic " + ConvertToBase64(username + ":" + password);
                string json = "";
                string jsondata = "";
                InfoBipResponse smsResponse = new InfoBipResponse();
                json = new JavaScriptSerializer().Serialize(msg);
                using (var request = new WebClient())
                {
                    request.Headers[HttpRequestHeader.Accept] = "application/json";
                    request.Headers[HttpRequestHeader.ContentType] = "application/json";
                    request.Headers.Add("authorization", authorizationHeader);
                    jsondata = request.UploadString(baseUrl, "POST", json);

                }

                smsResponse = new JavaScriptSerializer().Deserialize<InfoBipResponse>(jsondata);
                return smsResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
       
        private string ConvertToBase64(string input)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var base64string = Convert.ToBase64String(bytes);
                return base64string;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           
        }

        private string ConvertFromBase64(string base64Input)
        {
            try
            {
                var data = Convert.FromBase64String(base64Input);
                string outputString = Encoding.UTF8.GetString(data);
                return outputString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
   
     
    }

    public class InfoSMS
    {
        public string from { get; set; }
        public string to { get; set; }
        public string text { get; set; }
    }


    public class Status
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class message
    {
        public string to { get; set; }
        public Status status { get; set; }
        public int smsCount { get; set; }
        public string messageId { get; set; }
    }

    public class InfoBipResponse
    {
        public List<message> messages { get; set; }
    }

}
