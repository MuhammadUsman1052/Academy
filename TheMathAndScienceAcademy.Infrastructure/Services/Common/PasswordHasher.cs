using BCryptNet = BCrypt.Net.BCrypt;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Infrastructure.Services.Common;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
        => BCryptNet.HashPassword(password);
}
