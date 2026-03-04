using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Marker interface cho Command không trả về dữ liệu (void / Unit)
/// </summary>
public interface ICommand : IRequest<Unit> { }

/// <summary>
/// Marker interface cho Command có trả về dữ liệu
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Interface cho Handler xử lý ICommand (void)
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit>
    where TCommand : ICommand { }

/// <summary>
/// Interface cho Handler xử lý ICommand có trả về dữ liệu
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }
