using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client.Builders;

internal class Zone3Builder : ZoneBuilder<Zone3>
{
    public Zone3Builder(string name, Vector3 center) : base(new Zone3(name, center))
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
                Zone.Move(new Vector3(x, y, 0));
                break;
            
            case SelectedType.Wall:
                Zone.MoveWall(SelectedPoint, direction2d);
                break;
            
            case SelectedType.Point:
                Zone.MovePoint(SelectedPoint, direction2d);
                break;
            
            case SelectedType.Top:
                Zone.MoveTop(y);
                break;
            
            case SelectedType.Bottom:
                Zone.MoveBottom(y);
                Zone.Rotate(x);
                break;
        }
        
        Zone.ReloadCenter();
    }

    protected override void _selectNext()
    {
        if (Selected != SelectedType.Point && Selected != SelectedType.Wall) return;
        SelectedPoint = (SelectedPoint + 1) % Zone.Points.Count;
    }

    protected override void _selectPrev()
    {
        if (Selected != SelectedType.Point && Selected != SelectedType.Wall) return;
        SelectedPoint--;
        if (SelectedPoint < 0) SelectedPoint = Zone.Points.Count - 1;
    }

    protected override void _selectMode()
    {
        Selected++;
        if ((int) Selected >= Enum.GetValues(typeof(SelectedType)).Length)
        {
            Selected = 0;
        }
    }


    protected override bool _onKeyDown(Key key)
    {
        switch (key)
        {
            case Key.Insert:
                _addPoint();
                return true;
            case Key.Delete:
                _deletePoint();
                return true;
            default:
                return false;
        }
    }
    
    private void _addPoint()
    {
        if (Selected != SelectedType.Point) return;
        
        Vector2 newPoint = new Vector2(Zone.Points[SelectedPoint].X, Zone.Points[SelectedPoint].Y);
        Zone.Points.Insert(SelectedPoint, newPoint);
    }

    private void _deletePoint()
    {
        if (Selected != SelectedType.Point) return;
        
        if (Zone.Points.Count <= 3) return;
        Zone.Points.RemoveAt(SelectedPoint);
    }
    protected override void _draw()
    {
        Rgba red = new Rgba(255, 0, 0, 255);
        Rgba blue = new Rgba(0, 0, 255, 255);
        for (int i = 0; i < Zone.Points.Count; i++)
        {
            Rgba color = new Rgba(0, 255, 0, 127);
           
            if (Selected == SelectedType.All || (Selected == SelectedType.Wall && i == SelectedPoint)) color = new Rgba(0, 0, 255, 190);

            Vector2 p1 = Zone.Points[i];
            Vector2 p2 = Zone.Points[(i + 1) % Zone.Points.Count];
            float z2 = Zone.Z + Zone.Height;
            
            // Draw edges
            Rgba lineColor = Selected == SelectedType.Point && SelectedPoint == i ? blue : red;
            Alt.Natives.DrawLine(p1.X, p1.Y, Zone.Z, p1.X, p1.Y, z2, lineColor.R, lineColor.G, lineColor.B, lineColor.A); // vertical

            lineColor = Selected == SelectedType.Bottom || Selected == SelectedType.Wall && SelectedPoint == i ? blue : red;
            Alt.Natives.DrawLine(p1.X, p1.Y, Zone.Z, p2.X, p2.Y, Zone.Z, lineColor.R, lineColor.G, lineColor.B, lineColor.A); // bottom
            
            lineColor = Selected == SelectedType.Top || Selected == SelectedType.Wall && SelectedPoint == i ? blue : red;
            Alt.Natives.DrawLine(p1.X, p1.Y, z2, p2.X, p2.Y, z2, lineColor.R, lineColor.G, lineColor.B, lineColor.A); // top
            
            // Draw wall
            Alt.Natives.DrawPoly(p1.X, p1.Y, Zone.Z, p1.X, p1.Y, z2, p2.X, p2.Y, Zone.Z, color.R, color.G, color.B, color.A);
            Alt.Natives.DrawPoly(p1.X, p1.Y, z2, p2.X, p2.Y, z2, p2.X, p2.Y, Zone.Z, color.R, color.G, color.B, color.A); 
            Alt.Natives.DrawPoly(p2.X, p2.Y, Zone.Z, p2.X, p2.Y, z2, p1.X, p1.Y, z2, color.R, color.G, color.B, color.A);
            Alt.Natives.DrawPoly(p2.X, p2.Y, Zone.Z, p1.X, p1.Y, z2, p1.X, p1.Y, Zone.Z, color.R, color.G, color.B, color.A);
            
            // Vector3 center = _zone.GetCenter();
            if (Selected == SelectedType.Point && i == SelectedPoint) Alt.Natives.DrawMarkerSphere(p1.X, p1.Y, Zone.Z, 0.2f, 0, 0, 255, 127);
            // else Alt.Natives.DrawMarkerSphere(center.X, center.Y, center.Z, 0.1f, 255, 0, 0, 127);
            
            // DrawUtils.DrawText2d($"Current snap: ~b~{Snaps[Snap]}");
            DrawUtils.DrawText3d(Zone.Center, $"Mode ~b~[{Selected}] ~w~Snap ~g~[{Snaps[Snap]}]");
        }    
    }

    protected override void _onDispose()
    {
        
    }
}