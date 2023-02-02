using BL.Common;
using System.Net.Mail;

namespace BL.Helpers
{
    public class MailHelper
    {
        public static void SendEmail(string emailReceiving, string token, AppSettings appData, MailSettings mailSettings)
        {
            string emailOrigin = appData.SupportEmail;
            string passswordOrigin = appData.SupportPass;
            string urlDomain = appData.UrlDomain;
            string url = $"{urlDomain}{mailSettings.Path}{token}";
            MailMessage oMailMessage = new(
                emailOrigin,
                emailReceiving,
                mailSettings.Subject,
                $"{mailSettings.Body}{"<a href='" +url + "'>'"}{mailSettings.LinkDescription}{"</a>"}")
            {
                IsBodyHtml = true
            };
            SmtpClient oSmtpClient = new("smtp.gmail.com")
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new System.Net.NetworkCredential(emailOrigin, passswordOrigin)
            };
            oSmtpClient.Send(oMailMessage);
            oSmtpClient.Dispose();
        }
    }
}
