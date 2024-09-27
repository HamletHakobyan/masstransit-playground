using MassTransit;
using MassTransit.Middleware;
using Zema.IntegrationMessages;

namespace Consumer;

public class NotificationStateChangedConsumer : IConsumer<NotificationStateChanged>
{
    public Task Consume(ConsumeContext<NotificationStateChanged> context)
    {
        return Task.CompletedTask;
    }
}

public class NotificationStateChangedConsumerDefinition :
    ConsumerDefinition<NotificationStateChangedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<NotificationStateChangedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseRawJsonDeserializer(
            RawSerializerOptions.AnyMessageType,
            isDefault:true);
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
    }
}

