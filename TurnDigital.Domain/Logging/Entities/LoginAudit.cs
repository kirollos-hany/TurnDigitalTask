using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Logging.Enums;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Domain.Logging.Entities;

public class LoginAudit : IEntity
{
    private LoginAudit()
    {
        
    }

    public LoginAudit(User user, string userDisplayName, string ipAddress, string userAgent, LoginStatus status, DeviceInfo? deviceInfo)
    {
        User = user;

        UserDisplayName = userDisplayName;

        IpAddress = ipAddress;

        UserAgent = userAgent;

        Status = status;

        DeviceInfo = deviceInfo;

        TimeStamp = DateTime.UtcNow;
    }
    
    public int Id { get; private set; }
    
    public int UserId { get; private set; }
    
    public string UserDisplayName { get; private set; }
    
    public string IpAddress { get; private set; }
    
    public string UserAgent { get; private set; }
    
    public LoginStatus Status { get; private set; }
    
    public DeviceInfo? DeviceInfo { get; private set; }
    
    public DateTime TimeStamp { get; private set; }
    
    public User? User { get; private set; }
}