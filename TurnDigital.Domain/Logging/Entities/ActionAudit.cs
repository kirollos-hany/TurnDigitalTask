using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Logging.Enums;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Domain.Logging.Entities;

public class ActionAudit : IEntity
{
    private ActionAudit()
    {

    }

    public ActionAudit(int userId, string ipAddress, string userAgent, DeviceInfo? deviceInfo, string entityName, string entityId, string actionBy, string actionByRoles, AuditActionType type, string? oldValues, string values, string affectedColumns)
    {
        IpAddress = ipAddress;

        UserAgent = userAgent;

        DeviceInfo = deviceInfo;
        
        TimeStamp = DateTime.UtcNow;

        EntityName = entityName;

        EntityId = entityId;
        
        ActionBy = actionBy;
        
        ActionByRoles = actionByRoles;

        Type = type;
        
        OldValues = oldValues;
        
        Values = values;
        
        AffectedColumns = affectedColumns;

        UserId = userId;
    }
    
    public int Id { get; private set; }
    
    public int? UserId { get; private set; }

    public AuditActionType Type { get; private set; } 
    
    public string EntityName { get; private set; }

    public string EntityId { get; private set; }
    
    public string? OldValues { get; private set; }
    
    public string Values { get; private set; }

    public string IpAddress { get; private set; }

    public string UserAgent { get; private set; }
    
    public DeviceInfo? DeviceInfo { get; private set; }

    public DateTime TimeStamp { get; private set; }
    
    public string ActionBy { get; private set; }
    
    public string ActionByRoles { get; private set; }
    
    public string AffectedColumns { get; private set; }
    
    public User? User { get; private set; }
}