using System.Diagnostics;

namespace Aspire.Hosting.ApplicationModel;

[DebuggerDisplay("Type = {GetType().Name,nq}")]
public class AkkaNodeAnnotation : IResourceAnnotation
{
    public AkkaNodeAnnotation()
    {
    }
}