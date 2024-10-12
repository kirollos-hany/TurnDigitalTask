using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Logging;
using TurnDigital.Domain.Logging.Enums;
using TurnDigital.Domain.Security.Entities;

namespace TurnDigital.Domain.Security.Events;

public record LoginEvent(User User, LoginStatus Status) : IDomainEvent;