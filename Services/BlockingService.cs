namespace ZenCLI.Services;

public class BlockingService
{
    private const string HostFilePath = "/etc/hosts";
    private const string LocalhostIp = "127.0.0.1";
    private readonly string _backupFilePath;
    
    public BlockingService()
    {
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _backupFilePath = Path.Combine(homeDir, ".zencli", "hosts.backup");
    }

    public void StartBlocking(List<string> sitesToBlock)
    {
        if (sitesToBlock.Count == 0) return;

        try
        {
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
            Console.WriteLine("🔒 Sites have been blocked");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("❌ Error: Administrator access required (run with sudo)");
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
            
            Console.WriteLine("🔓 Blocks removed. Internet is free!");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("❌ Error: Administrator access required to unblock (run with sudo)");
        }
    }
    
    private void FlushDnsCache()
    {
        System.Diagnostics.Process.Start("dscacheutil", "-flushcache")?.WaitForExit();
        System.Diagnostics.Process.Start("killall", "-HUP mDNSResponder")?.WaitForExit();
    }
}