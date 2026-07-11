using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;
using ZenCLI.Models;
using ZenCLI.Services;

// Ініціалізація сервісів
var configManager = new ConfigManager();
var config = configManager.LoadConfig();
var blockingService = new BlockingService();
var pomodoroManager = new PomodoroManager();
var pixelArtAnimator = new PixelArtAnimator();

// Налаштування екстреного виходу (Ctrl+C)
Console.CancelKeyPress += (sender, e) =>
{
    Console.ResetColor();
    Console.CursorVisible = true;
    AnsiConsole.MarkupLine("\n\n[bold red]⚠️ Екстрена зупинка! Знімаємо блокування...[/]");
    blockingService.StopBlocking();
    Environment.Exit(0);
};

// Показуємо красиву анімацію завантаження лише один раз при старті
await ShowStartupAnimationAsync();

// Головний цикл програми
while (true)
{
    Console.Clear();
    ShowBanner();

    // Створюємо інтерактивне меню
    var selectedOption = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold springgreen2]Що будемо робити зараз?[/]")
            .PageSize(10)
            .HighlightStyle(new Style(foreground: Color.Black, background: Color.SpringGreen3)) // Колір вибраного пункту
            .AddChoices(new[] {
                "🚀 Почати фокус-сесію (Start)",
                "📋 Створити план завдань (Plan)",
                "⚙️ Налаштування таймера (Settings)",
                "🌐 Список заблокованих сайтів (List)",
                "➕ Додати сайт (Add)",
                "➖ Видалити сайт (Remove)",
                "❌ Вийти (Exit)"
            }));

    // Визначаємо, що робити на основі вибраного пункту
    switch (selectedOption)
    {
        case "🚀 Почати фокус-сесію (Start)":
            await StartFocusSessionAsync(config, blockingService, pomodoroManager);
            break;

        case "📋 Створити план завдань (Plan)":
            await CreatePlanAsync(config, blockingService, pomodoroManager);
            break;

        case "⚙️ Налаштування таймера (Settings)":
            UpdateSettings(config, configManager);
            break;

        case "🌐 Список заблокованих сайтів (List)":
            ShowBlockedSites(config);
            break;

        case "➕ Додати сайт (Add)":
            AddSite(config, configManager);
            break;

        case "➖ Видалити сайт (Remove)":
            RemoveSite(config, configManager);
            break;

        case "❌ Вийти (Exit)":
            AnsiConsole.MarkupLine("[bold springgreen3]Гарного дня! Бережи свій фокус.[/]");
            return; // Вихід з програми
    }

    // Пауза перед поверненням в меню, щоб користувач встиг прочитати результат
    AnsiConsole.MarkupLine("\n[grey]Натисни будь-яку клавішу для повернення в меню...[/]");
    Console.ReadKey(true);
}

