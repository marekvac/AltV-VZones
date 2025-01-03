using System.Text;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using MarcusCZ.AltV.VZones.Server.Adapters;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;


public class VZonesServer : AsyncResource
{
    public static ServerZoneManager Zones { get; private set; }
    private Thread? _zonesThread;
    private CancellationTokenSource _cts;
    private Task t;
    private HttpClient _httpClient;
    public string? WebhookUrl { get; set; }
    public bool GlobalChecking { get; set; }
    
    public override void OnStart()
    {
        if (!GlobalChecking) GlobalChecking = Alt.Resource.GetConfig()["vzones"]["global_checking"].GetBoolean() ?? false;
        if (WebhookUrl == null) WebhookUrl = Alt.Resource.GetConfig()["vzones"]["discord_webhook"].GetString() ?? null;
        
        Zones = new ServerZoneManager(GlobalChecking);
        _cts = new();
        t = Task.Run(() => Zones.DoChecking(_cts.Token), _cts.Token);
        
        _httpClient = new HttpClient();
        
        if (WebhookUrl != null)
        {
            AltAsync.OnClient<IPlayer, string>("polyzone:senddiscord", async (_,s) =>
            {
                var content = new StringContent(s, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync(WebhookUrl!, content);
            });
        }
        else
        {
            Alt.LogInfo("[VZones] Discord webhook not set. Discord export is disabled.");
        }
        
        Alt.RegisterMValueAdapter(new Zone3Adapter());
        Alt.RegisterMValueAdapter(new CylZoneAdapter());
        Alt.RegisterMValueAdapter(new Zone2Adapter());
        
        _registerNativeEmits();
        
        Alt.LogInfo("[VZones] VZones server started.");
    }

    public override void OnStop()
    {
        using (_cts) _cts.Cancel();
    }

    private void _registerNativeEmits()
    {
        Alt.OnServer<string, string, string?>("vzones:register", _registerZoneNative);
        Alt.OnServer<string, string>("vzones:register", (type, filename) => _registerZoneNative(type, filename, null));
        
        Alt.OnServer<string>("vzones:unregister", name =>
        {
            Zones.UnregisterZone(name);
        });
    }
    
    private void _registerZoneNative(string type, string filename, string? name)
    {
        if (name != null && name.Length > 0 && Zones.GetZone(name) != null)
        {
            Alt.LogError($"[VZones/vzones:register] Zone {name} already registered.");
            return;
        }
        
        if (!File.Exists(filename))
        {
            Alt.LogError($"[VZones/vzones:register] File {filename} not found.");
            return;
        }
        string json = File.ReadAllText(filename);
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
            zone3.OnEnter += (player, z) => Alt.Emit("vzones:on:enter", player, z.Name);
            zone3.OnLeave += (player, z) => Alt.Emit("vzones:on:leave", player, z.Name);
        }
        if (zone is CylZone cylZone)
        {
            cylZone.Active = true;
            cylZone.OnEnter += (player, z) => Alt.Emit("vzones:on:enter", player, z.Name);
            cylZone.OnLeave += (player, z) => Alt.Emit("vzones:on:leave", player, z.Name);
        }
        if (zone is Zone2 zone2)
        {
            zone2.Active = true;
            zone2.OnCross += (player, z) => Alt.Emit("vzones:on:cross", player, z.Name);
        }
        
        Zones.RegisterZone(zone);
    }
}