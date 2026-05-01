using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
namespace TaMarcado.Infraestrutura.Services;

public class SmtpEmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = configuration["Email:Host"]!;
        var port = int.Parse(configuration["Email:Port"]!);
        var user = configuration["Email:User"]!;
        var password = configuration["Email:Password"]!;
        var from = configuration["Email:From"]!;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var body = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(user, password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
