
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client.Exporters;

internal interface IZoneExporter
{
    public void ExportZone(IZone zone, string[] args);
}