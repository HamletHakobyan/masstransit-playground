using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;

namespace Playground;

public static class MassTransitExtensions
{
    
    public static void LocalstackHost(
        this IAmazonSqsBusFactoryConfigurator configurator,
        string scope,
        bool scopeTopics = false)
    {
        configurator.Host(new Uri("amazonsqs://localhost:4566"), h =>
        {
            h.AccessKey("admin");
            h.SecretKey("admin");
            
            h.Scope(scope, scopeTopics);

            h.Config(new AmazonSQSConfig { ServiceURL = "http://localhost:4566" });
            h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://localhost:4566" });
        });
    }
}