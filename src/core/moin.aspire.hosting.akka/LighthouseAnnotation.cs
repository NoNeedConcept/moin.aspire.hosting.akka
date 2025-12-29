using System.Diagnostics;

namespace Aspire.Hosting.ApplicationModel;

[DebuggerDisplay("Type = {GetType().Name,nq}")]
public class LighthouseAnnotation : IResourceAnnotation
{
    public LighthouseAnnotation()
    {
    }
}