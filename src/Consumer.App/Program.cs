using System;
using System.Threading.Tasks;
using Consumer.App.Components;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

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
                            factoryConfigurator.ConfigureEndpoints(context);
                        });

                        configurator.AddConsumer<OrderProcessedConsumer>();
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
    }
}
