namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(
        string toEmail,
        string subject,
        string body);

    Task SendAcademyAdminCredentialsAsync(
        string academyName,
        string adminName,
        string adminEmail,
        string temporaryPassword);
}