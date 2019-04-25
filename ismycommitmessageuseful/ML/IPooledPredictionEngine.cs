using Microsoft.ML;

namespace ismycommitmessageuseful.ML
{
    public interface IPooledPredictionEngine<TSample, TPrediction>
        where TSample : class
        where TPrediction : class, new()
    {
        ITransformer Model { get; }

        int MaxObjectsRetained { get; }

        TPrediction Predict(TSample sample);
    }
}
