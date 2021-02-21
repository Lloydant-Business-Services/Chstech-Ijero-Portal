using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Util;
using System.Net;


namespace Abundance_Nk.Model.Model
{
    public class Sms
    {
        string username = "0cd0f2f5";
        string password = "f7dbe54b";

        public string Sender { get; set; }

        public Sms()
        {
            this.Sender = "Sender";
        }

        public NexmoResponse SendSMS(string to, string text)
        {
            var wc = new WebClient() { BaseAddress = "http://rest.nexmo.com/sms/json" };
            wc.QueryString.Add("api_key", HttpUtility.UrlEncode(username));
            wc.QueryString.Add("api_secret", HttpUtility.UrlEncode(password));
            wc.QueryString.Add("from", HttpUtility.UrlEncode(Sender));
            wc.QueryString.Add("to", HttpUtility.UrlEncode(to));
            wc.QueryString.Add("text", HttpUtility.UrlEncode(text));
            return ParseSmsResponseJson(wc.DownloadString(""));
        }

        NexmoResponse ParseSmsResponseJson(string json)
        {
            json = json.Replace("-", "");  // hyphens are not allowed in in .NET var names
            return new JavaScriptSerializer().Deserialize<NexmoResponse>(json);
        }
    }
    
    public class NexmoResponse
    {
        public string Messagecount { get; set; }
        public List<NexmoMessageStatus> Messages { get; set; }
    }
    
    public class NexmoMessageStatus
    {
        public string MessageId { get; set; }
        public string To { get; set; }
        public string clientRef;
        public string Status { get; set; }
        public string ErrorText { get; set; }
        public string RemainingBalance { get; set; }
        public string MessagePrice { get; set; }
        public string Network;
    }

}



