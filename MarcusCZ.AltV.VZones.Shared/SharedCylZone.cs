using System.Numerics;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Shared.Enums;
using MarcusCZ.AltV.VZones.Shared.Structs;

namespace MarcusCZ.AltV.VZones.Shared;

public abstract class SharedCylZone : IZone
{
    public string Type => GetType().Name;
    public Vector3 Center { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public float Z
    {
        get => Center.Z;
        set => Center = Center with {Z = value};
    }
    public float Height { get; set; }
    public float Radius { get; set; }

    public bool Active { get; set; }
    
    public MarkerType MarkerType { get; set; } = MarkerType.MarkerCylinder;
    public uint StreamingDistance { get; set; } = 100;

    protected SharedCylZone()
    {
    }

    public SharedCylZone(string name, Vector3 center, float radius, float height = 5)
    {
        Center = center;
        Name = name;
        Radius = radius;
        Height = height;
    }
    
    public void Move(Vector3 direction)
    {
        Center += direction;
    }
    
    public bool IsInside(Vector3 point)
    {
        if (point.Z < Z) return false;
        if (point.Z > Z + Height) return false;

        return Vector2.Distance(new Vector2(point.X, point.Y), new Vector2(Center.X, Center.Y)) <= Radius;
    }

    public Vector3 GetCenter()
    {
        return Center;
    }

    public Line2? GetClosestSide(Vector3 point, float maxDistance)
    {
        return null;
    }

    public List<Line2> GetAllSides(Vector3 origin, float maxDistance)
    {
        return new List<Line2>();
    }

    public abstract IMValueBaseAdapter GetAdapter();

    public override string ToString()
    {
        return $"{nameof(Center)}: {Center}, {nameof(Name)}: {Name}, {nameof(Height)}: {Height}, {nameof(Radius)}: {Radius}, {nameof(Active)}: {Active}, {nameof(MarkerType)}: {MarkerType}, {nameof(StreamingDistance)}: {StreamingDistance}";
    }
}