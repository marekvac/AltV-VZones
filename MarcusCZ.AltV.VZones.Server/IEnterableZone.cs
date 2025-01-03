using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public interface IEnterableZone : IActiveZone
{
    public ServerZoneDelegate? OnEnter { get; }
    public ServerZoneDelegate? OnLeave { get; }
}