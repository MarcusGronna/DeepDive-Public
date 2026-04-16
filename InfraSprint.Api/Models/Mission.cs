namespace InfraSprint.Api.Models;

public class Mission
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int ThreatLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}
