namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(string password);
}
