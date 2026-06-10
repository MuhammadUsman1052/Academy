namespace TheMathAndScienceAcademy.Application.Abstractions;

public abstract class ResponseHandler
{
    protected ApiResponse<T> Created<T>(T data, string message)
        => new(true, message, data);

    protected ApiResponse<T> Updated<T>(T data, string message = "Updated successfully")
        => new(true, message, data);

    protected ApiResponse<T> Deleted<T>(string message)
        => new(true, message, default);

    protected ApiResponse<T> NotFound<T>(string message)
        => new(false, message, default);

    protected ApiResponse<T> BadRequest<T>(string message)
        => new(false, message, default);
}
