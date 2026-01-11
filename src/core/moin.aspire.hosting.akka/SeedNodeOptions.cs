using System;

namespace Aspire.Hosting;

public class SeedNodeOptions
{
    public EnvValueMode Mode { get; set; }
    public Func<string> SeedNodeEnvNameConfigure { get; set; }
}