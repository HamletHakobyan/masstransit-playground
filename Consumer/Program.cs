using Common;
using Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Settings.Configuration;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        ConfigureMassTransit(services, context);
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
        
        x.AddConsumer<PrioritizedMessageConsumer>()
            .Endpoint(configurator => configurator.Name = "development-contracts-prioritized-message-queue");
        
        x.UsingAmazonSqs(
            (registrationContext, configurator) =>
            {
                configurator.LocalstackHost();

                configurator.ConfigureEndpoints(registrationContext);
            });
    });
}