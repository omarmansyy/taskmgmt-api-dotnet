using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskMgmt.Api.Services.Interfaces;

namespace TaskMgmt.Api.Services.Implementations;
public class EmailService : IEmailService
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<EmailService> _log;
    public EmailService(IConfiguration cfg, ILogger<EmailService> log){ _cfg = cfg; _log = log; }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = _cfg["Email:Smtp:Host"];
        var from = _cfg["Email:From"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            _log.LogInformation("Email (dev): To={to}, Subject={subject}\n{html}", to, subject, htmlBody);
            return;
        }

        var port = int.TryParse(_cfg["Email:Smtp:Port"], out var p) ? p : 587;
        var user = _cfg["Email:Smtp:User"];
        var pass = _cfg["Email:Smtp:Pass"];
        var enableSsl = bool.TryParse(_cfg["Email:Smtp:Ssl"], out var ssl) ? ssl : true;

        using var client = new SmtpClient(host, port) { EnableSsl = enableSsl };
        if (!string.IsNullOrEmpty(user)) client.Credentials = new NetworkCredential(user, pass);
        using var msg = new MailMessage(from!, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(msg);
    }
}
