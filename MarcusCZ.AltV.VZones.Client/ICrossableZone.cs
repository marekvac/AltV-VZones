using System.Numerics;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public interface ICrossableZone : IActiveZone
{
    public ClientZoneDelegate? OnCross { get; }
    public float GetSignedDistance(Vector3 point);
}