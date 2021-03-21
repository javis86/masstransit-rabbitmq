using System;
using System.Threading.Tasks;
using Mt.Contracts;
using MassTransit;
using MassTransit.Courier;

namespace Consumer.App.Components
{
    public class OrderProcessedConsumer : IConsumer<IProcessOrder>
    {
        private readonly IBus _bus;

        public OrderProcessedConsumer(IBus bus)
        {
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<IProcessOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocatorActivity", new Uri("rabbitmq://localhost/Allocator_execute"), new {OrderId = context.Message.OrderId});
            builder.AddActivity("PaymentClientValidationActivity", new Uri("rabbitmq://localhost/PaymentClientValidationActiviy_execute"), new {OrderId = context.Message.OrderId});
            
            var routingSlip = builder.Build();
            await _bus.Execute(routingSlip);
            
            await context.RespondAsync<OrderProcessedOk>(new
            {
                OrderId = context.Message.OrderId,
                Date = context.Message.Date,
                Mensaje = "Procesado ok"
            });
        }
    }
}