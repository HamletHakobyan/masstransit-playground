using MassTransit;
using Microsoft.Extensions.Logging;
using Zema.Contracts;

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
        var sendEndpoint =
            await _bus.GetPublishSendEndpoint<PrioritizedMessage>();
        
        // while (!stoppingToken.IsCancellationRequested)
        // {
            await Task.Delay(5000, stoppingToken);
            await sendEndpoint.Send<ImportantPrioritizedMessage>(new { Name = "important message" }, stoppingToken);
            await sendEndpoint.Send<StandardPrioritizedMessage>(new { Name = "standard message" }, stoppingToken);
        // }
    }
}

public interface IScopedWorkerService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}