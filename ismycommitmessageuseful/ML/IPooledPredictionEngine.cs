using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
