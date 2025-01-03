using System.Numerics;
using AltV.Net.Client;

namespace MarcusCZ.AltV.VZones.Client;

public static class DrawUtils
{
    public static void DrawText2d(string text)
    {
        Alt.Natives.BeginTextCommandDisplayText("STRING");
        Alt.Natives.AddTextComponentSubstringPlayerName(text);
        Alt.Natives.SetTextFont(0);
        Alt.Natives.SetTextScale(1,0.5f);
        Alt.Natives.SetTextWrap(0,1);
        Alt.Natives.SetTextCentre(true);
        Alt.Natives.SetTextColour(255,255,255,255);
        Alt.Natives.SetTextJustification(0);
        Alt.Natives.SetTextOutline();
        Alt.Natives.SetTextDropShadow();
        Alt.Natives.EndTextCommandDisplayText(0.5f, 0.955f, 0);
    }
    
    public static void DrawText3d(Vector3 pos, string text)
    {
        Alt.Natives.SetDrawOrigin(pos.X, pos.Y, pos.Z, false);
        Alt.Natives.BeginTextCommandDisplayText("STRING");
        Alt.Natives.AddTextComponentSubstringPlayerName(text);
        Alt.Natives.SetTextFont(0);
        Alt.Natives.SetTextScale(1,0.3f);
        Alt.Natives.SetTextWrap(0.0f,1.0f);
        Alt.Natives.SetTextCentre(true);
        Alt.Natives.SetTextColour(255,255,255,255);
        Alt.Natives.SetTextOutline();
        Alt.Natives.EndTextCommandDisplayText(0,0,0);
        Alt.Natives.ClearDrawOrigin();
    }
}