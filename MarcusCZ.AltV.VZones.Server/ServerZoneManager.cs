using System.Collections.Immutable;
using System.Numerics;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public class ServerZoneManager
{
    private readonly Dictionary<string, IActiveZone> _zones = new();
    private readonly Dictionary<IPlayer, List<IActiveZone>> _zonesByPlayerOrdered = new();
    private readonly Dictionary<IPlayer, List<IActiveZone>?> _zonesByPlayer = new();
    private readonly Dictionary<IPlayer, List<IEnterableZone>> _currentZones = new();
    private readonly Dictionary<IPlayer, Position> _lastPositions = new();
    private readonly List<IPlayer> _globalChecking = new();

    public ServerZoneManager(bool globalCheckingEnabled)
    {
        if (globalCheckingEnabled)
        {
            Alt.OnPlayerConnect += (player, _) => 
            {
                if (_globalChecking.Contains(player)) return;
                _globalChecking.Add(player);
            };
            Alt.OnPlayerDisconnect += (player, _) => _globalChecking.Remove(player);
        }
    }

    public void RegisterZone(IActiveZone zone)
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
            if(Alt.IsDebug) Alt.LogColored($"~lm~[PolyZone]~lw~ Registered zone {zone.Name}");
        }
    }

    public void UnregisterZone(string name)
    {
        lock (_zones)
        {
            if (!_zones.Remove(name)) Alt.LogWarning($"Zone with name {name} is not registered!");
        }
    }
    
    public void UnregisterZone(IActiveZone zone)
    {
        lock (_zones)
        {
            if (!_zones.Remove(zone.Name)) Alt.LogWarning($"Zone with name {zone.Name} is not registered!");
        }
    }

    public IZone? GetZone(string name)
    {
        lock (_zones)
        {
            if (_zones.TryGetValue(name, out var zone)) return zone;
        }

        return null;
    }

    public void EnableGlobalChecking(IPlayer player)
    {
        lock (_globalChecking) _globalChecking.Add(player);
    }

    public void DisableGlobalChecking(IPlayer player)
    {
        lock (_globalChecking) _globalChecking.Remove(player);
    }

    public void SetZonesForPlayer(IPlayer player, List<IActiveZone>? zones)
    {
        lock (_zonesByPlayer) _zonesByPlayer[player] = zones;
    }
    
    public void AddZoneForPlayer(IPlayer player, IActiveZone zone)
    {
        lock (_zonesByPlayer)
        {
            if (!_zonesByPlayer.ContainsKey(player)) _zonesByPlayer[player] = new List<IActiveZone>();
            _zonesByPlayer[player]!.Add(zone);
        }
    }
    
    public void RemoveZoneForPlayer(IPlayer player, IActiveZone zone)
    {
        lock (_zonesByPlayer)
        {
            if (!_zonesByPlayer.ContainsKey(player)) return;
            _zonesByPlayer[player]!.Remove(zone);
            if (_zonesByPlayer[player]!.Count == 0) _zonesByPlayer.Remove(player);
        }
    }
    
    public void ClearZonesForPlayer(IPlayer player)
    {
        lock (_zonesByPlayer) _zonesByPlayer.Remove(player);
    }
    
    public static ServerZoneManager operator +(ServerZoneManager zoneManager, IActiveZone zone)
    {
        zoneManager.RegisterZone(zone);
        return zoneManager;
    }

    public static ServerZoneManager operator -(ServerZoneManager zoneManager, string name)
    {
        zoneManager.UnregisterZone(name);
        return zoneManager;
    }

    internal async Task DoChecking(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Alt.GetAllPlayers().Count == 0)
            {
                await Task.Delay(1000, token);
                continue;
            }
            
            foreach (var player in Alt.GetAllPlayers())
            {
                lock (_globalChecking)
                {
                    lock (_zonesByPlayer)
                    {
                        if (!_globalChecking.Contains(player) && (!_zonesByPlayer.ContainsKey(player) || _zonesByPlayer[player] == null))
                            continue;
                    }
                }
                if (!_lastPositions.ContainsKey(player)) _lastPositions.Add(player, Position.Zero);
                if (!_currentZones.ContainsKey(player)) _currentZones.Add(player, new List<IEnterableZone>());
                
                Position position = player.Position;
                if (player.IsInVehicle && player.Vehicle != null) position = player.Vehicle.Position;

                if (Vector3.Distance(position, _lastPositions[player]) > 1 || !_zonesByPlayerOrdered.ContainsKey(player))
                {
                    lock (_globalChecking)
                    {
                        if (_globalChecking.Contains(player))
                        {
                            lock (_zones)
                            {
                                _zonesByPlayerOrdered[player] = _zones.OrderBy(pair => Vector3.Distance(pair.Value.Center, position)).Select(pair => pair.Value).Take(5).ToList();
                            }
                        }
                        else
                        {
                            lock (_zonesByPlayer)
                            {
                                _zonesByPlayerOrdered[player] = _zonesByPlayer[player]!.OrderBy(zone => Vector3.Distance(zone.Center, position)).Take(5).ToList();
                            }
                        }
                    }
                }

                var left = _currentZones[player].Where(z => !z.IsInside(position)).ToArray();
                foreach (var z in left)
                {
                    z.OnLeave?.Invoke(player, z);
                    _currentZones[player].Remove(z);
                }
                
                var newZones = _zonesByPlayerOrdered[player].Where(z => z.IsInside(position)).ToArray();
                var newEnterableZones = newZones.OfType<IEnterableZone>().ToArray();
                foreach (var zone in newEnterableZones.Where(z => !_currentZones[player].Contains(z)))
                {
                    _currentZones[player].Add(zone);
                    zone.OnEnter?.Invoke(player, zone);
                }
                
                var crossableZones = newZones.OfType<ICrossableZone>().ToArray();
                foreach (var zone in crossableZones)
                {
                    float lastDistance = zone.GetSignedDistance(_lastPositions[player]);
                    float currentDistance = zone.GetSignedDistance(position);
                    if (lastDistance * currentDistance < 0)
                    {
                        zone.OnCross?.Invoke(player, zone);
                    }
                }
                
                _lastPositions[player] = position;
            }
            
            await Task.Delay(50, token);
        }
    }
}