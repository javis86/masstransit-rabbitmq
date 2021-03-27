using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentValidation.App.Components;

namespace PaymentValidation.App
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
                            factoryConfigurator.Host(new Uri("rabbitmq://localhost"),
                                c =>
                                {
                                    c.Username("guest");
                                    c.Password("guest");
                                });
                        });

                        configurator.AddActivitiesFromNamespaceContaining<PaymentClientValidationActivity>();
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
