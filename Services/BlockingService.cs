namespace ZenCLI.Services;

public class BlockingService
{
    private const string HostFilePath = "/etc/hosts";
    private const string LocalhostIp = "127.0.0.1";
    private readonly string _backupFilePath;
    
    public BlockingService()
    {
        string sudoUser = Environment.GetEnvironmentVariable("SUDO_USER") ?? Environment.GetEnvironmentVariable("USER") ?? "unknown";
        string homeDir = Path.Combine("/Users", sudoUser);
        _backupFilePath = Path.Combine(homeDir, ".zencli", "hosts.backup");
    }

    public void StartBlocking(List<string> sitesToBlock)
    {
        if (sitesToBlock.Count == 0) return;

        try
        {
            string zenDir = Path.GetDirectoryName(_backupFilePath)!;
            if (!Directory.Exists(zenDir))
                Directory.CreateDirectory(zenDir);

            string originalHosts = File.ReadAllText(HostFilePath);
            File.WriteAllText(_backupFilePath, originalHosts);

            using (StreamWriter sw = File.AppendText(HostFilePath))
            {
                sw.WriteLine();
                sw.WriteLine("# === ZENCLI BLOCK START ===");
                foreach (string s in sitesToBlock)
                {
                    sw.WriteLine($"{LocalhostIp} {s}");
                    sw.WriteLine($"{LocalhostIp} www.{s}");
                }
                sw.WriteLine("# === ZENCLI BLOCK END ===");
            }
            FlushDnsCache();
            Console.WriteLine("\uD83D\uDD12 Sites have been blocked");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\u274C Error during blocking: {ex.Message}");
        }
    }

    public void StopBlocking()
    {
        if (!File.Exists(_backupFilePath)) return;

        try
        {
            string cleanHosts = File.ReadAllText(_backupFilePath);
            File.WriteAllText(HostFilePath, cleanHosts);
            File.Delete(_backupFilePath);
            FlushDnsCache();
            Console.WriteLine("\uD83D\uDD13 Blocks removed. Internet is free!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\u274C Error during unblocking: {ex.Message}");
        }
    }
    
    private void FlushDnsCache()
    {
        System.Diagnostics.Process.Start("dscacheutil", "-flushcache")?.WaitForExit();
        System.Diagnostics.Process.Start("killall", "-HUP mDNSResponder")?.WaitForExit();
    }
}