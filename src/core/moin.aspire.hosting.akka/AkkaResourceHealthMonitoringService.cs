using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aspire.Hosting.Health;

internal sealed class AkkaResourceHealthMonitoringService(
    ResourceNotificationService notificationService,
    HealthCheckService healthCheckService,
    DistributedApplicationModel applicationModel,
    ILogger<AkkaResourceHealthMonitoringService> logger) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(_initialDelay, stoppingToken);

        var akkaResources = applicationModel.Resources.OfType<AkkaResource>().ToArray();
        
        if (akkaResources.Length == 0)
        {
            return;
        }

        logger.LogInformation("Starting health monitoring for {Count} Akka cluster(s)", akkaResources.Length);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var akkaResource in akkaResources)
            {
                await UpdateHealthStatusAsync(akkaResource, stoppingToken);
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task UpdateHealthStatusAsync(AkkaResource akkaResource, CancellationToken cancellationToken)
    {
        try
        {
            var healthCheckName = $"akka-cluster-{akkaResource.SystemName}";
            
            var healthReport = await healthCheckService.CheckHealthAsync(
                predicate: check => check.Name == healthCheckName,
                cancellationToken: cancellationToken);

            if (healthReport.Entries.TryGetValue(healthCheckName, out var entry))
            {
                await notificationService.PublishUpdateAsync(akkaResource, snapshot =>
                    snapshot with 
                    { 
                        State = entry.Status switch
                        {
                            HealthStatus.Healthy => new ResourceStateSnapshot("Running", KnownResourceStateStyles.Success),
                            HealthStatus.Degraded => new ResourceStateSnapshot("Degraded", KnownResourceStateStyles.Warn),
                            HealthStatus.Unhealthy => new ResourceStateSnapshot("Unhealthy", KnownResourceStateStyles.Error),
                            _ => snapshot.State
                        }
                    });
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update health status for Akka cluster {SystemName}", 
                akkaResource.SystemName);
        }
    }
}