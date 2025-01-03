using System.Numerics;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client.Builders;

internal class CylZoneBuilder : ZoneBuilder<CylZone>
{
    public CylZoneBuilder(string name, Vector3 center) : base(new CylZone(name, center, 2))
    {
        SelectedPoint = 0;
        Selected = SelectedType.All;
    }

    protected override void _transform(double xd, double yd)
    {
        float x = (float) xd * Snaps[Snap];
        float y = (float) yd * Snaps[Snap];
        
        switch (Selected)
        {
            case SelectedType.All:
                Zone.Move(new Vector3(x, y,0));
                break;

            case SelectedType.Top:
                if (Zone.Height + y < 0.005) return;
                Zone.Height += y;
                break;
            
            case SelectedType.Bottom:
                Zone.Z += y;
                Zone.Radius += x;
                break;
        }
    }

    protected override void _selectNext()
    {
        
    }

    protected override void _selectPrev()
    {
        
    }

    protected override void _selectMode()
    {
        Selected++;
        if (Selected == SelectedType.Point) Selected++;
        if (Selected == SelectedType.Wall) Selected++;
        if ((int) Selected >= Enum.GetValues(typeof(SelectedType)).Length)
        {
            Selected = 0;
        }
    }

    protected override void _draw()
    {
        Zone.Render(new Rgba(0,255,0,127));
        DrawUtils.DrawText3d(Zone.Center + new Vector3(0,0, Zone.Height / 2), $"Mode ~b~[{Selected}] ~w~Snap ~g~[{Snaps[Snap]}]");
    }

    protected override bool _onKeyDown(Key key) => false;

    protected override void _onDispose()
    {
        Zone.CancelRender();
    }
}