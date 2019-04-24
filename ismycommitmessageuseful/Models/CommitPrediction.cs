using Microsoft.ML.Data;

namespace ismycommitmessageuseful.Models
{
    public class CommitPrediction
    {
        [ColumnName("Score")]
        public float Usefulness { get; set; }
    }
}
