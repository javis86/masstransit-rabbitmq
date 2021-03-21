using System.Threading.Tasks;
using MassTransit.Courier;

namespace Consumer.App.Components
{
    public class PaymentClientValidationActiviy : IActivity<ValidationMessage, ValidationLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<ValidationMessage> context)
        {
            await Task.Delay(200);

            return context.Arguments.OrderId < 100 ? context.Faulted() : context.Completed();
        }

        public async Task<CompensationResult> Compensate(CompensateContext<ValidationLog> context)
        {
            await Task.Delay(200);
            return context.Compensated();
        }
    }

    public interface ValidationMessage
    {
        int OrderId { get; }
    }
    
    public interface ValidationLog
    {
        int OrderId { get; }
    }
}