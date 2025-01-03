using MarcusCZ.AltV.VTarget.Client;
using MarcusCZ.AltV.VTarget.Client.Data;
using MarcusCZ.AltV.VTarget.Client.Options;
using MarcusCZ.AltV.VZones.Client;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.VTargetAPI;

public delegate bool VOptionCheckCallback(IZone zone);
public delegate bool VOptionCallback(IZone zone, Alert alert);
public class VZoneOption : IVTargetOption
{
    public string Id { get; }
    public string Icon { get; set; }
    public string Label { get; set; }
    public float? Distance { get; set; }
    public string? Description { get; set; }
    public Background? Background { get; set; }
    public bool EnableInVehicle { get; set; }
    public Position Position { get; set; }
    public bool Show { get; set; }
    public bool Interact { get; set; }
    
    private List<VZoneOption>? _children;
    public List<IVTargetOption>? Children
    {
        get => _children?.Cast<IVTargetOption>().ToList();
        set => _children = value?.Cast<VZoneOption>().ToList();
    }

    public VOptionCheckCallback CanShow { get; set; }
    public VOptionCheckCallback CanInteract { get; set; }
    public VOptionCallback OnClick { get; set; }
    public VOptionCallback OnDisabledClick { get; set; }

    public VZoneOption(string icon, string label, string? id = null)
    {
        Icon = icon;
        Label = label;
        Id = id ?? Utils.RandomId();
        CanShow = _ => true;
        CanInteract = _ => true;
        OnClick = (_, _) => false;
        OnDisabledClick = (_,_) => false;
        Distance = 7;
        Position = Position.RIGHT;
    }

    public object[] Serialize()
    {
        string desc = Description ?? "";
        string bg = Background.ToString() ?? "";
        bg = bg.ToLower();
        object[] children = Array.Empty<object>();
        if (Children != null && Children.Count > 0)
        {
            children = new object[Children.Count];
            for (int i = 0; i < Children.Count; i++)
            {
                children[i] = Children[i].Serialize();
            }
        }
        object[] obj = {Id,Icon,Label,desc,bg,Interact,Position.ToString(),children};
        return obj;
    }
}