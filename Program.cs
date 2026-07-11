using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;
using ZenCLI.Models;
using ZenCLI.Services;

// Initialize services
var configManager = new ConfigManager();
var config = configManager.LoadConfig();
var blockingService = new BlockingService();
var pomodoroManager = new PomodoroManager();
var pixelArtAnimator = new PixelArtAnimator();

// Setup emergency exit handler (Ctrl+C)
Console.CancelKeyPress += (sender, e) =>
{
    Console.ResetColor();
    Console.CursorVisible = true;
    AnsiConsole.MarkupLine("\n\n[bold red]⚠️ Emergency stop! Removing blocking...[/]");
    blockingService.StopBlocking();
    Environment.Exit(0);
};

// Display beautiful loading animation only once on startup
await ShowStartupAnimationAsync();

// Main application loop
while (true)
{
    Console.Clear();
    ShowBanner();

    // Create interactive menu
    var selectedOption = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold springgreen2]What would you like to do?[/]")
            .PageSize(10)
            .HighlightStyle(new Style(foreground: Color.Black, background: Color.SpringGreen3)) // Highlight color
            .AddChoices(new[] {
                "🚀 Start Focus Session (Start)",
                "📋 Create Task Plan (Plan)",
                "⚙️ Timer Settings (Settings)",
                "🌐 View Blocked Sites (List)",
                "➕ Add Site (Add)",
                "➖ Remove Site (Remove)",
                "❌ Exit (Exit)"
            }));

    // Determine what to do based on selected option
    switch (selectedOption)
    {
        case "🚀 Start Focus Session (Start)":
            await StartFocusSessionAsync(config, blockingService, pomodoroManager);
            break;

        case "📋 Create Task Plan (Plan)":
            await CreatePlanAsync(config, blockingService, pomodoroManager);
            break;

        case "⚙️ Timer Settings (Settings)":
            UpdateSettings(config, configManager);
            break;

        case "🌐 View Blocked Sites (List)":
            ShowBlockedSites(config);
            break;

        case "➕ Add Site (Add)":
            AddSite(config, configManager);
            break;

        case "➖ Remove Site (Remove)":
            RemoveSite(config, configManager);
            break;

        case "❌ Exit (Exit)":
            AnsiConsole.MarkupLine("[bold springgreen3]Have a great day! Protect your focus.[/]");
            return; // Exit program
    }

    // Pause before returning to menu so user can read the result
    AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
    Console.ReadKey(true);
}

// ==========================================
// HELPER METHODS IMPLEMENTATION
// ========================================== 

async Task ShowStartupAnimationAsync()
{
    await AnsiConsole.Status()
        .Spinner(Spinner.Known.BouncingBar) 
        .SpinnerStyle(Style.Parse("springgreen3"))
        .StartAsync("Bypassing procrastination firewalls...", async ctx => 
        {
            await Task.Delay(500);
            ctx.Status("Encrypting focus sessions...");
            await Task.Delay(500);
            ctx.Status("[springgreen2]Access Granted. System Ready.[/]");
            await Task.Delay(300);
        });
    
    // Display beautiful pixel art animation
    Console.Clear();
    AnsiConsole.WriteLine();
    var animator = new PixelArtAnimator();
    await animator.DisplayAnimatedTextAsync("ZENCLI", delayMs: 40, useGradient: true);
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold cyan]Your Focus Helper[/]");
    await Task.Delay(800);
}

void ShowBanner()
{
    // Create header with different shades of green
    var figlet = new FigletText("ZenCLI")
        .Centered()
        .Color(Color.SpringGreen3);

    var panel = new Panel(figlet)
        .BorderColor(Color.DarkOliveGreen3)
        .RoundedBorder()
        .Header("[bold springgreen2]Your Focus Helper[/]")
        .Padding(1, 1);

    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();
}

async Task StartFocusSessionAsync(ZenConfig cfg, BlockingService blockSvc, PomodoroManager pomoMgr)
{
    var cts = new CancellationTokenSource();
    
    // Start separate thread to monitor key presses to stop the session
    Task.Run(() =>
    {
        AnsiConsole.MarkupLine("[grey](Press 'Q' at any time to stop the session)[/]");
        while (!cts.Token.IsCancellationRequested)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            {
                cts.Cancel();
            }
        }
    });

    blockSvc.StartBlocking(cfg.BlockedSites);
    
    while (!cts.Token.IsCancellationRequested)
    {
        await pomoMgr.StartTimerAsync(
            cfg.Breaks.PomodoroDurationMinutes,
            cfg.Breaks.ShortBreakDurationMinutes,
            cfg.Breaks.LongBreakDurationMinutes,
            cts.Token);
    }
    
    blockSvc.StopBlocking();
}

