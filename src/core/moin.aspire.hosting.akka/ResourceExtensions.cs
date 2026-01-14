using System.Linq;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace moin.aspire.hosting.akka;

public static class ResourceExtensions
{
    public static SeedNodeEnvNameOptions GetSeedNodeEnvNameOptions(this IResource resource, SeedNodeEnvNameOptions @default)
    {
        if (!resource.TryGetAnnotationsOfType<AkkaNodeAnnotation>(out var result)) return @default;
        var annotation = result.Single();
        return annotation.Options ?? @default;
    }
}