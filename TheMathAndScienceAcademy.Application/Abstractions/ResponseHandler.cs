using TheMathAndScienceAcademy.Application.Common;

namespace TheMathAndScienceAcademy.Application.Abstractions;

public abstract class ResponseHandler
{
    protected ApiResponse<T> Success<T>(T data, string message = ResponseMessages.Success)
        => new(true, message, data);

    protected ApiResponse<T> Created<T>(T data, string message)
        => new(true, message, data);

    protected ApiResponse<T> Updated<T>(T data, string message = "Updated successfully")
        => new(true, message, data);

    protected ApiResponse<T> Deleted<T>(string message)
        => new(true, message, default);

    protected ApiResponse<T> NotFound<T>(string message)
        => new(false, message, default);

    protected ApiResponse<T> BadRequest<T>(string message, IReadOnlyList<string>? errors = null)
        => new(false, message, default, errors);
}
