using AltV.Net.Client;
using AltV.Net.Data;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client.Render;

public class Zone2Render
{
    public static void RenderZone(IZone z, Rgba color)
    {
        if (z is not Zone2 zone) return;
        
        float z2 = zone.Z + zone.Height;
        Alt.Natives.DrawPoly(zone.P1.X, zone.P1.Y, zone.Z, zone.P1.X, zone.P1.Y, z2, zone.P2.X, zone.P2.Y, zone.Z, color.R, color.G, color.B, color.A);
        Alt.Natives.DrawPoly(zone.P1.X, zone.P1.Y, z2, zone.P2.X, zone.P2.Y, z2, zone.P2.X, zone.P2.Y, zone.Z, color.R, color.G, color.B, color.A); 
        Alt.Natives.DrawPoly(zone.P2.X, zone.P2.Y, zone.Z, zone.P2.X, zone.P2.Y, z2, zone.P1.X, zone.P1.Y, z2, color.R, color.G, color.B, color.A);
        Alt.Natives.DrawPoly(zone.P2.X, zone.P2.Y, zone.Z, zone.P1.X, zone.P1.Y, z2, zone.P1.X, zone.P1.Y, zone.Z, color.R, color.G, color.B, color.A);
    }
}