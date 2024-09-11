using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Playground;

public sealed class WorkerService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var scopedWorkerService = scope.ServiceProvider.GetRequiredService<IScopedWorkerService>();
        
        await scopedWorkerService.ExecuteAsync(stoppingToken);
    }
}