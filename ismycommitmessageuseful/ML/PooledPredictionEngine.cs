using Microsoft.Extensions.ObjectPool;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly ITransformer _model;
        private readonly ObjectPool<PredictionEngine<TSample, TPrediction>> _predictionEnginePool;
        private int _maxObjectsRetained;

        public PooledPredictionEngine(Stream model, int maxObjectsRetained)
        {
            _mlContext = new MLContext();
            _mlContext.Model.Load(model, out _);

            _maxObjectsRetained = maxObjectsRetained;

            _predictionEnginePool = CreatePredictionEnginePool();
        }

        public ITransformer Model => _model;

        public int MaxObjectsRetained => _maxObjectsRetained;

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
            var policy = new PooledPredictionEnginePolicy(_mlContext, _model);

            if (_maxObjectsRetained < 1)
                _maxObjectsRetained = Environment.ProcessorCount * 2;

            return new DefaultObjectPool<PredictionEngine<TSample, TPrediction>>(policy, _maxObjectsRetained);
        }
    }
}
