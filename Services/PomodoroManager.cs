using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace ZenCLI.Services;

public class PomodoroManager
{
    private int _sessionCount = 0;

    public async Task StartTimerAsync(int minutes, int shortBreakMinutes, int longBreakMinutes, CancellationToken token)
    {
        AnsiConsole.MarkupLine($"\n[bold green]🚀 Starting focus-session: {minutes} minutes. Let's go![/]");
        await RunProgressBarAsync("🎯 Focus", "green", minutes, token);

        if (token.IsCancellationRequested) return;

        _sessionCount++;
        Console.WriteLine("\a"); 

        bool isLongSession = (_sessionCount % 4 == 0);
        int currentBreakMinutes = isLongSession ? longBreakMinutes : shortBreakMinutes;
        string breakIcon = isLongSession ? "🛋️" : "☕";
        string breakName = isLongSession ? "Long break" : "Short break";

        AnsiConsole.MarkupLine($"\n[bold blue]{breakIcon} {breakName} time! {currentBreakMinutes} minutes. (Break #{_sessionCount})[/]");
        
        await RunProgressBarAsync($"{breakIcon} {breakName}", "blue", currentBreakMinutes, token);

        if (!token.IsCancellationRequested)
        {
            Console.WriteLine("\a"); 
            AnsiConsole.MarkupLine("\n[bold green]🧘 Break is over! Ready for next session?[/]\n");
        }
    }
    
    public async Task RunCustomTaskAsync(string taskName, int minutes, CancellationToken token)
    {
        bool isBreak = taskName.ToLower().Contains("перерва") || taskName.ToLower().Contains("break");
        string color = isBreak ? "blue" : "green";
        string icon = isBreak ? "☕" : "🚀";

        AnsiConsole.MarkupLine($"\n[bold {color}]{icon} Current task: {taskName} for {minutes} minutes.[/]");
        
        await RunProgressBarAsync(taskName, color, minutes, token);

        if (!token.IsCancellationRequested)
        {
            Console.WriteLine("\a");
            AnsiConsole.MarkupLine($"\n[bold {color}]✅ '{taskName}' finished![/]");
        }
    }

    private async Task RunProgressBarAsync(string description, string color, int minutes, CancellationToken token)
    {
        int totalSeconds = minutes * 60;

        await AnsiConsole.Progress()
            .AutoClear(false) 
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),                 
                new ProgressBarColumn(),                     
                new PercentageColumn(),                      
                new RemainingTimeColumn(),                   
                new SpinnerColumn(Spinner.Known.Dots),       
            })
            .StartAsync(async ctx =>
            {
                var progressTask = ctx.AddTask($"[{color}]{description}[/]", maxValue: totalSeconds);

                while (!progressTask.IsFinished && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000);       
                    progressTask.Increment(1);    
                }
                
                if (token.IsCancellationRequested)
                {
                    AnsiConsole.MarkupLine("\n[bold red]🛑 Stopped early.[/]");
                }
            });
    }
}