using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public interface IEnterableZone : IActiveZone
{
    public ClientZoneDelegate? OnEnter { get; }
    public ClientZoneDelegate? OnLeave { get; }
}