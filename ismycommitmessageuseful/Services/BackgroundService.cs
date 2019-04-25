using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Services
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        protected BackgroundService()
        {
            Cts = new CancellationTokenSource();
        }

        protected Task Task { get; set; }

        protected CancellationTokenSource Cts { get; }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            Task = ExecuteAsync(Cts.Token);

            if(Task.IsCompleted)
            {
                return Task;
            }

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Task == null)
                return;

            try
            {
                Cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(Task, Task.Delay(Timeout.Infinite, cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public virtual void Dispose()
        {
            Cts.Cancel();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
