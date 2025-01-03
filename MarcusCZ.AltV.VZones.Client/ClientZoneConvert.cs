using System.Text.Json;
using System.Text.Json.Nodes;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client;

public class ClientZoneConvert
{
    public static IZone? FromJson(string json)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(json);
        if (jsonNode == null) return null;
        string? type = jsonNode["Type"]?.GetValue<string>();
        if (type != null) return null;
        
        if (type == typeof(Zone3).Name)
        {
            return JsonSerializer.Deserialize<Zone3>(json, ZoneConvert.JsonOptions);
        } 
        if (type == typeof(CylZone).Name)
        {
            return JsonSerializer.Deserialize<CylZone>(json, ZoneConvert.JsonOptions);
        }

        if (type == typeof(Zone2).Name)
        {
            return JsonSerializer.Deserialize<Zone2>(json, ZoneConvert.JsonOptions);
        }

        return null;
    }
}