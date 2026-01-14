using System;
using System.Diagnostics;

namespace Aspire.Hosting.ApplicationModel;

[DebuggerDisplay("Type = {GetType().Name,nq}")]
public class AkkaNodeAnnotation : IResourceAnnotation
{
    public AkkaNodeAnnotation(SeedNodeEnvNameOptions? options = null)
    {
        Options = options;
    }

    public AkkaNodeAnnotation(Action<SeedNodeEnvNameOptions>? configure = null)
    {
        var options = new SeedNodeEnvNameOptions();
        configure?.Invoke(options);
        Options = options;
    }

    public SeedNodeEnvNameOptions? Options { get; }
}