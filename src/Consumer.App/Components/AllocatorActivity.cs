using System.Threading.Tasks;
using MassTransit.Courier;

namespace Consumer.App.Components
{
    public class AllocatorActivity : IActivity<AllocatorMessage, AllocatorLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<AllocatorMessage> context)
        {
            await Task.Delay(200);
            return context.Completed(new {OrderId = context.Arguments.OrderId});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocatorLog> context)
        {
            await Task.Delay(200);
            return context.Compensated();
        }
    }

    public interface AllocatorMessage
    {
        int OrderId { get; }
    }

    public interface AllocatorLog
    {
        int OrderId { get; }
    }
}