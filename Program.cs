using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Playground;
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
            (registrationContext, configurator) =>
            {
                configurator.LocalstackHost();

                configurator.MessageTopology.SetEntityNameFormatter(formatter);

                string[] priorities = ["standard", "low", "high"];
                foreach (var priority in priorities)
                {
                    var queueName =
                        $"{registrationContext.EndpointNameFormatter.Consumer<PrioritizedMessageConsumer>()}-{priority}";
                    configurator.ReceiveEndpoint(queueName,
                        configEndpoint =>
                        {
                            configEndpoint.Subscribe<PrioritizedMessage>(
                                subscriptionConfigurator =>
                                {
                                    subscriptionConfigurator.TopicSubscriptionAttributes["FilterPolicy"] =
                                        $"{{\"message\": {{\"priority\": [\"{priority}\"]}}}}";
                                    subscriptionConfigurator.TopicSubscriptionAttributes["FilterPolicyScope"] = "MessageBody";
                                });
                            
                            configEndpoint.ConfigureConsumer<PrioritizedMessageConsumer>(registrationContext);

                            configEndpoint.ConfigureConsumeTopology = false;
                        });
                    
                }

                configurator.ConfigureEndpoints(registrationContext);
            });
    });
}