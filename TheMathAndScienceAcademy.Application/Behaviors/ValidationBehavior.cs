using FluentValidation;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;

namespace TheMathAndScienceAcademy.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => failure.ErrorMessage)
            .Distinct()
            .ToList();

        if (errors.Count == 0)
        {
            return await next();
        }

        return CreateValidationResponse(errors);
    }

    private static TResponse CreateValidationResponse(IReadOnlyList<string> errors)
    {
        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
        {
            var dataType = responseType.GetGenericArguments()[0];
            var apiResponseType = typeof(ApiResponse<>).MakeGenericType(dataType);
            var response = Activator.CreateInstance(apiResponseType, false, ResponseMessages.ValidationFailed, null, errors);

            return (TResponse)response!;
        }

        throw new ValidationException(errors.Select(error => new FluentValidation.Results.ValidationFailure(string.Empty, error)));
    }
}
