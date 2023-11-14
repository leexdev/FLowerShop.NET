using System;
using System.Net;
using System.Net.Mail;

public class EmailService
{
    public void SendEmail(string toEmail, string subject, string body, string htmlBody)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("lepro2883@gmail.com");
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;

        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
        mail.AlternateViews.Add(htmlView);

        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential("lepro2883@gmail.com", "xrpz hlnm zygh ceay");
        smtp.EnableSsl = true;

        smtp.Send(mail);
    }
}
