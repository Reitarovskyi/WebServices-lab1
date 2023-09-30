using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace lab1.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string name, string email, string subject, string message)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var emailSender = emailSettings["Sender"];
                var emailSenderName = emailSettings["SenderName"];
                var password = emailSettings["Password"];

                var emailTo = "vovikusspambox@gmail.com";

                var mailServer = emailSettings["MailServer"];
                var mailPort = int.Parse(emailSettings["MailPort"]);

                MimeMessage mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(emailSenderName, emailSender));

                mimeMessage.To.Add(new MailboxAddress("", emailTo));

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = $"{name}<br/>{email}<br/><br/>{message}"
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(mailServer, mailPort, true);

                    await client.AuthenticateAsync(emailSender, password);
                    await client.SendAsync(mimeMessage);

                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
