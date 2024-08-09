using MarcusCZ.AltV.VTarget.Client.Options;
using MarcusCZ.AltV.VZones.Client;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public static class Zone3Ext
{
    private static Dictionary<Zone3, List<IVTargetOption>?> _dict = new ();
    
    public static void AddVTarget(this Zone3 zone, List<IVTargetOption>? options)
    {
        _dict[zone] = options;
    }

    public static List<IVTargetOption>? GetVTarget(this Zone3 zone)
    {
        if (!_dict.ContainsKey(zone)) return null;
        return _dict[zone];
    }
}