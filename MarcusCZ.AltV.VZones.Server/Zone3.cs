using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Elements.Entities;
using MarcusCZ.AltV.VZones.Server.Adapters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public class Zone3 : SharedZone3, IEnterableZone
{
    [JsonIgnore]
    public ServerZoneDelegate? OnEnter { get; set; }
    [JsonIgnore]
    public ServerZoneDelegate? OnLeave { get; set; }

    public Zone3()
    {
    }

    public Zone3(string name, Vector3 center) : base(name, center)
    {
        Active = true;
    }

    public Zone3(string name, float z, float height, List<Vector2> points) : base(name, z, height, points)
    {
        Active = true;
    }

    private static readonly Zone3Adapter Adapter = new();
    public override IMValueBaseAdapter GetAdapter()
    {
        return Adapter;
    }

    public bool IsInside(IPlayer player)
    {
        return IsInside(player.Position);
    }
    
    
}