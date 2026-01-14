using System;
using Aspire.Hosting;

namespace moin.aspire.hosting.akka;

public class AkkaEnvNameBuilder
{
    public AkkaEnvNameBuilder()
    {
        SeedNodeEnvNameOptions = new SeedNodeEnvNameOptions();
        HostnameEnvName = AkkaDefaultEnvName.HostnameEnvName;
        PortEnvName = AkkaDefaultEnvName.PortEnvName;
    }

    internal AkkaEnvNameBuilder(SeedNodeEnvNameOptions options, string hostnameEnvName, string portEnvName)
    {
        SeedNodeEnvNameOptions = options;
        HostnameEnvName = hostnameEnvName;
        PortEnvName = portEnvName;
    }

    public SeedNodeEnvNameOptions SeedNodeEnvNameOptions { get; private set; }
    public string HostnameEnvName { get; private set; }
    public string PortEnvName { get; private set; }

    public AkkaEnvNameBuilder WithSeedNodeEnvName(Action<SeedNodeEnvNameOptions> configure)
    {
        var options = new SeedNodeEnvNameOptions();
        configure.Invoke(options);
        SeedNodeEnvNameOptions = options;
        return this;
    }

    public AkkaEnvNameBuilder WithHostnameEnvName(string hostnameEnvName)
    {
        HostnameEnvName = hostnameEnvName;
        return this;
    }

    public AkkaEnvNameBuilder WithPortEnvName(string portEnvName)
    {
        PortEnvName = portEnvName;
        return this;
    }
}