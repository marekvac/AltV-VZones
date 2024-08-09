using AltV.Net.Client;
using AltV.Net.Data;
using MarcusCZ.AltV.VZones.Client;
using MarcusCZ.AltV.VZones.Client.Render;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public class Zone2Render : IZoneRender
{
    private Rgba _color2 = new (255, 0, 0, 127);
    
    public void RenderZone(IZone z)
    {
        if (z is not Zone2 zone) return;
        
        float z2 = zone.Z + zone.Height;
        Alt.Natives.DrawPoly(zone.P1.X, zone.P1.Y, zone.Z, zone.P1.X, zone.P1.Y, z2, zone.P2.X, zone.P2.Y, zone.Z, _color2.R, _color2.G, _color2.B, _color2.A);
        Alt.Natives.DrawPoly(zone.P1.X, zone.P1.Y, z2, zone.P2.X, zone.P2.Y, z2, zone.P2.X, zone.P2.Y, zone.Z, _color2.R, _color2.G, _color2.B, _color2.A); 
        Alt.Natives.DrawPoly(zone.P2.X, zone.P2.Y, zone.Z, zone.P2.X, zone.P2.Y, z2, zone.P1.X, zone.P1.Y, z2, _color2.R, _color2.G, _color2.B, _color2.A);
        Alt.Natives.DrawPoly(zone.P2.X, zone.P2.Y, zone.Z, zone.P1.X, zone.P1.Y, z2, zone.P1.X, zone.P1.Y, zone.Z, _color2.R, _color2.G, _color2.B, _color2.A);
    }
}