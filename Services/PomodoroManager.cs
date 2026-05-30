namespace ZenCLI.Services;

public class PomodoroManager
{
    public async Task StartTimerAsync(int minutes, int breakMinutes, CancellationToken token)
    {
        int totalSeconds = minutes * 60;
        Console.WriteLine($"\n Starting focus-session: {minutes} minutes. Let's go!");
        Console.CursorVisible = false;

        for (int i = totalSeconds; i >= 0; i--)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("\n\n🛑 Session stopped.");
                break;
            }
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(i);

            if (i > totalSeconds / 2) 
                Console.ForegroundColor = ConsoleColor.Green;
            else if (i > 60) 
                Console.ForegroundColor = ConsoleColor.Yellow;
            else 
                Console.ForegroundColor = ConsoleColor.Red;
            
            Console.Write($"\r Left: {timeSpan:mm\\:ss}     ");

            await Task.Delay(1000);
        }
        
        Console.ResetColor();
        Console.CursorVisible = true;
        
        Console.WriteLine("\a\n\n🎉 Session is end! You can have rest now!");
        
        Console.WriteLine($"\n☕ Break time! {breakMinutes} minutes.");
        Console.CursorVisible = false;
        
        int breakSeconds = breakMinutes * 60;
        for (int i = breakSeconds; i >= 0; i--)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("\n\n🛑 Session stopped.");
                break;
            }
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(i);

            if (i > breakSeconds / 2) 
                Console.ForegroundColor = ConsoleColor.Green;
            else if (i > 60) 
                Console.ForegroundColor = ConsoleColor.Yellow;
            else 
                Console.ForegroundColor = ConsoleColor.Red;
            
            Console.Write($"\r Left: {timeSpan:mm\\:ss}     ");

            await Task.Delay(1000);
        }
        
        Console.ResetColor();
        Console.CursorVisible = true;
        Console.WriteLine("\a\n\n🧘 Break is over! Ready for next session?");
    }
}

