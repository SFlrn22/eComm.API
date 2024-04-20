using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private readonly string FROM_EMAIL;
        private readonly string PASSWORD;
        private readonly string CLIENT;
        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            FROM_EMAIL = _appSettings.SmtpConfiguration.Address;
            PASSWORD = AesDecryptHelper.Decrypt(_appSettings.SmtpConfiguration.Password, AesKeyConfiguration.Key, AesKeyConfiguration.IV);
            CLIENT = _appSettings.SmtpConfiguration.Client;
        }
        public async Task SendEmailAsync(string subject, string body, string destination, byte[] pdf)
        {
            MailMessage message = new MailMessage()
            {
                From = new MailAddress(FROM_EMAIL),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            Attachment attachment = new Attachment(new MemoryStream(pdf.ConvertToPdf()), "Receipt.pdf");
            message.Attachments.Add(attachment);

            message.To.Add(new MailAddress(destination));

            var smtpClient = new SmtpClient(CLIENT)
            {
                Port = 587,
                Credentials = new NetworkCredential(FROM_EMAIL, PASSWORD),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);

            attachment.Dispose();
        }
    }
}
