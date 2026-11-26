using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Api.Services;

public class EmailService(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public async Task SendEmailAsync(string recipient, string subject, string htmlBody)
    {
        // --- SMTP Config ---
        var mailSettings = _config.GetSection("MailSettings");

        // --- Validate SMTP ---
        var host = mailSettings["Host"] ??
            throw new InvalidOperationException("SMTP host is missing.");

        var port = int.Parse(mailSettings["Port"] ??
            throw new InvalidOperationException("SMTP Port is missing."));

        var username = mailSettings["Username"] ??
            throw new InvalidOperationException("SMTP Username is missing.");

        var password = mailSettings["Password"] ??
            throw new InvalidOperationException("SMTP Password is missing.");

        var senderEmail = mailSettings["SenderEmail"] ??
            throw new InvalidOperationException("SMTP SenderEmail is missing.");

        var senderName = mailSettings["SenderName"] ??
            throw new InvalidOperationException("SMTP SenderName is missing.");

        // --- Create email ---
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(senderName, senderEmail));
        email.To.Add(MailboxAddress.Parse(recipient));
        email.Subject = subject;

        email.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        // --- Connect to SMTP ---
        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

        // --- Authenticate SMTP credentials ---
        await smtp.AuthenticateAsync(username, password);

        // --- Send email and disconnect SMTP ---
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}