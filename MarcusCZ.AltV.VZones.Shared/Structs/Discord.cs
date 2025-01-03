namespace MarcusCZ.AltV.VZones.Shared.Structs;

public struct DiscordEmbedFooter
{
    public string text { get; set; }
}

public struct DiscordEmbed
{
    public string title { get; set; }
    public string description { get; set; }
    public int color { get; set; }
    public DiscordEmbedFooter footer { get; set; }
}
    
public struct DiscordMessage
{
    public string content { get; set; }
    public DiscordEmbed[] embeds { get; set; }
}