using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net;

public class DashboardMailer : IEmailSender
{
    public DashboardMailer()
    {

    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        string fromMail = "jeremypoppleton@gmail.com";
        string fromPassword = "nonggfrgaooohzuv";

        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = subject;
        message.To.Add(new MailAddress(email));
        message.Body = "<html><body> " + htmlMessage + " </body></html>";
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };
        await smtpClient.SendMailAsync(message);
    }
}