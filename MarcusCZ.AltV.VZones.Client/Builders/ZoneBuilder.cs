using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using MarcusCZ.AltV.VZones.Client.Exporters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client.Builders;

internal abstract class ZoneBuilder<T> : IZoneBuilder where T : IZone
{
    protected static readonly float[] Snaps = {0.01f, 0.05f, 0.1f, 0.25f, 0.5f, 1, 2, 5, 10};
    protected T Zone;
    protected int SelectedPoint;
    protected SelectedType Selected = SelectedType.Point;
    protected int Snap;

    protected ZoneBuilder(T zone)
    {
        Zone = zone;
        Alt.OnTick += _onTick;
        Alt.OnKeyDown += OnKeyDown;
    }

    protected abstract void _transform(double xd, double yd);
    protected abstract void _selectNext();
    protected abstract void _selectPrev();
    protected abstract void _selectMode();
    protected abstract void _draw();
    protected abstract bool _onKeyDown(Key key);

    protected abstract void _onDispose();
    
    private void _selectSnap()
    {
        Snap++;
        if (Snap >= Snaps.Length) Snap = 0;
    }

    public void Export(IZoneExporter exporter, string[] args)
    {
        exporter.ExportZone(Zone, args);
    }

    public void Dispose()
    {
        Alt.OnTick -= _onTick;
        Alt.OnKeyDown -= OnKeyDown;
        _onDispose();
    }

    private static void _disableControls()
    {
        Alt.Natives.DisableControlAction(0, 212, true);
        Alt.Natives.DisableControlAction(0, 213, true);
    }
    
    private void _onTick()
    {
        _disableControls();
        _draw();
    }
    
    private async void OnKeyDown(Key key)
    {
        if (_onKeyDown(key)) return;
        
        Vector3 rot = Alt.Natives.GetGameplayCamRot(0);
        double zRad = rot.Z * (Math.PI / 180);
        switch (key)
        {
            case Key.Up:
                while (Alt.IsKeyDown(Key.Up))
                {
                    if (Selected == SelectedType.Bottom || Selected == SelectedType.Top) _transform(0,1);
                    else _transform(-Math.Sin(zRad),Math.Cos(zRad));
                    await Task.Delay(50);
                }
                break;
            
            case Key.Down:
                while (Alt.IsKeyDown(Key.Down))
                {
                    if (Selected == SelectedType.Bottom || Selected == SelectedType.Top) _transform(0,-1);
                    else _transform(Math.Sin(zRad),-Math.Cos(zRad));
                    await Task.Delay(50);
                }
                break;
            
            case Key.Right:
                while (Alt.IsKeyDown(Key.Right))
                {
                    if (Selected == SelectedType.Bottom || Selected == SelectedType.Top) _transform(1,0);
                    else _transform(Math.Cos(zRad), Math.Sin(zRad));
                    await Task.Delay(50);
                }
                break;
            
            case Key.Left:
                while (Alt.IsKeyDown(Key.Left))
                {
                    if (Selected == SelectedType.Bottom || Selected == SelectedType.Top) _transform(-1,0);
                    else _transform(-Math.Cos(zRad),-Math.Sin(zRad));
                    await Task.Delay(50);
                }
                break;
            
            case Key.PageUp:
                _selectNext();
                break;
            case Key.PageDown:
                _selectPrev();
                break;
            case Key.Home:
                _selectMode();
                break;
            case Key.End:
                _selectSnap();
                break;
        }
    }

    protected enum SelectedType
    {
        Point = 0,
        Wall = 1,
        Bottom = 2,
        Top = 3,
        All = 4
    }
}