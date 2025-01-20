using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Gigras.Software.General.Helper
{
    public static class EmailHelper
    {
        public static async Task SendEmailAsync(IConfiguration _configuration, string recipientName, string recipientEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress(recipientName, recipientEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], Convert.ToInt32(emailSettings["SmtpPort"]), true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["PasswordEnc"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public static async Task<string> ReadEmailTemplate(string rootpath, string templatename, string callbackUrl, string Content, string Subject)
        {
            string filePath = $"{rootpath}//Templates//" + templatename;
            string fileContent = "";

            if (File.Exists(filePath))
            {
                fileContent = File.ReadAllText(filePath);
                fileContent = fileContent.Replace("targetedSrc", callbackUrl);
                fileContent = fileContent.Replace("[BODY]", Content);
                fileContent = fileContent.Replace("[SUBJECT]", Subject);
            }

            return fileContent;
        }
    }
}