using MediatR;

namespace TurnDigital.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    
}

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : IRequest
{
    
}