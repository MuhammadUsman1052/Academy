
namespace TheMathAndScienceAcademy.Application.Abstractions;
public record ApiResponse<T>(bool Success, string Message, T? Data)
{
    public static ApiResponse<T> Ok(T data) => new(true, "Success", data);
}
