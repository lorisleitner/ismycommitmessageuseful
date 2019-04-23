using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.ML
{
    public interface IPooledPredictionEngine<TSample, TPrediction>
    {
        ITransformer Model { get; }

        int MaxObjectsRetained { get; }

        TPrediction Predict(TSample sample);
    }
}
