using MassTransit;
using MassTransit.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Playground;
using Playground.Persistence;
using Serilog;
using Serilog.Settings.Configuration;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        ConfigureMassTransit(services, context);

        services.AddHostedService<WorkerService>();
        services.AddScoped<IScopedWorkerService, ScopedWorkerService>();
    })
    .UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom
            .Configuration(
                context.Configuration,
                new ConfigurationReaderOptions { SectionName = "Serilog" });
    })
    .Build()
    .RunAsync();

static void ConfigureMassTransit(IServiceCollection services, HostBuilderContext context)
{
    services.AddMassTransit(x =>
    {
        var formatter = new KebabCaseFormatter(
            $"{context.HostingEnvironment.EnvironmentName.ToLower()}",
            includeNamespace: true);
        
        x.SetEndpointNameFormatter(formatter);
        
        x.AddConsumer<PrioritizedMessageConsumer>();
        
        x.UsingAmazonSqs(
            (ctx, configurator) =>
            {
                configurator.MessageTopology.SetEntityNameFormatter(formatter);

                var topicName = configurator.MessageTopology.EntityNameFormatter.FormatEntityName<PrioritizedMessage>();
                
                string[] priorities = ["standard", "low", "high"];
                foreach (var priority in priorities)
                {
                    var queueName = $"{ctx.EndpointNameFormatter.Consumer<PrioritizedMessageConsumer>()}-{priority}";
                    configurator.ReceiveEndpoint(queueName,
                        configEndpoint =>
                        {
                            configEndpoint.ConfigureConsumeTopology = false;
                            configEndpoint.Subscribe(topicName,
                                subscriptionConfigurator =>
                                {
                                    subscriptionConfigurator.TopicSubscriptionAttributes["FilterPolicy"] =
                                        $"{{\"message\": {{\"priority\": [\"{priority}\"]}}}}";
                                    subscriptionConfigurator.TopicSubscriptionAttributes["FilterPolicyScope"] = "MessageBody";
                                });
                        });
                    
                }

                configurator.LocalstackHost();
                configurator.ConfigureEndpoints(ctx);
            });
    });
}

public class KebabCaseFormatter :
    KebabCaseEndpointNameFormatter,
    IEntityNameFormatter
{
    private readonly string _topicSuffix;
    private readonly string _queueSuffix;

    public KebabCaseFormatter(
        string prefix,
        string topicSuffix = "topic",
        string queueSuffix = "queue",
        bool includeNamespace = false) :
        base(prefix, includeNamespace)
    {
        _topicSuffix = topicSuffix;
        _queueSuffix = queueSuffix;
    }

    public string FormatEntityName<T>()
    {
        var messageName = GetMessageName(typeof(T));
        return string.IsNullOrWhiteSpace(_topicSuffix)
            ? messageName
            : $"{messageName}-{_topicSuffix}";
    }

    public override string Consumer<T>()
    {
        var consumerName = base.Consumer<T>();
        return string.IsNullOrWhiteSpace(_queueSuffix)
                ? consumerName
            : $"{consumerName}-{_queueSuffix}";
    }
}