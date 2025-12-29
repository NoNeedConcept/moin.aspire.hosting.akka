using System.Threading;
using System.Threading.Tasks;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting.Health;

public class AkkaAggregateHealthCheck : IHealthCheck
{
    private readonly ResourceNotificationService _notificationService;
    private readonly AkkaResource _akkaResource;

    public AkkaAggregateHealthCheck(ResourceNotificationService notificationService, AkkaResource akkaResource)
    {
        _notificationService = notificationService;
        _akkaResource = akkaResource;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var healthyCount = 0;
        var totalCount = _akkaResource.Nodes.Length;

        foreach (var node in _akkaResource.Nodes)
        {
            var resourceEvent = await _notificationService.WaitForResourceAsync(
                node.Name,
                x => x.Snapshot.HealthStatus is not null,
                cancellationToken: cancellationToken);

            if (resourceEvent.Snapshot.HealthStatus == HealthStatus.Healthy)
            {
                healthyCount++;
            }
        }

        if (healthyCount == 0)
        {
            return HealthCheckResult.Unhealthy(
                $"All Akka nodes are unreachable (0/{totalCount})");
        }

        if (healthyCount < totalCount)
        {
            return HealthCheckResult.Degraded(
                $"Some Akka nodes are unreachable ({healthyCount}/{totalCount})");
        }

        return HealthCheckResult.Healthy(
            $"All Akka nodes are reachable ({healthyCount}/{totalCount})");
    }
}