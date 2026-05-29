using System.Text.Json;
using ZenCLI.Models;

namespace ZenCLI.Services;

public class ConfigManager
{
    private readonly string _configFilePath;

    public ConfigManager()
    {
        // To find home directory
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Creatig path to the covered directory 
        string zenDir = Path.Combine(homeDir, ".zencli");

        if (!Directory.Exists(zenDir))
        {
            Directory.CreateDirectory(zenDir);
        }

        _configFilePath = Path.Combine(zenDir, "config.json");
    }

    // Methond to read all settings we have set
    public ZenConfig LoadConfig()
    {
        if (!File.Exists(_configFilePath))
        {
            return new ZenConfig();
        }
        string json = File.ReadAllText(_configFilePath);
        return JsonSerializer.Deserialize<ZenConfig>(json) ?? new ZenConfig();
    }
    
    // Method for saving settings 
    public void SaveConfig(ZenConfig config)
    {
        var option = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(config, option);
        File.WriteAllText(_configFilePath, json);
    }
}