using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Marker interface cho Query có trả về dữ liệu
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Interface cho Handler xử lý IQuery
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
