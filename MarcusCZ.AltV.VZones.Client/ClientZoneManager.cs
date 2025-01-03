using System.Numerics;
using AltV.Net.Client;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public class ClientZoneManager
{
    private Dictionary<string, IZone> _zones;
    private List<IEnterableZone> _currentZones = new();
    private Vector3 _lastPosition = Vector3.Zero;

    public ClientZoneManager()
    {
        _zones = new Dictionary<string, IZone>();
    }

    public void RegisterZone(IZone zone)
    {
        if (zone.GetType() == typeof(Zone3) && ((Zone3) zone).Points.Count < 3)
        {
            Alt.LogError($"Zone3 {zone.Name} has less than 3 defined points! Skipping...");
            return;
        }
        
        lock (_zones)
        {
            if (_zones.ContainsKey(zone.Name))
            {
                Alt.LogWarning($"Zone with name {zone.Name} is already registered! Skipping...");
                return;
            }
            _zones.Add(zone.Name, zone);
            if (Alt.IsDebug) Alt.Log($"[PolyZone] Registered zone {zone.Name}");
        }
    }

    public void UnregisterZone(string name)
    {
        lock (_zones)
        {
            if (!_zones.Remove(name)) Alt.LogWarning($"Zone with name {name} is not registered!");
        }
    }

    public static ClientZoneManager operator +(ClientZoneManager clientZoneManager, IZone zone)
    {
        clientZoneManager.RegisterZone(zone);
        return clientZoneManager;
    }

    public static ClientZoneManager operator -(ClientZoneManager clientZoneManager, string name)
    {
        clientZoneManager.UnregisterZone(name);
        return clientZoneManager;
    }

    public IZone? GetZone(string name)
    {
        lock (_zones)
        {
            if (_zones.ContainsKey(name)) return _zones[name];
        }
        return null;
    }

    public T? GetZone<T>(string name) where T : IZone
    {
        lock (_zones)
        {
            if (_zones.ContainsKey(name) && _zones[name].GetType() == typeof(T)) return (T) _zones[name];
        }

        return default;
    }

    public IEnterableZone? GetZone3At(Vector3 location)
    {
        lock (_zones)
        {
            return _zones.Select(pair => pair.Value)
                .Where(zone => zone.GetType() == typeof(IEnterableZone))
                .Cast<IEnterableZone>()
                .OrderBy(zone => Vector3.Distance(zone.Center, location))
                .Take(3)
                .FirstOrDefault(zone => zone.IsInside(location));
        }
    }

    public List<IZone> GetClosestZonesAtRange(int n, float range)
    {
        lock (_zones)
        {
            return _zones.Values.Where(zone => Vector3.Distance(zone.Center, _lastPosition) <= range).Take(n).ToList();
        }
    }
    
    public List<IZone> GetClosestZones(int n)
    {
        lock (_zones)
        {
            return _zones.Values.Take(n).ToList();
        }
    }

    public List<T> GetClosestZones<T>(int n) where T : IZone
    {
        lock (_zones)
        {
            return _zones.Select(pair => pair.Value).Where(zone => typeof(T) == zone.GetType()).Take(n).Cast<T>().ToList();
        }
    }

    public List<IActiveZone> GetClosestActiveZones(int n)
    {
        lock (_zones)
        {
            return _zones.Select(pair => pair.Value).OfType<IActiveZone>().Where(zone => zone.Active).Take(n).ToList();
        }
    }

    internal async Task DoChecking(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int cnt;
            lock (_zones) cnt = _zones.Count;
            
            if (cnt == 0)
            {
                await Task.Delay(1000, token);
                continue;
            }
            
            Vector3 position = Alt.LocalPlayer.Position;
            if (Alt.LocalPlayer.IsInVehicle && Alt.LocalPlayer.Vehicle != null) position = Alt.LocalPlayer.Vehicle.Position;

            var distance = Vector3.Distance(_lastPosition, position);
            if (distance > 1)
            {
                lock (_zones)
                {
                    _zones = _zones.OrderBy(pair => Vector3.Distance(pair.Value.Center, _lastPosition)).ToDictionary(pair => pair.Key, pair => pair.Value);
                }
            }

            var left = _currentZones.Where(z => !z.IsInside(position)).ToArray();
            foreach (var zone in left)
            {
                zone.OnLeave?.Invoke(zone);
                _currentZones.Remove(zone);
            }
            
            // if (_currentZone != null && !_currentZone.IsInside(position))
            // {
            //     _currentZone.OnLeave?.Invoke(_currentZone);
            //     Alt.EmitServer("polyzone:leftZone", _currentZone.Name);
            //     _currentZone = null;
            // }

            var newZones = GetClosestActiveZones(10).Where(zone => zone.IsInside(position) && !_currentZones.Contains(zone)).ToArray();
            var newEnterableZones = newZones.OfType<IEnterableZone>().ToArray();
            foreach (var zone in newEnterableZones.Where(zone => !_currentZones.Contains(zone)))
            {
                zone.OnEnter?.Invoke(zone);
                _currentZones.Add(zone);
            }
            // if (newZone != null)
            // {
            //     if (_currentZone is not null)
            //     {
            //         _currentZone.OnLeave?.Invoke(_currentZone);
            //         Alt.EmitServer("polyzone:leftZone", _currentZone.Name);
            //     }
            //     _currentZone = newZone;
            //     _currentZone.OnEnter?.Invoke(_currentZone);
            //     Alt.EmitServer("polyzone:enteredZone", _currentZone.Name);
            // }

            var crossableZones = newZones.OfType<ICrossableZone>().ToArray();
            foreach (var zone in crossableZones)
            {
                float lastDistance = zone.GetSignedDistance(_lastPosition);
                float currentDistance = zone.GetSignedDistance(position);
                if (lastDistance * currentDistance < 0)
                {
                    zone.OnCross?.Invoke(zone);
                }
            }

            _lastPosition = position;
            

            await Task.Delay(100, token);
        }
    }
}