using System.Text.Json;
using AltV.Net.Client;
using MarcusCZ.AltV.VZones.Shared.Structs;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client.Exporters;

public class DiscordZoneExporter : IZoneExporter
{
    public void ExportZone(IZone zone, string[] args)
    {
        if (zone.GetType() == typeof(Zone2)) _exportZone2((Zone2) zone, args);
        if (zone.GetType() == typeof(Zone3)) _exportZone3((Zone3) zone, args);
        if (zone.GetType() == typeof(CylZone)) _exportCylZone((CylZone) zone, args);
    }

    private void _exportZone2(Zone2 zone, string[] args)
    {
        string code;
        string type = args[0];
        if (args.Contains("json"))
        {
            code = zone.ToJson();
            code = $"```json\n{code}\n```";
        }
        else
        {
            code = $"new Zone2(\"{zone.Name}\", new ({zone.P1.X}f, {zone.P1.Y}f), new ({zone.P2.X}f, {zone.P2.Y}f), {zone.Z}f, {zone.Height}f);";
            code = $"```csharp\n{code}\n```";
            type = "C#12";
        }
        DiscordEmbed embed = new DiscordEmbed();
        embed.description = $"Type: `Zone2`\nName: `{zone.Name}`\nExport type: `{type}`\n{code}";
        _sendEmbed(embed);
    }
    
    private void _exportZone3(Zone3 zone, string[] args)
    {
        string code;
        string type = args[0];
        if (args.Contains("json"))
        {
            code = zone.ToJson();
            code = $"```json\n{code}\n```";
        }
        else
        {
            string vectors = "";
            zone.Points.ForEach(point =>
            {
                vectors += $"\tnew ({point.X}f, {point.Y}f),\n";
            });
            vectors = vectors.TrimEnd('\n');
            vectors = vectors.TrimEnd(',');
            // string active = "\n{\n\tActive = true,\n\tOnEnter = zone =>\n\t{\n\t\t// On zone enter. !NOTE method is called async. In case of using natives, use AltAsync.RunOnMainThread(...)\n\t},\n\tOnLeave = zone =>\n\t{\n\t\t// On zone leave. !NOTE method is called async. In case of using natives, use AltAsync.RunOnMainThread(...)\n\t}\n}";
        
            code = $"new Zone3(\"{zone.Name}\", {zone.Z}f, {zone.Height}f, [\n{vectors}\n])";
            // code += active;
            code += ";";
            code = $"```csharp\n{code}\n```";
            type = "C#12";
        }
        
        DiscordEmbed embed = new DiscordEmbed();
        embed.description = $"Type: `Zone3`\nName: `{zone.Name}`\nExport type: `{type}`\n{code}";
        _sendEmbed(embed);
    }

    public void _exportCylZone(CylZone zone, string[] args)
    {
        string code;
        string type = args[0];
        if (args.Contains("json"))
        {
            code = zone.ToJson();
            code = $"```json\n{code}\n```";
        }
        else
        {   
            code = $"new CylZone(\"{zone.Name}\", new ({zone.Center.X}f, {zone.Center.Y}f, {zone.Center.Z}f), {zone.Radius}f, {zone.Height}f);";
            code = $"```csharp\n{code}\n```";
            type = "C#12";
        }
        DiscordEmbed embed = new DiscordEmbed();
        embed.description = $"Type: `CylZone`\nName: `{zone.Name}`\nExport type: `{type}`\n{code}";
        _sendEmbed(embed);
    }

    private void _sendEmbed(DiscordEmbed embed)
    {
        DiscordMessage message = new DiscordMessage();
        embed.title = "PolyZone";
        // embed.color = 5814783;
        embed.color = 88 << 16 | 185 << 8 | 255;
        
        embed.footer = new DiscordEmbedFooter {text = Alt.LocalPlayer.Name};
        message.embeds = new[] {embed};
        Alt.EmitServer("polyzone:senddiscord", JsonSerializer.Serialize(message));
    }
}