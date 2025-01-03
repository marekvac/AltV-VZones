namespace MarcusCZ.AltV.VZones.Shared;

public interface IActiveZone : IZone
{
    public bool Active { get; }
}