using System.Text.Json;
using System.Text.Json.Nodes;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Server;

public class ServerZoneConvert
{
    public static IZone? FromJson(string json)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(json, ZoneConvert.JsonOptions);
        if (jsonNode == null)
        {
            return null;
        }
        string? type = jsonNode["Type"]?.GetValue<string>();
        if (type == null)
        {
            return null;
        }
        
        if (type == typeof(Zone3).Name)
        {
            return JsonSerializer.Deserialize<Zone3>(json, ZoneConvert.JsonOptions);
        } 
        if (type == typeof(CylZone).Name)
        {
            return JsonSerializer.Deserialize<CylZone>(json, ZoneConvert.JsonOptions);
        }

        return null;
    }
}