using System.Reflection;
using MassTransit;
using MassTransit.AmazonSqsTransport.Configuration;
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

                configurator.ReceiveEndpoint(new ConsumerEndpointDefinition<PrioritizedMessageConsumer>(
                        new EndpointSettings<IEndpointDefinition<PrioritizedMessageConsumer>>()),
                    registrationContext.EndpointNameFormatter,
                    configEndpoint =>
                    {
                        configEndpoint.ConfigureConsumer<PrioritizedMessageConsumer>(registrationContext);
                        
                        configEndpoint.Subscribe<PrioritizedMessage>();
                        
                        configEndpoint.ConfigureConsumeTopology = false;
                    });
                configurator.ConfigureEndpoints(registrationContext);
            });
    });
}