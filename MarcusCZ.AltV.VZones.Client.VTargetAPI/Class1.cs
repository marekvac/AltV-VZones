using AltV.Net.Client.Async;
using MarcusCZ.AltV.VTarget.Client;
using MarcusCZ.AltV.VZones.Client;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public class Class1 : AsyncResource
{
    private VZonesClient? _inst;
    
    public override void OnStart()
    {
        _inst = new VZonesClient();
        _inst.OnStart();
        Target.RegisterProvider<VZoneOption>(new VZonesProvider());
    }

    public override void OnStop()
    {
        if (_inst != null) _inst.OnStop();
    }
}