using Microsoft.ML.Data;

namespace ismycommitmessageuseful.Models
{
    public class CommitInput
    {
        [ColumnName("Message")]
        public string Message { get; set; }

        [ColumnName("Label")]
        public float Usefulness { get; set; }
    }
}
