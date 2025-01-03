using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Elements.Entities;
using MarcusCZ.AltV.VZones.Server.Adapters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public class CylZone : SharedCylZone, IEnterableZone
{
    [JsonIgnore]
    public ServerZoneDelegate? OnEnter { get; set; }
    [JsonIgnore]
    public ServerZoneDelegate? OnLeave { get; set; }

    public CylZone()
    {
    }

    public CylZone(string name, Vector3 center, float radius, float height = 5) : base(name, center, radius, height)
    {
    }

    private static readonly CylZoneAdapter Adapter = new();
    public override IMValueBaseAdapter GetAdapter()
    {
        return Adapter;
    }
    
    public bool IsInside(IPlayer player)
    {
        return IsInside(player.Position);
    }
}