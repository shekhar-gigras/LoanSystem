using Gigras.Software.General.Model;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Gigras.Software.Generic.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string subject, string body);

        Task<bool> SendFormDetailsEmailAsync(string recipientEmail, string name, string email, string countryCode,
            string mobile, int interestedCategory, string message);

        Task<bool> AppointmentBookedDetailsEmailAsync(string recipientEmail, string projectname, DateTime appointmentDate,
                string timeslot);
        Task<bool> SubscriberEmailAsync(string recipientEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string subject, string body)
        {
            using (var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.EnableSsl = _emailSettings.SslRequest;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings!.FromEmailID!, _emailSettings.FromDisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(_emailSettings!.ToEmailID!);

                try
                {
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> SendFormDetailsEmailAsync(string recipientEmail, string name, string email, string countryCode,
            string mobile, int interestedCategory, string message)
        {
            using (var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.EnableSsl = _emailSettings.SslRequest;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmailID!, _emailSettings.FromDisplayName),
                    Subject = "Form Submission Details",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                // Build the HTML body
                var body = $@"
        <html>
        <body>
            <h2>Form Submission Details</h2>
            <table style='border-collapse: collapse; width: 100%;'>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>Name:</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{name}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>Email Address:</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{email}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>Mobile No:</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>+{countryCode} {mobile}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>Message:</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{message}</td>
                </tr>
            </table>
        </body>
        </html>";

                mailMessage.Body = body;

                try
                {
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> AppointmentBookedDetailsEmailAsync(string recipientEmail, string projectname, DateTime appointmentDate,
                string timeslot)
        {
            using (var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.EnableSsl = _emailSettings.SslRequest;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmailID!, _emailSettings.FromDisplayName),
                    Subject = "Appointment Book Details",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                // Build the HTML body
                var body = $@"
        <html>
                    <body>
                    <p>Dear Sir,</p>
                    <p>Thank you for booking an appointment with us!</p>
                    <p>Your appointment is scheduled for:</p>
                    <p>Project Name: {projectname}</p>
                    <p>Date: {appointmentDate.ToShortDateString()}</p>
                    <p>Time: {timeslot}</p>
                    <!-- Add more details as needed -->
                    <p>We look forward to meeting with you. If you have any questions or need to reschedule, please contact us.</p>
                     <p>Best regards,<br />
                        MIGSUN
                        </p>
                    </body>
                    </html>";

                mailMessage.Body = body;

                try
                {
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> SubscriberEmailAsync(string recipientEmail)
        {
            using (var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.EnableSsl = _emailSettings.SslRequest;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmailID!, _emailSettings.FromDisplayName),
                    Subject = "News Letter Subscription",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                // Build the HTML body
                var body = $@"
                        <html>
                        <body>
                        <p>Dear Sir,</p>
                        <p>Thank you for subscribing to our newsletter! We are excited to have you as part of our community.</p>
                        <!-- Add more content as needed -->
                        <p>If you have any questions, feel free to reach out.</p>
                        <p>Best regards,<br />
                        MIGSUN
                        </p>
                        </body>
                        </html>";

                mailMessage.Body = body;

                try
                {
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}