// ==========================================
// ЛОГІКА ДОПОМІЖНИХ МЕТОДІВ
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
    // Створюємо заголовок з різними відтінками зеленого
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
    
    // Запускаємо окремий потік для відслідковування натискання клавіш, щоб зупинити сесію
    Task.Run(() =>
    {
        AnsiConsole.MarkupLine("[grey](Натисни 'Q' у будь-який момент щоб зупинити сесію)[/]");
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
        new TextPrompt<int>("Скільки [green]завдань[/] маєш на сьогодні?")
            .Validate(n => n > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Має бути більше 0[/]")));
    
    List<PlanTask> planTasks = new List<PlanTask>();

    for (int i = 1; i <= plans; i++)
    {
        AnsiConsole.MarkupLine($"\n[bold cyan]--- Завдання #{i} ---[/]");
        PlanTask currentTask = new PlanTask();
        
        currentTask.Name = AnsiConsole.Prompt(new TextPrompt<string>("Назва завдання:"));
        currentTask.Minutes = AnsiConsole.Prompt(new TextPrompt<int>("Час на фокус (у хвилинах):").DefaultValue(25));
        currentTask.BreakMinutes = AnsiConsole.Prompt(new TextPrompt<int>("Час на перерву (у хвилинах):").DefaultValue(5));
        
        planTasks.Add(currentTask);
    }

    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.SpringGreen2).Title("[bold yellow]Твій план фокусування[/]");
    table.AddColumn("Завдання");
    table.AddColumn("Фокус");
    table.AddColumn("Перерва");

    foreach (var task in planTasks)
        table.AddRow($"[cyan]{task.Name}[/]", $"[green]{task.Minutes} хв[/]", $"[blue]{task.BreakMinutes} хв[/]");
    
    AnsiConsole.Write(table); 

    if (!AnsiConsole.Confirm("\n🚀 Починаємо цей план прямо зараз?", defaultValue: true))
    {
        AnsiConsole.MarkupLine("[red]🛑 План скасовано.[/]");
        return;
    }

    AnsiConsole.MarkupLine("\n[bold green]🔥 Поїхали! Хай прибуде з тобою фокус.[/]");
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
        await pomoMgr.RunCustomTaskAsync($"🎯 {task.Name} (Фокус)", task.Minutes, cts.Token);
        
        if (cts.Token.IsCancellationRequested) break;
        if (task.BreakMinutes > 0)
            await pomoMgr.RunCustomTaskAsync($"☕ {task.Name} (Перерва)", task.BreakMinutes, cts.Token);
    }

    blockSvc.StopBlocking();
    AnsiConsole.MarkupLine("\n[bold green]🎉 Всі завдання завершено![/]");
}

void UpdateSettings(ZenConfig cfg, ConfigManager cfgMgr)
{
    AnsiConsole.MarkupLine("[bold cyan]⚙️ Налаштування таймера[/]");
    
    cfg.Breaks.PomodoroDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Фокус (хвилини):")
            .DefaultValue(cfg.Breaks.PomodoroDurationMinutes));

    cfg.Breaks.ShortBreakDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Коротка перерва (хвилини):")
            .DefaultValue(cfg.Breaks.ShortBreakDurationMinutes));

    cfg.Breaks.LongBreakDurationMinutes = AnsiConsole.Prompt(
        new TextPrompt<int>("Довга перерва (хвилини):")
            .DefaultValue(cfg.Breaks.LongBreakDurationMinutes));
    
    cfgMgr.SaveConfig(cfg); 
    AnsiConsole.MarkupLine("[bold green]✅ Налаштування успішно збережено![/]");
}

void ShowBlockedSites(ZenConfig cfg)
{
    var sitesTree = new Tree("[red]Заблоковані сайти[/]");
    foreach (var site in cfg.BlockedSites) 
        sitesTree.AddNode($"[yellow]{site}[/]");
        
    AnsiConsole.Write(new Panel(sitesTree).BorderColor(Color.Red).RoundedBorder());
}

void AddSite(ZenConfig cfg, ConfigManager cfgMgr)
{
    string siteToAdd = AnsiConsole.Prompt(new TextPrompt<string>("Введи домен сайту (наприклад, youtube.com):")).ToLower();
    
    if (!cfg.BlockedSites.Contains(siteToAdd))
    {
        cfg.BlockedSites.Add(siteToAdd);
        cfgMgr.SaveConfig(cfg); 
        AnsiConsole.MarkupLine($"[green]✅ Сайт '{siteToAdd}' успішно додано до списку.[/]");
    }
    else 
    {
        AnsiConsole.MarkupLine($"[yellow]⚠️ Сайт '{siteToAdd}' вже є у списку.[/]");
    }
}

void RemoveSite(ZenConfig cfg, ConfigManager cfgMgr)
{
    if (cfg.BlockedSites.Count == 0)
    {
         AnsiConsole.MarkupLine("[yellow]Список сайтів порожній.[/]");
         return;
    }

    // Робимо видалення теж інтерактивним!
    var siteToRemove = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Обери сайт для видалення:")
            .AddChoices(cfg.BlockedSites));

    if (cfg.BlockedSites.Remove(siteToRemove))
    {
        cfgMgr.SaveConfig(cfg); 
        AnsiConsole.MarkupLine($"[green]🗑️ Сайт '{siteToRemove}' розблоковано.[/]");
    }
}