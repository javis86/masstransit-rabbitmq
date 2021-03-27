using System;
using System.Threading.Tasks;
using Consumer.App.Components;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Mt.Contracts;

//using Microsoft.Extensions.Logging;

namespace Consumer.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(configurator =>
                    {
                        configurator.UsingRabbitMq((context, factoryConfigurator) =>
                        {
                            factoryConfigurator.ReceiveEndpoint("RoutingProxy", endpointConfigurator =>
                            {
                                var activityContextConfiguration = new ActivityContextConfiguration(){};
                                var activityContextConfiguration2 = new ActivityContextConfiguration(){};
                                
                                var requestProxy = new RequestProxy(activityContextConfiguration, activityContextConfiguration2);
                                var responseProxy = new ResponseProxy();
                                
                                endpointConfigurator.Instance(requestProxy);
                                endpointConfigurator.Instance(responseProxy);
                            });
                            
                            factoryConfigurator.ConfigureEndpoints(context);
                        });

                        configurator.AddRequestClient<RequestRouting>();
                        configurator.AddConsumer<OrderProcessedConsumer>();
                        configurator.AddActivitiesFromNamespaceContaining<AllocatorActivity>();
                    });

                    services.AddHostedService<MassTransitHostedService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                })
                .RunConsoleAsync();
        }
        
        class RequestProxy :
            RoutingSlipRequestProxy<RequestRouting>
        {
            readonly ActivityContextConfiguration _secondActivity;
            readonly ActivityContextConfiguration _testActivity;

            public RequestProxy(ActivityContextConfiguration testActivity, ActivityContextConfiguration secondActivity)
            {
                _testActivity = testActivity;
                _secondActivity = secondActivity;
            }

            protected override async Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<RequestRouting> request)
            {
                builder.AddActivity("Allocator", new Uri("rabbitmq://localhost/Allocator_execute"), new {OrderId = request.Message.OrderId});
                builder.AddActivity("PaymentClientValidation", new Uri("rabbitmq://localhost/PaymentClientValidation_execute"), new {OrderId = request.Message.OrderId});
            }
        }

        class ResponseProxy :
            RoutingSlipResponseProxy<RequestRouting, ResponseRouting>
        {
            protected override Task<ResponseRouting> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, RequestRouting requestRouting)
            {
                return Task.FromResult(new ResponseRouting());
            }
        }
    }

    internal class ActivityContextConfiguration
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
    }
}
