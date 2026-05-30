using ZenCLI.Services;

var configManager = new ConfigManager();
var config = configManager.LoadConfig();
var blockingService = new BlockingService();
var pomodoroManager = new PomodoroManager();

Console.CancelKeyPress += (sender, e) =>
{
    Console.ResetColor();
    Console.CursorVisible = true;
    Console.WriteLine("\n\n⚠️ Emergency stop (Ctrl+C)! Removing blocks...");
    blockingService.StopBlocking();
    Environment.Exit(0);
};

if (args.Length == 0)
{
    ShowHelp();
    return; 
}

string command = args[0].ToLower();

switch (command)
{
    case "start":
        var cts = new CancellationTokenSource();
        
        var inputTask = Task.Run(() =>
        {
            if (Console.ReadLine()?.ToLower() == "stop")
                cts.Cancel();
        });
    
        blockingService.StartBlocking(config.BlockedSites);
        await pomodoroManager.StartTimerAsync(
            config.Breaks.PomodoroDurationMinutes,
            config.Breaks.ShortBreakDurationMinutes, 
            cts.Token);
        blockingService.StopBlocking();
        break;
        
    case "lst":
        Console.WriteLine("📋 Your block list: ");
        foreach (var site in config.BlockedSites)
        {
            Console.WriteLine($"   - {site}");
        }
        break;
        
    case "tms":
        Console.Write($"⚙️ Set your focus time (minutes) [{config.Breaks.PomodoroDurationMinutes}]: ");
        string timeInput = Console.ReadLine();
       
        int finalMinutes = config.Breaks.PomodoroDurationMinutes; 
        
        if (!string.IsNullOrWhiteSpace(timeInput))
        {
            if (!int.TryParse(timeInput, out finalMinutes))
            {
                Console.WriteLine("❌ Please enter a valid number");
                break;
            }
        }

        Console.Write($"☕ Set your break time (minutes) [{config.Breaks.ShortBreakDurationMinutes}]: ");
        string breakInput = Console.ReadLine();
        
        int finalBreakMinutes = config.Breaks.ShortBreakDurationMinutes;
        
        if (!string.IsNullOrWhiteSpace(breakInput))
        {
            if (!int.TryParse(breakInput, out finalBreakMinutes))
            {
                Console.WriteLine("❌ Please enter a valid number");
                break;
            }
        }
    
        Console.Write($"🛋️ Set your long break time (minutes) [{config.Breaks.LongBreakDurationMinutes}]: ");
        string longBreakInput = Console.ReadLine();
        int finalLongBreakMinutes = config.Breaks.LongBreakDurationMinutes;
        
        if (!string.IsNullOrWhiteSpace(longBreakInput))
        {
            if (!int.TryParse(longBreakInput, out finalLongBreakMinutes))
            {
                Console.WriteLine("❌ Please enter a valid number");
                break;
            }
        }
        
        config.Breaks.PomodoroDurationMinutes = finalMinutes;
        config.Breaks.ShortBreakDurationMinutes = finalBreakMinutes;
        config.Breaks.LongBreakDurationMinutes = finalLongBreakMinutes;
        
        configManager.SaveConfig(config); 

        Console.WriteLine($"✅ Settings saved! Focus: {finalMinutes}m, Short break: {finalBreakMinutes}m, Long break: {finalLongBreakMinutes}m");
        break;
        
    case "add":
        if (args.Length < 2)
        {
            Console.WriteLine("❌ Error write site (Help: zen add youtube.com)");
            return;
        }
        string siteToAdd = args[1].ToLower();
        if (!config.BlockedSites.Contains(siteToAdd))
        {
            config.BlockedSites.Add(siteToAdd);
            configManager.SaveConfig(config); 
            Console.WriteLine($"✅ Added site '{siteToAdd}' to the list");
        }
        else
        {
            Console.WriteLine($"⚠️ Site '{siteToAdd}' is already in the list");
        }
        break;
        
    case "remove":
        if (args.Length < 2)
        {
            Console.WriteLine("❌ Error write site (Help: zen remove youtube.com)");
            return;
        }
        string siteToRemove = args[1].ToLower();
        if (config.BlockedSites.Remove(siteToRemove))
        {
            configManager.SaveConfig(config); 
            Console.WriteLine($"🗑️ Removed site '{siteToRemove}' from block list");
        }
        break;
    default:
        Console.WriteLine($"Unknown command {command}");
        ShowHelp();
        break;
}

void ShowHelp()
{
    Console.WriteLine("\n🧘 ZenCLI \nyour list - your plan");
    Console.WriteLine("Commands available:");
    Console.WriteLine("  start          → start timer");
    Console.WriteLine("  lst            → plan with auto breaks");
    Console.WriteLine("  tms            → you can make your own breaks");
    Console.WriteLine("  add <site>     → add site to block list");
    Console.WriteLine("  remove <site>  → remove site\n");
}