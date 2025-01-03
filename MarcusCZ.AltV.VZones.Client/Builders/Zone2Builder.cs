using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client.Builders;

internal class Zone2Builder : ZoneBuilder<Zone2>
{
    public Zone2Builder(string name, Vector3 center) : base(new Zone2(name, center))
    {
        SelectedPoint = 0;
    }

    protected override void _transform(double xd, double yd)
    {
        float x = (float) xd * Snaps[Snap];
        float y = (float) yd * Snaps[Snap];
        
        Vector2 direction2d = new Vector2(x, y);
        switch (Selected)
        {
            case SelectedType.All:
                Zone += new Vector3(x, y,0);
                break;

            case SelectedType.Point:
                if (SelectedPoint == 0) Zone.P1 += direction2d;
                else Zone.P2 += direction2d;
                break;
            
            case SelectedType.Top:
                if (Zone.Height + y < 0.005) return;
                Zone.Height += y;
                break;
            
            case SelectedType.Bottom:
                Zone.Z += y;
                Zone.Rotate(x);
                break;
        }

        Zone.ReloadCenter();
    }

    protected override void _selectNext()
    {
        if (Selected != SelectedType.Point) return;
        SelectedPoint = (SelectedPoint + 1) % 2;
    }

    protected override void _selectPrev() => _selectNext();

    protected override void _selectMode()
    {
        Selected++;
        if (Selected == SelectedType.Wall) Selected++;
        if ((int) Selected >= Enum.GetValues(typeof(SelectedType)).Length)
        {
            Selected = 0;
        }
    }

    protected override void _draw()
    {
        Rgba red = new Rgba(255, 0, 0, 255);
        Rgba blue = new Rgba(0, 0, 255, 255);
        Rgba color = new Rgba(0, 255, 0, 127);
        float z2 = Zone.Z + Zone.Height;

        if (Selected == SelectedType.All) color = new Rgba(0,0,255,127);

        Rgba lineColor = Selected == SelectedType.Point && SelectedPoint == 0 ? blue : red;
        Alt.Natives.DrawLine(Zone.P1.X, Zone.P1.Y, Zone.Z, Zone.P1.X, Zone.P1.Y, z2, lineColor.R, lineColor.G, lineColor.B, lineColor.A);
        lineColor = SelectedPoint == 1 ? blue : red;
        Alt.Natives.DrawLine(Zone.P2.X, Zone.P2.Y, Zone.Z, Zone.P2.X, Zone.P2.Y, z2, lineColor.R, lineColor.G, lineColor.B, lineColor.A);
        
        lineColor = Selected == SelectedType.Bottom ? blue : red;
        Alt.Natives.DrawLine(Zone.P1.X, Zone.P1.Y, Zone.Z, Zone.P2.X, Zone.P2.Y, Zone.Z, lineColor.R, lineColor.G, lineColor.B, lineColor.A); // bottom
        lineColor = Selected == SelectedType.Top ? blue : red;
        Alt.Natives.DrawLine(Zone.P1.X, Zone.P1.Y, z2, Zone.P2.X, Zone.P2.Y, z2, lineColor.R, lineColor.G, lineColor.B, lineColor.A); // bottom
        
        Alt.Natives.DrawPoly(Zone.P1.X, Zone.P1.Y, Zone.Z, Zone.P1.X, Zone.P1.Y, z2, Zone.P2.X, Zone.P2.Y, Zone.Z, color.R, color.G, color.B, color.A);
        Alt.Natives.DrawPoly(Zone.P1.X, Zone.P1.Y, z2, Zone.P2.X, Zone.P2.Y, z2, Zone.P2.X, Zone.P2.Y, Zone.Z, color.R, color.G, color.B, color.A); 
        Alt.Natives.DrawPoly(Zone.P2.X, Zone.P2.Y, Zone.Z, Zone.P2.X, Zone.P2.Y, z2, Zone.P1.X, Zone.P1.Y, z2, color.R, color.G, color.B, color.A);
        Alt.Natives.DrawPoly(Zone.P2.X, Zone.P2.Y, Zone.Z, Zone.P1.X, Zone.P1.Y, z2, Zone.P1.X, Zone.P1.Y, Zone.Z, color.R, color.G, color.B, color.A);

        // DrawUtils.DrawText2d($"Current snap: ~b~{Snaps[Snap]}");
        DrawUtils.DrawText3d(Zone.Center, $"Mode ~b~[{Selected}] ~w~Snap ~g~[{Snaps[Snap]}]");
        
        if (Selected != SelectedType.Point) return;
        
        if (SelectedPoint == 0) Alt.Natives.DrawMarkerSphere(Zone.P1.X, Zone.P1.Y, Zone.Z, 0.2f, 0, 0, 255, 127);
        else Alt.Natives.DrawMarkerSphere(Zone.P2.X, Zone.P2.Y, Zone.Z, 0.2f, 0, 0, 255, 127);
    }

    protected override bool _onKeyDown(Key key) => false;

    protected override void _onDispose()
    {
        
    }
}