using System.Numerics;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public interface ICrossableZone : IActiveZone
{
    public ServerZoneDelegate? OnCross { get; }
    public float GetSignedDistance(Vector3 point);
}