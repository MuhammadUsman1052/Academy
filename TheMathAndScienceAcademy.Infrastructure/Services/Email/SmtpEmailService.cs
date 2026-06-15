using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
        };

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {ToEmail} with subject {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}", toEmail, subject);
            throw;
        }
    }

    public async Task SendAcademyAdminCredentialsAsync(string academyName, string adminName, string adminEmail, string temporaryPassword)
    {
        const string subject = "Welcome to The Math And Science Academy";
        var body = $"Dear {adminName},\n\n" +
                   "Your academy account has been created successfully.\n\n" +
                   $"Academy:\n{academyName}\n\n" +
                   $"Email:\n{adminEmail}\n\n" +
                   $"Temporary Password:\n{temporaryPassword}\n\n" +
                   "Please log in and change your password immediately.";

        await SendAsync(adminEmail, subject, body);
    }
}