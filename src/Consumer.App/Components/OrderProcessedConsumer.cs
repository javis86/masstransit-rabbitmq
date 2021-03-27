using System;
using System.Threading.Tasks;
using Mt.Contracts;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;

namespace Consumer.App.Components
{
    public class OrderProcessedConsumer : IConsumer<IProcessOrder>
    {
        private readonly IBus _bus;
        private readonly IRequestClient<RequestRouting> _requestClient;

        public OrderProcessedConsumer(IBus bus, IRequestClient<RequestRouting> requestClient)
        {
            _bus = bus;
            _requestClient = requestClient;
        }

        public async Task Consume(ConsumeContext<IProcessOrder> context)
        {
            Response<ResponseRouting> response = await _requestClient.GetResponse<ResponseRouting>(new RequestRouting() { OrderId = context.Message.OrderId});
            
            // Run routingslip without waiting for complete
            // var routingSlip = BuildRoutingSlip(context);
            // await _bus.Execute(routingSlip);
            
            await context.RespondAsync<OrderProcessedOk>(new
            {
                OrderId = context.Message.OrderId,
                Date = context.Message.Date,
                Mensaje = "Procesado ok"
            });
        }

        private static RoutingSlip BuildRoutingSlip(ConsumeContext<IProcessOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("Allocator", new Uri("rabbitmq://localhost/Allocator_execute"), new {OrderId = context.Message.OrderId});
            builder.AddActivity("PaymentClientValidation", new Uri("rabbitmq://localhost/PaymentClientValidation_execute"), new {OrderId = context.Message.OrderId});

            var routingSlip = builder.Build();
            return routingSlip;
        }
    }
}