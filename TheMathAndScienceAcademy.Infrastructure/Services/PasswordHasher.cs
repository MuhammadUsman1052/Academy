using TheMathAndScienceAcademy.Application.Abstractions;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TheMathAndScienceAcademy.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
        => BCryptNet.HashPassword(password);
}
