using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis.Availability;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace ByMyPC.Metrics
{
    public class DependencyHealthMetrics : BackgroundService
    {
        private readonly HealthCheckService healthCheckService;
        private readonly Meter meter = new("ByMyPc.Health");

        private readonly ConcurrentDictionary<string, int> _health = new()
        {
            ["ByMyPC"] = -1,
            ["Redis"] = -1,
            ["Kafka"] = -1,
        };

        public DependencyHealthMetrics(HealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
            meter.CreateObservableCounter(
                    "bymypc_dependency_health",
                    ObserveHealth,
                    description: "Dependency health: 1 = healthy, 0 = unhealthy, -1 = unknown"
                );
        }

        private IEnumerable<Measurement<int>> ObserveHealth()
        {
            return _health.Select(item =>
                new Measurement<int>(
                    item.Value,
                    new KeyValuePair<string, object?>(
                        "dependency", item.Key
                    )
                ));
        }
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var report = await healthCheckService.CheckHealthAsync(
                        stoppingToken);

                    foreach (var check in report.Entries)
                    {
                        _health[check.Key] =
                            check.Value.Status == HealthStatus.Healthy ? 1 : 0;
                    }
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception)
                {
                    
                }
                try
                {
                    await Task.Delay(
                   TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    
                }
               
            }
        }
        public override void Dispose()
        {
            meter.Dispose();
            base.Dispose();
        }
    }
}
