using System;
using System.Threading.Tasks;
using Mt.Contracts;
using MassTransit;

namespace Consumer.App.Components
{
    public class OrderProcessedConsumer : IConsumer<IProcessOrder>
    {
        public async Task Consume(ConsumeContext<IProcessOrder> context)
        {
            await context.RespondAsync<OrderProcessedOk>(new
            {
                OrderId = context.Message.OrderId,
                Date = context.Message.Date,
                Mensaje = "Procesado ok" 
            });
        }
    }
}