using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Services
{
    public abstract class TimedBackgroundService : BackgroundService
    {
        private Timer _timer;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, Interval);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            try
            {
                Cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(Task.Delay(Timeout.Infinite, cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _timer.Dispose();
        }

        private void DoWork(object _)
        {
            ExecuteAsync(Cts.Token).Wait();
        }

        public abstract TimeSpan Interval { get; }
    }
}
