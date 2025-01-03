using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Client;
using AltV.Net.Data;
using AltV.Net.Shared.Elements.Entities;
using MarcusCZ.AltV.VZones.Shared.Util;
using MarcusCZ.AltV.VZones.Client.Adapters;
using MarcusCZ.AltV.VZones.Client.Render;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public class Zone2 : SharedZone2, ICrossableZone, IZoneRenderableTick
{
    [JsonIgnore]
    public ISharedBlip? Blip { get; set; }
    [JsonIgnore]
    public ClientZoneDelegate? OnCross { get; set; }

    private static readonly Zone2Adapter Adapter = new ();
    public override IMValueBaseAdapter GetAdapter()
    {
        return Adapter;
    }

    public Zone2(string name, Vector3 center) : base(name, center)
    {
    }

    public Zone2(string name, Vector2 p1, Vector2 p2, float z, float height) : base(name, p1, p2, z, height)
    {
    }
    
    public void Move(Vector3 direction)
    {
        Vector2 dir2d = VectorUtils.AsVector2(direction);
        Z += direction.Z;
        P1 += dir2d;
        P2 += dir2d;
    }

    public void Rotate(float angle)
    {
        var angleInRadians = angle * Math.PI / 180;
        var center = GetCenter();
        float cos = (float) Math.Cos(angleInRadians);
        float sin = (float) Math.Sin(angleInRadians);
        P1 = _rotatePoint(P1, center, cos, sin);
        P2 = _rotatePoint(P2, center, cos, sin);
    }

    private Vector2 _rotatePoint(Vector2 point, Vector3 center, float cos, float sin)
    {
        var x = point.X - center.X;
        var y = point.Y - center.Y;
        return new Vector2(
            x * cos - y * sin + center.X,
            x * sin + y * cos + center.Y
        );
    }
    
    public static Zone2 operator +(Zone2 zone, Vector3 direction)
    {
        zone.Move(direction);
        return zone;
    }
    
    

    public void Render(Rgba color)
    {
        Zone2Render.RenderZone(this, color);
    }

    public void CancelRender()
    {
        
    }

    [JsonIgnore]
    public BlipTypeDelegate GetBlip { get; set; } = zone =>
    {
        return Alt.CreatePointBlip(zone.GetCenter());
    };
    
    public void CreateBlip()
    {
        Blip = GetBlip(this);
    }

    public void DestroyBlip()
    {
        Blip?.Destroy();
        Blip = null;
    }
}