using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Data;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

internal class DebugZoneRender
{
    private Vector3 _lastPosition = Vector3.Zero;
    public bool Active = false;
    private List<IZone>? _zones;
    private List<IZoneRenderable> _active = new();

    private readonly Dictionary<Type, Rgba> _colors = new()
    {
        {typeof(Zone2), new (255, 0, 0, 127)},
        {typeof(Zone3), new (0, 255, 0, 127)},
        {typeof(CylZone), new Rgba(0, 0, 255, 127)}
    };
    
    public void OnTick()
    {
        if (Vector3.Distance(_lastPosition, Alt.LocalPlayer.Position) > 1 || _zones == null)
        {
            _zones = VZonesClient.Zones.GetClosestZonesAtRange(10, 50);
            _lastPosition = Alt.LocalPlayer.Position;

            IEnumerable<IZoneRenderable> inactive = _active.Where(z => !_zones.Contains((IZone) z)).ToArray();
            foreach (var zoneRenderable in inactive)
            {
                zoneRenderable.CancelRender();
                _active.Remove(zoneRenderable);
            }
        }

        foreach (var zone in _zones)
        {
            string type = "[" + zone.GetType().Name + "]";
            DrawUtils.DrawText3d(zone.Center, type + " " + zone.Name);
            if (zone is IZoneRenderableTick renderableTick) renderableTick.Render(_colors[zone.GetType()]); 
            if (zone is IZoneRenderable renderable && !renderable.IsRendering())
            {
                renderable.Render(_colors[zone.GetType()]);
                _active.Add(renderable);
            }
        }
    }

    public void StopRender()
    {
        if (_active.Count == 0) return;
        
        foreach (var zone in _active)
        {
            zone.CancelRender();
        }
    }
}