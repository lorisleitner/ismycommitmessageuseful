using ismycommitmessageuseful.Database;
using ismycommitmessageuseful.ML;
using ismycommitmessageuseful.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Services
{
    public class UpdateModelService : TimedBackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMemoryCache _memoryCache;

        private readonly ILogger _logger;

        public UpdateModelService(IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache,
            ILogger<UpdateModelService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _memoryCache = memoryCache;

            _logger = logger;
        }

        public override TimeSpan Interval => TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing ML model now...");

            var mlContext = new MLContext();

            var data = await GetDataAsync().ConfigureAwait(false);

            _logger.LogInformation("Loaded {CommitCount} commits from database", data.Count());

            if(!data.Any())
            {
                _logger.LogError("Cancelling model generation because there is no training data available");
                return;
            }

            var trainingData = mlContext.Data.LoadFromEnumerable(data);

            var dataPipeline = mlContext.Transforms.CopyColumns("Label", "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText("MessageFeatures", new TextFeaturizingEstimator.Options
                {
                    CaseMode = TextNormalizingEstimator.CaseMode.Lower,
                    WordFeatureExtractor = new WordBagEstimator.Options { NgramLength = 2, UseAllLengths = true },
                    CharFeatureExtractor = new WordBagEstimator.Options { NgramLength = 3, UseAllLengths = false }
                }, "Message"))
                .Append(mlContext.Transforms.CopyColumns("Features", "MessageFeatures"))
                .Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features"))
                .AppendCacheCheckpoint(mlContext);

            var trainingPipeline = dataPipeline.Append(mlContext.Regression.Trainers.FastTree());

            var stopwatch = Stopwatch.StartNew();

            var model = trainingPipeline.Fit(trainingData);

            stopwatch.Stop();

            _logger.LogInformation("Trained model in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            var engine = new PooledPredictionEngine<CommitInput, CommitPrediction>(model, -1);
            _memoryCache.Set<IPooledPredictionEngine<CommitInput, CommitPrediction>>(CacheKeys.PredictionEngine, engine);

            _logger.LogInformation("Model generation finished");
        }

        private async Task<IEnumerable<CommitInput>> GetDataAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();

                var commits = await context
                    .Commits
                    .AsNoTracking()
                    .Where(x => x.UsefulCount + x.NotUsefulCount > 0)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var sampleData = new List<CommitInput>(commits.Count);

                foreach (var commit in commits)
                {
                    sampleData.Add(new CommitInput
                    {
                        Message = commit.Message,
                        Usefulness = (float)commit.UsefulCount / (commit.UsefulCount + commit.NotUsefulCount) * 100
                    });
                }

                return sampleData;
            }
        }
    }
}
