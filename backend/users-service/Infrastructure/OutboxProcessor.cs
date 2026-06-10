using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UsersService.Domain;

namespace UsersService.Infrastructure;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedAt == null)
                        .OrderBy(m => m.CreatedAt)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        var messageType = Type.GetType(message.Type);
                        if (messageType == null)
                        {
                            _logger.LogError("Unknown message type: {MessageType}", message.Type);
                            continue;
                        }

                        var messageData = JsonConvert.DeserializeObject(message.Data, messageType);
                        if (messageData == null)
                        {
                            _logger.LogError("Failed to deserialize message data: {MessageData}", message.Data);
                            continue;
                        }

                        await publisher.Publish(messageData, stoppingToken);

                        message.ProcessedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}