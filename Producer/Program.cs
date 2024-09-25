using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Serilog;
using Serilog.Settings.Configuration;
using Zema.Contracts;

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
            context.HostingEnvironment.EnvironmentName.ToLower(),
            includeNamespace: true);
        x.SetEndpointNameFormatter(formatter);
        
        // x.AddConsumer<PrioritizedMessageConsumer>();
        
        x.UsingAmazonSqs(
            (registrationContext, configurator) =>
            {
                configurator.LocalstackHost();

                configurator.MessageTopology.SetEntityNameFormatter(formatter);

                var topicName = configurator.MessageTopology.GetMessageTopology<PrioritizedMessage>().EntityName;
                var queueName = Endpoints.PrioritizedMessageName;
                configurator.ReceiveEndpoint(queueName,
                    configEndpoint =>
                    {
                        configEndpoint.Subscribe(topicName);

                        configEndpoint.ConfigureConsumeTopology = false;
                    });
                configurator.ConfigureEndpoints(registrationContext);
                configurator.DeployTopologyOnly = true;
            });
    });
}