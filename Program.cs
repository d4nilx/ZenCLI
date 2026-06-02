using Spectre.Console;
using ZenCLI.Models;
using ZenCLI.Services;

var configManager = new ConfigManager();
var config = configManager.LoadConfig();
var blockingService = new BlockingService();
var pomodoroManager = new PomodoroManager();

AnsiConsole.Write(
    new FigletText("ZenCLI")
        .LeftJustified()
        .Color(Color.SpringGreen3));

await AnsiConsole.Status()
    .Spinner(Spinner.Known.BouncingBar) 
    .SpinnerStyle(Style.Parse("green"))
    .StartAsync("Bypassing procrastination firewalls...", async ctx => 
    {
        await Task.Delay(500);
        ctx.Status("Encrypting focus sessions...");
        await Task.Delay(500);
        ctx.Status("[green]Access Granted. System Ready.[/]");
        await Task.Delay(300);
    });

AnsiConsole.MarkupLine("\n[bold springgreen3]Welcome to your focus helper[/]\n");

Console.CancelKeyPress += (sender, e) =>
{
    Console.ResetColor();
    Console.CursorVisible = true;
    AnsiConsole.MarkupLine("\n\n[bold red]⚠️ Emergency stop (Ctrl+C)! Removing blocks...[/]");
    blockingService.StopBlocking();
    Environment.Exit(0);
};

if (args.Length == 0)
{
    ShowHelp();
    return; 
}

string command = args[0].ToLower();
CancellationTokenSource cts;

switch (command)
{
    case "start":
        cts = new CancellationTokenSource();
        var inputTask = Task.Run(() =>
        {
            if (Console.ReadLine()?.ToLower() == "stop") cts.Cancel();
        });

        blockingService.StartBlocking(config.BlockedSites);
    
        while (!cts.Token.IsCancellationRequested)
        {
            await pomodoroManager.StartTimerAsync(
                config.Breaks.PomodoroDurationMinutes,
                config.Breaks.ShortBreakDurationMinutes,
                config.Breaks.LongBreakDurationMinutes,
                cts.Token);
        }
        blockingService.StopBlocking();
        break;
     
    case "lst":
        var sitesTree = new Tree("[red]Blocked Sites[/]");
        foreach (var site in config.BlockedSites) sitesTree.AddNode($"[yellow]{site}[/]");
        AnsiConsole.Write(new Panel(sitesTree).BorderColor(Color.Red).RoundedBorder());
        break;
        
    case "tms":
        AnsiConsole.MarkupLine("[bold cyan]⚙️ Timer Settings[/]");
        
        config.Breaks.PomodoroDurationMinutes = AnsiConsole.Prompt(
            new TextPrompt<int>("Focus time (minutes):")
                .DefaultValue(config.Breaks.PomodoroDurationMinutes));

        config.Breaks.ShortBreakDurationMinutes = AnsiConsole.Prompt(
            new TextPrompt<int>("Short break (minutes):")
                .DefaultValue(config.Breaks.ShortBreakDurationMinutes));
    
        config.Breaks.LongBreakDurationMinutes = AnsiConsole.Prompt(
            new TextPrompt<int>("Long break (minutes):")
                .DefaultValue(config.Breaks.LongBreakDurationMinutes));
        
        configManager.SaveConfig(config); 
        AnsiConsole.MarkupLine("[bold green]✅ Settings saved successfully![/]");
        break;
        
    case "add":
        if (args.Length < 2) { AnsiConsole.MarkupLine("[red]❌ Error write site (Help: zen add youtube.com)[/]"); return; }
        string siteToAdd = args[1].ToLower();
        if (!config.BlockedSites.Contains(siteToAdd))
        {
            config.BlockedSites.Add(siteToAdd);
            configManager.SaveConfig(config); 
            AnsiConsole.MarkupLine($"[green]✅ Added site '{siteToAdd}' to the list[/]");
        }
        else AnsiConsole.MarkupLine($"[yellow]⚠️ Site '{siteToAdd}' is already in the list[/]");
        break;
        
    case "remove":
        if (args.Length < 2) { AnsiConsole.MarkupLine("[red]❌ Error write site (Help: zen remove youtube.com)[/]"); return; }
        string siteToRemove = args[1].ToLower();
        if (config.BlockedSites.Remove(siteToRemove))
        {
            configManager.SaveConfig(config); 
            AnsiConsole.MarkupLine($"[green]🗑️ Removed site '{siteToRemove}' from block list[/]");
        }
        break;

    case "plan":
        int plans = AnsiConsole.Prompt(
            new TextPrompt<int>("How many [green]tasks[/] do you have?")
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

        var table = new Table().Border(TableBorder.Rounded).Title("[bold yellow]Your Focus Plan[/]");
        table.AddColumn("Task Name");
        table.AddColumn("Focus");
        table.AddColumn("Break");

        foreach (var task in planTasks)
            table.AddRow($"[cyan]{task.Name}[/]", $"[green]{task.Minutes}m[/]", $"[blue]{task.BreakMinutes}m[/]");
        
        AnsiConsole.Write(table); 

        if (!AnsiConsole.Confirm("\n🚀 Start this plan right now?", defaultValue: true))
        {
            AnsiConsole.MarkupLine("[red]🛑 Plan canceled.[/]");
            break;
        }

        AnsiConsole.MarkupLine("\n[bold green]🔥 Let's go! May the focus be with you.[/]");
        blockingService.StartBlocking(config.BlockedSites);
        cts = new CancellationTokenSource();

        foreach (var task in planTasks)
        {
            if (cts.Token.IsCancellationRequested) break; 
            await pomodoroManager.RunCustomTaskAsync($"🎯 {task.Name} (Focus)", task.Minutes, cts.Token);
            
            if (cts.Token.IsCancellationRequested) break;
            if (task.BreakMinutes > 0)
                await pomodoroManager.RunCustomTaskAsync($"☕ {task.Name} (Break)", task.BreakMinutes, cts.Token);
        }

        blockingService.StopBlocking();
        AnsiConsole.MarkupLine("\n[bold green]🎉 All tasks completed! Memory cleared.[/]");
        break;

    default:
        AnsiConsole.MarkupLine($"[red]Unknown command {command}[/]");
        ShowHelp();
        break;
}

void ShowHelp()
{
    var helpTable = new Table().Border(TableBorder.Minimal);
    helpTable.AddColumn("[springgreen3]Command[/]");
    helpTable.AddColumn("Description");
    helpTable.AddRow("[green]start[/]", "Start timer");
    helpTable.AddRow("[green]lst[/]", "Show blocked sites list");
    helpTable.AddRow("[green]tms[/]", "Make your own breaks");
    helpTable.AddRow("[green]plan[/]", "Create a custom task plan");
    helpTable.AddRow("[green]add <site>[/]", "Add site to block list");
    helpTable.AddRow("[green]remove <site>[/]", "Remove site");

    AnsiConsole.Write(
        new Panel(helpTable)
            .Header("[bold yellow]ZenCLI - Your list, your plan[/]")
            .Expand());
}