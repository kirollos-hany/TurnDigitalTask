using MediatR;

namespace TurnDigital.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
    
}