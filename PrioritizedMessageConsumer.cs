using MassTransit;
using Microsoft.Extensions.Logging;

namespace Playground;

public class PrioritizedMessageConsumer(ILogger<PrioritizedMessageConsumer> logger)
    : IConsumer<PrioritizedMessage>
{
    public Task Consume(ConsumeContext<PrioritizedMessage> context)
    {
        // if (context.Message.Name == "HamletHakobyan")
        // {
        //     throw new Exception("Exception thrown");
        // }
        logger.LogInformation("PrioritizedMessageConsumer: {message}", context.Message.Name);
        return Task.CompletedTask;
    }
}

public class PrioritizedMessageDefinition : ConsumerDefinition<PrioritizedMessageConsumer>
{
    public PrioritizedMessageDefinition()
    {
    }
}
