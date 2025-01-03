using System.Numerics;
using AltV.Net;
using AltV.Net.Shared.Elements.Entities;
using MarcusCZ.AltV.VZones.Shared.Structs;

namespace MarcusCZ.AltV.VZones.Shared;

public delegate ISharedBlip BlipTypeDelegate(IZone zone);
public interface IZone : IMValueConvertible
{
    public string Type { get; }
    public Vector3 Center { get; }
    public string Name { get; set; }
    public float Z { get; set; }
    public float Height { get; set; }
    bool IsInside(Vector3 point);
    Vector3 GetCenter();
    Line2? GetClosestSide(Vector3 point, float maxDistance);
    List<Line2> GetAllSides(Vector3 origin, float maxDistance);
}