using System;
using System.Diagnostics;

namespace Aspire.Hosting.ApplicationModel;

[DebuggerDisplay("Type = {GetType().Name,nq}")]
public class AkkaNodeAnnotation : IResourceAnnotation
{
    public AkkaNodeAnnotation(Func<string> seedNodeEnvConfigure, EnvValueMode mode)
    {
        SeedNodeEnvConfigure = seedNodeEnvConfigure;
        Mode = mode;
    }

    public Func<string> SeedNodeEnvConfigure { get; }
    public EnvValueMode Mode { get; }
}