using MassTransit;
using Microsoft.Extensions.Logging;

namespace Playground;

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
        await sendEndpoint.Send<PrioritizedMessage>(new("SomeName", "standard"), stoppingToken);
        await sendEndpoint.Send<PrioritizedMessage>(new("someOtherName", "high"), stoppingToken);
    }
}

public interface IScopedWorkerService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}