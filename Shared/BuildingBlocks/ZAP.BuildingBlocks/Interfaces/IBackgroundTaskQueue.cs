using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZAP.BuildingBlocks.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItem(Func<CancellationToken, ValueTask> workItem);

        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
