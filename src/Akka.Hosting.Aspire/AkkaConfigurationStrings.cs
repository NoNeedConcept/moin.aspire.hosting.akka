namespace Akka.Hosting.Aspire;

public static class AkkaConfigurationStrings
{
    private const string EnvSeparator = ":";
    internal const string Separator = ".";

    private static string GetAkkaString(string variable) =>
        $"akka.{variable}".Replace(Separator, EnvSeparator).ToLower();

    private static string Get(string prefix, string? variable = null)
        => string.IsNullOrEmpty(variable) ? prefix : $"{prefix}.{variable}";

    public static string GetAkkaActorString(string? variable = null)
        => GetAkkaString(Get("actor", variable));

    public static string GetAkkaRemoteString(string? variable = null)
        => GetAkkaString(Get("remote", variable));

    public static string GetAkkaClusterString(string? variable = null)
        => GetAkkaString(Get("cluster", variable));

    public static string GetAkkaRemoteDotNettyString(string? variable = null)
        => GetAkkaRemoteString(Get("dot-netty", variable));

    public static string GetAkkaRemoteDotNettyTcpString(string? variable = null)
        => GetAkkaRemoteDotNettyString(Get("tcp", variable));

    public static string GetAkkaClusterSeedNodesString()
        => GetAkkaClusterString("seed-nodes");

    public static string GetAkkaClusterRolesString()
        => GetAkkaClusterString("roles");

    public static string GetAkkaRemoteDotNettyTcpPortString()
        => GetAkkaRemoteDotNettyTcpString("port");
    
    public static string GetAkkaRemoteDotNettyTcpPublicPortString()
        => GetAkkaRemoteDotNettyTcpString("public-port");

    public static string GetAkkaRemoteDotNettyTcpHostnameString()
        => GetAkkaRemoteDotNettyTcpString("hostname");

    public static string GetAkkaRemoteDotNettyTcpPublicHostnameString()
        => GetAkkaRemoteDotNettyTcpString("public-hostname");
}