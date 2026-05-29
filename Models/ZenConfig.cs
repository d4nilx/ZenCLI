namespace ZenCLI.Models;

public class ZenConfig
{
    public List<string> BlockedSites { get; set; } = new List<string>();
    public BreakSettings Breaks { get; set; } = new BreakSettings();
}