using FlowerShop.Context;
using FlowerShop.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body, string htmlBody)
    {
        using (MailMessage mail = new MailMessage())
        {
            mail.From = new MailAddress("florist.bloomshop@gmail.com", "Bloom Shop");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            mail.AlternateViews.Add(htmlView);

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("florist.bloomshop@gmail.com", "vkbi xsdn dboc osbe");
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);
            }
        }
    }
}
