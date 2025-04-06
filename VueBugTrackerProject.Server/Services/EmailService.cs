using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Services
{
    /// <summary>
    /// Service to send an email.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
              _configuration = configuration;
        }

        /// <summary>
        /// Sends an email to a recepient.
        /// </summary>
        /// <param name="recepient"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        public async Task SendEmail(Account recepient, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("no reply","noreply@vuebugtracker.com"));
            message.To.Add(new MailboxAddress(recepient.UserName, recepient.Email));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = text
            };

            using var client = new SmtpClient();
            {
                //SMTP client is local IP address
                await client.ConnectAsync("127.0.0.1", 25);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            
        }

        /// <summary>
        /// Creates an email text with instructions to reset a user password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetPasswordResetEmailText(string id, string token)
        {
            return @$"<p>A request has been made to reset the passward for your account. If you made the request, please go to the link below:</p>
                    <a href='https://localhost:5173/resetpassword?id={id}&token={token}'>Reset password</a>
                    <p>The link will expire in 24 hours.</p>
                    <p>Please ignore this email if you did not make the request.</p>";
        }
    }
}
