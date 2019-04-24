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
        private Task _task;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _task = ExecuteAsync(_cts.Token);

            if(_task.IsCompleted)
            {
                return _task;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_task == null)
                return;

            try
            {
                _cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
