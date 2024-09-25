using MassTransit;
using Microsoft.Extensions.Logging;
using Zema.Contracts;

namespace Consumer;

public class PrioritizedMessageConsumer(
    ILogger<PrioritizedMessageConsumer> logger)
    : IConsumer<ImportantPrioritizedMessage>,
        IConsumer<StandardPrioritizedMessage>
{
    public Task Consume(ConsumeContext<ImportantPrioritizedMessage> context)
    {
        // if (context.Message.Name == "HamletHakobyan")
        // {
        //     throw new Exception("Exception thrown");
        // }
        logger.LogInformation("ImportantPrioritizedMessageConsumer: {message}", context.Message.Name);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<StandardPrioritizedMessage> context)
    {
        logger.LogInformation("StandardPrioritizedMessageConsumer: {message}", context.Message.Name);
        return Task.CompletedTask;
    }
}
