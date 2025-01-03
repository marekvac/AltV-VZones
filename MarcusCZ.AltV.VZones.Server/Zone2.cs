using System.Numerics;
using AltV.Net;
using MarcusCZ.AltV.VZones.Server.Adapters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public class Zone2 : SharedZone2, ICrossableZone
{
    public ServerZoneDelegate? OnCross { get; set; }
    
    public Zone2(string name, Vector3 center) : base(name, center)
    {
    }

    public Zone2(string name, Vector2 p1, Vector2 p2, float z, float height) : base(name, p1, p2, z, height)
    {
    }

    private static readonly Zone2Adapter Adapter = new();
    public override IMValueBaseAdapter GetAdapter() => Adapter;
}