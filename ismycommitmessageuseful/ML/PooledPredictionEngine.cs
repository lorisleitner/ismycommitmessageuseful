using Microsoft.Extensions.ObjectPool;
using Microsoft.ML;
using System;
using System.IO;

namespace ismycommitmessageuseful.ML
{
    public class PooledPredictionEngine<TSample, TPrediction>
        : IPooledPredictionEngine<TSample, TPrediction>
        where TSample : class
        where TPrediction : class, new()
    {
        private class PooledPredictionEnginePolicy
            : IPooledObjectPolicy<PredictionEngine<TSample, TPrediction>>
        {
            private readonly MLContext _mlContext;
            private readonly ITransformer _model;

            public PooledPredictionEnginePolicy(MLContext mlContext, ITransformer model)
            {
                _mlContext = mlContext;
                _model = model;
            }

            public PredictionEngine<TSample, TPrediction> Create()
            {
                return _mlContext.Model.CreatePredictionEngine<TSample, TPrediction>(_model);
            }

            public bool Return(PredictionEngine<TSample, TPrediction> obj)
            {
                if (obj == null)
                    return false;
                return true;
            }
        }

        private readonly MLContext _mlContext;
        private readonly ObjectPool<PredictionEngine<TSample, TPrediction>> _predictionEnginePool;

        private PooledPredictionEngine(int maxObjectsRetained)
        {
            _mlContext = new MLContext();

            MaxObjectsRetained = maxObjectsRetained;
        }

        public PooledPredictionEngine(Stream model, int maxObjectsRetained)
            : this(maxObjectsRetained)
        {
            _mlContext.Model.Load(model, out _);

            _predictionEnginePool = CreatePredictionEnginePool();
        }

        public PooledPredictionEngine(ITransformer model, int maxObjectsRetained)
            : this(maxObjectsRetained)
        {
            Model = model;

            _predictionEnginePool = CreatePredictionEnginePool();
        }

        public ITransformer Model { get; }

        public int MaxObjectsRetained { get; private set; }

        public TPrediction Predict(TSample sample)
        {
            var engine = _predictionEnginePool.Get();

            try
            {
                return engine.Predict(sample);
            }
            finally
            {
                _predictionEnginePool.Return(engine);
            }
        }

        private ObjectPool<PredictionEngine<TSample, TPrediction>> CreatePredictionEnginePool()
        {
            var policy = new PooledPredictionEnginePolicy(_mlContext, Model);

            if (MaxObjectsRetained < 1)
                MaxObjectsRetained = Environment.ProcessorCount * 2;

            return new DefaultObjectPool<PredictionEngine<TSample, TPrediction>>(policy, MaxObjectsRetained);
        }
    }
}
