using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client.Render;

public class Zone3Render : IZoneRender
{
    private Rgba _color3 = new (0, 255, 0, 127);
    
    public void RenderZone(IZone z)
    {
        if (z is not Zone3 zone) return;
        
        float z2 = zone.Z + zone.Height;
        for (int i = 0; i < zone.Points.Count; i++)
        {
            Vector2 p1 = zone.Points[i];
            Vector2 p2 = zone.Points[(i + 1) % zone.Points.Count];
            Alt.Natives.DrawPoly(p1.X, p1.Y, zone.Z, p1.X, p1.Y, z2, p2.X, p2.Y, zone.Z, _color3.R, _color3.G, _color3.B, _color3.A);
            Alt.Natives.DrawPoly(p1.X, p1.Y, z2, p2.X, p2.Y, z2, p2.X, p2.Y, zone.Z, _color3.R, _color3.G, _color3.B, _color3.A); 
            Alt.Natives.DrawPoly(p2.X, p2.Y, zone.Z, p2.X, p2.Y, z2, p1.X, p1.Y, z2, _color3.R, _color3.G, _color3.B, _color3.A);
            Alt.Natives.DrawPoly(p2.X, p2.Y, zone.Z, p1.X, p1.Y, z2, p1.X, p1.Y, zone.Z, _color3.R, _color3.G, _color3.B, _color3.A);
        }
    }
}