async Task CreatePlanAsync(ZenConfig cfg, BlockingService blockSvc, PomodoroManager pomoMgr)
{
    int plans = AnsiConsole.Prompt(
        new TextPrompt<int>("How many [green]tasks[/] do you have today?")
            .Validate(n => n > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Must be greater than 0[/]")));
    
    List<PlanTask> planTasks = new List<PlanTask>();

    for (int i = 1; i <= plans; i++)
    {
        AnsiConsole.MarkupLine($"\n[bold cyan]--- Task #{i} ---[/]");
        PlanTask currentTask = new PlanTask();
        
        currentTask.Name = AnsiConsole.Prompt(new TextPrompt<string>("Task name:"));
        currentTask.Minutes = AnsiConsole.Prompt(new TextPrompt<int>("Focus time (minutes):").DefaultValue(25));
        currentTask.BreakMinutes = AnsiConsole.Prompt(new TextPrompt<int>("Break time (minutes):").DefaultValue(5));
        
        planTasks.Add(currentTask);
    }

    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.SpringGreen2).Title("[bold yellow]Your Focus Plan[/]");
    table.AddColumn("Task");
    table.AddColumn("Focus");
    table.AddColumn("Break");

    foreach (var task in planTasks)
        table.AddRow($"[cyan]{task.Name}[/]", $"[green]{task.Minutes} min[/]", $"[blue]{task.BreakMinutes} min[/]");
    
    AnsiConsole.Write(table); 

    if (!AnsiConsole.Confirm("\n🚀 Start this plan now?", defaultValue: true))
    {
        AnsiConsole.MarkupLine("[red]🛑 Plan cancelled.[/]");
        return;
    }

    AnsiConsole.MarkupLine("\n[bold green]🔥 Let's go! May focus be with you.[/]");
    var cts = new CancellationTokenSource();
    
    Task.Run(() =>
    {
        while (!cts.Token.IsCancellationRequested)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q) cts.Cancel();
        }
    });

    blockSvc.StartBlocking(cfg.BlockedSites);

    foreach (var task in planTasks)
    {
        if (cts.Token.IsCancellationRequested) break; 
        await pomoMgr.RunCustomTaskAsync($"🎯 {task.Name} (Focus)", task.Minutes, cts.Token);
        
        if (cts.Token.IsCancellationRequested) break;
        if (task.BreakMinutes > 0)
            await pomoMgr.RunCustomTaskAsync($"☕ {task.Name} (Break)", task.BreakMinutes, cts.Token);
    }

    blockSvc.StopBlocking();
    AnsiConsole.MarkupLine("\n[bold green]🎉 All tasks completed![/]");
}

void UpdateSettings(ZenConfig cfg, ConfigManager cfgMgr)
{
    AnsiConsole.MarkupLine("[bold cyan]⚙️ Timer Settings[/]");
    
    cfg.Breaks.PomodoroDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Focus duration (minutes):")
            .DefaultValue(cfg.Breaks.PomodoroDurationMinutes));

    cfg.Breaks.ShortBreakDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Short break (minutes):")
            .DefaultValue(cfg.Breaks.ShortBreakDurationMinutes));

    cfg.Breaks.LongBreakDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Long break (minutes):")
            .DefaultValue(cfg.Breaks.LongBreakDurationMinutes));
    
    cfgMgr.SaveConfig(cfg); 
    AnsiConsole.MarkupLine("[bold green]✅ Settings saved successfully![/]");
}

void ShowBlockedSites(ZenConfig cfg)
{
    var sitesTree = new Tree("[red]Blocked Sites[/]");
    foreach (var site in cfg.BlockedSites) 
        sitesTree.AddNode($"[yellow]{site}[/]");
        
    AnsiConsole.Write(new Panel(sitesTree).BorderColor(Color.Red).RoundedBorder());
}

void AddSite(ZenConfig cfg, ConfigManager cfgMgr)
{
    string siteToAdd = AnsiConsole.Prompt(new TextPrompt<string>("Enter domain to block (e.g., youtube.com):")).ToLower();
    
    if (!cfg.BlockedSites.Contains(siteToAdd))
    {
        cfg.BlockedSites.Add(siteToAdd);
        cfgMgr.SaveConfig(cfg); 
        AnsiConsole.MarkupLine($"[green]✅ Site '{siteToAdd}' added successfully.[/]");
    }
    else 
    {
        AnsiConsole.MarkupLine($"[yellow]⚠️ Site '{siteToAdd}' already exists in the list.[/]");
    }
}

void RemoveSite(ZenConfig cfg, ConfigManager cfgMgr)
{
    if (cfg.BlockedSites.Count == 0)
    {
         AnsiConsole.MarkupLine("[yellow]Site list is empty.[/]");
         return;
    }

    // Make removal interactive too!
    var siteToRemove = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select site to remove:")
            .AddChoices(cfg.BlockedSites));

    if (cfg.BlockedSites.Remove(siteToRemove))
    {
        cfgMgr.SaveConfig(cfg); 
        AnsiConsole.MarkupLine($"[green]🗑️ Site '{siteToRemove}' unblocked.[/]");
    }
}