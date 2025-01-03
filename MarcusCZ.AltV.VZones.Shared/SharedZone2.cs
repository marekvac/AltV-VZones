using System.Numerics;
using AltV.Net;
using MarcusCZ.AltV.VZones.Shared.Structs;
using MarcusCZ.AltV.VZones.Shared.Util;

namespace MarcusCZ.AltV.VZones.Shared;

public abstract class SharedZone2 : IZone
{
    public string Type => GetType().Name;
    public Vector2 P1 { get; set; } 
    public Vector2 P2 { get; set; }
    public string Name { get; set; }
    public float Z { get; set; }
    public float Height { get; set; }
    private Vector3? _center;

    public Vector3 Center
    {
        get
        {
            if (!_center.HasValue) _center = GetCenter();
            return _center.Value;
        }
    }

    public bool Active { get; set; }
    
    public abstract IMValueBaseAdapter GetAdapter();
    
    protected SharedZone2(string name, Vector3 center)
    {
        Name = name;
        Height = 5;
        Z = center.Z;
        P1 = new Vector2(center.X, center.Y) + Vector2.One;
        P2 = new Vector2(center.X, center.Y) - Vector2.One;
    }

    protected SharedZone2(string name, Vector2 p1, Vector2 p2, float z, float height)
    {
        Name = name;
        P1 = p1;
        P2 = p2;
        Z = z;
        Height = height;
    }
    
    public bool IsInside(Vector3 point)
    {
        if (point.Z > Z + Height || point.Z < Z) return false;
        
        Vector2 point2d = VectorUtils.AsVector2(point);
        Vector2 wallDirection = P2 - P1;
        Vector2 toPosition = point2d - P1;
        float projection = Vector2.Dot(toPosition, Vector2.Normalize(wallDirection));
        return projection >= 0 && projection <= wallDirection.Length();
    }

    public Vector3 GetCenter()
    {
        float xMax = P1.X, xMin = P2.X, yMax = P1.Y, yMin = P2.Y;
        if (P2.X > xMax)
        {
            xMax = P2.X;
            xMin = P1.X;
        }

        if (P1.Y > yMax)
        {
            yMax = P2.Y;
            yMin = P1.Y;
        }
        
        float deltaX = xMax - xMin;
        float deltaY = yMax - yMin;
        
        return new Vector3(xMin + deltaX / 2, yMin + deltaY / 2, Z + Height / 2);
    }
    
    private Vector2 _getNormal()
    {
        Vector2 wallDirection = P2 - P1;
        return Vector2.Normalize(new Vector2(-wallDirection.Y, wallDirection.X));
    }

    public float GetSignedDistance(Vector3 point)
    {
        Vector2 point2d = VectorUtils.AsVector2(point);
        return Vector2.Dot(point2d - P1, _getNormal());
    }

    public void ReloadCenter()
    {
        _center = GetCenter();
    }

    public Line2? GetClosestSide(Vector3 point, float maxDistance)
    {
        Vector2 point2d = VectorUtils.AsVector2(point);
        
        if (Math.Max(Vector2.Distance(P1, point2d), Vector2.Distance(P2, point2d)) > maxDistance) return null;
        
        return new Line2(P1, P2);
    }

    public List<Line2> GetAllSides(Vector3 origin, float maxDistance)
    {
        Line2? line = GetClosestSide(origin, maxDistance);
        var list = new List<Line2>();
        if (line != null) list.Add(line.Value);
        return list;
    }
}