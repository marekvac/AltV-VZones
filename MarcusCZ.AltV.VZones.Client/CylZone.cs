using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using MarcusCZ.AltV.VZones.Client.Adapters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public class CylZone : SharedCylZone, IZoneRenderable, IEnterableZone
{
    [JsonIgnore]
    public ClientZoneDelegate? OnEnter { get; set; }
    [JsonIgnore]
    public ClientZoneDelegate? OnLeave { get; set; }

    private static readonly CylZoneAdapter Adapter = new ();
    public override IMValueBaseAdapter GetAdapter()
    {
        return Adapter;
    }

    public CylZone()
    {
    }

    public CylZone(string name, Vector3 center, float radius, float height = 5) : base(name, center, radius, height)
    {
    }
    
    private IMarker? _marker;
    public void Render(Rgba color)
    {
        if (_marker == null)
        {
            _marker = Alt.CreateMarker(MarkerType, Center, color, true, StreamingDistance);
        }

        _marker.Position = Center;
        _marker.Scale = new Position(Radius*2, Radius*2, Height);
    }

    public void CancelRender()
    {
        if (_marker != null)
        {
            _marker.Destroy();
            _marker = null;
        }
    }

    public bool IsRendering()
    {
        return _marker != null;
    }
}