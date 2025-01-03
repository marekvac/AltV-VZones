using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Async;
using MarcusCZ.AltV.VZones.Client.Adapters;
using MarcusCZ.AltV.VZones.Client.Builders;
using MarcusCZ.AltV.VZones.Client.Exporters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;
public class VZonesClient : AsyncResource
{
    private IZoneBuilder? _builder;
    public static ClientZoneManager Zones;
    private DiscordZoneExporter? _exporter;
    private readonly DebugZoneRender _zoneRender = new();
    private readonly CancellationTokenSource _cts = new();

    public override void OnStart()
    {
        Zones = new ClientZoneManager();
        Task.Run(() => Zones.DoChecking(_cts.Token), _cts.Token);
        _exporter = new();
        
        Alt.OnConsoleCommand += (name, args) =>
        {
            if (name != "zones" || args.Length < 1) return;
            
            if (_builder == null)
            {
                if (args[0] == "zone2" && args.Length == 2)
                {
                    _builder = new Zone2Builder(args[1], Alt.LocalPlayer.Position);
                } else if (args[0] == "zone3" && args.Length == 2)
                {
                    _builder = new Zone3Builder(args[1], Alt.LocalPlayer.Position);
                } else if (args[0] == "cylzone" && args.Length == 2)
                {
                    _builder = new CylZoneBuilder(args[1], Alt.LocalPlayer.Position);
                } else if (args[0] == "debug")
                {
                    if (_zoneRender.Active)
                    {
                        Alt.OnTick -= _zoneRender.OnTick;
                        _zoneRender.StopRender();
                        _zoneRender.Active = false;
                    }
                    else
                    {
                        Alt.OnTick += _zoneRender.OnTick;
                        _zoneRender.Active = true;
                    }
                }
            }
            else
            {
                if (args[0] == "exit")
                {
                    _builder?.Dispose();
                    _builder = null;
                } else if (args[0] == "export" && args.Length == 2)
                {
                    _builder?.Export(_exporter, new []{args[1]});
                }
            }
        };

        Alt.OnServer<string,string>("vzones:editor", (arg, arg2) =>
        {
            switch (arg)
            {
                case "zone2":
                    if (_builder != null) return;
                    _builder = new Zone2Builder(arg2, Alt.LocalPlayer.Position);
                    break;
                case "zone3":
                    if (_builder != null) return;
                    _builder = new Zone3Builder(arg2, Alt.LocalPlayer.Position);
                    break;
                case "cylzone":
                    if (_builder != null) return;
                    _builder = new CylZoneBuilder(arg2, Alt.LocalPlayer.Position);
                    break;
                case "exit":
                    if (_builder == null) return;
                    _builder.Dispose();
                    _builder = null;
                    break;
                case "export":
                    if (_builder == null) return;
                    _builder.Export(_exporter, new []{arg2});
                    break;
            }
        });
        
        Alt.RegisterMValueAdapter(new Zone3Adapter());
        Alt.RegisterMValueAdapter(new CylZoneAdapter());
        Alt.RegisterMValueAdapter(new Zone2Adapter());
        
        _registerNativeEmits();
        
        Alt.LogInfo("[VZones] VZones client started.");
    }

    public override void OnStop()
    {
        _builder?.Dispose();
        
        using (_cts) _cts.Cancel();
        if (_zoneRender.Active)
        {
            Alt.OnTick -= _zoneRender.OnTick;
            _zoneRender.StopRender();
            _zoneRender.Active = false;
        }
    }

    private void _registerNativeEmits()
    {
        Alt.OnClient<string,string,string?>("vzones:register", _registerZoneNative);
        Alt.OnClient<string,string>("vzones:register", (type, filename) => _registerZoneNative(type, filename, null));
        
        Alt.OnClient<string>("vzones:unregister", name =>
        {
            Zones.UnregisterZone(name);
        });
    }

    private static void _registerZoneNative(string type, string filename, string? name)
    {
        if (name != null && name.Length > 0 && Zones.GetZone(name) != null)
        {
            Alt.LogError($"[VZones/vzones:register] Zone {name} already registered.");
            return;
        }
        
        if (!Alt.FileExists(filename))
        {
            Alt.LogError($"[VZones/vzones:register] File {filename} not found.");
            return;
        }
        string json = Alt.ReadFile(filename);
        IActiveZone? zone = null;
        if (type == "zone2")
        {
            zone = ZoneConvert.FromJson<Zone2>(json);
        }
        else if (type == "zone3")
        {
            zone = ZoneConvert.FromJson<Zone3>(json);
        }
        else if (type == "cylzone")
        {
            zone = ZoneConvert.FromJson<CylZone>(json);
        }
        else
        {
            Alt.LogError($"[VZones/vzones:register] Invalid zone type {type}.");
            return;
        }
        
        if (zone == null)
        {
            Alt.LogError($"[VZones/vzones:register] Zone {filename} could not be deserialized.");
            return;
        }
        
        if (name != null && name.Length > 0)
        {
            zone.Name = name;
        }
        
        if (Zones.GetZone(zone.Name) != null)
        {
            Alt.LogError($"[VZones/vzones:register] Zone {name} already registered.");
            return;
        }
        
        if (zone is Zone3 zone3)
        {
            zone3.Active = true;
            zone3.OnEnter += z => Alt.EmitClient("vzones:on:enter", z.Name);
            zone3.OnLeave += z => Alt.EmitClient("vzones:on:leave", z.Name);
        }
        if (zone is CylZone cylZone)
        {
            cylZone.Active = true;
            cylZone.OnEnter += z => Alt.EmitClient("vzones:on:enter", z.Name);
            cylZone.OnLeave += z => Alt.EmitClient("vzones:on:leave", z.Name);
        }
        if (zone is Zone2 zone2)
        {
            zone2.Active = true;
            zone2.OnCross += z => Alt.EmitClient("vzones:on:cross", z.Name);
        }
        
        Zones.RegisterZone(zone);
    }
}