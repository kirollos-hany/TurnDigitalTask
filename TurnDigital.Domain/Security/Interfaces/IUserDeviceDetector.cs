using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Domain.Security.Interfaces;

public interface IUserDeviceDetector
{
    DeviceInfo? DetectDevice();

    string UserAgent();
}