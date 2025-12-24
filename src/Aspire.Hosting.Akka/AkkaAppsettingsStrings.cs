namespace Aspire.Hosting.Akka;

public static class AkkaAppsettingsStrings
{
    private const string EnvSeparator = "__";
    internal const string Separator = ".";

    internal static string GetAkkaString(string variable) =>
        $"akka.{variable}".Replace(Separator, EnvSeparator).ToLower();

    internal static string GetAkkaActorString(string variable)
        => GetAkkaString($"actor.{variable}");

    internal static string GetAkkaRemoteString(string variable)
        => GetAkkaString($"remote.{variable}");

    public static string GetAkkaClusterString(string variable)
        => GetAkkaString($"cluster.{variable}");


    internal static string GetAkkaRemoteDotNettyString(string variable)
        => GetAkkaRemoteString($"dot-netty.{variable}");

    internal static string GetAkkaRemoteDotNettyTcpString(string variable)
        => GetAkkaRemoteDotNettyString($"tcp.{variable}");

    internal static string GetAkkaClusterSeedNodesString()
        => GetAkkaClusterString("seed-nodes");
    
    internal static string GetAkkaClusterRolesString()
        => GetAkkaClusterString("roles");

    internal static string GetAkkaRemoteDotNettyTcpPortString()
        => GetAkkaRemoteDotNettyTcpString("port");
    

    internal static string GetAkkaRemoteDotNettyTcpPublicPortString()
        => GetAkkaRemoteDotNettyTcpString("public-port");

    internal static string GetAkkaRemoteDotNettyTcpHostnameString()
        => GetAkkaRemoteDotNettyTcpString("hostname");

    internal static string GetAkkaRemoteDotNettyTcpPublicHostnameString()
        => GetAkkaRemoteDotNettyTcpString("public-hostname");
}