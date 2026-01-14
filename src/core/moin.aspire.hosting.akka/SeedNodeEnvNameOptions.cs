using moin.aspire.hosting.akka;

namespace Aspire.Hosting;

public class SeedNodeEnvNameOptions
{
    public SeedNodeEnvNameOptions(string seedNodeEnvName = AkkaDefaultEnvName.SeedNodeEnvName, EnvValueMode mode = EnvValueMode.Array)
    {
        SeedNodeEnvName = seedNodeEnvName;
        Mode = mode;
    }
    
    public EnvValueMode Mode { get; set; } = EnvValueMode.Array;
    public string SeedNodeEnvName { get; set; } = AkkaDefaultEnvName.SeedNodeEnvName;
}