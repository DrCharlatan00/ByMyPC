using ByMyPc.Postgresql;
using ByMyPC.Services.Kafka;
using Microsoft.EntityFrameworkCore;

namespace ByMyPC.Services.BackgroudServices
{
    public class OutboxBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<OutboxBackgroundService> logger
        ) : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<OutboxBackgroundService> logger = logger;

        private const short SizeRead = 50;
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

#if DEBUG
            logger.LogInformation("Start Outbox Service Works");
#endif
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProccessBatchAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex) {
                    logger.LogError(ex, "Outbox send message if failed");
                }
                try
                {
                    await Task.Delay(PollingInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                   
                }

            }

        }

        private async Task ProccessBatchAsync(CancellationToken cancellationToken) {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var context =  scope.ServiceProvider.GetRequiredService<PgContext>();

            var producerKafka = scope.ServiceProvider.GetRequiredService<IKafkaProducerService>();

            var Messages = await context.OutboxMessages.Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .Take(SizeRead)
                .ToListAsync(cancellationToken);

            foreach (var item in Messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await producerKafka.SendMessageAsync(
                            item.Topic,
                            item.AggregateId,
                            item.Payload
                        );
                    item.ProcessedAt = DateTime.UtcNow;

                    await context.SaveChangesAsync(cancellationToken);

#if DEBUG
                    logger.LogInformation("outbox message is send to kafka id = {messageid}", item.Id);
#endif


                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex) {
                    item.RetryCount++;
                    logger.LogError(ex, "Failed send OutBox message id = {idmess}",item.Id);
                    await context.SaveChangesAsync(cancellationToken);
                }
                
            }

        }
    }
}
