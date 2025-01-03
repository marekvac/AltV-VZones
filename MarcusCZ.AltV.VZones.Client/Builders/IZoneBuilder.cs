using MarcusCZ.AltV.VZones.Client.Exporters;

namespace MarcusCZ.AltV.VZones.Client.Builders;

internal interface IZoneBuilder : IDisposable
{
    public void Export(IZoneExporter exporter, string[] args);
}