namespace LMS.Application.Common;

/// <summary>
/// Marker interface for commands
/// </summary>
public interface ICommand<out TResult>
{
}

/// <summary>
/// Handler for commands
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
