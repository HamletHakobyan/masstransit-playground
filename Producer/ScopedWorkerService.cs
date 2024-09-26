using MassTransit;
using Microsoft.Extensions.Logging;
using Zema.Contracts;
using Zema.IntegrationMessages;

namespace Producer;

public class ScopedWorkerService : IScopedWorkerService
{
    private readonly IBus _bus;
    private readonly ILogger<ScopedWorkerService> _logger;

    public ScopedWorkerService(
        IBus bus,
        ILogger<ScopedWorkerService> logger)
    {
        _bus = bus;
        _logger = logger;
    }
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var emailNotificationPrepared = new EmailNotificationPrepared(
            Guid.NewGuid(),
            "high",
            "unknown@loinloads.com",
            "test body",
            "test subject",
            new List<StorageFile>());
        // while (!stoppingToken.IsCancellationRequested)
        // {
            await Task.Delay(5000, stoppingToken);
            await _bus.Publish(emailNotificationPrepared, stoppingToken);
        // }
    }
}

public interface IScopedWorkerService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
