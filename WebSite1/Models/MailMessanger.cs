using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebSite.Models
{
    static public class MailMessanger
    {
        static public bool SendMessage(string emailFrom, string emailTo,string login,string password)
        {
            bool result = true;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(new MailAddress(emailTo));
                mail.Subject = "Subject";
                mail.Body = "Text";

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.mail.ru";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(login, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                result = false;
                //throw new Exception("Mail.Send: " + e.Message);
            }
            return result;
        }
    }
}