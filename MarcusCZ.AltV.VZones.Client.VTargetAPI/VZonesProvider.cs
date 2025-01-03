using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Async;
using MarcusCZ.AltV.VTarget.Client;
using MarcusCZ.AltV.VTarget.Client.Options;
using MarcusCZ.AltV.VTarget.Client.Providers;
using MarcusCZ.AltV.VZones.Client;
using MarcusCZ.AltV.VZones.Shared;
using MarcusCZ.AltV.VZones.Shared.Structs;
using MarcusCZ.AltV.VZones.Shared.Util;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public class VZonesProvider : IVAsyncProvider
{
    public Type GetTypeProvided
    {
        get => typeof(VZoneOption);
    }
    public bool BlockOthers
    {
        get => true;
    }

    private IZone? _lastZone;
    private List<IVTargetOption> _lastOptions = new();
    private bool _needRefresh;
    private Vector3 _lastPlayerPos = Vector3.Zero;
    private Vector3? _lastHitPos = Vector3.Zero;

    public bool GetOptions(out List<IVTargetOption> options)
    {
        options = new();
        return false;
    }

    public void EmitOnClick(IVTargetOption option, Alert alert)
    {
        if (option is VZoneOption zoneOption && _lastZone is not null)
        {
            if (option.Interact) _needRefresh = zoneOption.OnClick(_lastZone, alert);
            else _needRefresh = zoneOption.OnDisabledClick(_lastZone, alert);
        }
    }

    public async Task<List<IVTargetOption>> GetOptionsAsync()
    {
        List<IVTargetOption> options = new();
        foreach (var zone in VZonesClient.Zones.GetClosestZonesAtRange(5, 30))
        {
            List<IVTargetOption>? vtarget = zone.GetVTarget();
            if (vtarget == null) continue;

            Vector3 playerPos = Alt.LocalPlayer.Position;

            Vector3 camPos = await AltAsync.Do(() => Alt.Natives.GetGameplayCamCoord());
            Vector3 camRot = await AltAsync.Do(() => Alt.Natives.GetGameplayCamRot(0));
            Line3 rayLine = new Line3(camPos, VectorUtils.RayEnd(camPos, VectorUtils.ToRadians(camRot), 100));

            bool hit = false;
            Vector3 hitPos = default;
            foreach (var side in zone.GetAllSides(playerPos, 30))
            {
                Rectangle3 rectangle = new Rectangle3(new Vector3(side.P1, zone.Z), new Vector3(side.P2, zone.Z + zone.Height));
                
                if (rectangle.IntersectsLine3(rayLine, out hitPos))
                {
                    if (Target.Debug) _drawDebug(rectangle, rayLine, hitPos);
                    hit = true;
                    break;
                }
            }
            if (!hit) continue;

            if (!_needRefresh && Vector3.Distance(playerPos, _lastPlayerPos) < 1 && _lastHitPos.HasValue && Vector3.Distance(hitPos, _lastHitPos!.Value) < 1 && zone == _lastZone) return new (_lastOptions);
            
            _lastPlayerPos = playerPos;
            _lastHitPos = hitPos;
            
            options = _evalOptions(zone, vtarget, Vector3.Distance(hitPos, playerPos));
            _lastOptions = new List<IVTargetOption>(options);
            _lastZone = zone;
            _needRefresh = false;
            return options; 
        }

        _lastHitPos = null;
        _lastZone = null;
        return options;
    }

    private static List<IVTargetOption> _evalOptions(IZone zone, List<IVTargetOption> options, float hitDistance)
    {
        var newOptions = new List<IVTargetOption>();
        options.ForEach(option =>
        {
            if (option is not VZoneOption o) return;
            
            o.Show = o.CanShow(zone);

            if (!o.EnableInVehicle) o.Show = !Alt.LocalPlayer.IsInVehicle;
            
            if (o.Show && o.Distance is not null && o.Distance < hitDistance)
            {
                o.Show = false;
            }
            
            if (o.Show)
            {
                o.Interact = o.CanInteract(zone);
                if (o.Children is not null && o.Children.Count > 0)
                {
                    _evalOptions(zone, o.Children, hitDistance);
                }
                newOptions.Add(o);
            }
        });
        return newOptions;
    }

    // private bool _intersects(Vector3 rect1, Vector3 rect2, Vector3 rayStart, Vector3 rayEnd, out Vector3 hitPos)
    // {
    //     var rectMax = Vector3.Max(rect1, rect2);
    //     var rectMin = Vector3.Min(rect1, rect2);
    //
    //     if (Debug) _drawDebug(rectMin, rectMax, rayStart, rayEnd);
    //     
    //     hitPos = Vector3.Zero;
    //
    //     // primka steny v rovine XY
    //     Line xyWall = new Line(RaycastUtils.AsVector2(rect1), RaycastUtils.AsVector2(rect2));
    //     
    //     // primka raycastu v rovine XY
    //     Line xyRay = new Line(RaycastUtils.AsVector2(rayStart), RaycastUtils.AsVector2(rayEnd));
    //     
    //     // kontrola zda se primky protinaji v rovine XY
    //     Vector2? xyIntersection = _linesIntersect(xyWall, xyRay);
    //
    //     // Pokud se neprotinaji v teto rovine, hrac nemiri na stenu
    //     if (!xyIntersection.HasValue) return false;
    //     
    //     if (Debug) _drawDebugLine(new Vector3(xyIntersection.Value, rectMin.Z), new Vector3(xyIntersection.Value, rectMax.Z));
    //
    //     // primka steny v pruseciku v rovine XZ
    //     Line xzWall = new Line(
    //         xyIntersection.Value with {Y = rectMin.Z},
    //         xyIntersection.Value with {Y = rectMax.Z}
    //     );
    //     
    //     // primka raycastu v rovine XZ
    //     Line xzRay = new Line(
    //         new Vector2(rayStart.X, rayStart.Z),
    //         new Vector2(rayEnd.X, rayEnd.Z)
    //     );
    //
    //     // kontrola zda se primky protinaji v rovine XZ
    //     if (_linesIntersect(xzWall, xzRay) is var xzIntersection && xzIntersection is not null)
    //     {
    //         if (Debug) _drawDebugLine(rectMin with {Z = xzIntersection.Value.Y}, rectMax with {Z = xzIntersection.Value.Y});
    //         hitPos = new Vector3(xyIntersection.Value, xzIntersection.Value.Y);
    //         return true;
    //     }
    //     // primky se mohou v rovine XZ prekryvat, proto kontrolujeme posledni rovinu
    //     
    //     // primka steny v pruseciku v rovine YZ
    //     Line yzWall = new Line(
    //         new Vector2(xyIntersection.Value.Y, rectMin.Z),
    //         new Vector2(xyIntersection.Value.Y, rectMax.Z)
    //     );
    //     
    //     // primka raycastu v rovine YZ
    //     Line yzRay = new Line(
    //         new Vector2(rayStart.Y, rayStart.Z),
    //         new Vector2(rayEnd.Y, rayEnd.Z)
    //     );
    //     
    //     if (_linesIntersect(yzWall, yzRay) is var yzIntersection && yzIntersection is not null)
    //     {
    //         if (Debug) _drawDebugLine(rectMin with {Z = yzIntersection.Value.Y}, rectMax with {Z = yzIntersection.Value.Y});
    //         hitPos = new Vector3(xyIntersection.Value, yzIntersection.Value.Y);
    //         return true;
    //     }
    //     
    //     return false;
    // }
    //
    // private static Vector2? _linesIntersect(Line line1, Line line2)
    // {
    //     var denominator = ((line2.End.Y - line2.Start.Y) * (line1.End.X - line1.Start.X)) -
    //                       ((line2.End.X - line2.Start.X) * (line1.End.Y - line1.Start.Y));
    //
    //     if (denominator == 0)
    //     {
    //         return null;
    //     }
    //
    //     var a = line1.Start.Y - line2.Start.Y;
    //     var b = line1.Start.X - line2.Start.X;
    //     var numerator1 = ((line2.End.X - line2.Start.X) * a) - ((line2.End.Y - line2.Start.Y) * b);
    //     var numerator2 = ((line1.End.X - line1.Start.X) * a) - ((line1.End.Y - line1.Start.Y) * b);
    //     a = numerator1 / denominator;
    //     b = numerator2 / denominator;
    //
    //     if (a >= 0 && a <= 1 && b >= 0 && b <= 1)
    //     {
    //         return new Vector2(
    //             line1.Start.X + (a * (line1.End.X - line1.Start.X)),
    //             line1.Start.Y + (a * (line1.End.Y - line1.Start.Y))
    //         );
    //     }
    //
    //     return null;
    // }

    private static void _drawDebug(Rectangle3 rectangle, Line3 line, Vector3 hitPos)
    {
        var rectMax = rectangle.P1;
        var rectMin = rectangle.P2;
        
        AltAsync.RunOnMainThread(() =>
        {
            Alt.Natives.DrawLine(rectMin.X, rectMin.Y, rectMin.Z, rectMin.X, rectMin.Y, rectMax.Z, 255,0,0,255);
            Alt.Natives.DrawLine(rectMin.X, rectMin.Y, rectMax.Z, rectMax.X, rectMax.Y, rectMax.Z, 255,0,0,255);
            Alt.Natives.DrawLine(rectMax.X, rectMax.Y, rectMax.Z, rectMax.X, rectMax.Y, rectMin.Z, 255,0,0,255);
            Alt.Natives.DrawLine(rectMax.X, rectMax.Y, rectMin.Z, rectMin.X, rectMin.Y, rectMin.Z, 255,0,0,255);
            
            Alt.Natives.DrawLine(rectMin.X, rectMin.Y, rectMin.Z, rectMin.X, rectMax.Y, rectMin.Z, 0,255,0,255);
            Alt.Natives.DrawLine(rectMin.X, rectMax.Y, rectMin.Z, rectMax.X, rectMax.Y, rectMin.Z, 0,255,0,255);
            Alt.Natives.DrawLine(rectMax.X, rectMax.Y, rectMin.Z, rectMax.X, rectMin.Y, rectMin.Z, 0,255,0,255);
            Alt.Natives.DrawLine(rectMax.X, rectMin.Y, rectMin.Z, rectMin.X, rectMin.Y, rectMin.Z, 0,255,0,255);
            
            Alt.Natives.DrawLine(line.P1.X, line.P1.Y, line.P1.Z, line.P2.X, line.P2.Y, line.P2.Z, 0,0,255,255);
            
            Alt.Natives.DrawLine(hitPos.X, hitPos.Y, rectMin.Z, hitPos.X, hitPos.Y, rectMax.Z, 255,255,0,255);
            Alt.Natives.DrawLine(rectangle.P1.X, rectangle.P1.Y, hitPos.Z, rectangle.P2.X, rectangle.P2.Y, hitPos.Z, 255,255,0,255);
        });
    }
}