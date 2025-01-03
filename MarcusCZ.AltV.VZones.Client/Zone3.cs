using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Data;
using MarcusCZ.AltV.VZones.Client.Adapters;
using MarcusCZ.AltV.VZones.Client.Render;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public class Zone3 : SharedZone3, IZoneRenderableTick, IEnterableZone
{
    [JsonIgnore]
    public ClientZoneDelegate? OnEnter { get; set; }
    [JsonIgnore]
    public ClientZoneDelegate? OnLeave { get; set; }

    private static readonly Zone3Adapter Adapter = new();
    public override IMValueBaseAdapter GetAdapter()
    {
        return Adapter;
    }

    public Zone3()
    {
    }
    
    public Zone3(string name, Vector3 center) : base(name, center)
    {
    }

    public Zone3(string name, float z, float height, List<Vector2> points) : base(name, z, height, points)
    {
    }

    public void Render(Rgba color)
    {
        Zone3Render.RenderZone(this, color);
    }

    public void CancelRender()
    {
        
    }
}