using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class EmailClient
    {
        public EmailClient(string Subject, string EmailMessage, string ReceiverAddress)
        {
            try
            {
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("Semesterresult@fpno.edu.ng", "1@lloydant");
                smtpServer.EnableSsl = true;
                smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpServer.Timeout = 20000;
                MailMessage eMailMessage = new MailMessage();
                eMailMessage.From = new MailAddress("Semesterresult@fpno.edu.ng", "NEKEDE RESULT UNIT");
                eMailMessage.To.Add(ReceiverAddress);
                eMailMessage.Subject = Subject;
                eMailMessage.Body = EmailMessage;

                smtpServer.Send("Semesterresult@fpno.edu.ng", ReceiverAddress, Subject, EmailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
