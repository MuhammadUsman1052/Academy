namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface ITransactionService
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
}
