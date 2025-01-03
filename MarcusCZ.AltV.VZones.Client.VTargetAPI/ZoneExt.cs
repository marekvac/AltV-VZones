using MarcusCZ.AltV.VTarget.Client.Options;
using MarcusCZ.AltV.VZones.Client;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public static class ZoneExt
{
    private static Dictionary<IZone, List<IVTargetOption>?> _dict = new ();
    
    public static void AddVTarget(this IZone zone, List<IVTargetOption>? options)
    {
        _dict[zone] = options;
    }

    public static List<IVTargetOption>? GetVTarget(this IZone zone)
    {
        if (!_dict.ContainsKey(zone)) return null;
        return _dict[zone];
    }
}