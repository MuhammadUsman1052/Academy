using System.Security.Cryptography;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Infrastructure.Services;

public class TemporaryPasswordGenerator : ITemporaryPasswordGenerator
{
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Specials = "!@#$%^&*";
    private static readonly char[] All = (Upper + Lower + Digits + Specials).ToCharArray();

    public string Generate()
    {
        Span<char> password = stackalloc char[12];

        password[0] = Upper[RandomNumberGenerator.GetInt32(Upper.Length)];
        password[1] = Lower[RandomNumberGenerator.GetInt32(Lower.Length)];
        password[2] = Digits[RandomNumberGenerator.GetInt32(Digits.Length)];
        password[3] = Specials[RandomNumberGenerator.GetInt32(Specials.Length)];

        for (var i = 4; i < password.Length; i++)
        {
            password[i] = All[RandomNumberGenerator.GetInt32(All.Length)];
        }

        for (var i = password.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return password.ToString();
    }
}
