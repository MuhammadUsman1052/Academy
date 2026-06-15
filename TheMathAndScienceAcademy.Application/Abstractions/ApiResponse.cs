
namespace TheMathAndScienceAcademy.Application.Abstractions;
public record ApiResponse<T>(bool Success, string Message, T? Data, IReadOnlyList<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data) => new(true, "Success.", data);
}
