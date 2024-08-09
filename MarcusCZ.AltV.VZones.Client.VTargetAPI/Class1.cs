using AltV.Net.Client;
using AltV.Net.Client.Async;
using MarcusCZ.AltV.VTarget.Client;
using MarcusCZ.AltV.VZones.Client;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public class Class1 : AsyncResource
{
    private VZone? _inst;
    
    public override void OnStart()
    {
        _inst = new VZone();
        _inst.OnStart();
        VZone.RegisterBuilder<Zone2, Zone2Builder>();
        Alt.RegisterMValueAdapter(new Zone2Adapter());
        Target.RegisterProvider<VZoneOption>(new VZonesProvider());
    }

    public override void OnStop()
    {
        if (_inst != null) _inst.OnStop();
    }
}